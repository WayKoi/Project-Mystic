using Card_Test.Utilities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Card_Test.Base {
	public abstract class Plannable {
		public int ManaCost = 0, TargetType = 0;
		public bool Instant = false, Passive = false, Targeting = true;

		public abstract void UpdateValues(Character Caster);
		public abstract bool Additional(Character Caster, PlayReport report);
		public abstract bool Play(Character Caster, List<BattleChar> Targets, int Specific, PlayReport report = null);
		public abstract void Cancel(Character Caster);
		public abstract void Plan(Character Caster);
		public virtual bool RemoveFromPlan() { return true; }
	}

	public class PlanStep {
		public Plannable Planned;

		public Character Caster;
		public List<BattleChar> Targets;
		public int Specific;

		public bool Removable = true;

		public PlanStep (Plannable plan, Character caster, List<BattleChar> targs, int specific = -1) {
			Planned = plan;
			Caster = caster;
			Targets = targs;
			Specific = specific;
		}

		private bool TestPlan (PlayReport report) {
			Planned.UpdateValues(Caster);

			if (Planned.Passive) { 
				if (report != null) { report.Additional.Add("Passive effects cannot be planned"); } 
				return false; 
			}

			if (Caster.Mana < Planned.ManaCost) {
				if (report != null) { report.Additional.Add("Not enough mana to add to plan"); } 
				return false; 
			}

			if (Planned.Targeting) {
				BattleChar BattleCaster = null;
				int BattleCasterInd = 0;

				for (int i = 0; i < Targets.Count; i++) {
					if (Targets[i].Unit == Caster) {
						BattleCaster = Targets[i];
						BattleCasterInd = i;
						break;
					}
				}

				if (BattleCaster == null) {
					if (report != null) { report.Additional.Add("³Caster not found"); }
					return false;
				}

				if (Specific == -1 || (Specific < 0 || Specific >= Targets.Count)) { Specific = BattleCasterInd; } // make sure there is a target
				if (!Targets[Specific].Unit.HasHealth()) {
					if (report != null) { report.Additional.Add("Target has no health left"); } 
					return false; 
				}

				if (Planned.TargetType == 1) {
					if (Targets[Specific].Side != BattleCaster.Side) {
						if (report != null) { report.Additional.Add("Target has to be friendly"); }
						return false;
					}
				} // friends only

				if (Planned.TargetType == 2) {
					if (Targets[Specific].Side == BattleCaster.Side) {
						if (report != null) { report.Additional.Add("Target has to be an enemy"); }
						return false;
					}
				} // Enemies only
			} else {
				Specific = -1;
			}

			return Planned.Additional(Caster, report);
		}

		public bool Plan (PlayReport report) {
			bool check = TestPlan(report);
			if (!check) { return false; }

			Caster.Mana -= Planned.ManaCost;
			Planned.Plan(Caster);

			return true;
		}

		public void PlayStep (PlayReport report) {
			// PlayReport report = new PlayReport(Caster, Planned);
			report.Caster = Caster;
			report.Played = Planned;

			bool check = Planned.Play(Caster, Targets, Specific, report);
			if (!check) { Cancel(); }
		}

		public void Cancel () {
			Caster.Mana += Planned.ManaCost;
			Planned.Cancel(Caster);
		}

		public override string ToString() {
			return Planned.ToString();
		}
	}

	public class Plan {
		private List<PlanStep> Steps = new List<PlanStep>();
		private Character Owner;

		public Plan (Character owner) {
			Owner = owner;
		}

		public bool PlanOrCast (PlayReport report, Plannable toplan, List<BattleChar> targets, int specific = -1) {
			PlanStep step = new PlanStep(toplan, Owner, targets, specific);
			
			bool check = step.Plan(report);
			if (!check) { return false; }

			if (toplan.Instant) { step.PlayStep(report); return true; }

			Steps.Add(step);
			return true;
		}

		public bool Cast(PlayReport report, Plannable toplan, List<BattleChar> targets, int specific = -1) {
			PlanStep step = new PlanStep(toplan, Owner, targets, specific);
			step.PlayStep(report);
			return true;
		}

		public bool RemoveFromPlan (int ind) {
			if (ind < 0 || ind >= Steps.Count) { return false; }
			
			if (Steps[ind].Removable) {
				Steps[ind].Cancel();
				Steps.RemoveAt(ind);
				return true;
			}

			return false;
		}

		public void ClearPlan () {
			while (Steps.Count > 0) {
				RemoveFromPlan(0);
			}
		}

		public void ResetPlan() {
			Steps = new List<PlanStep>();
		}

		public string PlanOnTarget (int Target) {
			List<string> parts = new List<string>();

			for (int i = 0; i < Steps.Count; i++) {
				if (Steps[i].Specific == Target) {
					string step = Steps[i].ToString();
					parts.Add(new string(' ', step.Split('\n')[0].Length / 2) + (i + 1).ToString() + "\n" + step);
				}
			}

			return string.Join('\n', TextUI.MakeTable(parts, 0));
		}

		public void ExecutePlan () {
			List<PlanStep> steps = new List<PlanStep>();
			while (Steps.Count > 0) {
				PlayReport report = new PlayReport();
				
				PlanStep step = Steps[0];
				Steps.RemoveAt(0);

				if (!step.Planned.RemoveFromPlan()) {
					steps.Add(step);
					step.Removable = false;
				}

				step.PlayStep(report);
				report.PrintReport();
			}

			Steps = steps;
		}

		public int PlanSize () {
			return Steps.Count;
		}
	}
}
