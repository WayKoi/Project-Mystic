using System;
using System.Collections.Generic;
using System.Text;

namespace Card_Test.Tables {
    /*public static class EventTable {

        // table to pull from, min amount of enemies, max amount of enemies
        public static GameEvent[] BTableA = {
            new GameEvent(new int[] { 0, 1, 1 }, BattleEvent, 80),
            new GameEvent(new int[] { 0, 2, 2 }, BattleEvent, 20)
        };

        public static GameEvent[] BTableB = {
            new GameEvent(new int[] { 1, 1, 1 }, BattleEvent, 50),
            new GameEvent(new int[] { 1, 2, 2 }, BattleEvent, 45),
            new GameEvent(new int[] { 2, 1, 1 }, BattleEvent, 5)
        };

        public static GameEvent[] BTableC = {
            new GameEvent(new int[] { 3, 1, 1 }, BattleEvent, 20),
            new GameEvent(new int[] { 3, 2, 2 }, BattleEvent, 75),
            new GameEvent(new int[] { 4, 1, 1 }, BattleEvent, 5)
        };

        public static GameEvent[] BTableD = {
            new GameEvent(new int[] { 5, 1, 2 }, BattleEvent, 55),
            new GameEvent(new int[] { 5, 2, 3 }, BattleEvent, 40),
            new GameEvent(new int[] { 6, 1, 1 }, BattleEvent, 5)
        };

        public static GameEvent[] BTableE = {
            new GameEvent(new int[] { 7, 1, 1 }, BattleEvent, 15),
            new GameEvent(new int[] { 8, 2, 3 }, BattleEvent, 40),
            new GameEvent(new int[] { 9, 1, 2 }, BattleEvent, 40),
            new GameEvent(new int[] { 10, 1, 1 }, BattleEvent, 5)
        };

        public static GameEvent[] ETableA = {
            
        };

        public static GameEvent[] ETableLookup(int index) {
            switch (index) {
                case 0: return ETableA;
            }

            return null;
        }

        public static GameEvent[] BTableLookup(int index) {
            switch (index) {
                case 0: return BTableA;
                case 1: return BTableB;
                case 2: return BTableC;
                case 3: return BTableD;
                case 4: return BTableE;
            }

            return null;
        }

        public static void BattleEvent(int[] data) {
            if (data.Length < 3) { return; } // table to pull from, min amount of enemies, max amount of enemies
            List<Character> enemies = new List<Character>();
            int amount = Global.Rand.Next(data[1], data[2] + 1);
            
            while (amount > 0) {
                enemies.Add(new CardAI(BattleTable.GetEnemy(BattleTable.GetTable(data[0]))));
                amount--;
            }

            Battle batt = new Battle(Global.Run.Players.ToArray(), enemies.ToArray());
            bool result = batt.Run();

            if (!result) {
                Global.Run.TenFloor.EndGame();
            }
        }

        public static void RegularEvent(int[] data) {
            if (data.Length < 1) { return; } // Table index
            GameEvent[] selected = ETableLookup(data[0]);
            if (selected == null || selected.Length == 0) { return; }

            int chosen = Global.Rand.Next(1, 101);
            foreach (GameEvent entry in selected) {
                if (chosen <= entry.Chance) {
                    entry.RunAction();
                    return;
                }
                chosen -= entry.Chance;
            }
        }

    }

    public class GameEvent {
        public int[] Data;
        public Action<int[]> EventAction;
        public int Chance;

        public GameEvent(int[] data, Action<int[]> eventaction, int chance = 0) {
            Data = data;
            EventAction = eventaction;
            Chance = chance;
        }

        public void RunAction() {
            EventAction(Data);
        }
    }*/
}
