// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (c) rubicon IT GmbH, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// as published by the Free Software Foundation; either version 2.1 of the 
// License, or (at your option) any later version.
// 
// re-motion is distributed in the hope that it will be useful, 
// but WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
// GNU Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public License
// along with re-motion; if not, see http://www.gnu.org/licenses.
// 
using System;
using NUnit.Framework;
using Remotion.Development.Web.UnitTesting.Configuration;
using Remotion.ObjectBinding.UnitTests.Web.Domain;
using Remotion.ObjectBinding.Web.UI.Controls;

namespace Remotion.ObjectBinding.UnitTests.Web.UI.Controls.BocTextValueTests
{
  [TestFixture]
  public class Common : BocTest
  {
    private BocTextValueMock _bocTextValue;
    private TypeWithString _businessObject;
    private BusinessObjectReferenceDataSource _dataSource;
    private IBusinessObjectStringProperty _propertyStringValue;

    [SetUp]
    public override void SetUp ()
    {
      base.SetUp();
      _bocTextValue = new BocTextValueMock();
      _bocTextValue.ID = "BocTextValue";
      NamingContainer.Controls.Add (_bocTextValue);

      _businessObject = TypeWithString.Create();

      _propertyStringValue =
          (IBusinessObjectStringProperty) ((IBusinessObject) _businessObject).BusinessObjectClass.GetPropertyDefinition ("StringValue");

      _dataSource = new BusinessObjectReferenceDataSource();
      _dataSource.BusinessObject = (IBusinessObject) _businessObject;
    }


    [Test]
    public void EvaluateWaiConformityDebugLevelUndefined ()
    {
      WebConfigurationMock.Current = WebConfigurationFactory.GetDebugExceptionLevelUndefined();
      _bocTextValue.EvaluateWaiConformity();

      Assert.That (WcagHelperMock.HasWarning, Is.False);
      Assert.That (WcagHelperMock.HasError, Is.False);
    }

    [Test]
    public void EvaluateWaiConformityLevelA ()
    {
      WebConfigurationMock.Current = WebConfigurationFactory.GetLevelA();
      _bocTextValue.TextBoxStyle.AutoPostBack = true;
      _bocTextValue.EvaluateWaiConformity();

      Assert.That (WcagHelperMock.HasWarning, Is.False);
      Assert.That (WcagHelperMock.HasError, Is.False);
    }


    [Test]
    public void EvaluateWaiConformityDebugLevelAWithTextBoxStyleAutoPostBackTrue ()
    {
      WebConfigurationMock.Current = WebConfigurationFactory.GetDebugExceptionLevelA();
      _bocTextValue.TextBoxStyle.AutoPostBack = true;
      _bocTextValue.EvaluateWaiConformity();

      Assert.That (WcagHelperMock.HasWarning, Is.True);
      Assert.That (WcagHelperMock.Priority, Is.EqualTo (1));
      Assert.That (WcagHelperMock.Control, Is.SameAs (_bocTextValue));
      Assert.That (WcagHelperMock.Property, Is.EqualTo ("TextBoxStyle.AutoPostBack"));
    }

    [Test]
    public void GetTrackedClientIDsInReadOnlyMode ()
    {
      _bocTextValue.ReadOnly = true;
      string[] actual = _bocTextValue.GetTrackedClientIDs();
      Assert.That (actual, Is.Not.Null);
      Assert.That (actual.Length, Is.EqualTo (0));
    }

    [Test]
    public void GetTrackedClientIDsInEditMode ()
    {
      _bocTextValue.ReadOnly = false;
      string[] actual = _bocTextValue.GetTrackedClientIDs();
      Assert.That (actual, Is.Not.Null);
      Assert.That (actual.Length, Is.EqualTo (1));
      Assert.That (actual[0], Is.EqualTo (_bocTextValue.GetTextBoxClientID()));
    }


    [Test]
    public void SetValueToString ()
    {
      string value = "Foo Bar";
      _bocTextValue.IsDirty = false;
      _bocTextValue.Value = value;
      Assert.That (_bocTextValue.Value, Is.EqualTo (value));
      Assert.That (_bocTextValue.IsDirty, Is.True);
    }

    [Test]
    public void SetValueToNull ()
    {
      _bocTextValue.IsDirty = false;
      _bocTextValue.Value = null;
      Assert.That (_bocTextValue.Value, Is.EqualTo (null));
      Assert.That (_bocTextValue.IsDirty, Is.True);
    }


    [Test]
    public void HasValue_ValueIsSet_ReturnsTrue ()
    {
      _bocTextValue.Value = "x";
      Assert.That (_bocTextValue.HasValue, Is.True);
    }

    [Test]
    public void HasValue_TextContainsInvaliData_ReturnsTrue ()
    {
      _bocTextValue.ValueType = BocTextValueType.Date;
      _bocTextValue.Text = "x";
      Assert.That (_bocTextValue.HasValue, Is.True);
    }

    [Test]
    public void HasValue_TextContainsOnlyWhitespace_ReturnsFalse ()
    {
      _bocTextValue.ValueType = BocTextValueType.Date;
      _bocTextValue.Text = "  ";
      Assert.That (_bocTextValue.HasValue, Is.False);
      Assert.That (_bocTextValue.Value, Is.Null);
    }

    [Test]
    public void HasValue_ValueIsNull_ReturnsFalse ()
    {
      _bocTextValue.Value = null;
      Assert.That (_bocTextValue.HasValue, Is.False);
    }


    [Test]
    public void LoadValueAndInterimTrue ()
    {
      _businessObject.StringValue = "Foo Bar";
      _bocTextValue.DataSource = _dataSource;
      _bocTextValue.Property = _propertyStringValue;
      _bocTextValue.Value = null;
      _bocTextValue.IsDirty = true;

      _bocTextValue.LoadValue (true);
      Assert.That (_bocTextValue.Value, Is.EqualTo (null));
      Assert.That (_bocTextValue.IsDirty, Is.True);
    }

    [Test]
    public void LoadValueAndInterimFalseWithString ()
    {
      _businessObject.StringValue = "Foo Bar";
      _bocTextValue.DataSource = _dataSource;
      _bocTextValue.Property = _propertyStringValue;
      _bocTextValue.Value = null;
      _bocTextValue.IsDirty = true;

      _bocTextValue.LoadValue (false);
      Assert.That (_bocTextValue.Value, Is.EqualTo (_businessObject.StringValue));
      Assert.That (_bocTextValue.IsDirty, Is.False);
    }

    [Test]
    public void LoadValueAndInterimFalseWithNull ()
    {
      _businessObject.StringValue = null;
      _bocTextValue.DataSource = _dataSource;
      _bocTextValue.Property = _propertyStringValue;
      _bocTextValue.Value = "Foo Bar";
      _bocTextValue.IsDirty = true;

      _bocTextValue.LoadValue (false);
      Assert.That (_bocTextValue.Value, Is.EqualTo (_businessObject.StringValue));
      Assert.That (_bocTextValue.IsDirty, Is.False);
    }

    [Test]
    public void LoadValueAndInterimFalseWithDataSourceNull ()
    {
      _bocTextValue.DataSource = null;
      _bocTextValue.Property = _propertyStringValue;
      _bocTextValue.Value = "Foo Bar";
      _bocTextValue.IsDirty = true;

      _bocTextValue.LoadValue (false);
      Assert.That (_bocTextValue.Value, Is.EqualTo ("Foo Bar"));
      Assert.That (_bocTextValue.IsDirty, Is.True);
    }

    [Test]
    public void LoadValueAndInterimFalseWithPropertyNull ()
    {
      _bocTextValue.DataSource = _dataSource;
      _bocTextValue.Property = null;
      _bocTextValue.Value = "Foo Bar";
      _bocTextValue.IsDirty = true;

      _bocTextValue.LoadValue (false);
      Assert.That (_bocTextValue.Value, Is.EqualTo ("Foo Bar"));
      Assert.That (_bocTextValue.IsDirty, Is.True);
    }

    [Test]
    public void LoadValueAndInterimFalseWithDataSourceBusinessObjectNull ()
    {
      _dataSource.BusinessObject = null;
      _bocTextValue.DataSource = _dataSource;
      _bocTextValue.Property = _propertyStringValue;
      _bocTextValue.Value = "Foo Bar";
      _bocTextValue.IsDirty = true;

      _bocTextValue.LoadValue (false);
      Assert.That (_bocTextValue.Value, Is.EqualTo (null));
      Assert.That (_bocTextValue.IsDirty, Is.False);
    }


    [Test]
    public void LoadUnboundValueAndInterimTrue ()
    {
      string value = "Foo Bar";
      _bocTextValue.Value = null;
      _bocTextValue.IsDirty = true;

      _bocTextValue.LoadUnboundValue (value, true);
      Assert.That (_bocTextValue.Value, Is.EqualTo (null));
      Assert.That (_bocTextValue.IsDirty, Is.True);
    }

    [Test]
    public void LoadUnboundValueAndInterimFalseWithString ()
    {
      string value = "Foo Bar";
      _bocTextValue.Value = null;
      _bocTextValue.IsDirty = true;

      _bocTextValue.LoadUnboundValue (value, false);
      Assert.That (_bocTextValue.Value, Is.EqualTo (value));
      Assert.That (_bocTextValue.IsDirty, Is.False);
    }

    [Test]
    public void LoadUnboundValueAndInterimFalseWithNull ()
    {
      string value = null;
      _bocTextValue.Value = "Foo Bar";
      _bocTextValue.IsDirty = true;

      _bocTextValue.LoadUnboundValue (value, false);
      Assert.That (_bocTextValue.Value, Is.EqualTo (value));
      Assert.That (_bocTextValue.IsDirty, Is.False);
    }

    [Test]
    public void SaveValueAndInterimTrue ()
    {
      _businessObject.StringValue = "Foo Bar";
      _bocTextValue.DataSource = _dataSource;
      _bocTextValue.Property = _propertyStringValue;
      _bocTextValue.Value = null;
      _bocTextValue.IsDirty = true;

      _bocTextValue.SaveValue (true);
      Assert.That (_businessObject.StringValue, Is.EqualTo ("Foo Bar"));
      Assert.That (_bocTextValue.IsDirty, Is.True);
    }

    [Test]
    public void SaveValueAndInterimFalse ()
    {
      _businessObject.StringValue = "Foo Bar";
      _bocTextValue.DataSource = _dataSource;
      _bocTextValue.Property = _propertyStringValue;
      _bocTextValue.Value = null;
      _bocTextValue.IsDirty = true;

      _bocTextValue.SaveValue (false);
      Assert.That (_businessObject.StringValue, Is.EqualTo (null));
      Assert.That (_bocTextValue.IsDirty, Is.False);
    }

    [Test]
    public void SaveValueAndIsDirtyFalse ()
    {
      _businessObject.StringValue = "Foo Bar";
      _bocTextValue.DataSource = _dataSource;
      _bocTextValue.Property = _propertyStringValue;
      _bocTextValue.Value = null;
      _bocTextValue.IsDirty = false;

      _bocTextValue.SaveValue (false);
      Assert.That (_businessObject.StringValue, Is.EqualTo ("Foo Bar"));
      Assert.That (_bocTextValue.IsDirty, Is.False);
    }
  }
}
