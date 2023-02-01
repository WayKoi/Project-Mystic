using Card_Test.Base;
using Card_Test.Items;
using Card_Test.Tables;
using Card_Test.Utilities;
using Sorting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Card_Test {
	public class Battle {
		private Character Player;
		private List<BattleChar> Involved = new List<BattleChar>();

		private MenuItem[] BattleMenu;

		private static bool PlayPlan = false;

		public Battle (Character[] players, Character[] enemies) {
			Player = players[0];
			Player.PlanVisible = true;
			
			foreach (Character unit in players) {
				Involved.Add(new BattleChar(unit, 0));
			}

			foreach (Character unit in enemies) {
				Involved.Add(new BattleChar(unit, 1));
			}

			/*Left.AddRange(players);
			Right.AddRange(enemies);*/
			// new MenuItem(new string[] { "Target", "T" }, TellTarget, TextUI.Parse, "tell a friendly party member who to target\n  Target (who you are telling) (who they are targeting)"),
			
			BattleMenu = new MenuItem[] {
				new MenuItem(new string[] { "Plan", "P" }, PlayerAddPlan, TextUI.Parse, "plan or cast a move\n  Plan (Card to play) (Target)"),
				new MenuItem(new string[] { "Fuse", "F" }, FusionPlan, TextUI.Parse, "Fuse Cards together, costs Fusion counters\n  Fuse (Cards) (Target)"),
				new MenuItem(new string[] { "Side", "Si" }, SidePlan, TextUI.Parse, "Side cast a card, costs a side cast counter and half the mana of the card\n  Side (Card) (Target)"),
				new MenuItem(new string[] { "Multi", "M" }, MultiPlan, TextUI.Parse, "Multi cast a card, Needs an open slot\n  Multi (Card) (Target)"),
				new MenuItem(new string[] { "Remove", "R" }, PlayerRemovePlan, TextUI.Parse, "remove part of the plan\n  r (plan to remove)"),
				new MenuItem(new string[] { "Stone", "S" }, Stone, TextUI.Parse, "use a stones effect\n Stone (Stone #) (Target optional)"),
				new MenuItem(new string[] { "Stone?", "S?" }, StoneInfo, TextUI.Parse, "look at a stones info\n S? (Stone #)" ),
				new MenuItem(new string[] { "Clear", "Cl" }, PlayerClearPlan, TextUI.Parse, "clear the current plan"),
				new MenuItem(new string[] { "Execute", "E" }, PlayPlanMenu, TextUI.Parse, "execute the current plan and play out the turn")
			};
		}

		public bool Run () {
			// retrurns true if the player won the battle, false otherwise
			// reset the decks

			if (Global.Run.Player != null) {
				for (int i = 0; i < Global.Run.Player.Stones.Count; i++) {
					Global.Run.Player.Stones[i].Effect.Refresh();
				}
			}

			for (int i = 0; i < Involved.Count; i++) {
				Involved[i].Unit.Plan.ResetPlan();
				Involved[i].Unit.MultiCastSlots = Involved[i].Unit.MaxMulti;
			}

			PlayReport StatusReport;

			// start the battle
			bool battle = true;
			while (battle) {
				Sort.BubbleSort(Involved, Compare.BattleChar);
				// start of turn effects
				for (int i = 0; i < Involved.Count; i++) {
					if (Involved[i].Unit.HasHealth()) {
						Involved[i].Unit.DrawCard();             // draw card
						Involved[i].Unit.Mana = Math.Max(Involved[i].Unit.MaxMana - Involved[i].Overload, 0); // refresh mana
						// Involved[i].Unit.Plan.ClearPlan();            // clear the plan

						Involved[i].Unit.FusionCounters = Involved[i].Unit.MaxFusion; // refresh counters
						Involved[i].Unit.SideCastCounters = Involved[i].Unit.MaxSide; // refresh counters

						if (Involved[i].Unit is CardAI) {        // generate a plan if they are an AI
							(Involved[i].Unit as CardAI).GenPlan(Involved, Involved[i]);
							// (Involved[i].Unit as CardAI).PreferredTarget = -1;
						}

						Involved[i].Effect = 0; // this resets the current effect on the player,
						Involved[i].Overload = 0; // reset the overload for the next turn
					}
				}

				

				if (Global.Run.Player != null) {
					for (int i = 0; i < Global.Run.Player.Stones.Count; i++) {
						Global.Run.Player.Stones[i].Effect.TurnRefresh();
					}
				}

				PlayPlan = false;
				while (!PlayPlan) {
					// print the scene
					PrintScene();
					if (Player.HasHealth()) {
						TextUI.Prompt("What do you want to do?", BattleMenu);
					} else {
						PlayPlan = true;
					}
				}

				// execute the plan

				bool left, right;
				string lastAttack = "";

				if (Global.Run.Player != null) {
					for (int i = 0; i < Global.Run.Player.Stones.Count; i++) {
						Stone check = Global.Run.Player.Stones[i];
						if (check.Effect.IsPassive && !check.Effect.EndofTurn) {
							PlayReport report = new PlayReport();
							Player.Plan.Cast(report, Global.Run.Player.Stones[i], Involved);
							report.PrintReport();
							// check.RunEffect(Involved, -1);
						}
					}
				}

				StatusReport = new PlayReport();

				for (int i = 0; i < Involved.Count; i++) {
					if (Involved[i].Unit.HasHealth()) {
						Involved[i].RunStatusStart(StatusReport);
					}
				}

				StatusReport.PrintReport();

				for (int i = 0; i < Involved.Count; i++) {
					left = SideAlive(0);
					right = SideAlive(1);

					if (Involved[i].Unit.HasHealth() && left && right) {
						Involved[i].Unit.Plan.ExecutePlan();
						lastAttack = Involved[i].Unit.Name;
					}
				}

				StatusReport = new PlayReport();

				for (int i = 0; i < Involved.Count; i++) {
					if (Involved[i].Unit.HasHealth()) {
						Involved[i].RunStatusEnd(StatusReport);
					}
				}

				StatusReport.PrintReport();

				left = SideAlive(0);
				right = SideAlive(1);

				// check if battle is over

				if (!left) {
					// players are all dead
					TextUI.PrintFormatted("You were bested by " + lastAttack);
					TextUI.PrintFormatted("Game Over");
					Console.ReadLine();
					return false;
				} else if (!right) {
					// enemies are all dead
					if (Global.Run.Player != null) {
						for (int i = 0; i < Involved.Count; i++) {
							if (!Involved[i].Unit.HasHealth() && Involved[i].Unit is CardAI) {
								DropTable.Drop(Global.Run.Player, Involved[i].Unit, (Involved[i].Unit as CardAI).Drop);
							}
						}
					}

					Console.WriteLine();
					TextUI.PrintFormatted("Congratulations you've won!");

					if (Player.Health <= 0) {
						TextUI.PrintFormatted(Player.Name + " is revived with 1 health");
						Player.Health = 1;
					}

					TextUI.Wait();

					for (int i = 0; i < Involved.Count; i++) {
						if (!Involved[i].Unit.HasHealth() && Involved[i].Unit is CardAI && (Involved[i].Unit as CardAI).Drop != null) {
							(Involved[i].Unit as CardAI).Drop.RunAction();
						}
					}

					return true;
				}

				if (battle) {
					if (Global.Run.Player != null) {
						for (int i = 0; i < Global.Run.Player.Stones.Count; i++) {
							Stone check = Global.Run.Player.Stones[i];
							if (check.Effect.IsPassive && check.Effect.EndofTurn) {
								PlayReport report = new PlayReport();
								Player.Plan.Cast(report, Global.Run.Player.Stones[i], Involved);
								report.PrintReport();
								// check.RunEffect(Involved, -1);
							}
						}
					}

					TextUI.PrintFormatted("\nPress anything to continue the battle");
					Console.ReadLine();
				}
			}

			return false;
		}

		public bool TellTarget (int[] data) {
			if (data.Length	!= 2) { return false; }
			if (data[0] < 0 || data[0] >= Involved.Count || data[1] < 0 || data[1] >= Involved.Count) { return false; }
			if (Involved[data[0]].Side != 0) {
				TextUI.PrintFormatted("You cannot boss " + Involved[data[0]].Unit.Name + " around, they are not friendly");
				return false;
			}

			if (!(Involved[data[0]].Unit is CardAI)) {
				TextUI.PrintFormatted("You cannot boss " + Involved[data[0]].Unit.Name + " around");
				return false;
			}

			if (!Involved[data[0]].Unit.HasHealth() || !Involved[data[1]].Unit.HasHealth()) {
				TextUI.PrintFormatted("That target is not alive");
			}

			TextUI.PrintFormatted("You tell " + Involved[data[0]].Unit.Name + " to target " + Involved[data[1]].Unit.Name);

			(Involved[data[0]].Unit as CardAI).Plan.ClearPlan();
			// (Involved[data[0]].Unit as CardAI).PreferredTarget = data[1];
			(Involved[data[0]].Unit as CardAI).GenPlan(Involved, Involved[data[0]]);

			TextUI.Wait();

			return true;
		}

		public bool SideAlive (int side) {
			bool alive = false;

			foreach (BattleChar unit in Involved) {
				if (unit.Side == side && unit.Unit.HasHealth()) {
					alive = true;
					break;
				}
			}

			return alive;
		}

		public bool Stone (int[] data) {
			if (data == null || data.Length < 1) { return false; }
			if (Global.Run.Player == null) { return false; }
			if (data[0] < 1 || data[0] > Global.Run.Player.Stones.Count) { return false; }
			data[0]--;

			int plansize = Player.Plan.PlanSize();
			PlayReport report = new PlayReport();
			bool check = Player.Plan.PlanOrCast(report, Global.Run.Player.Stones[data[0]], Involved, (data.Length > 1 ? data[1] : -1));
			report.PrintReport();

			if ((!check && plansize == Player.Plan.PlanSize()) || Global.Run.Player.Stones[data[0]].Instant) {
				TextUI.Wait();
			}

			return true;
		}

		public bool StoneInfo (int[] data) {
			if (data == null || data.Length < 1) { return false; }
			if (Global.Run.Player == null) { return false; }
			if (data[0] < 1 || data[0] > Global.Run.Player.Stones.Count) { return false; }
			data[0]--;

			Console.Clear();
			TextUI.PrintFormatted(Global.Run.Player.Stones[data[0]].GetInfo());
			TextUI.Wait();

			return true;
		}

		public bool PlayerAddPlan(int[] data) {
			if (data.Length < 1) { return false; }
			if (data[0] < 1 || data[0] > Player.Hand.Count) { return false; }
			data[0]--;

			PlayReport report = new PlayReport();
			Plannable toplan = Player.Hand[data[0]];
			int plansize = Player.Plan.PlanSize();

			bool check = Player.Plan.PlanOrCast(report, toplan, Involved, (data.Length >= 2 ? data[1] : -1));
			report.PrintReport();

			if (check && plansize == Player.Plan.PlanSize()) {
				TextUI.Wait();
			}

			return check;
		}

		public bool SidePlan(int[] data) {
			if (data.Length < 1) { return false; }
			if (data[0] < 1 || data[0] > Player.Hand.Count) { return false; }
			data[0]--;

			PlayReport report = new PlayReport();
			Plannable toplan = new SidePlan(Player.Hand[data[0]]);
			int plansize = Player.Plan.PlanSize();

			bool check = Player.Plan.PlanOrCast(report, toplan, Involved, (data.Length >= 2 ? data[1] : -1));
			report.PrintReport();

			if (check && plansize == Player.Plan.PlanSize()) {
				TextUI.Wait();
			}

			return check;
		}

		public bool MultiPlan(int[] data) {
			if (data.Length < 1) { return false; }
			if (data[0] < 1 || data[0] > Player.Hand.Count) { return false; }
			data[0]--;

			PlayReport report = new PlayReport();
			Plannable toplan = new MultiPlan(Player.Hand[data[0]]);
			int plansize = Player.Plan.PlanSize();

			bool check = Player.Plan.PlanOrCast(report, toplan, Involved);
			report.PrintReport();

			if (check && plansize == Player.Plan.PlanSize()) {
				TextUI.Wait();
			}

			return check;
		}

		public bool FusionPlan(int[] data) {
			if (data.Length < 3) { return false; }
			if (data[data.Length - 1] < 0 || data[data.Length - 1] >= Involved.Count) { return false; }

			int target = data[data.Length - 1];
			List<Card> Fuse = new List<Card>();

			for (int i = 0; i < data.Length - 1; i++) {
				if (data[i] < 1 || data[i] > Player.Hand.Count) { return false; }
				if (Fuse.Contains(Player.Hand[data[i] - 1])) { return false; }
				Fuse.Add(Player.Hand[data[i] - 1]);
			}

			PlayReport report = new PlayReport();
			Plannable toplan = new FusionPlan(Fuse);
			int plansize = Player.Plan.PlanSize();

			bool check = Player.Plan.PlanOrCast(report, toplan, Involved, target);
			report.PrintReport();

			if (check && plansize == Player.Plan.PlanSize()) {
				TextUI.Wait();
			}

			return check;
		}

		public bool PlayerRemovePlan(int[] data) {
			if (data.Length < 1) { return false; }
			bool check = Player.Plan.RemoveFromPlan(data[0] - 1);
			if (!check) { TextUI.PrintFormatted("Unable to remove that from the plan"); }
			return check;
		}

		public bool PlayerClearPlan (int[] dum) {
			Player.Plan.ClearPlan();
			return true;
		}

		public bool PlayPlanMenu (int[] dum) {
			PlayPlan = true;
			return true;
		}

		public void PrintScene() {
			Console.Clear();
			int count = 1;

			if (Global.Run.Player != null && Global.Run.Player.Stones.Count > 0) {
				string build = "";
				foreach (Stone stone in Global.Run.Player.Stones) {
					build += "(" + count + ") " + Global.Run.Player.Stones[count - 1].Symbol + "  ";
					count++;
				}

				TextUI.PrintFormatted("Stones : " + build + "\n");
			}

			List<string> printout = new List<string>();

			string left = "", right = "";

			List<int> leftchars = BattleUtil.GetFromSide(0, Involved);
			List<int> rightchars = BattleUtil.GetFromSide(1, Involved);

			count = 0;
			foreach (BattleChar unit in Involved) {
				string statuses = "";

				foreach (Status stat in unit.Statuses) {	
					statuses += " (" + stat.Link.Name + ")";
				}

				List<string> leftPlan = new List<string>();
				
				foreach (int num in leftchars) {
					string get = Involved[num].Unit.Plan.PlanOnTarget(count);

					if (Involved[num].Unit.PlanVisible && get != string.Empty) {
						leftPlan.Add(get);
					}
				}
				
				List<string> rightPlan = new List<string>();

				foreach (int num in rightchars) {
					string get = Involved[num].Unit.Plan.PlanOnTarget(count);

					if (Involved[num].Unit.PlanVisible && get != string.Empty) {
						rightPlan.Add(get);
					}
				}

				string leftA = "", rightA = "";

				if (leftPlan.Count != 0) {
					leftPlan.Add("\n\n\n-> ");
					leftA = String.Join('\n', TextUI.MakeTable(leftPlan, 1));
				}

				if (rightPlan.Count != 0) {
					rightPlan.Insert(0, "\n\n\n <-");
					rightA = String.Join('\n', TextUI.MakeTable(rightPlan, 1));
				}

				string add = "(" + count + ") " + unit.Unit.ToString() + statuses + " " + new string('}', unit.Shields.Count);
				string append = add;

				if (leftA != "" || rightA != "") {
					append = String.Join('\n', TextUI.MakeTable((new string[] { leftA, "\n\n\n" + add, rightA }).ToList(), 0));
				}

				if (unit.Side == 0) {
					left += (left != "" ? "\n" : "") + append;
				} else {
					right += (right != "" ? "\n" : "") + append;
				}

				count++;
			}

			printout.Add(left);
			printout.Add(right);

			TextUI.PrintFormatted(String.Join('\n', TextUI.MakeTable(printout)));

			Console.WriteLine();
			TextUI.PrintFormatted("-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-");
			TextUI.PrintFormatted(Player.ManaToString() + (Player.FusionsToString().Length > 0 ? "\n" + Player.FusionsToString() : "") + (Player.SidesToString().Length > 0 ? "\n" + Player.SidesToString() : ""));
			TextUI.PrintFormatted("Current Hand");
			TextUI.PrintFormatted(Player.HandToString());

			string notarg = Player.Plan.PlanOnTarget(-1);

			if (notarg != string.Empty || Player.MultiCastSlots != 0) {
				TextUI.PrintFormatted("Current Plan");
				TextUI.PrintFormatted(Player.MultiToString());
				TextUI.PrintFormatted(notarg);
			}

			TextUI.PrintFormatted("-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-");
		}
	}

	public class BattleChar {
		public Character Unit;
		public int Side, Effect = 0, Overload = 0;
		public List<Shield> Shields = new List<Shield>();
		public List<Status> Statuses = new List<Status>();

		public BattleChar (Character unit, int side) {
			Unit = unit;
			Side = side;

			Unit.RefreshDeck();
			Unit.DrawCard(Unit.Cards.StartHandSize);
		}

		public bool HasStatus (TStatus test) {
			for (int i = 0; i < Statuses.Count; i++) {
				if (Statuses[i].Link == test) {
					return true;
				}
			}
			return false;
		}

		public int TakeDamage (Character Caster, int amount, int type, PlayReport report, bool bustShields = true) {
			if (amount == 0 || !Unit.HasHealth()) { return 0; }
			double mitigator = 0;
			int broken = 0;

			// report.Affected.Add(this);
			// damage, healing, shields broken, shields added, reaction, effect, damage blocked
			int[] reportInt = { 0, 0, 0, 0, 0, 0, 0 };

			// reaction damage
			int effect = Types.Search(type).GetEffect();
			Reaction react = Reactions.GetReaction(Effect, effect);
			reportInt[4] = Reactions.GetReactionIndex(react);

			double reactmult = Caster == null ? react.Mult : Caster.GetReactionMult(react);

			int reactDamage = (int)(amount * reactmult);

			if (bustShields) {
				while (Shields.Count > 0 && mitigator < 1) {
					mitigator += Shields[Shields.Count - 1].Mitigation;
					broken++;
					Shields.RemoveAt(Shields.Count - 1);
				}
			}

			reportInt[2] = broken;

			int damage = (int) Math.Max(reactDamage * (1 - mitigator), 0);
			reportInt[6] = reactDamage - damage;

			if (mitigator < 1) {
				reportInt[5] = effect;

				Effect = effect;

				if (!Effects.Table[effect].Stays) {
					Effect = 0;
				}

				reportInt[0] = Unit.TakeDamage(damage, type);
			}

			report.Steps.Add(new ReportStep(this, reportInt[0], 0, reportInt[2], reportInt[3], reportInt[4], reportInt[5], reportInt[6]));
			// report.AffectedEffects.Add(reportInt);

			return reportInt[0];
		}

		public int Heal (int amount, int type, PlayReport report) {
			if (amount == 0) { return 0; }
			
			// damage, healing, shields broken, shields added, reaction, effect, damage blocked
			int[] reportInt = { 0, 0, 0, 0, 0, 0, 0 };

			Reaction react = Reactions.GetReaction(Effect, Types.Search(type).GetEffect());
			reportInt[4] = Reactions.GetReactionIndex(react);

			reportInt[5] = Types.Search(type).GetEffect();
			Effect = Types.Search(type).GetEffect();

			amount = (int) (amount * react.Mult);
			int amt = Unit.Heal(amount, type);
			reportInt[1] = amt;

			if (amt != 0) {
				/*report.Affected.Add(this);
				report.AffectedEffects.Add(reportInt);*/
				report.Steps.Add(new ReportStep(this, 0, reportInt[1], 0, 0, reportInt[4], reportInt[5], 0));
			}

			return amt;
		}

		public void RunStatusStart (PlayReport report) {
			for (int i = 0; i < Statuses.Count; i++) {
				Statuses[i].Start(report);
			}
		}

		public void RunStatusEnd(PlayReport report) {
			for (int i = 0; i < Statuses.Count; i++) {
				Statuses[i].End(report);
			}

			for (int i = 0; i < Statuses.Count; i++) {
				if (Statuses[i].TurnsLeft == 0) {
					Statuses.RemoveAt(i);
					i--;
				}
			}
		}

		public override string ToString () {
			return Unit.Name + " " + Unit.HealthToString();
		}
	}

	public class Shield {
		public double Mitigation;
		
		public Shield (double mit) {
			Mitigation = mit;
		}
	}
}
