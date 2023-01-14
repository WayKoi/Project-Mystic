using Card_Test.Tables;
using System;
using System.Collections.Generic;
using System.Text;
using Card_Test.Utilities;

namespace Card_Test.Tables {
	public static class Types {

		private static CardType[] Table = {
			// basevals = damage, healing, targettype = 0 is everyone, 1 is friends, 2 is enemies
			// CalcVals = Start, Dip, Sit, Change

			new CardType(0, "Time",   "Tme", new int[] { 1 }, new int[] { 0, 0, 1 }, new double[] { 0, 0, 0, 0 }, 0, 0, Time, true, null, null, new int[] { 1, 4 }),
			new CardType(0, "Vision", "Vis", new int[] { 1 }, new int[] { 0, 0, 2 }, new double[] { 0, 0, 0, 0 }, 0, 0, Vision, true, null, null, new int[] { 0, 0 }),
			new CardType(0, "Shield", "Shl", new int[] { 1 }, new int[] { 0, 0, 1 }, new double[] { 0, 0, 0, 0 }, 0, 0, Shield, false, null, null, new int[] { 0, 4 }),

			new CardType(0, "Physical", "Phy", new int[] { 1 }, new int[] { 10, 0, 0 }, new double[] { 1.5, 0, 0, 0 }),

			// T1 Magic Types
			new CardType(0, "Drop", "₀Drp⁰", new int[] { 0, 3 }, new int[] { 12, 0, 0 }, new double[] { 1.6, 4, 1, 0.1 }, 5),
			new CardType(0, "Char", "³Chr⁰", new int[] { 0, 2 }, new int[] { 15, 0, 0 }, new double[] { 1.7, 3, 2, 0.2 }, 5),
			new CardType(0, "Gust", "⁸Gst⁰", new int[] { 0, 5 }, new int[] { 17, 0, 0 }, new double[] { 1.6, 4, 2, 0.1 }, 2),
			new CardType(0, "Clay", "₂Cly⁰", new int[] { 0, 4 }, new int[] { 14, 0, 0 }, new double[] { 1.5, 2, 3, 0.15 }),

			// T2 Magic Types
			new CardType(1, "Water", "₀Wtr⁰", new int[] { 0, 3 }, new int[] { 17, 0, 0 }, new double[] { 1.6, 4, 1, 0.1 }, 5, 1),
			new CardType(1, "Fire",  "³Fyr⁰", new int[] { 0, 2 }, new int[] { 20, 0, 0 }, new double[] { 1.7, 3, 2, 0.2 }, 5, 1),
			new CardType(1, "Wind",  "⁸Wyn⁰", new int[] { 0, 5 }, new int[] { 21, 0, 0 }, new double[] { 1.6, 4, 2, 0.1 }, 2, 1),
			new CardType(1, "Earth", "₂Ert⁰", new int[] { 0, 4 }, new int[] { 19, 0, 0 }, new double[] { 1.5, 2, 3, 0.1 }),
			new CardType(1, "Heal",  "¹Hea⁰", new int[] { 0, 6 }, new int[] { 0, 14, 0 }, new double[] { 2, 3, 6, 0.25 }, 5),
			new CardType(1, "Spark", "²Spk⁰", new int[] { 0, 7 }, new int[] { 20, 0, 0 }, new double[] { 1.8, 3, 2, 0.2 }),
			new CardType(1, "Flash", "²Fsh⁰", new int[] { 0, 8 }, new int[] { 18, 0, 0 }, new double[] { 2.1, 4, 4, 0.25 }),
			new CardType(1, "Shade", "⁴Sde⁰", new int[] { 0, 9 }, new int[] { 18, 0, 0 }, new double[] { 1.85, 3, 4, 0.2 }),

			// T3 Magic Types
			new CardType(2, "Tsunami", "₀Tsu⁰", new int[] { 0, 3 }, new int[] { 28, 0, 0 }, new double[] { 1.6, 4, 1, 0.1 }, 8, 1),
			new CardType(2, "Inferno", "³Inf⁰", new int[] { 0, 2 }, new int[] { 25, 0, 0 }, new double[] { 1.7, 3, 2, 0.2 }, 8, 1),
			new CardType(2, "Cyclone", "⁸Cyc⁰", new int[] { 0, 5 }, new int[] { 26, 0, 0 }, new double[] { 1.6, 4, 2, 0.1 }, 5, 1),
			new CardType(2, "Terra",   "₂Ter⁰", new int[] { 0, 4 }, new int[] { 24, 0, 0 }, new double[] { 1.5, 2, 3, 0.1 }),
			new CardType(2, "Mend",    "¹Mnd⁰", new int[] { 0, 6 }, new int[] { 0, 19, 0 }, new double[] { 2, 3, 6, 0.25 }),
			new CardType(2, "Bolt",    "²Blt⁰", new int[] { 0, 7 }, new int[] { 25, 0, 0 }, new double[] { 1.8, 3, 2, 0.2 }),
			new CardType(2, "Light",   "²Lyt⁰", new int[] { 0, 8 }, new int[] { 23, 0, 0 }, new double[] { 2.1, 4, 4, 0.25 }),
			new CardType(2, "Dark",    "⁴Drk⁰", new int[] { 0, 9 }, new int[] { 23, 0, 0 }, new double[] { 1.85, 3, 4, 0.2 }),
			new CardType(2, "Dream",   "⁶Drm⁰", new int[] { 0, 10 }, new int[] { 28, 0, 0 }, new double[] { 2, 2, 9, 0.5 }),

			// T4 Magic Types
			new CardType(3, "Thunder", "²Thd⁰", new int[] { 0, 7 }, new int[] { 35, 0, 0 }, new double[] { 1.8, 3, 3, 0.2 }),

			new CardType(1, "Sand",  "₂Snd⁰", new int[] { 0, 5, 4, 12 }, new int[] { 22, 0, 0 }, new double[] { 1.6, 4, 4, 0.1 }, 5),
			new CardType(1, "Ash",   "₁Ash⁰", new int[] { 0, 2, 6 },     new int[] { 24, 0, 0 }, new double[] { 1.5, 5, 4, 0.05 }),
			new CardType(1, "Mud",   "₂Mud⁰", new int[] { 0, 3, 4 },     new int[] { 20, 0, 0 }, new double[] { 1.4, 0, 6, -0.1 }),
			
			new CardType(2, "Dune",    "₂Dne⁰", new int[] { 0, 5, 4, 12 }, new int[] { 28, 0, 0 }, new double[] { 1.6, 4, 4, 0.1 }, 5, 1),
			new CardType(2, "Cinder",  "₁Cin⁰", new int[] { 0, 2, 6 },	 new int[] { 30, 0, 0 }, new double[] { 1.5, 5, 4, 0.05 }),
			new CardType(2, "Swamp",   "₂Swp⁰", new int[] { 0, 3, 4 },	 new int[] { 24, 0, 0 }, new double[] { 1.5, 0, 4, -0.15 }),
			new CardType(2, "Whirl",   "₀Whl⁰", new int[] { 0, 5, 3 },	 new int[] { 28, 0, 0 }, new double[] { 1.8, 3, 3, 0.21 }, 10),
			new CardType(2, "Sprout",  "⁷Spr⁰", new int[] { 0, 4, 6 },	 new int[] { 28, 0, 0 }, new double[] { 1.7, 4, 1, 0.15 }, 10),
			new CardType(2, "Lava",    "₁Lva⁰", new int[] { 0, 4, 2 },	 new int[] { 21, 0, 0 }, new double[] { 2, 4, 3, 0.21 }, 10),
			new CardType(2, "Ice",     "⁵Ice⁰", new int[] { 0, 3, 9, 11 },	 new int[] { 20, 0, 0 }, new double[] { 2.1, 4, 3, 0.25 }, 5),
			new CardType(2, "Crystal", "₄Crs⁰", new int[] { 0, 4, 8 },	 new int[] { 30, 0, 0 }, new double[] { 2.5, 1, 6, 1.35 }),
			new CardType(2, "Vampire", "⁴Vmp⁰", new int[] { 0, 6, 9 },	 new int[] { 30, 0, 0 }, new double[] { 1.2, 0, 4, 0.1 }),
			new CardType(2, "Pixie",   "₄Pix⁰", new int[] { 0, 6, 8 },	 new int[] { 24, 0, 0 }, new double[] { 1.6, 2, 5, 0.15 }),

			new CardType(3, "Pheonix",  "₁Phx⁰", new int[] { 0, 6, 2 },   new int[] { 35, 0, 0 }, new double[] { 1.5, 5, 4, 0.05 }),
			new CardType(3, "Typhoon",  "₀Typ⁰", new int[] { 0, 5, 3 },   new int[] { 34, 0, 0 }, new double[] { 1.8, 3, 3, 0.21 }),
			new CardType(3, "Eclipse",  "⁴Ecl⁰", new int[] { 9, 10, 0 },   new int[] { 38, 0, 0 }, new double[] { 1.4, 0, 4, -0.1 }),
			new CardType(3, "Volcanic", "₁Vol⁰", new int[] { 0, 4, 2 },   new int[] { 27, 0, 0 }, new double[] { 2, 4, 3, 0.21 }),
			new CardType(3, "Plant",    "⁷Pla⁰", new int[] { 0, 4, 6 },   new int[] { 34, 0, 0 }, new double[] { 1.75, 4, 2, 0.15 }),
			new CardType(3, "Tundra",   "⁵Tnr⁰", new int[] { 0, 3, 9, 5, 4, 12, 11 },   new int[] { 34, 0, 0 }, new double[] { 2, 2, 6, 0.4 }),
			new CardType(3, "Glacier",  "⁵Glc⁰", new int[] { 0, 3, 9, 11 },   new int[] { 27, 0, 0 }, new double[] { 2.1, 4, 3, 0.25 }),
			new CardType(3, "Gem",      "₄Gem⁰", new int[] { 0, 4, 8 },   new int[] { 40, 0, 0 }, new double[] { 2.45, 1, 6, 1.3 }),
			new CardType(3, "Storm",    "⁴Srm⁰", new int[] { 0, 3, 5, 7 },   new int[] { 37, 0, 0 }, new double[] { 1.8, 3, 3, 0.2 }),
			new CardType(3, "Fairy",    "₄Fai⁰", new int[] { 0, 6, 8 },   new int[] { 30, 0, 0 }, new double[] { 1.6, 2, 5, 0.15 }),

		};

		public static CardType Search (string name) {
			return Search(Translate(name));
		}

		public static CardType Search(int index) {
			if (index < 0 || index >= Table.Length) { return null; }
			return Table[index];
		}

		public static int[] Translate (string[] types) {
			List<int> translated = new List<int>();

			for (int i = 0; i < types.Length; i++) {
				translated.Add(Translate(types[i]));
			}

			return translated.ToArray();
		}

		public static int Translate (string type) {
			switch (type) {
				case "any": return -1;

				case "time": return 0;
				case "vision": return 1;
				case "shield": return 2;

				case "physical": return 3;
				
				case "drop": return 4;
				case "char": return 5;
				case "gust": return 6;
				case "clay": return 7;

				case "water": return 8;
				case "fire": return 9;
				case "wind": return 10;
				case "earth": return 11;
				case "heal": return 12;
				case "spark": return 13;
				case "flash": return 14;
				case "shade": return 15;

				case "tsunami": return 16;
				case "inferno": return 17;
				case "cyclone": return 18;
				case "terra": return 19;
				case "mend": return 20;
				case "bolt": return 21;
				case "light": return 22;
				case "dark": return 23;
				case "dream": return 24;

				case "thunder": return 25;

				case "sand": return 26;
				case "ash": return 27;
				case "mud": return 28;

				case "dune":	return 29;
				case "cinder":	return 30;
				case "swamp":	return 31;
				case "whirl":	return 32;
				case "sprout":	return 33;
				case "lava":	return 34;
				case "ice":		return 35;
				case "crystal": return 36;
				case "vampire": return 37;
				case "pixie":	return 38;

				case "pheonix":  return 39;
				case "typhoon":  return 40;
				case "eclipse":  return 41;
				case "volcanic": return 42;
				case "plant":    return 43;
				case "tundra":   return 44;
				case "glacier":  return 45;
				case "gem":      return 46;
				case "storm":    return 47;
				case "fairy":    return 48;
			}

			TranslateError(type);
			return -2;
		}

		private static void TranslateError(string translate) {
			TextUI.PrintFormatted("³Could not translate " + translate + "⁰");
			TextUI.Wait();
		}

		public static string Viualize () {
			List<string> colA = new List<string>(), colB = new List<string>(), colC = new List<string>();
			List<string>[] cols = { colA, colB, colC };

			int count = Table.Length / cols.Length;

			int typecount = Table.Length;
			for (int i = 0; i < typecount; i++) {
				cols[i / (count + 1)].Add(i.ToString() + ". " + (i < 10 ? " " : "") + Table[i].Name);
			}

			List<string> combine = new List<string>();

			for (int i = 0; i < cols.Length; i++) {
				combine.Add(string.Join('\n', cols[i]));
			}

			string body = string.Join('\n', TextUI.MakeTable(combine, 3));
			string head = new string(' ', body.Split('\n')[0].Length / 2 - 3) + "Types\n" + new string('-', body.Split('\n')[0].Length - 3) + "\n";

			return head + body;
			
		}

		public static void Vision (Card cast, List<BattleChar> targets, int specific, PlayReport report) {
			report.Additional.Add(targets[specific].Unit.Name + "s plan is revealed!");
			Status stat = new Status(cast, StatusTable.Table[5], targets[specific], 1, 0, 0.5, report);
		}

		public static void Time (Card cast, List<BattleChar> targets, int specific, PlayReport report) {
			int tier = cast.Tier;
			report.Additional.Add(targets[specific].Unit.Name + " draws " + tier + " card" + (tier > 1 ? "s" : ""));
			targets[specific].Unit.DrawCard(tier);
		}

		public static void Shield (Card cast, List<BattleChar> targets, int specific, PlayReport report) {
			double[] mits = { 0.1, 0.25, 0.5, 0.75, 1 };
			// TextUI.PrintFormatted(targets[specific].Unit.Name + " gets a shield");
			report.Affected.Add(targets[specific]);
			report.AffectedEffects.Add(new int[] { 0, 0, 0, 1, -1, 0, 0 });
			targets[specific].Shields.Add(new Shield(mits[(int) Math.Min(cast.Tier, 4)]));
		}
	}

	public class CardType {
		public string Name, Short;
		public int[] BaseTypes;
		
		public bool Instant;

		public int BaseDamage, BaseHealing, TargetType, Tier, StatusChance, StatusAdd;
			// CalcVals = Start, Dip, Sit, Change
		public double[] CalcVals;
		public int[] Damage, Healing;
		
		private Action<Card, List<BattleChar>, int, PlayReport> Additional;
			// restricted mods, submods and tier limits
		public int[] ResMod, ResSub, TierLim;
		public Action<Card> Rules;

		public bool HasStatus = true;
		 
		public CardType (int tier, string name, string shortname, int[] basetypes, int[] basevals, double[] calcvals, int statusBase = 0, int statusAdd = 0, Action<Card, List<BattleChar>, int, PlayReport> additional = null, bool instant = false, int[] resmod = null, int[] ressub = null, int[] tierlim = null, Action<Card> rules = null) {
			Tier = tier;
		
			Name = name;
			Short = shortname;
			BaseTypes = basetypes;
			Instant = instant;

			BaseDamage  = basevals[0];
			BaseHealing = basevals[1];
			TargetType  = basevals[2];

			CalcVals = calcvals;

			StatusChance = statusBase;
			StatusAdd = statusAdd;

			Additional = additional;

			ResMod = resmod;
			ResSub = ressub;
			TierLim = (tierlim != null && tierlim.Length > 1 ? tierlim : new int[] { 1, 9 });
			Rules = rules;
			
			CalcTable();

			HasStatus = GetStatus() != -1;
		}

		private void CalcTable () {
			int tableSize = TierLim[1] - TierLim[0] + 1;
			Damage = new int[tableSize];
			Healing = new int[tableSize];

			double mult  = CalcVals[0];
			int dip = (int) CalcVals[1];
			int sit = (int) CalcVals[2];
			double change = CalcVals[3];

			Damage[0]  = BaseDamage;
			Healing[0] = BaseHealing;

			int spot = 1;
			tableSize--;
			while (tableSize > 0) {
				Damage[spot] = (int)Math.Ceiling(Damage[spot - 1] * mult);
				Healing[spot] = (int)Math.Ceiling(Healing[spot - 1] * mult);

				if (dip > 0) {
					mult -= change;
					dip--;
				} else if (sit > 0) {
					sit--;
				} else {
					mult += change;
				}
				
				spot++;
				tableSize--;
			}
		}

		public int GetDamage(int tier) {
			tier = Math.Max(tier, TierLim[0]);
			tier = Math.Min(tier, TierLim[1]);
			return  Damage[tier - TierLim[0]];
		}

		public int GetHealing(int tier) {
			tier = Math.Max(tier, TierLim[0]);
			tier = Math.Min(tier, TierLim[1]);
			return Healing[tier - TierLim[0]];
		}

		public void CastAdditional(Card card, List<BattleChar> targets, int specific, PlayReport report) {
			if (specific == -1 || !targets[specific].Unit.HasHealth()) { return; }
			if (Additional != null) {
				Additional(card, targets, specific, report);
			}

			int stat = GetStatus();
			if (stat != -1 && card.Sub != 5) {
				// try and apply status
				int attempt = Global.Rand.Next(1, 101);
				if (attempt <= StatusChance + (StatusAdd * (card.Tier - 1))) {
					// success
					int turncount = Math.Min(Global.Rand.Next(1, (card.Tier / 2) + 1), 4);
					Status tem = new Status(card, StatusTable.Table[stat], targets[specific], turncount, card.Tier, 0.5, report);
					report.Additional.Add(targets[specific].Unit.Name + " is afflicted with " + StatusTable.Table[stat].Name);
				}
			}
		}

		public bool HasBaseType(int type) {
			for (int i = 0; i < BaseTypes.Length; i++) {
				if (type == BaseTypes[i]) {
					return true;
				}
			}
			return false;
		}
		public int GetEffect() {
			int effect = 0;

			for (int i = BaseTypes.Length - 1; i >= 0; i--) {
				BaseType test = Tables.BaseTypes.Search(BaseTypes[i]);
				if (test != null && test.Effect != 0) {
					effect = test.Effect;
					break;
				}
			}

			return effect;
		}

		public int GetStatus() {
			int stat = -1;

			for (int i = BaseTypes.Length - 1; i >= 0; i--) {
				BaseType test = Tables.BaseTypes.Search(BaseTypes[i]);
				if (test != null && test.Status != -1) {
					stat = test.Status;
					break;
				}
			}

			return stat;
		}
	}
}
