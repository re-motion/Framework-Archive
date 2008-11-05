/* Copyright (C) 2005 - 2008 rubicon informationstechnologie gmbh
 *
 * This program is free software: you can redistribute it and/or modify it under 
 * the terms of the re:motion license agreement in license.txt. If you did not 
 * receive it, please visit http://www.re-motion.org/licensing.
 * 
 * Unless otherwise provided, this software is distributed on an "AS IS" basis, 
 * WITHOUT WARRANTY OF ANY KIND, either express or implied. 
 */

using System;
using Remotion.Diagnostics.ToText;
using Remotion.SecurityManager.AclTools.Expansion.ConsoleApplication;
using Remotion.Text.CommandLine;
using Remotion.Utilities;

namespace Remotion.SecurityManager.AclTools.Expansion
{
  public class AclExpanderApplicationSettings : ConsoleApplicationSettings, IToText
  {
    [CommandLineStringArgument ("user", true, Placeholder = "accountants/john.doe", Description = "Fully qualified name of user(s) to query access types for.")]
    public string UserName;

    [CommandLineStringArgument ("last", true, Placeholder = "Doe", Description = "Last name of user(s) to query access types for.")]
    public string UserLastName;

    [CommandLineStringArgument ("first", true, Placeholder = "John", Description = "First name of user(s) to query access types for.")]
    public string UserFirstName;

    [CommandLineStringArgument ("dir", true, Placeholder = "c:\\temp", Description = "Directory the ACL-expansion gets written to.")]
    public string Directory = ".";

    [CommandLineStringArgument ("culture", true, Placeholder = "", Description = "Culture to set for output (e.g. en-US, de-AT).")]
    public string CultureName = "de-AT";

    [CommandLineFlagArgument ("multifile", false, Description = "Whether to create a single file for all users + permissions or a master file and several detail files.")]
    public bool UseMultipleFileOutput;

    [CommandLineFlagArgument ("verbose", false, Description = "Verbose output")]
    public bool Verbose;


    public void ToText (IToTextBuilder toTextBuilder)
    {
      ArgumentUtility.CheckNotNull ("toTextBuilder", toTextBuilder);
      toTextBuilder.sb ().e ("user", UserName).e ("last", UserLastName).e ("first", UserFirstName).e ("dir", Directory).e ("culture", CultureName).e ("multifile", UseMultipleFileOutput).e ("verbose", Verbose).se ();
    }
  }
}