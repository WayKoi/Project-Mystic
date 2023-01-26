﻿using System;
using System.Collections.Generic;
using System.Text;
using Card_Test.Base;
using Card_Test.Files;
using Card_Test.Tables;
using Sorting;

namespace Card_Test {
	public class CardAI : Character {

		// simulate the field
		// give all cards in the hand a value, choose targets immediately
		// choose an amount of them to simulate and continue down that path
		// at each step re calc the values of the cards
		// repeat until out of possible plays (out of mana, no more cards, hit maxplay)
		// collect all branches of the trees
		// order by value
		// apply the accuarcy to them
		// choose top move left





		// this class is the AI that plays as the enemy
		private double RespondRate, TempResponse = 0; // how often they actually play on their turn
		private double Accuracy, TempAccuracy = 0; // how accurate their plays are
		public Drops Drop;
		public int MaxPlay;
		public int PreferredTarget = -1;

		public int FringeSize = 6;

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
			int handsize = Hand.Count;
			List<SimNode> Choices = new List<SimNode>();

			for (int i = 0; i < handsize; i++) {
				if (!point.CheckUsed(Hand[i])) {
					SimNode test = ChooseTarget(point.Current, Hand[i]);

					PlanStep step = new PlanStep(test.Play, this, point.Current.Involved, test.Target);
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

		private SimNode ChooseTarget(FieldSim sim, Plannable plan) {
			if (!plan.Targeting) { return new SimNode(sim, plan); }
			List<CharSim> Targs;

			if (plan.TargetType == 0) { // Anyone
				Targs = sim.Total;
			} else if (plan.TargetType == 1) { // Friendlies
				Targs = sim.Friends;
			} else { // Enemies
				Targs = sim.Enemies;
			}

			List<SimNode> Nodes = new List<SimNode>();
			int targetCount = Targs.Count;

			for (int i = 0; i < targetCount; i++) {
				Nodes.Add(new SimNode(sim, plan, Targs[i].Index));
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
		public Plannable Play;
		public bool Valid = true;
		public int Depth = 0;

		private SimNode Link = null;

		public SimNode(SimNode parent, Plannable play, int target = -1) {
			Target = target;
			Play = play;
			Link = parent;
			Current = new FieldSim(Link.Current);
			if (Play == null) { return; }
			ValuePlay(Current.SimCard(Play, target));

			Depth = Link.Depth + 1;
		}

		public SimNode(FieldSim state, Plannable play, int target = -1) {
			Target = target;
			Play = play;
			Current = new FieldSim(state);
			if (Play == null) { return; }
			ValuePlay(Current.SimCard(Play, target));
		}

		public void AddDescendant(Plannable play, int target = -1) {
			AddDescendant(new SimNode(this, play, target));
		}

		public void AddDescendant(SimNode node) {
			node.Link = this;
			node.Depth = Depth + 1;
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

			bool Flip = false; // false means that damage is good
			if (Target != -1 && Current.Friends[0].Side == Current.Total[Target].Side) { Flip = true; }

			Value = 0;
			// negative effects
			Value += report.Damage * (Flip ? -1 : 1);
			Value += report.Defeated * 200 * (Flip ? -1 : 1);

			// positive effects
			Value += report.Healing * (Flip ? 1 : -1);
			Value += report.Drawn * 50 * (Target == -1 || !Flip ? 1 : -1);
			Value += report.SGained * 40 * (Flip ? 1 : -1);

			// Neutral Effects
			Value += report.Summons * 100;
			Value = (int) (Value * Math.Max(report.TargetsAffected, 1));
		}

		// checks if the play has been used in this branch already
		public bool CheckUsed(Plannable check) {
			if (Play == check) { return true; }
			if (Link == null) { return false; }
			return Link.CheckUsed(check);
		}

		public void GenPlan(Character Caster) {
			if (Link != null) { Link.GenPlan(Caster); }
			if (Play != null) { Caster.Plan.PlanOrCast(null, Play, Current.Involved, Target); }
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
			report.Defeated += report.Damage >= Total[target].Health ? 1 : 0;
			report.Healing = CalcAmount(Total[target], card.Healing, cardEffect, false);

			if (Change) { Total[target].Health += report.Healing - report.Damage; Total[target].Effect = cardEffect; }

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
		public int Index, Health, MaxHealth, Effect, Side, Shields;

		public CharSim (int index, int health, int maxhp, int eff, int side, int shields) {
			Index = index;
			Health = health;
			MaxHealth = maxhp;
			Effect = eff;
			Side = side;
			Shields = shields;
		}

		public CharSim (CharSim copy) : this (copy.Index, copy.Health, copy.MaxHealth, copy.Effect, copy.Side, copy.Shields) { }
	}

	public class SimReport {
		public int Damage, Healing, Defeated = 0, TargetsAffected = 0, Summons = 0, Drawn = 0, SGained = 0;

		public SimReport (int damage = 0, int healing = 0) {
			Damage = damage;
			Healing = healing;
		}
	}
}
