﻿using Card_Test.Base;
using Card_Test.Tables;
using Card_Test.Utilities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Card_Test.Items {
	public class FusionPlan : Plannable {
		private List<Card> Fuse = new List<Card>();
		private Card Result = null;

		public FusionPlan(List<Card> fuse) {
			for (int i = 0; i < fuse.Count; i++) {
				Fuse.Add(fuse[i]);
				if (fuse[i].ManaCost > ManaCost) {
					ManaCost = fuse[i].ManaCost;
				}
			}

			if (Fuse.Count <= 1) { return; }

			Card inter = Fuse[0];
			for (int i = 0; i < Fuse.Count - 1; i++) {
				inter = Fusions.Fuse(inter, Fuse[i + 1]);
			}

			Result = inter;

			TargetType = Result.TargetType;
			Instant = Result.Instant;
			Targeting = Result.Targeting;
		}

		public override bool Additional(Character Caster, PlayReport report) {
			if (Result == null || Fuse == null || Fuse.Count < 2) {
				if (report != null) { report.Additional.Add("Not enough cards to do a fusion"); }
				return false;
			}

			if (Caster.FusionCounters < Fuse.Count - 1) {
				if (report != null) { report.Additional.Add("Not enough fusion counters to do that fusion"); }
				return false;
			}

			return true;
		}

		public override void Cancel(Character Caster) {
			for (int i = 0; i < Fuse.Count; i++) {
				Caster.Hand.Add(Fuse[i]);
			}

			Caster.FusionCounters += Fuse.Count - 1;
		}

		public override void Plan(Character Caster) {
			for (int i = 0; i < Fuse.Count; i++) {
				Caster.Hand.Remove(Fuse[i]);
			}

			Caster.FusionCounters -= Fuse.Count - 1;
		}

		public override bool Play(Character Caster, List<BattleChar> Targets, int Specific, PlayReport report = null) {
			return Result.Play(Caster, Targets, Specific, report);
		}

		public override void UpdateValues(Character caster) { }

		public override string ToString() {
			List<string> cards = new List<string>();

			for (int i = 0; i < Fuse.Count; i++) {
				cards.Add(Fuse[i].ToString());
				if (i < Fuse.Count - 1) { cards.Add("\n\n+"); }
			}

			return string.Join('\n', TextUI.MakeTable(cards, 0));
		}
	}

	public class SidePlan : Plannable {
		private Card Side;
		private int Counter = 1;

		public SidePlan(Card side) {
			Side = side;
		}

        public override bool Additional(Character Caster, PlayReport report) {
			if (Caster.SideCastCounters <= 0) {
				if (report != null) { report.Additional.Add("Not enough sidecast counters to side cast"); }
				return false;
			}

			return true;
        }

        public override void Cancel(Character Caster) {
			Side.Cancel(Caster);
			if (Counter == 1) { Caster.SideCastCounters++; }
        }

        public override void Plan(Character Caster) {
			Side.Plan(Caster);
			Caster.SideCastCounters--;
        }

        public override bool Play(Character Caster, List<BattleChar> Targets, int Specific, PlayReport report = null) {
			if (Counter == 0) {
				return Side.Play(Caster, Targets, Specific, report);
			} else {
				Counter--;
			}

			return true;
        }

        public override void UpdateValues(Character Caster) {
			Side.UpdateValues(Caster);

			ManaCost = (int) Math.Ceiling(Side.ManaCost / 2.0);
			TargetType = Side.TargetType;

			Targeting = Side.Targeting;
		}

        public override bool RemoveFromPlan() {
			return Counter == 0;
		}

        public override string ToString() {
			string build = Side.ToString() + "\n";
			build += (Counter == 1 ? "₃" : "³") + "-----⁰";
			return build;
        }
    }
}
