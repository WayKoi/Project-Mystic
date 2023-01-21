using Card_Test.Tables;
using System;
using System.Collections.Generic;
using System.Text;
using Card_Test.Utilities;

namespace Card_Test.Tables {
	public static class StatusTable {
		public static TStatus[] Table = {
			new TStatus("²Sink⁰", "sinks in the sand", "sand", true, PreventPlanning, BaseEndConnector),
			new TStatus("⁵Frozen⁰", "is frozen in a block of ice", "ice", false, PreventPlanning, EndOfTurnDamage),
			new TStatus("³Ignited⁰", "is currently on fire", "fire", false, BaseStartConnector, EndOfTurnDamage),
			new TStatus("₀Soaked⁰", "is covered in water", "water", true, BaseStartConnector, BaseEndConnector),
			new TStatus("¹Rooted⁰", "is stuck in the roots", "life", true, PreventPlanning, BaseEndConnector),
			new TStatus("⁶Vision⁰", "has their plan revealed", "none", false, VisionStart, EndOfVision),
			new TStatus("⁸Vortex⁰", "is trapped in a vortex", "wind", false, BaseStartConnector, EndOfTurnDamage)
		};

		private static void EndOfVision (BattleChar affected, Status stat, PlayReport report) {
			BaseEnd(affected, stat, null, false);
			if (report == null) { report = new PlayReport(); }

			if (stat.TurnsLeft == 0) {
				report.Additional.Add("Vision on " + affected.Unit.Name + " is lost");
				affected.Unit.PlanVisible = false;
			}
		}

		private static void EndOfTurnDamage (BattleChar affected, Status stat, PlayReport report) {
			if (report == null) { report = new PlayReport(null, null); }

			if (stat.Cast != null) {
				int damage = (int) (stat.Cast.LookupType().GetDamage(stat.Tier) * stat.Multiplier);
				affected.TakeDamage(null, damage, stat.Cast.Type, report, false);
			}

			BaseEnd(affected, stat, report, false);
		}

		private static void PreventPlanning (BattleChar affected, Status stat, PlayReport report) {
			affected.Unit.Plan.ClearPlan();
			BaseStart(affected, stat, report, false);
		}

		private static void VisionStart (BattleChar affected, Status stat, PlayReport report) {
			affected.Unit.PlanVisible = true;
			BaseStart(affected, stat, report, false);
		}

		// BASE EFFECTS
		private static void BaseStartConnector(BattleChar affected, Status stat, PlayReport report) {
			BaseStart(affected, stat, report, report == null);
		}

		private static void BaseStart (BattleChar affected, Status stat, PlayReport report = null, bool print = true) {
			if (report == null) {
				report = new PlayReport();
			}

			if (stat.Link.Effect) {
				affected.Effect = BaseTypes.Search(stat.Link.Type).Effect;

				/*report.Affected.Add(affected);
				// damage, healing, shields broken, shields added, reaction, effect, damage blocked
				report.AffectedEffects.Add(new int[] { 0, 0, 0, 0, -1, affected.Effect, 0 });*/
				report.Steps.Add(new ReportStep(affected, 0, 0, 0, 0, -1, affected.Effect));

			}

			report.Additional.Add(affected.Unit.Name + " " + stat.Link.Phrase);

			if (print) {
				report.PrintReport();
			}
		}

		private static void BaseEndConnector (BattleChar affected, Status stat, PlayReport report) {
			BaseEnd(affected, stat, report, report == null);
		}

		private static void BaseEnd (BattleChar affected, Status stat, PlayReport report = null, bool print = true) {
			if (report == null) {
				report = new PlayReport(null, null);
			}

			if (stat.TurnsLeft > 0) {
				stat.TurnsLeft--;

				if (stat.TurnsLeft == 0) {
					// affected.Statuses.Remove(stat);
					report.Additional.Add(affected.Unit.Name + " recovers from " + stat.Link.Name);
				}
			}

			if (print) {
				report.PrintReport();
			}
		}
	}

	public class Status {
		public Card Cast;
		public TStatus Link;
		public BattleChar Affected;
		public int TurnsLeft, Tier;
		public double Multiplier;

		public Status (Card cast, TStatus link, BattleChar affected, int turns, int tier, double mult = 0.5, PlayReport report = null) {
			Cast = cast;
			Link = link;
			Affected = affected;
			TurnsLeft = turns;
			Tier = tier;
			Multiplier = mult;

			if (!Affected.HasStatus(Link)) {
				Affected.Statuses.Add(this);
				Start(report);
			}
		}

		public void Start (PlayReport report = null) {
			if (Link.Start != null) {
				Link.Start(Affected, this, report);
			}
		}

		public void End(PlayReport report = null) {
			if (Link.End != null) {
				Link.End(Affected, this, report);
			}
		}
	}

	public class TStatus {
		public string Name, Phrase;
		public string Type;
		public bool Effect;
					// effected,  effect
		public Action<BattleChar, Status, PlayReport> End, Start;

		public TStatus (string name, string phrase, string type, bool effect, Action<BattleChar, Status, PlayReport> start = null, Action<BattleChar, Status, PlayReport> end = null) {
			Name = name;
			Phrase = phrase;
			End = end;
			Start = start;
			Type = type;
			Effect = effect;
		}
	}
}
