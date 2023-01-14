using System;
using System.Collections.Generic;
using System.Text;
using Card_Test.Files;
using Card_Test.Tables;
using Sorting;

namespace Card_Test {
	public class CardAI : Character {
		// this class is the AI that plays as the enemy
		private double RespondRate; // how often they actually play on their turn
		private double Accuracy, TempAccuracy = 0; // how accurate their plays are
		public Drops Drop;
		public int MaxPlay;
		public int PreferredTarget = -1;

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

		public void GenPlan (List<BattleChar> Targets, BattleChar Self) {
			// when we dont respond, accuracy goes up
			if (Rand.Next(1, 101) > RespondRate) { TempAccuracy += 10; return; }
			if (Hand.Count == 0) { return; }
			if (!HasHealth()) { return; }
			// list of all possible moves
			// remove impossible moves
			// give moves a value
			// base on accuracy, choose a move

			List<int> Friendly = new List<int>();
			List<int> Enemy    = new List<int>();
			int SelfInd = 0;

			for (int i = 0; i < Targets.Count; i++) {
				if (Targets[i].Side == Self.Side) {
					Friendly.Add(i);
				} else {
					Enemy.Add(i);
				}

				if (Targets[i] == Self) {
					SelfInd = i;
				}
			}

			List<Move> moves = new List<Move>();

			// first sweep gets all possible card plays
			Sort.BubbleSort(Hand, Compare.Card);

			int upper = (int) Math.Pow(2, Hand.Count);
			for (int i = 1; i < upper; i++ ) {
				Move move = new Move();
				int count = Hand.Count;
				int val = i;
				while (count >= 0) {
					if (val >= (int) Math.Pow(2, count)) {
						val -= (int)Math.Pow(2, count);
						move.Cards.Add(count);
					}
					count--;
				}

				moves.Add(move);
			}

			// sweep in the other direction as well
			Sort.BubbleSort(Hand, Compare.InverseCard);

			upper = (int)Math.Pow(2, Hand.Count);
			for (int i = 1; i < upper; i++) {
				Move move = new Move();
				int count = Hand.Count;
				int val = i;
				while (count >= 0) {
					if (val >= (int)Math.Pow(2, count)) {
						val -= (int)Math.Pow(2, count);
						move.Cards.Add(count);
					}
					count--;
				}

				moves.Add(move);
			}

			// second sweep removes ones that cost to much or are impossible

			upper = moves.Count;
			for (int i = 0; i < upper; i++) {
				int cardamt = 0;
				int subupper = moves[i].Cards.Count;
				for (int ii = 0; ii < subupper; ii++) {
					moves[i].cost += Hand[moves[i].Cards[ii]].Tier;
					cardamt++;
				}

				if (moves[i].cost > Mana || cardamt > MaxPlay) {
					moves.RemoveAt(i);
					i--;
					upper--;
				}
			}

			if (moves.Count == 0) { return; }
			// third sweep adds targets

			upper = moves.Count;
			for (int i = 0; i < upper; i++) {
				int subupper = moves[i].Cards.Count;
				for (int ii = 0; ii < subupper; ii++) {
					Card card = Hand[moves[i].Cards[ii]];
					int damage = (int)(Types.Search(card.Type).GetDamage(card.Tier) * GetAffinity(card.LookupType()));
					int healing = (int)(Types.Search(card.Type).GetHealing(card.Tier) * GetAffinity(card.LookupType()));

					if (damage > 0) {
						// target enemy
						List<PossibleTarg> targs = new List<PossibleTarg>();
						for (int iii = 0; iii < Enemy.Count; iii++) {
							if (Targets[Enemy[iii]].Unit.HasHealth()) {
								int curEff = 0;
								int subsubupper = moves[i].Targets.Count;
								for (int iv = 0; iv < subsubupper; iv++) {
									if (moves[i].Targets[iv] == Enemy[iii] && Targets[Enemy[iii]].Unit.HasHealth()) {
										curEff = moves[i].TargetEffects[iv];
									}
								}

								targs.Add(new PossibleTarg((int) (damage * Reactions.GetReaction(curEff, Types.Search(card.Type).GetEffect()).Mult), Enemy[iii]));
							}
						}

						// find the best enemy
						for (int iii = 0; iii < targs.Count; iii++) {
							double percHealth = ((double) (Targets[targs[iii].Target].Unit.Health - targs[iii].Damage) / (double) (Targets[targs[iii].Target].Unit.MaxHealth));
							int value = targs[iii].Damage;

							if (percHealth <= 0) {	
								value *= 3;
							} else {
								value = (int)(value * (1 + (1 - percHealth)));
							}

							if (targs[iii].Target == PreferredTarget) {
								value *= 10;
							}

							targs[iii].Value = value;
						}

						Sort.BubbleSort(targs, Compare.PossibleTarg);
						if (targs.Count == 0) { return; }

						List<PossibleTarg> choosefrom = new List<PossibleTarg>();
						for (int iii = 0; iii < targs.Count; iii++) {
							if (iii > 0 && targs[iii].Value != targs[iii - 1].Value) {
								break;
							}
							choosefrom.Add(targs[iii]);
						}

						int chosenTarg = Global.Rand.Next(0, choosefrom.Count);
						moves[i].Targets.Add(choosefrom[chosenTarg].Target);
						moves[i].TargetEffects.Add(Types.Search(card.Type).GetEffect());
						moves[i].HealMult.Add(0);
						damage = choosefrom[chosenTarg].Damage;
					} else if (healing > 0) {
						// target friendly
						int mostHealed = 0;
						int healedAmt = 0;
						int healMult = 0;

						for (int iii = 0; iii < Friendly.Count; iii++) {
							if (Targets[Friendly[iii]].Unit.HasHealth()) {
								double percFull = (double) Targets[Friendly[iii]].Unit.Health / (double) Targets[Friendly[iii]].Unit.MaxHealth;
								int amt = Math.Min(Targets[Friendly[iii]].Unit.MaxHealth, Targets[Friendly[iii]].Unit.Health + healing) - Targets[Friendly[iii]].Unit.Health;
								if (amt > healedAmt) {
									mostHealed = iii;
									healedAmt = amt;
									healMult = (int) (1 / (percFull));
								}
							}
						}

						moves[i].Targets.Add(Friendly[mostHealed]);
						moves[i].TargetEffects.Add(Types.Search(card.Type).GetEffect());
						moves[i].HealMult.Add(healMult);
						healing = healedAmt;
					} else {
						// some special effect is happening, like a shield
						// hard code those cases
						if (card.Type == 2) {	
							// its a shield
							moves[i].shields++;

							int bestTarg = 0;
							int highHealth = 0;
							foreach (int targ in Friendly) {
								if (highHealth < Targets[targ].Unit.Health) {
									highHealth = Targets[targ].Unit.Health;
									bestTarg = targ;
								}
							}

							moves[i].Targets.Add(bestTarg);
							moves[i].TargetEffects.Add(0);
							moves[i].HealMult.Add(0);
						} else {
							// its something that the AI cannot play
							moves.RemoveAt(i);
							i--;
							upper--;
							break;
						}
					}

					moves[i].Damage.Add(damage);
					moves[i].Healing.Add(healing);
					moves[i].damageTotal += damage;
					moves[i].healingTotal += healing;
				}
			}

			// fourth sweep gives values to the moves

			upper = moves.Count;
			for (int i = 0; i < upper; i++) {
				// moves[i].value
				for (int ii = 0; ii < moves[i].Damage.Count; ii++) {
					moves[i].value += moves[i].Damage[ii];
					moves[i].value += moves[i].Healing[ii] * moves[i].HealMult[ii];
					moves[i].value += ((moves[i].shields) * (Targets[moves[i].Targets[ii]].Unit.Health / 4));
				}
				
				moves[i].value *= (1 + (moves[i].Targets.Count / Targets.Count));
				moves[i].value /= (Mana - moves[i].cost) + 1;
			}	

			// sort the moves by value

			Sort.QuickSort(moves, 0, moves.Count - 1, Compare.Move);

			// remove top moves until the amount left is equal to one or the list contains accuracy % of original moves
			
			int origsize = moves.Count;
			double acc = moves.Count / (double) origsize;
			while (acc * 100 > Accuracy + TempAccuracy && moves.Count > 1) {
				moves.RemoveAt(0);
				acc = moves.Count / (double) origsize;
			}

			// plan one of the top moves

			if (moves.Count > 0) {
				Move chosen = moves[0];
				TempAccuracy = 0;
			
			
				/*TextUI.PrintFormatted(Deck);
				TextUI.PrintFormatted(Hand);*/

				// make sure that the plan does something
				if (chosen.damageTotal != 0 || chosen.healingTotal != 0 || chosen.shields != 0) {
					// sort the cards in the plan, so that when they are removed the other indexes don't get messed up
					bool sorted = false;
					while (!sorted) {
						sorted = true;
						for (int i = 0; i < chosen.Cards.Count - 1; i++) {
							if (chosen.Cards[i] < chosen.Cards[i + 1]) {
								Sort.Swap(chosen.Cards, i, i + 1);
								Sort.Swap(chosen.Targets, i, i + 1);
							}
						}
					}

					for (int i = 0; i < chosen.Cards.Count; i++) {
						// new int[] { chosen.Cards[i] + 1, chosen.Targets[i] }
						Plan.PlanOrCast(null, Hand[chosen.Cards[i]], Targets, chosen.Targets[i]);
					}
				}
			}
		}
	}

	public class Move {
		public List<int> Cards = new List<int>();
		public List<int> Targets = new List<int>();
		public List<int> TargetEffects = new List<int>();
		public List<int> Damage = new List<int>();
		public List<int> Healing = new List<int>();
		public List<int> HealMult = new List<int>();
		public int value = 0, cost = 0, shields = 0, damageTotal = 0, healingTotal = 0;
	}

	public class PossibleTarg {
		public int Damage, Target, Value;

		public PossibleTarg (int damage, int target, int value = 0) {
			Damage = damage;
			Target = target;
			Value = value;
		}
	}
}
