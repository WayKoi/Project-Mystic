using System;
using System.Collections.Generic;
using System.Text;

namespace Card_Test.Tables {
	public static class BossTable {
		public static Boss Beetle = new Boss(new AIEntry[] {
			EnemyTable.Grub,
			EnemyTable.Beetle,
			EnemyTable.Grub
		});

		public static Boss Salamander = new Boss(new AIEntry[] {
			EnemyTable.Worm,
			EnemyTable.Salamander
		});

		public static Boss Camel = new Boss(new AIEntry[] {
			EnemyTable.Camel
		});

		public static Boss Mannequin = new Boss(new AIEntry[] {
			EnemyTable.MLeg,
			EnemyTable.MArm,
			EnemyTable.MBody,
			EnemyTable.MArm,
			EnemyTable.MLeg
		});

		public static Boss Mammoth = new Boss(new AIEntry[] {
			EnemyTable.Mammoth
		});

		public static Boss Demon = new Boss(new AIEntry[] {
			EnemyTable.Demon
		});

		public static Boss Warlock = new Boss(new AIEntry[] {
			EnemyTable.Warlock
		});

	}

	public class Boss {
		public AIEntry[] Enemies;

		public Boss (AIEntry[] enemies) {
			Enemies = enemies;
		}

		public void RunBattle () {
			List<Character> send = new List<Character>();

			foreach (AIEntry tab in Enemies) {
				send.Add(AIEntry.GenEntry(tab));
			}

			Battle batt = new Battle(Global.Run.Players.ToArray(), send.ToArray());
			bool result = batt.Run();

			if (!result) {
				Global.Run.TenFloor.EndGame();
			}
		}
	}
}
