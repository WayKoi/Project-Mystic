using Card_Test.Tables;
using Card_Test.Base;
using Sorting;
using System;
using System.Collections.Generic;
using System.Text;

namespace Card_Test.Items {
	public class Gear {
		public string Name, Visual;
		public int Upgrades = 0, MaxUpgrades; // maxupgrades == 0 means no limit

		public Character Owner;

		public List<TGearEffect> Possible = new List<TGearEffect>();
		public List<GearEffect> Effects = new List<GearEffect>();
		public int[] Rolls; // how many upgrades happen on each upgrade
			// for instance, if Rolls = { 3, 2, 1, 1, 2 } that means that the first time the gear is upgraded
			// it will roll effects 3 times, and the next time it will roll 2 times etc.

		public bool Enchanted = false;

		public Gear (Character owner, string name, string visual, List<TGearEffect> possible, int[] rolls, int maxupgrade = -1) {
			Owner = owner;
			Name = name;
			Visual = visual;
			// Possible = possible;

			for (int i = 0; i < possible.Count; i++) {
				Possible.Add(new TGearEffect(possible[i]));
			}

			Rolls = rolls;

			MaxUpgrades = maxupgrade;
		}

		public bool Upgrade (bool stop = false) {
			if (MaxUpgrades != 0 && Upgrades > MaxUpgrades) {
				TextUI.PrintFormatted(Name + " cannot be upgraded further");
				return false;
			}

			if (stop) {
				Console.Clear();
			}

			TextUI.PrintFormatted(Name + " gets upgraded!\n");

			List<string> print = new List<string>();
			string before = ToString();
			print.Add(before);
			print.Add(new string('\n', (int) Math.Ceiling(before.Split('\n').Length / 2.0)) + "->");

			List<Rollable> possible = new List<Rollable>();

			for (int i = 0; i < Possible.Count; i++) {
				possible.Add(Possible[i]);
			}

			List<Rollable> ret = Rollable.Roll(possible, Rolls[Math.Min(Upgrades, Rolls.Length - 1)]);

			while (ret.Count > 0) {
				GearEffect found = null;

				for (int i = 0; i < Effects.Count; i++) {
					if (TGearEffect.Compare((TGearEffect) ret[0], Effects[i])) {
						found = Effects[i];
						break;
					}
				}

				if (found == null) {
					Effects.Add(TGearEffect.GenerateEffect((TGearEffect)ret[0], Owner));
					Sort.BubbleSort(Effects, Compare.GearEffect);
				} else {
					TGearEffect.AddToEffect(Owner, (TGearEffect) ret[0], found);
				}

				ret.RemoveAt(0);
			}

			print.Add(ToString());
			TextUI.PrintFormatted(String.Join('\n', TextUI.MakeTable(print)) + "\n");

			if (stop) {
				TextUI.Wait();
			}

			Upgrades++;

			return true;
		}

		public override string ToString() {
			List<string> print = new List<string>();
			print.Add(Name + "\n" + Visual);

			string build = "";
			for (int i = 0; i < Effects.Count; i++) {
				build += "\n" + Effects[i].ToString();
			}

			if (build.Equals("")) {
				build += "\nNo Effects";
			}

			print.Add(build);

			return String.Join('\n', TextUI.MakeTable(print));
		}

		public List<string> ToFileLines () {
			List<string> lines = new List<string>();

			lines.Add(Name);
			lines.Add(Upgrades + " " + MaxUpgrades + (Enchanted ? " 1" : ""));
			lines.Add(string.Join(' ', Rolls));
			
			for (int i = 0; i < Possible.Count; i++) {
				lines.Add(Possible[i].ToFileLine());
			}

			// Enchants
			if (Effects.Count > 0 /* || enchant */) {
				lines.Add("#");
				
			}

			// Gear effects
			if (Effects.Count > 0) {
				lines.Add("#");

				for (int i = 0; i < Effects.Count; i++) {
					lines.Add(Effects[i].ToFileLine());
				}	
			}

			return lines;
		}
	}

	public class TGearEffect : Rollable {
		public static string[] TypeNames = { "Affinity", "Resistance", "Mana", "Max Health", "Deck Size", "Trunk Size", "Reaction Percent" };
		
		public int Type;
		public int Min, Max, AffType = 0;

		public TGearEffect (int type, int[] data, int chance, int maxrolls = 0, int current = 0) : base (chance, maxrolls, current) {
			Type = type;
			// Data = data;
			Min = data[0];
			Max = data[1];

			if (data.Length > 2) { AffType = data[2]; }
		}

		public TGearEffect (TGearEffect copy) : this (copy.Type, new int[] { copy.Min, copy.Max, copy.AffType }, copy.Chance, copy.MaxRolls, copy.Current) { }

		public static GearEffect GenerateEffect (TGearEffect template, Character owner) {
			int amt = Global.Rand.Next(template.Min, template.Max + 1);

			switch (template.Type) {
				// data =  min, max, affType
				case 1: return new GAffinity(owner, amt, template.AffType);
				case 2: return new GResistance(owner, amt, template.AffType);
				// data = min, max
				case 3: return new GMana(owner, amt);
				case 4: return new GHealth(owner, amt);
				case 5: return new GDeckSize(owner, amt);
				case 6: return new GTrunkSize(owner, amt);
				case 7: return new GReactionPerc(owner, amt);
			}

			return null;
		}

		public static GearEffect GenerateEffect(Character owner, int type, int amt, int afftype = 0) {
			switch (type) {
				// data = affType, min, max
				case 1: return new GAffinity(owner, amt, afftype);
				case 2: return new GResistance(owner, amt, afftype);
				// data = min, max
				case 3: return new GMana(owner, amt);
				case 4: return new GHealth(owner, amt);
				case 5: return new GDeckSize(owner, amt);
				case 6: return new GTrunkSize(owner, amt);
				case 7: return new GReactionPerc(owner, amt);
			}

			return null;
		}

		public static bool Compare (TGearEffect template, GearEffect effect) {
			if (template.Type != effect.Type) { return false; }

			switch (template.Type) {
				// data = affType, min, max
				case 1: return template.AffType == (effect as GAffinity).AffType;
				case 2: return template.AffType == (effect as GResistance).AffType;
				// data = min, max
				case 3: return true;
				case 4: return true;
				case 5: return true;
				case 6: return true;
				case 7: return true;
			}

			return false;
		}

		public static void AddToEffect (Character Owner, TGearEffect template, GearEffect effect) {
			effect.AddEffect(Owner, Global.Rand.Next(template.Min, template.Max + 1));
		}

		public string ToFileLine () {
			BaseType typename = BaseTypes.Search(AffType);
			string start = Type.ToString() + " " + Min + " " + Max + (typename != null ? " " + typename.Name.ToLower() : "");
			string end = Chance.ToString() + " " + MaxRolls + (Current > 0 ? " " + Current : "");

			return start + ":" + end;
		}

		public override string ToString() {
			string build = "";
			string amount = (Min != Max ? Min + "-" + Max : Min.ToString());

			switch (Type) {
				// data = min, max, affType
				case 1: build += "+" + amount + "% ¹" + BaseTypes.Search(AffType).Name + " Affinity⁰"; break;// affinity
				case 2: build += "+" + amount + "% ²" + BaseTypes.Search(AffType).Name + " Resistance⁰"; break;// resistance
				case 3: build += "+" + amount + " ⁵Mana⁰"; break;// mana
				case 4: build += "+" + amount + " ³Max Health⁰"; break;// health
				case 5: build += "+" + amount + " ²Deck Size⁰"; break;// decksize
				case 6: build += "+" + amount + " ²Trunk Size⁰"; break;// trunksize
				case 7: build += "+" + amount + "% ⁶Reaction Affinity⁰"; break; // reaction perc
			}

			build += "\n⁴Chance " + Chance + " : Max " + MaxRolls + "⁰";

			return build;
		}
	}

	public abstract class GearEffect {
		public int Type, Amount = 0;

		public abstract void AddEffect(Character Owner, int amount);
		public abstract void StartOfBattle(Character Owner, List<BattleChar> targets);
		public abstract void StartOfTurn(Character Owner, List<BattleChar> targets);
		public virtual string ToFileLine () {
			return Type + " " + Amount;
		}

	}

	public class GAffinity : GearEffect {
		public int AffType;

		public GAffinity (Character owner, int amount, int afftype) {
			Type = 1;
			AffType = afftype;
			AddEffect(owner, amount);
		}
		
		public override void AddEffect(Character Owner, int amount) {
			Owner.ChangeAffinity(AffType, amount);
			Amount += amount;
		}

		public override void StartOfBattle(Character Owner, List<BattleChar> targets) {}
		public override void StartOfTurn(Character Owner, List<BattleChar> targets) {}
		public override string ToString() {
			return "+" + Amount + "% ¹" + BaseTypes.Search(AffType).Name + " Affinity⁰";
		}

		public override string ToFileLine() {
			return base.ToFileLine() + " " + BaseTypes.Search(AffType).Name;
		}
	}

	public class GResistance : GearEffect {
		public int AffType;

		public GResistance(Character owner, int amount, int afftype) {
			Type = 2;
			AffType = afftype;
			AddEffect(owner, amount);
		}

		public override void AddEffect(Character Owner, int amount) {
			Owner.ChangeResistance(AffType, amount);
			Amount += amount;
		}
		public override void StartOfBattle(Character Owner, List<BattleChar> targets) { }
		public override void StartOfTurn(Character Owner, List<BattleChar> targets) { }
		public override string ToString() {
			return "+" + Amount + "% ²" + BaseTypes.Search(AffType).Name + " Resistance⁰";
		}
		public override string ToFileLine() {
			return base.ToFileLine() + " " + BaseTypes.Search(AffType).Name;
		}
	}

	public class GMana : GearEffect {
		public GMana(Character owner, int amount) {
			Type = 3;
			AddEffect(owner, amount);
		}

		public override void AddEffect(Character Owner, int amount) {
			Owner.MaxMana += amount;
			Amount += amount;
		}
		public override void StartOfBattle(Character Owner, List<BattleChar> targets) { }
		public override void StartOfTurn(Character Owner, List<BattleChar> targets) { }
		public override string ToString() {
			return "+" + Amount + " ⁵Mana⁰";
		}
	}

	public class GHealth : GearEffect {
		public GHealth(Character owner, int amount) {
			Type = 4;
			AddEffect(owner, amount);
		}

		public override void AddEffect(Character Owner, int amount) {
			Owner.MaxHealth += amount;
			Owner.Health += amount;
			Amount += amount;
		}
		public override void StartOfBattle(Character Owner, List<BattleChar> targets) { }
		public override void StartOfTurn(Character Owner, List<BattleChar> targets) { }
		public override string ToString() {
			return "+" + Amount + " ³Max Health⁰";
		}
	}

	public class GDeckSize : GearEffect {
		public GDeckSize(Character owner, int amount) {
			Type = 5;
			AddEffect(owner, amount);
		}

		public override void AddEffect(Character Owner, int amount) {
			Owner.Cards.DeckLim += amount;
			Amount += amount;
		}
		public override void StartOfBattle(Character Owner, List<BattleChar> targets) { }
		public override void StartOfTurn(Character Owner, List<BattleChar> targets) { }
		public override string ToString() {
			return "+" + Amount + " ²Deck Size⁰";
		}
	}

	public class GTrunkSize : GearEffect {
		public GTrunkSize(Character owner, int amount) {
			Type = 6;
			AddEffect(owner, amount);
		}

		public override void AddEffect(Character Owner, int amount) {
			Owner.Cards.TrunkLim += amount;
			Amount += amount;
		}
		public override void StartOfBattle(Character Owner, List<BattleChar> targets) { }
		public override void StartOfTurn(Character Owner, List<BattleChar> targets) { }
		public override string ToString() {
			return "+" + Amount + " ²Trunk Size⁰";
		}
	}

	public class GReactionPerc : GearEffect {
		public GReactionPerc(Character owner, int amount) {
			Type = 7;
			AddEffect(owner, amount);
		}

		public override void AddEffect(Character Owner, int amount) {
			Owner.ReactAdd += amount / 100.0;
			Amount += amount;
		}
		public override void StartOfBattle(Character Owner, List<BattleChar> targets) { }
		public override void StartOfTurn(Character Owner, List<BattleChar> targets) { }
		public override string ToString() {
			return "+" + Amount + "% ⁶Reaction Affinity⁰";
		}
	}
}
