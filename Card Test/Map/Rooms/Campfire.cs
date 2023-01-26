using System;
using System.Collections.Generic;
using System.Text;

namespace Card_Test.Map.Rooms {
	public class Campfire : Room {
		private string ExtraDesc = "\nThe fire is lit";

		public Campfire (Room replace) : base (replace) {
			Description = "This room has a fire pit in the middle\nSitting by a lit fire will restore your parties health";
			RoomName = "campfire";
			Symbol = "²δ⁰";

			MaxActivate = Global.Rand.Next(1, 3);
			RoomType = 3;
			ActivateAction = UseCampfire;
		}

		private void UseCampfire (int uses, int max) {
			if (uses < max) {
				int healamt = Global.Rand.Next(10, 25);
				TextUI.PrintFormatted("You" + (Global.Run.Players.Count > 1 ? ", and your party" : "") + " sit by the campfire");

				foreach (Character unit in Global.Run.Players) {
					int amt = unit.Heal((int)(Math.Min(unit.MaxHealth, 160) / 100.0 * healamt));
					TextUI.PrintFormatted(unit.Name + " Heals " + amt + " " + unit.HealthToString());
				}

				if (uses + 1 == max) {
					ExtraDesc = "\nThe fire seems to have gone out";
					Symbol = "⁴μ⁰";
					Console.ForegroundColor = ConsoleColor.Red;
					TextUI.PrintFormatted("The fire has gone out!");
					Console.ResetColor();
				}
			} else {
				TextUI.PrintFormatted("The fire has gone out!");
			}

			TextUI.PrintFormatted("Press enter to continue");
			Console.ReadLine();
		}

		public override string GetDescription () {
			return Description + ExtraDesc;
		}
	}
}
