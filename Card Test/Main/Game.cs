﻿using System;
using System.Collections.Generic;
using System.Text;
using Card_Test.Map;
using Card_Test.Tables;
using Card_Test.Map.Rooms;
using Card_Test.Files;
using Card_Test.Utilities;

namespace Card_Test {
	public static class Game {
		public static void Start () {
			/*Deck test = Reader.ReadDeck("tango");
			// Writer.WriteDeck(test, "Test");
			test.EditDeck();*/

			// PackCreator.CreatePack();
			// DeckCreator.CreateDeck();
			// GearCreator.CreateGear();

			/*Writer.WriteGear(TGear.Generate(GearTable.Tango[2], null), "test");
			TGear test = Reader.ReadTGear("test");
			TextUI.PrintFormatted(test.ToString());
			TextUI.Wait();*/

			// while (true) { TestFloor(FloorTable.IvoryHallsII); } // hang

			MenuItem[] CharacterMenu = {
				new MenuItem(new string[] { "View", "V" }, ViewDetails, TextUI.Parse, "view a characters details" ),
				new MenuItem(new string[] { "Choose", "C" }, ChooseCharacter, TextUI.Parse, "choose a character")
			};

			while (true) {
				while (Global.Run == null) {
					PrintCharacterSelection();
					TextUI.Prompt("What would you like to do?", CharacterMenu);
				}

				// Global.Run.Player.MaxFusion = 2;
				// Global.Run.Player.MaxSide = 1;
				// Global.Run.Player.MaxMulti = 1;

				// Global.Run.Player.Material = 10000;

				/*for (int i = 0; i < 5; i++) {
					for (int ii = 0; ii < Global.Run.Player.Gear.Count; ii++) {
						Global.Run.Player.Gear[ii].Upgrade(true);
					}
				}*/

				/*Console.Clear();
				StoneTable.RollStone("Test Gem", "TG", StoneTable.ApocalypseGemStone, 4);*/

				/*Global.Run.Player.MaxHealth = 1000;
                Global.Run.Player.Health = 1000;

                CardAI test = new CardAI("Fusionist", 1, 4, Reader.ReadDeck("warlock"), null, 100, 100, 3);
                test.MaxFusion = 1;
                Battle batt = new Battle(Global.Run.Players.ToArray(), new Character[] { test });
                batt.Run();*/
				
				// DropTable.Four(null);

				Global.Run.TenFloor.Init();
				Global.Run.TenFloor.Run();
				
				Global.Run = null;
			}
		}

		public static void PrintCharacterSelection () {
			Console.Clear();

			TextUI.PrintFormatted("Choose a character\n");
			
			int amt = 1;
			foreach (CTableEntry ent in CharacterTable.Table) {
				TextUI.PrintFormatted(" " + amt + ": " + ent.Token[0] + ent.Name + "⁰");
				amt++;
			}

			Console.WriteLine();
		}

		public static bool ChooseCharacter (int[] data) {
			if (data.Length != 1) { return false; }
			if (data[0] < 1 || data[0] > CharacterTable.Table.Length) { return false; }

			Global.Run = new Current();
			Global.Run.Player = CharacterTable.CreateCharacter(CharacterTable.Table[data[0] - 1]);
			Global.Run.Players.Add(Global.Run.Player);

			return true;
		}

		public static bool ViewDetails (int[] data) {
			if (data.Length != 1) { return false; }
			if (data[0] < 1 || data[0] > CharacterTable.Table.Length) { return false; }
			data[0]--;

			Console.Clear();

			Deck view = Reader.ReadDeck(CharacterTable.Table[data[0]].Name);

			string build = CharacterTable.Table[data[0]].ToString();
			build += "\n\nDefault\n" + view.Default + "\n\n" + view.DeckToString();
			TextUI.PrintFormatted(build + "\n");

			TextUI.Wait();
			return true;
		}

		public static void TestPack (Pack chosen) {
			while (true) {
				Console.Clear();
				TextUI.PrintFormatted(chosen.ToString());

				List<Card> pulls = chosen.Pull();

				List<string> cards = new List<string>();

				foreach (Card card in pulls) {
					cards.Add(card.ToString());
				}

				TextUI.PrintFormatted("\nYou open the pack");
				TextUI.PrintFormatted(String.Join('\n', TextUI.Combine(cards)));
				TextUI.PrintFormatted("All cards are added to your deck\n");

				TextUI.Wait();
			}
		}

		public static void TestFloor (FloorGen plan) {
			Console.Clear();

			new Floor(plan, true);

			/*floor.RevealFloor();

			TextUI.PrintFormatted(floor.ToString());*/

			TextUI.Wait();
			GC.Collect();
		}
	}
}
