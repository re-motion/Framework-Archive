// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// version 3.0 as published by the Free Software Foundation.
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
using Remotion.Data.DomainObjects;

namespace Remotion.Data.UnitTests.DomainObjects.TestDomain
{
  [Serializable]
  [DBTable]
  [TestDomain]
  [Instantiable]
  public abstract class Computer : TestDomainBase
  {
    public static Computer NewObject ()
    {
      return NewObject<Computer> ();
    }

    public new static Computer GetObject (ObjectID id)
    {
      return GetObject<Computer> (id);
    }

    protected Computer ()
    {
    }

    [StringProperty (IsNullable = false, MaximumLength = 20)]
    public abstract string SerialNumber { get; set; }

    [DBBidirectionalRelation ("Computer", ContainsForeignKey = true)]
    public abstract Employee Employee { get; set; }

    [StorageClassTransaction]
    public abstract int Int32TransactionProperty { get; set; }

    [StorageClassTransaction]
    public abstract DateTime DateTimeTransactionProperty { get; set; }

    [StorageClassTransaction]
    [DBBidirectionalRelation ("ComputerTransactionProperty", ContainsForeignKey = true)]
    public abstract Employee EmployeeTransactionProperty { get; set; }
  }
}
