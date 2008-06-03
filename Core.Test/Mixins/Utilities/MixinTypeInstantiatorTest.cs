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
using NUnit.Framework;
using Remotion.UnitTests.Mixins.SampleTypes;
using Remotion.Mixins;
using Remotion.Mixins.Utilities;

namespace Remotion.UnitTests.Mixins.Utilities
{
  [TestFixture]
  public class MixinTypeInstantiatorTest
  {
    [Test]
    public void InstantiateNonGenericMixin()
    {
      MixinTypeInstantiator instantiator = new MixinTypeInstantiator (typeof (BaseType3));
      Type t = instantiator.GetClosedMixinType (typeof (object));
      Assert.AreEqual (typeof (object), t);
    }

    [Test]
    public void InstantiateGenericMixinSimpleConstraints ()
    {
      MixinTypeInstantiator instantiator = new MixinTypeInstantiator (typeof (BaseType3));
      Type t = instantiator.GetClosedMixinType (typeof (BT3Mixin6<,>));
      Assert.AreEqual (typeof (BT3Mixin6<IBT3Mixin6ThisDependencies, IBT3Mixin6BaseDependencies>), t);
    }

    [Test]
    public void InstantiateGenericMixinWithBaseType ()
    {
      MixinTypeInstantiator instantiator = new MixinTypeInstantiator (typeof (BaseType3));
      Type t = instantiator.GetClosedMixinType (typeof (BT3Mixin3<,>));
      Assert.AreEqual (typeof (BT3Mixin3<BaseType3, IBaseType33>), t);
    }

    [Test]
    [ExpectedException (typeof (ConfigurationException), ExpectedMessage = "incompatible constraints", MatchType = MessageMatch.Contains)]
    public void MixinWithUnsatisfiableGenericConstraintsThrows ()
    {
      MixinTypeInstantiator instantiator = new MixinTypeInstantiator (typeof (BaseType3));
      instantiator.GetClosedMixinType (typeof (GenericTypeInstantiatorTest.IncompatibleConstraints1<>));
    }

    class MixinIntroducingGenericInterfaceWithUnclearThisType<T> : Mixin<T>, IEquatable<T>
    where T : class, ICloneable
    {
      public bool Equals (T other)
      {
        throw new NotImplementedException ();
      }
    }

    [Test]
    [ExpectedException (typeof (ConfigurationException), ExpectedMessage = "is bound to the mixin's This parameter. That's is not allowed",
      MatchType = MessageMatch.Contains)]
    public void ThrowsIfGenericInterfaceArgumentMightBeUnexpected ()
    {
      MixinTypeInstantiator instantiator = new MixinTypeInstantiator (typeof (BaseType1));
      instantiator.GetClosedMixinType (typeof (MixinIntroducingGenericInterfaceWithUnclearThisType<>));
    }

    class MixinIntroducingNonGenericInterface<T> : Mixin<T>, IEquatable<BaseType1>
      where T : class, ICloneable
    {
      public bool Equals (BaseType1 other)
      {
        throw new NotImplementedException ();
      }
    }

    [Test]
    public void ConstraintsAllowedWhenNoGenericInterfaceBoundToThem ()
    {
      MixinTypeInstantiator instantiator = new MixinTypeInstantiator (typeof (BaseType1));
      Type t = instantiator.GetClosedMixinType (typeof (MixinIntroducingNonGenericInterface<>));
      Assert.IsTrue (typeof (IEquatable<BaseType1>).IsAssignableFrom(t));
      Assert.AreEqual (typeof (ICloneable), t.GetGenericArguments ()[0]);
    }
  }
}
