using System;
using System.Collections.Generic;
using System.Text;

namespace Card_Test.Tables {
	public static class Reactions {
		public static Reaction NoReaction = new Reaction("None", null, null, 1);

		public static Reaction[] Table = {
            new Reaction("Swamped", new int[] { 2, 3 }, new int[] { 3, 2 }, 1.2),
            new Reaction("Extinguish", 1, 2, 0.8),
            new Reaction("Scald", 2, 1, 1.3),
            new Reaction("Combust", new int[] { 1, 9, 9 },new int[] { 4, 1, 10 }, 1.5),
            new Reaction("Frost Burnt", new int[] { 5, 1 }, new int[] { 1, 5 }, 1.5),
            new Reaction("Dehydrate", new int[] { 2, 6, 9 }, new int[] { 6, 4, 6 }, 1.25),
            new Reaction("Deep Freeze", new int[] { 2, 9 },new int[] { 5, 5 }, 1.5),
            new Reaction("Up-Root", 9, 4, 1.8),
            new Reaction("Overgrow", new int[] { 2, 9, 9 },new int[] { 9, 2, 8 }, 1.75),
            new Reaction("Conduct", new int[] { 2, 6, 5 }, new int[] { 10, 10, 10 }, 1.5),
            new Reaction("Ground", 3, 10, 0.5),
            new Reaction("Clean", 3, 4, 0.8),
            new Reaction("Rehydrate", 6, 2, 0.8),
            new Reaction("Blind", new int[] { 7, 8 }, new int[] { 8, 7 }, 2)
		};
		
		public static Reaction GetReaction (int A, int B) {
			foreach (Reaction react in Table) {
				for (int i = 0; i < react.A.Length; i++) {
					if (react.A[i] == A && react.B[i] == B) {
						return react;
					}
				}
			}

			return NoReaction;
		}

		public static int GetReactionIndex (Reaction react) {
			if (react == null) { return -1; }
			for (int i = 0; i < Table.Length; i++) {
				if (react == Table[i]) {
					return i;
				}
			}
			return -1;
		}
	}

	public class Reaction {
		public string Name;
		public int[] A, B;
		public double Mult;

		public Reaction (string name, int a, int b, double mult) {
			Name = name;
			A = new int[] { a };
			B = new int[] { b };
			Mult = mult;
		}

		public Reaction(string name, int[] a, int[] b, double mult) {
			Name = name;
			A = a;
			B = b;
			Mult = mult;
		}
	}
}
