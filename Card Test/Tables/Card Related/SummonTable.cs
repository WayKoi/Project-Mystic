using Card_Test.Files;
using Card_Test.Base;
using System;
using System.Collections.Generic;
using System.Text;

namespace Card_Test.Tables {
	public static class SummonTable {
		
		private static STableEntry Default = new STableEntry("any", -1, new Summon("Default", "Default", 100, 3, false));

		private static STableEntry[] Table = new STableEntry[] {
			// special summons

			// type based summons

			new STableEntry("sprout", new int[] { 1, 2 },
				new Summon("Seedling",
					"seedling",
					new int[] { 30, 40 },
					new int[] { 2, 3 },
					false, 100, 100, 2
				)
			),

			new STableEntry("sprout", new int[] { 3, 4 },
				new Summon("Sprout",
					"sprout",
					new int[] { 50, 60 },
					3,
					false, 100, 100, 2
				)
			),

			new STableEntry("sprout", new int[] { 5, 6, 7, 8, 9 },
				new Summon("Weeping Vines",
					"weepv1",
					new int[] { 80, 90, 100, 105, 110 },
					3,
					false, 100, 100, 2
				)
			),

			new STableEntry("plant", new int[] { 1, 2, 3, 4 },
				new Summon[] {
					new Summon("Flower Bud",
						"flbud1",
						new int[] { 15, 20, 30, 50 },
						new int[] { 1, 2 },
						false, 100, 100, 2
					),

					new Summon("Ivy",
						"ivy",
						new int[] { 30, 35, 40, 50 },
						2,
						false, 100, 100, 2, 4
					),

					new	Summon ("Mushroom",
						"mush1",
						new int[] { 40, 45, 50, 55 },
						3,
						false, 100, 100, 2
					)
				}
			),

			new STableEntry("plant", new int[] { 5, 6, 7, 8, 9 },
				new Summon[] {
					new Summon("Pitcher Plant",
						"pitch1",
						new int[] { 100, 110, 120, 130, 140 },
						3,
						false, 100, 100, 2
					),

					new Summon("Venus Flytrap",
						"vfly1",
						new int[] { 60, 70, 80, 90 },
						new int[] { 3, 3, 4 },
						false, 100, 100, 2, 4
					),

					new Summon ("Cobra Lily",
						"cobralily1",
						new int[] { 40, 55, 70, 80 },
						4,
						false, 100, 100, 2
					)
				}
			),

			new STableEntry("shade", new int[] { 1, 2, 3, 4 },
				new Summon("Imp",
					new string[] { "imp1", "imp2", "imp3", "imp4" },
					new int[] { 30, 35, 40, 45 },
					2, false, 80, 80, 2
				)
			),

			new STableEntry("crystal", new int[] { 1, 2, 3, 4 },
				new Summon("Mineral Golem",
					new string[] { "mg1", "mg2", "mg3", "mg4" },
					new int[] { 30, 40, 50, 60 },
					2, false, 80, 80, 2
				)
			),

			new STableEntry("crystal", new int[] { 5, 6, 7, 8, 9 },
				new Summon("Animated Crystal",
					new string[] { "ac1", "ac2", "ac3", "ac4", "ac5" },
					new int[] { 100, 105, 110, 120, 140 },
					new int[] { 3, 3, 3, 4 },
					false, 80, 75, 2
				)
			),

			// Tier based summons

			// default summons
			new STableEntry("any", -1, 
				new Summon("[T] Elemental", 
				new string[] { "E1", "E2", "E3", "E4", "E5", "E6", "E7", "E8", "E9" }, // decks
				new int[] { 30, 40, 50, 60, 70, 80, 90, 100, 120 }, // health
				new int[] { 2, 2, 2, 3, 3, 3, 4 }) // mana
			),
		};
		

		public static List<CardAI> CreateSummon (Card sent) {
			// search through the table
			STableEntry entry = null;

			for (int i = 0; i < Table.Length; i++) {
				if (Table[i].Check(sent)) {
					entry = Table[i];
					break;
				}
			}

			if (entry == null) {
				entry = Default;
			}

			return entry.Generate(sent);
		}

	}

	public class STableEntry {
		private int[] Types, Tier; // for searching, -1 on either means any type / tier respectively
		private int Amount; // amount of summons created
		private List<Summon> Choices = new List<Summon>();

		public STableEntry (string[] types, int[] tier, Summon[] summons, int amount = 1) {
			Types = Tables.Types.Translate(types);
			Tier = tier;
			
			for (int i = 0; i < summons.Length; i++) {
				Choices.Add(summons[i]);
			}

			Amount = amount;
		}

		public STableEntry(string type, int[] tier, Summon[] summons, int amount = 1) : this (new string[] { type }, tier, summons, amount) { }
		public STableEntry(string type, int[] tier, Summon summon, int amount = 1) : this(new string[] { type }, tier, new Summon[] { summon }, amount) { }
		public STableEntry(string[] types, int[] tier, Summon summon, int amount = 1) : this(types, tier, new Summon[] { summon }, amount) { }
		public STableEntry(string type, int tier, Summon[] summons, int amount = 1) : this(new string[] { type }, new int[] { tier }, summons, amount) { }
		public STableEntry(string type, int tier, Summon summon, int amount = 1) : this(new string[] { type }, new int[] { tier }, new Summon[] { summon }, amount) { }
		public STableEntry(string[] types, int tier, Summon summon, int amount = 1) : this(types, new int[] { tier }, new Summon[] { summon }, amount) { }


		public bool Check (Card card) {
			bool found = false;
			for (int i = 0; i < Tier.Length; i++) {
				if (Tier[i] == -1 || Tier[i] == card.Tier) {
					found = true;
				}
			}
			
			if (found) {
				for (int i = 0; i < Types.Length; i++) { // type check
					if (Types[i] == -1 || card.Type == Types[i]) {
						return true;
					}
				}
			}

			return false;
		}

		public List<CardAI> Generate (Card card) {
			List<CardAI> ret = new List<CardAI>();
			Rollable[] chosen = Rollable.Roll(Choices.ToArray(), Amount).ToArray();

			int tier = 0;
			for (int i = 0; i < Tier.Length; i++) {
				if (Tier[i] == -1 || Tier[i] == card.Tier) {
					tier = i;
					break;
				}
			}

			for (int i = 0; i < Amount; i++) {
				// this generates summons at the summon tier specified, if the tier listed is -1 it uses the cards tier
				ret.Add((chosen[i] as Summon).Generate(card.Type, Tier[tier] == -1 ? card.Tier - card.Element.TierLim[0] : tier));
			}

			return ret;
		}
	}

	public class Summon : Rollable {
		private string Name; // [T] will be replaced by the type of the summon
		private string[] Decks; // name of the deck to load
		private bool ChangeDeckType; // this will determine if the deck is loaded as the summons type or unchanged
			// ex. = true and type = sand all cards in the deck loaded will be of type sand

		// AI vars
		private int Acc, Response, MaxPlay; // accuracy of plays and response rate
		private int[] Health, Mana;

		public Summon (string name, string[] decks, int[] health, int[] mana, bool changeType = true, int acc = 100, int res = 100, int maxplay = 1, int chance = 1, int maxrolls = 0) : base (chance, maxrolls) {
			Name = name;
			Decks = decks;

			ChangeDeckType = changeType;

			Acc = acc;
			Response = res;
			MaxPlay = maxplay;
			Health = health;
			Mana = mana;
		}

		public Summon(string name, string deck, int[] health, int[] mana, bool changeType = true, int acc = 100, int res = 100, int maxplay = 1, int chance = 1, int maxrolls = 0) : this (name, new string[] { deck }, health, mana, changeType, acc, res, maxplay, chance, maxrolls) { }
		public Summon(string name, string deck, int health, int[] mana, bool changeType = true, int acc = 100, int res = 100, int maxplay = 1, int chance = 1, int maxrolls = 0) : this(name, new string[] { deck }, new int[] { health }, mana, changeType, acc, res, maxplay, chance, maxrolls) { }
		public Summon(string name, string deck, int[] health, int mana, bool changeType = true, int acc = 100, int res = 100, int maxplay = 1, int chance = 1, int maxrolls = 0) : this(name, new string[] { deck }, health, new int[] { mana }, changeType, acc, res, maxplay, chance, maxrolls) { }
		public Summon(string name, string deck, int health, int mana, bool changeType = true, int acc = 100, int res = 100, int maxplay = 1, int chance = 1, int maxrolls = 0) : this(name, new string[] { deck }, new int[] { health }, new int[] { mana }, changeType, acc, res, maxplay, chance, maxrolls) { }
		public Summon(string name, string[] decks, int health, int[] mana, bool changeType = true, int acc = 100, int res = 100, int maxplay = 1, int chance = 1, int maxrolls = 0) : this(name, decks, new int[] { health }, mana, changeType, acc, res, maxplay, chance, maxrolls) { }
		public Summon(string name, string[] decks, int[] health, int mana, bool changeType = true, int acc = 100, int res = 100, int maxplay = 1, int chance = 1, int maxrolls = 0) : this(name, decks, health, new int[] { mana }, changeType, acc, res, maxplay, chance, maxrolls) { }
		public Summon(string name, string[] decks, int health, int mana, bool changeType = true, int acc = 100, int res = 100, int maxplay = 1, int chance = 1, int maxrolls = 0) : this(name, decks, new int[] { health }, new int[] { mana }, changeType, acc, res, maxplay, chance, maxrolls) { }

		public CardAI Generate (int Type, int Tier) {
			TDeck deck = Reader.ReadTDeck("Summon/" + Decks[Math.Min(Decks.Length - 1, Tier)], (ChangeDeckType ? Type : -1));
			string madeName = Name.Replace("[T]", Types.Search(Type).Name);
			return new CardAI(madeName, Health[Math.Min(Health.Length - 1, Tier)], Mana[Math.Min(Mana.Length - 1, Tier)], deck, null, Response, Acc, MaxPlay);
		}
	}
}
