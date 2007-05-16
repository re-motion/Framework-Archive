using System;
using System.Collections.Generic;
using System.Text;
using Mixins.UnitTests.SampleTypes;
using NUnit.Framework;
using Mixins.CodeGeneration;
using System.Reflection;

namespace Mixins.UnitTests.Mixins
{
  [TestFixture]
  public class CacheTests : MixinTestBase
  {
    [Test]
    public void TypesAreCached()
    {
      Type t1 = TypeFactory.Current.GetConcreteType (typeof (BaseType1));
      Type t2 = TypeFactory.Current.GetConcreteType (typeof (BaseType1));
      Assert.AreSame (t1, t2);
    }

    [Test]
    public void CacheRespectsDifferentTypeFactories ()
    {
      Type t1;
      using (new CurrentTypeFactoryScope(Assembly.GetExecutingAssembly()))
      {
        t1 = TypeFactory.Current.GetConcreteType (typeof (BaseType1));
      }

      Type t2;
      using (new CurrentTypeFactoryScope (Assembly.GetExecutingAssembly ()))
      {
        t2 = TypeFactory.Current.GetConcreteType (typeof (BaseType1));
      }

      Assert.AreNotSame (t1, t2);
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
    [Ignore ("TODO: Make serialization work")]
    public void CacheEvenWorksForSerialization ()
    {
      BaseType1 bt1 = ObjectFactory.Create<BaseType1>().With();
      BaseType1 bt2 = ObjectFactory.Create<BaseType1> ().With ();

      Assert.AreSame (bt1.GetType(), bt2.GetType());

      BaseType1[] array = SerializationTests.SerializeAndDeserialize (new BaseType1[] {bt1, bt2});
      Assert.AreNotSame (bt1.GetType(), array[0].GetType());

      Assert.AreSame (array[0].GetType(), array[1].GetType());
    }

    [Test]
    [Ignore ("TODO: Cache generated mixin types")]
    public void GeneratedMixinTypesAreCached()
    {
      Assert.Fail();
    }

    [Test]
    [Ignore ("TODO: Cache generated mixin types")]
    public void MixinTypesAreCached ()
    {
      Assert.Fail();
    }

    [Test]
    [Ignore ("TODO: Cache generated mixin types")]
    public void MixinTypeCacheRespectsDifferentTypeFactories ()
    {
      Assert.Fail ();
    }

    [Test]
    [Ignore ("TODO: Cache generated mixin types")]
    public void MixinTypeCacheIsBoundToConcreteTypeBuilder ()
    {
      Assert.Fail ();
    }

    [Test]
    [Ignore ("TODO: Make serialization work, TODO: Cache generated mixin types")]
    public void MixinTypeCacheEvenWorksForSerialization ()
    {
      Assert.Fail ();
    }
  }
}
