using Card_Test.Base;
using Card_Test.Files;
using Card_Test.Items;
using Card_Test.Tables;
using System;
using System.Collections.Generic;
using System.Text;

namespace Card_Test.Tables {
	public static class CharacterTable {
		public static CTableEntry[] Table = {
			new CTableEntry("Tango", 3, 100, 0, GearTable.Tango, new KeyPair[] { new KeyPair("Fire", 10), new KeyPair("Water", -10) }, null, "³Ω⁰"),
			new CTableEntry("Mia", 3, 120, 0, GearTable.Mia, new KeyPair[] { new KeyPair("Water", 15), new KeyPair("Fire", -10) }, null, "₀Ω⁰"),
			new CTableEntry("Rich", 2, 90, 20, GearTable.Rich, new KeyPair[] { new KeyPair("Water", 20), new KeyPair("Earth", 20) }, new KeyPair[] { new KeyPair("Physical", 20) }, "⁹Ω⁰" ),
			new CTableEntry("Marco", 4, 100, 0, GearTable.Marco, new KeyPair[] { new KeyPair("Magic", -50), new KeyPair("Physical", 50) }, null, "⁴Ω⁰" ),
			new CTableEntry("Wendy", 4, 80, 0, GearTable.Wendy, new KeyPair[] { new KeyPair("Wind", 10) }, null, "⁸Ω⁰" ),
			new CTableEntry("Nix", 3, 110, 0, GearTable.Nix, new KeyPair[] { new KeyPair("Plant", 20) }, null, "¹Ω⁰" ),
			new CTableEntry("Wyatt", 2, 70, 0, GearTable.Wyatt, new KeyPair[] { new KeyPair("Electric", 10), new KeyPair("Earth", -50) }, null, "²Ω⁰" ),
			new CTableEntry("Rod", 2, 120, 0, GearTable.Rod, new KeyPair[] { new KeyPair("Earth", 10), new KeyPair("Electric", -50) }, null, "₂Ω⁰" )
		};

		public static Player CreateCharacter (CTableEntry entry) {
			Player ret = new Player(entry.Name, entry.StartHealth, entry.StartMana, Reader.ReadTDeck(entry.Name), entry.StartMaterial, entry.Token);

			for (int i = 0; i < entry.Gear.Length; i++) {
				ret.Gear.Add(TGear.Generate(Reader.ReadTGear(entry.Gear[i]), ret));
			}

			if (entry.Affinity != null) {
				for (int i = 0; i < entry.Affinity.GetLength(0); i++) {
					ret.ChangeAffinity(entry.Affinity[i]);
				}
			}

			if (entry.Resistances != null) {
				for (int i = 0; i < entry.Resistances.GetLength(0); i++) {
					ret.ChangeResistance(entry.Resistances[i]);
				}
			}

			return ret;
		}
	}

	public class CTableEntry {
		public string Name;
		public string[] Gear;
		public int StartMana, StartHealth, StartMaterial;
		public KeyPair[] Affinity, Resistances;
		public string Token;

		public CTableEntry (string name, int startMana, int startHealth, int startmaterial, string[] startinggear, KeyPair[] affinity = null, KeyPair[] resistances = null, string token = "⁰Ω⁰") {
			Name = name;
			StartMana = startMana;
			StartHealth = startHealth;
			Gear = startinggear;
			Affinity = affinity;
			Resistances = resistances;
			StartMaterial = startmaterial;
			Token = token;
		}

		public override string ToString () {
			int colHei = Math.Max(Resistances != null ? Resistances.GetLength(0) : 1, Affinity != null ? Affinity.GetLength(0) : 1);
			string[,] cols = new string[2, colHei];

			if (Affinity != null) {
				for (int i = 0; i < Affinity.GetLength(0); i++) {
					// Affinity[i, 0]
					// Affinity[i, 1]
					cols[0, i] = Affinity[i].Name + "   " + (Affinity[i].Amount > 0 ? "+" : "") + Affinity[i].Amount + "%";
				}
			} else {
				cols[0, 0] = "None";
			}

			if (Resistances != null) {
				for (int i = 0; i < Resistances.GetLength(0); i++) {
					// Affinity[i, 0]
					// Affinity[i, 1]
					cols[1, i] = Resistances[i].Name + "   " + (Resistances[i].Amount > 0 ? "+" : "") + Resistances[i].Amount + "%";
				}
			} else {
				cols[1, 0] = "None";
			}

			List<string> table = TextUI.MakeTable(new string[] { "Affinities", "Resistances" }, cols);

			string build = String.Format("{0," + (-1 * table[0].Length) + "}", Name) + "\n";
			build += String.Format("{0," + (-1 * table[0].Length) + "}", StartMana.ToString() + " Mana") + "\n";
			build += String.Format("{0," + (-1 * table[0].Length) + "}", StartHealth.ToString() + " Health") + "\n";
			build += String.Format("{0," + (-1 * table[0].Length) + "}", StartMaterial.ToString() + " Material") + "\n";

			return build + "\n" + String.Join('\n', table);
		}
	}
}
