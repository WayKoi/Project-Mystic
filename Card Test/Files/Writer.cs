using Card_Test.Items;
using Card_Test.Tables;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

namespace Card_Test.Files {
	public static class Writer {
		private static string Dir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

		public static void WritePack(Pack content, string name) {
			WriteFile("Pack", name + ".pck", content.ToFile());
		}

		public static void WriteDeck (Deck content, string name) {
			List<string> lines = new List<string>();

			lines.Add(content.StartHandSize + " " + content.DeckLim + " " + content.TrunkLim);

			lines.AddRange(content.ToFileLines());

			WriteFile("Deck", name + ".dck", lines);
		}

		public static void WriteGear (Gear content, string name) {
			List<string> lines = content.ToFileLines();
			WriteFile("Gear", name + ".gear", lines);
		}

		public static void WriteGear(TGear content, string name) {
			WriteGear(TGear.Generate(content, null), name);
		}

		// Private methods
		private static void WriteFile(string subdir, string name, List<string> contents) {
			string path = Dir + "\\" + subdir + "\\" + name;
			path = path.Replace('/', '\\');
			string pathnoFile = path.Replace("\\" + path.Split('\\')[path.Split('\\').Length - 1], "");

			if (!Directory.Exists(pathnoFile)) { Directory.CreateDirectory(pathnoFile); }
			if (!File.Exists(path)) { var tem = File.Create(path); tem.Close(); }
			File.WriteAllLines(path, contents);
		}

		private static void WriteError(string fail) {
			TextUI.PrintFormatted("³Could not write to " + fail + "⁰");
			TextUI.Wait();
		}

	}
}
