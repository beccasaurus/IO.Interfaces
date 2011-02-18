using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using NUnit.Framework;
using IO.Interfaces.Extensions;

namespace IO.Interfaces.Specs {

	/// <summary>Simple base class for our specs to inherit from</summary>
	public class Spec {

		[SetUp]
		public void BeforeEach() {
			if (Directory.Exists(TempDirectory))
				Directory.Delete(TempDirectory, true);
			Directory.CreateDirectory(TempDirectory);
		}

		public static string ProjectDirectory { get { return FullCombinedPath(Directory.GetCurrentDirectory(), "..", ".."); } }
		public static string SpecDirectory    { get { return FullCombinedPath(ProjectDirectory, "spec");                    } }
		public static string TempDirectory    { get { return FullCombinedPath(SpecDirectory, "tmp");                        } }

		/// <summary>Returns a path to a temporary directory that we delete before each spec runs</summary>
		public static string PathToTemp(params string[] relativePathParts) {
			return FullCombinedPath(TempDirectory, relativePathParts);
		}

		/// <summary>.NET 3.5 doesn't support Path.Combine(params string[] parts), so we had to implement it ourselves</summary>
		public static string FullCombinedPath(params string[] parts) {
			return Path.GetFullPath(parts.Combine());
		}

		/// <summary>Lets you say FullCombinedPath("..", otherParts.ToArray()) for making paths</summary>
		public static string FullCombinedPath(string part1, params string[] parts) {
			var list = new List<string>(parts);
			list.Insert(0, part1);
			return FullCombinedPath(list.ToArray());
		}
	}
}
