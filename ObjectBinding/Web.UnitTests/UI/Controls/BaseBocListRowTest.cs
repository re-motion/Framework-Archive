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
  }


  [Test]
  public void CompareRowsWithSimpleColumnHavingTypeWithStringAscending_A_A()
  {
    BocListSortingOrderEntryMock[] sortingOrder = new BocListSortingOrderEntryMock[1];
    sortingOrder[0] = new BocListSortingOrderEntryMock (
        _typeWithStringFirstValueSimpleColumn, SortingDirection.Ascending);

    BocListSortingOrderProviderMock provider = new BocListSortingOrderProviderMock();
    provider.SetSortingOrder (sortingOrder);
    
    BocListRow rowWithTypeWithStringAA = new BocListRow (provider, 0, _stringValueAA);
    BocListRow rowWithTypeWithStringAB = new BocListRow (provider, 0, _stringValueAB);

    int compareResultLeftRight = rowWithTypeWithStringAA.CompareTo (rowWithTypeWithStringAB);
    int compareResultRightLeft = rowWithTypeWithStringAB.CompareTo (rowWithTypeWithStringAA);

    Assert.IsTrue (compareResultLeftRight == 0, "Left - Right != zero");
    Assert.IsTrue (compareResultRightLeft == 0, "Right - Left != zero");
  }

  [Test]
  public void CompareRowsWithSimpleColumnHavingValueTypeWithStringDescending_A_A()
  {
    BocListSortingOrderEntryMock[] sortingOrder = new BocListSortingOrderEntryMock[1];
    sortingOrder[0] = new BocListSortingOrderEntryMock (
        _typeWithStringFirstValueSimpleColumn, SortingDirection.Descending);

    BocListSortingOrderProviderMock provider = new BocListSortingOrderProviderMock();
    provider.SetSortingOrder (sortingOrder);
    
    BocListRow rowWithTypeWithStringAA = new BocListRow (provider, 0, _stringValueAA);
    BocListRow rowWithTypeWithStringAB = new BocListRow (provider, 0, _stringValueAB);

    int compareResultLeftRight = rowWithTypeWithStringAA.CompareTo (rowWithTypeWithStringAB);
    int compareResultRightLeft = rowWithTypeWithStringAB.CompareTo (rowWithTypeWithStringAA);

    Assert.IsTrue (compareResultLeftRight == 0, "Left - Right != zero");
    Assert.IsTrue (compareResultRightLeft == 0, "Right - Left != zero");
  }

  [Test]
  public void CompareRowsWithSimpleColumnHavingValueTypeWithStringAscending_A_B()
  {
    BocListSortingOrderEntryMock[] sortingOrder = new BocListSortingOrderEntryMock[1];
    sortingOrder[0] = 
        new BocListSortingOrderEntryMock (_typeWithStringFirstValueSimpleColumn, SortingDirection.Ascending);

    BocListSortingOrderProviderMock provider = new BocListSortingOrderProviderMock();
    provider.SetSortingOrder (sortingOrder);
    
    BocListRow rowWithTypeWithStringAA = new BocListRow (provider, 0, _stringValueAA);
    BocListRow rowWithTypeWithStringBA = new BocListRow (provider, 0, _stringValueBA);

    int compareResultLeftRight = rowWithTypeWithStringAA.CompareTo (rowWithTypeWithStringBA);
    int compareResultRightLeft = rowWithTypeWithStringBA.CompareTo (rowWithTypeWithStringAA);

    Assert.IsTrue (compareResultLeftRight < 0, "Left - Right <= zero.");
    Assert.IsTrue (compareResultRightLeft > 0, "Right - Left >= zero.");
  }

  [Test]
  public void CompareRowsWithSimpleColumnHavingValueTypeWithStringDescending_A_B()
  {
    BocListSortingOrderEntryMock[] sortingOrder = new BocListSortingOrderEntryMock[1];
    sortingOrder[0] = new BocListSortingOrderEntryMock (
        _typeWithStringFirstValueSimpleColumn, SortingDirection.Descending);

    BocListSortingOrderProviderMock provider = new BocListSortingOrderProviderMock();
    provider.SetSortingOrder (sortingOrder);
    
    BocListRow rowWithTypeWithStringAA = new BocListRow (provider, 0, _stringValueAA);
    BocListRow rowWithTypeWithStringBA = new BocListRow (provider, 0, _stringValueBA);

    int compareResultLeftRight = rowWithTypeWithStringAA.CompareTo (rowWithTypeWithStringBA);
    int compareResultRightLeft = rowWithTypeWithStringBA.CompareTo (rowWithTypeWithStringAA);

    Assert.IsTrue (compareResultLeftRight > 0, "Right - Left >= zero.");
    Assert.IsTrue (compareResultRightLeft < 0, "Left - Right <= zero.");
  }


  [Test]
  public void CompareRowsWithSimpleColumnHavingTypeWithReferenceAscending_A_A()
  {
    BocListSortingOrderEntryMock[] sortingOrder = new BocListSortingOrderEntryMock[1];
    sortingOrder[0] = new BocListSortingOrderEntryMock (
        _typeWithReferenceFirstValueSimpleColumn, SortingDirection.Ascending);

    BocListSortingOrderProviderMock provider = new BocListSortingOrderProviderMock();
    provider.SetSortingOrder (sortingOrder);
    
    BocListRow rowWithTypeWithReferenceAA = new BocListRow (provider, 0, _referenceValueAA);
    BocListRow rowWithTypeWithReferenceAB = new BocListRow (provider, 0, _referenceValueAB);

    int compareResultLeftRight = rowWithTypeWithReferenceAA.CompareTo (rowWithTypeWithReferenceAB);
    int compareResultRightLeft = rowWithTypeWithReferenceAB.CompareTo (rowWithTypeWithReferenceAA);

    Assert.IsTrue (compareResultLeftRight == 0, "Left - Right != zero");
    Assert.IsTrue (compareResultRightLeft == 0, "Right - Left != zero");
  }

  [Test]
  public void CompareRowsWithSimpleColumnHavingTypeWithReferenceDescending_A_A()
  {
    BocListSortingOrderEntryMock[] sortingOrder = new BocListSortingOrderEntryMock[1];
    sortingOrder[0] = new BocListSortingOrderEntryMock (
        _typeWithReferenceFirstValueSimpleColumn, SortingDirection.Descending);

    BocListSortingOrderProviderMock provider = new BocListSortingOrderProviderMock();
    provider.SetSortingOrder (sortingOrder);
    
    BocListRow rowWithTypeWithReferenceAA = new BocListRow (provider, 0, _referenceValueAA);
    BocListRow rowWithTypeWithReferenceAB = new BocListRow (provider, 0, _referenceValueAB);

    int compareResultLeftRight = rowWithTypeWithReferenceAA.CompareTo (rowWithTypeWithReferenceAB);
    int compareResultRightLeft = rowWithTypeWithReferenceAB.CompareTo (rowWithTypeWithReferenceAA);

    Assert.IsTrue (compareResultLeftRight == 0, "Left - Right != zero");
    Assert.IsTrue (compareResultRightLeft == 0, "Right - Left != zero");
  }

  [Test]
  public void CompareRowsWithSimpleColumnHavingTypeWithReferenceAscending_A_B()
  {
    BocListSortingOrderEntryMock[] sortingOrder = new BocListSortingOrderEntryMock[1];
    sortingOrder[0] = new BocListSortingOrderEntryMock (
        _typeWithReferenceFirstValueSimpleColumn, SortingDirection.Ascending);

    BocListSortingOrderProviderMock provider = new BocListSortingOrderProviderMock();
    provider.SetSortingOrder (sortingOrder);
    
    BocListRow rowWithTypeWithReferenceAA = new BocListRow (provider, 0, _referenceValueAA);
    BocListRow rowWithTypeWithReferenceBA = new BocListRow (provider, 0, _referenceValueBA);

    int compareResultLeftRight = rowWithTypeWithReferenceAA.CompareTo (rowWithTypeWithReferenceBA);
    int compareResultRightLeft = rowWithTypeWithReferenceBA.CompareTo (rowWithTypeWithReferenceAA);

    Assert.IsTrue (compareResultLeftRight < 0, "Left - Right <= zero.");
    Assert.IsTrue (compareResultRightLeft > 0, "Right - Left >= zero.");
  }

  [Test]
  public void CompareRowsWithSimpleColumnHavingTypeWithReferenceDescending_A_B()
  {
    BocListSortingOrderEntryMock[] sortingOrder = new BocListSortingOrderEntryMock[1];
    sortingOrder[0] = new BocListSortingOrderEntryMock (
        _typeWithReferenceFirstValueSimpleColumn, SortingDirection.Descending);

    BocListSortingOrderProviderMock provider = new BocListSortingOrderProviderMock();
    provider.SetSortingOrder (sortingOrder);
    
    BocListRow rowWithTypeWithReferenceAA = new BocListRow (provider, 0, _referenceValueAA);
    BocListRow rowWithTypeWithReferenceBA = new BocListRow (provider, 0, _referenceValueBA);

    int compareResultLeftRight = rowWithTypeWithReferenceAA.CompareTo (rowWithTypeWithReferenceBA);
    int compareResultRightLeft = rowWithTypeWithReferenceBA.CompareTo (rowWithTypeWithReferenceAA);

    Assert.IsTrue (compareResultLeftRight > 0, "Right - Left >= zero.");
    Assert.IsTrue (compareResultRightLeft < 0, "Left - Right <= zero.");
  }


  [Test]
  public void CompareRowsWithCompoundColumnHavingValueTypeWithStringAscending_AA_AA()
  {
    BocListSortingOrderEntryMock[] sortingOrder = new BocListSortingOrderEntryMock[1];
    sortingOrder[0] = new BocListSortingOrderEntryMock (
        _typeWithStringFirstValueFirstValueCompoundColumn, SortingDirection.Ascending);

    BocListSortingOrderProviderMock provider = new BocListSortingOrderProviderMock();
    provider.SetSortingOrder (sortingOrder);
    
    BocListRow rowWithTypeWithStringAA = new BocListRow (provider, 0, _stringValueAA);
    BocListRow rowWithTypeWithStringAB = new BocListRow (provider, 0, _stringValueAB);

    int compareResultLeftRight = rowWithTypeWithStringAA.CompareTo (rowWithTypeWithStringAB);
    int compareResultRightLeft = rowWithTypeWithStringAB.CompareTo (rowWithTypeWithStringAA);

    Assert.IsTrue (compareResultLeftRight == 0, "Left - Right != zero");
    Assert.IsTrue (compareResultRightLeft == 0, "Right - Left != zero");
  }

  [Test]
  public void CompareRowsWithCompoundColumnHavingValueTypeWithStringDescending_AA_AA()
  {
    BocListSortingOrderEntryMock[] sortingOrder = new BocListSortingOrderEntryMock[1];
    sortingOrder[0] = new BocListSortingOrderEntryMock (
        _typeWithStringFirstValueFirstValueCompoundColumn, SortingDirection.Descending);

    BocListSortingOrderProviderMock provider = new BocListSortingOrderProviderMock();
    provider.SetSortingOrder (sortingOrder);
    
    BocListRow rowWithTypeWithStringAA = new BocListRow (provider, 0, _stringValueAA);
    BocListRow rowWithTypeWithStringAB = new BocListRow (provider, 0, _stringValueAB);

    int compareResultLeftRight = rowWithTypeWithStringAA.CompareTo (rowWithTypeWithStringAB);
    int compareResultRightLeft = rowWithTypeWithStringAB.CompareTo (rowWithTypeWithStringAA);

    Assert.IsTrue (compareResultLeftRight == 0, "Left - Right != zero");
    Assert.IsTrue (compareResultRightLeft == 0, "Right - Left != zero");
  }

  [Test]
  public void CompareRowsWithCompoundColumnHavingValueTypeWithStringAscending_AA_BA()
  {
    BocListSortingOrderEntryMock[] sortingOrder = new BocListSortingOrderEntryMock[1];
    sortingOrder[0] = new BocListSortingOrderEntryMock (
        _typeWithStringFirstValueSecondValueCompoundColumn, SortingDirection.Ascending);

    BocListSortingOrderProviderMock provider = new BocListSortingOrderProviderMock();
    provider.SetSortingOrder (sortingOrder);
    
    BocListRow rowWithTypeWithStringAA = new BocListRow (provider, 0, _stringValueAA);
    BocListRow rowWithTypeWithStringBA = new BocListRow (provider, 0, _stringValueBA);

    int compareResultLeftRight = rowWithTypeWithStringAA.CompareTo (rowWithTypeWithStringBA);
    int compareResultRightLeft = rowWithTypeWithStringBA.CompareTo (rowWithTypeWithStringAA);

    Assert.IsTrue (compareResultLeftRight < 0, "Left - Right <= zero.");
    Assert.IsTrue (compareResultRightLeft > 0, "Right - Left >= zero.");
  }

  [Test]
  public void CompareRowsWithCompoundColumnHavingValueTypeWithStringDescending_AA_BA()
  {
    BocListSortingOrderEntryMock[] sortingOrder = new BocListSortingOrderEntryMock[1];
    sortingOrder[0] = new BocListSortingOrderEntryMock (
        _typeWithStringFirstValueSecondValueCompoundColumn, SortingDirection.Descending);

    BocListSortingOrderProviderMock provider = new BocListSortingOrderProviderMock();
    provider.SetSortingOrder (sortingOrder);
    
    BocListRow rowWithTypeWithStringAA = new BocListRow (provider, 0, _stringValueAA);
    BocListRow rowWithTypeWithStringBA = new BocListRow (provider, 0, _stringValueBA);

    int compareResultLeftRight = rowWithTypeWithStringAA.CompareTo (rowWithTypeWithStringBA);
    int compareResultRightLeft = rowWithTypeWithStringBA.CompareTo (rowWithTypeWithStringAA);

    Assert.IsTrue (compareResultLeftRight > 0, "Right - Left >= zero.");
    Assert.IsTrue (compareResultRightLeft < 0, "Left - Right <= zero.");
  }

  [Test]
  public void CompareRowsWithCompoundColumnHavingValueTypeWithStringAscending_AA_AB()
  {
    BocListSortingOrderEntryMock[] sortingOrder = new BocListSortingOrderEntryMock[1];
    sortingOrder[0] = new BocListSortingOrderEntryMock (
        _typeWithStringFirstValueSecondValueCompoundColumn, SortingDirection.Ascending);

    BocListSortingOrderProviderMock provider = new BocListSortingOrderProviderMock();
    provider.SetSortingOrder (sortingOrder);
    
    BocListRow rowWithTypeWithStringAA = new BocListRow (provider, 0, _stringValueAA);
    BocListRow rowWithTypeWithStringAB = new BocListRow (provider, 0, _stringValueAB);

    int compareResultLeftRight = rowWithTypeWithStringAA.CompareTo (rowWithTypeWithStringAB);
    int compareResultRightLeft = rowWithTypeWithStringAB.CompareTo (rowWithTypeWithStringAA);

    Assert.IsTrue (compareResultLeftRight < 0, "Left - Right <= zero.");
    Assert.IsTrue (compareResultRightLeft > 0, "Right - Left >= zero.");
  }

  [Test]
  public void CompareRowsWithCompoundColumnHavingValueTypeWithStringDescending_AA_AB()
  {
    BocListSortingOrderEntryMock[] sortingOrder = new BocListSortingOrderEntryMock[1];
    sortingOrder[0] = new BocListSortingOrderEntryMock (
        _typeWithStringFirstValueSecondValueCompoundColumn, SortingDirection.Descending);

    BocListSortingOrderProviderMock provider = new BocListSortingOrderProviderMock();
    provider.SetSortingOrder (sortingOrder);
    
    BocListRow rowWithTypeWithStringAA = new BocListRow (provider, 0, _stringValueAA);
    BocListRow rowWithTypeWithStringAB = new BocListRow (provider, 0, _stringValueAB);

    int compareResultLeftRight = rowWithTypeWithStringAA.CompareTo (rowWithTypeWithStringAB);
    int compareResultRightLeft = rowWithTypeWithStringAB.CompareTo (rowWithTypeWithStringAA);

    Assert.IsTrue (compareResultLeftRight > 0, "Right - Left >= zero.");
    Assert.IsTrue (compareResultRightLeft < 0, "Left - Right <= zero.");
  }


  [Test]
  public void CompareRowsWithCompoundColumnHavingValueTypeWithReferenceAscending_AA_AA()
  {
    BocListSortingOrderEntryMock[] sortingOrder = new BocListSortingOrderEntryMock[1];
    sortingOrder[0] = new BocListSortingOrderEntryMock (
        _typeWithReferenceFirstValueFirstValueCompoundColumn, SortingDirection.Ascending);

    BocListSortingOrderProviderMock provider = new BocListSortingOrderProviderMock();
    provider.SetSortingOrder (sortingOrder);
    
    BocListRow rowWithTypeWithReferenceAA = new BocListRow (provider, 0, _referenceValueAA);
    BocListRow rowWithTypeWithReferenceAB = new BocListRow (provider, 0, _referenceValueAB);

    int compareResultLeftRight = rowWithTypeWithReferenceAA.CompareTo (rowWithTypeWithReferenceAB);
    int compareResultRightLeft = rowWithTypeWithReferenceAB.CompareTo (rowWithTypeWithReferenceAA);

    Assert.IsTrue (compareResultLeftRight == 0, "Left - Right != zero");
    Assert.IsTrue (compareResultRightLeft == 0, "Right - Left != zero");
  }

  [Test]
  public void CompareRowsWithCompoundColumnHavingValueTypeWithReferenceDescending_AA_AA()
  {
    BocListSortingOrderEntryMock[] sortingOrder = new BocListSortingOrderEntryMock[1];
    sortingOrder[0] = new BocListSortingOrderEntryMock (
        _typeWithReferenceFirstValueFirstValueCompoundColumn, SortingDirection.Descending);

    BocListSortingOrderProviderMock provider = new BocListSortingOrderProviderMock();
    provider.SetSortingOrder (sortingOrder);
    
    BocListRow rowWithTypeWithReferenceAA = new BocListRow (provider, 0, _referenceValueAA);
    BocListRow rowWithTypeWithReferenceAB = new BocListRow (provider, 0, _referenceValueAB);

    int compareResultLeftRight = rowWithTypeWithReferenceAA.CompareTo (rowWithTypeWithReferenceAB);
    int compareResultRightLeft = rowWithTypeWithReferenceAB.CompareTo (rowWithTypeWithReferenceAA);

    Assert.IsTrue (compareResultLeftRight == 0, "Left - Right != zero");
    Assert.IsTrue (compareResultRightLeft == 0, "Right - Left != zero");
  }

  [Test]
  public void CompareRowsWithCompoundColumnHavingValueTypeWithReferenceAscending_AA_BA()
  {
    BocListSortingOrderEntryMock[] sortingOrder = new BocListSortingOrderEntryMock[1];
    sortingOrder[0] = new BocListSortingOrderEntryMock (
        _typeWithReferenceFirstValueSecondValueCompoundColumn, SortingDirection.Ascending);

    BocListSortingOrderProviderMock provider = new BocListSortingOrderProviderMock();
    provider.SetSortingOrder (sortingOrder);
    
    BocListRow rowWithTypeWithReferenceAA = new BocListRow (provider, 0, _referenceValueAA);
    BocListRow rowWithTypeWithReferenceBA = new BocListRow (provider, 0, _referenceValueBA);

    int compareResultLeftRight = rowWithTypeWithReferenceAA.CompareTo (rowWithTypeWithReferenceBA);
    int compareResultRightLeft = rowWithTypeWithReferenceBA.CompareTo (rowWithTypeWithReferenceAA);

    Assert.IsTrue (compareResultLeftRight < 0, "Left - Right <= zero.");
    Assert.IsTrue (compareResultRightLeft > 0, "Right - Left >= zero.");
  }

  [Test]
  public void CompareRowsWithCompoundColumnHavingValueTypeWithReferenceDescending_AA_BA()
  {
    BocListSortingOrderEntryMock[] sortingOrder = new BocListSortingOrderEntryMock[1];
    sortingOrder[0] = new BocListSortingOrderEntryMock (
        _typeWithReferenceFirstValueSecondValueCompoundColumn, SortingDirection.Descending);

    BocListSortingOrderProviderMock provider = new BocListSortingOrderProviderMock();
    provider.SetSortingOrder (sortingOrder);
    
    BocListRow rowWithTypeWithReferenceAA = new BocListRow (provider, 0, _referenceValueAA);
    BocListRow rowWithTypeWithReferenceBA = new BocListRow (provider, 0, _referenceValueBA);

    int compareResultLeftRight = rowWithTypeWithReferenceAA.CompareTo (rowWithTypeWithReferenceBA);
    int compareResultRightLeft = rowWithTypeWithReferenceBA.CompareTo (rowWithTypeWithReferenceAA);

    Assert.IsTrue (compareResultLeftRight > 0, "Right - Left >= zero.");
    Assert.IsTrue (compareResultRightLeft < 0, "Left - Right <= zero.");
  }

  [Test]
  public void CompareRowsWithCompoundColumnHavingValueTypeWithReferenceAscending_AA_AB()
  {
    BocListSortingOrderEntryMock[] sortingOrder = new BocListSortingOrderEntryMock[1];
    sortingOrder[0] = new BocListSortingOrderEntryMock (
        _typeWithReferenceFirstValueSecondValueCompoundColumn, SortingDirection.Ascending);

    BocListSortingOrderProviderMock provider = new BocListSortingOrderProviderMock();
    provider.SetSortingOrder (sortingOrder);
    
    BocListRow rowWithTypeWithReferenceAA = new BocListRow (provider, 0, _referenceValueAA);
    BocListRow rowWithTypeWithReferenceAB = new BocListRow (provider, 0, _referenceValueAB);

    int compareResultLeftRight = rowWithTypeWithReferenceAA.CompareTo (rowWithTypeWithReferenceAB);
    int compareResultRightLeft = rowWithTypeWithReferenceAB.CompareTo (rowWithTypeWithReferenceAA);

    Assert.IsTrue (compareResultLeftRight < 0, "Left - Right <= zero.");
    Assert.IsTrue (compareResultRightLeft > 0, "Right - Left >= zero.");
  }

  [Test]
  public void CompareRowsWithCompoundColumnHavingValueTypeWithReferenceDescending_AA_AB()
  {
    BocListSortingOrderEntryMock[] sortingOrder = new BocListSortingOrderEntryMock[1];
    sortingOrder[0] = new BocListSortingOrderEntryMock (
        _typeWithReferenceFirstValueSecondValueCompoundColumn, SortingDirection.Descending);

    BocListSortingOrderProviderMock provider = new BocListSortingOrderProviderMock();
    provider.SetSortingOrder (sortingOrder);
    
    BocListRow rowWithTypeWithReferenceAA = new BocListRow (provider, 0, _referenceValueAA);
    BocListRow rowWithTypeWithReferenceAB = new BocListRow (provider, 0, _referenceValueAB);

    int compareResultLeftRight = rowWithTypeWithReferenceAA.CompareTo (rowWithTypeWithReferenceAB);
    int compareResultRightLeft = rowWithTypeWithReferenceAB.CompareTo (rowWithTypeWithReferenceAA);

    Assert.IsTrue (compareResultLeftRight > 0, "Right - Left >= zero.");
    Assert.IsTrue (compareResultRightLeft < 0, "Left - Right <= zero.");
  }

}

}
