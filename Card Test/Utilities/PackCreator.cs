using Card_Test.Map;
using Card_Test.Files;
using Card_Test.Tables;
using System;
using System.Collections.Generic;
using System.Text;

namespace Card_Test.Utilities {
    public static class PackCreator {
		private static string Tables;
		private static MenuItem[] Menu;

		private static Pack Make;

		static PackCreator() {
			Make = Reader.ReadPack("Default");

			List<string> prompt = new List<string>();

			prompt.Add(Types.Viualize());
			prompt.Add(Mods.Viualize());
			prompt.Add(SubMods.Viualize());

			Tables = string.Join('\n', TextUI.MakeTable(prompt));

			Menu = new MenuItem[] {
				new MenuItem(new string[] { "Add", "A" }, AddCard, TextUI.Parse, "add a card to the new pack (type tier mod sub made chance max)"),
				new MenuItem(new string[] { "Remove", "R" }, RemoveCard, TextUI.Parse, "remove a card from the new pack"),
				new MenuItem(new string[] { "Weight", "W" }, WeightCard, TextUI.Parse, "Change the weight of a card in the pack (card# NewWeight MaxRolls)"),
				new MenuItem(new string[] { "Load", "L" }, MenuStub, LoadParse, "load a pack from a file"),
				new MenuItem(new string[] { "Clear", "Cl" }, ClearPack, TextUI.Parse, "reset the pack"),
				new MenuItem(new string[] { "Values", "V" }, ChangeValues, TextUI.Parse, "change the pack values (Chance Size MaxRoll)"),
				new MenuItem(new string[] { "Names", "N" }, MenuStub, ChangeNames, "change the name and symbols (PackName Symbol(three chars) SubSymbol)"),
				new MenuItem(new string[] { "Test", "T" }, TestPack, TextUI.Parse, "test the Pack (#ofTimes)"),
				new MenuItem(new string[] { "Finish", "F" }, MenuStub, FinishParse, "finish creating the pack and save it to the filename given")
			};

			
		}

		private static int[] LoadParse(string toParse) {
			string[] commands = toParse.Split(' ');
			if (commands.Length < 2) { return new int[] { 0 }; }

			Make = Reader.ReadPack(commands[1]);

			return null;
		}

		private static int[] FinishParse(string toParse) {
			string[] commands = toParse.Split(' ');
			if (commands.Length < 2) { return new int[] { 0 }; }

			Writer.WritePack(Make, commands[1]);
			ClearPack(null);

			return null;
		}

		private static int[] ChangeNames(string parse) {
			string[] chop = parse.Split(' ');
			if (chop.Length < 2) { return new int[] { 0 }; }

			string[] def = { Make.Name, Make.Symbol, Make.SubSymbol };

			for (int i = 1; i < chop.Length; i++) {
				def[i - 1] = chop[i];
			}

			Make.Name = def[0].Replace('_', ' ');
			Make.Symbol = def[1].Replace('_', ' ');
			Make.SubSymbol = def[2].Replace('_', ' ');

			return null;
		}

		private static bool TestPack(int[] data) {
			if (data.Length < 1) { return false; }

			int amt = data[0];
			while (amt > 0) {
				Console.Clear();

				List<TCard> cards = Make.PullTCards();
				List<string> contents = new List<string>();

				contents.Add(Make.ToString());

				for (int i = 0; i < cards.Count; i++) {
					contents.Add("\n" + cards[i].ToString());
				}

				TextUI.PrintFormatted(String.Join('\n', TextUI.MakeTable(contents)));
				TextUI.Wait();

				amt--;
			}

			return true;
		}

		private static bool MenuStub(int[] dum) {
			return (dum == null);
		}

		private static bool ClearPack(int[] dum) {
			Make = Reader.ReadPack("Default");
			return true;
		}

		private static bool ChangeValues(int[] data) {
			if (data.Length < 1) { return false; }

			int[] def = { 0, 1, 0 };

			for (int ii = 0; ii < data.Length && ii < def.Length; ii++) {
				def[ii] = data[ii];
			}

			Make.Chance = def[0];
			Make.Size = def[1];
			Make.MaxRolls = def[2];

			return true;
		}

		private static bool RemoveCard(int[] data) {
			if (data.Length < 1) { return false; }
			if (data[0] > Make.Contents.Count || data[0] < 1) { return false; }

			Make.Contents.RemoveAt(data[0] - 1);

			return true;
		}

		private static bool WeightCard(int[] data) {
			if (data.Length < 2) { return false; }
			if (data[0] > Make.Contents.Count || data[0] < 1) { return false; }

			PackE chosen = (PackE) Make.Contents[data[0] - 1];

			int[] def = { chosen.Chance, chosen.MaxRolls };

			for (int i = 1; i < data.Length; i++) {
				def[i - 1] = data[i];
			}

			chosen.Chance = def[0];
			chosen.MaxRolls = def[1];

			return true;
		}

		private static bool AddCard(int[] data) {
			if (data.Length < 1) { return false; }

			int[] def = { 0, 1, 0, 0, 0, 1, 0 };

			for (int ii = 0; ii < data.Length && ii < def.Length; ii++) {
				def[ii] = data[ii];
			}

			PackE tem = new PackE(new TCard(def[0], def[1], def[2], def[3], def[4]), def[5], def[6]);
			Make.AddPackE(tem);

			return true;
		}

		public static void CreatePack() {
			bool creating = true;
			while (creating) {
				PrintScene();
				TextUI.Prompt("What would you like to do?", Menu);
			}
		}

		private static void PrintScene() {
			Console.Clear();
			TextUI.PrintFormatted(Tables);
			TextUI.PrintFormatted(Make.EditString());
			TextUI.PrintFormatted("Cost = " + ShopValues.PackValue(Make).ToString());
		}

	}
}
