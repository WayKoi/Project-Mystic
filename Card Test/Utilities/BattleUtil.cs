using System;
using System.Collections.Generic;
using System.Text;

namespace Card_Test.Utilities {
	public static class BattleUtil {
		public static List<int> GetFromSide (int side, List<BattleChar> targets) {
			List<int> ret = new List<int>();

			for (int i = 0; i < targets.Count; i++) {
				if (targets[i].Side == side) {
					ret.Add(i);
				}
			}

			return ret;
		}

		public static BattleChar FindCharacter(List<BattleChar> batts, Character find) {
			int battCount = batts.Count;
			
			for (int i = 0; i < battCount; i++) {
				if (batts[i].Unit == find) {
					return batts[i];
				}
			}

			return null;
		}

	}
}
