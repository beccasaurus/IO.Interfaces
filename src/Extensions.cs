using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;

/// <summary>Extensions used by IFile</summary>
/// <remarks>
/// If you use the IFile namespace, you will get all of our IFile and IDirectory extensions.
///
/// IFile and IDirectory make use of other extensions which you may or may not want to opt in to use.
///
/// These extensions are contained here, in IFile.Extensions.
/// </remarks>
namespace IFile.Extensions {

	public static class StringArrayExtensions {

		/// <summary>Does a Path.Combine with the given string[]</summary>
		/// <remarks>
		/// IFile aims to be .NET 3.5 compatible, so we can't use .NET 4.0's Path.Combine(string[] parts)
		///
		/// So we use this, which does the same thing!
		/// </remarks>
		public static string Combine(this string[] parts) {
			var list = new List<string>(parts);
			var path = list.First();
			list.RemoveAt(0);
			foreach (var part in list)
				path = Path.Combine(path, part);
			return path;
		}
	}
}
