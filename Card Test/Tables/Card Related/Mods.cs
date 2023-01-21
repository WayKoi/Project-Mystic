using Card_Test.Utilities;
using Sorting;
using System;
using System.Collections.Generic;
using System.Text;

namespace Card_Test.Tables {
	public static class Mods {
		private static CardMod[] Table = {
			new CardMod("None", " "),
			new CardMod("Jumping", "√", Jumping),
			new CardMod("AOE", "Θ", AOE),
			new CardMod("Summon", "Ω", Summon),
		};

		public static int TableLength () {
			return Table.Length;
		}

		public static CardMod Search(string name) {
			return Search(Translate(name));
		}

		public static CardMod Search(int index) {
			if (index < 0 || index >= Table.Length) { return null; }
			return Table[index];
		}

		public static int Translate(string type) {
			switch (type) {
				case "none": return 0;
				case "jumping": return 1;
				case "aoe": return 2;
				case "summon": return 3;
			}

			return -1;
		}

		public static string Viualize() {
			List<string> colA = new List<string>(), colB = new List<string>(), colC = new List<string>();
			List<string>[] cols = { colA, colB };

			int count = Table.Length / cols.Length;

			int typecount = Table.Length;
			for (int i = 0; i < typecount; i++) {
				cols[i / (count + 1)].Add(i.ToString() + ". " + (i < 10 ? " " : "") + Table[i].Name);
			}

			List<string> combine = new List<string>();

			for (int i = 0; i < cols.Length; i++) {
				combine.Add(string.Join('\n', cols[i]));
			}

			string body = string.Join('\n', TextUI.MakeTable(combine, 3));
			string head = new string(' ', body.Split('\n')[0].Length / 2 - 3) + "Mods\n" + new string('-', body.Split('\n')[0].Length - 3) + "\n";

			return head + body;

		}

		private static void Summon (Card Cast, Character Caster, List<BattleChar> targets, int specific, int[] data, PlayReport report) {
			BattleChar castedBy = null;
			
			foreach (BattleChar unit in targets) {
				if (unit.Unit == Caster) {
					castedBy = unit;
					break;
				}
			}

			if (castedBy != null) {
				List<CardAI> summoned = SummonTable.CreateSummon(Cast);

				if (summoned == null || summoned.Count == 0) {
					report.Additional.Add("Summoning Failed!");
					return;
				}

				for (int i = 0; i < summoned.Count; i++) {
					if (castedBy.Side == 0) { summoned[i].PlanVisible = true; }
					BattleChar summonBatt = new BattleChar(summoned[i], castedBy.Side);
					report.Additional.Add(summoned[i].Name + " joins the battle!");
					targets.Add(summonBatt);
				}
			}
		}

		private static void AOE(Card Cast, Character Caster, List<BattleChar> targets, int specific, int[] data, PlayReport report) {
			int[] tierAmt = { 50, 55, 60, 65, 70, 75, 80 };
			int tier = Math.Min(Cast.Tier, 7) - 1;

			int side = targets[specific].Side;
			List<int> SubTargets = BattleUtil.GetFromSide(side, targets);
			SubTargets.Remove(specific);

			if (SubTargets.Count == 0) { return; }
			
			foreach (int targ in SubTargets) {
				if (targets[targ].Unit.HasHealth()) {
					int vampfrom = targets[targ].TakeDamage(Caster, (int)(data[0] * tierAmt[tier] / 100.0), Cast.Type, report);
					// targets[targ].Heal((int)(data[1] * tierAmt[tier] / 100.0) + (int)(vampfrom * Cast.LookupType().VampPercent), Cast.Type, report);

					CardType test = Types.Search(Cast.Type);
					if (test != null) {
						test.CastAdditional(Cast, targets, targ, report);
					}
				}
			}
		}

		private static void Jumping (Card Cast, Character Caster, List<BattleChar> targets, int specific, int[] data, PlayReport report) {
			int[] tierAmt = { 50, 50, 60, 60, 70, 70, 75};
			int tier = Math.Min(Cast.Tier, 7) - 1;

			int side = targets[specific].Side;
			List<int> SubTargets = BattleUtil.GetFromSide(side, targets);
			// SubTargets.Remove(specific);

			if (SubTargets.Count == 0) { return; }
			int chosen = Global.Rand.Next(0, SubTargets.Count);

			while (SubTargets.Count > 0 && !targets[SubTargets[chosen]].Unit.HasHealth()) {
				SubTargets.RemoveAt(chosen);
				chosen = Global.Rand.Next(0, SubTargets.Count);
			}

			if (SubTargets.Count == 0) { return; }
			// TextUI.PrintFormatted("Jumps to " + targets[SubTargets[chosen]].Unit.Name);
			int vampfrom = targets[SubTargets[chosen]].TakeDamage(Caster, (int) (data[0] * tierAmt[tier] / 100.0), Cast.Type, report);
			// targets[SubTargets[chosen]].Heal((int)(data[1] * tierAmt[tier] / 100.0) + (int)(vampfrom * Cast.LookupType().VampPercent), Cast.Type, report);

			CardType test = Types.Search(Cast.Type);
			if (test != null) {
				test.CastAdditional(Cast, targets, SubTargets[chosen], report);
			}
		}
	}

	public class CardMod {
		public string Name, Symbol;
		//        casted,    caster,      targets,  specific, { damage, healing }
		private Action<Card, Character, List<BattleChar>, int, int[], PlayReport> ModAction;

		public CardMod (string name, string symbol, Action<Card, Character, List<BattleChar>, int, int[], PlayReport> act = null) {
			Name = name;
			Symbol = symbol;
			ModAction = act;
		}

		public void RunAction (Card Cast, Character Caster, List<BattleChar> targets, int specific, int[] data, PlayReport report) {
			if (ModAction != null && data.Length == 2) {
				ModAction(Cast, Caster, targets, specific, data, report);
			}
		}
	}
}
