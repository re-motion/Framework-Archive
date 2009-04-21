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
using Remotion.Data.UnitTests.DomainObjects.TestDomain;
using Remotion.Utilities;
using Remotion.Web.ExecutionEngine;
using Remotion.Web.ExecutionEngine.Infrastructure;

namespace Remotion.Data.UnitTests.DomainObjects.Web.WxeFunctions
{
  [Serializable]
  public class NoAutoCommitTestTransactedFunction : WxeFunction
  {
    // types

    // static members and constants

    // member fields

    // construction and disposing

    public NoAutoCommitTestTransactedFunction (ITransactionMode transactionMode, ObjectID objectWithAllDataTypes)
        : base (transactionMode, objectWithAllDataTypes)
    {
      Assertion.IsFalse (TransactionMode.AutoCommit);
    }

    // methods and properties

    [WxeParameter (1, true, WxeParameterDirection.In)]
    public ObjectID ObjectWithAllDataTypes
    {
      get { return (ObjectID) Variables["ObjectWithAllDataTypes"]; }
      set { Variables["ObjectWithAllDataTypes"] = value; }
    }

    private void Step1 ()
    {
      ClassWithAllDataTypes objectWithAllDataTypes = ClassWithAllDataTypes.GetObject (ObjectWithAllDataTypes);

      objectWithAllDataTypes.Int32Property = 10;
    }
  }
}
