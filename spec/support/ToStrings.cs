using System;
using System.Linq;
using System.Collections.Generic;

namespace System.Collections.Generic {
	public static class ToStringsListExtension {
		public static List<string> ToStrings<T>(this List<T> self) {
			return self.Select(o => o.ToString()).ToList();
		}
	}
}
