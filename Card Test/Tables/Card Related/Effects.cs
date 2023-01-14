using System;
using System.Collections.Generic;
using System.Text;

namespace Card_Test.Tables {
	public static class Effects {
		// these are purely for reactions
		public static Effect[] Table = {
			new Effect("None"),
			new Effect("³Burn⁰"),
			new Effect("₀Wet⁰"),
			new Effect("₂Muddy⁰"),
			new Effect("⁸Wind⁰", false),
			new Effect("⁵Frosty⁰"),
			new Effect("²Dry⁰"),
			new Effect("⁴Dark⁰", false),
			new Effect("⁸Light⁰", false),
			new Effect("¹Sprout⁰"),
			new Effect("²Shock⁰", false)
		}; // ⁰¹²³⁴⁵⁶⁷⁸⁹

	}

	public class Effect {
		public string Name;
		// if stays = false, that means that the element is purely used for reactions
		public bool Stays; // if the effect stays after it is cast
		
		public Effect(string name, bool stays = true) {
			Name = name;
			Stays = stays;
		}
	}
}
