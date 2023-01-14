using Card_Test.Tables;
using System;
using System.Collections.Generic;
using System.Text;

namespace Card_Test.Map.Rooms {
	public class Inn : Room {
		private bool Open = true;
		private bool Shopping = false;
		private int[] Prices = new int[3];
		private MenuItem[] ShopMenu;
		private string[] names = { "The Sleeper Inn", "The Slept Inn", "Major Z's", "Mr. Magorium's Wonder Innporium" };
		private int chosenName;

		public Inn(Room replace, int cost) : base(replace) {
			Description = "This room seems to be some sort of Inn\nYou see a figure behind a counter, act to approach";
			RoomName = "Inn";
			Symbol = "⁸I⁰";
			chosenName = Global.Rand.Next(0, names.Length);

			RoomType = 5;
			ActivateAction = Shop;
			MaxConnections = 1;

			Prices[0] = cost / 4;
			Prices[1] = cost / 2;
			Prices[2] = cost;

			ShopMenu = new MenuItem[] {
				new MenuItem(new string[] { "Leave", "L" }, LeaveShop, TextUI.Parse, "leave the Inn"),
				new MenuItem(new string[] { "Stay", "S" }, StayInInn, TextUI.Parse, "stay and heal at the Inn"),
			};
		}

		public bool StayInInn (int[] index) {
			if (index.Length != 1) { return false; }
			if (index[0] <= 0 || index[0] > 3) { return false; }
			index[0]--;
			bool afford = false;

			if (Global.Run.Player.Material < Prices[index[0]]) {
				TextUI.PrintFormatted("\"You don\'t have enough Material to stay here for that long!\"\n");
			} else if (Global.Run.Player.Material < Prices[index[0]] * Global.Run.Players.Count) {
				TextUI.PrintFormatted("\"Prices are PER PERSON!\"\n");
			} else {
				afford = true;
				int cost = Prices[index[0]] * Global.Run.Players.Count;
				TextUI.PrintFormatted("You" + (Global.Run.Players.Count > 1 ? ", and your party" : "") + " choose to stay at the Inn");
				TextUI.PrintFormatted("You pay the Inn keeper");
				TextUI.PrintFormatted(Global.Run.Player.Name + " Material " + Global.Run.Player.Material + " -> " + (Global.Run.Player.Material - cost));

				Global.Run.Player.Material -= cost;

				int[] divisors = { 4, 2, 1 };

				int playerAmt = 0;
				foreach (Character unit in Global.Run.Players) {
					int amt = unit.MaxHealth / divisors[index[0]];
					int healed = unit.Heal(amt);
					TextUI.PrintFormatted("\n" + unit.Name + " heals " + healed + "\n  " + (unit.Health - healed) + " -> " + unit.Health);
					if (unit == Global.Run.Player) {
						playerAmt = healed;
					}
				}

				int x = (int) (100.0 * ((double) playerAmt / (double) Global.Run.Player.MaxHealth));
				int chance = (int) Math.Pow(x - 40, 3) / 2300;

				int roll = Global.Rand.Next(0, 100);

				if (roll < chance && !Global.Run.InnEvent) {
					TextUI.PrintFormatted("\nUpon waking up " + Global.Run.Player.Name + " feels something under their pillow\n");
					Card card = new Card(Types.Translate("dream"), 2, 1);
					TextUI.PrintFormatted(card.ToString());
					Global.Run.Player.Cards.AddCardTrunk(card);
					TextUI.PrintFormatted("\nThe card is added to your trunk");
					Global.Run.InnEvent = true;
				}

				TextUI.PrintFormatted("\n\"Thank you, come to " + names[chosenName] + " again!\"\n");
			}

			TextUI.Wait();

			Global.Run.Player.Cards.ValidateDeck();

			return afford;
		}

		public bool LeaveShop(int[] index) {
			Shopping = false;

			TextUI.PrintFormatted("\"Come again, if I'm still around\"");

			return true;
		}

		public void Shop(int amt, int max) {
			if (!Open) {
				TextUI.PrintFormatted("The Inn seems to have vanished");
			} else {
				Shopping = true;
				while (Shopping) {
					PrintScene();
					TextUI.Prompt("What would you like to do?", ShopMenu);
				}
			}

			TextUI.Wait();
		}

		public override void BossDefeated() {
			Open = false;
			Description = "Strangely, the Inn seems to have vanished\nMaybe defeating the boss did something here";
		}

		public void PrintScene() {
			Console.Clear();

			TextUI.PrintFormatted("You spot a board listing prices on the counter\n");

			TextUI.PrintFormatted("    Welcome to " + names[chosenName]);
			TextUI.PrintFormatted("1 : Quarter health " + Prices[0] + " Material");
			TextUI.PrintFormatted("2 : Half health " + Prices[1] + " Material");
			TextUI.PrintFormatted("3 : Full health " + Prices[2] + " Material");
			TextUI.PrintFormatted("    Prices are per person!");

			TextUI.PrintFormatted("\n" + Global.Run.Player.Name + " Material : " + Global.Run.Player.Material);

			foreach (Character unit in Global.Run.Players) {
				TextUI.PrintFormatted(unit.Name + " " + unit.HealthToString());
			}

			Console.WriteLine();
		}

	}
}
