using Card_Test.Tables;
using Card_Test.Utilities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Card_Test.Tables {
	public static class Fusions {
		// add an effect to the card, or change the element of a card with a tier change
		// if fusion attempted doesn't exists then the most recent card in the fusion gets returned
		public static Fusion[] Elements = {
			// Base Type Fusions (something + itself or same tier upgrader)
			new Fusion("drop", "drop", BaseTierChange, "water"),
			new Fusion(new string[] { "char" }, new string[] { "char", "gust" }, BaseTierChange, "fire"),
			new Fusion("gust", "gust", BaseTierChange, "wind"),
			new Fusion("clay", "clay", BaseTierChange, "earth"),

			new Fusion("water", "water", BaseTierChange, "tsunami"),
			new Fusion(new string[] { "fire" }, new string[] { "fire", "wind" }, BaseTierChange, "inferno"),
			new Fusion("wind", "wind", BaseTierChange, "cyclone"),
			new Fusion("earth", "earth", BaseTierChange, "terra"),
			new Fusion("heal", "heal", BaseTierChange, "mend"),
			new Fusion("spark", "spark", BaseTierChange, "bolt"),
			new Fusion("flash", "flash", BaseTierChange, "light"),
			new Fusion("shade", "shade", BaseTierChange, "dark"),

			new Fusion("bolt", "bolt", BaseTierChange, "thunder"),

			// Upgrading fusions (like char + fire = fire but +1 tier)
			
			new Fusion("char", "fire", UpdgradeTierChange, "fire"),
			new Fusion("drop", "water", UpdgradeTierChange, "water"),
			new Fusion("gust", "wind", UpdgradeTierChange, "wind"),
			new Fusion("clay", "earth", UpdgradeTierChange, "earth"),

			new Fusion(new string[] { "char", "fire", "inferno" }, "inferno", UpdgradeTierChange, "inferno"),
			new Fusion(new string[] { "drop", "water", "tsunami" }, "tsunami", UpdgradeTierChange, "tsunami"),
			new Fusion(new string[] { "gust", "wind", "cyclone" }, "cyclone", UpdgradeTierChange, "cyclone"),
			new Fusion(new string[] { "clay", "earth", "terra" }, "terra", UpdgradeTierChange, "terra"),

			new Fusion("spark", "bolt", UpdgradeTierChange, "bolt"),
			new Fusion("bolt", "thunder", UpdgradeTierChange, "thunder"),
			new Fusion("flash", "light", UpdgradeTierChange, "light"),
			new Fusion("shade", "dark", UpdgradeTierChange, "dark"),
			new Fusion("heal", "mend", UpdgradeTierChange, "mend"),

			new Fusion("char", "ash", UpdgradeTierChange, "ash"),
			new Fusion(new string[] { "ash", "fire", "heal" }, "cinder", UpdgradeTierChange, "cinder"),
			new Fusion(new string[] { "ash", "cinder", "inferno", "fire", "heal", "mend" }, "pheonix", UpdgradeTierChange, "pheonix"),

			new Fusion(new string[] { "clay", "drop" }, "mud", UpdgradeTierChange, "mud"),
			new Fusion(new string[] { "mud", "earth", "water" }, "swamp", UpdgradeTierChange, "swamp"),
			
			new Fusion(new string[] { "clay", "gust" }, "sand", UpdgradeTierChange, "sand"),
			new Fusion(new string[] { "sand", "wind", "earth" } , "dune", UpdgradeTierChange, "dune"),

			new Fusion(new string[] { "water", "wind" }, "whirl", UpdgradeTierChange, "whirl"),
			new Fusion(new string[] { "earth", "heal" }, "sprout", UpdgradeTierChange, "sprout"),
			new Fusion(new string[] { "earth", "fire" }, "lava", UpdgradeTierChange, "lava"),
			new Fusion(new string[] { "water", "shade" }, "ice", UpdgradeTierChange, "ice"),
			new Fusion(new string[] { "earth", "flash" }, "crystal", UpdgradeTierChange, "crystal"),
			new Fusion(new string[] { "heal", "shade" }, "vampire", UpdgradeTierChange, "vampire"),
			new Fusion(new string[] { "heal", "flash" }, "pixie", UpdgradeTierChange, "pixie"),

			new Fusion(new string[] { "water", "wind", "whirl", "cyclone", "tsunami", "typhoon" }, "typhoon", UpdgradeTierChange, "typhoon"),
			new Fusion(new string[] { "flash", "light", "shade", "dark", "eclipse" }, "eclipse", UpdgradeTierChange, "eclipse"),
			new Fusion(new string[] { "earth", "fire", "lava", "inferno", "terra", "volcanic" }, "volcanic", UpdgradeTierChange, "volcanic"),
			new Fusion(new string[] { "water", "sprout", "terra", "earth", "tsunami", "flash", "light", "heal", "mend", "plant" }, "plant", UpdgradeTierChange, "plant"),
			new Fusion(new string[] { "earth", "dune", "wind", "ice", "glacier", "terra", "cyclone", "shade", "dark", "tundra" }, "tundra", UpdgradeTierChange, "tundra"),
			new Fusion(new string[] { "ice", "glacier", "shade", "dark", "water", "tsunami" }, "glacier", UpdgradeTierChange, "glacier"),
			new Fusion(new string[] { "flash", "light", "gem", "earth", "terra", "crystal" }, "gem", UpdgradeTierChange, "gem"),
			new Fusion(new string[] { "spark", "bolt", "thunder", "whirl", "typhoon", "wind", "cyclone", "water", "tsunami" }, "storm", UpdgradeTierChange, "storm"),
			new Fusion(new string[] { "flash", "light", "pixie", "heal", "mend" }, "fairy", UpdgradeTierChange, "fairy"),
			

			// Fusion Type Fusions
				// Sand
			new Fusion(new string[] { "gust" }, new string[] { "clay", "earth" }, BaseTierChange, "sand"),
			new Fusion("wind", "clay", BaseTierChange, "sand"),
				// Ash
			new Fusion("char", "heal", BaseTierChange, "ash"),
				// Mud
			new Fusion(new string[] { "drop" }, new string[] { "clay", "earth" }, BaseTierChange, "mud"),
			new Fusion("clay", "water", BaseTierChange, "mud"),
				// Dune
			new Fusion(new string[] { "sand" }, new string[] { "sand", "wind", "earth", "cyclone", "terra" }, BaseTierChange, "dune"),
			new Fusion(new string[] { "wind" }, new string[] { "earth", "terra" }, BaseTierChange, "dune"),
			new Fusion("earth", "cyclone", BaseTierChange, "dune"),
				// Cinder
			new Fusion(new string[] { "ash" }, new string[] { "ash", "fire", "heal", "inferno", "mend" }, BaseTierChange, "cinder"),
			new Fusion(new string[] { "fire" }, new string[] { "heal", "mend" }, BaseTierChange, "cinder"),
			new Fusion("heal", "inferno", BaseTierChange, "cinder"),
				// Swamp
			new Fusion(new string[] { "mud" }, new string[] { "mud", "water", "earth", "tsunami", "terra" }, BaseTierChange, "swamp"),
			new Fusion(new string[] { "water" }, new string[] { "earth", "terra" }, BaseTierChange, "swamp"),
			new Fusion("earth", "tsunami", BaseTierChange, "swamp"),
				// Whirl
			new Fusion(new string[] { "water" }, new string[] { "wind", "cyclone" }, BaseTierChange, "whirl"),
			new Fusion("wind", "tsunami", BaseTierChange, "whirl"),
				// Sprout
			new Fusion(new string[] { "earth" }, new string[] { "heal", "mend" }, BaseTierChange, "sprout"),
			new Fusion("heal", "terra", BaseTierChange, "sprout"),
				// Lava
			new Fusion(new string[] { "fire" }, new string[] { "earth", "terra" }, BaseTierChange, "lava"),
			new Fusion("inferno", "earth", BaseTierChange, "lava"),
				// Ice
			new Fusion(new string[] { "water" }, new string[] { "shade", "dark" }, BaseTierChange, "ice"),
			new Fusion("tsunami", "shade", BaseTierChange, "ice"),
				// Crystal
			new Fusion(new string[] { "earth" }, new string[] { "flash", "light" }, BaseTierChange, "crystal"),
			new Fusion("flash", "terra", BaseTierChange, "crystal"),
				// Vampire
			new Fusion(new string[] { "heal" }, new string[] { "shade", "dark" }, BaseTierChange, "vampire"),
			new Fusion("mend", "shade", BaseTierChange, "vampire"),
				// Pixie
			new Fusion(new string[] { "heal" }, new string[] { "flash", "light" }, BaseTierChange, "pixie"),
			new Fusion("mend", "flash", BaseTierChange, "pixie"),
				
				// pheonix
			new Fusion("cinder", new string[] { "inferno", "mend", "cinder" }, BaseTierChange, "pheonix"),
			new Fusion("inferno", "mend", BaseTierChange, "pheonix"),
				// Typhoon
			new Fusion("whirl", new string[] { "cyclone", "tsunami", "whirl" }, BaseTierChange, "typhoon"),
			new Fusion("tsunami", "cyclone", BaseTierChange, "typhoon"),
				// Eclipse
			new Fusion("light", "dark", BaseTierChange, "eclipse"),
				// Volcanic
			new Fusion("lava", new string[] { "lava", "terra", "inferno" }, BaseTierChange, "volcanic"),
			new Fusion("terra", "inferno", BaseTierChange, "volcanic"),
				// plant
			new Fusion("sprout", new string[] { "terra", "mend", "sprout" }, BaseTierChange, "plant"),
			new Fusion("terra", "mend", BaseTierChange, "plant"),
				// Tundra
			new Fusion("dune", new string[] { "ice", "glacier" }, BaseTierChange, "tundra"),
				// glacier
			new Fusion("ice", new string[] { "ice", "dark", "tsunami" }, BaseTierChange, "glacier"),
			new Fusion("tsunami", "dark", BaseTierChange, "glacier"),
				// Gem
			new Fusion("crystal", new string[] { "crystal", "terra", "light" }, BaseTierChange, "gem"),
			new Fusion("terra", "light", BaseTierChange, "gem"),
				// storm
			new Fusion(new string[] { "bolt", "thunder" }, new string[] { "whirl", "typhoon" }, BaseTierChange, "storm"),
				// fairy
			new Fusion("pixie", new string[] { "pixie", "light", "mend" }, BaseTierChange, "fairy"),
			new Fusion("mend", "light", BaseTierChange, "fairy"),

			// Strange Fusions
				// time card adds a sub mod
			new Fusion("time", "any", TimeTierChange, "any", null, TimeSubFusion),
				// physical + anything = anything with tier + 1
			new Fusion("physical", "any", PhysTierChange, "any")
		};

		public static ModFusion[] Mods = {
			new ModFusion(1, 1, 2),
			new ModFusion(3, -1, 3)
		};

		private static void TimeSubFusion(Card A, Card B, Card Result) {
			Result.ChangeSub(SubMods.Translate("plusone"));
		}

		public static Card Fuse (Card A, Card B) {
            for (int i = 0; i < Elements.Length; i++) {
                bool[,] SideCheck = new bool[2, 2];

                for (int ii = 0; ii < Elements[i].A.Length; ii++) {
                    SideCheck[0, 0] = Elements[i].A[ii] == A.Type || Elements[i].A[ii] == -1 || SideCheck[0, 0];
                    SideCheck[1, 0] = Elements[i].A[ii] == B.Type || Elements[i].A[ii] == -1 || SideCheck[1, 0];
                }

                for (int ii = 0; ii < Elements[i].B.Length; ii++) {
                    SideCheck[0, 1] = Elements[i].B[ii] == A.Type || Elements[i].B[ii] == -1 || SideCheck[0, 1];
                    SideCheck[1, 1] = Elements[i].B[ii] == B.Type || Elements[i].B[ii] == -1 || SideCheck[1, 1];
                }

                // sidecheck
                //    A    B
                // A bool bool
                // B bool bool

                if ((SideCheck[0, 0] && SideCheck[1, 1]) || (SideCheck[0, 1] && SideCheck[1, 0])) {
                    // mod fusion occurs
                    int resType = Elements[i].OutType;

                    // if resType == -1 then the type that is in the B array is prioritized
                    if (resType == -1) {
                        if (SideCheck[0, 1] && SideCheck[1, 0]) {
                            resType = A.Type;
                        } else {
                            resType = B.Type;
                        }
                    }

                    Card Result = new Card(resType, 1);

					Elements[i].TierChange(A, B, Result);

                    Elements[i].FuseMod(A, B, Result);
					Elements[i].FuseSub(A, B, Result);
					Elements[i].RunAdditional(Result);

                    return Result;
                }
            }

            return new Card(B);
		}

		public static void UpdgradeTierChange (Card A, Card B, Card Result) {
			int start = A.Element.Tier > B.Element.Tier ? A.Tier : B.Tier;
			int change = Math.Min(Math.Min(A.Element.Tier + 1, B.Element.Tier + 1), 2);

			Result.ChangeTier(start + change);
		}

		public static void BaseTierChange(Card A, Card B, Card Result) {
			int change = Math.Min(A.Tier + B.Tier - 2, Math.Max(A.Tier + 2, B.Tier + 2));

			Result.ChangeTier(change);
		}

		public static void TimeTierChange(Card A, Card B, Card Result) {
			int time = Types.Translate("time");
			int start = A.Type != time ? A.Tier : B.Tier;
			start = A.Type == time && B.Type == time ? Math.Max(A.Tier, B.Tier) : start;

			Result.ChangeTier(start);
		}

		public static void PhysTierChange(Card A, Card B, Card Result) {
			int phys = Types.Translate("physical");
			int start = A.Type != phys ? A.Tier : B.Tier;
			start = A.Type == phys && B.Type == phys ? Math.Max(A.Tier, B.Tier) : start;

			Result.ChangeTier(start + 1);
		}

		public static void BaseSubFusion(Card A, Card B, Card Result) {
			if (A.Sub == 0 && B.Sub == 0) {
				return;
			}

			if (A.Sub > 0 ^ B.Sub > 0) {
				Result.ChangeSub(A.Sub + B.Sub);
				return;
			}
		}

		public static void BaseModFusion (Card A, Card B, Card Result) {
			// -1 means any mod
			// if one of A or B has a mod then the mod will be transferred to the result
			// if both have mods
			// check the modfusion table
			// if not found it will give the result card the mod of B

			if (A.Mod == 0 && B.Mod == 0) {
				return;
			}

			// exclusive check for existance of a mod
			if (A.Mod > 0 ^ B.Mod > 0) {
				Result.ChangeMod(A.Mod + B.Mod);
				return;
			}

			bool fusionHappend = false;
			for (int i = 0; i < Mods.Length; i++) {
				bool[,] SideCheck = new bool[2, 2];
				
				for (int ii = 0; ii < Mods[i].A.Length; ii++) {
					SideCheck[0, 0] = Mods[i].A[ii] == A.Mod || Mods[i].A[ii] == -1 || SideCheck[0, 0];
					SideCheck[1, 0] = Mods[i].A[ii] == B.Mod || Mods[i].A[ii] == -1 || SideCheck[1, 0];
				}

				for (int ii = 0; ii < Mods[i].B.Length; ii++) {
					SideCheck[0, 1] = Mods[i].B[ii] == A.Mod || Mods[i].B[ii] == -1 || SideCheck[0, 1];
					SideCheck[1, 1] = Mods[i].B[ii] == B.Mod || Mods[i].B[ii] == -1 || SideCheck[1, 1];
				}

				// sidecheck
				//    A    B
				// A bool bool
				// B bool bool

				if ((SideCheck[0, 0] && SideCheck[1, 1]) || (SideCheck[0, 1] && SideCheck[1, 0])) {
					// mod fusion occurs
					Result.ChangeMod(Mods[i].Mod);
					fusionHappend = true;
					break;
				}
			}

			if (!fusionHappend) {
				Result.ChangeMod(B.Mod);
			}
		}
	}

	public class Fusion {
		public int[] A, B;
		public int OutType; // outtype = -1 means that the type is just the type of the card in B
				   // in 1  in 2  out
		public Action<Card, Card, Card> FuseMod;
		public Action<Card, Card, Card> FuseSub;
		public Action<Card, Card, Card> TierChange;
					// out   data
		private Action<Card, int[]> Additional;
		private int[] AdditionalData;

		private Fusion (Action<Card, Card, Card> tierChange, string outType, Action<Card, Card, Card> modfusion = null, Action<Card, Card, Card> subfusion = null, Action<Card, int[]> additional = null, int[] additionalData = null) {
			TierChange = tierChange;
			OutType = Types.Translate(outType);

			FuseMod = modfusion;
			if (FuseMod == null) {
				FuseMod = Fusions.BaseModFusion;
			}

			FuseSub = subfusion;
			if (FuseSub == null) {
				FuseSub = Fusions.BaseSubFusion;
			}

			Additional = additional;
		}

		public Fusion(string[] a, string[] b, Action<Card, Card, Card> tierChange, string outType, Action<Card, Card, Card> modfusion = null, Action<Card, Card, Card> subfusion = null, int[] additionalData = null, Action<Card, int[]> additional = null) : this(tierChange, outType, modfusion, subfusion, additional, additionalData) {
			A = new int[a.Length];
			B = new int[b.Length];

			for (int i = 0; i < a.Length; i++) {
				A[i] = Types.Translate(a[i]);
			}

			for (int i = 0; i < b.Length; i++) {
				B[i] = Types.Translate(b[i]);
			}
		}

		public Fusion(string a, string b, Action<Card, Card, Card> tierChange, string outType, Action<Card, Card, Card> modfusion = null, Action<Card, Card, Card> subfusion = null, int[] additionalData = null, Action<Card, int[]> additional = null) : this(new string[] { a }, new string[] { b }, tierChange, outType, modfusion, subfusion, additionalData, additional) { }

		public Fusion(string[] a, string b, Action<Card, Card, Card> tierChange, string outType, Action<Card, Card, Card> modfusion = null, Action<Card, Card, Card> subfusion = null, int[] additionalData = null, Action<Card, int[]> additional = null) : this(a , new string[] { b }, tierChange, outType, modfusion, subfusion, additionalData, additional) { }

		public Fusion(string a, string[] b, Action<Card, Card, Card> tierChange, string outType, Action<Card, Card, Card> modfusion = null, Action<Card, Card, Card> subfusion = null, int[] additionalData = null, Action<Card, int[]> additional = null) : this(new string[] { a }, b, tierChange, outType, modfusion, subfusion, additionalData, additional) { }

		public void RunAdditional (Card Result) {
			if (Additional != null) {
				Additional(Result, AdditionalData);
			}
		}
	}

	public class ModFusion {
		public int[] A, B;
		public int Mod;

		private ModFusion (int modout) {
			Mod = modout;
		}

		public ModFusion(int a, int b, int modout) : this (modout) {
			A = new int[] { a };
			B = new int[] { b };
		}

		public ModFusion(int[] a, int[] b, int modout) : this(modout) {
			A = a;
			B = b;
		}
	}
}
