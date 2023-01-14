using System;
using System.Collections.Generic;
using System.Text;
using Card_Test.Map;
using Card_Test.Items;
using Card_Test.Tables;
using Card_Test.Utilities;

namespace Card_Test {
	public static class Global {
		public static Random Rand = new Random();
		public static Current Run;

		public static int Roll (int[] chances) {
			int total = 0;
			foreach (int num in chances) {
				total += num;
			}

			int chosen = Rand.Next(1, total + 1);
			for (int i = 0; i < chances.Length; i++) {
				if (chosen <= chances[i]) {
					return i;
				}
				chosen -= chances[i];
			}

			return -1;
		}
	}
}
