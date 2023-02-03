using Card_Test.Base;
using Card_Test.Map;
using System;
using System.Collections.Generic;
using System.Text;

namespace Card_Test.Tables {
	public static class FloorTable {
        public static FloorGen OverGrowth = new FloorGen("Overgrowth I", 0,
            4, 3, 4, 2, 2,
            new RoomGen(1, 0, 1, 2, new int[] { 0 }, null, new int[] { 2, 3 }),
            BossTable.Beetle,
            BattleTable.Overgrowth
		);

        public static FloorGen SandyCaverns = new FloorGen("Sandy Caverns", 1,
            7, 5, 5, 4, 2,
            new RoomGen(1, 1, 2, 3, new int[] { 0, 0 }, null, new int[] { 2, 3 }),
            BossTable.Salamander,
            BattleTable.SandyCaverns,
            50, 1
        );

        public static FloorGen Oasis = new FloorGen("Oasis", 1,
            8, 4, 6, 4, 2,
            new RoomGen(1, 1, 2, 3, new int[] { 0, 0 }, null, new int[] { 2, 3 }),
            BossTable.Camel,
            BattleTable.Oasis,
            50, 1
        );

        public static FloorGen IvoryHalls = new FloorGen("Ivory Halls I", 2,
            7, 6, 6, 3, 4,
            new RoomGen(1, 1, 2, 3, new int[] { 0, 0, 3 }, new int[] { 0 }, new int[] { 2, 3 }),
            BossTable.Mannequin,
            BattleTable.IvoryHalls,
            50, 1
        );

        public static FloorGen OvergrowthII = new FloorGen("Overgrowth II", 2,
            8, 4, 6, 4, 3,
            new RoomGen(1, 1, 2, 3, new int[] { 0, 0, 3 }, new int[] { 0 }, new int[] { 2, 3 }),
            BossTable.Mammoth,
            BattleTable.OvergrowthII,
            1, 10
        );

        public static FloorGen Catacombs = new FloorGen("Catacombs", 3,
            7, 7, 7, 4, 3,
            new RoomGen(1, 1, 4, 4, new int[] { 0, 0, 0 }, new int[] { 1, 2 }),
            BossTable.Demon,
            BattleTable.Catacombs,
            1, 10
        );

        public static FloorGen IvoryHallsII = new FloorGen("Ivory Halls II", 3,
            7, 6, 6, 3, 5,
            new RoomGen(1, 1, 4, 4, new int[] { 0, 0, 0 }, new int[] { 1, 2 }),
            BossTable.Reaper,
            BattleTable.IvoryHallsII,
            50, 1
        );

        public static FloorGen CrystalHollow = new FloorGen("Crystal Hollow", 4,
            10, 7, 8, 5, 3,
            new RoomGen(1, 1, 4, 5, new int[] { 0, 0, 0 }, new int[] { 1, 2 }),
            BossTable.Warlock,
            BattleTable.CrystalHollow,
            100, 1
        );

        // Temp Floor 6
        public static FloorGen Testing = new FloorGen("Test", 5,
            10, 10, 8, 5, 3,
            new RoomGen(3, 1, 0, 0, new int[] { 1, 1, 1, 2, 2, 2, 3, 3, 3 }, new int[] { 1, 1, 1, 2, 2, 2 }),
            BossTable.Warlock,
            BattleTable.CrystalHollow,
            100, 1
        );
            
        // ------------------------------------------------------------------------------------------------------
        //  Pools
        // ------------------------------------------------------------------------------------------------------

        public static FloorPool TierZero = new FloorPool(
            new FPoolEntry[] {
                new FPoolEntry(OverGrowth, 1)
            }
        );

        public static FloorPool TierOne = new FloorPool(
            new FPoolEntry[] {
                new FPoolEntry(SandyCaverns, 1),
                new FPoolEntry(Oasis, 1)
            }
        );

        public static FloorPool TierTwo = new FloorPool(
            new FPoolEntry[] {
                new FPoolEntry(IvoryHalls, 1),
                new FPoolEntry(OvergrowthII, 1)
            }
        );

        public static FloorPool TierThree = new FloorPool(
            new FPoolEntry[] {
                new FPoolEntry(Catacombs, 1),
                new FPoolEntry(IvoryHallsII, 1)
            }
        );

        public static FloorPool TierFour = new FloorPool(
            new FPoolEntry[] {
                new FPoolEntry(CrystalHollow, 1)
            }
        );

        public static FloorPool TierFive = new FloorPool(
            new FPoolEntry[] {
                new FPoolEntry(Testing, 1)
            }
        );

    }

    public class FloorPool {
        public FPoolEntry[] Entries;

        public FloorPool (FPoolEntry[] entries) {
            Entries = entries;
        }

        public FloorGen Roll () {
            List<Rollable> rolled = Rollable.Roll(Entries, 1);
            if (rolled.Count == 0) { return null; }
            return (rolled[0] as FPoolEntry).Gen;
        }
    }

    public class FPoolEntry : Rollable {
        public FloorGen Gen;

        public FPoolEntry(FloorGen gen, int chance) : base (chance) {
            Gen = gen;
        }
    }
}
