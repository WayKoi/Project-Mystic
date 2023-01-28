using Card_Test.Items;
using Card_Test.Utilities;
using System;
using System.Collections.Generic;
using System.Text;
using Card_Test.Base;

namespace Card_Test.Tables {
	public static class StoneTable {
		public static Stone RollStone (string name, string symbol, StoneEffect[] table, int choices = 3) {
			List<StoneEffect> effectChoices = new List<StoneEffect>();
			List<int> choiceOdds = new List<int>();

			foreach (StoneEffect eff in table) {
				if (eff.Chance > 0) {
					effectChoices.Add(eff);
					choiceOdds.Add(eff.Chance);
				}
			}

			List<StoneEffect> Options = new List<StoneEffect>();

			while (choices > 0 && effectChoices.Count > 0) {
				int chosen = Global.Roll(choiceOdds.ToArray());
				Options.Add(effectChoices[chosen]);
				effectChoices.RemoveAt(chosen);
				choiceOdds.RemoveAt(chosen);
				choices--;
			}

			TextUI.PrintFormatted(Global.Run.Player.Name + " obtains the " + name + "\n");
			TextUI.PrintFormatted("Choose effect for The " + name + " to have\n");
			for (int i = 0; i < Options.Count; i++) {
				TextUI.PrintFormatted(" " + (i + 1).ToString() + ": " + Options[i].Description);
			}
			Console.WriteLine();

			int effChosen = TextUI.Prompt("Which would you like?", 1, Options.Count);
			effChosen--;

			Stone generated = new Stone(name, symbol, Global.Run.Player, Options[effChosen]);

			generated.Chosen();

			return generated;
		}

		public static void RestrictApplyEffects(Stone used, int[] dum) {
			// for restricting applying effects to the field
			DuneGem[1].Chance = 0;
			IvoryGem[0].Chance = 0;
		}

		public static void RestrictUnusedManaEffects(Stone used, int[] dum) {
			DuneGem[0].Chance = 0;
			IvoryGem[1].Chance = 0;
		}

		public static void EffectField(PlayReport report, Stone Used, int[] data, List<BattleChar> targets, int specific) {
			// data contains the effect to give
			if (data == null || data.Length != 1) { return; }
			// PlayReport report = new PlayReport(Used.Owner, Used);
			report.Caster = Used.Owner;
			report.Played = Used;

			List<int> enems = BattleUtil.GetFromSide(1, targets);

			foreach (int num in enems) {
				if (targets[num].Unit.HasHealth()) {
					targets[num].Effect = data[0];
					/*report.Affected.Add(targets[num]);
					// damage, healing, shields broken, shields added, reaction, effect, damage blocked
					report.AffectedEffects.Add(new int[] { 0, 0, 0, 0, -1, data[0], 0 });*/
					report.Steps.Add(new ReportStep(targets[num], 0, 0, 0, 0, -1, data[0]));
				}
			}

			// report.PrintReport();
		}

		public static void StatusField(PlayReport report, Stone Used, int[] data, List<BattleChar> targets, int specific) {
			// data contains status to give, turns to give it for, tier
			if (data == null || data.Length != 3) { return; }
			// PlayReport report = new PlayReport(Used.Owner, Used);
			report.Caster = Used.Owner;
			report.Played = Used;

			List<int> enems = BattleUtil.GetFromSide(1, targets);

			foreach (int num in enems) {
				if (targets[num].Unit.HasHealth()) {
					Status stat = new Status(null, StatusTable.Table[data[0]], targets[num], data[1], data[2], 0.5, report);
					// report.Additional.Add(targets[num].Unit.Name + " " + stat.Link.Phrase);
				}
			}

			// report.PrintReport();
		}

		public static void DrawGhost (PlayReport report, Stone Used, int[] data, List<BattleChar> targets, int specific) {
			// data contains type of card to generate
			if (data == null || data.Length != 1) { return; }
			// PlayReport report = new PlayReport(Used.Owner, Used);
			report.Caster = Used.Owner;
			report.Played = Used;

			Card generate = Used.Owner.Cards.GetGhostCard();
			generate.ChangeType(data[0]);

			Used.Owner.Hand.Add(generate);
			report.Additional.Add(Used.Owner.Name + " draws a Ghost " + Types.Search(data[0]).Name + " card");

			/*report.PrintReport();
			TextUI.Wait();*/
		}

		public static StoneEffect[] DuneGem = {
			new StoneEffect(10, "Takes unused mana and restores 5% max HP to party at end of turn (Max 25%), restriction : all other unused mana effects", 
				true, false, false, true, -1, -1, null, DuneA, null, RestrictUnusedManaEffects
			),
			new StoneEffect(10, "Applies dry to all enemies before casting starts, restriction : all other similar stone effects",
				true, false, false, false, -1, -1, new int[] { 6 }, EffectField, null, RestrictApplyEffects
			),
			new StoneEffect(10, "Restores 5% max HP at end of turn to random friendly party member",
				true, false, false, true, -1, -1, null, DuneB
			),

			new StoneEffect(10, "3 Mana, Heal specific target for 50% max HP, 1 per battle",
				false, true, false, false, 3, 1, null, DuneC
			),
			new StoneEffect(10, "2 Mana, Sink the enemies in sand for a turn making them unable to attack this turn, 1 per battle",
				false, false, false, false, 2, 1, new int[] { 0, 1, 0 }, StatusField
			),
			new StoneEffect(10, "1 Mana, Create a sand card based on a card in your deck and draw it, Instant effect, 2 per battle",
				false, false, true, false, 1, 2, new int[] { Types.Translate("sand") }, DrawGhost
			)
		};

		public static void DuneA (PlayReport report, Stone Used, int[] data, List<BattleChar> targets, int specific) {
			int restored = Math.Min(Used.Owner.Mana * 5, 25);
			if (restored == 0) { return; }

			List<int> friends = BattleUtil.GetFromSide(0, targets);
			// PlayReport report = new PlayReport(Used.Owner, Used);
			report.Caster = Used.Owner;
			report.Played = Used;

			foreach (int num in friends) {
				targets[num].Heal(targets[num].Unit.MaxHealth / 100 * restored, Types.Translate("sand"), report);
			}

			// report.PrintReport();
		}
		
		public static void DuneB (PlayReport report, Stone Used, int[] data, List<BattleChar> targets, int specific) {
			List<int> friends = BattleUtil.GetFromSide(0, targets);
			if (friends.Count == 0) { return; }

			// PlayReport report = new PlayReport(Used.Owner, Used);
			report.Caster = Used.Owner;
			report.Played = Used;

			int num = Global.Rand.Next(0, friends.Count);
			targets[num].Heal((int) (targets[num].Unit.MaxHealth / 20.0), Types.Translate("sand"), report);

			// report.PrintReport();
		}

		public static void DuneC (PlayReport report, Stone Used, int[] data, List<BattleChar> targets, int specific) {
			// PlayReport report = new PlayReport(Used.Owner, Used);
			report.Caster = Used.Owner;
			report.Played = Used;

			targets[specific].Heal(targets[specific].Unit.MaxHealth / 2, Types.Translate("sand"), report);

			// report.PrintReport();
		}

		public static StoneEffect[] IvoryGem = {
			new StoneEffect(10, "Applies Sprout to all enemies before casting starts, restriction : all other similar stone effects",
				true, false, false, false, -1, -1, new int[] { 9 }, EffectField, null, RestrictApplyEffects
			),
			new StoneEffect(10, "Takes unused mana and draws a card for every mana not used at the end of the turn (max 2), restriction : all other unused mana effects",
				true, false, false, true, -1, -1, null, IvoryA, null, RestrictUnusedManaEffects
			),
			new StoneEffect(10, "Player gains a 50% shield at the end of the turn",
				true, false, false, true, -1, -1, null, IvoryB
			),

			new StoneEffect(10, "2 Mana, Draw 2, 1 per battle",
				false, false, true, false, 2, 1, null, IvoryD
			),
			new StoneEffect(10, "2 Mana, Root all opponents for a turn making them unable to attack, 1 per battle",
				false, false, false, false, 2, 1, new int[] { 4, 1, 0 }, StatusField
			),
			new StoneEffect(10, "4 Mana, Entire party gets 2 shields, 1 per battle",
				false, false, false, false, 4, 1, null, IvoryC
			),
		};

		public static void IvoryA(PlayReport report, Stone Used, int[] data, List<BattleChar> targets, int specific) {
			int unused = Math.Min(Used.Owner.Mana, 2);
			if (unused == 0) { return; }

			// PlayReport report = new PlayReport(Used.Owner, Used);
			report.Caster = Used.Owner;
			report.Played = Used;

			int User = 0;
			int count = 0;
			foreach (BattleChar unit in targets) {
				if (unit.Unit == Used.Owner) {
					User = count;
					break;
				}
				count++;
			}

			Types.Time(new Card(0, unused), targets, User, report);

			// report.PrintReport();
		}

		public static void IvoryB(PlayReport report, Stone Used, int[] data, List<BattleChar> targets, int specific) {
			// PlayReport report = new PlayReport(Used.Owner, Used);
			report.Caster = Used.Owner;
			report.Played = Used;

			int User = 0;
			int count = 0;
			foreach (BattleChar unit in targets) {
				if (unit.Unit == Used.Owner) {
					User = count;
					break;
				}
				count++;
			}

			Types.Shield(new Card(2, 0), targets, User, report);

			// report.PrintReport();
		}

		public static void IvoryC (PlayReport report, Stone Used, int[] data, List<BattleChar> targets, int specific) {
			// PlayReport report = new PlayReport(Used.Owner, Used);
			report.Caster = Used.Owner;
			report.Played = Used;

			List<int> friends = BattleUtil.GetFromSide(0, targets);

			foreach (int num in friends) {
				Types.Shield(new Card(2, 2), targets, num, report);
			}

			// report.PrintReport();
		}

		public static void IvoryD(PlayReport report, Stone Used, int[] data, List<BattleChar> targets, int specific) {
			report.Caster = Used.Owner;
			report.Played = Used;

			int User = 0;
			int count = 0;
			foreach (BattleChar unit in targets) {
				if (unit.Unit == Used.Owner) {
					User = count;
					break;
				}
				count++;
			}

			Types.Time(new Card(0, 2), targets, User, report);
		}
	}

	public class StoneEffect {
		public string Description;
		public bool RequiresTarget, IsPassive, IsInstant, EndofTurn, UsedThisTurn = false;
		//          chance of appearing, cost of activating
		public int Chance, Cost, UsesLeft = 0, MaxPerBattle;
		public int[] Data, ChosenData;
		//             used     data     targets    specific
		public Action<PlayReport, Stone, int[], List<BattleChar>, int> Effect;
		public Action<Stone, int[]> Chosen;

		public StoneEffect (int chance, string desc, bool ispassive, bool targeting, bool instant, bool endofturn, int cost, int maxperbattle, int[] data, Action<PlayReport, Stone, int[], List<BattleChar>, int> effect, int[] chosenData = null, Action<Stone, int[]> chosen = null) {
			Description = desc;
			IsPassive = ispassive;
			RequiresTarget = targeting;
			IsInstant = instant;
			EndofTurn = endofturn;

			Cost = cost;
			MaxPerBattle = maxperbattle;

			Data = data;
			Effect = effect;

			Chance = chance;
			
			ChosenData = chosenData;
			Chosen = chosen;
		}

		public void Refresh () {
			UsesLeft = MaxPerBattle;
			TurnRefresh();
		}

		public void TurnRefresh () {
			UsedThisTurn = false;
		}
	}

	public class Stone : Plannable {
		public string Name, Symbol;
		public Player Owner;
		public StoneEffect Effect;

		public Stone (string name, string symbol, Player owner, StoneEffect eff) {
			Name = name;
			Symbol = symbol;
			Owner = owner;
			Effect = eff;

			Owner.Stones.Add(this);

			ManaCost = eff.Cost;
			Targeting = eff.RequiresTarget;
			Instant = eff.IsInstant;
			Passive = eff.IsPassive;
		}

		public void Chosen () {
			if (Effect.Chosen != null) {
				Effect.Chosen(this, Effect.ChosenData);
			}
		}

		public override bool Additional(Character Caster, PlayReport report) {
			if (Effect.UsedThisTurn) {
				if (report != null) { report.Additional.Add("This effect can only be used once per turn"); } 
				return false;
			}

			if (Effect.UsesLeft <= 0) {
				if (report != null) { report.Additional.Add("This effect cannot be used again this battle"); }
				return false;
			}

			return true;
		}

		public override void Cancel(Character Caster) { Effect.UsedThisTurn = false; }
		public override void Plan(Character Caster) { Effect.UsedThisTurn = true; }
		public override void UpdateValues(Character caster) {
			if (Effect.Cost == -1) { // -1 means that it consumes remaining mana
				ManaCost = Math.Max(caster.Mana, 1);
			}
		}
		public override bool Play(Character Caster, List<BattleChar> Targets, int Specific, PlayReport report = null) {
			bool print = report == null;
			if (report == null) { report = new PlayReport(); }

			if (Effect.Effect != null) {
				Effect.UsesLeft--;
				Effect.Effect(report, this, Effect.Data, Targets, Specific);
			}

			if (print) {
				report.PrintReport();
			}

			return true;
		}

		public override string ToString() {
			string build = "   ^   \n" +
						   "  /|\\  \n" +
						   " /_|_\\ \n" +
						   " \\ | / \n" +
						   "  \\|/  \n" +
						   "   V   \n";
			build += Name;

			return build;
		}

		public string GetInfo () {
			string build = Name + " : " + (Effect.IsInstant ? "Instant " : "") + (Effect.EndofTurn ? "End of Turn " : "") + (Effect.IsPassive ? "Passive" : "Active") + (Effect.RequiresTarget ? " Requires Target" : "") + "\n";
			if (!Effect.IsPassive) {
				build += "  Uses left : " + Effect.UsesLeft + "/" + Effect.MaxPerBattle + "\n\n";
			}

			build += Effect.Description + "\n";

			return build;
		}
	}
}
