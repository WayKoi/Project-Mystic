using Card_Test.Items;
using System;
using System.Collections.Generic;
using System.Text;
using Card_Test.Files;

namespace Card_Test.Tables {
	public static class DropTable {
		public static Drops BasicA = new Drops(3, 8);
		public static Drops BasicB = new Drops(6, 10);
		public static Drops BasicC = new Drops(8, 14);
		public static Drops BasicD = new Drops(10, 18);
		public static Drops BasicEA = new Drops(12, 22);
		public static Drops BasicEB = new Drops(18, 24);

		public static Drops BossA = new Drops(25, 35, null, One);
		public static Drops BossB = new Drops(35, 45, null, Two);
		public static Drops BossC = new Drops(50, 60, null, Three);
		public static Drops BossD = new Drops(100, 120, null, Four);
		public static Drops BossE = new Drops(180, 220);

		public static Drops RareA = new Drops(30, 50);
		public static Drops RareB = new Drops(50, 75);
		public static Drops RareC = new Drops(70, 80);
		public static Drops RareD = new Drops(75, 85);

		public static bool Drop (Player player, Character dropping, Drops drops) {
			if (drops == null) { return false; }
			int material = Global.Rand.Next(drops.MinMaterial, drops.MaxMaterial + 1);
			if (material > 0) {
				player.Material += material;
				TextUI.PrintFormatted(dropping.Name + " drops " + material + " Material");
			} else {
				TextUI.PrintFormatted(dropping.Name + " did not drop any Material");
			}

			return true;
		}

		public static void One (int[] args) {
			Console.Clear();
			TextUI.PrintFormatted("Congratulations! You have defeated the boss!");

			TextUI.PrintFormatted(Global.Run.Player.Name + " Gains a mana container!\n");
			TextUI.PrintFormatted("  Mana " + Global.Run.Player.MaxMana + " -> " + (Global.Run.Player.MaxMana + 1) + "\n");
			Global.Run.Player.MaxMana++;

			Boss(/*"Beetle"*/);
		}

		public static void Two(int[] args) {
			Console.Clear();
			TextUI.PrintFormatted("Congratulations! You have defeated the boss!");
			Boss(/*"Salamander"*/);

			Console.Clear();
			StoneTable.RollStone("Dune Gem", "DG", StoneTable.DuneGem);
		}

		public static void Three(int[] args) {
			Console.Clear();
			TextUI.PrintFormatted("Congratulations! You have defeated the boss!");

			Boss(/*"Mannequin"*/);

			Console.Clear();
			StoneTable.RollStone("Ivory Gem", "IG", StoneTable.IvoryGem);
		}

		public static void Four(int[] args) {
			Console.Clear();
			TextUI.PrintFormatted("Congratulations! You have defeated the boss!");

			Boss(/*"Demon"*/);

			Console.Clear();
			StoneTable.RollStone("Apocalypse Gem Stone", "AG", StoneTable.ApocalypseGemStone);

			/*Console.Clear();
			StoneTable.RollStone("Ivory Gem", "IG", StoneTable.IvoryGem);*/
		}

		public static void Boss (/*string pack*/) {
			foreach (Gear gear in Global.Run.Player.Gear) {
				if (gear.Name.Equals("Satchel")) {
					gear.Upgrade();
					break;
				}
			}

			TextUI.PrintFormatted(Global.Run.Player.Name + " is healed 50% max health");
			int amt = Global.Run.Player.MaxHealth / 2;
			string before = Global.Run.Player.HealthToString();
			Global.Run.Player.Heal(amt);
			TextUI.PrintFormatted("  " + before + " -> " + Global.Run.Player.HealthToString() + "\n");

			/*TextUI.PrintFormatted(Global.Run.Player.Name + " gets a pack!\n" + pack + "\n");
			List<Card> pulls = Reader.ReadPack(pack).Pull();

			List<string> cards = new List<string>();

			foreach (Card card in pulls) {
				cards.Add(card.ToString());
			}

			TextUI.PrintFormatted(Global.Run.Player.Name + " opens the pack");
			TextUI.PrintFormatted(String.Join('\n', TextUI.Combine(cards)));
			TextUI.PrintFormatted("All cards are added to the Trunk\n");

			Global.Run.Player.Cards.AddCardTrunk(pulls.ToArray());*/

			TextUI.Wait();

			Global.Run.Player.Cards.ValidateDeck();
		}
	}

	public class Drops {
		private int ActionChance;
		private Action<int[]> DropAction;
		private int[] ActionArgs;
		public int MinMaterial, MaxMaterial;

		public Drops (int minmat, int maxmat, int[] args = null, Action<int[]> dropaction = null, int actionchance = 100) {
			MinMaterial = minmat;
			MaxMaterial = maxmat;
			ActionArgs = args;
			DropAction = dropaction;
			ActionChance = actionchance;
		}

		public void RunAction () {
			if (DropAction == null) { return; }
			int check = Global.Rand.Next(0, 100);
			if (check < ActionChance) {
				DropAction(ActionArgs);
			}
		}
	}
}
