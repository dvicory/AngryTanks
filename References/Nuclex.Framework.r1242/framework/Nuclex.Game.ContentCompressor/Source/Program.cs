#region CPL License
/*
Nuclex Framework
Copyright (C) 2002-2007 Nuclex Development Labs

This library is free software; you can redistribute it and/or
modify it under the terms of the IBM Common Public License as
published by the IBM Corporation; either version 1.0 of the
License, or (at your option) any later version.

This library is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
IBM Common Public License for more details.

You should have received a copy of the IBM Common Public
License along with this library
*/
#endregion

using System;
using System.Collections.Generic;
using System.IO;

using SevenZip.Compression.LZMA;

using Nuclex.Support;
using Nuclex.Support.Parsing;

namespace Nuclex.Game.ContentCompressor {

  /// <summary>
  ///   Contains the main program code for the nuclex game content compression utility
  /// </summary>
  /// <remarks>
  ///   <para>
  ///     Valid command lines
  ///     - Nuclex.Game.ContentCompressor Content\*.*
  ///     - Nuclex.Game.ContentCompressor -package:Content.lzma Content\*.*
  ///   </para>
  /// </remarks>
  public static class Program {

    /// <summary>Program entry point</summary>
    /// <param name="arguments">Not used</param>
    public static void Main(string[] arguments) {
      CommandLine commandLine = CommandLine.Parse(Environment.CommandLine);
      if(commandLine.HasArgument("help") || commandLine.Arguments.Count <= 1) {
        displayHelp();
        return;
      }

      // Obtain the current working directory
      string cwd = System.Environment.CurrentDirectory;

      // Scan any folders the user has specified for .xnb assets
      List<string> files = determineFilesToCompress(commandLine);

      // If the '--package' argument was provided, build a single big package
      // containing all the content files
      if(commandLine.HasArgument("package")) {

        string packagePath = getCommandLineOption(commandLine, "package");
        if(packagePath == null) {
          Console.WriteLine("No package file specified");
          return;
        }

        Console.WriteLine(string.Format("Creating package file {0}...", packagePath));
        LzmaPackageBuilder.Build(packagePath, transformToPackageFile(cwd, files));

      } else { // Otherwise, compress each file individually

        for(int index = 0; index < files.Count; ++index) {
          LzmaContentCompressor.CompressContentFile(files[index]);
        }

      }

      // After successful compression, delete the input files if the user so requested
      // by specifying the '--delete' argument.
      if(commandLine.HasArgument("delete")) {
        Console.WriteLine("Deleting input files...");
        for(int index = 0; index < files.Count; ++index) {
          File.Delete(files[index]);
        }
      }

      Console.WriteLine(
        string.Format(
          "Successfully compressed {0} file{1}",
          files.Count, (files.Count == 1) ? "" : "s"
        )
      );
    }

    /// <summary>Displays syntax help for the application</summary>
    private static void displayHelp() {
      Console.WriteLine("Nuclex LZMA Content Compressor for XNA");
      Console.WriteLine();
      Console.WriteLine("Syntax: Compress [/package:<pkgfile>] [/delete] {@filelist(s)|file(s)}");
      Console.WriteLine();
      Console.WriteLine("/package:<pkgfile>  Will compress the files into a package, similar to");
      Console.WriteLine("                    to a .7z archive. Otherwise, files will be compressed");
      Console.WriteLine("                    individually");
      Console.WriteLine();
      Console.WriteLine("/delete             Deletes the original files after they were");
      Console.WriteLine("                    successfully compressed. Useful for scripting.");
      Console.WriteLine();
      Console.WriteLine("@filelist           Opens the file 'filelist' as a text file and reads");
      Console.WriteLine("                    the files to be compressed from this text file");
      Console.WriteLine();
      Console.WriteLine("file                File or wildcards of files that will be compressed");
    }

    /// <summary>Builds a list of the files that should be compressed</summary>
    /// <param name="commandLine">
    ///   Command line from which to take the paths and masks that will be compressed
    /// </param>
    /// <returns>A list of all files to compress with the absolute paths</returns>
    private static List<string> determineFilesToCompress(CommandLine commandLine) {
      List<string> files = new List<string>();

      // Search for any loose values on the command line
      for(int index = 1; index < commandLine.Arguments.Count; ++index) {

        if(commandLine.Arguments[index].Name == null) {
          string value = commandLine.Arguments[index].Value;

          // if this is a directory, recursively add all files in the directory
          if(value.StartsWith("@")) {
            files.AddRange(allLinesInFile(value.Substring(1)));
          } else if(Directory.Exists(value)) {
            files.AddRange(allFiles(value, "*.*"));
          } else { // otherwise, assume it's a file mask
            string directory = Path.GetDirectoryName(value);
            string mask = Path.GetFileName(value);
            files.AddRange(allFiles(directory, mask));
          }
        }

      }

      return files;
    }

    /// <summary>Enumerates over all lines in a text file</summary>
    /// <param name="path">Path to a text file whose lines will be enumerated</param>
    /// <returns>An enumerable list of strings for the lines in the file</returns>
    private static IEnumerable<string> allLinesInFile(string path) {
      using(
        FileStream stream = new FileStream(
          path, FileMode.Open, FileAccess.Read, FileShare.Read
        )
      ) {
        StreamReader reader = new StreamReader(stream);

        string line;
        while((line = reader.ReadLine()) != null) {
          yield return line;
        }
      }
    }

    /// <summary>Retrieves the value of an option on the command line</summary>
    /// <param name="commandLine">Command line the option will be retrieved from</param>
    /// <param name="optionName">Name of the option that will be retrieved</param>
    /// <returns>The value assigned to the specified option</returns>
    private static string getCommandLineOption(
      CommandLine commandLine, string optionName
    ) {
      for(int index = 0; index < commandLine.Arguments.Count; ++index) {
        if(commandLine.Arguments[index].Name == optionName) {
          return commandLine.Arguments[index].Value;
        }
      }
      return null;
    }

    /// <summary>Transforms a list of absolute paths into a list of package files</summary>
    /// <param name="basePath">
    ///   Base path to which the packaged files' names will be relative
    /// </param>
    /// <param name="files">List of absolute paths to be transformed</param>
    /// <returns>An enumerable list of package files</returns>
    private static IEnumerable<LzmaPackageBuilder.PackageFile> transformToPackageFile(
      string basePath, IEnumerable<string> files
    ) {
      foreach(string file in files) {
        string name = Path.ChangeExtension(file, null);
        name = PathHelper.MakeRelative(basePath, Path.GetFullPath(name));

        yield return new LzmaPackageBuilder.PackageFile(file, name);
      }
    }

    /// <summary>
    ///   Returns an enumerator that iterates over all files that match the specified mask
    ///   in a given directory and its subdirectories.
    /// </summary>
    /// <param name="directory">Directory to begin the enumeration in</param>
    /// <param name="mask">Mask of files to search for</param>
    /// <returns>An enumerator that iterates of all files matching the mask</returns>
    private static IEnumerable<string> allFiles(string directory, string mask) {
      Queue<string> remainingDirectories = new Queue<string>();
      remainingDirectories.Enqueue(directory);

      while(remainingDirectories.Count > 0) {

        string searchedDirectory = remainingDirectories.Dequeue();
        if(searchedDirectory == string.Empty) {
          searchedDirectory = ".";
        }

        string[] subDirectories = Directory.GetDirectories(searchedDirectory);
        foreach(string subDirectory in subDirectories)
          remainingDirectories.Enqueue(subDirectory);

        string[] contentFiles = Directory.GetFiles(searchedDirectory, mask);
        foreach(string contentFile in contentFiles)
          yield return contentFile;

      }
    }

  }

} // namespace Nuclex.Game.ContentCompressor
