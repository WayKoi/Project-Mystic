using System;
using System.Collections.Generic;
using System.Text;

namespace Card_Test.Tables {
	public static class BaseTypes {
		private static BaseType[] Table = {
			// name status effect
			new BaseType("Magic", -1, 0),
			new BaseType("Physical", -1, 0),

			new BaseType("Fire", 2, 1),
			new BaseType("Water", 3, 2),
			new BaseType("Earth", -1, 3),
			new BaseType("Wind", -1, 4),

			new BaseType("Life", 4, 9),
			new BaseType("Electric", -1, 10),
			new BaseType("Light", -1, 1),
			new BaseType("Dark", -1, 7),
			new BaseType("Dream", -1, 0),

			new BaseType("Ice", 1, 5),
			new BaseType("Sand", 0, 6)
		};

		public static string Viualize() {
			List<string> colA = new List<string>();
			List<string>[] cols = { colA };

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
			string head = new string(' ', body.Split('\n')[0].Length / 2 - 3) + "Types\n" + new string('-', body.Split('\n')[0].Length - 3) + "\n";

			return head + body;

		}

		public static int TableLength() {
			return Table.Length;
		}

		public static BaseType Search(string name) {
			return Search(Translate(name));
		}

		public static BaseType Search(int index) {
			if (index < 0 || index >= Table.Length) { return Table[0]; }
			return Table[index];
		}

		public static int Translate(string type) {
			switch (type) {
				case "magic":    return 0;
				case "physical": return 1;

				case "fire":  return 2;
				case "water": return 3;
				case "earth": return 4;
				case "wind":  return 5;
				
				case "life":     return 6;
				case "electric": return 7;
				case "light":    return 8;
				case "dark":     return 9;
				case "dream":    return 10;
				
				case "ice":  return 11;
				case "sand": return 12;
			}

			return -1;
		}

	}

	public class BaseType {
		public string Name;
		public int Status, Effect;

		public BaseType(string name, int status, int effect) {
			Name = name;
			Status = status;
			Effect = effect;
		}
	}
}
