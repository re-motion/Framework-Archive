// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// version 3.0 as published by the Free Software Foundation.
// 
// re-motion is distributed in the hope that it will be useful, 
// but WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
// GNU Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public License
// along with re-motion; if not, see http://www.gnu.org/licenses.
// 
using System;
using Microsoft.Scripting.Hosting;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;

namespace Remotion.Scripting.UnitTests
{
  [TestFixture]
  public class ScriptEnvironmentTest
  {
    [Test]
    public void Ctor_ScriptScope ()
    {
      ScriptScope scriptScope = ScriptingHelper.CreateScriptScope (ScriptingHost.ScriptLanguageType.Python);
      var scriptEnvironment = new ScriptEnvironment (scriptScope);
      Assert.That (scriptEnvironment.ScriptScope, Is.EqualTo (scriptScope));
    }

    [Test]
    public void DefaultCtor ()
    {
      var scriptEnvironment = new ScriptEnvironment ();
      Assert.That (scriptEnvironment.ScriptScope, Is.Not.Null);
    }

    [Test]
    public void Import ()
    {
      var scriptEnvironment = new ScriptEnvironment ();
      scriptEnvironment.Import ("Remotion", "Remotion.Diagnostics.ToText", "To", "ToTextBuilder");
      Assert.That (scriptEnvironment.ScriptScope.GetVariable("To"), Is.Not.Null);
      Assert.That (scriptEnvironment.ScriptScope.GetVariable ("ToTextBuilder"), Is.Not.Null);
    }
  }
}