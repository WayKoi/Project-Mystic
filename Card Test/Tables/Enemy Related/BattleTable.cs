using System;
using System.Collections.Generic;
using System.Text;

namespace Card_Test.Tables {
	public static class BattleTable {
		// other vars
		private static Random Rand = new Random();

		// tables 
		public static BattleEntry[] TableA = { 
			new BattleEntry(EnemyTable.Rat, 15),
			new BattleEntry(EnemyTable.Wolf, 55),
			new BattleEntry(EnemyTable.Spider, 30)
		};

		public static BattleEntry[] TableB = {
			new BattleEntry(EnemyTable.Snake, 50),
			new BattleEntry(EnemyTable.MudMonster, 35),
			new BattleEntry(EnemyTable.Scorpion, 15)
		};

		public static BattleEntry[] TableBA = {
			new BattleEntry(EnemyTable.SandWizard, 100)
		};

		public static BattleEntry[] TableC = {
			new BattleEntry(EnemyTable.MHead, 10),
			new BattleEntry(EnemyTable.MLeg, 45),
			new BattleEntry(EnemyTable.MArm, 45)
		};

		public static BattleEntry[] TableCA = {
			new BattleEntry(EnemyTable.BuzzSaw, 100)
		};

		public static BattleEntry[] TableD = {
			new BattleEntry(EnemyTable.Zombie, 50),
			new BattleEntry(EnemyTable.Skeleton, 15),
			new BattleEntry(EnemyTable.Banshee, 5),
			new BattleEntry(EnemyTable.Poltergeist, 5),
			new BattleEntry(EnemyTable.BloodBat, 25)
		};

		public static BattleEntry[] TableDA = {
			new BattleEntry(EnemyTable.Vampire, 100)
		};

		public static BattleEntry[] TableEA = {
			new BattleEntry(EnemyTable.LDragon, 50),
			new BattleEntry(EnemyTable.ACrystal, 50)
		};

		public static BattleEntry[] TableEB = {
			new BattleEntry(EnemyTable.Imp, 100)
		};

		public static BattleEntry[] TableEC = {
			new BattleEntry(EnemyTable.MGolem, 100)
		};

		public static BattleEntry[] TableED = {
			new BattleEntry(EnemyTable.Mimic, 100)
		};



		public static BattleEntry[] GetTable(int floor) {
			switch (floor) {
				case 0: return TableA;
				case 1: return TableB;
				case 2: return TableBA;
				case 3: return TableC;
				case 4: return TableCA;
				case 5: return TableD;
				case 6: return TableDA;
				case 7: return TableEA;
				case 8: return TableEB;
				case 9: return TableEC;
				case 10: return TableED;
			}

			return null;
		}

		// methods
		public static AIEntry GetEnemy (BattleEntry[] Choices) {
			int chosen = Rand.Next(1, 101);

			foreach (BattleEntry entry in Choices) {
				if (chosen <= entry.Chance) {
					return entry.Enemy;
				}
				chosen -= entry.Chance;
			}

			return null;
		}

	}

	public class BattleEntry {
		public AIEntry Enemy;
		public int Chance;

		public BattleEntry(AIEntry enemy, int chance) {
			Enemy = enemy;
			Chance = chance;
		}
	}
}
