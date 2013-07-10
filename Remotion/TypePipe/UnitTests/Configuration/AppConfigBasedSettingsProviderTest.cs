// Copyright (c) rubicon IT GmbH, www.rubicon.eu
//
// See the NOTICE file distributed with this work for additional information
// regarding copyright ownership.  rubicon licenses this file to you under 
// the Apache License, Version 2.0 (the "License"); you may not use this 
// file except in compliance with the License.  You may obtain a copy of the 
// License at
//
//   http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software 
// distributed under the License is distributed on an "AS IS" BASIS, WITHOUT 
// WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.  See the 
// License for the specific language governing permissions and limitations
// under the License.
// 

using System;
using NUnit.Framework;
using Remotion.Development.UnitTesting;
using Remotion.Development.UnitTesting.Configuration;
using Remotion.TypePipe.Configuration;

namespace Remotion.TypePipe.UnitTests.Configuration
{
  [TestFixture]
  public class AppConfigBasedSettingsProviderTest
  {
    private AppConfigBasedSettingsProvider _provider;
    private TypePipeConfigurationSection _section;

    [SetUp]
    public void SetUp ()
    {
      _section = new TypePipeConfigurationSection();
      _provider = new AppConfigBasedSettingsProvider();
      PrivateInvoke.SetNonPublicField (_provider, "_section", _section);
    }

    [Test]
    public void ForceStrongNaming_False ()
    {
      var xmlFragment = "<typePipe/>";
      ConfigurationHelper.DeserializeSection (_section, xmlFragment);

      Assert.That (_provider.ForceStrongNaming, Is.False);
    }

    [Test]
    public void ForceStrongNaming_True ()
    {
      var xmlFragment = "<typePipe><forceStrongNaming/></typePipe>";
      ConfigurationHelper.DeserializeSection (_section, xmlFragment);

      Assert.That (_provider.ForceStrongNaming, Is.True);
      Assert.That (_provider.KeyFilePath, Is.Empty);
    }

    [Test]
    public void KeyFilePath ()
    {
      var xmlFragment = @"<typePipe><forceStrongNaming keyFilePath=""C:\key.snk""/></typePipe>";
      ConfigurationHelper.DeserializeSection (_section, xmlFragment);

      Assert.That (_provider.KeyFilePath, Is.EqualTo (@"C:\key.snk"));
    }

    [Test]
    public void EnableComplexSerialization_False ()
    {
      var xmlFragment = "<typePipe/>";
      ConfigurationHelper.DeserializeSection (_section, xmlFragment);

      Assert.That (_provider.EnableSerializationWithoutAssemblySaving, Is.False);
    }

    [Test]
    public void EnableComplexSerialization_True ()
    {
      var xmlFragment = "<typePipe><enableSerializationWithoutAssemblySaving/></typePipe>";
      ConfigurationHelper.DeserializeSection (_section, xmlFragment);

      Assert.That (_provider.EnableSerializationWithoutAssemblySaving, Is.True);
    }

    [Test]
    public void GetSettings_Defaults ()
    {
      var xmlFragment = "<typePipe/>";
      ConfigurationHelper.DeserializeSection (_section, xmlFragment);

      var settings = _provider.GetSettings();

      Assert.That (settings.ForceStrongNaming, Is.False);
      Assert.That (settings.KeyFilePath, Is.Empty);
      Assert.That (settings.EnableSerializationWithoutAssemblySaving, Is.False);
    }

    [Test]
    public void GetSettings_CustomValues ()
    {
      var xmlFragment = @"<typePipe><forceStrongNaming keyFilePath=""C:\key.snk""/><enableSerializationWithoutAssemblySaving/></typePipe>";
      ConfigurationHelper.DeserializeSection (_section, xmlFragment);

      var settings = _provider.GetSettings();

      Assert.That (settings.ForceStrongNaming, Is.True);
      Assert.That (settings.KeyFilePath, Is.EqualTo (@"C:\key.snk"));
      Assert.That (settings.EnableSerializationWithoutAssemblySaving, Is.True);
    }
  }
}