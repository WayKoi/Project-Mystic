using Card_Test.Items;
using Card_Test.Tables;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Card_Test.Files {
	public static class Reader {
		private static string Dir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

		public static Pack ReadPack(string name, int type = -1) {
			List<string> lines = ReadFile("Pack", name + ".pck");
			bool readable = true;
			if (lines == null || lines.Count < 4) { ReadError("Pack \"" + name + "\""); return ReadPack("Default"); }

			string packname = lines[0];
			string symbol = string.Format("{0,-3}", lines[1]);
			string subsymbol = string.Format("{0,-1}", lines[2]);

			int[] def = { 0, 1, 0 };

			string[] args = lines[3].Split(' ');

			for (int i = 0; i < args.Length; i++) {
				readable = int.TryParse(args[i], out def[i]) && readable;
			}

			if (!readable) { ReadError("Pack \"" + name + "\""); return ReadPack("Default"); }

			lines.RemoveRange(0, 4); // remove all data lines and leave the cards

			List<PackE> cards = new List<PackE>();

			while (lines.Count > 0 && readable) {
				args = lines[0].Split(':');

				TCard tem = InterpretTCard(args[0], type);

				int[] subdef = { 0, 0 };
				string[] chop = args[1].Split(' ');

				for (int i = 0; i < chop.Length; i++) {
					readable = int.TryParse(chop[i], out subdef[i]) && readable;
				}

				lines.RemoveAt(0);

				readable = tem.Convert().Valid && readable;
				cards.Add(new PackE(tem, subdef[0], subdef[1]));
			}

			if (!readable) { ReadError("Pack \"" + name + "\""); return ReadPack("Default"); }

			return new Pack(packname, symbol, subsymbol, def[0], def[1], cards.ToArray(), def[2]);
		}

		public static Deck ReadDeck (string name, int type = -1) {
			return Decks.FillDeck(ReadTDeck(name, type));
		}

		public static TDeck ReadTDeck(string name, int type = -1) {
			List<string> cont = ReadFile("Deck", name + ".dck");
			bool readable = true;
			if (cont == null || cont.Count < 2) { ReadError("Deck \"" + name + "\""); return ReadTDeck("Default"); }

			string[] args = cont[0].Split(' ');
			cont.RemoveAt(0);

			int[] vals = { 3, -1, -1 };

			for (int i = 0; i < args.Length; i++) {
				readable = int.TryParse(args[i], out vals[i]) && readable;
			}

			if (!readable) { ReadError("Deck \"" + name + "\""); return ReadTDeck("Default"); }

			List<TCard> cards = new List<TCard>();
			List<TCard> trunk = new List<TCard>();

			while (cont.Count > 0 && readable) {
				// we are reading into the trunk
				if (cont[0][0] == '#') { cont.RemoveAt(0); break; }
				TCard tem = InterpretTCard(cont[0], type);

				cont.RemoveAt(0);

				readable = tem.Convert().Valid && readable;
				cards.Add(tem);
			}

			while (cont.Count > 0 && readable) {
				TCard tem = InterpretTCard(cont[0], type);

				cont.RemoveAt(0);

				readable = tem.Convert().Valid && readable;
				trunk.Add(tem);
			}

			if (!readable) { ReadError("Deck \"" + name + "\""); return ReadTDeck("Default"); }

			TCard basic = cards[0];
			cards.RemoveAt(0);

			TDeck ret = new TDeck(basic, cards.ToArray(), trunk.ToArray(), vals[0], vals[1], vals[2]);

			return ret;
		}

		public static TGear ReadTGear (string name) {
			List<string> cont = ReadFile("Gear", name + ".gear");
			bool readable = true;
			if (cont == null || cont.Count < 3) { ReadError("Gear \"" + name + "\""); return ReadTGear("Default"); }

			string Name = cont[0];
			// Reading Upgrades, MaxUpgrades, Enchancted
			string[] args = cont[1].Split(' ');
			int[] gearvals = { 0, -1, 0 };

			for (int i = 0; i < args.Length; i++) {
				readable = int.TryParse(args[i], out gearvals[i]) && readable;
			}

			if (!readable) { ReadError("Gear \"" + name + "\""); return ReadTGear("Default"); }

			// Reading Rolls
			List<int> rolls = new List<int>();
			args = cont[2].Split(' ');

			for (int i = 0; i < args.Length; i++) {
				int tem;
				readable = int.TryParse(args[i], out tem) && readable;
				rolls.Add(tem);
			}

			if (!readable) { ReadError("Gear \"" + name + "\""); return ReadTGear("Default"); }

			cont.RemoveRange(0, 3);

			// Read Effects
			List<TGearEffect> possible = new List<TGearEffect>();
			while (cont.Count > 0 && readable) {
				if (cont[0][0] == '#') { cont.RemoveAt(0); break; }
				string[] data = cont[0].Split(':');
				// data[0] = upgradetype min max (afftype)
				string affType = "magic";
				args = data[0].Split(' ');
				int[] typeminmax = new int[] { 0, 0, 0 };

				for (int i = 0; i < args.Length && i < 3; i++) {
					readable = int.TryParse(args[i], out typeminmax[i]) && readable;
				}

				if (args.Length > 3) {
					affType = args[3];
				}

				// data[1] = chance maxrolls (currentrolls)
				args = data[1].Split(' ');
				int[] vals = new int[] { 0, 0, 0 };

				for (int i = 0; i < args.Length; i++) {
					readable = int.TryParse(args[i], out vals[i]) && readable;
				}

				TGearEffect eff = new TGearEffect(typeminmax[0], new int[] { typeminmax[1], typeminmax[2], BaseTypes.Translate(affType) }, vals[0], vals[1], vals[2]);

				cont.RemoveAt(0);

				possible.Add(eff);
			}

			// Read Enchants
			while (cont.Count > 0 && readable) {
				if (cont[0][0] == '#') { cont.RemoveAt(0); break; }
				cont.RemoveAt(0);
				if (false) {
					// not implemented yet
				}
			}

			// Read CurrentEffects
			List<TGearReadEffect> read = new List<TGearReadEffect>();
			while (cont.Count > 0 && readable) {
				args = cont[0].Split(' ');
				int[] vals = new int[] { 0, 0 };
				string affType = "magic";

				for (int i = 0; i < args.Length && i < 2; i++) {
					readable = int.TryParse(args[i], out vals[i]) && readable;
				}

				cont.RemoveAt(0);

				read.Add(new TGearReadEffect(vals[0], vals[1], affType));
			}

			TGear gear = new TGear(Name, GearTable.TempVisual, possible, rolls.ToArray(), gearvals[1]);
			gear.Upgrades = gearvals[0];
			gear.Enchanted = gearvals[1] == 1;

			gear.Read = read;

			return gear;
		}

		// Private methods
		private static List<string> ReadFile (string subdir, string name) {
			string path = Dir + "/" + subdir + "/" + name;
			
			if (File.Exists(path)) {
				return File.ReadAllLines(path).ToList();
			}

			return null;
		}

		private static TCard InterpretTCard(string parse, int type = -1) {
			string[] args = parse.Split(' ');
			int[] def = { 0, 1, 0, 0, 0 };

			for (int ii = 0; ii < args.Length; ii++) {
				int.TryParse(args[ii], out def[ii]);
			}

			TCard tem = new TCard((type != -1 ? type : def[0]), def[1], def[2], def[3], def[4]);

			return tem;
		}

		private static Card InterpretCard(string parse, int type = -1) {
			return InterpretTCard(parse, type).Convert();
        }

		private static void ReadError (string fail) {
			TextUI.PrintFormatted("³Could not read " + fail + ", Reading default⁰");
			TextUI.Wait();
		}
	}
}
