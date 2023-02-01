using System;
using System.Collections.Generic;
using System.Text;

namespace Card_Test.Base {
	public struct KeyPair {
		public string Name;
		public int Amount;

		public KeyPair(string name, int amount) {
			Name = name;
			Amount = amount;
		}
	}
}
