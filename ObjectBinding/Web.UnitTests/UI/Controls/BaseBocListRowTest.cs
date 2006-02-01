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

  private ReflectionBusinessObjectClass _typeWithStringClass;

  private BusinessObjectPropertyPath _typeWithStringFirstValuePath;
  private BusinessObjectPropertyPath _typeWithStringSecondValuePath;

  private BocSimpleColumnDefinition _typeWithStringFirstValueSimpleColumn;
  private BocSimpleColumnDefinition _typeWithStringSecondValueSimpleColumn;

  private BocCompoundColumnDefinition _typeWithStringFirstValueFirstValueCompoundColumn;
  private BocCompoundColumnDefinition _typeWithStringFirstValueSecondValueCompoundColumn;

  [SetUp]
  public virtual void SetUp()
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
}

}
