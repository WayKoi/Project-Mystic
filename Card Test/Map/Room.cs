using Card_Test.Map.Rooms;
using System;
using System.Collections.Generic;
using System.Text;

namespace Card_Test.Map {
    public class Room {
        protected int RoomType = 0;
        protected string RoomName = "Regular";
        protected string Description = "There is nothing special about this room";
        protected string Symbol = " ";

        protected int EnterCount = 0, ActivateCount = 0, ExitCount = 0;
        protected int MaxActivate = 0;
        protected Action<int> EnterAction;
        protected Action<int, int> ActivateAction;
        protected Action<int> ExitAction;
        
        public int MaxConnections = 4;
        public bool[] Connections;
        // order is N E S W
        public int[] Pos = { 0, 0 };

        public bool Explored = false, PlayerHere = false, Traversed = false;
        public bool Locked = false;

        public Floor Link = null;

        public Room(bool[] connects = null) {
            if (connects == null) { connects = new bool[4]; }
            Connections = connects;
        }

        public Room (Room replace) {
            Connections = replace.Connections;
        }

        public bool CanBranch () {
            int count = 0;
            for (int i = 0; i < Connections.Length; i++) {
                if (Connections[i]) {
                    count++;
                }
            }
            return count < MaxConnections;
        }

        public override string ToString() {
            // N E S W
            // ┌ ┐ └ ┘ │ ─
            string doorSymb = (Locked ? "+" : " ");

            string build = "┌" + (Connections[0] ? doorSymb : "─") + "┐\n"; ;
            build += (Connections[3] ? doorSymb : "│") + GetSymbol() + (Connections[1] ? doorSymb : "│");
            build += "\n└" + (Connections[2] ? doorSymb : "─") + "┘";

            return build;
        }

        public string GetSymbol () {
            return PlayerHere ? (Global.Run.Player != null ? Global.Run.Player.Token : "⁰Ω⁰") : Symbol;
        }

        public string GetRoomName() {
            return RoomName;
        }

        public void Enter() {
            Explored = true;
            PlayerHere = true;
            if (EnterAction != null) {
                EnterAction(EnterCount);
            }
            EnterCount++;
        }

        public void Exit() {
            PlayerHere = false;
            if (ExitAction != null) {
                ExitAction(ExitCount);
            }
            ExitCount++;
        }

        public void Active() {
            if (ActivateAction != null) {
                ActivateAction(ActivateCount, MaxActivate);
            } else {
                TextUI.PrintFormatted("There is nothing in the room to activate, press enter to continue");
                Console.ReadLine();
            }
            ActivateCount++;
        }

        public int GetRoomType () {
            return RoomType;
        }

        public virtual string GetDescription () {
            return Description;
        }

        public void SetActivateAction (Action<int, int> act) {
            ActivateAction = act;
        }

        public void SetEnterAction(Action<int> act) {
            EnterAction = act;
        }

        public bool HasEnterAction() {
            return EnterAction != null;
        }

        public virtual void BossDefeated () { }
    }

    public class TRoom {
        public int Type;
        public int[] Data, SubData;

        public TRoom (int type, int[] data = null, int[] subdata = null) {
            Type = type;
            Data = data;
            SubData = subdata;
        }

        public static Room Generate (TRoom template) {
            Room ret = null;
            switch (template.Type) {
                case 0: ret = new Room(); break;
                case 3: ret = new Campfire(new Room()); break;
                case 4: ret = new ShopRoom(new Room(), template.Data[0], template.SubData); break;
                case 5: ret = new Inn(new Room(), template.Data[0]); break;
                case 6: ret = new Cauldron(new Room()); break;
                case 7: ret = new Altar(new Room()); break;
            }
            return ret;
        }
    }
}
