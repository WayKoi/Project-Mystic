using System;
using System.Collections.Generic;
using System.Text;
using Card_Test.Tables;

namespace Card_Test.Map {
    public class Dungeon {
        public List<Floor> Floors = new List<Floor>();
        private int FloorNum = 0;
        private MenuItem[] MoveMenu;
        public bool GameRunning = true;

        private FloorGen[] FloorPlans = {
            FloorTable.OverGrowth,
            FloorTable.SandyCaverns,
            FloorTable.IvoryHalls,
            FloorTable.Catacombs,
            FloorTable.CrystalHollow,
            FloorTable.Testing
        };

        public void Init() {
            MoveMenu = new MenuItem[] {
                new MenuItem(new string[] { "W", "A", "S", "D" }, Move, ParseMove, "move in that direction"),
                new MenuItem(new string[] { "Q" }, ActivateRoom, ParseMove, "try and interact with the room"),
                new MenuItem(new string[] { "View", "V" }, ViewRoom, ParseMove, "look around the room"),
                new MenuItem(new string[] { "Edit", "E" }, EditDeck, ParseMove, "edit your deck"),
                new MenuItem(new string[] { "Inventory", "I" }, ViewCharacter, ParseMove, "view your character"),
                new MenuItem(new string[] { "Restart", "Res" }, EndGame, TextUI.DummyParse, "Restart the game"),
                new MenuItem(new string[] { "Skip" }, SkipTo, TextUI.Parse, null),
                new MenuItem(new string[] { "Reveal" }, RevealFloor, ParseMove, null)
            };

			for (int i = 0; i < FloorPlans.Length; i++) {
                FloorPlans[i].StartRoom = MoveUp;
                FloorPlans[i].EndRoom = MoveDown;

                Floor add = new Floor(FloorPlans[i]);
				Floors.Add(add);
			}
		}

        public bool EditDeck (int[] dum) {
            return Global.Run.Player.Cards.EditDeck(true);
        }

        public void Run() {
            while (GameRunning) {
                PrintScene();
                TextUI.Prompt("What would you like to do?", MoveMenu);
            }
        }

        public bool EndGame (int[] dum = null) {
            GameRunning = false;
            return true;
        }

        public bool RevealFloor(int[] dum) {
			Floors[FloorNum].RevealFloor();
			TextUI.PrintFormatted("You cheat and reveal the whole map, press enter to continue");
			Console.ReadLine();

			return true;
		}

        public bool SkipTo(int[] to) {
            if (to.Length < 1 || to[0] < 1 || to[0] > Floors.Count) { return false; }
            FloorNum = to[0] - 1;
            TextUI.PrintFormatted("You cheat your way to floor " + to[0] + ", press enter to continue");
            Console.ReadLine();

            return true;
        }

        public bool ActivateRoom(int[] dum) {
			Room point = Floors[FloorNum].GetCurrentPosition();
			point.Active();

			return true;
		}

        public bool ViewRoom(int[] dum) {
			Room point = Floors[FloorNum].GetCurrentPosition();
			TextUI.PrintFormatted("You look around the room");
			TextUI.PrintFormatted(point.GetDescription());
			TextUI.PrintFormatted("Press enter to continue");
			Console.ReadLine();

			return true;
		}

        public bool ViewCharacter(int[] dum) {
            Global.Run.Player.ViewCharacter();
            return true;
        }

        public void MoveUp(int dum, int dumm) {
            if (FloorNum <= 0) {
                TextUI.PrintFormatted("You cannot move up a floor, press enter to continue");
                Console.ReadLine();
                return;
            }

            TextUI.PrintFormatted("You move up a floor, press enter to continue");
            Console.ReadLine();
            FloorNum--;
        }

        public void MoveDown(int dum, int dumm) {
            if (FloorNum >= Floors.Count - 1) {
                TextUI.PrintFormatted("You cannot move down a floor, press enter to continue");
                Console.ReadLine();
                return;
            }

            TextUI.PrintFormatted("You move down a floor, press enter to continue");
            Console.ReadLine();
            FloorNum++;
        }

        public bool Move(int[] movement) {
			if (movement == null || movement.Length == 0) { return false; }
			if (movement[0] == -1) { return false; }

			return Floors[FloorNum].MoveTo(movement[0]);
		}

        public int[] ParseMove(string input) {
            int[] ret = { -1 };
            input = input.ToLower();

            switch (input) {
                case "w": ret[0] = 0; break;
                case "d": ret[0] = 1; break;
                case "s": ret[0] = 2; break;
                case "a": ret[0] = 3; break;
            }

            return ret;
        }  

        public void PrintScene() {
			Console.Clear();
			// TextUI.PrintFormatted(new string(' ', Floors[FloorNum].Width * 3 + 7) + "  W");
			TextUI.PrintFormatted("\n\t" + Floors[FloorNum].Name + "\n");
			// TextUI.PrintFormatted(new string(' ', Floors[FloorNum].Width * 3 + 7) + "  S\n");
			TextUI.PrintFormatted(Floors[FloorNum].GetCurrentRoomDetails());
			// Ω
			string floor = Floors[FloorNum].ToString();

			TextUI.PrintFormatted("\n" + floor);

			TextUI.PrintFormatted(Global.Run.Player + " Material : " + Global.Run.Player.Material);
			Console.WriteLine();
		}
    }
}
