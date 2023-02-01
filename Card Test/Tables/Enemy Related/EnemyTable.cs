using System;
using System.Collections.Generic;
using System.Text;
using Card_Test.Base;

namespace Card_Test.Tables {
	public static class EnemyTable {
		// floor 1
		public static AIEntry Wolf = new AIEntry("Wolf", 40, 2, "PhysA", DropTable.BasicA, 50, 75, 2);
		public static AIEntry Rat = new AIEntry("Rat", 30, 3, "PhysB", DropTable.BasicA, 40, 95, 2);
		public static AIEntry Spider = new AIEntry("Spider", 60, 2, "PhysA", DropTable.BasicA, 60, 50, 1);

		public static AIEntry Beetle = new AIEntry("Beetle", 100, 2, "Beetle", DropTable.BossA, 90, 80, 2);
		public static AIEntry Grub = new AIEntry("Grub", 15, 1, "Grub", DropTable.BasicA, 70, 100, 1);

		// Sandy Caverns and Oasis
		public static AIEntry SandWizard = new AIEntry("Sand Wizard", 120, 4, "SandWizard", DropTable.RareA, 20, 100, 2);
		
		public static AIEntry Salamander = new AIEntry("Salamander", 120, 3, "Salamander", DropTable.BossB, 70, 80, 1);
		public static AIEntry Worm = new AIEntry("Worm", 40, 1, "Worm", DropTable.BasicB, 70, 100);

		public static AIEntry Snake = new AIEntry("Snake", 50, 2, "Snake", DropTable.BasicB, 70, 60);
		public static AIEntry Scorpion = new AIEntry("Scorpion", 45, 4, "Scorpion", DropTable.BasicB, 60, 80, 1);

		public static AIEntry Hyena = new AIEntry("Hyena", 50, 3, "PhysB", DropTable.BasicB, 60, 80, 2);
		public static AIEntry DragonFly = new AIEntry("Dragon Fly", 60, 2, "dragonfly", DropTable.BasicB, 100, 50, 1);
		public static AIEntry Crocodile = new AIEntry("Crocodile", 100, 4, "crocodile", DropTable.RareA, 40, 80, 2);

		public static AIEntry Camel = new AIEntry("Camel", 160, 2, "camel", DropTable.BossB, 60, 60, 2);


		public static AIEntry MudMonster = new AIEntry("Mud Monster", 40, 2, "MudMonster", DropTable.BasicB, 75, 95, 2);

		// floor 3
		public static AIEntry MHead = new AIEntry("Mannequin Head", 30, 1, "MHead", DropTable.BasicC, 40, 100);
		public static AIEntry MLeg = new AIEntry("Mannequin Leg", 50, 3, "MLegArm", DropTable.BasicC, 40, 50);
		public static AIEntry MArm = new AIEntry("Mannequin Arm", 35, 3, "MLegArm", DropTable.BasicC, 50, 60);
		public static AIEntry BuzzSaw = new AIEntry("Living Buzz Saw", 100, 6, "BuzzSaw", DropTable.RareB, 35, 100);

		public static AIEntry MBody = new AIEntry("Mannequin Body", 100, 2, "MBody", DropTable.BossC, 70, 80, 2);

		public static AIEntry Boar = new AIEntry("Boar", 50, 3, "boar", DropTable.BasicC, 50, 50, 2);
		public static AIEntry GiantSpider = new AIEntry("Giant Spider", 80, 3, "giantspider", DropTable.BasicC, 50, 70, 2);
		public static AIEntry Mice = new AIEntry("Mice", 100, 4, "mice", DropTable.BasicC, 40, 100, 4);
		public static AIEntry SproutWizard = new AIEntry("Sprout Wizard", 120, 3, "sproutwizard", DropTable.RareB, 20, 100, 2);
		
		public static AIEntry Mammoth = new AIEntry("Mammoth", 250, 4, "mammoth", DropTable.BossC, 70, 100, 2);

		// floor 4
		public static AIEntry Vampire = new AIEntry("Vampire", 120, 6, "Vampire", DropTable.RareC, 30, 100, 2);
		public static AIEntry Zombie = new AIEntry("Zombie", 80, 4, "Zombie", DropTable.BasicD, 70, 50, 2);
		public static AIEntry Skeleton = new AIEntry("Skeleton", 50, 3, "Skeleton", DropTable.BasicD, 80, 95);
		public static AIEntry BloodBat = new AIEntry("Blood Bat", 45, 6, "BloodBat", DropTable.BasicD, 75, 50, 2);
		public static AIEntry Banshee = new AIEntry("Banshee", 45, 6, "Banshee", DropTable.BasicD, 30, 90, 2);
		public static AIEntry Poltergeist = new AIEntry("Poltergeist", 50, 3, "Poltergeist", DropTable.BasicD, 30, 90, 2);

		public static AIEntry Demon = new AIEntry("Demon", 300, 4, "Demon", DropTable.BossD, 80, 80, 2);

		// floor 5
		public static AIEntry Mimic = new AIEntry("Mimic", 200, 4, "Mimic", DropTable.RareD, 60, 80, 2);
		public static AIEntry LDragon = new AIEntry("Lesser Dragon", 150, 4, "LesserDragon", DropTable.BasicEB, 60, 80, 2);
		public static AIEntry ACrystal = new AIEntry("Animated Crystal", 200, 4, "AnimatedCrystal", DropTable.BasicEA, 40, 50, 2);
		public static AIEntry MGolem = new AIEntry("Mineral Golem", 50, 2, "MineralGolem", DropTable.BasicEA, 90, 50, 1);
		public static AIEntry Imp = new AIEntry("Imp", 30, 2, "Imp", DropTable.BasicEA, 90, 80);

		public static AIEntry Warlock = new AIEntry("Warlock", 400, 4, "Warlock", DropTable.BossE, 80, 100, 2);

		// floor 6
		// floor 7
		// floor 8
		// floor 9
		// floor 10
	}

	public class AIEntry {
		public string Name;
		public int MaxHealth, MaxMana, RespondRate, Accuracy, MaxPlay;
		public string Deck;
		public Drops Drop;
		public KeyPair[] Affinity, Resistances;

		public AIEntry(string name, int maxhealth, int maxmana, string deck, Drops drop = null, int resrate = 100, int acc = 100, int maxplay = 1, KeyPair[] affinity = null, KeyPair[] resistance = null) {
			Name = name;
			MaxHealth = maxhealth;
			MaxMana = maxmana;
			RespondRate = resrate;
			Accuracy = acc;
			Deck = deck;
			Drop = drop;
			MaxPlay = maxplay;

			Affinity = affinity;
			Resistances = resistance;
		}

		public static CardAI GenEntry(AIEntry entry) {
			CardAI ret = new CardAI(entry);

			if (entry.Affinity != null) {
				for (int i = 0; i < entry.Affinity.GetLength(0); i++) {
					ret.ChangeAffinity(BaseTypes.Translate(entry.Affinity[i].Name), entry.Affinity[i].Amount);
				}
			}

			if (entry.Resistances != null) {
				for (int i = 0; i < entry.Resistances.GetLength(0); i++) {
					ret.ChangeResistance(BaseTypes.Translate(entry.Resistances[i].Name), entry.Resistances[i].Amount);
				}
			}

			return ret;
		}
	}
}	
