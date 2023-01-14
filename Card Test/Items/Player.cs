using Card_Test.Tables;
using System;
using System.Collections.Generic;
using System.Text;

namespace Card_Test.Items {
	public class Player : Character {
		public int Material;
		public List<Stone> Stones = new List<Stone>();
		public List<Gear> Gear = new List<Gear>();
		public string Token;

		public Player(string name, int maxhealth, int maxmana, TDeck deck = null, int startMaterial = 0, string token = "⁰Ω⁰") : base(name, maxhealth, maxmana, deck) {
			Material = startMaterial;
			Token = token;
			PlanVisible = true;
		}

		public void ViewCharacter () {
			Mana = MaxMana;
			string build = Name + " " + HealthToString() + "\n " + ManaToString() + "\n" + " Material : " + Material + "\n";

			List<string> collect = new List<string>();

			string sub = "";
			for (int i = 0; i < Affinity.Count; i++) {
				if (Affinity[i] != 100) {
					int aff = Affinity[i] - 100;
					sub += (aff >= 0 ? "+" : "") + string.Format("{0}% {1}\n", aff, BaseTypes.Search(i).Name);
				}
			}

			if (sub.Equals("")) { sub = "None"; }
			collect.Add(TextUI.GenerateHeading("Affinities", TextUI.LongestLine(sub)) + sub);

			sub = "";
			for (int i = 0; i < Resistances.Count; i++) {
				if (Resistances[i] != 0) {
					int res = Resistances[i];
					sub += (res >= 0 ? "+" : "") + string.Format("{0}% {1}\n", res, BaseTypes.Search(i).Name);
				}
			}

			if (sub.Equals("")) { sub = "None"; }
			collect.Add(TextUI.GenerateHeading("Resistances", TextUI.LongestLine(sub)) + sub);

			List<string> gear = new List<string>();

			for (int i = 0; i < Gear.Count; i++) {
				gear.Add(Gear[i].ToString());
			}

			Console.Clear();
			TextUI.PrintFormatted(build);
			TextUI.PrintFormatted(string.Join('\n', TextUI.MakeTable(collect, 3)));
			TextUI.PrintFormatted(string.Join('\n', TextUI.MakeTable(gear, 1)) + "\n");
			TextUI.Wait();
		}

	}
}
