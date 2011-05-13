// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// as published by the Free Software Foundation; either version 2.1 of the 
// License, or (at your option) any later version.
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
using System.IO;
using NUnit.Framework;
using Remotion.Data.DomainObjects.Persistence.Rdbms.SchemaGeneration;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Persistence.Rdbms.SchemaGeneration
{
  [TestFixture]
  public class FileGeneratorTest : SchemaGenerationTestBase
  {
    private FileGenerator _fileGenerator;
    private Script _script;

    public override void TestFixtureSetUp ()
    {
      base.TestFixtureSetUp ();

      if (Directory.Exists ("TestOutputPath"))
        Directory.Delete ("TestOutputPath", true);
    }

    public override void SetUp ()
    {
      base.SetUp ();

      _fileGenerator = new FileGenerator ("TestOutputPath");
      _script = new Script (SchemaGenerationFirstStorageProviderDefinition, "SetupScript", "TearDownScript");
    }

    public override void TearDown ()
    {
      base.TearDown ();

      if (Directory.Exists ("TestOutputPath"))
        Directory.Delete ("TestOutputPath", true);
    }

    [Test]
    public void WriteScriptsToDisk_IncludeStorageProviderNameIsTrue ()
    {
      var setupFileName = @"TestOutputPath\SetupDB_SchemaGenerationFirstStorageProvider.sql";
      var tearDownFileName = @"TestOutputPath\TearDownDB_SchemaGenerationFirstStorageProvider.sql";
      if (File.Exists (setupFileName))
        File.Delete (setupFileName);
      if (File.Exists (tearDownFileName))
        File.Delete (tearDownFileName);

      _fileGenerator.WriteScriptsToDisk (_script, true);

      Assert.That (File.Exists (setupFileName), Is.True);
      Assert.That (File.ReadAllText (setupFileName), Is.EqualTo ("SetupScript"));
      Assert.That (File.Exists (tearDownFileName), Is.True);
      Assert.That (File.ReadAllText (tearDownFileName), Is.EqualTo ("TearDownScript"));
      File.Delete (setupFileName);
      File.Delete (tearDownFileName);
    }

    [Test]
    public void WriteScriptsToDisk_IncludeStorageProviderNameIsFalse ()
    {
      var setupFileName = @"TestOutputPath\SetupDB.sql";
      var tearDownFileName = @"TestOutputPath\TearDownDB.sql";
      if (File.Exists (setupFileName)) File.Delete (setupFileName);
      if (File.Exists (tearDownFileName)) File.Delete (tearDownFileName);
      
      _fileGenerator.WriteScriptsToDisk (_script, false);

      Assert.That (File.Exists (setupFileName), Is.True);
      Assert.That (File.ReadAllText (setupFileName), Is.EqualTo ("SetupScript"));
      Assert.That (File.Exists (tearDownFileName), Is.True);
      Assert.That (File.ReadAllText (tearDownFileName), Is.EqualTo ("TearDownScript"));
      File.Delete (setupFileName);
      File.Delete (tearDownFileName);
    }
  }
}