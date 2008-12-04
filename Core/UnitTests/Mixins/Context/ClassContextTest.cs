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
using System.Collections.Generic;
using System.Reflection;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Development.UnitTesting;
using Remotion.Mixins;
using Remotion.Mixins.Context;
using Remotion.Mixins.Context.FluentBuilders;
using Remotion.Mixins.Context.Serialization;
using Remotion.UnitTests.Mixins.SampleTypes;
using Rhino.Mocks;

namespace Remotion.UnitTests.Mixins.Context
{
  [TestFixture]
  public class ClassContextTest
  {
    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage = "Object was tried to be added twice", MatchType = MessageMatch.Contains)]
    public void ConstructorThrowsOnDuplicateMixinContexts ()
    {
      new ClassContext (typeof (string), typeof (object), typeof (object));
    }

    [Test]
    public void ConstructorWithMixinParameters()
    {
      var context = new ClassContext (typeof (BaseType1), typeof (BT1Mixin1), typeof (BT1Mixin2));
      Assert.AreEqual (2, context.Mixins.Count);
      Assert.IsTrue (context.Mixins.ContainsKey (typeof (BT1Mixin1)));
      Assert.IsTrue (context.Mixins.ContainsKey (typeof (BT1Mixin2)));
      Assert.IsFalse (context.Mixins.ContainsKey (typeof (BT2Mixin1)));
    }

    [Test]
    public void ConstructorWithMixinParameters_DefaultValues ()
    {
      var context = new ClassContext (typeof (BaseType1));
      Assert.That (context.Mixins, Is.Empty);
    }

    [Test]
    public void Mixins ()
    {
      var classContext = new ClassContext (typeof (BaseType7));

      Assert.IsFalse (classContext.Mixins.ContainsKey (typeof (BT7Mixin1)));
      MixinContext mixinContext = classContext.Mixins[typeof (BT7Mixin1)];
      Assert.IsNull (mixinContext);

      classContext = new ClassContext (typeof (BaseType7), typeof (BT7Mixin1));
      Assert.IsTrue (classContext.Mixins.ContainsKey (typeof (BT7Mixin1)));
      mixinContext = classContext.Mixins[typeof (BT7Mixin1)];
      Assert.AreSame (mixinContext, classContext.Mixins[typeof (BT7Mixin1)]);
    }

    [Test]
    public void CompleteInterfaces_Empty()
    {
      var context = new ClassContext (typeof (BaseType5), new MixinContext[0], new Type[0]);
      Assert.AreEqual (0, context.CompleteInterfaces.Count);
      Assert.IsEmpty (context.CompleteInterfaces);
      Assert.IsFalse (context.CompleteInterfaces.ContainsKey (typeof (IBT5MixinC1)));
    }

    [Test]
    public void CompleteInterfaces_NonEmpty ()
    {
      var context = new ClassContext (typeof (BaseType5), new MixinContext[0], new[] { typeof (IBT5MixinC1) });
      Assert.AreEqual (1, context.CompleteInterfaces.Count);
      Assert.Contains (typeof (IBT5MixinC1), context.CompleteInterfaces);
      Assert.IsTrue (context.CompleteInterfaces.ContainsKey (typeof (IBT5MixinC1)));
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage = "The mixin type System.Object was tried to be added twice.\r\n"
                                                                      + "Parameter name: mixinTypes")]
    public void ConstructorThrows_OnDuplicateMixinTypes ()
    {
      new ClassContext (typeof (string), typeof (object), typeof (object));
    }

    [Test]
    public void DuplicateCompleteInterfacesAreIgnored ()
    {
      var context = new ClassContext (typeof (BaseType5), new MixinContext[0], new[] { typeof (IBT5MixinC1), typeof (IBT5MixinC1) });
      Assert.AreEqual (1, context.CompleteInterfaces.Count);
    }

    [Test]
    public void SpecializeWithTypeArguments ()
    {
      ClassContext original = new ClassContextBuilder (typeof (List<>)).AddMixin<BT1Mixin1>().WithDependency<IBaseType2>().BuildClassContext();

      ClassContext specialized = original.SpecializeWithTypeArguments (new[] { typeof (int) });
      Assert.IsNotNull (specialized);
      Assert.AreEqual (typeof (List<int>), specialized.Type);
      Assert.IsTrue (specialized.Mixins.ContainsKey (typeof (BT1Mixin1)));
      Assert.IsTrue (specialized.Mixins[typeof (BT1Mixin1)].ExplicitDependencies.ContainsKey (typeof (IBaseType2)));
    }

    [Test]
    public void GenericTypesNotTransparentlyConvertedToTypeDefinitions ()
    {
      var context = new ClassContext (typeof (List<int>));
      Assert.AreEqual (typeof (List<int>), context.Type);
    }

    [Test]
    public void ContainsAssignableMixin ()
    {
      var context = new ClassContext (typeof (object), typeof (IList<int>));

      Assert.IsTrue (context.Mixins.ContainsKey (typeof (IList<int>)));
      Assert.IsTrue (context.Mixins.ContainsAssignableMixin (typeof (IList<int>)));

      Assert.IsFalse (context.Mixins.ContainsKey (typeof (ICollection<int>)));
      Assert.IsTrue (context.Mixins.ContainsAssignableMixin (typeof (ICollection<int>)));

      Assert.IsFalse (context.Mixins.ContainsKey (typeof (object)));
      Assert.IsTrue (context.Mixins.ContainsAssignableMixin (typeof (object)));

      Assert.IsFalse (context.Mixins.ContainsKey (typeof (List<int>)));
      Assert.IsFalse (context.Mixins.ContainsAssignableMixin (typeof (List<int>)));

      Assert.IsFalse (context.Mixins.ContainsKey (typeof (IList<>)));
      Assert.IsFalse (context.Mixins.ContainsAssignableMixin (typeof (List<>)));
    }

    [Test]
    public void CloneForSpecificType ()
    {
      var mixins = new[] {
                             new MixinContext (MixinKind.Extending, typeof (BT1Mixin1), MemberVisibility.Private, new[] { typeof (IBT1Mixin1) }),
                             new MixinContext (MixinKind.Extending, typeof (BT1Mixin2), MemberVisibility.Private)
                         };
      var interfaces = new[] { typeof (ICBT6Mixin1), typeof (ICBT6Mixin2)};
      var source = new ClassContext (typeof (BaseType1), mixins, interfaces);
      ClassContext clone = source.CloneForSpecificType (typeof (BaseType2));
      Assert.AreNotEqual (source, clone);
      Assert.That(clone.Mixins, Is.EquivalentTo(mixins));
      Assert.That (clone.CompleteInterfaces, Is.EquivalentTo (interfaces));
      Assert.AreEqual (typeof (BaseType2), clone.Type);
      Assert.AreEqual (typeof (BaseType1), source.Type);
    }

    [Test]
    public void Equals_True ()
    {
      var c1 =
          new ClassContext (
              typeof (BaseType1),
              new[] {new MixinContext (MixinKind.Extending, typeof (BT1Mixin1), MemberVisibility.Private), new MixinContext (MixinKind.Extending, typeof (BT1Mixin2), MemberVisibility.Private)},
              new[] {typeof (IBT5MixinC1), typeof (IBT5MixinC2)});
      var c2 =
          new ClassContext (
              typeof (BaseType1),
              new[] { new MixinContext (MixinKind.Extending, typeof (BT1Mixin1), MemberVisibility.Private), new MixinContext (MixinKind.Extending, typeof (BT1Mixin2), MemberVisibility.Private) },
              new[] { typeof (IBT5MixinC1), typeof (IBT5MixinC2) });
      var c3 =
          new ClassContext (
              typeof (BaseType1),
              new[] { new MixinContext (MixinKind.Extending, typeof (BT1Mixin2), MemberVisibility.Private), new MixinContext (MixinKind.Extending, typeof (BT1Mixin1), MemberVisibility.Private) },
              new[] { typeof (IBT5MixinC1), typeof (IBT5MixinC2) });
      var c4 =
          new ClassContext (
              typeof (BaseType1),
              new[] {new MixinContext (MixinKind.Extending, typeof (BT1Mixin1), MemberVisibility.Private), new MixinContext (MixinKind.Extending, typeof (BT1Mixin2), MemberVisibility.Private)},
              new[] { typeof (IBT5MixinC2), typeof (IBT5MixinC1) });

      Assert.AreEqual (c1, c2);
      Assert.AreEqual (c1, c3);
      Assert.AreEqual (c1, c4);

      var c5 = new ClassContext (typeof (BaseType1));
      var c6 = new ClassContext (typeof (BaseType1));
      Assert.AreEqual (c5, c6);
    }

    [Test]
    public void Equals_False_ClassType ()
    {
      var c1 = new ClassContext (typeof (BaseType1));
      var c2 = new ClassContext (typeof (BaseType2));

      Assert.AreNotEqual (c1, c2);
    }

    [Test]
    public void Equals_False_Mixins ()
    {
      var c1 = new ClassContext (typeof (BaseType1), typeof (BT1Mixin1));
      var c3 = new ClassContext (typeof (BaseType1), typeof (BT1Mixin2));
      var c4 = new ClassContext (typeof (BaseType1), typeof (BT1Mixin1), typeof (BT1Mixin2));

      Assert.AreNotEqual (c1, c3);
      Assert.AreNotEqual (c1, c4);
      Assert.AreNotEqual (c3, c4);
    }

    [Test]
    public void Equals_False_CompleteInterfaces ()
    {
      var c1 =
          new ClassContext (
              typeof (BaseType1),
              new MixinContext[0],
              new[] { typeof (IBT5MixinC1) });
      var c2 =
          new ClassContext (
              typeof (BaseType1),
              new MixinContext[0],
              new[] { typeof (IBT5MixinC2) });
      var c3 =
          new ClassContext (
              typeof (BaseType1),
              new MixinContext[0],
              new[] { typeof (IBT5MixinC1), typeof (IBT5MixinC2) });

      Assert.AreNotEqual (c1, c2);
      Assert.AreNotEqual (c1, c3);
      Assert.AreNotEqual (c2, c3);
    }

    [Test]
    public void GetHashCode_Equal ()
    {
      var c1 =
          new ClassContext (
              typeof (BaseType1),
              new[] { new MixinContext (MixinKind.Extending, typeof (BT1Mixin1), MemberVisibility.Private), new MixinContext (MixinKind.Extending, typeof (BT1Mixin2), MemberVisibility.Private) },
              new[] { typeof (IBT5MixinC1), typeof (IBT5MixinC2) });
      var c2 =
          new ClassContext (
              typeof (BaseType1),
              new[] { new MixinContext (MixinKind.Extending, typeof (BT1Mixin1), MemberVisibility.Private), new MixinContext (MixinKind.Extending, typeof (BT1Mixin2), MemberVisibility.Private) },
              new[] { typeof (IBT5MixinC1), typeof (IBT5MixinC2) });
      var c3 =
          new ClassContext (
              typeof (BaseType1),
              new[] { new MixinContext (MixinKind.Extending, typeof (BT1Mixin2), MemberVisibility.Private), new MixinContext (MixinKind.Extending, typeof (BT1Mixin1), MemberVisibility.Private) },
              new[] { typeof (IBT5MixinC1), typeof (IBT5MixinC2) });
      var c4 =
          new ClassContext (
              typeof (BaseType1),
              new[] { new MixinContext (MixinKind.Extending, typeof (BT1Mixin1), MemberVisibility.Private), new MixinContext (MixinKind.Extending, typeof (BT1Mixin2), MemberVisibility.Private) },
              new[] { typeof (IBT5MixinC2), typeof (IBT5MixinC1) });

      Assert.AreEqual (c1.GetHashCode (), c2.GetHashCode ());
      Assert.AreEqual (c1.GetHashCode (), c3.GetHashCode ());
      Assert.AreEqual (c1.GetHashCode (), c4.GetHashCode ());

      var c5 = new ClassContext (typeof (BaseType1));
      var c6 = new ClassContext (typeof (BaseType1));
      Assert.AreEqual (c5.GetHashCode (), c6.GetHashCode ());
    }

    [Test]
    public void Serialize()
    {
      var serializer = MockRepository.GenerateMock<IClassContextSerializer> ();
      var context = new ClassContext (typeof (BaseType1), new[] { new MixinContext (MixinKind.Used, typeof (BT1Mixin1), MemberVisibility.Public) }, new[] { typeof (int), typeof (string) });
      context.Serialize (serializer);

      serializer.AssertWasCalled (mock => mock.AddClassType (context.Type));
      serializer.AssertWasCalled (mock => mock.AddMixins (context.Mixins));
      serializer.AssertWasCalled (mock => mock.AddCompleteInterfaces (context.CompleteInterfaces));
    }

    [Test]
    public void Deserialize ()
    {
      var expectedContext = new ClassContext (typeof (BaseType1), new[] { new MixinContext (MixinKind.Used, typeof (BT1Mixin1), MemberVisibility.Public) }, new[] { typeof (int), typeof (string) });

      var deserializer = MockRepository.GenerateMock<IClassContextDeserializer> ();
      deserializer.Expect (mock => mock.GetClassType ()).Return (expectedContext.Type);
      deserializer.Expect (mock => mock.GetMixins ()).Return (expectedContext.Mixins);
      deserializer.Expect (mock => mock.GetCompleteInterfaces ()).Return (expectedContext.CompleteInterfaces);

      var context = ClassContext.Deserialize (deserializer);

      Assert.That (context, Is.EqualTo (expectedContext));
    }

    [Test]
    public void Serialization_IsUpToDate ()
    {
      var properties = typeof (ClassContext).GetProperties (BindingFlags.Public | BindingFlags.Instance);
      Assert.That (typeof (IClassContextSerializer).GetMethods ().Length, Is.EqualTo (properties.Length));
      Assert.That (typeof (IClassContextDeserializer).GetMethods ().Length, Is.EqualTo (properties.Length));
    }
  }
}