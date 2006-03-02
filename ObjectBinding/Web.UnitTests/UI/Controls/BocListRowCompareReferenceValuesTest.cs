using System;
using NUnit.Framework;
using Rubicon.Development.UnitTesting;
using Rubicon.NullableValueTypes;
using Rubicon.ObjectBinding.Reflection;
using Rubicon.ObjectBinding.Web.UI.Controls;
using Rubicon.ObjectBinding.Web.UnitTests.Domain;
using Rubicon.Web.Configuration;
using Rubicon.Web.UI;
using Rubicon.Web.UI.Controls;
using Rubicon.Web.UnitTests.Configuration;
using Rubicon.Web.Utilities;

namespace Rubicon.ObjectBinding.Web.UnitTests.UI.Controls
{

[TestFixture]
public class BocListRowsCompareReferenceValuesTest : BaseBocListRowCompareValuesTest
{
  private TypeWithReference _valueAA;
  private TypeWithReference _valueAB;
  private TypeWithReference _valueBA;

  private TypeWithReference _valueNullA;
  private TypeWithReference _valueNullB;
  private TypeWithReference _valueBNull;

  private ReflectionBusinessObjectClass _class;

  private BusinessObjectPropertyPath _firstValuePath;
  private BusinessObjectPropertyPath _secondValuePath;

  private BocSimpleColumnDefinition _firstValueSimpleColumn;
  private BocSimpleColumnDefinition _secondValueSimpleColumn;

  private BocCompoundColumnDefinition _firstValueFirstValueCompoundColumn;
  private BocCompoundColumnDefinition _firstValueSecondValueCompoundColumn;

  private BocCustomColumnDefinition _firstValueCustomColumn;
  private BocCustomColumnDefinition _secondValueCustomColumn;

  [SetUp]
  public virtual void SetUp()
  {
    _valueAA = new TypeWithReference();
    _valueAA.FirstValue = new TypeWithReference ("A");
    _valueAA.SecondValue = new TypeWithReference("A");

    _valueAB = new TypeWithReference();
    _valueAB.FirstValue = new TypeWithReference ("A");
    _valueAB.SecondValue = new TypeWithReference("B");

    _valueBA = new TypeWithReference();
    _valueBA.FirstValue = new TypeWithReference ("B");
    _valueBA.SecondValue = new TypeWithReference("A");

    _valueNullA = new TypeWithReference();
    _valueNullA.FirstValue = null;
    _valueNullA.SecondValue = new TypeWithReference("A");

    _valueNullB = new TypeWithReference();
    _valueNullB.FirstValue = null;
    _valueNullB.SecondValue = new TypeWithReference("B");

    _valueBNull = new TypeWithReference();
    _valueBNull.FirstValue = new TypeWithReference ("B");
    _valueBNull.SecondValue = null;


    _class = new ReflectionBusinessObjectClass (typeof (TypeWithReference));

    
    _firstValuePath = BusinessObjectPropertyPath.Parse (_class, "FirstValue");
    _secondValuePath = BusinessObjectPropertyPath.Parse (_class, "SecondValue");


    _firstValueSimpleColumn = new BocSimpleColumnDefinition ();
    _firstValueSimpleColumn.PropertyPath = _firstValuePath;

    _secondValueSimpleColumn = new BocSimpleColumnDefinition ();
    _secondValueSimpleColumn.PropertyPath = _secondValuePath;


    _firstValueFirstValueCompoundColumn = new BocCompoundColumnDefinition();
    _firstValueFirstValueCompoundColumn.PropertyPathBindings.Add (
        new PropertyPathBinding (_firstValuePath));
    _firstValueFirstValueCompoundColumn.PropertyPathBindings.Add (
        new PropertyPathBinding (_firstValuePath));
    _firstValueFirstValueCompoundColumn.FormatString = "{0}, {1}";

    _firstValueSecondValueCompoundColumn = new BocCompoundColumnDefinition();
    _firstValueSecondValueCompoundColumn.PropertyPathBindings.Add (
        new PropertyPathBinding (_firstValuePath));
    _firstValueSecondValueCompoundColumn.PropertyPathBindings.Add (
        new PropertyPathBinding (_secondValuePath));
    _firstValueSecondValueCompoundColumn.FormatString = "{0}, {1}";


    _firstValueCustomColumn = new BocCustomColumnDefinition ();
    _firstValueCustomColumn.PropertyPath = _firstValuePath;
    _firstValueCustomColumn.IsSortable = true;

    _secondValueCustomColumn = new BocCustomColumnDefinition ();
    _secondValueCustomColumn.PropertyPath = _secondValuePath;
    _secondValueCustomColumn.IsSortable = true;
  }


  [Test]
  public void CompareRowsWithSimpleColumns()
  {
    CompareEqualValuesAscending (_firstValueSimpleColumn, _valueAA, _valueAB);
    CompareEqualValuesAscending (_firstValueSimpleColumn, _valueNullA, _valueNullB);
    
    CompareEqualValuesDescending (_firstValueSimpleColumn, _valueAA, _valueAB);
    CompareEqualValuesDescending (_firstValueSimpleColumn, _valueNullA, _valueNullB);

    CompareAscendingValuesAscending (_firstValueSimpleColumn, _valueAA, _valueBA);
    CompareAscendingValuesAscending (_firstValueSimpleColumn, _valueNullA, _valueAA);
    
    CompareAscendingValuesDescending (_firstValueSimpleColumn, _valueAA, _valueBA);
    CompareAscendingValuesDescending (_firstValueSimpleColumn, _valueNullA, _valueAA);
  }

  [Test]
  public void CompareRowsWithCompoundColumns()
  {
    CompareEqualValuesAscending (_firstValueFirstValueCompoundColumn, _valueAA, _valueAB);
    CompareEqualValuesAscending (_firstValueFirstValueCompoundColumn, _valueNullA, _valueNullB);
    
    CompareEqualValuesDescending (_firstValueFirstValueCompoundColumn, _valueAA, _valueAB);
    CompareEqualValuesDescending (_firstValueFirstValueCompoundColumn, _valueNullA, _valueNullB);
    
    CompareAscendingValuesAscending (_firstValueSecondValueCompoundColumn, _valueAA, _valueBA);
    CompareAscendingValuesAscending (_firstValueSecondValueCompoundColumn, _valueAA, _valueAB);
    CompareAscendingValuesAscending (_firstValueSecondValueCompoundColumn, _valueNullA, _valueBNull);
    CompareAscendingValuesAscending (_firstValueSecondValueCompoundColumn, _valueNullA, _valueNullB);
    
    CompareAscendingValuesDescending (_firstValueSecondValueCompoundColumn, _valueAA, _valueBA);
    CompareAscendingValuesDescending (_firstValueSecondValueCompoundColumn, _valueAA, _valueAB);
    CompareAscendingValuesDescending (_firstValueSecondValueCompoundColumn, _valueNullA, _valueBNull);
    CompareAscendingValuesDescending (_firstValueSecondValueCompoundColumn, _valueNullA, _valueNullB);
  }


  [Test]
  public void CompareRowsWithCustomColumns()
  {
    CompareEqualValuesAscending (_firstValueCustomColumn, _valueAA, _valueAB);
    CompareEqualValuesAscending (_firstValueCustomColumn, _valueNullA, _valueNullB);
    
    CompareEqualValuesDescending (_firstValueCustomColumn, _valueAA, _valueAB);
    CompareEqualValuesDescending (_firstValueCustomColumn, _valueNullA, _valueNullB);
    
    CompareAscendingValuesAscending (_firstValueCustomColumn, _valueAA, _valueBA);
    CompareAscendingValuesAscending (_firstValueCustomColumn, _valueNullA, _valueBA);
    
    CompareAscendingValuesDescending (_firstValueCustomColumn, _valueAA, _valueBA);
    CompareAscendingValuesDescending (_firstValueCustomColumn, _valueNullA, _valueBA);
  }
}

}
