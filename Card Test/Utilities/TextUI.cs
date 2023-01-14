using System;
using System.Collections.Generic;
using System.Text;

namespace Card_Test {
    public static class TextUI {

        public static string RetryMessage = "Please try again";
        public static string special = "⁰¹²³⁴⁵⁶⁷⁸⁹₀₁₂₃₄₅₆₇₈₉"; // ⁰¹²³⁴⁵⁶⁷⁸⁹₀₁₂₃₄₅₆₇₈₉
        public static ConsoleColor[] Cols = {
            ConsoleColor.Gray,
            ConsoleColor.Green, 
            ConsoleColor.Yellow, 
            ConsoleColor.Red,
            ConsoleColor.DarkGray, 
            ConsoleColor.Cyan, 
            ConsoleColor.DarkMagenta,
            ConsoleColor.DarkGreen,
            ConsoleColor.White,
            ConsoleColor.DarkCyan,
            ConsoleColor.Blue,
            ConsoleColor.DarkRed,
            ConsoleColor.DarkYellow,
            ConsoleColor.DarkBlue,
            ConsoleColor.Magenta
        };


        public static int Prompt(string prompt, int mini, int maxi, int[] Invalids = null) {
            int chosen = mini - 1;
            bool finished = false;

            while (!finished) {
                Console.Write(prompt + "\n> ");
                string input = Console.ReadLine();
                int.TryParse(input, out chosen);

                if (chosen >= mini && chosen <= maxi) {
                    finished = true;
                    if (Invalids != null) {
                        foreach (int invalid in Invalids) {
                            if (chosen == invalid) {
                                finished = false;
                                break;
                            }
                        }
                    }
                }

                if (!finished) {
                    TextUI.PrintFormatted(RetryMessage);
                }
            }

            return chosen;
        }

        public static string Prompt(string prompt, string[] Valid) {
            bool finished = false;
            string input = "";

            while (!finished) {
                Console.Write(prompt + "\n> ");
                input = Console.ReadLine();

                foreach (string valid in Valid) {
                    if (input.ToLower().Equals(valid.ToLower())) {
                        finished = true;
                        break;
                    }
                }

                if (!finished) {
                    TextUI.PrintFormatted(RetryMessage);
                }
            }

            return input.ToLower();
        }

        public static void Prompt(string prompt, MenuItem[] items) {
            bool finished = false;

            while (!finished) {
                Console.Write(prompt + "\n> ");
                string input = Console.ReadLine();
                string[] chop = input.Split(' ');
                string[] info = { "?" };
                bool askedinfo = false;

                foreach (string special in info) {
                    if (chop[0].ToLower().Equals(special.ToLower())) {
                        askedinfo = true;
                        break;
                    }
                }

                if (askedinfo) {
                    PrintInfo(items);
                } else {
                    bool founditem = false;
                    foreach (MenuItem item in items) {
                        foreach (string key in item.Keywords) {
                            if (key.ToLower().Equals(chop[0].ToLower())) {
                                founditem = true;
                                finished = item.Run(item.Parse(input));
                            }
                        }

                        if (founditem) {
                            break;
                        }
                    }

                    if (!finished) {
                        TextUI.PrintFormatted(RetryMessage);
                    }
                }
            }
        }

        private static void PrintInfo(MenuItem[] items) {
            TextUI.PrintFormatted("\nMenus are NOT case-sensitive");
            foreach (MenuItem item in items) {
                if (item.Description != null) {
                    TextUI.PrintFormatted(item.Description);
                }
            }
            Console.WriteLine();
        }

        public static int[] Parse(string input) {
            string[] chop = input.Split(' ');
            if (chop.Length == 0) { return null; }
            int[] result = new int[chop.Length - 1];
            
            for (int i = 1; i < chop.Length; i++) {
                int.TryParse(chop[i], out result[i - 1]);
            }

            return result;
        }

        public static int[] DummyParse (string input) {
            return null;
        }

        public static List<string> Combine (List<string> toCombine, int gap = 1) {
            List<string> combined = new List<string>();

            for (int i = 0; i < toCombine.Count; i++) {
                string[] chop = toCombine[i].Split('\n');
                for (int ii = 0; ii < chop.Length; ii++) {
                    if (combined.Count <= ii) {
                        combined.Add("");
                    }
                    combined[ii] += chop[ii] + new string(' ', gap);
                }
            }

            return combined;
        }

        public static List<string> MakeTable (string[] colHeadings, string[,] values, bool drawHeading = true, int gap = 1) {
            List<string> table = new List<string>();
            List<int> ColWids = new List<int>();

            for (int i = 0; i < colHeadings.Length; i++) {
				int wid = colHeadings[i].Length;
                for (int ii = 0; ii < values.GetLength(1); ii++) {
                    if (values[i, ii] != null && wid < GetLength(values[i, ii])) {
                        wid = GetLength(values[i, ii]);
                    }
                }
                ColWids.Add(wid);
            }

            for (int i = 0; i < colHeadings.Length; i++) {
                List<string> col = new List<string>();
                if (drawHeading) {
                    col.Add(String.Format("{0," + (-1 * ColWids[i]).ToString() + "}", colHeadings[i]));
                }

                for (int ii = 0; ii < values.GetLength(1); ii++) {
                    int diff = (values[i, ii] != null ? values[i, ii].Length - GetLength(values[i, ii]) : 0);
                    col.Add(String.Format("{0," + (-1 * (ColWids[i] + diff)).ToString() + "}", values[i, ii] == null ? "" : values[i, ii]));
                }

                table.Add(String.Join('\n', col));
            }

            return Combine(table, gap);
        }

        public static List<string> MakeTable (List<string> toBreak, int gap = 1) {
            if (toBreak.Count == 0) { return new List<string>(); }

            string[] headings = new string[toBreak.Count];

            for (int i = 0; i < headings.Length; i++) {
                headings[i] = "";
            }

            int height = 0;
            foreach (string test in toBreak) {
                int testHei = test.Split('\n').Length;
                if (testHei > height) {
                    height = testHei;
                }
            }

            string[,] values = new string[headings.Length, height];

            for (int i = 0; i < toBreak.Count; i++) {
                string[] chop = toBreak[i].Split('\n');
                for (int ii = 0; ii < chop.Length; ii++) {
                    values[i,ii] = chop[ii];
                }
            }

            return MakeTable(headings, values, false, gap);
        }

        public static void Wait () {
            TextUI.PrintFormatted("press anything to continue");
            Console.ReadLine();
        }

        public static int GetLength (string check) {
            int count = 0;
            for (int i = 0; i < check.Length; i++) {
                if (check[i] == '~') {
                    count++;
                    i++;
                } else {
                    int ind = special.IndexOf(check[i]);
                    if (ind == -1) {
                        count++;
                    }
                }
            }

            return count;
        }

        public static void PrintFormatted (string print) {
            for (int i = 0; i < print.Length; i++) {
                if (print[i] == '~' && i < print.Length - 1) {
                    Console.Write(print[i + 1]);
                    i++;
                } else {
                    if ((print[i] > 8300 && print[i] < 8331) || (print[i] > 177 && print[i] < 186)) {
                        int ind = special.IndexOf(print[i]);
                        if (ind != -1) {
                            Console.ForegroundColor = Cols[ind];
                        } else {
                            Console.Write(print[i]);
                        }
                    } else {
                        Console.Write(print[i]);
                    }
                }
            }

            Console.WriteLine();
        }

        public static int LongestLine (string check) {
            string[] chop = check.Split('\n');

            int longest = 0;
            for (int i = 0; i < chop.Length; i++) {
                if (chop[i].Length > longest) {
                    longest = chop[i].Length;
                }
            }

            return longest;
        }

        public static string GenerateHeading (string text, int length, int diff = 0) {
            string build = new string(' ', Math.Max(((length - text.Length) / 2) - diff, 0));
            build += text + "\n";
            build += new string('-', Math.Max(length - diff, text.Length)) + "\n";
            return build;
        }
    }

    public class MenuItem {
        public string[] Keywords;
        public Func<int[], bool> Run;
        public Func<string, int[]> Parse;
        public string Description = null;
        public MenuItem(string[] keys, Func<int[], bool> run, Func<string, int[]> parse, string description = null) {
            Keywords = keys;
            Run = run;
			Parse = parse;

            if (description != null) {
                Description = "Use ";

                for (int i = 0; i < keys.Length; i++) {
                    Description += "\"" + keys[i] + "\" " + (i == keys.Length - 1 ? "to " : (i == keys.Length - 2 ? "or " : ""));
                }

                Description += description;
            }
        }
    }
}
