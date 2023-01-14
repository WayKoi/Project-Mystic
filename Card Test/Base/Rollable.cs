using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Card_Test.Base {
    public class Rollable {
        // how this is going to go down is Chance is a value that is collected on all rollable objects
        // then the maxRolls is the max number of times that when rolled this rollable object can be selected
        // This makes it so that a certain card in a pack cant be rolled multiple times
        // Current is just memory for how many times that this option has been selected
        private int _chance;
        public int Chance { set { _chance = Math.Max(0, value); } get { return _chance; } }
        // max rolls = 0 means that there is no limit
        public int MaxRolls { set; get; }
        public int Current { private set; get; }

        public Rollable(int chance, int maxrolls = 0, int current = 0) {
            Chance = chance;
            MaxRolls = maxrolls;
            Current = current;
        }

        public void ResetCount() {
            Current = 0;
        }

        public void Chosen() {
            Current++;
        }

        public bool Roll(int max = 100) {
            if (MaxRolls != 0 && (Current >= MaxRolls)) { return false; }

            max = Math.Max(max, 0);
            int check = Global.Rand.Next(0, max);
            bool result = check < Chance;

            if (result) { Chosen(); }
            return result;
        }

        public static List<Rollable> Roll(Rollable[] choices, int amt) {
            return Roll(choices.ToList(), amt);
        }

        public static List<Rollable> Roll(List<Rollable> choices, int amt) {
            List<Rollable> Valid = new List<Rollable>();

            int chanceTotal = 0;
            int choiceCount = choices.Count;
            for (int i = 0; i < choiceCount; i++) {
                if (choices[i].MaxRolls <= 0 || (choices[i].Current < choices[i].MaxRolls)) {
                    Valid.Add(choices[i]);
                    chanceTotal += choices[i].Chance;
                }
            }

            List<Rollable> Pulls = new List<Rollable>();
            choiceCount = Valid.Count;
            while (amt > 0 && Valid.Count > 0) {
                int chosen = Global.Rand.Next(0, chanceTotal);
                
                for (int i = 0; i < choiceCount; i++) {
                    chosen -= Valid[i].Chance;
                    if (chosen < 0) {
                        Valid[i].Chosen();
                        Pulls.Add(Valid[i]);

                        if (choices[i].MaxRolls != 0 && Valid[i].Current >= Valid[i].MaxRolls) {
                            chanceTotal -= Valid[i].Chance;
                            Valid.RemoveAt(i);
                        }
                        
                        break;
                    }
                }

                amt--;
            }

            return Pulls;
        }
    }
}
