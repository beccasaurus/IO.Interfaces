using System;

/// <summary>Public extensions you get when using the IFile namespace</summary>
/// <remarks>
/// When you use the IFile namespace you get:
///
///  - the IFile/IDirectory interfaces
///  - a few extensions on global classes, such as String.ToFile() and String.ToDirectory()
///
/// For any of our other extensions (used internally), you can use IFile.Extensions.
/// </remarks>
namespace IFile {

	/// <summary>Extension methods on String that you get when using the IFile namespace</summary>
	public static class StringExtensions {

		/// <summary>Assuming this string is the path to a file, returns an IFile</summary>
		public static IFile AsFile(this string path) {
			return new Implementations.RealFile(path);
		}

		/// <summary>Assuming this string is the path to a directory, returns an IDirectory</summary>
		public static IDirectory AsDirectory(this string path) {
			return new Implementations.RealDirectory(path);
		}

		/// <summary>Alias for AsDirectory()</summary>
		public static IDirectory AsDir(this string path) {
			return path.AsDirectory();
		}
	}
}
