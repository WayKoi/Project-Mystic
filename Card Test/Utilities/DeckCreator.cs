using Card_Test.Files;
using Card_Test.Tables;
using System;
using System.Collections.Generic;
using System.Text;

namespace Card_Test.Utilities {
	public static class DeckCreator {
		private static string Tables;
		private static MenuItem[] Menu;

		private static Deck Make = new Deck(new Card(3, 1), new Card[0], 3);

		static DeckCreator () {
			List<string> prompt = new List<string>();

			prompt.Add(Types.Viualize());
			prompt.Add(Mods.Viualize());
			prompt.Add(SubMods.Viualize());

			Tables = string.Join('\n', TextUI.MakeTable(prompt));

			Menu = new MenuItem[] {
				new MenuItem(new string[] { "Add", "A" }, AddCard, TextUI.Parse, "add a card to the new deck (type tier mod sub made)"),
				new MenuItem(new string[] { "Remove", "R" }, RemoveCard, TextUI.Parse, "remove a card from the new deck"),
				new MenuItem(new string[] { "Load", "L" }, MenuStub, LoadParse, "load a deck from a file"),
				new MenuItem(new string[] { "Default", "D" }, ChangeDefault, TextUI.Parse, "change the default card"),
				new MenuItem(new string[] { "Clear", "Cl" }, ClearDeck, TextUI.Parse, "reset the deck"),
				new MenuItem(new string[] { "Values", "V" }, ChangeValues, TextUI.Parse, "change the deck values (handsize decklim trunklim)"),
				new MenuItem(new string[] { "Test", "T" }, MenuStub, TestDeck, "test using the deck (playerHealth playerMana EnemyHealth EnemyMana EnemyDeckName)"),
				new MenuItem(new string[] { "Finish", "F" }, MenuStub, FinishParse, "finish creating the deck and save it to the filename given")
			};
		}

		private static int[] LoadParse(string toParse) {
			string[] commands = toParse.Split(' ');
			if (commands.Length < 2) { return new int[] { 0 }; }

			Make = Reader.ReadDeck(commands[1]);

			return null;
		}

		private static int[] FinishParse (string toParse) {
			string[] commands = toParse.Split(' ');
			if (commands.Length < 2) { return new int[] { 0 }; }
			
			Writer.WriteDeck(Make, commands[1]);
			ClearDeck(null);

			return null;
		}

		private static bool MenuStub (int[] dum) {
			return (dum == null);
		}

		private static int[] TestDeck (string parse) {
			// parse string is
			// (cmd) playerHealth, playerMana, EnemyHealth, EnemyMana, EnemyDeckName
			Global.Run = new Current();

			string[] chop = parse.Split(' ');
			string enemDeck = chop[chop.Length - 1];
			int[] data = new int[Math.Min(chop.Length - 1, 4)];

			for (int i = 1; i < chop.Length && i < 5; i++) {
				int.TryParse(chop[i], out data[i - 1]);
			}

			if (data.Length < 0) { return null; }
			int[] def = { 100, 3, 100, 3 };

			for (int ii = 0; ii < data.Length && ii < def.Length; ii++) {
				def[ii] = data[ii];
			}

			Character[] Player = new Character[] { new Character("Tester", def[0], def[1], Make) };
			Character[] Enemy = new Character[] { new CardAI("Dummy", def[2], def[3], Reader.ReadDeck(enemDeck), null, 100, 100, 10) };

			Battle batt = new Battle(Player, Enemy);
			batt.Run();

			return null;
		}

		private static bool ClearDeck (int[] dum) {
			Make = new Deck(new Card(3, 1), new Card[0], 3);
			return true;
		}

		private static bool AddCard (int[] data) {
			if (data.Length < 1) { return false; }

			int[] def = { 0, 1, 0, 0, 0 };

			for (int ii = 0; ii < data.Length && ii < def.Length; ii++) {
				def[ii] = data[ii];
			}

			Card tem = new Card(def[0], def[1], def[2], def[3], def[4]);
			if (tem.Valid) { Make.AddCard(tem); }

			return true;
		}

		private static bool ChangeValues(int[] data) {
			if (data.Length < 1) { return false; }

			int[] def = { 0, -1, -1 };

			for (int ii = 0; ii < data.Length && ii < def.Length; ii++) {
				def[ii] = data[ii];
			}

			Make.StartHandSize = def[0];
			Make.DeckLim = def[1];
			Make.TrunkLim = def[2];

			return true;
		}

		private static bool RemoveCard (int[] data) {
			if (data.Length < 1) { return false; }
			if (data[0] > Make.DeckSize() || data[0] < 1) { return false; }
			
			Make.RemoveCardEdit(data[0] - 1);

			return true;
		}

		private static bool ChangeDefault (int[] data) {
			if (data.Length < 1) { return false; }

			int[] def = { 0, 1, 0, 0, 0 };

			for (int ii = 0; ii < data.Length && ii < def.Length; ii++) {
				def[ii] = data[ii];
			}

			Card tem = new Card(def[0], def[1], def[2], def[3], def[4]);
			Make.SetDefault(tem);

			return true;
		}

		public static void CreateDeck () {	
			bool creating = true;
			while (creating) {
				PrintScene();
				TextUI.Prompt("What would you like to do?", Menu);
			}
		}

		private static void PrintScene () {
			Console.Clear();
			TextUI.PrintFormatted(Tables);
			TextUI.PrintFormatted(Make.EditString());
		}

	}
}
