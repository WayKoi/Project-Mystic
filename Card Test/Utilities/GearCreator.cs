using Card_Test.Files;
using Card_Test.Items;
using Card_Test.Tables;
using System;
using System.Collections.Generic;
using System.Text;

namespace Card_Test.Utilities {
	public static class GearCreator {
		private static string Tables;
		private static MenuItem[] Menu;

		private static TGear Make = Reader.ReadTGear("Default");

		static GearCreator() {
			List<string> prompt = new List<string>();

			List<string> effects = new List<string>();
			int count = TGearEffect.TypeNames.Length;

			for (int i = 0; i < count; i++) {
				effects.Add((i + 1).ToString() + ". " + (i < 10 ? " " : "") + TGearEffect.TypeNames[i]);
			}

			List<string> combine = new List<string>();
			combine.Add(string.Join('\n', effects));

			string body = string.Join('\n', TextUI.MakeTable(combine, 3));
			string head = new string(' ', body.Split('\n')[0].Length / 2 - 3) + "Effects\n" + new string('-', body.Split('\n')[0].Length - 3) + "\n";

			prompt.Add(head + body);
			prompt.Add(BaseTypes.Viualize());

			Tables = string.Join('\n', TextUI.MakeTable(prompt)) + "\n";
			Tables = string.Join('\n', TextUI.MakeTable(prompt)) + "\n";
			Menu = new MenuItem[] {
				new MenuItem(new string[] { "Add", "A" }, AddEffect, TextUI.Parse, "add an effect to the piece of gear (efftype min max chance maxrolls)"),
				new MenuItem(new string[] { "Weight", "W" }, WeightEffect, TextUI.Parse, "Change the weight of an effect on the gear (effect# NewWeight MaxRolls)"),
				new MenuItem(new string[] { "Edit", "E" }, EditEffect, TextUI.Parse, "Change the values of an effect on the gear (effect# Min Max AffType)"),
				new MenuItem(new string[] { "Rolls", "Ro" }, EditRolls, TextUI.Parse, "Change the rolls on the piece of gear"),
				new MenuItem(new string[] { "Name", "N" }, MenuStub, EditName, "Change the name of the gear"),
				new MenuItem(new string[] { "Gear", "G" }, EditGear, TextUI.Parse, "Change the values of the gear (maxupgrades upgrades)"),
				new MenuItem(new string[] { "Remove", "R" }, RemoveEffect, TextUI.Parse, "remove an effect from the piece of gear"),
				new MenuItem(new string[] { "Clear", "Cl" }, ClearGear, TextUI.Parse, "reset the gear"),
				new MenuItem(new string[] { "Load", "L" }, MenuStub, LoadParse, "load a piece of gear from a file"),
				new MenuItem(new string[] { "Test", "T" }, TestGear, TextUI.Parse, "test using the deck (playerHealth playerMana EnemyHealth EnemyMana EnemyDeckName)"),
				new MenuItem(new string[] { "Finish", "F" }, MenuStub, FinishParse, "finish creating the piece of gear and save it to the filename given")
			};
		}

		private static int[] LoadParse(string toParse) {
			string[] commands = toParse.Split(' ');
			if (commands.Length < 2) { return new int[] { 0 }; }

			Make = Reader.ReadTGear(commands[1]);

			return null;
		}

		private static int[] FinishParse(string toParse) {
			string[] commands = toParse.Split(' ');
			if (commands.Length < 2) { return new int[] { 0 }; }

			Writer.WriteGear(Make, commands[1]);
			ClearGear(null);

			GC.Collect();

			return null;
		}

		private static int[] EditName(string toParse) {
			string[] commands = toParse.Split(' ');
			if (commands.Length < 2) { return new int[] { 0 }; }

			Make.Name = commands[1].Replace('_', ' ');

			return null;
		}

		private static bool EditGear(int[] data) {
			if (data.Length < 1) { return false; }
			
			int[] def = { Make.MaxUpgrades, Make.Upgrades };

			for (int ii = 0; ii < data.Length && ii < def.Length; ii++) {
				def[ii] = data[ii];
			}

			Make.MaxUpgrades = def[0];
			Make.Upgrades = def[1];

			return true;
		}

		private static bool EditRolls(int[] data) {
			if (data.Length < 1) { return false; }
			List<int> rolls = new List<int>();

			for (int i = 0; i < data.Length; i++) {
				rolls.Add(data[i]);
				if (data[i] <= 0) { return false; }
			}

			Make.Rolls = rolls.ToArray();

			return true;
		}

		private static bool TestGear(int[] data) {
			if (data.Length < 2) { return false; }

			int amt = data[0];
			int rollamt = data[1];

			while (amt > 0) {
				Character nully = new Character("nully", 0, 0, Reader.ReadDeck("Default"));
				Gear gen = TGear.Generate(Make, nully);

				Console.Clear();

				int counter = rollamt;
				while (counter > 0) {
					gen.Upgrade();

					counter--;
				}

				TextUI.Wait();

				amt--;
			}

			return true;
		}

		private static bool AddEffect (int[] data) {
			if (data.Length < 1) { return false; }

			// efftype, min, max, chance, maxrolls
			int[] def = { 1, 0, 0, 1, 0 };

			for (int ii = 0; ii < data.Length && ii < def.Length; ii++) {
				def[ii] = data[ii];
			}

			if (def[1] > def[2]) {
				def[2] = def[1];
			}

			TGearEffect eff = new TGearEffect(def[0], new int[] { def[1], def[2], 0 }, def[3], def[4]);
			Make.AddEffect(eff);

			return true;
		}

		private static bool RemoveEffect(int[] data) {
			if (data.Length < 1) { return false; }
			if (data[0] > Make.Effects.Count || data[0] < 1) { return false; }

			Make.Effects.RemoveAt(data[0] - 1);

			return true;
		}

		private static bool ClearGear (int[] dum) {
			Make = Reader.ReadTGear("Default");
			return true;
		}

		private static bool WeightEffect(int[] data) {
			if (data.Length < 2) { return false; }
			if (data[0] > Make.Effects.Count || data[0] < 1) { return false; }

			TGearEffect chosen = Make.Effects[data[0] - 1];

			int[] def = { chosen.Chance, chosen.MaxRolls };

			for (int i = 1; i < data.Length && i < def.Length + 1; i++) {
				def[i - 1] = data[i];
			}

			chosen.Chance = def[0];
			chosen.MaxRolls = def[1];

			return true;
		}

		private static bool EditEffect(int[] data) {
			if (data.Length < 2) { return false; }
			if (data[0] > Make.Effects.Count || data[0] < 1) { return false; }

			TGearEffect chosen = Make.Effects[data[0] - 1];

			int[] def = { chosen.Min, chosen.Max, chosen.AffType };

			for (int i = 1; i < data.Length; i++) {
				def[i - 1] = data[i];
			}

			if (def[0] > def[1]) {
				def[1] = def[0];
			}

			chosen.Min = def[0];
			chosen.Max = def[1];
			chosen.AffType = Math.Max(Math.Min(def[2], BaseTypes.TableLength() - 1), 0);

			Make.SortEffects();

			return true;
		}

		public static void CreateGear() {
			bool creating = true;
			while (creating) {
				PrintScene();
				TextUI.Prompt("What would you like to do?", Menu);
			}
		}

		private static bool MenuStub(int[] dum) {
			return (dum == null);
		}

		private static void PrintScene() {
			Console.Clear();
			TextUI.PrintFormatted(Tables);
			TextUI.PrintFormatted(Make.ToString());
		}
	}
}
