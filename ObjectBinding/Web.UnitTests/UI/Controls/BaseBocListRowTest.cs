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
public class BocListRowTest
{
  private TypeWithString _stringValueAA;
  private TypeWithString _stringValueAB;
  private TypeWithString _stringValueBA;

  private TypeWithReference _referenceValueAA;
  private TypeWithReference _referenceValueAB;
  private TypeWithReference _referenceValueBA;

  private ReflectionBusinessObjectClass _typeWithStringClass;
  private ReflectionBusinessObjectClass _typeWithReferenceClass;

  private BusinessObjectPropertyPath _typeWithStringFirstValuePath;
  private BusinessObjectPropertyPath _typeWithStringSecondValuePath;

  private BusinessObjectPropertyPath _typeWithReferenceFirstValuePath;
  private BusinessObjectPropertyPath _typeWithReferenceSecondValuePath;

  private BocSimpleColumnDefinition _typeWithStringFirstValueSimpleColumn;
  private BocSimpleColumnDefinition _typeWithStringSecondValueSimpleColumn;

  private BocSimpleColumnDefinition _typeWithReferenceFirstValueSimpleColumn;
  private BocSimpleColumnDefinition _typeWithReferenceSecondValueSimpleColumn;

  private BocCompoundColumnDefinition _typeWithStringFirstValueFirstValueCompoundColumn;
  private BocCompoundColumnDefinition _typeWithStringFirstValueSecondValueCompoundColumn;

  private BocCompoundColumnDefinition _typeWithReferenceFirstValueFirstValueCompoundColumn;
  private BocCompoundColumnDefinition _typeWithReferenceFirstValueSecondValueCompoundColumn;

  private BocCustomColumnDefinition _typeWithStringFirstValueCustomColumn;
  private BocCustomColumnDefinition _typeWithStringSecondValueCustomColumn;

  private BocCustomColumnDefinition _typeWithReferenceFirstValueCustomColumn;
  private BocCustomColumnDefinition _typeWithReferenceSecondValueCustomColumn;

  [SetUp]
  public virtual void SetUp()
  {
    InitializeStringValue();
    InititalizeReferenceValue();
  }

  private void InitializeStringValue()
  {
    _stringValueAA = new TypeWithString();
    _stringValueAA.FirstValue = "A";
    _stringValueAA.SecondValue = "A";

    _stringValueAB = new TypeWithString();
    _stringValueAB.FirstValue = "A";
    _stringValueAB.SecondValue = "B";

    _stringValueBA = new TypeWithString();
    _stringValueBA.FirstValue = "B";
    _stringValueBA.SecondValue = "A";

    _typeWithStringClass = new ReflectionBusinessObjectClass (typeof (TypeWithString));


    _typeWithStringFirstValuePath = BusinessObjectPropertyPath.Parse (_typeWithStringClass, "FirstValue");
    _typeWithStringSecondValuePath = BusinessObjectPropertyPath.Parse (_typeWithStringClass, "SecondValue");


    _typeWithStringFirstValueSimpleColumn = new BocSimpleColumnDefinition ();
    _typeWithStringFirstValueSimpleColumn.PropertyPath = _typeWithStringFirstValuePath;

    _typeWithStringSecondValueSimpleColumn = new BocSimpleColumnDefinition ();
    _typeWithStringSecondValueSimpleColumn.PropertyPath = _typeWithStringSecondValuePath;


    _typeWithStringFirstValueFirstValueCompoundColumn = new BocCompoundColumnDefinition();
    _typeWithStringFirstValueFirstValueCompoundColumn.PropertyPathBindings.Add (
        new PropertyPathBinding (_typeWithStringFirstValuePath));
    _typeWithStringFirstValueFirstValueCompoundColumn.PropertyPathBindings.Add (
        new PropertyPathBinding (_typeWithStringFirstValuePath));
    _typeWithStringFirstValueFirstValueCompoundColumn.FormatString = "{0}, {1}";

    _typeWithStringFirstValueSecondValueCompoundColumn = new BocCompoundColumnDefinition();
    _typeWithStringFirstValueSecondValueCompoundColumn.PropertyPathBindings.Add (
        new PropertyPathBinding (_typeWithStringFirstValuePath));
    _typeWithStringFirstValueSecondValueCompoundColumn.PropertyPathBindings.Add (
        new PropertyPathBinding (_typeWithStringSecondValuePath));
    _typeWithStringFirstValueSecondValueCompoundColumn.FormatString = "{0}, {1}";


    _typeWithStringFirstValueCustomColumn = new BocCustomColumnDefinition ();
    _typeWithStringFirstValueCustomColumn.PropertyPath = _typeWithStringFirstValuePath;
    _typeWithStringFirstValueCustomColumn.IsSortable = true;

    _typeWithStringSecondValueCustomColumn = new BocCustomColumnDefinition ();
    _typeWithStringSecondValueCustomColumn.PropertyPath = _typeWithStringSecondValuePath;
    _typeWithStringSecondValueCustomColumn.IsSortable = true;
  }

  private void InititalizeReferenceValue()
  {
    _referenceValueAA = new TypeWithReference();
    _referenceValueAA.FirstValue = new TypeWithReference ("A");
    _referenceValueAA.SecondValue = new TypeWithReference("A");

    _referenceValueAB = new TypeWithReference();
    _referenceValueAB.FirstValue = new TypeWithReference ("A");
    _referenceValueAB.SecondValue = new TypeWithReference("B");

    _referenceValueBA = new TypeWithReference();
    _referenceValueBA.FirstValue = new TypeWithReference ("B");
    _referenceValueBA.SecondValue = new TypeWithReference("A");


    _typeWithReferenceClass = new ReflectionBusinessObjectClass (typeof (TypeWithReference));

    
    _typeWithReferenceFirstValuePath = BusinessObjectPropertyPath.Parse (_typeWithReferenceClass, "FirstValue");
    _typeWithReferenceSecondValuePath = BusinessObjectPropertyPath.Parse (_typeWithReferenceClass, "SecondValue");


    _typeWithReferenceFirstValueSimpleColumn = new BocSimpleColumnDefinition ();
    _typeWithReferenceFirstValueSimpleColumn.PropertyPath = _typeWithReferenceFirstValuePath;

    _typeWithReferenceSecondValueSimpleColumn = new BocSimpleColumnDefinition ();
    _typeWithReferenceSecondValueSimpleColumn.PropertyPath = _typeWithReferenceSecondValuePath;


    _typeWithReferenceFirstValueFirstValueCompoundColumn = new BocCompoundColumnDefinition();
    _typeWithReferenceFirstValueFirstValueCompoundColumn.PropertyPathBindings.Add (
        new PropertyPathBinding (_typeWithReferenceFirstValuePath));
    _typeWithReferenceFirstValueFirstValueCompoundColumn.PropertyPathBindings.Add (
        new PropertyPathBinding (_typeWithReferenceFirstValuePath));
    _typeWithReferenceFirstValueFirstValueCompoundColumn.FormatString = "{0}, {1}";

    _typeWithReferenceFirstValueSecondValueCompoundColumn = new BocCompoundColumnDefinition();
    _typeWithReferenceFirstValueSecondValueCompoundColumn.PropertyPathBindings.Add (
        new PropertyPathBinding (_typeWithReferenceFirstValuePath));
    _typeWithReferenceFirstValueSecondValueCompoundColumn.PropertyPathBindings.Add (
        new PropertyPathBinding (_typeWithReferenceSecondValuePath));
    _typeWithReferenceFirstValueSecondValueCompoundColumn.FormatString = "{0}, {1}";


    _typeWithReferenceFirstValueCustomColumn = new BocCustomColumnDefinition ();
    _typeWithReferenceFirstValueCustomColumn.PropertyPath = _typeWithReferenceFirstValuePath;
    _typeWithReferenceFirstValueCustomColumn.IsSortable = true;

    _typeWithReferenceSecondValueCustomColumn = new BocCustomColumnDefinition ();
    _typeWithReferenceSecondValueCustomColumn.PropertyPath = _typeWithReferenceSecondValuePath;
    _typeWithReferenceSecondValueCustomColumn.IsSortable = true;
  }


  [Test]
  public void CompareRowsWithSimpleColumnHavingTypeWithStringAscending_A_A()
  {
    CompareEqualValuesAscending (_typeWithStringFirstValueSimpleColumn, _stringValueAA, _stringValueAB);
  }

  [Test]
  public void CompareRowsWithSimpleColumnHavingValueTypeWithStringDescending_A_A()
  {
    CompareEqualValuesDescending (_typeWithStringFirstValueSimpleColumn, _stringValueAA, _stringValueAB);
  }

  [Test]
  public void CompareRowsWithSimpleColumnHavingValueTypeWithStringAscending_A_B()
  {
    CompareAscendingValuesAscending (_typeWithStringFirstValueSimpleColumn, _stringValueAA, _stringValueBA);
  }

  [Test]
  public void CompareRowsWithSimpleColumnHavingValueTypeWithStringDescending_A_B()
  {
    CompareAscendingValuesDescending (_typeWithStringFirstValueSimpleColumn, _stringValueAA, _stringValueBA);
  }


  [Test]
  public void CompareRowsWithSimpleColumnHavingTypeWithReferenceAscending_A_A()
  {
    CompareEqualValuesAscending (_typeWithReferenceFirstValueSimpleColumn, _referenceValueAA, _referenceValueAB);
  }

  [Test]
  public void CompareRowsWithSimpleColumnHavingTypeWithReferenceDescending_A_A()
  {
    CompareEqualValuesDescending (_typeWithReferenceFirstValueSimpleColumn, _referenceValueAA, _referenceValueAB);
  }

  [Test]
  public void CompareRowsWithSimpleColumnHavingTypeWithReferenceAscending_A_B()
  {
    CompareAscendingValuesAscending (_typeWithReferenceFirstValueSimpleColumn, _referenceValueAA, _referenceValueBA);
  }

  [Test]
  public void CompareRowsWithSimpleColumnHavingTypeWithReferenceDescending_A_B()
  {
    CompareAscendingValuesDescending (_typeWithReferenceFirstValueSimpleColumn, _referenceValueAA, _referenceValueBA);
  }


  [Test]
  public void CompareRowsWithCompoundColumnHavingValueTypeWithStringAscending_AA_AA()
  {
    CompareEqualValuesAscending (_typeWithStringFirstValueFirstValueCompoundColumn, _stringValueAA, _stringValueAB);
  }

  [Test]
  public void CompareRowsWithCompoundColumnHavingValueTypeWithStringDescending_AA_AA()
  {
    CompareEqualValuesDescending (_typeWithStringFirstValueFirstValueCompoundColumn, _stringValueAA, _stringValueAB);
  }

  [Test]
  public void CompareRowsWithCompoundColumnHavingValueTypeWithStringAscending_AA_BA()
  {
    CompareAscendingValuesAscending (
        _typeWithStringFirstValueSecondValueCompoundColumn, _stringValueAA, _stringValueBA);
  }

  [Test]
  public void CompareRowsWithCompoundColumnHavingValueTypeWithStringDescending_AA_BA()
  {
    CompareAscendingValuesDescending (
        _typeWithStringFirstValueSecondValueCompoundColumn, _stringValueAA, _stringValueBA);
  }

  [Test]
  public void CompareRowsWithCompoundColumnHavingValueTypeWithStringAscending_AA_AB()
  {
    CompareAscendingValuesAscending (
        _typeWithStringFirstValueSecondValueCompoundColumn, _stringValueAA, _stringValueAB);
  }

  [Test]
  public void CompareRowsWithCompoundColumnHavingValueTypeWithStringDescending_AA_AB()
  {
    CompareAscendingValuesDescending (
        _typeWithStringFirstValueSecondValueCompoundColumn, _stringValueAA, _stringValueAB);
  }


  [Test]
  public void CompareRowsWithCompoundColumnHavingValueTypeWithReferenceAscending_AA_AA()
  {
    CompareEqualValuesAscending (
        _typeWithReferenceFirstValueFirstValueCompoundColumn, _referenceValueAA, _referenceValueAB);
  }

  [Test]
  public void CompareRowsWithCompoundColumnHavingValueTypeWithReferenceDescending_AA_AA()
  {
    CompareEqualValuesDescending (
        _typeWithReferenceFirstValueFirstValueCompoundColumn, _referenceValueAA, _referenceValueAB);
  }

  [Test]
  public void CompareRowsWithCompoundColumnHavingValueTypeWithReferenceAscending_AA_BA()
  {
    CompareAscendingValuesAscending (
        _typeWithReferenceFirstValueSecondValueCompoundColumn, _referenceValueAA, _referenceValueBA);
  }

  [Test]
  public void CompareRowsWithCompoundColumnHavingValueTypeWithReferenceDescending_AA_BA()
  {
    CompareAscendingValuesDescending (
        _typeWithReferenceFirstValueSecondValueCompoundColumn, _referenceValueAA, _referenceValueBA);
  }

  [Test]
  public void CompareRowsWithCompoundColumnHavingValueTypeWithReferenceAscending_AA_AB()
  {
    CompareAscendingValuesAscending (
        _typeWithReferenceFirstValueSecondValueCompoundColumn, _referenceValueAA, _referenceValueAB);
  }

  [Test]
  public void CompareRowsWithCompoundColumnHavingValueTypeWithReferenceDescending_AA_AB()
  {
    CompareAscendingValuesDescending (
        _typeWithReferenceFirstValueSecondValueCompoundColumn, _referenceValueAA, _referenceValueAB);
  }


  [Test]
  public void CompareRowsWithCustomColumnHavingTypeWithStringAscending_A_A()
  {
    CompareEqualValuesAscending (_typeWithStringFirstValueCustomColumn, _stringValueAA, _stringValueAB);
  }

  [Test]
  public void CompareRowsWithCustomColumnHavingValueTypeWithStringDescending_A_A()
  {
    CompareEqualValuesDescending (_typeWithStringFirstValueCustomColumn, _stringValueAA, _stringValueAB);
  }

  [Test]
  public void CompareRowsWithCustomColumnHavingValueTypeWithStringAscending_A_B()
  {
    CompareAscendingValuesAscending (_typeWithStringFirstValueCustomColumn, _stringValueAA, _stringValueBA);
  }

  [Test]
  public void CompareRowsWithCustomColumnHavingValueTypeWithStringDescending_A_B()
  {
    CompareAscendingValuesDescending (_typeWithStringFirstValueCustomColumn, _stringValueAA, _stringValueBA);
  }


  [Test]
  public void CompareRowsWithCustomColumnHavingTypeWithReferenceAscending_A_A()
  {
    CompareEqualValuesAscending (_typeWithReferenceFirstValueCustomColumn, _referenceValueAA, _referenceValueAB);
  }

  [Test]
  public void CompareRowsWithCustomColumnHavingTypeWithReferenceDescending_A_A()
  {
    CompareEqualValuesDescending (_typeWithReferenceFirstValueCustomColumn, _referenceValueAA, _referenceValueAB);
  }

  [Test]
  public void CompareRowsWithCustomColumnHavingTypeWithReferenceAscending_A_B()
  {
    CompareAscendingValuesAscending (_typeWithReferenceFirstValueCustomColumn, _referenceValueAA, _referenceValueBA);
  }

  [Test]
  public void CompareRowsWithCustomColumnHavingTypeWithReferenceDescending_A_B()
  {
    CompareAscendingValuesDescending (_typeWithReferenceFirstValueCustomColumn, _referenceValueAA, _referenceValueBA);
  }


  private void CompareEqualValuesAscending (
      BocColumnDefinition column, ReflectionBusinessObject left, ReflectionBusinessObject right)
  {
    BocListSortingOrderEntryMock[] sortingOrder = new BocListSortingOrderEntryMock[1];
    sortingOrder[0] = new BocListSortingOrderEntryMock (column, SortingDirection.Ascending);

    BocListSortingOrderProviderMock provider = new BocListSortingOrderProviderMock();
    provider.SetSortingOrder (sortingOrder);
    
    BocListRow rowLeft = new BocListRow (provider, 0, left);
    BocListRow rowRight = new BocListRow (provider, 0, right);

    int compareResultLeftRight = rowLeft.CompareTo (rowRight);
    int compareResultRightLeft = rowRight.CompareTo (rowLeft);

    Assert.IsTrue (compareResultLeftRight == 0, "Left - Right != zero");
    Assert.IsTrue (compareResultRightLeft == 0, "Right - Left != zero");
  }

  private void CompareEqualValuesDescending (
      BocColumnDefinition column, ReflectionBusinessObject left, ReflectionBusinessObject right)
  {
    BocListSortingOrderEntryMock[] sortingOrder = new BocListSortingOrderEntryMock[1];
    sortingOrder[0] = new BocListSortingOrderEntryMock (column, SortingDirection.Descending);

    BocListSortingOrderProviderMock provider = new BocListSortingOrderProviderMock();
    provider.SetSortingOrder (sortingOrder);
    
    BocListRow rowLeft = new BocListRow (provider, 0, left);
    BocListRow rowRight = new BocListRow (provider, 0, right);

    int compareResultLeftRight = rowLeft.CompareTo (rowRight);
    int compareResultRightLeft = rowRight.CompareTo (rowLeft);

    Assert.IsTrue (compareResultLeftRight == 0, "Left - Right != zero");
    Assert.IsTrue (compareResultRightLeft == 0, "Right - Left != zero");
  }

  private void CompareAscendingValuesAscending (
      BocColumnDefinition column, ReflectionBusinessObject left, ReflectionBusinessObject right)
  {
    BocListSortingOrderEntryMock[] sortingOrder = new BocListSortingOrderEntryMock[1];
    sortingOrder[0] = new BocListSortingOrderEntryMock (column, SortingDirection.Ascending);

    BocListSortingOrderProviderMock provider = new BocListSortingOrderProviderMock();
    provider.SetSortingOrder (sortingOrder);
    
    BocListRow rowLeft = new BocListRow (provider, 0, left);
    BocListRow rowRight = new BocListRow (provider, 0, right);

    int compareResultLeftRight = rowLeft.CompareTo (rowRight);
    int compareResultRightLeft = rowRight.CompareTo (rowLeft);

    Assert.IsTrue (compareResultLeftRight < 0, "Left - Right <= zero.");
    Assert.IsTrue (compareResultRightLeft > 0, "Right - Left >= zero.");
  }

  private void CompareAscendingValuesDescending (
      BocColumnDefinition column, ReflectionBusinessObject left, ReflectionBusinessObject right)
  {
    BocListSortingOrderEntryMock[] sortingOrder = new BocListSortingOrderEntryMock[1];
    sortingOrder[0] = new BocListSortingOrderEntryMock (column, SortingDirection.Descending);

    BocListSortingOrderProviderMock provider = new BocListSortingOrderProviderMock();
    provider.SetSortingOrder (sortingOrder);
    
    BocListRow rowLeft = new BocListRow (provider, 0, left);
    BocListRow rowRight = new BocListRow (provider, 0, right);

    int compareResultLeftRight = rowLeft.CompareTo (rowRight);
    int compareResultRightLeft = rowRight.CompareTo (rowLeft);

    Assert.IsTrue (compareResultLeftRight > 0, "Right - Left >= zero.");
    Assert.IsTrue (compareResultRightLeft < 0, "Left - Right <= zero.");
  }
}

}
