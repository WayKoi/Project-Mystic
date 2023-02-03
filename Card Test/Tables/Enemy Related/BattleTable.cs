using Card_Test.Base;
using Card_Test.Tables;
using System;
using System.Collections.Generic;
using System.Text;

namespace Card_Test.Tables {
	public static class BattleTable {
		
		public static BattlePool[] Overgrowth = {
			new BattlePool(1, 2, new BTableEntry[] {
				new BTableEntry(EnemyTable.Rat, 15, 1),
				new BTableEntry(EnemyTable.Wolf, 55),
				new BTableEntry(EnemyTable.Spider, 30, 1)
			}, 1)
		};

		public static BattlePool[] SandyCaverns = {
			new BattlePool(1, 2, new BTableEntry[] {
				new BTableEntry(EnemyTable.Snake, 50),
				new BTableEntry(EnemyTable.Scorpion, 15)
			}, 9),

			new BattlePool(1, 1, new BTableEntry[] {
				new BTableEntry(EnemyTable.SandWizard, 1)
			}, 1)
		};

		public static BattlePool[] Oasis = {
			new BattlePool(1, 2, new BTableEntry[] {
				new BTableEntry(EnemyTable.Snake, 40),
				new BTableEntry(EnemyTable.Scorpion, 5),
				new BTableEntry(EnemyTable.DragonFly, 30),
				new BTableEntry(EnemyTable.Hyena, 50)
			}, 9),

			new BattlePool(1, 1, new BTableEntry[] {
				new BTableEntry(EnemyTable.Crocodile, 1)
			}, 1)
		};

		public static BattlePool[] IvoryHalls = {
			new BattlePool(1, 2, new BTableEntry[] {
				new BTableEntry(EnemyTable.MHead, 10, 1),
				new BTableEntry(EnemyTable.MLeg, 45),
				new BTableEntry(EnemyTable.MArm, 45)
			}, 2),

			new BattlePool(2, 3, new BTableEntry[] {
				new BTableEntry(EnemyTable.MHead, 5, 1),
				new BTableEntry(EnemyTable.MLeg, 45),
				new BTableEntry(EnemyTable.MArm, 45)
			}, 7),

			new BattlePool(1, 1, new BTableEntry[] {
				new BTableEntry(EnemyTable.BuzzSaw, 1)
			}, 1)
		};

		public static BattlePool[] OvergrowthII = {
			new BattlePool(1, 2, new BTableEntry[] {
				new BTableEntry(EnemyTable.Boar, 45),
				new BTableEntry(EnemyTable.Mice, 10, 1),
				new BTableEntry(EnemyTable.GiantSpider, 45, 1)
			}, 7),

			new BattlePool(2, 3, new BTableEntry[] {
				new BTableEntry(EnemyTable.Boar, 45),
				new BTableEntry(EnemyTable.Mice, 10, 1),
				new BTableEntry(EnemyTable.GiantSpider, 45, 1)
			}, 2),

			new BattlePool(1, 1, new BTableEntry[] {
				new BTableEntry(EnemyTable.SproutWizard, 1)
			}, 1)
		};

		public static BattlePool[] Catacombs = {
			new BattlePool(1, 2, new BTableEntry[] {
				new BTableEntry(EnemyTable.Zombie, 20, 1),
				new BTableEntry(EnemyTable.Skeleton, 50),
				new BTableEntry(EnemyTable.BloodBat, 20, 1)
			}, 7),

			new BattlePool(2, 3, new BTableEntry[] {
				new BTableEntry(EnemyTable.Zombie, 20, 1),
				new BTableEntry(EnemyTable.Skeleton, 60),
				new BTableEntry(EnemyTable.BloodBat, 20, 1)
			}, 2),

			new BattlePool(1, 1, new BTableEntry[] {
				new BTableEntry(EnemyTable.Vampire, 1)
			}, 1)
		};

		public static BattlePool[] IvoryHallsII = {
			new BattlePool(1, 2, new BTableEntry[] {
				new BTableEntry(EnemyTable.Banshee, 5, 1),
				new BTableEntry(EnemyTable.Poltergeist, 5, 1),
				new BTableEntry(EnemyTable.Spirit, 10, 1)
			}, 7),

			new BattlePool(2, 3, new BTableEntry[] {
				new BTableEntry(EnemyTable.Banshee, 5, 1),
				new BTableEntry(EnemyTable.Poltergeist, 5, 1),
				new BTableEntry(EnemyTable.Spirit, 10, 1)
			}, 2),

			new BattlePool(1, 1, new BTableEntry[] {
				new BTableEntry(EnemyTable.Phantom, 1)
			}, 1)
		};

		public static BattlePool[] CrystalHollow = {
			new BattlePool(1, 1, new BTableEntry[] {
				new BTableEntry(EnemyTable.LDragon, 50),
				new BTableEntry(EnemyTable.ACrystal, 50)
			}, 6),

			new BattlePool(3, 3, new BTableEntry[] {
				new BTableEntry(EnemyTable.Imp, 1)
			}, 7),

			new BattlePool(2, 3, new BTableEntry[] {
				new BTableEntry(EnemyTable.MGolem, 1)
			}, 6),

			new BattlePool(1, 1, new BTableEntry[] {
				new BTableEntry(EnemyTable.Mimic, 1)
			}, 1)
		};
	}

	public class BTableEntry : Rollable {
		public AIEntry Enemy;

		public BTableEntry(AIEntry enemy, int chance, int Maxrolls = 0) : base (chance, Maxrolls) {
			Enemy = enemy;
		}

		public CardAI Generate () {
			return new CardAI(Enemy);
		}
	}

	public class BattlePool : Rollable {
		public BTableEntry[] Pool;
		public int Min, Max;

		public BattlePool (int min, int max, BTableEntry[] pool, int chance) : base (chance) {
			Min = min;
			Max = max;
			Pool = pool;
		}

		public List<CardAI> RollPool () {
			List<CardAI> ret = new List<CardAI>();

			int amt = Global.Rand.Next(Min, Max + 1);
			List<Rollable> rolled = Rollable.Roll(Pool, amt);
			while (rolled.Count > 0) {
				ret.Add((rolled[0] as BTableEntry).Generate());
				rolled.RemoveAt(0);
			}

			for (int i = 0; i < Pool.Length; i++) {
				Pool[i].ResetCount();
			}

			return ret;
		}

		public void RunBattle () {
			List<CardAI> chosen = RollPool();

			Battle batt = new Battle(Global.Run.Players.ToArray(), chosen.ToArray());
			bool result = batt.Run();

			if (!result) {
				Global.Run.TenFloor.EndGame();
			}
		}
	}
}
