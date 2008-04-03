using System;
using System.Reflection;
using NUnit.Framework;
using Remotion.Data.DomainObjects.ObjectBinding.PropertyTypes;
using Remotion.Data.DomainObjects.Queries;
using Remotion.ObjectBinding;

namespace Remotion.Data.DomainObjects.ObjectBinding.UnitTests.PropertyTypes
{
  [TestFixture]
  public class EnumerationPropertyTest
  {
    // types

    public enum TestEnum
    {
      Undefined = 0,
      Value1 = 1,
      Value2 = 2
    }

    [UndefinedEnumValue (Undefined)]
    public enum TestEnumWithUndefinedAttribute
    {
      Undefined = 0,
      Value1 = 1,
      Value2 = 2
    }

    public class EnumerationObject : BindableSearchObject
    {
      private TestEnumWithUndefinedAttribute _enumerationWithUndefinedAttribute;
      private TestEnum _enumeration;

      public EnumerationObject ()
      {
      }

      public TestEnumWithUndefinedAttribute EnumerationWithUndefinedAttribute
      {
        get { return _enumerationWithUndefinedAttribute; }
        set { _enumerationWithUndefinedAttribute = value; }
      }

      public TestEnum Enumeration
      {
        get { return _enumeration; }
        set { _enumeration = value; }
      }

      public override IQuery CreateQuery()
      {
        throw new NotImplementedException();
      }
    }

    private EnumerationProperty _enumerationPropertyWithUndefinedAttribute;
    private EnumerationProperty _enumerationProperty;
    private EnumerationObject _enumerationObject;

    [SetUp]
    public void SetUp()
    {
      _enumerationObject = new EnumerationObject();
      _enumerationObject.Enumeration = TestEnum.Undefined;
      _enumerationObject.EnumerationWithUndefinedAttribute = TestEnumWithUndefinedAttribute.Undefined;

      Type type = typeof (EnumerationObject);
      PropertyInfo enumPropertyInfo = type.GetProperty ("Enumeration");
      Assert.IsNotNull (enumPropertyInfo);

      IBusinessObjectClass businessObjectClass = new SearchObjectClass (type);
      _enumerationProperty = new EnumerationProperty (businessObjectClass, enumPropertyInfo, true, typeof (TestEnum), false);

      PropertyInfo enumPropertyInfoWithUndefinedAttribute = type.GetProperty ("EnumerationWithUndefinedAttribute");
      Assert.IsNotNull (enumPropertyInfoWithUndefinedAttribute);

      _enumerationPropertyWithUndefinedAttribute =
          new EnumerationProperty (businessObjectClass, enumPropertyInfoWithUndefinedAttribute, true, typeof (TestEnumWithUndefinedAttribute), false);
    }

    [Test]
    public void GetAllValues()
    {
      IEnumerationValueInfo[] valueInfos = _enumerationProperty.GetAllValues (null);

      Assert.AreEqual (Enum.GetValues (typeof (TestEnum)).Length, valueInfos.Length);
    }

    [Test]
    public void GetAllValuesWithUndefinedAttribute()
    {
      IEnumerationValueInfo[] valueInfos = _enumerationPropertyWithUndefinedAttribute.GetAllValues (null);

      Assert.AreEqual (Enum.GetValues (typeof (TestEnumWithUndefinedAttribute)).Length - 1, valueInfos.Length);
      foreach (IEnumerationValueInfo valueInfo in valueInfos)
      {
        if ((TestEnumWithUndefinedAttribute) valueInfo.Value == TestEnumWithUndefinedAttribute.Undefined)
          Assert.Fail ("TestEnum.Undefined must not be returned by GetAllValues");
      }
    }

    [Test]
    public void FromInternalType()
    {
      Assert.AreEqual (TestEnum.Undefined, _enumerationProperty.FromInternalType (_enumerationObject, TestEnum.Undefined));
    }

    [Test]
    public void FromInternalTypeWithUndefinedAttribute()
    {
      Assert.IsNull (_enumerationPropertyWithUndefinedAttribute.FromInternalType (_enumerationObject, TestEnumWithUndefinedAttribute.Undefined));
    }

    [Test]
    public void ToInternalType()
    {
      Assert.AreEqual (TestEnum.Undefined, _enumerationProperty.ToInternalType (TestEnum.Undefined));
    }

    [Test]
    public void ToInternalTypeWithUndefinedAttribute()
    {
      Assert.AreEqual (TestEnumWithUndefinedAttribute.Undefined, _enumerationPropertyWithUndefinedAttribute.ToInternalType (null));
    }
  }
}