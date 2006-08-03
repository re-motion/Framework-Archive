// --------------------------------------------------------------------------------------
// $Workfile: Program.cs $
// $Revision: 1 $ of $Date: 10.03.06 15:02 $ by $Author: Harald-rene.flasch $
//
// Copyright 2006
// rubicon informationstechnologie gmbh
// --------------------------------------------------------------------------------------

using System;
using Rubicon.Text.CommandLine;
using Rubicon.ObjectBinding.Web.CodeGenerator;

namespace Rubicon.ObjectBinding.Web.CodeGenerator
{
	internal class Program
	{
		static string ApplicationInfo
		{
			get
			{
				string version = System.Reflection.Assembly.GetCallingAssembly().FullName;
				string[] info = version.Split(',');

				return "UIGen Version " + info[1].Replace("Version=", string.Empty).Trim();
			}
		}

		static string CopyrightInformation
		{
			get { return "Copyright (c) 2006, rubicon informationstechnologie gmbh"; }
		}

		static void PrintUsage(CommandLineClassParser parser, string message)
		{
			if (message != null)
				System.Console.WriteLine(message);

			System.Console.WriteLine("Usage:");
			System.Console.WriteLine(parser.GetAsciiSynopsis("UIGen", 79));
		}

		static int Main(string[] args)
		{
			Arguments arguments;
			CommandLineClassParser parser = new CommandLineClassParser(typeof(Arguments));

			try
			{
				arguments = (Arguments)parser.Parse(args);
			}
			catch (CommandLineArgumentException e)
			{
				PrintUsage(parser, e.Message);
				return 1;
			}

			try
			{
				if (args.Length == 0)
				{
					PrintUsage(parser, null);
					return 0;
				}

				if (arguments.ApplicationInfo)
				{
					System.Console.WriteLine(ApplicationInfo);
					System.Console.WriteLine(CopyrightInformation);
					System.Console.WriteLine();
				}

				if (arguments.Placeholders != null)
				{
					UiGenerator generator = new UiGenerator(
							new UiGenerator.OutputMethod(System.Console.WriteLine),
							arguments.Placeholders,
              arguments.AssemblyDirectory);

					generator.DumpPlaceholders();
				}

				if (arguments.UIGenConfiguration != null)
				{
					UiGenerator generator = new UiGenerator(new UiGenerator.OutputMethod(
							System.Console.WriteLine),
							arguments.UIGenConfiguration,
              arguments.AssemblyDirectory);

					generator.Build();
				}
			}
			catch (CodeGeneratorException e)
			{
				System.Console.Error.WriteLine("UIGen error: {0}", e.Message);
			}
			catch (Exception e)
			{
				System.Console.Error.WriteLine("Execution aborted: {0}", e.Message);
			}

			return 0;
		}
	}
}
