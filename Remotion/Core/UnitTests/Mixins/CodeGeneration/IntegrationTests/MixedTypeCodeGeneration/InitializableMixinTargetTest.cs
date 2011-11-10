// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) rubicon IT GmbH, www.rubicon.eu
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
using NUnit.Framework;
using Remotion.Development.UnitTesting;
using Remotion.Mixins;
using Remotion.Mixins.CodeGeneration.DynamicProxy;
using Remotion.Mixins.Utilities;
using Remotion.UnitTests.Mixins.CodeGeneration.TestDomain;
using Remotion.Reflection;
using System.Linq;
using Remotion.UnitTests.Mixins.TestDomain;

namespace Remotion.UnitTests.Mixins.CodeGeneration.IntegrationTests.MixedTypeCodeGeneration
{
  [TestFixture]
  public class InitializableMixinTargetTest : CodeGenerationBaseTest
  {
    [Test]
    public void GeneratedTypeImplementsIInitializableMixinTarget ()
    {
      Type concreteType = CreateMixedType (typeof (BaseType1), typeof (NullMixin));
      Assert.That (typeof (IInitializableMixinTarget).IsAssignableFrom (concreteType));
    }

    [Test]
    public void Initialize_SetsFirstProxy ()
    {
      var instance = (IInitializableMixinTarget) CreateMixedObject<BaseType1> (typeof (NullMixin));

      var oldProxy = instance.FirstNextCallProxy;
      instance.Initialize ();

      Assert.That (instance.FirstNextCallProxy, Is.Not.SameAs (oldProxy));
      Assert.That (PrivateInvoke.GetPublicField (instance.FirstNextCallProxy, "__depth"), Is.EqualTo (0));
    }

    [Test]
    public void Initialize_CreatesMixins ()
    {
      var instance = (IInitializableMixinTarget) CreateMixedObject<BaseType1> (typeof (NullMixin));

      var oldMixins = instance.Mixins;
      instance.Initialize ();
      
      Assert.That (instance.Mixins, Is.Not.SameAs (oldMixins));
      Assert.That (instance.Mixins.Length, Is.EqualTo (1));
      Assert.That (instance.Mixins[0], Is.InstanceOf(typeof (NullMixin)));
    }

    [Test]
    public void Initialize_InitializesMixins ()
    {
      var instance = (IInitializableMixinTarget) CreateMixedObject<NullTarget> (typeof (MixinWithOnInitializedAndOnDeserialized));
      ((MixinWithOnInitializedAndOnDeserialized) instance.Mixins[0]).OnInitializedCalled = false;

      instance.Initialize ();

      Assert.That (((MixinWithOnInitializedAndOnDeserialized) instance.Mixins[0]).OnInitializedCalled, Is.True);
    }

    [Test]
    public void Initialize_InitializesMixins_WithNextCallProxies ()
    {
      var instance = (IInitializableMixinTarget) ObjectFactory.Create<BaseType7> (ParamList.Empty);
      instance.Initialize ();

      Assert.That (GetDepthValue (instance.Mixins[0]), Is.EqualTo (1));
      Assert.That (GetDepthValue (instance.Mixins[1]), Is.EqualTo (2));
      Assert.That (GetDepthValue (instance.Mixins[2]), Is.EqualTo (3));
      Assert.That (GetDepthValue (instance.Mixins[3]), Is.EqualTo (4));
      Assert.That (GetDepthValue (instance.Mixins[4]), Is.EqualTo (5));
      Assert.That (GetDepthValue (instance.Mixins[5]), Is.EqualTo (6));
      Assert.That (GetDepthValue (instance.Mixins[6]), Is.EqualTo (7));
    }

    [Test]
    public void InitializeAfterDeserialization_SetsFirstProxy ()
    {
      var instance = (IInitializableMixinTarget) CreateMixedObject<BaseType1> (typeof (NullMixin));

      var oldProxy = instance.FirstNextCallProxy;
      instance.InitializeAfterDeserialization (new object[] { new NullMixin() });

      Assert.That (instance.FirstNextCallProxy, Is.Not.SameAs (oldProxy));
      Assert.That (PrivateInvoke.GetPublicField (instance.FirstNextCallProxy, "__depth"), Is.EqualTo (0));
    }

    [Test]
    public void InitializeAfterDeserialization_UsesGivenMixins ()
    {
      var instance = (IInitializableMixinTarget) CreateMixedObject<BaseType1> (typeof (NullMixin));

      var mixins = new object[] { new NullMixin () };
      instance.InitializeAfterDeserialization (mixins);

      Assert.That (instance.Mixins, Is.SameAs (mixins));
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException))]
    public void InitializeAfterDeserialization_ChecksMixins ()
    {
      var instance = (IInitializableMixinTarget) CreateMixedObject<NullTarget> (typeof (NullMixin));

      var mixins = new object[] { };
      instance.InitializeAfterDeserialization (mixins);
    }
    
    [Test]
    public void InitializeAfterDeserialization_InitializesMixins ()
    {
      var instance = (IInitializableMixinTarget) CreateMixedObject<NullTarget> (typeof (MixinWithOnInitializedAndOnDeserialized));

      var mixins = new object[] { new MixinWithOnInitializedAndOnDeserialized() };
      instance.InitializeAfterDeserialization (mixins);

      Assert.That (((MixinWithOnInitializedAndOnDeserialized) instance.Mixins[0]).OnDeserializedCalled, Is.True);
    }

    [Test]
    public void InitializeAfterDeserialization_InitializesMixins_WithNextCallProxies ()
    {
      var instance = (IInitializableMixinTarget) ObjectFactory.Create<BaseType7> (ParamList.Empty);

      // create new copies of the mixins without initializing them
      var mixins = instance.Mixins.Select (m => Activator.CreateInstance (m.GetType ())).ToArray ();

      instance.InitializeAfterDeserialization (mixins);

      Assert.That (GetDepthValue (instance.Mixins[0]), Is.EqualTo (1));
      Assert.That (GetDepthValue (instance.Mixins[1]), Is.EqualTo (2));
      Assert.That (GetDepthValue (instance.Mixins[2]), Is.EqualTo (3));
      Assert.That (GetDepthValue (instance.Mixins[3]), Is.EqualTo (4));
      Assert.That (GetDepthValue (instance.Mixins[4]), Is.EqualTo (5));
      Assert.That (GetDepthValue (instance.Mixins[5]), Is.EqualTo (6));
      Assert.That (GetDepthValue (instance.Mixins[6]), Is.EqualTo (7));
    }

    private object GetDepthValue (object mixin)
    {
      var nextProperty = MixinReflector.GetNextProperty (mixin.GetType ());
      var baseValue = nextProperty.GetValue (mixin, null);
      return PrivateInvoke.GetPublicField (baseValue, "__depth");
    }
  }
}
