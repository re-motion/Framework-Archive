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
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.ExtensibleEnums;
using Remotion.ExtensibleEnums.Infrastructure;
using Remotion.UnitTests.ExtensibleEnums.TestDomain;
using Rhino.Mocks;

namespace Remotion.UnitTests.ExtensibleEnums
{
  [TestFixture]
  public class ExtensibleEnumDefinitionTest
  {
    private Color _red;
    private Color _green;
    private Color _blue;

    [SetUp]
    public void SetUp ()
    {
      _red = new Color ("Red");
      _green = new Color ("Green");
      _blue = new Color ("Blue");
    }

    [Test]
    public void GetValueInfos ()
    {
      var definition = CreateDefinition (_red, _green, _blue);
      var valueInstances = definition.GetValueInfos().Select(info => info.Value).ToArray();
      
      Assert.That (valueInstances, Is.EquivalentTo (new[] { _red, _green, _blue }));
    }

    [Test]
    public void GetValueInfos_CachesValues ()
    {
      var valueDiscoveryServiceMock = MockRepository.GenerateMock<IExtensibleEnumValueDiscoveryService> ();
      valueDiscoveryServiceMock
          .Expect (mock => mock.GetValues (Arg<ExtensibleEnumDefinition<Color>>.Is.Anything))
          .Return (new[] { _red })
          .Repeat.Once ();
      valueDiscoveryServiceMock.Replay ();

      var extensibleEnumDefinition = new ExtensibleEnumDefinition<Color> (valueDiscoveryServiceMock);
      var values1 = extensibleEnumDefinition.GetValueInfos ();
      var values2 = extensibleEnumDefinition.GetValueInfos ();

      valueDiscoveryServiceMock.VerifyAllExpectations ();
      Assert.That (values1, Is.SameAs (values2));
    }

    [Test]
    public void GetValueInfos_PassesExtensibleEnumDefinitionInstance_ToExtensibleEnumValueDiscoveryService ()
    {
      var valueDiscoveryServiceMock = MockRepository.GenerateMock<IExtensibleEnumValueDiscoveryService> ();
      valueDiscoveryServiceMock.Stub (mock => mock.GetValues (Arg<ExtensibleEnumDefinition<Color>>.Is.Anything)).Return (new[] { _red });

      var extensibleEnumDefinition = new ExtensibleEnumDefinition<Color> (valueDiscoveryServiceMock);
      extensibleEnumDefinition.GetValueInfos ();

      valueDiscoveryServiceMock.AssertWasCalled (mock => mock.GetValues (extensibleEnumDefinition));
    }

    [Test]
    public void GetValueInfos_DefaultOrder_IsAlphabetic ()
    {
      var definition = CreateDefinition (_red, _blue, _green);

      var values = definition.GetValueInfos ().Select (info => info.Value).ToArray ();

      Assert.That (values, Is.EqualTo (new[] { _blue, _green, _red }));
    }

    [Test]
    [ExpectedException (typeof (InvalidExtensibleEnumDefinitionException), ExpectedMessage = 
        "Extensible enum 'Remotion.UnitTests.ExtensibleEnums.TestDomain.Color' defines two values with ID 'Red'.")]
    public void GetValueInfos_DuplicateIDs ()
    {
      var definition = CreateDefinition (_red, _red);
      definition.GetValueInfos ();
    }

    [Test]
    [ExpectedException (typeof (InvalidExtensibleEnumDefinitionException), ExpectedMessage =
        "Extensible enum 'Remotion.UnitTests.ExtensibleEnums.TestDomain.Color' does not define any values.")]
    public void GetValueInfos_NoValues ()
    {
      var definition = CreateDefinition ();
      definition.GetValueInfos ();
    }

    [Test]
    public void GetValueInfoByID ()
    {
      var definition = CreateDefinition (_red, _green);
      var valueInfo = definition.GetValueInfoByID ("Red");

      Assert.That (valueInfo.Value, Is.EqualTo (_red));
    }

    [Test]
    [ExpectedException (typeof (KeyNotFoundException), 
        ExpectedMessage = "The extensible enum type 'Remotion.UnitTests.ExtensibleEnums.TestDomain.Color' does not define a value called '?'.")]
    public void GetValueInfoByID_WrongIDThrows ()
    {
      var definition = CreateDefinition (_red, _green);
      definition.GetValueInfoByID ("?");
    }

    [Test]
    [ExpectedException (typeof (InvalidExtensibleEnumDefinitionException), ExpectedMessage =
        "Extensible enum 'Remotion.UnitTests.ExtensibleEnums.TestDomain.Color' defines two values with ID 'Red'.")]
    public void GetValueInfoByID_DuplicateIDs ()
    {
      var definition = CreateDefinition (_red, _red);
      definition.GetValueInfoByID ("ID");
    }

    [Test]
    public void TryGetValueInfoByID ()
    {
      var definition = CreateDefinition (_red, _green);

      ExtensibleEnumInfo<Color> result;
      var success = definition.TryGetValueInfoByID ("Red", out result);

      var expected = Color.Values.Red ();
      Assert.That (success, Is.True);
      Assert.That (result.Value, Is.EqualTo (expected));
    }

    [Test]
    public void TryGetValueInfoByID_WrongID ()
    {
      var definition = CreateDefinition (_red, _green);

      ExtensibleEnumInfo<Color> result;
      var success = definition.TryGetValueInfoByID ("?", out result);

      Assert.That (success, Is.False);
      Assert.That (result, Is.Null);
    }

    [Test]
    public void GetValueInfos_NonGeneric ()
    {
      var definition = CreateDefinition (_red, _green);

      ReadOnlyCollection<IExtensibleEnumInfo> valueInfos = ((IExtensibleEnumDefinition) definition).GetValueInfos ();
      Assert.That (valueInfos, Is.EqualTo (definition.GetValueInfos ()));
    }

    [Test]
    public void GetValueInfoByID_NonGeneric ()
    {
      var definition = CreateDefinition (_red, _green);

      IExtensibleEnumInfo valueInfo = ((IExtensibleEnumDefinition) definition).GetValueInfoByID ("Red");
      Assert.That (valueInfo, Is.SameAs (definition.GetValueInfoByID ("Red")));
    }

    [Test]
    public void TryGetValueInfoByID_NonGeneric ()
    {
      var definition = CreateDefinition (_red, _green);

      IExtensibleEnumInfo valueInfo;
      bool success = ((IExtensibleEnumDefinition) definition).TryGetValueInfoByID ("Red", out valueInfo);
      
      ExtensibleEnumInfo<Color> expectedValueInfo;
      bool expectedSuccess = definition.TryGetValueInfoByID ("Red", out expectedValueInfo);

      Assert.That (success, Is.EqualTo (expectedSuccess));
      Assert.That (valueInfo, Is.SameAs (expectedValueInfo));
    }

    private ExtensibleEnumDefinition<Color> CreateDefinition (params Color[] colors)
    {
      var valueDiscoveryServiceStub = MockRepository.GenerateStub<IExtensibleEnumValueDiscoveryService> ();
      var definition = new ExtensibleEnumDefinition<Color> (valueDiscoveryServiceStub);

      valueDiscoveryServiceStub.Stub (stub => stub.GetValues (Arg<ExtensibleEnumDefinition<Color>>.Is.Anything)).Return (colors);
      return definition;
    }
  }
}
