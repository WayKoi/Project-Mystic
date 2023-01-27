using Card_Test.Map;
using System;
using System.Collections.Generic;
using System.Text;

namespace Card_Test.Tables {
	public static class FloorTable {
        public static FloorGen OverGrowth = new FloorGen("Overgrowth", 0,
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

        public static FloorGen IvoryHalls = new FloorGen("Ivory Halls", 2,
            7, 6, 6, 4, 3,
            new RoomGen(1, 1, 2, 3, new int[] { 0, 0, 3 }, new int[] { 0 }, new int[] { 2, 3 }),
            BossTable.Mannequin,
            BattleTable.IvoryHalls,
            50, 1
        );

        public static FloorGen Catacombs = new FloorGen("Catacombs", 3,
            7, 7, 7, 4, 3,
            new RoomGen(1, 1, 4, 4, new int[] { 0, 0, 0 }, new int[] { 1, 2 }),
            BossTable.Demon,
            BattleTable.Catacombs
        );

        public static FloorGen CrystalHollow = new FloorGen("Crystal Hollow", 4,
            9, 9, 8, 5, 3,
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
            BattleTable.CrystalHollow
        );
	}
}
