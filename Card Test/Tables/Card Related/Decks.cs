using Sorting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Card_Test.Tables {
	public static class Decks {

		/*public static TDeck Template = new TDeck(def,
			new TCard[] {

			}, handsize
		);*/



		public static Deck FillDeck (TDeck template) {
			if (template == null) { return null; }

			Card def = template.Default.Convert();
			List<Card> contents = new List<Card>();
			List<Card> trunk = new List<Card>();

			for (int i = 0; i < template.Contents.Length; i++) {
				contents.Add(template.Contents[i].Convert());
			}

			for (int i = 0; i < template.Trunk.Length; i++) {
				trunk.Add(template.Trunk[i].Convert());
			}

			Deck gen = new Deck(def, contents.ToArray(), template.StartHandSize, template.DeckLim, template.TrunkLim);
			gen.AddCardTrunk(trunk.ToArray());

			return gen;
		}

	}

	public class TDeck {
		public TCard Default;
		public TCard[] Contents;
		public TCard[] Trunk;
		public int StartHandSize;

		public int DeckLim = -1, TrunkLim = -1;

		public TDeck (TCard def, TCard[] contents, TCard[] trunk, int starthand, int decklim = -1, int trunklim = -1) {
			Default = def;
			Contents = contents;
			Trunk = trunk;
			StartHandSize = starthand;

			DeckLim = decklim;
			TrunkLim = trunklim;
		}

		public string DeckToString () {
			Deck temp = Decks.FillDeck(this);
			return temp.DeckToString();
		}
	}

	public class TCard {
		public int Type, Tier, Mod, Sub, Made;

		public TCard (int type, int tier, int mod = 0, int sub = 0, int made = 0) {
			Type = type;
			Tier = tier;
			Mod = mod;
			Sub = sub;
			Made = made;
		}

		public Card Convert () {
			return new Card(Type, Tier, Mod, Sub, Made);
		}

		public override string ToString() {
			return (Convert()).ToString();
		}

		public string ToFileLine() {
			return Convert().ToFileLine();
		}
	}

	public class Deck {
		public Card Default { private set; get; }
		public List<Card> Content { private set; get; }
		private List<Card> Trunk = new List<Card>();
		public int StartHandSize { set; get; }
		private bool Editing = false;
		private MenuItem[] EditDeckMenu;

		public int DeckLim = -1, TrunkLim = -1;

		private Deck () {
			EditDeckMenu = new MenuItem[] {
				new MenuItem(new string[] { "Leave", "L" }, StopEditing, TextUI.Parse, "stop editing the deck"),
				new MenuItem(new string[] { "Remove", "R" }, RemoveCard, TextUI.Parse, "remove a card from the deck"),
				new MenuItem(new string[] { "Add", "A" }, AddCardToDeck, TextUI.Parse, "add a card from the trunk to the deck"),
				new MenuItem(new string[] { "RemoveTrunk", "RT" }, RemoveCardFromTrunk, TextUI.Parse, "remove a card from the trunk")
			};
		}

		public Deck (Card def, Card[] content, int startHandSize, int decklim = -1, int trunklim = -1) : this () {
			Default = def;
			Content = content.ToList();
			StartHandSize = startHandSize;

			DeckLim = decklim;
			TrunkLim = trunklim;

			Sort.BubbleSort(Content, Compare.Card);
		}

		public void SetDefault (Card def) {
			Default = def;
		}

		public List<string> ToFileLines () {
			List<string> lines = new List<string>();

			lines.Add(Default.ToFileLine());
			
			int amt = Content.Count;
			for (int i = 0; i < amt; i++) {
				lines.Add(Content[i].ToFileLine());
			}

			amt = Trunk.Count;

			if (amt > 0) {
				lines.Add("#");
				for (int i = 0; i < amt; i++) {
					lines.Add(Trunk[i].ToFileLine());
				}
			}

			return lines;
		}

		public string EditString () {
			List<string> combine = new List<string>();

			combine.Add(Default.ToString());
			combine.Add("   ");

			for (int i = 0; i < Content.Count; i++) {
				string ind = (i + 1).ToString();
				combine.Add(Content[i].ToString() + "\n " + (ind.Length > 1 ? "" : " ") + ind);
			}

			return StartHandSize + " " + DeckLim + " " + TrunkLim + "\n" +string.Join('\n', TextUI.MakeTable(combine, 0));
		}

		public void ValidateDeck () {
			if (DeckLim != -1 && Content.Count > DeckLim) { EditDeck(); }
			if (TrunkLim != -1 && Trunk.Count > TrunkLim) { EditDeck(); }
		}

		public void AddCard (Card card, bool sort = true) {
			Content.Add(card);
			if (sort) { Sort.BubbleSort(Content, Compare.Card); }
		}

		public void AddCard(Card[] card, bool sort = true) {
			Content.AddRange(card);
			if (sort) { Sort.BubbleSort(Content, Compare.Card); }
		}

		public void AddCardTrunk(Card card, bool sort = true) {
			Trunk.Add(card);
			if (sort) { Sort.BubbleSort(Trunk, Compare.Card); }
		}

		public void AddCardTrunk(Card[] card, bool sort = true) {
			Trunk.AddRange(card);
			if (sort) { Sort.BubbleSort(Trunk, Compare.Card); }
		}

		public string DeckToString() {
			string[] lines = new string[7];
			lines[0] = "Deck" + (DeckLim != -1 ? " " + Content.Count + "/" + DeckLim : "");

			for (int i = 0; i < Content.Count; i++) {
				lines[1] += " " + (i.ToString().Length <= 1 ? " " : "") + (i + 1) + "  ";

				string[] chop = Content[i].ToString().Split("\n");
				for (int ii = 0; ii < chop.Length; ii++) {
					lines[ii + 2] += chop[ii];
				}
			}

			if (Content.Count == 0) {
				lines[1] = "Empty";
			}

			return String.Join('\n', lines);
		}

		public string TrunkToString () {
			string[] lines = new string[7];
			lines[0] = "Trunk" + (TrunkLim != -1 ? " " + Trunk.Count + "/" + TrunkLim : ""); ;

			for (int i = 0; i < Trunk.Count; i++) {
				lines[1] += " " + (i.ToString().Length <= 1 ? " " : "") + (i + 1) + "  ";

				string[] chop = Trunk[i].ToString().Split("\n");
				for (int ii = 0; ii < chop.Length; ii++) {
					lines[ii + 2] += chop[ii];
				}
			}

			if (Trunk.Count == 0) {
				lines[1] = "Empty";
			}

			return String.Join('\n', lines);
		}

		public Card GetGhostCard () {
			Card ret;

			if (Content.Count == 0) {
				ret = new Card(Default);
			} else {
				 ret = new Card(Content[Global.Rand.Next(0, Content.Count)]);
			}

			return ret;
		}

		public int TrunkCount () {
			return Trunk.Count;
		}

		public Card CardFromTrunk (bool remove = false) {
			PrintTrunkChoose();
			int chosen = TextUI.Prompt("Which card do you choose? (0 to cancel)", 0, Trunk.Count);
			if (chosen == 0) { return null; }

			Card ret = Trunk[chosen - 1];

			if (remove) {
				Trunk.RemoveAt(chosen - 1);
			}

			return ret;
		}

		public void PrintTrunkChoose () {
			Console.Clear();
			TextUI.PrintFormatted("Choose a card from your trunk");
			TextUI.PrintFormatted(TrunkToString() + "\n");
		}

		public bool EditDeck () {
			Editing = true;
			while (Editing) {
				PrintDeckEdit();
				TextUI.Prompt("What would you like to do?", EditDeckMenu);
			}

			return true;
		}

		public bool RemoveCard (int[] data) {
			if (data.Length != 1) { return false; }
			if (data[0] <= 0 || data[0] > Content.Count) { return false; }
			data[0]--;

			TextUI.PrintFormatted("Removed\n" + Content[data[0]] + "\nFrom the deck");

			Trunk.Add(Content[data[0]]);
			Content.RemoveAt(data[0]);

			Sort.BubbleSort(Content, Compare.Card);
			Sort.BubbleSort(Trunk, Compare.Card);

			TextUI.Wait();

			return true;
		}

		public bool RemoveCardFromTrunk(int[] data) {
			if (data.Length != 1) { return false; }
			if (data[0] <= 0 || data[0] > Trunk.Count) { return false; }
			data[0]--;

			TextUI.PrintFormatted("Removed\n" + Trunk[data[0]] + "\nFrom the trunk");

			Trunk.Remove(Trunk[data[0]]);

			TextUI.Wait();

			return true;
		}

		public bool AddCardToDeck(int[] data) {
			if (data.Length != 1) { return false; }
			if (data[0] <= 0 || data[0] > Trunk.Count) { return false; }
			data[0]--;

			TextUI.PrintFormatted("Added\n" + Trunk[data[0]] + "\nTo the deck");

			Content.Add(Trunk[data[0]]);
			Trunk.RemoveAt(data[0]);

			Sort.BubbleSort(Content, Compare.Card);
			Sort.BubbleSort(Trunk, Compare.Card);

			TextUI.PrintFormatted("Press enter to continue");
			Console.ReadLine();

			return true;
		}

		public void RemoveCardEdit (int ind) {
			Content.RemoveAt(ind);
		}

		public int DeckSize () {
			return Content.Count;
		}

		public bool StopEditing (int[] data) {
			if ((DeckLim == -1 || Content.Count <= DeckLim) && (TrunkLim == -1 || Trunk.Count <= TrunkLim)) {
				Editing = false;
				return true;
			}
			
			if (TrunkLim != -1 && Trunk.Count > TrunkLim) {
				TextUI.PrintFormatted("Trunk is overfilled!");
			}

			if (DeckLim != -1 && Content.Count > DeckLim) {
				TextUI.PrintFormatted("Deck is overfilled!");
			}

			return false;
		}

		public void PrintDeckEdit () {
			Console.Clear();
			TextUI.PrintFormatted("Default Card\n" + Default + "\n");
			TextUI.PrintFormatted(DeckToString() + "\n");
			TextUI.PrintFormatted(TrunkToString() + "\n");
		}
	}
}
