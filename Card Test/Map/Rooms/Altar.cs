using Card_Test.Items;
using Card_Test.Tables;
using Card_Test.Utilities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Card_Test.Map.Rooms {
	public class Altar : Room {
		private static int[] Payouts = { 70, 100, 150 };
		private static int[] MinPayouts = { 20, 40, 60 };
		// give health
		// give pack
		// give upgraded best card
		// give gear upgrade
		// give all gear one upgrade
		// give material
		private static int[][] PayChances = {
			new int[] { 15, 20,  4, 1, 0, 10 },
			new int[] {  8, 10,  8, 2, 1, 10 },
			new int[] {  2, 10, 12, 4, 2, 10 }
		};
		

		private MenuItem[] Interact;
		private bool Interacting = false;
		private List<Card> Offering = new List<Card>();
		private int MaxOffering = 3;
		private bool Glowing = true;


		public Altar(Room replace) : base(replace) {
			RoomType = 7;
			Description = "This room has a large Altar sitting in the center";
			RoomName = "Altar";
			Symbol = "²A⁰";

			Interact = new MenuItem[] {
				new MenuItem(new string[] { "??" }, Info, TextUI.Parse, "get more info about the altar"),
				new MenuItem(new string[] { "Card", "C" }, AddCard, TextUI.Parse, "place a card by the altar"),
				new MenuItem(new string[] { "Take", "T" }, RemoveCard, TextUI.Parse, "take back a card that was placed"),
				new MenuItem(new string[] { "Offer", "O" }, Offer, TextUI.Parse, "offer the placed cards to the altar"),
				new MenuItem(new string[] { "Edit", "E" }, EditDeck, TextUI.Parse, "edit your deck"),
				new MenuItem(new string[] { "Leave", "L" }, Leave, TextUI.DummyParse, "step away from the altar")
			};

			ActivateAction = Act;
		}

		public bool Offer (int[] dum) {
			if (Offering.Count <= 0) { TextUI.PrintFormatted("Nothing is placed in front of the altar"); return false; }

			while (Offering.Count > 0) {
				int value = ShopValues.CardValue(Offering[0]);

				if (Offering[0].Made == 1) { // the altar does not like the player giving bakc cards that the altar created
					value /= 10;
				}

				if (value > Global.Run.AltarHighValue) {
					Global.Run.AltarHighValue = value;
					Global.Run.BestAltarCard = Offering[0];
				}

				Global.Run.AltarValue += value;
				Offering.RemoveAt(0);
			}

			TextUI.PrintFormatted("All cards offered fade from the spots where they were placed\n");

			int outcome = RollAltar();

			if (outcome == -1) {
				TextUI.PrintFormatted("⁴The light fades⁰");
				Glowing = false;
				Symbol = "⁴A⁰";
				Leave(null);
			} else {
				TextUI.PrintFormatted("²The light gets brighter!⁰");

				int state = Math.Min(Global.Run.AltarStage, Payouts.Length - 1);
				int[] outcomes = new int[PayChances[state].Length];
				for (int i = 0; i < PayChances[state].Length; i++) {
					outcomes[i] = PayChances[state][i];
				}

				bool rewardGiven = false;
				while (!rewardGiven) {
					switch (outcome) {
						case -1: rewardGiven = true; TextUI.PrintFormatted("Get shafted son"); break;
						case 0: rewardGiven = HealthReward();	break;
						case 1: rewardGiven = PackReward();		break;
						case 2: rewardGiven = UpgradeBest();	break;
						case 3: rewardGiven = SingleGearUp();	break;
						case 4: rewardGiven = AllGearUp();		break;
						case 5: rewardGiven = MaterialReward(); break;
					}

					if (!rewardGiven) {
						outcomes[outcome] = 0;
						outcome = Global.Roll(outcomes);
					}
				}

				Global.Run.AltarStage++;
				Global.Run.AltarValue = 0;

				TextUI.PrintFormatted("\n⁸The altar still shines⁰");
				TextUI.Wait();
			}

			return true;
		}

		private bool MaterialReward () {
			int amt = Global.Rand.Next((int) (Global.Run.AltarValue * 7.0 / 8.0), (int) (Global.Run.AltarValue * 9.0 / 8.0));
			TextUI.PrintFormatted(Global.Run.Player.Name + " gains " + amt + " Material!");
			Global.Run.Player.Material += amt;
			return true;
		}

		private bool HealthReward () {
			int amt = Global.Rand.Next(Global.Run.Player.MaxHealth / 8, Global.Run.Player.MaxHealth / 4);
			TextUI.PrintFormatted(Global.Run.Player.Name + " gains " + amt + " max health!");
			Global.Run.Player.MaxHealth += amt;
			Global.Run.Player.Health += amt;
			return true;
		}

		private bool PackReward () {
			int state = Math.Min(Global.Run.AltarStage, Packs.AltarTable.Length - 1);
			List<Card> pulls = Packs.AltarTable[state].Pull();
			TextUI.PrintFormatted("Cards appear in the spots in front of the altar!\n");
			List<string> pullstring = new List<string>();

			for (int i = 0; i < pulls.Count; i++) {
				pullstring.Add(pulls[i].ToString());
				pulls[i].ChangeMade(1);
				Global.Run.Player.Cards.AddCardTrunk(pulls[i]);
			}

			TextUI.PrintFormatted(string.Join('\n', TextUI.MakeTable(pullstring)));
			TextUI.PrintFormatted("\nAll cards are added to the trunk");
			return true;
		}

		private bool UpgradeBest () {
			// tier up
			// give a mod / upgrade a mod
			if (Global.Run.BestAltarCard == null) { return false; }
			int[] modvalues = { 1, 2, 3, 4 };
			int ModValue = modvalues[Global.Run.BestAltarCard.Mod];
			List<int> chances = new List<int>();

			for (int i = 0; i < modvalues.Length; i++) {
				if (modvalues[i] > ModValue) {
					chances.Add(11 - modvalues[i]);
				}
			}

			bool modup = false;
			if (chances.Count > 0) {
				// chance for mod upgrade
				int check = Global.Rand.Next(1, 11);
				if (ModValue < check) {
					// mitigate upgrade
					check = Global.Roll(chances.ToArray());
					Global.Run.BestAltarCard.ChangeMod(check);
					modup = true;
				}
			}
			
			int[] tierupchances = { (modup ? 6 : 0), 5, (modup ? 1 : 3), (modup ? 0 : 1) };
			int chosen = Global.Roll(tierupchances);
			Global.Run.BestAltarCard.ChangeTier(Global.Run.BestAltarCard.Tier + chosen);

			TextUI.PrintFormatted("A card appears in front of the altar\n");
			TextUI.PrintFormatted(Global.Run.BestAltarCard.ToString());
			TextUI.PrintFormatted("\nThe card is added to the trunk");

			Global.Run.BestAltarCard.ChangeMade(1);
			Global.Run.Player.Cards.AddCardTrunk(Global.Run.BestAltarCard);

			Global.Run.BestAltarCard = null;
			Global.Run.AltarHighValue = 0;

			return true;
		}

		private bool SingleGearUp () {
			List<Gear> pieces = new List<Gear>();
			for (int i = 0; i < Global.Run.Player.Gear.Count; i++) {
				if (Global.Run.Player.Gear[i].MaxUpgrades == 0 || Global.Run.Player.Gear[i].Upgrades < Global.Run.Player.Gear[i].MaxUpgrades) {
					pieces.Add(Global.Run.Player.Gear[i]);
				}
			}

			if (pieces.Count == 0) { return false; }
			int chosen = Global.Rand.Next(0, pieces.Count);
			pieces[chosen].Upgrade();
			
			return true;
		}

		private bool AllGearUp () {
			List<Gear> pieces = new List<Gear>();
			for (int i = 0; i < Global.Run.Player.Gear.Count; i++) {
				if (Global.Run.Player.Gear[i].MaxUpgrades == 0 || Global.Run.Player.Gear[i].Upgrades < Global.Run.Player.Gear[i].MaxUpgrades) {
					pieces.Add(Global.Run.Player.Gear[i]);
				}
			}

			if (pieces.Count == 0) { return false; }
			for (int i = 0; i < pieces.Count; i++) {
				pieces[i].Upgrade();
			}

			return true;
		}

		public int RollAltar () {
			int state = Math.Min(Global.Run.AltarStage, Payouts.Length - 1);
			if (Global.Run.AltarValue < MinPayouts[state]) { return -1; }

			int check = Global.Rand.Next(1, Payouts[state] + 1);
			if (check > Global.Run.AltarValue) { return -1; }

			int reward = Global.Roll(PayChances[state]);
			return reward;
		}

		public bool RemoveCard (int[] data) {
			if (data.Length < 1) { return false; }
			if (data[0] < 1 || data[0] > Offering.Count) { return false; }

			TextUI.PrintFormatted(Global.Run.Player.Name + " takes");
			TextUI.PrintFormatted(Offering[data[0] - 1].ToString() + "\nback");

			Global.Run.Player.Cards.AddCardTrunk(Offering[data[0] - 1]);
			Offering.RemoveAt(data[0] - 1);

			TextUI.Wait();

			Global.Run.Player.Cards.ValidateDeck();

			return true;
		}

		public bool AddCard(int[] dum) {
			if (Offering.Count >= MaxOffering) {
				TextUI.PrintFormatted("There are only three spots for cards in front of the altar");
				return false;
			}

			if (Global.Run.Player.Cards.TrunkCount() == 0) {
				TextUI.PrintFormatted("You have no cards in your trunk");
				TextUI.PrintFormatted("Try editing your deck first with \"Edit\" or \"E\"\n");
				return false;
			}

			Card chosen = Global.Run.Player.Cards.CardFromTrunk(true);

			if (chosen != null) {
				Offering.Add(chosen);

				Console.Clear();
				TextUI.PrintFormatted("Chosen Card");
				TextUI.PrintFormatted(Offering[Offering.Count - 1].ToString());
				TextUI.PrintFormatted("gets placed in front of the altar\n");

				TextUI.Wait();
			}

			return true;
		}

		public bool Info(int[] dum) {

			TextUI.PrintFormatted("You can offer cards to the altar over the course of the game to get various rewards");
			TextUI.PrintFormatted("The more the cards are worth the higher the chance for the altar to payout");
			TextUI.PrintFormatted("Cards given to the altar will be remembered from altar to altar");
			TextUI.Wait();

			return true;
		}

		public bool Leave(int[] index) {
			Interacting = false;

			TextUI.PrintFormatted("You step away from the altar");
			TextUI.Wait();

			Global.Run.Player.Cards.ValidateDeck();

			return true;
		}

		public void Act(int amt, int max) {
			if (!Glowing) {
				TextUI.PrintFormatted("The altar is no longer glowing");
				TextUI.Wait();
				return;
			}
			

			Interacting = true;
			while (Interacting) {
				PrintScene();
				TextUI.Prompt("What would you like to do?", Interact);
			}
		}

		public bool EditDeck(int[] dum) {
			return Global.Run.Player.Cards.EditDeck();
		}

		public void PrintScene () {
			Console.Clear();
			TextUI.PrintFormatted(Global.Run.Player.Name + " approaches the altar");

			string visual =   "         _        \n"
							+ "        /⁴_⁰\\       \n"
							+ "       /⁴/ \\⁰\\      \n"
							+ "      _\\⁴\\_/⁰/_     \n"
							+ "     /   __  \\    \n"
							+ "    /    ²_⁰ \\__\\   \n"
							+ "    |  _²/ \\⁰   |   \n"
							+ "    |__/²\\_/⁰   |   \n"
							+ "     \\ \\     /    \n"
							+ "     / /   \\ \\    \n"
							+ "    /_________\\   \n";

			TextUI.PrintFormatted(visual);
			
			TextUI.PrintFormatted("Current Offering");
			List<string> print = new List<string>();
			print.Add("");
			for (int i = 0; i < Offering.Count; i++) {
				print.Add(Offering[i].ToString() + "\n  " + (i + 1));
			}

			TextUI.PrintFormatted(string.Join('\n', TextUI.MakeTable(print)) + "\n");
		}
	}
}
