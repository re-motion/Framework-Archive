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
using Remotion.ObjectBinding.Web.UI.Controls;
using Remotion.ObjectBinding.Web.UI.Controls.BocListImplementation;
using Rhino.Mocks;

namespace Remotion.ObjectBinding.UnitTests.Web.UI.Controls.BocListImplementation
{
  [TestFixture]
  public class UniqueIdentifierBasedRowIDProviderTest
  {
    [Test]
    public void GetControlRowD ()
    {
      var rowIDProvider = new UniqueIdentifierBasedRowIDProvider();
      Assert.That (rowIDProvider.GetControlRowID (new BocListRow (5, CreateObject ("a"))), Is.EqualTo ("a"));
    }

    [Test]
    public void GetItemRowD ()
    {
      var rowIDProvider = new UniqueIdentifierBasedRowIDProvider();
      Assert.That (rowIDProvider.GetItemRowID (new BocListRow (3, CreateObject ("a"))), Is.EqualTo ("3_a"));
    }

    [Test]
    public void GetRowFromItemRowID_IndexMatches ()
    {
      var values = new[]
                   {
                       CreateObject ("a"),
                       CreateObject ("b"),
                       CreateObject ("c")
                   };
      var rowIDProvider = new UniqueIdentifierBasedRowIDProvider();

      var row = rowIDProvider.GetRowFromItemRowID (values, "1_b");

      Assert.That (row.Index, Is.EqualTo (1));
      Assert.That (row.BusinessObject, Is.SameAs (values[1]));
    }

    [Test]
    public void GetRowFromItemRowID_IndexDoesNotMatches_IndexTooBig ()
    {
      var values = new[]
                   {
                       CreateObject ("a"),
                       CreateObject ("b"),
                       CreateObject ("c"),
                       CreateObject ("d"),
                       CreateObject ("e"),
                       CreateObject ("f"),
                       CreateObject ("g"),
                       CreateObject ("h")
                   };
      var rowIDProvider = new UniqueIdentifierBasedRowIDProvider();

      var row = rowIDProvider.GetRowFromItemRowID (values, "5_b");

      Assert.That (row.Index, Is.EqualTo (1));
      Assert.That (row.BusinessObject, Is.SameAs (values[1]));
    }

    [Test]
    public void GetRowFromItemRowID_IndexDoesNotMatches_IndexTooSmall ()
    {
      var values = new[]
                   {
                       CreateObject ("a"),
                       CreateObject ("b"),
                       CreateObject ("c"),
                       CreateObject ("d"),
                       CreateObject ("e"),
                       CreateObject ("f"),
                       CreateObject ("g"),
                       CreateObject ("h")
                   };
      var rowIDProvider = new UniqueIdentifierBasedRowIDProvider();

      var row = rowIDProvider.GetRowFromItemRowID (values, "2_g");

      Assert.That (row.Index, Is.EqualTo (6));
      Assert.That (row.BusinessObject, Is.SameAs (values[6]));
    }

    [Test]
    public void GetRowFromItemRowID_IndexGreaterThanValueLength ()
    {
      var values = new[]
                   {
                       CreateObject ("a"),
                       CreateObject ("b")
                   };
      var rowIDProvider = new UniqueIdentifierBasedRowIDProvider();

      var row = rowIDProvider.GetRowFromItemRowID (values, "2_b");

      Assert.That (row.Index, Is.EqualTo (1));
      Assert.That (row.BusinessObject, Is.SameAs (values[1]));
    }

    [Test]
    public void GetRowFromItemRowID_ItemNotInValueList ()
    {
      var rowIDProvider = new UniqueIdentifierBasedRowIDProvider();
      var values = new[]
                   {
                       CreateObject ("a"),
                       CreateObject ("c"),
                       CreateObject ("b")
                   };

      var row = rowIDProvider.GetRowFromItemRowID (values, "1_d");

      Assert.That (row, Is.Null);
    }

    [Test]
    public void GetRowFromItemRowID_InvalidRowIDFormat_ThrowsFormatException ()
    {
      var rowIDProvider = new UniqueIdentifierBasedRowIDProvider ();

      Assert.That (
          () => rowIDProvider.GetRowFromItemRowID (new IBusinessObject[0], "x"),
          Throws.TypeOf<FormatException>().With.Message.EqualTo ("RowID 'x' could not be parsed. Expected format: '<rowIndex>_<unqiueIdentifier>'"));
    }

    [Test]
    public void GetRowFromItemRowID_InvalidRowIndexFormat_ThrowsFormatException ()
    {
      var rowIDProvider = new UniqueIdentifierBasedRowIDProvider ();

      Assert.That (
          () => rowIDProvider.GetRowFromItemRowID (new IBusinessObject[0], "a_x"),
          Throws.TypeOf<FormatException>().With.Message.EqualTo ("RowID 'a_x' could not be parsed. Expected format: '<rowIndex>_<unqiueIdentifier>'"));
    }

    [Test]
    public void AddRow_HasNoEffect ()
    {
      var rowIDProvider = new UniqueIdentifierBasedRowIDProvider();

      rowIDProvider.AddRow (new BocListRow (3, CreateObject ("a")));

      Assert.That (rowIDProvider.GetItemRowID (new BocListRow (3, CreateObject ("a"))), Is.EqualTo ("3_a"));
      Assert.That (rowIDProvider.GetItemRowID (new BocListRow (4, CreateObject ("a"))), Is.EqualTo ("4_a"));
      Assert.That (rowIDProvider.GetItemRowID (new BocListRow (3, CreateObject ("b"))), Is.EqualTo ("3_b"));
    }

    [Test]
    public void RemoveRow_HasNoEffect ()
    {
      var rowIDProvider = new UniqueIdentifierBasedRowIDProvider();

      rowIDProvider.RemoveRow (new BocListRow (3, CreateObject ("a")));

      Assert.That (rowIDProvider.GetItemRowID (new BocListRow (3, CreateObject ("a"))), Is.EqualTo ("3_a"));
    }

    [Test]
    public void GetItemRowID_GetRowFromItemRowID ()
    {
      var rowIDProvider = new IndexBasedRowIDProvider (new IBusinessObject[3]);
      var rowID = rowIDProvider.GetItemRowID (new BocListRow (1, CreateObject("b")));

      var values = new[]
                   {
                       CreateObject ("a"),
                       CreateObject ("b"),
                       CreateObject ("c")
                   };

      var row = rowIDProvider.GetRowFromItemRowID (values, rowID);
      Assert.That (row.Index, Is.EqualTo (1));
      Assert.That (row.BusinessObject, Is.SameAs (values[1]));
    }

    private IBusinessObject CreateObject (string id)
    {
      var obj = MockRepository.GenerateStub<IBusinessObjectWithIdentity>();
      obj.Stub (_ => _.UniqueIdentifier).Return (id);

      return obj;
    }
  }
}