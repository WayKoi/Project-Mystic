using Card_Test.Tables;
using System;
using System.Collections.Generic;
using System.Text;

namespace Card_Test.Map.Rooms {
	public class BattleRoom : Room {
		private GameEvent Chosen = null;
		private bool Happen = true;

		public BattleRoom(Room replace, GameEvent[] events) : base(replace) {
			Symbol = "B";

			RoomType = 0;

			Chosen = ChooseBattleEvent(events);
			EnterAction = ActivateBattle;
		}

		public void ActivateBattle (int times) {
			if (times > 0 || !Happen) { return; }
			Symbol = " ";
			Chosen.RunAction();
		}

		public override void BossDefeated() {
			Happen = false;
			Symbol = " ";
		}

		public static GameEvent ChooseBattleEvent(GameEvent[] events) {
			if (events == null || events.Length <= 0) { return null; }

			List<int> odds = new List<int>();
			for (int i = 0; i < events.Length; i++) {
				odds.Add(events[i].Chance);
			}

			int chosen = Global.Roll(odds.ToArray());

			return events[chosen];
		}
	}
}
