using System;
using System.Collections.Generic;
using System.Text;
using Card_Test.Map.Rooms;
using Card_Test.Tables;

namespace Card_Test.Map {
    public class Floor {
        //                                    N          E         S         W
        private static int[,] changes = { { 0, -1 }, { 1, 0 }, { 0, 1 }, { -1, 0 } };
        private static int BaseWeight;

        private Room[,] Rooms;
        private int Width, Height, Magnetic;
        private FloorGen Gen;

        private Boss BossBattle;

        public string Name;

        public Floor (FloorGen gen, bool print = false) {
            Gen = gen;
            
            Name = Gen.Name;

            Width = gen.Width;
            Height = gen.Height;
            Magnetic = gen.Magnetic;
            BaseWeight = gen.BaseWeight;
            BossBattle = gen.Boss;

            Rooms = new Room[Width, Height];

            int startx = Global.Rand.Next((int) Math.Round(Width / 4.0), (int) Math.Round(Width * 3.0 / 4.0));
            int starty = Global.Rand.Next((int) Math.Round(Height / 4.0), (int) Math.Round(Height * 3.0 / 4.0));

            Room start = new StartRoom(new Room(new bool[4]));
            start.Pos[0] = startx; start.Pos[1] = starty;
            start.SetActivateAction(gen.StartRoom);

            Rooms[startx, starty] = start;
            
            List<Room> Stack = new List<Room>();
            List<Room> TemStack = new List<Room>();

            int prependlen = gen.Prepend == null ? 0 : gen.Prepend.Length;

            if (prependlen > 0) {
                start.MaxConnections = 1;
            }

            int stackSize = (gen.BossLen - 1) - prependlen;

            for (int i = 0; i < gen.BranchAmt; i++) {
                stackSize += gen.BranchLen - i;
            }

            // campfires + inns
            for (int i = 0; i < gen.RoomAmt[0]; i++) {
                TemStack.Add(GenRoom(3));
            }

            for (int i = 0; i < gen.RoomAmt[1]; i++) {
                TemStack.Add(GenRoom(5));
            }

            // cauldrons and altars
            for (int i = 0; i < gen.SpecialRooms[0]; i++) {
                TemStack.Add(Global.Rand.Next(0, 2) == 0 ? GenRoom(6) : GenRoom(7));
            }

            for (int i = 0; i < gen.SpecialRooms[1]; i++) {
                TemStack.Add(GenRoom(6));
            }

            for (int i = 0; i < gen.SpecialRooms[2]; i++) {
                TemStack.Add(GenRoom(7));
            }

            // battles
            int battleamt = Global.Rand.Next(Gen.BattleData[0], Gen.BattleData[1] + 1);
            for (int i = 0; i < battleamt; i++) {
                TemStack.Add(GenRoom(-1));
            }

            // the shops
            for (int i = 0; i < gen.ShopTypes.Length; i++) {
                for (int ii = 0; ii < gen.ShopTypes[i]; ii++) {
                    switch (i) {
                        case 0: TemStack.Add(GenRoom(4)); break;
                        case 1: TemStack.Add(new ShopRoom(new Room(new bool[4]), gen.ShopTier, new int[] { 1 })); break;
                        case 2: TemStack.Add(new ShopRoom(new Room(new bool[4]), gen.ShopTier, new int[] { 0, 1 })); break;
                        case 3: TemStack.Add(new ShopRoom(new Room(new bool[4]), gen.ShopTier, new int[] { 0, 0, 1 })); break;
                    }
                }
            }

            // add the regular rooms
            while (TemStack.Count < Math.Min(stackSize - 1, (Width * Height) - 3 - prependlen)) {
                TemStack.Add(GenRoom(0));
            }

            // prepend the rooms
            for (int i = 0; i < prependlen; i++) {
                Room prep = TRoom.Generate(Gen.Prepend[i]);
                if (i < prependlen - 1) { prep.MaxConnections = 2; }
                Stack.Add(prep);
            }

            // remove from temstack and push to stack
            while (TemStack.Count > 0) {
                int chosen = Global.Rand.Next(0, TemStack.Count);
                Stack.Add(TemStack[chosen]);
                TemStack.RemoveAt(chosen);
            }

            // insert the fail safe room (so that the inn or other single path rooms dont break generation)
            Stack.Insert(prependlen, GenRoom(0));

            // insert the boss room
            Room broom = GenRoom(2);
            broom.SetActivateAction(gen.EndRoom);
            Stack.Insert(gen.BossLen - 1, broom);

            List<Room> Pop = new List<Room>();
            Pop.Add(start);

            // generate main branch
            GenerateBranch(start, startx, starty, gen.BossLen + prependlen, Stack, Pop, print);

            // generate other branches
            int BranchAmt = gen.BranchAmt;
            while (BranchAmt > 0 && Pop.Count > 0) {
                List<RoomChance> chances = GetBranchChances(Pop, print);

                int chosen = Global.Roll(RoomChances(chances));

                if (chosen != -1) {
                    bool made = GenerateBranch(Pop[chosen], Pop[chosen].Pos[0], Pop[chosen].Pos[1], gen.BranchLen - (gen.BranchAmt - BranchAmt), Stack, Pop, print);

                    if (!made) {
                        Pop.RemoveAt(chosen);
                    } else {
                        BranchAmt--;
                    }
                }
            }

            while (Pop.Count > 0 && Stack.Count > 0 && !GridFull()) {
                int chosen = Global.Rand.Next(0, Pop.Count);

                bool made = GenerateBranch(Pop[chosen], Pop[chosen].Pos[0], Pop[chosen].Pos[1], 1, Stack, Pop, print);

                if (!made) {
                    Pop.RemoveAt(chosen);
                }
            }

            if (Stack.Count > 0) {
                TextUI.PrintFormatted("³Code Broken!⁰");
            }
        }
        
        private List<RoomChance> GetBranchChances (List<Room> Pop, bool print = false) {
            List<RoomChance> rooms = new List<RoomChance>();
            int[,] spots = new int[Width, Height];

            for (int i = 0; i < Height; i++) {
                for (int ii = 0; ii < Width; ii++) {
                    if (Rooms[ii, i] == null) {
                        spots[ii, i] = GetNeighbours(ii, i) > 1 ? BaseWeight : Math.Max(BaseWeight + Magnetic, 1);
                    }
                }
            }

            for (int i = 0; i < Pop.Count; i++) {
                int chance = SumNeighbours(Pop[i], spots);
                if (Pop[i].CanBranch() && chance > 0) {
                    rooms.Add(new RoomChance(Pop[i], chance));
                } else {
                    Pop.RemoveAt(i);
                    i--;
                }
            }

            if (print) {
			    for (int i = 0; i < Height; i++) {
				    for (int ii = 0; ii < Width; ii++) {
					    Console.Write(spots[ii, i] + "\t");
				    }
                    Console.Write("\n");
			    }
            }
            
			return rooms;
        }

        private int[] RoomChances(List<RoomChance> rooms) {
            List<int> chances = new List<int>();
            int roomcount = rooms.Count;

            for (int i = 0; i < roomcount; i++) {   
                chances.Add(rooms[i].Chance);
            }

            return chances.ToArray();
        }

        private int SumNeighbours (int x, int y, int[,] spots) {
            int count = 0;

            for (int ii = 0; ii < 4; ii++) {
                int px = x + changes[ii, 0], py = y + changes[ii, 1];
                bool cond = (px < 0 || px >= Width) ||
                            (py < 0 || py >= Height);

                if (!cond) {
                    count += spots[px, py];
                }
            }

            return count;
        }

        private int SumNeighbours(Room point, int[,] spots) {
            return point == null ? 0 : SumNeighbours(point.Pos[0], point.Pos[1], spots);
        }

        private bool GridFull () {
            for (int i = 0; i < Height; i++) {
                for (int ii = 0; ii < Width; ii++) {
                    if (Rooms[ii, i] == null) {
                        return false;
                    }
                }
            }

            return true;
        }

        private bool GenerateBranch (Room start, int x, int y, int len, List<Room> Stack, List<Room> Pop, bool print = false) {
            if (Stack.Count <= 0 || len <= 0) { return true; }
            if (!start.CanBranch()) { return false; }
            // check if there is a connection that can be branched from

            List<int> Branchable = new List<int>();
			List<int> SpotWeights = new List<int>();

            for (int i = 0; i < start.Connections.Length; i++) {
                bool cond = !start.Connections[i] && 
                            (x + changes[i, 0]) >= 0 && (x + changes[i, 0]) < Width &&
                            (y + changes[i, 1]) >= 0 && (y + changes[i, 1]) < Height &&
                            Rooms[x + changes[i, 0], y + changes[i, 1]] == null;

                if (cond) {
                    int px = x + changes[i, 0], py = (y + changes[i, 1]);

                    int neigh = GetNeighbours(px, py);

                    int weight = Math.Max(BaseWeight + (neigh > 1 ? 0 : Magnetic), 1);

                    SpotWeights.Add(weight);
                    Branchable.Add(i);
                }
            }

            if (Branchable.Count <= 0) { return false; }

            int chosenspot = Global.Roll(SpotWeights.ToArray());
            int dir = Branchable[chosenspot];

            Room gen = Stack[0];
            Pop.Add(Stack[0]);
            Stack.RemoveAt(0);

            if (gen == null) { return false; }

            Rooms[x + changes[dir, 0], y + changes[dir, 1]] = gen;
            gen.Pos[0] = x + changes[dir, 0]; gen.Pos[1] = y + changes[dir, 1];
            gen.Connections[(dir + 2) % 4] = true;
            start.Connections[dir] = true;

            if (print) {
                gen.Explored = true;
                TextUI.PrintFormatted(ToString());
            }

			bool next = GenerateBranch(gen, x + changes[dir, 0], y + changes[dir, 1], len - 1, Stack, Pop, print);

			if (!next) {
				return GenerateBranch(start, x, y, len - 1, Stack, Pop, print);
			}

			return true;
		}

        private int GetNeighbours (int x, int y) {
            int count = 0;

            for (int ii = 0; ii < 4; ii++) {
                int px = x + changes[ii, 0], py = y + changes[ii, 1];
                bool cond = (px < 0 || px >= Width) ||
                            (py < 0 || py >= Height) ||
                            Rooms[px, py] != null;

                if (cond) {
                    count++;
                }
            }

            return count;
        }

        private int GetNeighbours (Room point) {
            return GetNeighbours(point.Pos[0], point.Pos[1]);
        }

        public Room GenRoom (int type) {
            switch (type) {
                case -1: return new BattleRoom(new Room(new bool[4]), Gen.Battles);
                case 0: return new Room(new bool[4]);
                case 1: return new StartRoom(new Room(new bool[4]));
                case 2: return new BossRoom(new Room(new bool[4]), RunBossBattle);
                case 3: return new Campfire(new Room(new bool[4]));
                case 4: return new ShopRoom(new Room(new bool[4]), Gen.ShopTier, Gen.ShopWeights);
                case 5: return new Inn(new Room(new bool[4]), Gen.InnCost);
                case 6: return new Cauldron(new Room(new bool[4]));
                case 7: return new Altar(new Room(new bool[4]));
            }

            return null;
        }

        public void RevealFloor () {
            for (int i = 0; i < Height; i++) {
                for (int ii = 0; ii < Width; ii++) {
                    if (Rooms[ii, i] != null) {
                        Rooms[ii, i].Explored = true;
                    }
                }
            }
        }

        public void RunBossBattle(int dum) {
            if (dum > 0) { return; }
            if (BossBattle == null) { return; }
            BossBattle.RunBattle();

            foreach (Room rom in Rooms) {
                if (rom != null) {
                    rom.BossDefeated();
                }
            }
        }

        public bool MoveTo(int direction) {
            // direction = N E S W
            Room point = GetCurrentPosition();
            if (point == null) { return false; }
            Room travel = point.Connections[direction] ? Rooms[point.Pos[0] + changes[direction, 0], point.Pos[1] + changes[direction, 1]] : null;

            if (travel == null) { return false; }
            if (travel.Locked) {
                travel.Explored = true;
                TextUI.PrintFormatted("That room is locked, you need to find a key somewhere on the floor");
                TextUI.Wait();
                return true;
            }

            if (travel.GetRoomType() == 2 && travel.Explored == false) {
                travel.Explored = true;
                TextUI.PrintFormatted("You see a boss in the room ahead, you decide to hesitate before going in");
                TextUI.Wait();
                return true;
            }

            point.Exit();
            travel.Enter();

            return true;
        }

        public Room GetCurrentPosition() {
            foreach (Room rom in Rooms) {
                if (rom != null && rom.PlayerHere) {
                    return rom;
                }
            }

            return null;
        }

        public string GetCurrentRoomDetails() {
            return "You are currently in a " + GetCurrentPosition().GetRoomName() + " Room";
        }

        public override string ToString() {
            string build = "";/*"--\n";*/

            for (int i = 0; i < Height; i++) {
                List<string> row = new List<string>();
                for (int ii = 0; ii < Width; ii++) {
                    if (Rooms[ii, i] != null && Rooms[ii, i].Explored) {
                        row.Add(Rooms[ii, i].ToString());
                    } else {
                        row.Add("   \n\n");
                    }
                }
                build += String.Join('\n', TextUI.MakeTable(row, 0)) + (i < Height ? "\n" : "");
            }

            return build ;/*+ "\n--";*/
        }

        private class RoomChance {
            public Room Point;
            public int Chance;

            public RoomChance (Room point, int chance) {
                Point = point;
                Chance = chance;
            }
        }
    }

    public class FloorGen {
        public int Width, Height, BossLen, BranchLen, BranchAmt, BranchChance, Magnetic, ShopTier, InnCost, BaseWeight;
        // roomamt = Campfires, inns
        // specialroooms = { random, cauldrons, altars }
        public int[] RoomAmt, ShopTypes, ShopWeights, SpecialRooms;
        // shop types = # random, # card, # pack, # gear

        public Boss Boss;
        public TRoom[] Prepend;
        public BattlePool[] Battles;
        // BattleData = # min, # max
        public int[] BattleData;

        public Action<int, int> StartRoom, EndRoom;

        public string Name;

        public FloorGen(string name, int[] Gen, int[] RoomVals, int[] Chances, int[] roomamt, int[] shopTypes, int[] shopweights, int[] specialrooms, int[] battleData, Boss boss = null, BattlePool[] battles = null, Action<int, int> startRoom = null, Action<int, int> endRoom = null, TRoom[] prepend = null) {
            Name = name;    
            Width = Gen[0];
            Height = Gen[1];
            BossLen = Gen[2];
            BranchLen = Gen[3];
            BranchAmt = Gen[4];

            ShopTier = RoomVals[0];
            InnCost = RoomVals[1];

            Magnetic = Chances[0];
            BaseWeight = Chances.Length > 1 ? Chances[1] : 10;

            RoomAmt = roomamt;
            ShopTypes = shopTypes;
            SpecialRooms = specialrooms;
            ShopWeights = shopweights;
            BattleData = battleData;

            Boss = boss;
            Battles = battles;

            Prepend = prepend;

            StartRoom = startRoom;
            EndRoom = endRoom;
        }
    }
}
