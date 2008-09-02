/* Copyright (C) 2005 - 2008 rubicon informationstechnologie gmbh
 *
 * This program is free software: you can redistribute it and/or modify it under 
 * the terms of the re:motion license agreement in license.txt. If you did not 
 * receive it, please visit http://www.re-motion.org/licensing.
 * 
 * Unless otherwise provided, this software is distributed on an "AS IS" basis, 
 * WITHOUT WARRANTY OF ANY KIND, either express or implied. 
 */

// --------------------------------------------------------------------------------------
// $Workfile: Program.cs $
// $Revision: 1 $ of $Date: 10.03.06 15:02 $ by $Author: Harald-rene.flasch $
//
// Copyright 2006
// rubicon informationstechnologie gmbh
// --------------------------------------------------------------------------------------

using System;
using Remotion.Text.CommandLine;
using Remotion.ObjectBinding.Web.CodeGenerator;

namespace Remotion.ObjectBinding.Web.CodeGenerator
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
					UiGenerator generator = new UiGenerator(
              new UiGenerator.OutputMethod(System.Console.WriteLine),
							arguments.UIGenConfiguration,
              arguments.AssemblyDirectory);

					generator.Build();
				}
			}
			catch (CodeGeneratorException e)
			{
				System.Console.Error.WriteLine("UIGen error: {0}", e.Message);
			}
      /*
			catch (Exception e)
			{ Not meaningful enough, so RG commented it out. I want that exception raw.
				System.Console.Error.WriteLine("Execution aborted: {0}", e.Message);
			}
       */

			return 0;
		 }
	}
}
