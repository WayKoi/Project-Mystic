using Card_Test.Base;
using Card_Test.Tables;
using System;
using System.Collections.Generic;
using System.Text;

namespace Card_Test.Map.Rooms {
	public class BattleRoom : Room {
		private BattlePool Chosen = null;
		private bool Happen = true;

		public BattleRoom(Room replace, BattlePool[] events) : base(replace) {
			Symbol = "B";

			RoomType = 0;

			Chosen = ChooseBattleEvent(events);
			EnterAction = ActivateBattle;
		}

		public void ActivateBattle (int times) {
			if (times > 0 || !Happen) { return; }
			Symbol = " ";
			Chosen.RunBattle();
		}

		public override void BossDefeated() {
			Happen = false;
			Symbol = " ";
		}

		public static BattlePool ChooseBattleEvent(BattlePool[] events) {
			if (events == null || events.Length <= 0) { return null; }
			List<Rollable> ret = Rollable.Roll(events, 1);
			return (ret != null && ret.Count > 0) ? (BattlePool) ret[0] : null;
		}
	}
}
