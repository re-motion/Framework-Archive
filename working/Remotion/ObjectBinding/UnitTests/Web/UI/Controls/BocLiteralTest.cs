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
using Remotion.ObjectBinding.UnitTests.Web.Domain;
using Remotion.ObjectBinding.Web.UI.Controls;

namespace Remotion.ObjectBinding.UnitTests.Web.UI.Controls
{
  [TestFixture]
  public class BocLiteralTest : BocTest
  {
    private BocLiteral _bocLiteral;
    private TypeWithString _businessObject;
    private BusinessObjectReferenceDataSource _dataSource;
    private IBusinessObjectStringProperty _propertyStringValue;

    [SetUp]
    public override void SetUp ()
    {
      base.SetUp ();
      _bocLiteral = new BocLiteral ();
      _bocLiteral.ID = "BocTextValue";
      NamingContainer.Controls.Add (_bocLiteral);

      _businessObject = TypeWithString.Create ();

      _propertyStringValue =
          (IBusinessObjectStringProperty) ((IBusinessObject) _businessObject).BusinessObjectClass.GetPropertyDefinition ("StringValue");

      _dataSource = new BusinessObjectReferenceDataSource ();
      _dataSource.BusinessObject = (IBusinessObject) _businessObject;
    }


    [Test]
    public void HasValue_ValueIsSet_ReturnsTrue ()
    {
      _bocLiteral.Value = "x";
      Assert.IsTrue (_bocLiteral.HasValue);
    }

    [Test]
    public void HasValue_TextContainsOnlyWhitespace_ReturnsFalse ()
    {
      _bocLiteral.Value = "  ";
      Assert.IsFalse (_bocLiteral.HasValue);
    }

    [Test]
    public void HasValue_ValueIsNull_ReturnsFalse ()
    {
      _bocLiteral.Value = null;
      Assert.IsFalse (_bocLiteral.HasValue);
    }


    [Test]
    public void LoadValueAndInterimTrue ()
    {
      _businessObject.StringValue = "Foo Bar";
      _bocLiteral.DataSource = _dataSource;
      _bocLiteral.Property = _propertyStringValue;
      _bocLiteral.Value = null;

      _bocLiteral.LoadValue (true);
      Assert.AreEqual ("Foo Bar", _bocLiteral.Value);
    }

    [Test]
    public void LoadValueAndInterimFalseWithString ()
    {
      _businessObject.StringValue = "Foo Bar";
      _bocLiteral.DataSource = _dataSource;
      _bocLiteral.Property = _propertyStringValue;
      _bocLiteral.Value = null;

      _bocLiteral.LoadValue (false);
      Assert.AreEqual (_businessObject.StringValue, _bocLiteral.Value);
    }

    [Test]
    public void LoadValueAndInterimFalseWithNull ()
    {
      _businessObject.StringValue = null;
      _bocLiteral.DataSource = _dataSource;
      _bocLiteral.Property = _propertyStringValue;
      _bocLiteral.Value = "Foo Bar";

      _bocLiteral.LoadValue (false);
      Assert.AreEqual (_businessObject.StringValue, _bocLiteral.Value);
    }

    [Test]
    public void LoadValueAndInterimFalseWithDataSourceNull ()
    {
      _bocLiteral.DataSource = null;
      _bocLiteral.Property = _propertyStringValue;
      _bocLiteral.Value = "Foo Bar";

      _bocLiteral.LoadValue (false);
      Assert.AreEqual ("Foo Bar", _bocLiteral.Value);
    }

    [Test]
    public void LoadValueAndInterimFalseWithPropertyNull ()
    {
      _bocLiteral.DataSource = _dataSource;
      _bocLiteral.Property = null;
      _bocLiteral.Value = "Foo Bar";

      _bocLiteral.LoadValue (false);
      Assert.AreEqual ("Foo Bar", _bocLiteral.Value);
    }

    [Test]
    public void LoadValueAndInterimFalseWithDataSourceBusinessObjectNull ()
    {
      _dataSource.BusinessObject = null;
      _bocLiteral.DataSource = _dataSource;
      _bocLiteral.Property = _propertyStringValue;
      _bocLiteral.Value = "Foo Bar";

      _bocLiteral.LoadValue (false);
      Assert.AreEqual (null, _bocLiteral.Value);
    }


    [Test]
    public void LoadUnboundValueAndInterimTrue ()
    {
      string value = "Foo Bar";
      _bocLiteral.Value = null;

      _bocLiteral.LoadUnboundValue (value, true);
      Assert.AreEqual ("Foo Bar", _bocLiteral.Value);
    }

    [Test]
    public void LoadUnboundValueAndInterimFalseWithString ()
    {
      string value = "Foo Bar";
      _bocLiteral.Value = null;

      _bocLiteral.LoadUnboundValue (value, false);
      Assert.AreEqual (value, _bocLiteral.Value);
    }

    [Test]
    public void LoadUnboundValueAndInterimFalseWithNull ()
    {
      string value = null;
      _bocLiteral.Value = "Foo Bar";

      _bocLiteral.LoadUnboundValue (value, false);
      Assert.AreEqual (value, _bocLiteral.Value);
    }
  }
}