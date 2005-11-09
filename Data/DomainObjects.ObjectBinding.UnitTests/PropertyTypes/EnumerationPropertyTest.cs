using System;
using System.Reflection;
using NUnit.Framework;

using Rubicon.Data.DomainObjects.ObjectBinding.PropertyTypes;
using Rubicon.ObjectBinding;

namespace Rubicon.Data.DomainObjects.ObjectBinding.UnitTests.PropertyTypes
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

  [UndefinedEnumValue (TestEnumWithUndefinedAttribute.Undefined)]
  public enum TestEnumWithUndefinedAttribute
  {
    Undefined = 0,
    Value1 = 1,
    Value2 = 2
  }

  // static members and constants

  // member fields

  private TestEnumWithUndefinedAttribute _enumerationWithUndefinedAttribute;
  private TestEnum _enumeration;

  private EnumerationProperty _enumerationPropertyWithUndefinedAttribute;
  private EnumerationProperty _enumerationProperty;

  // construction and disposing

  public EnumerationPropertyTest ()
  {
    _enumeration = TestEnum.Undefined;
    _enumerationWithUndefinedAttribute = TestEnumWithUndefinedAttribute.Undefined;
  }

  // methods and properties

  [SetUp]
  public void SetUp ()
  {
    Type type = this.GetType ();
    PropertyInfo enumPropertyInfo = type.GetProperty ("Enumeration", BindingFlags.NonPublic | BindingFlags.Instance);
    Assert.IsNotNull (enumPropertyInfo);

    _enumerationProperty = new EnumerationProperty (enumPropertyInfo, true, typeof (TestEnum), false);

    PropertyInfo enumPropertyInfoWithUndefinedAttribute = type.GetProperty ("EnumerationWithUndefinedAttribute", BindingFlags.NonPublic | BindingFlags.Instance);
    Assert.IsNotNull (enumPropertyInfoWithUndefinedAttribute);

    _enumerationPropertyWithUndefinedAttribute = new EnumerationProperty (enumPropertyInfoWithUndefinedAttribute, true, typeof (TestEnumWithUndefinedAttribute), false);
  }

  private TestEnumWithUndefinedAttribute EnumerationWithUndefinedAttribute
  {
    get { return _enumerationWithUndefinedAttribute; }
    set { _enumerationWithUndefinedAttribute = value; }
  }

  private TestEnum Enumeration
  {
    get { return _enumeration; }
    set { _enumeration = value; }
  }

  [Test]
  public void GetAllValues ()
  {
    IEnumerationValueInfo[] valueInfos = _enumerationProperty.GetAllValues ();

    Assert.AreEqual (Enum.GetValues (typeof (TestEnum)).Length, valueInfos.Length);
  }

  [Test]
  public void GetAllValuesWithUndefinedAttribute ()
  {
    IEnumerationValueInfo[] valueInfos = _enumerationPropertyWithUndefinedAttribute.GetAllValues ();

    Assert.AreEqual (Enum.GetValues (typeof (TestEnumWithUndefinedAttribute)).Length - 1, valueInfos.Length);
    foreach (IEnumerationValueInfo valueInfo in valueInfos)
    {
      if ((TestEnumWithUndefinedAttribute) valueInfo.Value == TestEnumWithUndefinedAttribute.Undefined)
        Assert.Fail ("TestEnum.Undefined must not be returned by GetAllValues");
    }
  }

  [Test]
  public void FromInternalType ()
  {
    Assert.AreEqual (TestEnum.Undefined, _enumerationProperty.FromInternalType (TestEnum.Undefined));      
  }

  [Test]
  public void FromInternalTypeWithUndefinedAttribute ()
  {
    Assert.IsNull (_enumerationPropertyWithUndefinedAttribute.FromInternalType (TestEnumWithUndefinedAttribute.Undefined));      
  }

  [Test]
  public void ToInternalType ()
  {
    Assert.AreEqual (TestEnum.Undefined, _enumerationProperty.ToInternalType (TestEnum.Undefined));      
  }

  [Test]
  public void ToInternalTypeWithUndefinedAttribute ()
  {
    Assert.AreEqual (TestEnumWithUndefinedAttribute.Undefined, _enumerationPropertyWithUndefinedAttribute.ToInternalType (null));      
  }
}
}
