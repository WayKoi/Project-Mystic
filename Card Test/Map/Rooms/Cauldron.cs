using Card_Test.Tables;
using Card_Test.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Card_Test.Map.Rooms {
	public class Cauldron : Room {
		private string ExtraDesc = "\nThe fire under it is lit";
		private bool Extinguished = false;
		private int MaterialAdded = 0, TotalAdded = 0;
		private MenuItem[] Interact;
		private bool Interacting = false;

		private int CauldronState; // 0 = red, 1 = yellow, 2 = green
		private List<Card> Dissolved = new List<Card>();
		private int maxDissovle = 3;
		private double[] MinChances = { 10, 5, 1, 0 };
		private double[] AddPerMaterial = { 0.5, 0.25, 0.1, 0.05 };

		private int[] Works = { 25, 75, 100 };
		private int Used = 0;
		private int[] ReBrew = { 25, 0 };

		public Cauldron(Room replace) : base(replace) {
			RoomType = 6;
			Description = "This room has a large cauldron sitting in the center";
			RoomName = "Cauldron";
			Symbol = "²σ⁰";

			CauldronState = Global.Rand.Next(0, 3);

			Interact = new MenuItem[] {
				new MenuItem(new string[] { "??" }, Info, TextUI.Parse, "get more info about the cauldron"),
				new MenuItem(new string[] { "Material", "M" }, AddMaterial, TextUI.Parse, "add material to the cauldron"),
				new MenuItem(new string[] { "Card", "C" }, AddCard, TextUI.Parse, "add a card in your trunk to the cauldron"),
				new MenuItem(new string[] { "Finish", "F" }, Finish, TextUI.Parse, "get the result of your efforts"),
				new MenuItem(new string[] { "Edit", "E" }, EditDeck, TextUI.Parse, "edit your deck"),
				new MenuItem(new string[] { "Leave", "L" }, Leave, TextUI.DummyParse, "step away from the cauldron")
			};

			ActivateAction = Act;
		}

		public bool AddCard (int[] dum) {
			if (Dissolved.Count >= maxDissovle) {
				TextUI.PrintFormatted("Dissolving another card would only taint the solution");
				return false;
			}

			if (CauldronState == 0) {
				TextUI.PrintFormatted("The cauldron is too unstable to add more to it\nTry adding material");
				return false;
			}

			if (Global.Run.Player.Cards.TrunkCount() == 0) { 
				TextUI.PrintFormatted("You have no cards in your trunk");
				TextUI.PrintFormatted("Try editing your deck first with \"Edit\" or \"E\"\n");
				return false;
			}

			Card chosen = Global.Run.Player.Cards.CardFromTrunk(true);

			if (chosen != null) {
				Dissolved.Add(chosen);
				Console.Clear();
				TextUI.PrintFormatted("Chosen Card");
				TextUI.PrintFormatted(Dissolved[Dissolved.Count - 1].ToString());
				TextUI.PrintFormatted("gets dissolved into the cauldron\n");

				MaterialAdded = 0;
				TextUI.PrintFormatted("The colour of the cauldron shifts!");
				CauldronState--;

				TextUI.Wait();
			}

			return true;
		}

		public bool Finish (int[] data) {
			int roll = Global.Rand.Next(1, 101);

			bool success = true;
			if (roll <= Works[CauldronState]) {
				// success
				Console.Clear();
				Console.ForegroundColor = ConsoleColor.Green;
				TextUI.PrintFormatted("Success!\n");
				Console.ResetColor();

				Card Result;
				switch (Dissolved.Count) {
					case 1: Result = OneAdded(); break;
					case 2: Result = MoreAdded(); break;
					case 3: Result = MoreAdded(); break;
					default: Result = NoneAdded(); break;
				}

				if (Result != null) {
					TextUI.PrintFormatted(Global.Run.Player.Name + " gets");
					TextUI.PrintFormatted(Result.ToString());
					TextUI.PrintFormatted("The card was added to the trunk");

					Global.Run.Player.Cards.AddCardTrunk(Result);

					TextUI.Wait();

					Global.Run.Player.Cards.ValidateDeck();
				}

			} else {
				// fail
				Console.ForegroundColor = ConsoleColor.Red;
				TextUI.PrintFormatted("\nSomething's wrong!\n");
				Console.ResetColor();

				int chosen = Global.Rand.Next(1, 101);

				if (chosen <= 20) {
					// blow up
					TextUI.PrintFormatted("The cauldron erupts, splashing on " + Global.Run.Player.Name);
					int damage = Global.Rand.Next(Global.Run.Player.Health / 4, Global.Run.Player.Health / 2);
					string healthb = Global.Run.Player.HealthToString();
					Global.Run.Player.Health -= damage;
					TextUI.PrintFormatted(Global.Run.Player.Name + " takes " + damage + " damage " + healthb + "->" + Global.Run.Player.HealthToString());
					TextUI.PrintFormatted("The fire under the cauldron goes out");
				} else {
					// grow cold
					TextUI.PrintFormatted("The cauldron grows cold");
					TextUI.PrintFormatted("All cards thrown in are lost\n");
					int materialSalv = Global.Rand.Next(0, TotalAdded * 3 / 4);
					TextUI.PrintFormatted(Global.Run.Player.Name + " was able to salvage " + materialSalv + " Material back from the cauldron");
					Global.Run.Player.Material += materialSalv;
				}

				success = false;
			}

			bool rebrew = Global.Rand.Next(0, 100) < ReBrew[Used];
			Used++;

			if (!rebrew || !success) {
				Symbol = "⁴σ⁰";
				ExtraDesc = "\nThe fire under it has gone out";
				Extinguished = true;
				Interacting = false;

				TextUI.PrintFormatted(Global.Run.Player.Name + " steps away from the now cold cauldron\n");
			} else {
				TextUI.PrintFormatted("¹The fire stays lit!⁰\nThe Cauldron can be used agian");
				MaterialAdded = 0;
				TotalAdded = 0;
				Dissolved.Clear();
				CauldronState = Global.Rand.Next(0, 2);
			}

			TextUI.Wait();

			return true;
		}

		public Card NoneAdded () {
			// open cauldron pack and give whatever it gives
			return Packs.Cauldron.Pull()[0];
		}

		public Card OneAdded () {
			// try and upgrade the tier of the card added
			Card upgrade = Dissolved[0];
			List<string> printout = new List<string>();
			printout.Add(upgrade.ToString());
			printout.Add("\n\n -> ");

			upgrade.ChangeTier(upgrade.Tier + 1);
			printout.Add(upgrade.ToString());

			TextUI.PrintFormatted(string.Join('\n', TextUI.MakeTable(printout, 0)));

			return upgrade;
		}

		public Card MoreAdded () {
			List<string> printout = new List<string>();

			while (Dissolved.Count > 1) {
				Card A = Dissolved[0];
				Card B = Dissolved[1];

				if (printout.Count <= 0) {
					printout.Add(A.ToString());
				}

				printout.Add("\n\n+");
				printout.Add(B.ToString());

				Dissolved.RemoveAt(0);
				Dissolved.RemoveAt(0);

				Card Result = Fusions.Fuse(A, B);
				Dissolved.Insert(0, Result);
			}

			printout.Add("\n\n->");
			printout.Add(Dissolved[0].ToString());

			TextUI.PrintFormatted(string.Join('\n', TextUI.MakeTable(printout)));

			return Dissolved[0];
		}

		public bool Leave (int[] index) {
			Interacting = false;

			TextUI.PrintFormatted("You step away from the cauldron");
			TextUI.Wait();

			return true;
		}

		public void Act (int amt, int max) {
			if (Extinguished) {
				TextUI.PrintFormatted("The Cauldron has stopped bubbling\n");
				TextUI.Wait();
				return;
			}

			Interacting = true;
			while (Interacting) {
				PrintScene();
				TextUI.Prompt("What would you like to do?", Interact);
			}
		}

		public bool AddMaterial (int[] data) {
			if (data.Length != 1) { return false; }
			if (data[0] <= 0) { return false; }
			if (data[0] > Global.Run.Player.Material) {
				TextUI.PrintFormatted(Global.Run.Player.Name + " does not have that much material");
				return false;
			}

			if (CauldronState == 2) {
				TextUI.PrintFormatted("The cauldron is stable, it would be a waste of material to add any more");
				TextUI.Wait();
				return true;
			}

			Global.Run.Player.Material -= data[0];

			TextUI.PrintFormatted(Global.Run.Player.Name + " adds material one by one,");

			int amt = data[0];
			bool colourshift = false;
			while (amt > 0 && !colourshift) {
				amt--;
				MaterialAdded++;
				TotalAdded++;
				Console.Write(data[0] - amt + " ");
				int roll = Global.Rand.Next(1, 101);
				if (roll <= MinChances[Dissolved.Count] + (AddPerMaterial[Dissolved.Count] * MaterialAdded)) {
					// colour changes
					CauldronState++;
					colourshift = true;
				}
			}

			Console.WriteLine();

			if (colourshift) {
				TextUI.PrintFormatted("The colour of the cauldron shifts!");
				TextUI.PrintFormatted(Global.Run.Player.Name + " stops adding material to the cauldron");
				TextUI.PrintFormatted(amt + " Material is left over");
				Global.Run.Player.Material += amt;
			} else {
				TextUI.PrintFormatted("The colour of the cauldron stays the same");
			}

			TextUI.Wait();

			return true;
		}

		public bool Info (int[] dum) {
			TextUI.PrintFormatted("The cauldron allows you to fuse cards by dissolving them in order");
			TextUI.PrintFormatted("You can dissolve up to three cards\n but be careful the more cards you dissolve the more material is needed to make the cauldron stable again");
			TextUI.PrintFormatted("The cauldrons stability is displayed by the colour of the solution");
			TextUI.PrintFormatted("Adding material makes the cauldron more stable\n");
			TextUI.PrintFormatted("Dissolving no cards grants a random card");
			TextUI.PrintFormatted("Dissolving a single card grants an upgrade to that card");
			TextUI.PrintFormatted("Dissolving multiple cards fuses them together in the order they were dissolved");
		
			TextUI.Wait();

			return true;
		}

		public bool EditDeck(int[] dum) {
			return Global.Run.Player.Cards.EditDeck();
		}

		public override string GetDescription() {
			return Description + ExtraDesc;
		}

		public void PrintScene () {
			Console.Clear();

			TextUI.PrintFormatted("You approach the cauldron\n");
			List<string> printouts = new List<string>();

			string build = "    C.\'.\'*B        \n" +
						   "     _C.*,\'.B_     \n" +
			               "     )     (     \n" +
			               "    / (_)   \\    \n" +
			               "    \\       /    \n" +
			               "    / --|-- \\    ";

			string cols = "³²¹";
			char Col = cols[CauldronState];
			build = build.Replace('C', Col).Replace('B', '⁰');

			printouts.Add(build);

			string add = "Material : " + TotalAdded + "\n";

			if (Dissolved != null) {
				List<string> combine = new List<string>();

				for (int i = 0; i < Dissolved.Count; i++) {
					combine.Add(Dissolved[i].ToString());
					if (i < Dissolved.Count - 1) {
						combine.Add("\n\n + ");
					}
				}	

				add += string.Join('\n', TextUI.MakeTable(combine, 0)) + "\n";
			} else {
				add += "No card added";
			}

			printouts.Add(add);

			string printout = String.Join('\n', TextUI.MakeTable(printouts));

			TextUI.PrintFormatted(printout + "\n\n" + Global.Run.Player.Name + " Material : " + Global.Run.Player.Material + "\n");
		}

	}
}
