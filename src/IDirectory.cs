using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using IO.Interfaces.Extensions;
using IO.Interfaces.Implementations;

namespace IO.Interfaces {

	/// <summary>Represents a directory.  Any class that has a public string Path property can be an IDirectory</summary>
	/// <remarks>Why?  So you can use the IDirectoryExtensions with your IDirectory class to Copy(), Move(), Delete(), etc the Directory.</remarks>
	public interface IDirectory {
		string Path { get; set; }
	}

	/// <summary>The actual methods for an IDirectory</summary>
	public static class IDirectoryExtensions {

		/// <summary>returns the IDirectory.Path, if it exists, else null</summary>
		public static string Path(this IDirectory dir) {
			return (dir == null) ? null : dir.Path;
		}

		public static bool   Exists(this IDirectory dir)       { return Directory.Exists(dir.Path); }
		public static bool   DoesNotExist(this IDirectory dir) { return ! dir.Exists(); }
		public static string Name(this IDirectory dir)         { return System.IO.Path.GetFileName(dir.Path); }
		public static void   Delete(this IDirectory dir)       { Directory.Delete(dir.Path, true); }
		public static string FullPath(this IDirectory dir)     { return System.IO.Path.GetFullPath(dir.Path); }

		/// <summary>Creates this directory (if it doesn't exist)</summary>
		public static IDirectory Create(this IDirectory dir) {
			if (! dir.Exists())
				Directory.CreateDirectory(dir.Path);
			return dir;
		}

		/// <summary>
		/// Returns an IFile with the specified path relative to this IDirectory's Path. 
		/// It might not exist, but we don't return null!
		/// </summary>
		public static IFile GetFile(this IDirectory dir, string path) {
			return new RealFile(System.IO.Path.Combine(dir.Path, path));
		}

		/// <summary>
		/// Returns an IDirectory with the specified path relative to this IDirectory's Path. 
		/// It might not exist, but we don't return null!
		/// </summary>
		public static IDirectory GetDirectory(this IDirectory dir, string path) {
			return new RealDirectory(System.IO.Path.Combine(dir.Path, path));
		}

		/// <summary>Shortcut to GetDirectory</summary>
		public static IDirectory GetDir(this IDirectory dir, string path){ return dir.GetDirectory(path); }

		public static IDirectory Copy(this IDirectory dir, params string[] destinationParts) {
			var destination = destinationParts.Combine();
			if (Directory.Exists(destination))
				dir.CopyToExactPath(System.IO.Path.Combine(destination, dir.Name()));
			else
				dir.CopyToExactPath(destination);
			return dir;
		}

		public static string Relative(this IDirectory dir, string path) {
			var fullPath    = path.AsFile().FullPath();
			var fullDirPath = dir.FullPath();

			var index = fullPath.IndexOf(fullDirPath);
			if (index >= 0) 
				return fullPath.Substring(index + fullDirPath.Length);

			// This is ugly!  :(
			var dirsUp  = 1;
			var baseDir = System.IO.Path.GetDirectoryName(dir.Path);
			while (baseDir != null) {
				if (fullPath.Contains(baseDir)) {
					var dots = "";
					for (int i = 0; i < dirsUp; i++)
						dots = ".." + System.IO.Path.DirectorySeparatorChar.ToString() + dots;
					return dots + baseDir.AsDir().Relative(fullPath).TrimStart(System.IO.Path.DirectorySeparatorChar);
				}
				dirsUp++;
				baseDir = System.IO.Path.GetDirectoryName(baseDir);
			}

			// Didn't find a common path ... not relative?
			return null;
		}

		public static string Relative(this IDirectory dir, IFile file) {
			return dir.Relative(file.Path);
		}

		public static IDirectory CopyToExactPath(this IDirectory dir, string exactPath) {
			Directory.CreateDirectory(exactPath);
			dir.Dirs().ForEach(fullDir => Directory.CreateDirectory(System.IO.Path.Combine(exactPath, dir.Relative(fullDir.Path).TrimStart('/'))));
			dir.Files().ForEach(file => file.Copy(System.IO.Path.Combine(exactPath, dir.Relative(file.Path).TrimStart('/'))));
			dir.Path = exactPath;
			return dir;
		}

		public static IDirectory Move(this IDirectory dir, params string[] destinationParts) {
			var destination = destinationParts.Combine();
			if (Directory.Exists(destination)) {
				// move INTO the existing directory
				var newPath = System.IO.Path.Combine(destination, dir.Name());
				Directory.Move(dir.Path, newPath);
				dir.Path = newPath;
			} else {
				// move to the exact destination
				Directory.Move(dir.Path, destination);
				dir.Path = destination;
			}
			return dir;
		}

		/// <summary>Returns all of the files in this directory (as as list of IFile)</summary>
		public static List<IFile> Files(this IDirectory dir) {
			return Directory.GetFiles(dir.Path, "*", SearchOption.AllDirectories).Select(path => new RealFile(path) as IFile).ToList();
		}

		/// <summary>Returns just the top level of subdirectories in this directory.  See Directories() to get *all* subdirectories</summary>
		public static List<IDirectory> SubDirectories(this IDirectory dir) {
			return Directory.GetDirectories(dir.Path).Select(path => path.AsDir()).ToList();
		}

		/// <summary>Alias for IDirectory.SubDirectories()</summary>
		public static List<IDirectory> SubDirs(this IDirectory dir) { return dir.SubDirectories(); }

		/// <summary>Returns all of the directories in this directory (as as list of IDirectory)</summary>
		public static List<IDirectory> Directories(this IDirectory dir) {
			return Directory.GetDirectories(dir.Path, "*", SearchOption.AllDirectories).Select(path => path.AsDir()).ToList();
		}

		/// <summary>Shortcut for Directories()</summary>
		public static List<IDirectory> Dirs(this IDirectory dir) { return dir.Directories(); }

		public static List<IFile> Search(this IDirectory dir, string matcher) {
			return dir.Search(matcher, true);
		}

		public static List<IFile> Search(this IDirectory dir, string matcher, bool ignoreCase) {
			// We have to initially substitude ** out for something besides a single * because then we replace single *'s.
			matcher = "^" + matcher.
							Replace("\\", "/").                           // replace backslashes with forward slashes
							Replace(".", "\\.").                          // escape periods
							Replace("**", ".REAL_REGEX_STAR").            // setup ** replacement
							Replace("*", @"[^\/]*").                      // replace * with "anything that's not a forward slash"
							Replace("REAL_REGEX_STAR", "*") + "$";        // add * back for ** replacement

			var regex = ignoreCase ? new Regex(matcher, RegexOptions.IgnoreCase) : new Regex(matcher);
			return dir.Search(regex);
		}

		public static List<IFile> Search(this IDirectory dir, Regex regex) {
			return dir.Files().Where(file => {
				var relative = dir.Relative(file).TrimStart('\\').TrimStart('/');
				return regex.IsMatch(relative);
			}).ToList();
		}

		public static string ToString(this IDirectory dir) { return dir.Path; }
	}

	public static class ListOfIDirectoryExtensions {
		public static List<string> Paths(this List<IDirectory> dirs) {
			return dirs.Select(dir => dir.Path).ToList();
		}
	}
}
