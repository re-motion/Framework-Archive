using System;
using System.Collections.Generic;
using System.Text;
using Rubicon.Mixins.UnitTests.SampleTypes;
using NUnit.Framework;
using Rubicon.Mixins.CodeGeneration;
using System.Reflection;
using Rubicon.Development.UnitTesting;
using System.IO;
using System.Threading;

namespace Rubicon.Mixins.UnitTests.Mixins
{
  [TestFixture]
  public class ConcreteTypeBuilderTests : MixinTestBase
  {
    [Test]
    public void TypesAreCached()
    {
      Type t1 = TypeFactory.GetConcreteType (typeof (BaseType1));
      Type t2 = TypeFactory.GetConcreteType (typeof (BaseType1));
      Assert.AreSame (t1, t2);
    }

    [Test]
    public void CacheIsBoundToConcreteTypeBuilder ()
    {
      Type t1 = TypeFactory.GetConcreteType (typeof (BaseType1));
      ConcreteTypeBuilder.SetCurrent (null);
      Type t2 = TypeFactory.GetConcreteType (typeof (BaseType1));
      Assert.AreNotSame (t1, t2);
    }

    [Test]
    public void CacheEvenWorksForSerialization ()
    {
      BaseType1 bt1 = ObjectFactory.Create<BaseType1>().With();
      BaseType1 bt2 = ObjectFactory.Create<BaseType1> ().With ();

      Assert.AreSame (bt1.GetType(), bt2.GetType());

      BaseType1[] array = Serializer.SerializeAndDeserialize (new BaseType1[] {bt1, bt2});
      Assert.AreSame (bt1.GetType(), array[0].GetType());
      Assert.AreSame (array[0].GetType(), array[1].GetType());
    }

    [Test]
    public void GeneratedMixinTypesAreCached()
    {
      ClassOverridingMixinMembers c1 = ObjectFactory.Create<ClassOverridingMixinMembers>().With();
      ClassOverridingMixinMembers c2 = ObjectFactory.Create<ClassOverridingMixinMembers> ().With ();

      Assert.AreSame (Mixin.Get<MixinWithAbstractMembers> (c1).GetType(), Mixin.Get<MixinWithAbstractMembers> (c2).GetType());
    }

    [Test]
    public void MixinTypeCacheIsBoundToConcreteTypeBuilder ()
    {
      ClassOverridingMixinMembers c1 = ObjectFactory.Create<ClassOverridingMixinMembers> ().With ();
      ConcreteTypeBuilder.SetCurrent (null);
      ClassOverridingMixinMembers c2 = ObjectFactory.Create<ClassOverridingMixinMembers> ().With ();

      Assert.AreNotSame (Mixin.Get<MixinWithAbstractMembers> (c1).GetType (), Mixin.Get<MixinWithAbstractMembers> (c2).GetType ());
    }

    [Test]
    public void MixinTypeCacheEvenWorksForSerialization ()
    {
      ClassOverridingMixinMembers c1 = ObjectFactory.Create<ClassOverridingMixinMembers> ().With ();
      ClassOverridingMixinMembers c2 = ObjectFactory.Create<ClassOverridingMixinMembers> ().With ();

      Assert.AreSame (Mixin.Get<MixinWithAbstractMembers> (c1).GetType (), Mixin.Get<MixinWithAbstractMembers> (c2).GetType ());

      ClassOverridingMixinMembers[] array = Serializer.SerializeAndDeserialize (new ClassOverridingMixinMembers[] { c1, c2 });
      Assert.AreSame (Mixin.Get<MixinWithAbstractMembers> (array[0]).GetType (), Mixin.Get<MixinWithAbstractMembers> (array[1]).GetType ());
    }

    [Test]
    public void CurrentIsGlobalSingleton ()
    {
      ConcreteTypeBuilder newBuilder = new ConcreteTypeBuilder ();
      Assert.IsFalse (ConcreteTypeBuilder.HasCurrent);
      Thread setterThread = new Thread ((ThreadStart) delegate { ConcreteTypeBuilder.SetCurrent (newBuilder); });
      setterThread.Start ();
      setterThread.Join ();

      Assert.IsTrue (ConcreteTypeBuilder.HasCurrent);
      Assert.AreSame (newBuilder, ConcreteTypeBuilder.Current);
    }
  }
}
