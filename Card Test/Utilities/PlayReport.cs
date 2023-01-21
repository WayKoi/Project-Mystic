using Card_Test.Tables;
using System;
using System.Collections.Generic;
using System.Text;
using Card_Test.Base;

namespace Card_Test.Utilities {
	public class PlayReport {
		public Character Caster;
		public Plannable Played;
		// damage, healing, shields broken, shields added, reaction, effect, damage blocked
		public List<ReportStep> Steps = new List<ReportStep>();
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

			for (int i = 0; i < Steps.Count; i++) {
				build.Add(Steps[i].ToString());
			}

			for (int i = 0; i < Additional.Count; i++) {
				build.Add("²" + Additional[i] + "⁰");
			}

			printout.Add(String.Join('\n', build));

			return String.Join('\n', TextUI.MakeTable(printout));
		}
	}

	public struct ReportStep {
		public int Damage, Healing, SBroken, SAdded, React, Effect, Blocked;
		public string HealthStamp;
		public BattleChar Affected;

		public ReportStep (BattleChar Aff, int damage = 0, int healing = 0, int sbroke = 0, int sadd = 0, int react = -1, int eff = 0, int blocked = 0) {
			Affected = Aff;
			HealthStamp = (Affected == null) ? "0/0" : Affected.Unit.HealthToString();

			Damage = damage;
			Healing = healing;
			SBroken = sbroke;
			SAdded = sadd;
			React = react;
			Effect = eff;
			Blocked = blocked;
		}

		public override string ToString() {
			// Character -> (Wet) : reaction : 2 Shields blocked amt : amt Damage : amt Healed : Gets # shields
			string subBuild = Affected.Unit.Name;

			if (Effect != 0) {
				subBuild += " -> (" + Effects.Table[Effect].Name + ")";
			}

			if (React != -1) {
				subBuild += " : ²Triggers " + Reactions.Table[React].Name + "⁰";
			}

			if (SBroken != 0) {
				subBuild += " : ²Breaks " + SBroken + " Shield" + (SBroken > 1 ? "s" : "") + " Blocking " + Blocked + "⁰";
			}

			if (Damage != 0) {
				subBuild += " : Takes ³" + Damage + "⁰ Damage ⁴" + HealthStamp + "⁰";
			}

			if (Healing != 0) {
				subBuild += " : Heals ¹" + Healing + " ⁴" + HealthStamp + "⁰";
			}

			if (SAdded != 0) {
				subBuild += " : Gets ²" + SAdded + " Shield" + (SAdded > 1 ? "s⁰" : "⁰");
			}

			if (!Effects.Table[Effect].Stays) {
				subBuild += " : Loses (" + Effects.Table[Effect].Name + ")";
			}

			if (!Affected.Unit.HasHealth()) {
				subBuild += " : ³Perishes⁰";
			}

			return subBuild;
		}
	}
}
