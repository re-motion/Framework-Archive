// 
//  Copyright (C) 2005 - 2008 rubicon informationstechnologie gmbh
// 
//  This program is free software: you can redistribute it and/or modify it under 
//  the terms of the re:motion license agreement in license.txt. If you did not 
//  receive it, please visit http://www.re-motion.org/licensing.
//  
//  Unless otherwise provided, this software is distributed on an "AS IS" basis, 
//  WITHOUT WARRANTY OF ANY KIND, either express or implied. 
// 
// 

using System;
using System.IO;
using Remotion.SecurityManager.AclTools.Expansion.ConsoleApplication;

namespace Remotion.SecurityManager.UnitTests.AclTools.Expansion.ConsoleApplication
{
  public class ConsoleApplicationTestApplicationRunner : IApplicationRunner<ConsoleApplicationTestSettings>
  {
    public void Run (ConsoleApplicationTestSettings settings, TextWriter errorWriter, TextWriter logWriter)
    {
    }
  }
}