using Card_Test.Tables;
using System;
using System.Collections.Generic;
using System.Text;

namespace Card_Test.Map.Rooms {
	public class ShopRoom : Room {
		private Shop Content;

		public ShopRoom(Room replace, int tier, int[] typeWeights) : base(replace) {
			RoomType = 4;
			Description = "There is a hunched figure sitting behind a counter, they beckon for you to buy something from them";
			RoomName = "shop";
			Symbol = "⁷$⁰";

			RoomType = 4;
			ActivateAction = Shop;
			Content = new Shop(tier, -1, typeWeights);
			MaxConnections = 2;

			if (Content.Type == 2) {
				Description = "There are racks of clothing, weapons and satchels around the room";
				RoomName = "Tailor";
				Symbol = "⁶T⁰";
				MaxConnections = 1;
			}
		}

		public void Shop (int amt, int max) {
			Content.StartShopping();
		}

        public override void BossDefeated() {
			Content.Open = false;
        }
    }
}
