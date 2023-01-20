using Card_Test.Tables;
using Card_Test.Utilities;
using System;
using System.Collections.Generic;
using System.Text;
using Card_Test.Base;
using Card_Test.Items;

namespace Card_Test {
	public class Character {
		public string Name { get; protected set; }
		public int Health, MaxHealth, Mana, MaxMana;

		public int FusionCounters = 0, MaxFusion = 0;
		public int SideCastCounters = 0, MaxSide = 0;
		public int MultiCastSlots = 0, MaxMulti = 0;

		public List<Card> Hand = new List<Card>();
		public List<int> Shuffled = new List<int>();

		public Deck Cards;

		public Plan Plan;
		public bool PlanVisible = false;
		
		protected Random Rand = new Random();

		// these are straight mulitpliers so be careful
		protected List<int> Resistances = new List<int>();
		protected List<int> Affinity = new List<int>();
		public double ReactAdd = 0;

		private Character (string name, int maxhealth, int maxmana) {
			Name = name;
			MaxHealth = maxhealth;
			Health = MaxHealth;

			MaxMana = maxmana;
			Mana = MaxMana;

			Plan = new Plan(this);

			for (int i = 0; i < BaseTypes.TableLength(); i++) {
				Resistances.Add(0);
				Affinity.Add(100);
			}
		}

		public Character (string name, int maxhealth, int maxmana, TDeck deck = null) : this(name, maxhealth, maxmana) {
			Cards = Decks.FillDeck(deck);
		}

		public Character(string name, int maxhealth, int maxmana, Deck deck = null) : this (name, maxhealth, maxmana) {
			Cards = deck;
		}

		public void SetResistance (int ind, int value) {
			if (ind > Affinity.Count || ind < 0) { return; }
			Resistances[ind] = value;
		}

		public void ChangeResistance (int ind, int change) {
			if (ind > Affinity.Count || ind < 0) { return; }
			Resistances[ind] += change;
		}

		public void SetAffinity (int ind, int value) {
			if (ind > Affinity.Count || ind < 0) { return; }
			Affinity[ind] = value;
		}

		public void ChangeAffinity (int ind, int change) {
			if (ind > Affinity.Count || ind < 0) { return; }
			Affinity[ind] += change;
		}

		public virtual double GetAffinity (CardType type) {
			int affinity = 100;

			foreach (int basetype in type.BaseTypes) {
				affinity += Affinity[basetype] - 100;
			}

			return affinity / 100.0;
		}

		public int GetAffinity (int BaseType) {
			if (BaseType > Affinity.Count || BaseType < 0) { return 0; }
			return Affinity[BaseType];
		}

		public virtual double GetResistance (CardType type) {
			double resistance = 0;

			foreach (int basetype in type.BaseTypes) {
				resistance += Resistances[basetype];
			}

			return (100 + (resistance * -1)) / 100.0;
		}

		public virtual double GetReactionMult (Reaction react) {
			if (react.Mult < 1) {
				return Math.Max(react.Mult - ReactAdd, 0);
			}
			return Math.Max(react.Mult + ReactAdd, 1);
		}

		public void AddCard (Card card) {
			Cards.AddCard(card);
		}

		public void AddCard (Card[] cards) {
			foreach (Card card in cards) {
				AddCard(card);
			}
		}

		public void Reset() {
			Health = MaxHealth;
		}

		public string HandToString() {
			string[] lines = new string[6];

			lines[5] = "";

			for (int i = 0; i < Hand.Count; i++) {
				lines[5] += " " + ((i + 1) < 10 ? " " : "") + (i + 1) + "  ";
			}

			for (int i = 0; i < Hand.Count; i++) {
				string[] chop = Hand[i].ToString().Split("\n");
				for (int ii = 0; ii < chop.Length; ii++) {
					lines[ii] += chop[ii];
				}
			}

			return String.Join('\n', lines);
		}

		public void RefreshDeck () {
			ClearHand();
			Shuffled.Clear();

			List<int> Thick = new List<int>();
			List<int> Thin = new List<int>();
			List<int> Regular = new List<int>();

			int hind = SubMods.Translate("thick"), tind = SubMods.Translate("thin");
			int decksize = Cards.Content.Count;
			for (int i = 0; i < decksize; i++) {
				if (Cards.Content[i].Sub == hind) {
					Thick.Add(i);
				} else if (Cards.Content[i].Sub == tind) {
					Thin.Add(i);
				} else {
					Regular.Add(i);
				}
			}

			int regsize = Regular.Count;
			while (regsize > 0) {
				int chosen = Global.Rand.Next(0, regsize);

				if (regsize % 2 == 0) {
					Thick.Add(Regular[chosen]);
				} else {
					Thin.Add(Regular[chosen]);
				}

				Regular.RemoveAt(chosen);
				regsize--;
			}

			int thinsize = Thin.Count;
			while (thinsize > 0) {
				int chosen = Global.Rand.Next(0, thinsize);
				Shuffled.Add(Thin[chosen]);
				Thin.RemoveAt(chosen);
				thinsize--;
			}

			int heavysize = Thick.Count;
			while (heavysize > 0) {
				int chosen = Global.Rand.Next(0, heavysize);
				Shuffled.Add(Thick[chosen]);
				Thick.RemoveAt(chosen);
				heavysize--;
			}
		}

		public void ClearHand () {
			Hand.Clear();
		}

		public void DrawCard (int amount = 1) {
			while (amount > 0) {
				if (Shuffled.Count == 0) { 
					Hand.Add(new Card(Cards.Default));
				} else {
					Hand.Add(new Card(Cards.Content[Shuffled[0]]));
					Shuffled.RemoveAt(0);
				}

				amount--;
			}
		}

		public int TakeDamage (int amount, int type) {
			int damage = (int) (amount * GetResistance(Types.Search(type)));
			Health -= damage;
			Health = Math.Max(Health, 0);

			return damage;
		}

		public int Heal (int amount, int type = 0) {
			if (!HasHealth()) { return 0; }

			amount = (int) (amount * GetResistance(Types.Search(type)));
			int orighealth = Health;
			Health += amount;
			Health = Math.Min(Health, MaxHealth);
			amount = Health - orighealth;

			return amount;
		}

		public bool HasHealth () {
			return Health > 0;
		}

		public string ManaToString () {
			string full = new string('O', Mana);
			if (full.Length > 0) { full = "⁵" + full + "⁰"; }

			return "Mana " + full + new string('.', MaxMana - Mana);
		}

		public string FusionsToString() {
			if (MaxFusion == 0) { return ""; }

			string full = new string('▲', FusionCounters);
			if (full.Length > 0) { full = "⁶" + full + "⁰"; }

			return "Fusion " + full + new string('.', MaxFusion - FusionCounters);
		}

		public string SidesToString() {
			if (MaxSide == 0) { return ""; }

			string full = new string('<', SideCastCounters);
			if (full.Length > 0) { full = "₃" + full + "⁰"; }

			return "Side " + full + new string('.', MaxSide - SideCastCounters);
		}

		public string MultiToString() {
			if (MaxMulti == 0 || MultiCastSlots == 0) { return ""; }

			List<string> multis = new List<string>();
			for (int i = 0; i < MultiCastSlots; i++) {
				multis.Add(new MultiPlan(null).ToString());
			}

			return string.Join('\n', TextUI.MakeTable(multis, 0));
		}

		public string HealthToString () {
			return Health.ToString() + "\\" + MaxHealth.ToString(); 
		}

		public override string ToString() {
			return Name + " " + HealthToString();
		}
	}
}
