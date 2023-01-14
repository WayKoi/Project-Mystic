using System;
using System.Collections.Generic;
using System.Text;
using Card_Test.Utilities;

namespace Card_Test.Tables {
	public static class SubMods {
		private static CardMod[] Table = {
			new CardMod("None", " "),
			new CardMod("Overload", "ε", Overload),
			new CardMod("Plus One", "+", PlusOne),
			// new CardMod("Plus One", "-", MinusOne),
			new CardMod("Thick", "↓"),
			new CardMod("Thin", "↑"),
			new CardMod("Status", "S", AfflictStatus)
		};

		public static CardMod Search(string name) {
			return Search(Translate(name));
		}

		public static CardMod Search(int index) {
			if (index < 0 || index >= Table.Length) { return null; }
			return Table[index];
		}

		public static int Translate(string type) {
			switch (type) {
				case "none":     return 0;
				case "overload": return 1;
				case "plusone":  return 2;
				case "thick":    return 3;
				case "thin":     return 4;
				case "status":   return 5;
			}

			return -1;
		}

		public static int TableLength() {
			return Table.Length;
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
			string head = new string(' ', body.Split('\n')[0].Length / 2 - 3) + "Subs\n" + new string('-', body.Split('\n')[0].Length - 3) + "\n";

			return head + body;

		}

		private static void AfflictStatus(Card Cast, Character Caster, List<BattleChar> targets, int specific, int[] data, PlayReport report) {
			CardType type = Cast.LookupType();
			int status = type.GetStatus();

			if (status != -1) {
				// make the turns it lasts more balanced if it is unbalanced
				Status stat = new Status(Cast, StatusTable.Table[status], targets[specific], Cast.Tier + 1, Cast.Tier, 0.5, report);
			}
		}

		private static void PlusOne(Card Cast, Character Caster, List<BattleChar> targets, int specific, int[] data, PlayReport report) {
			Caster.DrawCard();
			report.Additional.Add(Caster.Name + " draws an extra card");
			// TextUI.PrintFormatted(Caster.Name + " draws an extra card");
		}

		private static void Overload(Card Cast, Character Caster, List<BattleChar> targets, int specific, int[] data, PlayReport report) {
			int overload = (int)Math.Ceiling(Cast.Tier / 2.0);

			foreach (BattleChar unit in targets) {
				if (unit.Unit == Caster) {
					unit.Overload = overload;
					break;
				}
			}

			report.Additional.Add(Caster.Name + " will have " + overload + " less mana next turn");

			// TextUI.PrintFormatted(Caster.Name + " casted an Overload spell and will have " + overload + " less mana next turn");
		}
	}
}
