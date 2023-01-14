using System;
using System.Collections.Generic;
using System.Text;

namespace Card_Test.Map.Rooms {
	public class StartRoom : Room {
		public StartRoom (Room replace) : base (replace) {
			RoomType = 1;
			Description = "This room has a staircase leading upwards";
			RoomName = "start";
			Symbol = "⁴↑⁰";

			Explored = true;
			PlayerHere = true;
		}
	}
}
