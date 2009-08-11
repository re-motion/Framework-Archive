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
using System.Reflection;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Mixins;
using Remotion.Mixins.Definitions;
using Remotion.UnitTests.Mixins.SampleTypes;
using System.Linq;

namespace Remotion.UnitTests.Mixins.Definitions
{
  [TestFixture]
  public class MixinDefinitionBuilderTest
  {
    [Test]
    public void CorrectlyCopiesContext()
    {
      TargetClassDefinition targetClass = TargetClassDefinitionUtility.GetActiveConfiguration (typeof (BaseType1));
      Assert.IsNull (targetClass.Parent);
      Assert.AreEqual ("BaseType1", targetClass.Name);

      Assert.IsTrue (targetClass.Mixins.ContainsKey (typeof (BT1Mixin1)));
      Assert.IsTrue (targetClass.Mixins.ContainsKey (typeof (BT1Mixin2)));
      Assert.AreSame (targetClass, targetClass.Mixins[typeof (BT1Mixin1)].Parent);
    }

    [Test]
    public void MixinAppliedToInterface()
    {
      TargetClassDefinition targetClass = UnvalidatedDefinitionBuilder.BuildUnvalidatedDefinition (typeof (IBaseType2), typeof (BT2Mixin1));
      Assert.IsTrue (targetClass.IsInterface);

      MethodInfo method = typeof (IBaseType2).GetMethod ("IfcMethod");
      Assert.IsNotNull (method);

      MemberDefinitionBase member = targetClass.Methods[method];
      Assert.IsNotNull (member);

      MixinDefinition mixin = targetClass.Mixins[typeof (BT2Mixin1)];
      Assert.IsNotNull (mixin);
      Assert.AreSame (targetClass, mixin.TargetClass);
    }

    [Test]
    public void MixinIndicesCorrespondToPositionInArray()
    {
      TargetClassDefinition bt3 = TargetClassDefinitionUtility.GetActiveConfiguration (typeof (BaseType3));
      for (int i = 0; i < bt3.Mixins.Count; ++i)
        Assert.AreEqual (i, bt3.Mixins[i].MixinIndex);
    }

    [Test]
    public void OverriddenMixinMethod()
    {
      TargetClassDefinition overrider = TargetClassDefinitionUtility.GetActiveConfiguration (typeof (ClassOverridingMixinMembers));
      MixinDefinition mixin = overrider.Mixins[typeof (MixinWithAbstractMembers)];
      Assert.IsNotNull (mixin);
      Assert.IsTrue (mixin.HasOverriddenMembers());

      MethodDefinition method = mixin.Methods[typeof (MixinWithAbstractMembers).GetMethod ("AbstractMethod", BindingFlags.Instance | BindingFlags.NonPublic)];
      Assert.IsNotNull (method);
      MethodDefinition overridingMethod = overrider.Methods[typeof (ClassOverridingMixinMembers).GetMethod ("AbstractMethod")];
      Assert.IsNotNull (overridingMethod);
      Assert.AreSame (method, overridingMethod.Base);
      Assert.IsTrue (method.Overrides.ContainsKey (typeof (ClassOverridingMixinMembers)));
      Assert.AreSame (overridingMethod, method.Overrides[typeof (ClassOverridingMixinMembers)]);
    }

    [Test]
    public void NotOverriddenAbstractMixinMethodSucceeds()
    {
      TargetClassDefinition bt1 = UnvalidatedDefinitionBuilder.BuildUnvalidatedDefinition (typeof (BaseType1), typeof (MixinWithAbstractMembers));
      MixinDefinition mixin = bt1.Mixins[typeof (MixinWithAbstractMembers)];
      MethodDefinition method = mixin.Methods[typeof (MixinWithAbstractMembers).GetMethod ("AbstractMethod", BindingFlags.Instance | BindingFlags.NonPublic)];
      Assert.AreEqual (0, method.Overrides.Count);
    }

    [Test]
    [ExpectedException (typeof (ConfigurationException))]
    public void ThrowsOnMixinMethodOverridedWrongSig()
    {
      UnvalidatedDefinitionBuilder.BuildUnvalidatedDefinition (typeof (ClassOverridingMixinMethodWrongSig), typeof (MixinWithAbstractMembers));
    }

    [Test]
    [ExpectedException (typeof (ConfigurationException))]
    public void ThrowsOnMixinOverrideWithoutMixin()
    {
      UnvalidatedDefinitionBuilder.BuildUnvalidatedDefinition (typeof (ClassOverridingMixinMembers));
    }

    [Test]
    public void GenericMixinsAreAllowed()
    {
      Assert.IsTrue (UnvalidatedDefinitionBuilder.BuildUnvalidatedDefinition (typeof (BaseType3), typeof (BT3Mixin3<,>))
          .HasMixinWithConfiguredType(typeof(BT3Mixin3<,>)));
    }

    [Test]
    public void GenericMixinsAreClosed ()
    {
      MixinDefinition def = UnvalidatedDefinitionBuilder.BuildUnvalidatedDefinition (typeof (BaseType3), typeof (BT3Mixin3<,>))
          .GetMixinByConfiguredType(typeof (BT3Mixin3<,>));
      Assert.IsFalse (def.Type.IsGenericTypeDefinition);
    }

    [Test]
    public void GenericMixinsAreSetToConstraintOrBaseType ()
    {
      MixinDefinition def = UnvalidatedDefinitionBuilder.BuildUnvalidatedDefinition (typeof (BaseType3), typeof (BT3Mixin3<,>))
          .GetMixinByConfiguredType (typeof (BT3Mixin3<,>));
      Assert.AreEqual (typeof (BaseType3), def.Type.GetGenericArguments()[0]);
      Assert.AreEqual (typeof (IBaseType33), def.Type.GetGenericArguments()[1]);
    }

    private class MixinIntroducingGenericInterfaceWithTargetAsThisType<T>: Mixin<T>, IEquatable<T>
        where T: class
    {
      public bool Equals (T other)
      {
        throw new NotImplementedException();
      }
    }

    [Test]
    public void GenericInterfaceArgumentIsBaseTypeWhenPossible()
    {
      TargetClassDefinition def = UnvalidatedDefinitionBuilder.BuildUnvalidatedDefinition (typeof (BaseType1),
          typeof (MixinIntroducingGenericInterfaceWithTargetAsThisType<>));
      Assert.IsTrue (def.ReceivedInterfaces.ContainsKey (typeof (IEquatable<BaseType1>)));
    }

    [Test]
    public void ExplicitMixinDependenciesCorrectlyCopied ()
    {
      TargetClassDefinition targetClass = TargetClassDefinitionUtility.GetActiveConfiguration (typeof (TargetClassWithAdditionalDependencies));
      Assert.IsTrue (targetClass.RequiredMixinTypes.ContainsKey (typeof (MixinWithNoAdditionalDependency)));
      Assert.IsTrue (targetClass.Mixins[typeof (MixinWithAdditionalClassDependency)].MixinDependencies.ContainsKey (typeof (MixinWithNoAdditionalDependency)));
    }

    public class MixinWithDependency : Mixin<object, IMixinBeingDependedUpon>
    {
    }

    public interface IMixinBeingDependedUpon { }

    public class MixinBeingDependedUpon : IMixinBeingDependedUpon
    {
    }

    [Uses (typeof (MixinWithDependency), AdditionalDependencies = new Type[] { typeof (IMixinBeingDependedUpon) })]
    [Uses (typeof (MixinBeingDependedUpon))]
    public class MixinTargetWithExplicitDependencies { }

    [Test]
    public void DuplicateDependenciesDontMatter ()
    {
      TargetClassDefinition mt = TargetClassDefinitionUtility.GetActiveConfiguration (typeof (MixinTargetWithExplicitDependencies));
      Assert.IsTrue (mt.RequiredBaseCallTypes.ContainsKey (typeof (IMixinBeingDependedUpon)));
      Assert.IsTrue (mt.Mixins[typeof (MixinWithDependency)].BaseDependencies.ContainsKey (typeof (IMixinBeingDependedUpon)));
      Assert.IsTrue (mt.Mixins[typeof (MixinWithDependency)].MixinDependencies.ContainsKey (typeof (IMixinBeingDependedUpon)));

      // no exceptions occurred while ordering
    }

    [Test]
    public void HasOverriddenMembersTrue ()
    {
      TargetClassDefinition definition =
          UnvalidatedDefinitionBuilder.BuildUnvalidatedDefinition (typeof (ClassOverridingMixinMembers), typeof (MixinWithAbstractMembers));
      Assert.IsTrue (definition.Mixins[0].HasOverriddenMembers ());
    }

    [Test]
    public void HasOverriddenMembersFalse ()
    {
      TargetClassDefinition definition = TargetClassDefinitionUtility.GetActiveConfiguration (typeof (BaseType1));
      Assert.IsFalse (definition.Mixins[0].HasOverriddenMembers ());
    }

    [Test]
    public void GetProtectedOverriders ()
    {
      const BindingFlags bf = BindingFlags.NonPublic | BindingFlags.Instance;

      TargetClassDefinition bt1 = UnvalidatedDefinitionBuilder.BuildUnvalidatedDefinition (typeof (BaseType1), typeof (MixinWithProtectedOverrider));
      var overriders = bt1.Mixins[0].GetProtectedOverriders ();
      Assert.That (overriders.Select (m => m.MethodInfo).ToArray (), Is.EquivalentTo (new[] {
          typeof (MixinWithProtectedOverrider).GetMethod ("VirtualMethod", bf),
          typeof (MixinWithProtectedOverrider).GetMethod ("get_VirtualProperty", bf),
          typeof (MixinWithProtectedOverrider).GetMethod ("add_VirtualEvent", bf),
          typeof (MixinWithProtectedOverrider).GetMethod ("remove_VirtualEvent", bf),
          }));
    }

    [Test]
    public void HasProtectedOverridersTrue ()
    {
      TargetClassDefinition bt1 = UnvalidatedDefinitionBuilder.BuildUnvalidatedDefinition (typeof (BaseType1), typeof (MixinWithProtectedOverrider));
      Assert.IsFalse (bt1.HasProtectedOverriders ());
      Assert.IsTrue (bt1.Mixins[0].HasProtectedOverriders ());
    }

    [Test]
    public void HasProtectedOverridersFalse ()
    {
      TargetClassDefinition bt1 = TargetClassDefinitionUtility.GetActiveConfiguration (typeof (BaseType1));
      Assert.IsFalse (bt1.HasProtectedOverriders ());
      Assert.IsFalse (bt1.Mixins[0].HasProtectedOverriders ());
    }

    [Test]
    public void AcceptsAlphabeticOrdering_True ()
    {
      using (MixinConfiguration.BuildNew ()
          .ForClass<ClassWithMixinsAcceptingAlphabeticOrdering> ().AddMixin<MixinAcceptingAlphabeticOrdering1> ()
          .EnterScope ())
      {
        MixinDefinition accepter = TargetClassDefinitionUtility.GetActiveConfiguration (typeof (ClassWithMixinsAcceptingAlphabeticOrdering)).Mixins[typeof (MixinAcceptingAlphabeticOrdering1)];
        Assert.IsTrue (accepter.AcceptsAlphabeticOrdering);
      }
    }

    [Test]
    public void AcceptsAlphabeticOrdering_False ()
    {
      using (MixinConfiguration.BuildNew ().ForClass<NullTarget> ().AddMixin<NullMixin> ().EnterScope ())
      {
        MixinDefinition accepter = TargetClassDefinitionUtility.GetActiveConfiguration (typeof (NullTarget)).Mixins[typeof (NullMixin)];
        Assert.IsFalse (accepter.AcceptsAlphabeticOrdering);
      }
    }

    [Test]
    public void MixinKind_Extending ()
    {
      using (MixinConfiguration.BuildNew ().ForClass<NullTarget> ().AddMixin<NullMixin> ().OfKind (MixinKind.Extending).EnterScope ())
      {
        MixinDefinition mixin = TargetClassDefinitionUtility.GetActiveConfiguration (typeof (NullTarget)).Mixins[typeof (NullMixin)];
        Assert.That (mixin.MixinKind, Is.EqualTo (MixinKind.Extending));
      }
    }

    [Test]
    public void MixinKind_Used ()
    {
      using (MixinConfiguration.BuildNew ().ForClass<NullTarget> ().AddMixin<NullMixin> ().OfKind (MixinKind.Used).EnterScope ())
      {
        MixinDefinition mixin = TargetClassDefinitionUtility.GetActiveConfiguration (typeof (NullTarget)).Mixins[typeof (NullMixin)];
        Assert.That (mixin.MixinKind, Is.EqualTo (MixinKind.Used));
      }
    }
  }
}
