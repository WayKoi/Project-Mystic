using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Card_Test.Base;
using Card_Test.Files;
using Card_Test.Items;
using Card_Test.Tables;
using Sorting;

namespace Card_Test {
	public class CardAI : Character {
		// this class is the AI that plays as the enemy
		private double RespondRate, TempResponse = 0; // how often they actually play on their turn
		private double Accuracy, TempAccuracy = 0; // how accurate their plays are
		public Drops Drop;
		public int MaxPlay;

		public int FringeSize = 6;

		private List<SimWrapper> Possible; 

		public CardAI(string name, int maxhealth, int maxmana, TDeck deck = null, Drops drop = null, int respondrate = 100, int accuracy = 100, int maxplay = 1) : base (name, maxhealth, maxmana, deck) {
			RespondRate = respondrate;
			Accuracy = accuracy;
			Drop = drop;
			MaxPlay = maxplay;
		}

		public CardAI(string name, int maxhealth, int maxmana, Deck deck = null, Drops drop = null, int respondrate = 100, int accuracy = 100, int maxplay = 1) : base(name, maxhealth, maxmana, deck) {
			RespondRate = respondrate;
			Accuracy = accuracy;
			Drop = drop;
			MaxPlay = maxplay;
		}

		public CardAI(AIEntry tab) : this (tab.Name, tab.MaxHealth, tab.MaxMana, Reader.ReadDeck(tab.Deck), tab.Drop, tab.RespondRate, tab.Accuracy, tab.MaxPlay) { }

		public void GenPlan(List<BattleChar> Targets, BattleChar Self) {
			if (Global.Rand.Next(0, 100) > RespondRate + TempResponse) {
				TempAccuracy += 5;
				TempResponse += 10;
				return;
			}

			Possible = new List<SimWrapper>();
			int handcount = Hand.Count;

			for (int i = 0; i < handcount; i++) {
				Possible.Add(new SimWrapper(Hand[i], i));
			}

			// Add logic for planning fusions and sides and multis here
			if (MultiCastSlots > 0) {
				for (int i = 0; i < handcount; i++) {
					Possible.Add(new SimWrapper(new MultiPlan(Hand[i]), i));
				}
			}

			if (SideCastCounters > 0) {
				for (int i = 0; i < handcount; i++) {
					Possible.Add(new SimWrapper(new SidePlan(Hand[i]), i));
				}
			}

			if (FusionCounters > 0) {
				for (int i = 0; i < handcount; i++) {
					for (int ii = i + 1; ii < handcount; ii++) {
						Possible.Add(new SimWrapper(new FusionPlan((new Card[] { Hand[i], Hand[ii] }).ToList()), new int[] { i, ii }));
					}
				}
			}

			FieldSim Point = new FieldSim(Targets, Self.Side);
			Point.Mana = Mana;
			Point.Side = SideCastCounters;
			Point.Multi = MultiCastSlots;
			Point.Fusion = FusionCounters;

			SimNode Root = new SimNode(Point, null);

			GenPlan(Root);

			// traverse the tree based on value
			SimNode travel = Root;
			int desccount = travel.Desc.Count;

			while (desccount > 0) {
				Sort.BubbleSort(travel.Desc, Compare.SimNode);

				for (int i = 0; i < desccount; i++) {
					int check = Global.Rand.Next(0, 100);
					if (check < Accuracy + TempAccuracy || i == desccount - 1) {
						travel = travel.Desc[i];
						break;
					}
				}

				desccount = travel.Desc.Count;
			}

			travel.GenPlan(this);

			TempAccuracy = 0;
			TempResponse = 0;
		}

		private void GenPlan(SimNode point) {
			if (point.Depth >= MaxPlay) { return; }
			int possibleCount = Possible.Count;
			List<SimNode> Choices = new List<SimNode>();

			for (int i = 0; i < possibleCount; i++) {
				if (!point.CheckUsed(Possible[i])) {
					SimNode test = ChooseTarget(point.Current, Possible[i]);

					PlanStep step = new PlanStep(test.Play.Plan, this, point.Current.Involved, test.Target);
					if (step.TestPlan(null)) {
						Choices.Add(test);
					}
				}
			}

			// add side, fusion and multicasting plans here
			if (Choices.Count == 0) { return; }
			Sort.BubbleSort(Choices, Compare.SimNode);

			// pull from the fringe
			int pull = 0;
			while (Choices.Count > 0 && pull < FringeSize) {
				if (Choices[0].Value <= 0) { break; }
				if (point.CanAddDesc(Choices[0])) {
					point.AddDescendant(Choices[0]);
					pull++;
				}

				Choices.RemoveAt(0);
			}

			for (int i = 0; i < point.Desc.Count; i++) {
				GenPlan(point.Desc[i]);
			}
		}

		private SimNode ChooseTarget(FieldSim sim, SimWrapper plan) {
			if (!plan.Plan.Targeting) { return new SimNode(sim, plan); }
			List<CharSim> Targs;

			if (plan.Plan.TargetType == 0) { // Anyone
				Targs = sim.Total;
			} else if (plan.Plan.TargetType == 1) { // Friendlies
				Targs = sim.Friends;
			} else { // Enemies
				Targs = sim.Enemies;
			}

			List<SimNode> Nodes = new List<SimNode>();
			int targetCount = Targs.Count;

			for (int i = 0; i < targetCount; i++) {
				if (Targs[i].Health > 0) {
					Nodes.Add(new SimNode(sim, plan, Targs[i].Index));
				}
			}

			if (Nodes.Count <= 0) { return new SimNode(sim, plan); }
			Sort.BubbleSort(Nodes, Compare.SimNode);
			
			return Nodes[0];
		}
	}

	public class SimNode {
		public List<SimNode> Desc = new List<SimNode>();
		public int Value = 0, Target = -1;
		public FieldSim Current;
		public SimWrapper Play;
		public bool Valid = true;
		public int Depth = 0;

		public List<int> Needed = new List<int>();

		private SimNode Link = null;

		public SimNode(SimNode parent, SimWrapper play, int target = -1) {
			Target = target;
			Play = play;
			Link = parent;

			Needed.AddRange(parent.Needed);
			if (play != null) { Needed.AddRange(play.Need); }

			Current = new FieldSim(Link.Current);
			if (Play == null) { return; }
			ValuePlay(Current.SimCard(Play.Plan, target));

			Depth = Link.Depth + 1;
		}

		public SimNode(FieldSim state, SimWrapper play, int target = -1) {
			Target = target;
			Play = play;
			Current = new FieldSim(state);

			if (play != null) {
				foreach (int i in play.Need) {
					Needed.Add(i);
				}
			}

			if (Play == null) { return; }
			ValuePlay(Current.SimCard(Play.Plan, target));
		}

		public void AddDescendant(SimWrapper play, int target = -1) {
			AddDescendant(new SimNode(this, play, target));
		}

		public void AddDescendant(SimNode node) {
			node.Link = this;
			node.Depth = Depth + 1;
			node.Needed.AddRange(Needed);
			Desc.Add(node);
		}

		public bool CanAddDesc(SimNode node) {
			node.Validate();
			return node.Valid;
		}

		public void Validate() {
			Valid = Current.Mana >= 0 && Current.Fusion >= 0 && Current.Side >= 0 && Current.Multi >= 0;
		}

		private void ValuePlay(SimReport report) {
			if (report == null) { return; }
			if (Target != -1 && Current.Total[Target].Health <= 0) { return; }

			bool Flip = false; // false means that damage is good
			if (Target != -1 && Current.Friends[0].Side == Current.Total[Target].Side) { Flip = true; }

			Value = 0;
			// negative effects
			Value += report.Damage * (Flip ? -1 : 1);
			Value += report.Defeated * 200 * (Flip ? -1 : 1);

			// positive effects
			double healMult = (Target != -1 ? 0.40 + (Current.Total[Target].Health / (double)Current.Total[Target].MaxHealth) : 1);
			Value += (int) (report.Healing * healMult) * (Flip ? 1 : -1);

			double shieldMult = (Target != -1) ? Math.Min(Current.Total[Target].MaxHealth - Current.Total[Target].Health, 60) : 30;
			Value += (int) (report.SGained * shieldMult) * (Flip ? 1 : -1);

			Value += report.Drawn * 50 * (Target == -1 || !Flip ? 1 : -1);

			// Neutral Effects
			Value += report.Summons * 100;
			Value = (int) (Value * Math.Max(report.TargetsAffected, 1));

			if (Value == 0) {	
				return;
			}
		}

		// checks if the play has been used in this branch already
		public bool CheckUsed(SimWrapper check) {
			bool ret = false;

			for (int i = 0; i < Needed.Count; i++) {
				if (check.Need.Contains(Needed[i])) {
					ret = true;
					break;
				}
			}

			return ret;
		}

		public void GenPlan(Character Caster) {
			if (Link != null) { Link.GenPlan(Caster); }
			if (Play != null) { Caster.Plan.PlanOrCast(null, Play.Plan, Current.Involved, Target); }
		}
	}

	public class FieldSim {
		public List<BattleChar> Involved;
		public List<CharSim> Friends = new List<CharSim>();
		public List<CharSim> Enemies = new List<CharSim>();
		public List<CharSim> Total = new List<CharSim>();

		private int TotalCount, FriendCount, EnemyCount;

		public int Mana = 0, Fusion = 0, Side = 0, Multi = 0;

		public FieldSim (List<BattleChar> involved, int FriendlySide) {
			Involved = involved;
			int invAmt = Involved.Count;
			for (int i = 0; i < invAmt; i++) {
				Character point = Involved[i].Unit;
				CharSim sim = new CharSim(i, point.Health, point.MaxHealth, Involved[i].Effect, Involved[i].Side, Involved[i].Shields.Count);
				
				if (Involved[i].Side == FriendlySide) {
					Friends.Add(sim);
				} else {
					Enemies.Add(sim);
				}

				Total.Add(sim);
			}

			TotalCount = Total.Count;
			FriendCount = Friends.Count;
			EnemyCount = Enemies.Count;
		}

		public FieldSim (FieldSim copy) {
			int friendSide = copy.Friends[0].Side;
			for (int i = 0; i < copy.TotalCount; i++) {
				CharSim sim = new CharSim(copy.Total[i]);

				if (copy.Total[i].Side == friendSide) {
					Friends.Add(sim);
				} else {
					Enemies.Add(sim);
				}

				Total.Add(sim);
			}

			Involved = copy.Involved;

			TotalCount = Total.Count;
			FriendCount = Friends.Count;
			EnemyCount = Enemies.Count;

			Mana = copy.Mana;
			Fusion = copy.Fusion;
			Side = copy.Side;
			Multi = copy.Multi;
		}

		public SimReport SimCard (Plannable plan, int target, bool Change = true) {
			if (plan == null) { return null; }
			SimReport report = new SimReport();

			Card card = plan.CardEquiv();

			if (Change) {
				Mana -= plan.ManaCost;
				Fusion -= plan.FusionCost;
				Side -= plan.SideCost;
				Multi -= plan.MultiSlots;
			}

			// summons are not interactable the turn that they are summoned, so there is no reason to add them to the sim
			if (card.Mod == Mods.Translate("summon")) {
				List<CardAI> lis = SummonTable.CreateSummon(card);
				report.Summons += lis.Count;
			}

			report.Drawn   += (card.Sub == SubMods.Translate("plusone")) ? 1 : 0;
			report.Drawn   += (card.Type == Types.Translate("time")) ? card.Tier : 0;
			report.SGained += (card.Type == Types.Translate("shield")) ? Math.Max(card.Tier, 1) : 0;

			if (target == -1) { return report; }

			int cardEffect = card.Element.GetEffect();

			report.Damage  = CalcAmount(Total[target], card.Damage, cardEffect);
			report.Defeated += report.Damage >= Total[target].Health && Total[target].Health != 0 ? 1 : 0;
			report.Healing = CalcAmount(Total[target], card.Healing, cardEffect, false);

			if (Change) { Total[target].HealthChange += report.Healing - report.Damage; Total[target].Effect = cardEffect; }

			report.TargetsAffected = 1;

			if (card.Mod == Mods.Translate("aoe")) {
				int targeting = Total[target].Side;
				for (int i = 0; i < TotalCount; i++) {
					if (Total[i].Side == targeting && i != target) {
						int tem = CalcAmount(Total[i], card.Damage, cardEffect) / 2;
						int temh = CalcAmount(Total[i], card.Healing, cardEffect, false) / 2;

						report.Damage   += tem;
						report.Defeated += tem >= Total[i].Health ? 1 : 0;

						report.Healing += temh;

						if (Change) { Total[i].Health += temh - tem; Total[i].Effect = cardEffect; }
						report.TargetsAffected++;
					}
				}
			} else if (card.Sub == SubMods.Translate("jumping")) {
				// I dont bother going through the reactions and checking overhealing / overkilling because the target is random
				// I also do not change the values of any target as the AI does not know which one it will hit
				// These values are entirely for hueristic purposes
				int amt = (card.Damage / 2);
				int targeting = Total[target].Side;

				for (int i = 0; i < TotalCount; i++) {
					if (Total[i].Side == targeting && i != target) {
						if (amt >= Total[i].Health) {
							report.Defeated++; // I assume that if it can jump to someone it can defeat it will
							break;
						}
					}
				}
				
				report.Damage += amt;
				report.Healing += (card.Healing / 2);
				report.TargetsAffected++;
			}

			return report;
		}

		private int CalcAmount (CharSim sim, int amt, int effect, bool damage = true) {
			int start = (int) (amt * Reactions.GetReaction(sim.Effect, effect).Mult);

			// we have to account for overhealing / overkilling
			if (damage) {
				start = Math.Min(start, sim.Health);
			} else { // healing
				start = Math.Min(start, sim.MaxHealth - sim.Health);
			}

			return start;
		}
	}

	public class CharSim {
		public int Index, Health, MaxHealth, Effect, Side, Shields, HealthChange = 0;

		public CharSim (int index, int health, int maxhp, int eff, int side, int shields, int healthchange = 0) {
			Index = index;
			Health = health;

			HealthChange = healthchange;
			Health += HealthChange;
			HealthChange = 0;

			MaxHealth = maxhp;
			Effect = eff;
			Side = side;
			Shields = shields;
		}

		public CharSim (CharSim copy) : this (copy.Index, copy.Health, copy.MaxHealth, copy.Effect, copy.Side, copy.Shields, copy.HealthChange) {  }
	}

	public class SimReport {
		public int Damage, Healing, Defeated = 0, TargetsAffected = 0, Summons = 0, Drawn = 0, SGained = 0;

		public SimReport (int damage = 0, int healing = 0) {
			Damage = damage;
			Healing = healing;
		}
	}

	public class SimWrapper {
		public Plannable Plan;
		public int[] Need;

		public SimWrapper(Plannable plan, int[] needed) {
			Plan = plan;
			Need = needed;
		}

		public SimWrapper(Plannable plan, int need) {
			Plan = plan;
			Need = new int[] { need };
		}
	}
}
