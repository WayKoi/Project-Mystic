using Card_Test.Base;
using Card_Test.Tables;
using System;
using System.Collections.Generic;
using System.Text;

namespace Card_Test.Map {
    public class FloorGen {
        private static int[] ShopTiers = { 1, 1, 2 };
        private static int[] InnCosts = { 40, 40, 60 };

        public int FloorTier;

        public int Width, Height, BossLen, BranchLen, BranchAmt, BranchChance, Magnetic, BaseWeight;
        public int ShopTier, InnCost;

        public Boss Boss;
        public BattlePool[] Battles;

        public RoomGen Rooms;
        public TRoom[] Prepend;

        public string Name;

        public FloorGen(string name, int floorTier, int wid, int hei, int bosslen, int branchlen, int branchamt, RoomGen rooms, Boss boss = null, BattlePool[] battles = null, int magnetic = 10, int baseWeight = 10, TRoom[] prepend = null) {
            Name = name;

            FloorTier = floorTier;

            Width = wid;
            Height = hei;
            BossLen = bosslen;
            BranchLen = branchlen;
            BranchAmt = branchamt;

            ShopTier = ShopTiers[Math.Min(FloorTier, ShopTiers.Length - 1)];
            InnCost = InnCosts[Math.Min(FloorTier, InnCosts.Length - 1)];

            Magnetic = magnetic;
            BaseWeight = baseWeight;

            Rooms = rooms;
            Boss = boss;
            Battles = battles;

            Prepend = prepend;
        }
    }

    public struct RoomGen {
        public int Campfires, Inns;
        public int[] Shops, Specials;
        public int[] ShopWeights;
        public int[] Battles;
        public RareRoom[] Rares;

        public RoomGen (int campfires = 0, int inns = 0, int battleMin = 0, int battleMax = 0, int[] shops = null, int[] specials = null, int[] shopweights = null, RareRoom[] rareRooms = null) {
            Campfires = campfires;
            Inns = inns;

            Battles = new int[] { battleMin, battleMax };

            Shops = shops;
            Specials = specials;

            ShopWeights = shopweights;

            Rares = rareRooms;
        }
    }

    public class RareRoom : Rollable {
        public TRoom Room;
        public int Max;

        public RareRoom (int RoomType, int chance, int max = 100, int[] Data = null) : base (chance) {
            Room = new TRoom(RoomType, Data);
            Max = max;
        }
    }


}
