﻿namespace ZipDiff
{
	using System;
	using System.IO;
	using ZipDiff.Core;
	using ZipDiff.Core.Output;

	class Program
	{
		const int EXITCODE_ERROR = 2;
		const int EXITCODE_DIFF = 1;

		static void Main(string[] args)
		{
			OutputLogo();

			var options = new Options();
			if (!CommandLine.Parser.Default.ParseArguments(args, options))
			{
				Console.WriteLine("No zip files have been specified.");
				Environment.Exit(EXITCODE_ERROR);
			}

			var file1 = new FileInfo(options.File1);
			var file2 = new FileInfo(options.File2);

			CheckFile(file1);
			CheckFile(file2);

			Console.WriteLine("File 1 = {0}", file1.Name);
			Console.WriteLine("File 2 = {0}", file2.Name);
			Console.WriteLine();

			var calc = new DifferenceCalculator(file1, file2)
			{
				CompareCrcValues = options.CompareCrcValues,
				CompareTimestamps = options.CompareTimestamps,
				IgnoreCase = options.IgnoreCase,
				RegExPattern = options.RegExPattern
			};

			var diff = calc.GetDifferences();

			if (!string.IsNullOrWhiteSpace(options.OutputFile))
			{
				WriteOutputFile(options.OutputFile, diff);
			}

			if (diff.HasDifferences())
			{
				if (options.Verbose)
				{
					Console.WriteLine(diff);
					Console.WriteLine("{0} and {1} are different", options.File1, options.File2);
				}

				if (options.ExitWithErrorOnDifference)
				{
					Environment.Exit(EXITCODE_DIFF);
				}
			}
			else
			{
				Console.WriteLine("No differences found.");
			}
		}

		static void CheckFile(FileInfo file)
		{
			if (!file.Exists)
			{
				Console.WriteLine("'{0}' does not exist.", file.Name);
				Environment.Exit(EXITCODE_ERROR);
			}

			if ((file.Attributes & FileAttributes.Directory) == FileAttributes.Directory)
			{
				Console.WriteLine("'{0}' is a directory.", file.Name);
				Environment.Exit(EXITCODE_ERROR);
			}
		}

		static void WriteOutputFile(string filename, Differences diff)
		{
			var builder = BuilderFactory.Create(filename);
			builder.Build(filename, diff);
		}

		static void OutputLogo()
		{
			Console.WriteLine(@"
  _______       _____  _  __  __ 
 |___  (_)     |  __ \(_)/ _|/ _|
    / / _ _ __ | |  | |_| |_| |_ 
   / / | | '_ \| |  | | |  _|  _|
  / /__| | |_) | |__| | | | | |  
 /_____|_| .__/|_____/|_|_| |_|  
         | |                     
         |_|                     
");
		}
	}
}