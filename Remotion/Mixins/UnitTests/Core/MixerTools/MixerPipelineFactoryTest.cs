// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (c) rubicon IT GmbH, www.rubicon.eu
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
using NUnit.Framework;
using Remotion.Mixins.MixerTools;

namespace Remotion.Mixins.UnitTests.Core.MixerTools
{
  [TestFixture]
  public class MixerPipelineFactoryTest
  {
    private MixerPipelineFactory _factory;

    [SetUp]
    public void SetUp ()
    {
      _factory = new MixerPipelineFactory ("Assembly");
    }
    
    [Test]
    public void CreatePipeline ()
    {
      var pipeline = _factory.CreatePipeline (@"c:\directory");

      // TODO 5370: Inspect settings copied from default pipeline.

      Assert.That (pipeline.CodeManager.AssemblyDirectory, Is.EqualTo (@"c:\directory"));
      Assert.That (pipeline.CodeManager.AssemblyNamePattern, Is.EqualTo ("Assembly"));
    }

    [Test]
    public void GetModulePath ()
    {
      Assert.That (_factory.GetModulePath (@"c:\directory"), Is.EqualTo (@"c:\directory\Assembly.dll"));
    }
  }
}
