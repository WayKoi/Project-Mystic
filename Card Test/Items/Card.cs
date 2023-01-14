using Card_Test.Base;
using Card_Test.Tables;
using Card_Test.Utilities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Card_Test {
	public class Card : Plannable {
		// ┌ ┐ └ ┘ │ ─ ╒ ═ ╕ ╤ ┤ ├ ┴ ╘ ╧ ╛
		private static string[,] Frames = {
			{ "①┌───┐②", "③│④", "③│④", "①└───┘②" },
			{ "①╒───╕②", "③│④", "③│④", "①╘───╛②" },
			{ "①╒─═─╕②", "③│④", "③│④", "①╘─═─╛②" },
			{ "①╒═══╕②", "③│④", "③│④", "①╘═══╛②" },
			{ "①╔═══╗②", "③║④", "③║④", "①╚═══╝②" }
		};

		public CardType Element;

		public int Type { private set; get; } 
		public int Tier { private set; get; }
		public int Mod { private set; get; }
		public int Sub { private set; get; }
		public int Made { private set; get; } // this is for keeping track of where the card came from
		public bool Valid { private set; get; }

		public int Damage { private set; get; }
		public int Healing { private set; get; }

		public Card (int type, int tier, int mod = 0, int sub = 0, int made = 0) {
			Type = type;
			Tier = tier;
			Mod = mod;
			Sub = sub;
			Made = made;

			Valid = true;
			Validate();

			UpdateValues(null);
		}

		private void CalcValues() {
			if (!Valid) { return; }
			
			CardMod mod = Mods.Search(Mod);
			if (mod == Mods.Search("summon")) {
				Damage = 0;
				Healing = 0;
			} else {
				CardType type = LookupType();

				Damage = type.Damage[Tier - type.TierLim[0]];
				Healing = type.Healing[Tier - type.TierLim[0]];
			}

			TargetType = Element.TargetType;
		}

		public Card(Card Copy) : this (Copy.Type, Copy.Tier, Copy.Mod, Copy.Sub, Copy.Made) { }

		public void Validate() {
			Element = LookupType();
			if (Element == null) { Valid = false; return; }
			if (Mod >= Mods.TableLength()) { Valid = false; return; }
			if (Sub >= SubMods.TableLength()) { Valid = false; return; }

			if (Element.ResMod != null) {
				for (int i = 0; i < Element.ResMod.Length; i++) {
					if (Mod == Element.ResMod[i]) {
						Mod = 0;
					}
				}
			}

			if (Element.ResSub != null) {
				for (int i = 0; i < Element.ResSub.Length; i++) {
					if (Sub == Element.ResSub[i]) {
						Sub = 0;
					}
				}
			}

			Tier = Math.Min(Tier, Element.TierLim[1]);
			Tier = Math.Max(Tier, Element.TierLim[0]);

			if (!Element.HasStatus && Sub == SubMods.Translate("status")) {
				Sub = 0;
			}

			if (Element.Rules != null) {
				Element.Rules(this);
			}
		}

		public void ChangeMod(int to) {
			if (to < 0 || to >= Mods.TableLength()) { return; }
			Mod = to;
			CalcValues();
		}

		public void ChangeSub(int to) {
			if (to < 0 || to >= SubMods.TableLength()) { return; }
			Sub = to;
			CalcValues();
		}

		public void ChangeTier(int to) {
			if (!Valid) { return; }
			Tier = to;
			Validate();
			CalcValues();
		}

		public void ChangeType(int to) {
			int tem = Type;
			Type = to;
			CardType check = LookupType();
			if (check == null) { Valid = false; } else { Validate(); }
			if (!Valid) { Type = tem; }
			CalcValues();
		}

		public void ChangeMade(int to) {
			if (!Valid) { return; }
			Made = to;
			Validate();
		}

		public CardType LookupType () {
			return Types.Search(Type);
		}

		// Overrides
		public override bool Additional(PlayReport report) { return true; }
		public override void Cancel(Character Caster) { Caster.Hand.Add(this); }
		public override void Plan(Character Caster) { Caster.Hand.Remove(this); }
		public override void UpdateValues(Character caster) {
			CalcValues();

			ManaCost = Tier;
			TargetType = Element.TargetType;
			Instant = Element.Instant;

			Targeting = (Mod != Mods.Translate("summon"));
		}

		public override bool Play(Character Caster, List<BattleChar> Targets, int Specific, PlayReport report = null) {
			if (report == null) { report = new PlayReport(); }

			report.Caster = Caster;
			report.Played = this;

			if (Targeting && (Specific < 0 || Specific >= Targets.Count)) { report.Additional.Add("No Target, Added back to the hand"); return false; }
			if (Targeting && !Targets[Specific].Unit.HasHealth()) { report.Additional.Add("Target has no health, Added back to the hand"); return false; }

			if (Targeting) {
				int DamageAmt = (int) (Damage * Caster.GetAffinity(Element));
				int HealingAmt = (int) (Healing * Caster.GetAffinity(Element));
				Targets[Specific].TakeDamage(Caster, DamageAmt, Type, report);
				Targets[Specific].Heal(HealingAmt, Type, report);
			}

			// Additional stuff
			Element.CastAdditional(this, Targets, Specific, report);
			Mods.Search(Mod).RunAction(this, Caster, Targets, Specific, new int[] { Damage, Healing }, report);
			SubMods.Search(Sub).RunAction(this, Caster, Targets, Specific, new int[] { Damage, Healing }, report);

			return true;
		}

		// String returning methods
		public override string ToString() {
			CardType type = Types.Search(Type);

			int frame = Types.Search(Type).Tier;

			string build = Frames[frame, 0] + "\n" + Frames[frame, 1] + "  " + SubMods.Search(Sub).Symbol + Frames[frame, 2] + "\n" + Frames[frame, 1];
			build += "!!!" + Frames[frame, 2] + "\n" + Frames[frame, 1];
			build += Tier.ToString() + " " + Mods.Search(Mod).Symbol + Frames[frame, 2] + "\n" + Frames[frame, 3];
			
			string col = Healing > 0 ? "¹" : "";
			string nor = Healing > 0 ? "⁰" : "";
			string sec = Instant ? "²" : col;
			string secnor = Instant ? "⁰" : nor;

			build = build.Replace("①", col).Replace("②", nor).Replace("③", sec).Replace("④", secnor);
			build = build.Replace("!!!", type.Short);

			return build;
		}

		public string Details() {
			List<string> print = new List<string>();

			print.Add(this.ToString());

			string build = "Type : " + LookupType().Name + "\n";
			build += "Mod : " + Mods.Search(Mod).Name + "\n";
			build += "Sub Mod : " + SubMods.Search(Sub).Name + "\n";
			build += "Effect : " + Effects.Table[LookupType().GetEffect()].Name;
			print.Add(build);

			return String.Join('\n', TextUI.MakeTable(print));
		}

		public string ToFileLine () {
			string build = Type + " " + Tier;

			if (Made != 0 || Sub != 0 || Mod != 0) {
				build += " " + Mod;
			}

			if (Made != 0 || Sub != 0) {
				build += " " + Sub;
			}

			if (Made != 0) {
				build += " " + Made;
			}

			return build;
		}
	}

	
}
