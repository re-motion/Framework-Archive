using System;
using System.Collections;
using System.Web;
using System.Web.SessionState;
using System.Collections.Specialized;
using System.Reflection;
using System.Threading;
using NUnit.Framework;
using Rubicon.NullableValueTypes;
using Rubicon.Development.UnitTesting;
using Rubicon.Web.ExecutionEngine;
using Rubicon.Utilities;
using Rubicon.Collections;
using Rubicon.Web.UnitTests.AspNetFramework;

namespace Rubicon.Web.UnitTests.ExecutionEngine
{

[TestFixture]
public class WxeParameterConverterTest
{
  private const string c_param = "param";

  private NameObjectCollection _callerParameters;

  private WxeParameterDeclaration _requiredObjectInParameter;
  private WxeParameterDeclaration _requiredStringInParameter;
  private WxeParameterDeclaration _requiredInt32InParameter;
  private WxeParameterDeclaration _requiredOutParameter;
  private WxeParameterDeclaration _stringInParameter;
  private WxeParameterDeclaration _int32InParameter;

  [SetUp]
  public virtual void SetUp()
  {
    _requiredObjectInParameter = new WxeParameterDeclaration (c_param, true, WxeParameterDirection.In, typeof (object));
    _requiredStringInParameter = new WxeParameterDeclaration (c_param, true, WxeParameterDirection.In, typeof (string));
    _requiredInt32InParameter = new WxeParameterDeclaration (c_param, true, WxeParameterDirection.In, typeof (Int32));
    _requiredOutParameter = new WxeParameterDeclaration (c_param, true, WxeParameterDirection.Out, typeof (string));

    _stringInParameter = new WxeParameterDeclaration (c_param, false, WxeParameterDirection.In, typeof (string));
    _int32InParameter = new WxeParameterDeclaration (c_param, false, WxeParameterDirection.In, typeof (Int32));

    _callerParameters = new NameObjectCollection();
  }

  [Test]
  public void ConvertObjectToStringRequiredStringInParameter()
  {
    string value = "Hello World!";
    WxeParameterConverterMock converter = new WxeParameterConverterMock (_requiredStringInParameter);
    Assert.AreEqual (value, converter.ConvertObjectToString (value));
  }

  [Test]
  [ExpectedException (typeof (WxeException))]
  public void ConvertObjectToStringRequiredStringInParameterWithNullValue()
  {
    WxeParameterConverterMock converter = new WxeParameterConverterMock (_requiredStringInParameter);
    converter.ConvertObjectToString (null);
    Assert.Fail();
  }

  [Test]
  public void ConvertObjectToStringRequiredInt32InParameter()
  {
    int value = 1;
    WxeParameterConverterMock converter = new WxeParameterConverterMock (_requiredInt32InParameter);
    Assert.AreEqual (value.ToString(), converter.ConvertObjectToString (value));
  }

  [Test]
  [ExpectedException (typeof (WxeException))]
  public void ConvertObjectToStringRequiredInt32InParameterWithNullValue()
  {
    WxeParameterConverterMock converter = new WxeParameterConverterMock (_requiredInt32InParameter);
    converter.ConvertObjectToString (null);
    Assert.Fail();
  }

  [Test]
  public void ConvertObjectToStringRequiredVarRefInParameter()
  {
    int value = 1;
    WxeVariableReference varRef = new WxeVariableReference (c_param);
    _callerParameters.Add (c_param, value);
    WxeParameterConverterMock converter = new WxeParameterConverterMock (_requiredInt32InParameter);
    Assert.AreEqual (value.ToString(), converter.ConvertVarRefToString (varRef, _callerParameters));
  }

  [Test]
  [ExpectedException (typeof (WxeException))]
  public void ConvertVarRefToStringRequiredVarRefInParameterWithNoCallerParameters()
  {
    WxeVariableReference varRef = new WxeVariableReference (c_param);
    WxeParameterConverterMock converter = new WxeParameterConverterMock (_requiredInt32InParameter);
    converter.ConvertVarRefToString (varRef, null);
    Assert.Fail();
  }

  [Test]
  [ExpectedException (typeof (WxeException))]
  public void ConvertVarRefToStringRequiredVarRefInParameterWithVarRef()
  {
    WxeVariableReference value = new WxeVariableReference (c_param);
    WxeVariableReference varRef = new WxeVariableReference (c_param);
    _callerParameters.Add (c_param, value);
    WxeParameterConverterMock converter = new WxeParameterConverterMock (_requiredInt32InParameter);
    converter.ConvertVarRefToString (varRef, _callerParameters);
    Assert.Fail();
  }

  [Test]
  public void ConvertObjectToStringStringInParameter()
  {
    string value = "Hello World!";
    WxeParameterConverterMock converter = new WxeParameterConverterMock (_stringInParameter);
    Assert.AreEqual (value, converter.ConvertObjectToString (value));
  }

  [Test]
  public void ConvertObjectToStringStringInParameterWithNullValue()
  {
    WxeParameterConverterMock converter = new WxeParameterConverterMock (_stringInParameter);
    Assert.AreEqual (string.Empty, converter.ConvertObjectToString (null));
  }

  [Test]
  public void ConvertObjectToStringInt32InParameter()
  {
    int value = 1;
    WxeParameterConverterMock converter = new WxeParameterConverterMock (_int32InParameter);
    Assert.AreEqual (value.ToString(), converter.ConvertObjectToString (value));
  }

  [Test]
  public void ConvertObjectToStringInt32InParameterWithNullValue()
  {
    WxeParameterConverterMock converter = new WxeParameterConverterMock (_int32InParameter);
    Assert.AreEqual (string.Empty, converter.ConvertObjectToString (null));
  }

  [Test]
  public void ConvertObjectToStringVarRefInt32InParameter()
  {
    int value = 1;
    WxeVariableReference varRef = new WxeVariableReference (c_param);
    _callerParameters.Add (c_param, value);
    WxeParameterConverterMock converter = new WxeParameterConverterMock (_int32InParameter);
    Assert.AreEqual (value.ToString(), converter.ConvertVarRefToString (varRef, _callerParameters));
  }

  [Test]
  public void ConvertVarRefToStringVarRefInt32InParameterWithNoCallerParameters()
  {
    WxeVariableReference varRef = new WxeVariableReference (c_param);
    WxeParameterConverterMock converter = new WxeParameterConverterMock (_int32InParameter);
    Assert.AreEqual (string.Empty, converter.ConvertVarRefToString (varRef, null));
  }

  [Test]
  public void ConvertVarRefToStringVarRefInParameterWithVarRef()
  {
    WxeVariableReference value = new WxeVariableReference (c_param);
    WxeVariableReference varRef = new WxeVariableReference (c_param);
    _callerParameters.Add (c_param, value);
    WxeParameterConverterMock converter = new WxeParameterConverterMock (_int32InParameter);
    Assert.AreEqual (string.Empty, converter.ConvertVarRefToString (varRef, _callerParameters));    
  }
  [Test]
  [ExpectedException (typeof (WxeException))]
  public void CheckForRequiredOutParameter()
  {
    WxeParameterConverterMock converter = new WxeParameterConverterMock (_requiredOutParameter);
    converter.CheckForRequiredOutParameter();
    Assert.Fail();
  }

  [Test]
  public void TryConvertStringToStringRequiredStringInParameter()
  {
    string value = "Hello World!";
    WxeParameterConverterMock converter = new WxeParameterConverterMock (_requiredStringInParameter);
    Assert.AreEqual (value, converter.TryConvertStringToString (value));
  }

  [Test]
  [ExpectedException (typeof (WxeException))]
  public void TryConvertStringToStringRequiredStringInParameterWithEmptyString()
  {
    WxeParameterConverterMock converter = new WxeParameterConverterMock (_requiredStringInParameter);
    converter.TryConvertStringToString (string.Empty);
    Assert.Fail();
  }

  [Test]
  public void TryConvertStringToStringInt32InParameter()
  {
    int value = 1;
    WxeParameterConverterMock converter = new WxeParameterConverterMock (_requiredInt32InParameter);
    Assert.AreEqual (value, converter.TryConvertStringToString (value));
  }

  [Test]
  public void TryConvertStringToStringStringInParameterWithEmptyString()
  {
    string value = string.Empty;
    WxeParameterConverterMock converter = new WxeParameterConverterMock (_stringInParameter);
    Assert.AreEqual (value, converter.TryConvertStringToString (value));
  }

  [Test]
  public void TryConvertObjectToStringForParseMethodRequiredInt32InParameter()
  {
    int value = 1;
    WxeParameterConverterMock converter = new WxeParameterConverterMock (_requiredInt32InParameter);
    Assert.AreEqual (value.ToString(), converter.TryConvertObjectToStringForParseMethod (value));
  }

  [Test]
  public void TryConvertObjectToStringForParseMethodRequiredInt32InParameterWithNullValue()
  {
    object value = null;
    WxeParameterConverterMock converter = new WxeParameterConverterMock (_requiredInt32InParameter);
    Assert.AreEqual (value, converter.TryConvertObjectToStringForParseMethod (value));
  }

  [Test]
  public void TryConvertObjectToStringForParseMethodRequiredInt32InParameterWithForTypeWithoutParseMethod()
  {
    object value = new object();
    WxeParameterConverterMock converter = new WxeParameterConverterMock (_requiredObjectInParameter);
    Assert.AreEqual (value, converter.TryConvertObjectToStringForParseMethod (value));
  }

  [Test]
  [ExpectedException (typeof (WxeException))]
  public void TryConvertNullToStringRequiredInt32InParameterWithNullValue()
  {
    WxeParameterConverterMock converter = new WxeParameterConverterMock (_requiredInt32InParameter);
    converter.TryConvertNullToString (null);
    Assert.Fail();
  }

  [Test]
  public void TryConvertNullToStringRequiredInt32InParameterWithNonNullValue()
  {
    int value = 1;
    WxeParameterConverterMock converter = new WxeParameterConverterMock (_requiredInt32InParameter);
    Assert.AreEqual (value, converter.TryConvertNullToString (value));
  }

  [Test]
  public void TryConvertNullToStringInt32InParameterWithNullValue()
  {
    object value = null;
    WxeParameterConverterMock converter = new WxeParameterConverterMock (_int32InParameter);
    Assert.AreEqual (string.Empty, converter.TryConvertNullToString (value));
  }
}

}
