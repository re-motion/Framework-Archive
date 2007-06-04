using System;
using System.Collections.Generic;
using System.Text;
using Mixins.UnitTests.SampleTypes;
using NUnit.Framework;
using Mixins.CodeGeneration;
using System.Reflection;
using Rubicon.Development.UnitTesting;

namespace Mixins.UnitTests.Mixins
{
  [TestFixture]
  public class ConcreteTypeBuilderTests : MixinTestBase
  {
    [Test]
    public void TypesAreCached()
    {
      Type t1 = TypeFactory.Current.GetConcreteType (typeof (BaseType1));
      Type t2 = TypeFactory.Current.GetConcreteType (typeof (BaseType1));
      Assert.AreSame (t1, t2);
    }

    [Test]
    public void CacheIsBoundToConcreteTypeBuilder ()
    {
      Type t1 = TypeFactory.Current.GetConcreteType (typeof (BaseType1));
      ConcreteTypeBuilder.SetCurrent (null);
      Type t2 = TypeFactory.Current.GetConcreteType (typeof (BaseType1));
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
      ClassOverridingMixinMethod c1 = ObjectFactory.Create<ClassOverridingMixinMethod>().With();
      ClassOverridingMixinMethod c2 = ObjectFactory.Create<ClassOverridingMixinMethod> ().With ();

      Assert.AreSame (Mixin.Get<AbstractMixin> (c1).GetType(), Mixin.Get<AbstractMixin> (c2).GetType());
    }

    [Test]
    public void MixinTypeCacheIsBoundToConcreteTypeBuilder ()
    {
      ClassOverridingMixinMethod c1 = ObjectFactory.Create<ClassOverridingMixinMethod> ().With ();
      ConcreteTypeBuilder.SetCurrent (null);
      ClassOverridingMixinMethod c2 = ObjectFactory.Create<ClassOverridingMixinMethod> ().With ();

      Assert.AreNotSame (Mixin.Get<AbstractMixin> (c1).GetType (), Mixin.Get<AbstractMixin> (c2).GetType ());
    }

    [Test]
    public void MixinTypeCacheEvenWorksForSerialization ()
    {
      ClassOverridingMixinMethod c1 = ObjectFactory.Create<ClassOverridingMixinMethod> ().With ();
      ClassOverridingMixinMethod c2 = ObjectFactory.Create<ClassOverridingMixinMethod> ().With ();

      Assert.AreSame (Mixin.Get<AbstractMixin> (c1).GetType (), Mixin.Get<AbstractMixin> (c2).GetType ());

      ClassOverridingMixinMethod[] array = Serializer.SerializeAndDeserialize (new ClassOverridingMixinMethod[] { c1, c2 });
      Assert.AreSame (Mixin.Get<AbstractMixin> (array[0]).GetType (), Mixin.Get<AbstractMixin> (array[1]).GetType ());
    }
  }
}
