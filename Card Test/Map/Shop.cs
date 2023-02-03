using Card_Test.Items;
using Card_Test.Tables;
using Sorting;
using System;
using System.Collections.Generic;
using System.Text;

namespace Card_Test.Map {
	public class Shop {
		public static string[] ShopType = {
			"Card", "Pack", "Gear", "Tailor", "Travelling"
		};

		public static int[,] AttitudeMults = {
			//     happy,        neutral,          stingy
			{ 90, 100, 110 }, { 90, 110, 100 }, { 100, 110, 90 }
			// buying min, buying max, selling multiplier
		};

		public static int[] DefaultTypeWeights = { 2, 3, 1 }, DefaultAttitudeWeights = { 1, 3, 1 };

		public int Type, Attitude;
		public List<ShopItem> Items = new List<ShopItem>();
		private MenuItem[] ShopMenu;
		public bool Shopping = false, Open = true;
		private int[] TypeWeights, AttitudeWeights;

		public Shop (int tier, int type = -1, int[] typeWeights = null) {
			ShopMenu = new MenuItem[] {
				new MenuItem(new string[] { "View", "V" }, ViewItem, TextUI.Parse, "view the details of an item on the shelf"),
				new MenuItem(new string[] { "Leave", "L" }, LeaveShop, TextUI.Parse, "leave the shop"),
				new MenuItem(new string[] { "Buy", "B" }, BuyItem, TextUI.Parse, "purchase something on the shelf"),
				new MenuItem(new string[] { "Sell", "S" }, SellCard, TextUI.Parse, "sell a card from your trunk"),
				new MenuItem(new string[] { "Edit", "E" }, EditDeck, TextUI.Parse, "edit your deck"),
			};

			AttitudeWeights = DefaultAttitudeWeights;

			TypeWeights = DefaultTypeWeights;
			if (typeWeights != null) {
				TypeWeights = typeWeights;
			}

			Attitude = Global.Roll(AttitudeWeights);

			Type = type;
			if (type == -1) { Type = Global.Roll(TypeWeights); }

			if (Type == 0) {
				Pack[][] pullfrom = { Packs.TableA, Packs.TableA, Packs.TableB, Packs.TableB };
				int[] tierAmt = { 3, 4, 3, 4 };

				tier = Math.Min(tier, pullfrom.Length - 1);
				List<Pack> forsale = Packs.RollPacks(pullfrom[tier], 2);

				for (int i = 0; i < forsale.Count; i++) {
					List<TCard> cards = forsale[i].PullTCards();
					for (int ii = 0; ii < cards.Count; ii++) {
						ShopItem item = new ShopCard(cards[ii]);
						Items.Add(item);
						item.Cost = (int)(item.Cost * Global.Rand.Next(AttitudeMults[Attitude, 0], AttitudeMults[Attitude, 1]) / 100.0);
					}
				}

			} else if (Type == 1) {
				Pack[][] pullfrom = { Packs.TableA, Packs.TableA, Packs.TableB, Packs.TableB };
				int[] tierAmt = { 3, 4, 3, 4 };

				tier = Math.Min(tier, pullfrom.Length - 1);
				List<Pack> forsale = Packs.RollPacks(pullfrom[tier], tierAmt[tier]);

				for (int i = 0; i < forsale.Count; i++) {
					ShopItem item = new ShopPack(forsale[i]);
					Items.Add(item);
					item.Cost = (int)(item.Cost * Global.Rand.Next(AttitudeMults[Attitude, 0], AttitudeMults[Attitude, 1]) / 100.0);
				}
			} else if (Type == 2) {
				if (Global.Run != null && Global.Run.Player != null) {
					for (int i = 0; i < Global.Run.Player.Gear.Count; i++) {
						if (Global.Run.Player.Gear[i].MaxUpgrades == 0 || Global.Run.Player.Gear[i].Upgrades < Global.Run.Player.Gear[i].MaxUpgrades) {
							ShopItem item = new ShopGear(i);
							Items.Add(item);
							item.Cost = (int)(item.Cost * Global.Rand.Next(AttitudeMults[Attitude, 0], AttitudeMults[Attitude, 1]) / 100.0);
						}
					}
				}
			}

			Sort.BubbleSort(Items, Compare.ShopItem);
		}

		public void StartShopping() {
			for (int i = 0; i < Items.Count; i++) {
				if (Items[i] is ShopGear && (Items[i] as ShopGear).Tier != Global.Run.Player.Gear[(Items[i] as ShopGear).Index].Upgrades) {
					(Items[i] as ShopGear).Tier = Global.Run.Player.Gear[(Items[i] as ShopGear).Index].Upgrades;
					Items[i].CalcCost();
					Items[i].Cost = (int)(Items[i].Cost * Global.Rand.Next(AttitudeMults[Attitude, 0], AttitudeMults[Attitude, 1]) / 100.0);
				}
			}

			if (!Open) {
				TextUI.PrintFormatted("It seems that the shopkeep isn\'t around, and neither are their wares");
			} else {
				Shopping = true;
				while (Shopping) {
					PrintScene();
					TextUI.Prompt("What would you like to do?", ShopMenu);
				}
			}

			TextUI.Wait();
		}

		public void PrintScene() {
			Console.Clear();

			List<string> printout = new List<string>();

			if (Items.Count > 0) {
				int itemtype = -1;
				List<string> sub = new List<string>();
				for (int i = 0; i < Items.Count; i++) {
					if (itemtype == -1) { itemtype = Items[i].Type; }
					if (i > 0 && itemtype != Items[i].Type) {
						printout.Add(ShopItem.TypeNames[itemtype] + "\n" + String.Join('\n', TextUI.MakeTable(sub)));
						sub.Clear();
						itemtype = Items[i].Type;
					}

					string inter = Items[i].ToString().Split('\n')[0];
					sub.Add(String.Format("{0," + inter.Length / 2 + "}", "(" + (i + 1) + ")") + new string(' ', (int) Math.Ceiling(inter.Length / 2.0)) + "\n" + Items[i]);
				}

				printout.Add(ShopItem.TypeNames[itemtype] + "\n" + String.Join('\n', TextUI.MakeTable(sub)));

				TextUI.PrintFormatted(ShopType[Type] + " Shop shelf");
				TextUI.PrintFormatted(String.Join('\n', printout));
			} else {
				TextUI.PrintFormatted("SOLD OUT!");
			}

			TextUI.PrintFormatted("\n" + Global.Run.Player.Name + " Material : " + Global.Run.Player.Material + "\n");
		}

		public bool EditDeck(int[] dum) {
			return Global.Run.Player.Cards.EditDeck();
		}

		public bool LeaveShop(int[] index) {
			Shopping = false;

			TextUI.PrintFormatted("\"Come again, if I'm still around\"");

			Global.Run.Player.Cards.ValidateDeck();

			return true;
		}

		public bool ViewItem(int[] data) {
			if (data == null || data.Length < 1) { return false; }
			if (data[0] < 1 || data[0] > Items.Count) { return false; }

			TextUI.PrintFormatted(Items[data[0] - 1].View() + "\n");
			TextUI.Wait();

			return true;
		}

		public bool BuyItem (int[] data) {
			if (data == null || data.Length < 1) { return false; }
			if (data[0] < 1 || data[0] > Items.Count) { return false; }
			data[0]--;

			if (Global.Run.Player.Material < Items[data[0]].Cost) {
				TextUI.PrintFormatted("\n\"You think I'm runnin\' a charity here? You can't afford that!\"");
				TextUI.Wait();
				return false;
			}

			Console.Clear();
			TextUI.PrintFormatted(Global.Run.Player.Name + " pays the shopkeep " + Items[data[0]].Cost);
			Global.Run.Player.Material -= Items[data[0]].Cost;
			TextUI.PrintFormatted("  " + Global.Run.Player.Name + " has " + Global.Run.Player.Material + " Material left");

			Items[data[0]].BuyAction();
			Items.RemoveAt(data[0]);

			TextUI.Wait();

			return true;
		}

		public bool SellCard (int[] data) {
			if (Global.Run.Player.Cards.TrunkCount() == 0) {
				TextUI.PrintFormatted("You have no cards in your trunk");
				TextUI.PrintFormatted("Try editing your deck first with \"Edit\" or \"E\"\n");
				TextUI.Wait();
				return true;
			}

			Card ToSell = Global.Run.Player.Cards.CardFromTrunk(true);

			if (ToSell != null) {
				TextUI.PrintFormatted(Global.Run.Player.Name + " shows\n" + ToSell + "\nto the shopkeeper\n");
				
				int value = (int) ((ShopValues.CardValue(ToSell) / 2) * (AttitudeMults[Attitude, 2] / 100.0));

				TextUI.PrintFormatted("\"I'll give ya " + value + " Material for that card, whattya say?\"");

				string promptreturn = TextUI.Prompt("Accept? (yes, y, no, n)", new string[] { "yes", "y", "no", "n" });
				if (promptreturn.StartsWith('y')) {
					TextUI.PrintFormatted("\"Thanks for your business\"");
					TextUI.PrintFormatted(Global.Run.Player.Name + " hands over the card\nThe shopkeeper gives " + value + " Material in return");
					Global.Run.Player.Material += value;
				} else {
					TextUI.PrintFormatted("\"No harm in asking\"");
					Global.Run.Player.Cards.AddCardTrunk(ToSell);
				}

				TextUI.Wait();
			}

			return true;
		}
	}

	public static class ShopValues {
		public static int CardValue (TCard card) {
			return CardValue(card.Convert());
		}

		public static int CardValue(Card card) {
			double[] modMults = { 0, 0.25, 0.5, 1 };
			double[] subMults = { 0, -0.5, 0.5, -0.2, 0.2, 1 };

			int cost = (Types.Search(card.Type).BaseDamage + Types.Search(card.Type).BaseHealing) / 2;

			if (cost == 0) {
				cost = (int) (10.0 * card.Tier / 2.0);
			}

			double mult = 1 + modMults[card.Mod] + subMults[card.Sub] + ((card.Tier - 1) * 0.15);

			return (int) (cost * mult) + (card.Element.Tier * 5);
		}

		public static int GearUpgradeCost (Gear gear) {
			int cost = (int) (10 * Math.Pow(gear.Upgrades, 2) + 50);

			return cost;
		}

		public static int PackValue (Pack pack) {
			int total = 0;
			int chancetotal = 0;

			foreach (PackE packE in pack.Contents) {
				total += CardValue(packE.Card) * packE.Chance;
				chancetotal += packE.Chance;
			}

			return (total / Math.Max(chancetotal, 1)) * pack.Size;
		}
	}

	public abstract class ShopItem {
		public static string[] TypeNames = {
			"Cards", "Packs", "Gear Upgrades"
		};

		// type = 0 is card, 1 = pack
		public int Cost, Type;

		public override string ToString() {
			return ItemToString() + "\n " + Cost;
		}

		public abstract void CalcCost ();
		public abstract string View ();
		public abstract void BuyAction ();
		public abstract string ItemToString();
	}

	public class ShopCard : ShopItem {
		public TCard Item;

		public ShopCard (TCard item) {
			Item = item;
			Type = 0;
			CalcCost();
		}

		public override void CalcCost() {
			Cost = ShopValues.CardValue(Item);
		}

		public override string View() {
			return Item.Convert().Details();
		}

		public override void BuyAction() {
			TextUI.PrintFormatted(Item.ToString());
			TextUI.PrintFormatted("gets added to your trunk");

			Global.Run.Player.Cards.AddCardTrunk(Item.Convert());
		}

		public override string ItemToString() {
			return Item.ToString();
		}
	}

	public class ShopPack : ShopItem {
		public Pack Item;
		
		public ShopPack (Pack toSell) {
			Item = toSell;
			Type = 1;
			CalcCost();
		}

		public override void CalcCost() {
			Cost = ShopValues.PackValue(Item);
		}

		public override string View() {
			return Item.PackContents();
		}

		public override void BuyAction() {
			List<Card> pulls = Item.Pull();
			List<string> printout = new List<string>();

			printout.Add(Item.ToString());

			List<string> cards = new List<string>();

			foreach (Card card in pulls) {
				cards.Add(card.ToString());
			}

			string build = "Contained\n\n";
			build += String.Join('\n', TextUI.Combine(cards));
			printout.Add(build);

			TextUI.PrintFormatted("\nYou open the pack");
			TextUI.PrintFormatted(String.Join('\n', TextUI.MakeTable(printout)));
			TextUI.PrintFormatted("All cards are added to your trunk\n");

			Global.Run.Player.Cards.AddCardTrunk(pulls.ToArray());
		}

		public override string ItemToString() {
			return Item.ToString();
		}
	}

	public class ShopGear : ShopItem {
		public int Index, Tier;

		public ShopGear(int toUpgrade) {
			Index = toUpgrade;
			Tier = Global.Run.Player.Gear[Index].Upgrades;
			Type = 2;
			CalcCost();
		}

		public override void CalcCost() {
			Cost = ShopValues.GearUpgradeCost(Global.Run.Player.Gear[Index]);
		}

		public override string View() {
			return Global.Run.Player.Gear[Index].ToString();
		}

		public override void BuyAction() {
			Global.Run.Player.Gear[Index].Upgrade();
		}

		public override string ItemToString() {
			return Global.Run.Player.Gear[Index].Name + "\n" + Global.Run.Player.Gear[Index].Visual;
		}
	}
}
