using System;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.ObjectBinding.BindableObject;
using Remotion.ObjectBinding.BindableObject.Properties;
using Remotion.ObjectBinding.UnitTests.Core.TestDomain;

namespace Remotion.ObjectBinding.UnitTests.Core.BindableObject
{
  [TestFixture]
  public class GuidPropertyTest : TestBase
  {
    private const string c_expectedGuidString = "01234567-0123-0123-0123-0123456789ab";
    private const string c_guidEmptyString = "00000000-0000-0000-0000-000000000000";
    private BindableObjectProvider _businessObjectProvider;

    public override void SetUp ()
    {
      base.SetUp();

      _businessObjectProvider = new BindableObjectProvider();
    }

    [Test]
    public void Initialize ()
    {
      IBusinessObjectStringProperty property = new GuidProperty (
          new PropertyBase.Parameters (_businessObjectProvider, GetPropertyInfo (typeof (ClassWithAllDataTypes), "Guid"),
          typeof (Guid), null, true, false));

      Assert.That (property.MaxLength, Is.EqualTo (38));
    }

    [Test]
    public void ConvertFromNativePropertyType_Scalar ()
    {
      PropertyBase property = GetScalarProperty();

      Assert.That (property.ConvertFromNativePropertyType (new Guid (c_expectedGuidString)), Is.EqualTo (c_expectedGuidString));
      Assert.That (property.ConvertFromNativePropertyType (Guid.Empty), Is.EqualTo (c_guidEmptyString));
      Assert.That (property.ConvertFromNativePropertyType (null), Is.EqualTo (null));
    }

    [Test]
    public void ConvertFromNativePropertyType_Array ()
    {
      PropertyBase property = GetArrayProperty();
      Guid[] guids = new Guid[] { new Guid (c_expectedGuidString), Guid.Empty, new Guid (c_expectedGuidString)};
      string[] expectedStrings = new string[] { c_expectedGuidString, c_guidEmptyString, c_expectedGuidString };

      Assert.That (property.ConvertFromNativePropertyType (guids), Is.EqualTo (expectedStrings));
      Assert.That (property.ConvertFromNativePropertyType (new Guid[0]), Is.EqualTo (new string[0]));
      Assert.That (property.ConvertFromNativePropertyType (null), Is.EqualTo (null));
    }

    [Test]
    public void ConvertFromNativePropertyType_NullableArray ()
    {
      PropertyBase property = GetNullableArrayProperty ();
      Guid?[] guids = new Guid?[] { new Guid (c_expectedGuidString), Guid.Empty, null, new Guid (c_expectedGuidString) };
      string[] expectedStrings = new string[] { c_expectedGuidString, c_guidEmptyString, null, c_expectedGuidString };

      Assert.That (property.ConvertFromNativePropertyType (guids), Is.EqualTo (expectedStrings));
    }

    [Test]
    public void ConvertToNativePropertyType_Scalar ()
    {
      PropertyBase property = GetScalarProperty();

      Assert.That (property.ConvertToNativePropertyType (c_expectedGuidString), Is.EqualTo (new Guid (c_expectedGuidString)));
      Assert.That (property.ConvertToNativePropertyType (string.Empty), Is.EqualTo (Guid.Empty));
      Assert.That (property.ConvertToNativePropertyType (c_guidEmptyString), Is.EqualTo (Guid.Empty));
      Assert.That (property.ConvertToNativePropertyType (null), Is.EqualTo (null));
    }

    [Test]
    public void ConvertToNativePropertyType_Array ()
    {
      PropertyBase property = GetArrayProperty ();
      Guid[] expectedGuids = new Guid[] { new Guid (c_expectedGuidString), Guid.Empty, Guid.Empty, new Guid (c_expectedGuidString), Guid.Empty };
      string[] strings = new string[] { c_expectedGuidString, string.Empty, null, c_expectedGuidString, c_guidEmptyString };

      Assert.That (property.ConvertToNativePropertyType (strings), Is.EqualTo (expectedGuids));
      Assert.That (property.ConvertToNativePropertyType (new string[0]), Is.EqualTo (new Guid[0]));
      Assert.That (property.ConvertToNativePropertyType (null), Is.EqualTo (null));
    }

    [Test]
    public void ConvertToNativePropertyType_NullableArray ()
    {
      PropertyBase property = GetNullableArrayProperty();
      Guid?[] expectedGuids = new Guid?[] { new Guid (c_expectedGuidString), Guid.Empty, null, new Guid (c_expectedGuidString), Guid.Empty };
      string[] strings = new string[] { c_expectedGuidString, string.Empty, null, c_expectedGuidString, c_guidEmptyString };

      Assert.That (property.ConvertToNativePropertyType (strings), Is.EqualTo (expectedGuids));
    }

    private PropertyBase GetScalarProperty ()
    {
      return new GuidProperty (
          new PropertyBase.Parameters (_businessObjectProvider, GetPropertyInfo (typeof (ClassWithValueType<Guid>), "Scalar"),
          typeof (Guid), null, true, false));
    }

    private PropertyBase GetArrayProperty ()
    {
      return new GuidProperty (
          new PropertyBase.Parameters (
              _businessObjectProvider,
              GetPropertyInfo (typeof (ClassWithValueType<Guid>), "Array"),
              typeof (Guid),
              new ListInfo (typeof (Guid[]), typeof (Guid)),
              false,
              false));
    }

    private PropertyBase GetNullableArrayProperty ()
    {
      return new GuidProperty (
          new PropertyBase.Parameters (
              _businessObjectProvider,
              GetPropertyInfo (typeof (ClassWithValueType<Guid>), "NullableArray"),
              typeof (Guid),
              new ListInfo (typeof (Guid?[]), typeof (Guid?)),
              false,
              false));
    }
  }
}