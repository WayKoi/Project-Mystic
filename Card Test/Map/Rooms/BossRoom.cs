using System;
using System.Collections.Generic;
using System.Text;

namespace Card_Test.Map.Rooms {
	public class BossRoom : Room {
 		public BossRoom(Room replace, Action<int> BossAction) : base(replace) {
			RoomType = 2;
			Description = "This room has a staircase leading downwards";
			RoomName = "boss";
			Symbol = "³↓⁰";
			MaxConnections = 1;

			EnterAction = BossAction;
		}

		public override void BossDefeated() {
			Symbol = "⁴↓⁰";
		}

	}
}
