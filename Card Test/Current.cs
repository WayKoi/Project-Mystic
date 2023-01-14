using Card_Test.Map;
using Card_Test.Items;
using System;
using System.Collections.Generic;
using System.Text;

namespace Card_Test {
	// this is where the current runs data will be stored
	public class Current {
		// altar variables
		public int AltarValue = 0, AltarStage = 0, AltarHighValue = 0;
		public Card BestAltarCard = null;

		public List<Character> Players = new List<Character>();
		public Player Player = null;
		public Dungeon TenFloor = new Dungeon();
		public bool InnEvent = false;
	}
}
