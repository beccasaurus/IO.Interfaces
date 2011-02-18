using System;
using IO.Interfaces;

/// <summary>Sample IFile and IDirectory concrete implementations (used internally)</summary>
/// <remarks>
/// We provide these simple concrete implementations of IFile and IDirectory.
///
/// We don't want to pollute the IFile namespace with these, as you'll probably never, ever 
/// use them directly, so we put them here.
///
/// Instead, to get an IFile/IDirectory, the easiest thing to do is call @"C:\".AsDirectory() 
/// or "README".AsFile() using our global extension methods.
/// </remarks>
namespace IO.Interfaces.Implementations {

	/// <summary>Concrete implementation of IFile</summary>
	public class RealFile : IFile {
		public RealFile(){}
		public RealFile(string path){ Path = path; }
		public string Path { get; set; }
		public override string ToString(){ return Path; }
	}

	/// <summary>Concrete implementation of IDirectory</summary>
	public class RealDirectory : IDirectory {
		public RealDirectory(){}
		public RealDirectory(string path){ Path = path; }
		public string Path { get; set; }
		public override string ToString(){ return Path; }
	}
}
