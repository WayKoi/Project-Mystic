using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Card_Test.Map;
using Card_Test;
using Card_Test.Items;
using Card_Test.Tables;

namespace Sorting {
	public static class Sort {
		public static void QuickSort<T>(IList<T> lis, int low, int high, Func<object, object, bool> Compare) {
			if (low < high) {
				int pin = Partition(lis, low, high, Compare);

				QuickSort(lis, low, pin - 1, Compare);
				QuickSort(lis, pin + 1, high, Compare);
			}
		}

		private static int Partition<T>(IList<T> lis, int low, int high, Func<object, object, bool> Compare) {
			object piv = lis[high];
			int i = low - 1;

			for (int ii = low; ii < high; ii++) {
				if (Compare(lis[ii], piv)) {
					i++;

					Swap(lis, i, ii);
				}
			}

			Swap(lis, i + 1, high);
			return i + 1;

		}

		public static void Swap<T>(IList<T> lis, int a, int b) {
			T tem = lis[a];
			lis[a] = lis[b];
			lis[b] = tem;
		}

		public static void BubbleSort<T>(IList<T> lis, Func<object, object, bool> Compare) {
			bool sorted = false;
			while (!sorted) {
				sorted = true;
				for (int i = 0; i < lis.Count - 1; i++) {
					if (Compare(lis[i], lis[i + 1])) {
						Swap(lis, i, i + 1);
						sorted = false;
					}
				}
			}
		}
	}

	public static class Compare {
		public static bool String(object A, object B) {
			string a = A as string;
			string b = B as string;
			// returns true if a < b
			if (a.Length < b.Length) {
				return true;
			} else if (a.Length > b.Length) {
				return false;
			} else {
				// we actually have to check the string
				for (int i = 0; i < a.Length; i++) {
					if (a[i] < b[i]) {
						return true;
					} else if (a[i] > b[i]) {
						return false;
					}
				}
			}

			return false;
		}

		public static bool BattleChar (object A, object B) {
			if (!(A is BattleChar) || !(B is BattleChar)) { return false; }
			return (A as BattleChar).Side > (B as BattleChar).Side;
		}

		public static bool Move (object A, object B) {
			if (!(A is Move) || !(B is Move)) { return false; }
			return (A as Move).value > (B as Move).value;
		}

		public static bool Card(object A, object B) {
			if (!(A is Card) || !(B is Card)) { return false; }

			Card ca = A as Card;
			Card cb = B as Card;

			if (ca.Element.Tier == cb.Element.Tier) {
				if (ca.Type == cb.Type) {
					if (ca.Tier == cb.Tier) {
						if (ca.Mod == cb.Mod) {
							return ca.Sub > cb.Sub;
						}

						return ca.Mod > cb.Mod;
					}

					return ca.Tier > cb.Tier;
				}

				return ca.Type > cb.Type;
			}

			return ca.Element.Tier > cb.Element.Tier;
		}

		public static bool InverseCard(object A, object B) {
			if (!(A is Card) || !(B is Card)) { return false; }

			Card ca = A as Card;
			Card cb = B as Card;

			if (ca.Element.Tier == cb.Element.Tier) {
				if (ca.Type == cb.Type) {
					if (ca.Tier == cb.Tier) {
						if (ca.Mod == cb.Mod) {
							return ca.Sub < cb.Sub;
						}

						return ca.Mod < cb.Mod;
					}

					return ca.Tier < cb.Tier;
				}

				return ca.Type < cb.Type;
			}

			return ca.Element.Tier < cb.Element.Tier;
		}

		public static bool PossibleTarg (object A, object B) {
			if (!(A is PossibleTarg) || !(B is PossibleTarg)) { return false; }
			return (A as PossibleTarg).Value < (B as PossibleTarg).Value;
		}

		public static bool ShopItem (object A, object B) {
			if (!(B is ShopItem) || !(B is ShopItem)) { return false; }
			return (A as ShopItem).Type > (B as ShopItem).Type;
		}

		public static bool GearEffect(object A, object B) {
			if (!(A is GearEffect) || !(B is GearEffect)) { return false; }
			return (A as GearEffect).Type > (B as GearEffect).Type;
		}

		public static bool PackE(object A, object B) {
			if (!(A is PackE) || !(B is PackE)) { return false; }
			return Compare.Card((A as PackE).Card.Convert(), (B as PackE).Card.Convert());
		}

		public static bool TGearEffect(object A, object B) {
			if (!(A is TGearEffect) || !(B is TGearEffect)) { return false; }
			
			TGearEffect a = A as TGearEffect;
			TGearEffect b = B as TGearEffect;

			if (a.Type > b.Type) {
				return true;
			} else if (a.Type < b.Type) {
				return false;
			}

			if (a.Min == b.Min) {	
				return a.Max > b.Max;
			}

			return a.Min > b.Min;

		}

		public static bool SimNode(object A, object B) {
			if (!(A is SimNode) || !(B is SimNode)) { return false; }

			SimNode a = A as SimNode;
			SimNode b = B as SimNode;

			return a.Value < b.Value;
		}
	}
}
