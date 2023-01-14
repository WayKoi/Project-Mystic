using Card_Test.Items;
using Card_Test.Tables;
using Sorting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Card_Test.Tables {
	public static class GearTable {
		// ┌ ┐ └ ┘ │ ─ ╒ ═ ╕ ╤ ┤ ├ ┴ ╘ ╧ ╛
		public static string TempVisual =
			"┌─────────┐\n" +
			"│         │\n" +
			"│         │\n" +
			"│Temporary│\n" +
			"│         │\n" +
			"│         │\n" +
			"└─────────┘";

		public static string[] Tango = new string[] {
			"tango/goggles",
			"tango/cloak",
			"tango/grimoire",
			"satchel"
		};

		public static string[] Mia = new string[] {
			"mia/hat",
			"mia/robe",
			"mia/staff",
			"satchel"
		};

		public static string[] Rich = new string[] {
			"rich/cufflings",
			"rich/suit",
			"rich/gloves",
			"satchel"
		};

		public static string[] Wendy = new string[] {
			"wendy/necklace",
			"wendy/coat",
			"wendy/orb",
			"satchel"
		};

		public static string[] Marco = new string[] {
			"marco/cap",
			"marco/uniform",
			"marco/bat",
			"satchel"
		};

		public static string[] Nix = new string[] {
			"nix/crown",
			"nix/tunic",
			"nix/staff",
			"satchel"
		};

		public static string[] Wyatt = new string[] {
			"wyatt/boots",
			"wyatt/coat",
			"wyatt/rod",
			"satchel"
		};

		public static string[] Rod = new string[] {
			"rod/coat",
			"rod/ring",
			"rod/backpack"
		};

	}

	public class TGear {
		public string Name, Visual;
		public List<TGearEffect> Effects;
		public List<TGearReadEffect> Read;
		public int[] Rolls;
		public int Upgrades = 0, MaxUpgrades;
		public bool Enchanted = false;

		public TGear (string name, string visual, List<TGearEffect> effects, int[] rolls = null, int maxUpgrades = -1) {
			Name = name;
			Visual = visual;
			Effects = effects;
			Rolls = rolls;
			MaxUpgrades = maxUpgrades;
		}

		public static Gear Generate (TGear gear, Character Owner) {
			Gear gen = new Gear(Owner, gear.Name, gear.Visual, gear.Effects.ToList(), gear.Rolls, gear.MaxUpgrades);

			if (gear.Read != null) {
				for (int i = 0; i < gear.Read.Count; i++) {
					GearEffect eff = TGearEffect.GenerateEffect(Owner, gear.Read[i].Type, gear.Read[i].Amount, BaseTypes.Translate(gear.Read[i].AffType));
					gen.Effects.Add(eff);
				}
			}

			gen.Enchanted = gear.Enchanted;
			gen.Upgrades = gear.Upgrades;

			return gen;
		}

		public override string ToString() {
			List<string> print = new List<string>();
			print.Add(Name + "\n" + Visual + "\n" + string.Join(' ', Rolls) + "\nUpgrades " + Upgrades + " : Max " + MaxUpgrades);

			List<string> eff = new List<string>();
			List<string> chances = new List<string>();

			string build = "";

			for (int i = 0; i < Effects.Count; i++) {
				string[] chop = Effects[i].ToString().Split('\n');
				eff.Add((i + 1).ToString() + ". " + chop[0]);
				chances.Add(chop[1]);
			}

			if (eff.Count == 0) {
				build += "\nNo Effects";
			} else {
				string[] tem = new string[] {
					string.Join('\n', eff),
					string.Join('\n', chances)
				};

				build = "\n" + string.Join('\n', TextUI.MakeTable(tem.ToList(), 2));
			}

			print.Add(build);

			return String.Join('\n', TextUI.MakeTable(print));
		}

		public void AddEffect(TGearEffect eff) {
			Effects.Add(eff);
			SortEffects();
		}

		public void SortEffects() {
			Sort.BubbleSort(Effects, Compare.TGearEffect);
		}


	}

	public class TGearReadEffect {
		public int Type, Amount;
		public string AffType;

		public TGearReadEffect (int type, int amount, string afftype = "magic") {
			Type = type;
			Amount = amount;
			AffType = afftype;
		}
	}
}
