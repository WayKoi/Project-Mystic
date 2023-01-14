using Card_Test.Files;
using Card_Test.Base;
using Sorting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Card_Test.Tables {
	public static class Packs {
		// chance, cost, size
		public static Pack[] TableA;
		public static Pack[] TableB;
		public static Pack[] AltarTable;
		public static Pack Cauldron;

		static Packs () {
			TableA = new Pack[] {
				Reader.ReadPack("Drop"),
				Reader.ReadPack("Char"),
				Reader.ReadPack("Gust"),
				Reader.ReadPack("Clay"),
				Reader.ReadPack("ESNA")
			};

			TableB = new Pack[] {
				Reader.ReadPack("Fire"),
				Reader.ReadPack("Water"),
				Reader.ReadPack("Wind"),
				Reader.ReadPack("Earth"),
				Reader.ReadPack("Heal"),
				Reader.ReadPack("Spark"),
				Reader.ReadPack("ESNB")
			};

			AltarTable = new Pack[] {
				Reader.ReadPack("AltA"),
				Reader.ReadPack("AltB")
			};

			Cauldron = Reader.ReadPack("CauldronA");
		}

		public static List<Pack> RollPacks (Pack[] choices, int amt) {
			List<Rollable> packs = new List<Rollable>();
			
			for (int i = 0; i < choices.Length; i++) {
				packs.Add(choices[i]);
			}

			List<Pack> chosen = new List<Pack>();
			List<Rollable> rolled = Rollable.Roll(packs, amt);

			int rolledAmt = rolled.Count;
			for (int i = 0; i < rolledAmt; i++) {
				chosen.Add((Pack) rolled[i]);
			}

			return chosen;
		}
	}

	public class Pack : Rollable {
		public string Name, Symbol, SubSymbol;
		public List<Rollable> Contents = new List<Rollable>();
		public int Size;

		public Pack (string name, string symbol, string subsymbol, int chance, int size, List<PackE> contents, int max = 0) : base (chance, max) {
			Name = name;
			Symbol = symbol;
			SubSymbol = subsymbol;
			Contents.AddRange(contents);
			Size = size;
		}

		public Pack(string name, string symbol, string subsymbol, int chance, int size, PackE[] contents, int max = 0) : this (name, symbol, subsymbol, chance, size, contents.ToList(), max) { }

		public List<Card> Pull () {
			List<Card> Pulls = new List<Card>();
			List<TCard> templates = PullTCards();

			int templateamt = templates.Count;
			for (int i = 0; i < templateamt; i++) {
				Pulls.Add(templates[i].Convert());
			}

			return Pulls;
		}

		public void AddPackE(PackE add) {
			Contents.Add(add);
			Sort.BubbleSort(Contents, Compare.PackE);
		}

		public List<TCard> PullTCards() {
			List<TCard> Pulls = new List<TCard>();
			List<Rollable> ret = Rollable.Roll(Contents, Size);

			int retAmt = ret.Count;
			for (int i = 0; i < retAmt; i++) {
				Pulls.Add(((PackE)ret[i]).Card);
			}

			ResetRolls();
			return Pulls;
		}

		private void ResetRolls() {
			for (int i = 0; i < Contents.Count; i++) {
				Contents[i].ResetCount();
			}
		}

		public override string ToString() {
			// ┌ ┐ └ ┘ │ ─ ╒ ═ ╕ ╤ ┤ ├ ┴ ╘ ╧ ╛
			string build = "";

			//build += "╒╤╤╤╕\n";
			//build += "├┴┴┴┤\n";
			//build += "│   │\n";
			//build += "│   │\n";
			//build += "│   │\n";
			//build += "├┬┬┬┤\n";
			//build += "╘╧╧╧╛\n";

			build += "╒╤╤╤╕\n";
			build += "├┴┴┴┤\n";
			build += "│   │\n";
			build += "│" + Symbol + "│\n";
			build += "│" + Size + " " + SubSymbol + "│\n";
			build += "├┬┬┬┤\n";
			build += "╘╧╧╧╛";

			return build;
		}

		public List<string> ToFile() {
			List<string> lines = new List<string>();

			lines.Add(Name);
			lines.Add(Symbol);
			lines.Add(SubSymbol);
			lines.Add(Chance + " " + Size + (MaxRolls == 0 ? "" : " " + MaxRolls));

			for (int i = 0; i < Contents.Count; i++) {
				lines.Add(((PackE) Contents[i]).ToFileLine());
			}

			return lines;
        }

		public string PackContents () {
			List<string> cards = new List<string>();

			foreach (PackE e in Contents) {
				cards.Add(e.Card.ToString());
			}

			return Name + " Contains " + Size + " cards from\n" + String.Join('\n', TextUI.Combine(cards));
		}

		public List<TCard> GetPackCards () {
			List<TCard> cards = new List<TCard>();

			for (int i = 0; i < Contents.Count; i++) {
				cards.Add(((PackE) Contents[i]).Card);
			}

			return cards;
		}

		public string EditString() {
			List<string> cards = new List<string>();

			cards.Add(ToString() + "\n Max:");
			for (int i = 0; i < Contents.Count; i++) {
				cards.Add(((i + 1) < 10 ? "  " : " ") + (i + 1).ToString() + "  \n" + Contents[i].ToString());
			}

			List<string> ret = new List<string>();

			ret.Add("\"" + Name + "\" \"" + Symbol + "\" \"" + SubSymbol + "\"");
			ret.Add(Chance + " " + Size + " " + MaxRolls);
			ret.Add(String.Join('\n', TextUI.MakeTable(cards, 0)));

			return String.Join('\n', ret);
		}
	}

	public class PackE : Rollable {
		public TCard Card;

		public PackE (TCard card, int chance, int max = 0) : base (chance, max) {
			Card = card;
		}

		public string ToFileLine() {
			return Card.ToFileLine() + ":" + Chance + (MaxRolls == 0 ? "" : " " + MaxRolls);
		}

        public override string ToString() {
			return Card.ToString() + "\n " + Chance.ToString() + "\n " + MaxRolls;

		}
    }
}
