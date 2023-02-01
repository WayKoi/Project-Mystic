using System;
using System.Collections.Generic;
using System.Text;
using Card_Test.Tables;

namespace Card_Test.Map {
    public class Dungeon {
        public List<Floor> Floors = new List<Floor>();
        private MenuItem[] MoveMenu;
        public bool GameRunning = true;

        private Floor CurrentFloor = null;

        private FloorPool[] FloorPools = {
            FloorTable.TierZero,
            FloorTable.TierOne,
            FloorTable.TierTwo,
            FloorTable.TierThree,
            FloorTable.TierFour,
            FloorTable.TierFive
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

			for (int i = 0; i < FloorPools.Length; i++) {
                FloorGen gen = FloorPools[i].Roll();
                Floor add = new Floor(gen);

                add.Link = this;

                if (i != 0) {
                    add.ChangeUpLink(Floors[i - 1]);
                    Floors[i - 1].ChangeDownLink(add);
                }

				Floors.Add(add);
			}

            CurrentFloor = Floors[0];
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
            CurrentFloor.RevealFloor();
			TextUI.PrintFormatted("You cheat and reveal the whole map, press enter to continue");
			Console.ReadLine();

			return true;
		}

        public bool SkipTo(int[] to) {
            if (CurrentFloor == null || CurrentFloor.Down == null) { return false; }

            CurrentFloor = CurrentFloor.Down;
            TextUI.PrintFormatted("You cheat your way to " + CurrentFloor.Name + ", press enter to continue");
            Console.ReadLine();

            return true;
        }

        public bool ActivateRoom(int[] dum) {
			Room point = CurrentFloor.GetCurrentPosition();
			point.Active();

			return true;
		}

        public bool ViewRoom(int[] dum) {
			Room point = CurrentFloor.GetCurrentPosition();
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

        public void ChangeFloor (Floor to) {
            if (to == null) {
                TextUI.PrintFormatted("You cannot move to that floor");
            } else {
                TextUI.PrintFormatted("You move to " + to.Name);
                CurrentFloor = to;
            }

            TextUI.Wait();
        }

        public bool Move(int[] movement) {
			if (movement == null || movement.Length == 0) { return false; }
			if (movement[0] == -1) { return false; }

			return CurrentFloor.MoveTo(movement[0]);
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

            if (CurrentFloor != null) {
			    TextUI.PrintFormatted("\n\t" + CurrentFloor.Name + "\n");
			    TextUI.PrintFormatted(CurrentFloor.GetCurrentRoomDetails());
			    string floor = CurrentFloor.ToString();

			    TextUI.PrintFormatted("\n" + floor);
            }

            TextUI.PrintFormatted(Global.Run.Player + " Material : " + Global.Run.Player.Material);
			Console.WriteLine();
		}
    }
}
