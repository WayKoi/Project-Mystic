using Card_Test.Tables;
using System;
using System.Collections.Generic;
using System.Text;
using Card_Test.Base;

namespace Card_Test.Utilities {
	public class PlayReport {
		public Character Caster;
		public Plannable Played;
		public List<BattleChar> Affected = new List<BattleChar>();
		// damage, healing, shields broken, shields added, reaction, effect, damage blocked
		public List<int[]> AffectedEffects = new List<int[]>();
		public List<string> Additional = new List<string>();

		public PlayReport(Character caster = null, Plannable played = null) {
			Played = played;
			Caster = caster;
		}

		public void PrintReport() {
			TextUI.PrintFormatted(CreateReport());
		}

		private string CreateReport() {
			List<string> printout = new List<string>();

			if (Caster != null && Played != null) {
				printout.Add(Caster.Name + " Plays\n" + Played);
			}

			List<string> build = new List<string>();
			build.Add(" ");

			for (int i = 0; i < Affected.Count; i++) {
				// Char -> (Wet) : reaction : 2 Shields blocked amt : amt Damage : amt Healed : Gets # shields
				if (AffectedEffects.Count > i) {
					string subBuild = Affected[i].Unit.Name;

					if (AffectedEffects[i][5] != 0) {
						subBuild += " -> (" + Effects.Table[AffectedEffects[i][5]].Name + ")";
					}

					if (AffectedEffects[i][4] != -1) {
						subBuild += " : ²Triggers " + Reactions.Table[AffectedEffects[i][4]].Name + "⁰";
					}

					if (AffectedEffects[i][2] != 0) {
						subBuild += " : ²Breaks " + AffectedEffects[i][2] + " Shield" + (AffectedEffects[i][2] > 1 ? "s" : "") + " Blocking " + AffectedEffects[i][6] + "⁰";
					}

					if (AffectedEffects[i][0] != 0) {
						subBuild += " : Takes ³" + AffectedEffects[i][0] + "⁰ Damage ⁴" + Affected[i].Unit.HealthToString() + "⁰";
					}

					if (AffectedEffects[i][1] != 0) {
						subBuild += " : Heals ¹" + AffectedEffects[i][1] + "⁰";
					}

					if (AffectedEffects[i][3] != 0) {
						subBuild += " : Gets ²" + AffectedEffects[i][3] + " Shield" + (AffectedEffects[i][3] > 1 ? "s⁰" : "⁰");
					}

					if (!Effects.Table[AffectedEffects[i][5]].Stays) {
						subBuild += " : Loses (" + Effects.Table[AffectedEffects[i][5]].Name + ")";
					}

					if (!Affected[i].Unit.HasHealth()) {
						subBuild += " : ³Perishes⁰";
					}

					build.Add(subBuild);
				}
			}

			for (int i = 0; i < Additional.Count; i++) {
				build.Add("²" + Additional[i] + "⁰");
			}

			printout.Add(String.Join('\n', build));

			return String.Join('\n', TextUI.MakeTable(printout));
		}
	}
}
