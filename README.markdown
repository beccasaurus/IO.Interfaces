IFile
=====

Why?
----

I've been working on a project that has lots of classes that represent a file or a directory.

Let's say you have a class called `ZipFile` or `Png` to represent .zip or .png files on the file system. 
Or you might have a class called `Scripts` that represents a directory filled with script files.

It's kind of a pain to re-implement the same things over and over again for each of these classes. 
For example, you would probably want to implement a method/property for checking to see if a `ZipFile` 
or `Png` actually `Exists()`.  If you might be copying or moving these files, you would want to implement 
`Copy()` and `Move()` methods for each.

What a pain.

How?
----

So, what's a good solution?  Interfaces + Extension Methods to the rescue!

    using IFile;

    public class ZipFile : SomeBaseClass, IFile {
      public string Path { get; set; }
    }

    public class Png : SomeBaseClass, IFile {
      public string Path { get; set; }
    }

    public class Scripts : SomeBaseClass, IDirectory {
      public string Path { get; set; }
    }

All you need to do is add IFile or IDirectory to you class and you'll get abunchof methods 
on your instances.  All IFile and IDirectory require is a `public string Path` property.

    var file = MyZipOrPngFileOrWhatever();

    if (file.Exists())
      file.Move(@"C:\wherever");

    myScriptsDirectory.Files().ForEach(file => Console.WriteLine("Script: {0}", file));

That (and more) will all work once you've added IFile/IDirectory to your class.

Alpha
-----

NOTE: This code is tested but it was pulled out of another library, so it could have MUCH better tests.  This is Alpha!

Luckily, most of the code is really trivial  :)

Installation and Usage
----------------------

This will be packaged soon ... for now, feel free to use the code directly

License
-------

IFile is released under the MIT license.
