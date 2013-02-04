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
using Remotion.Data.DomainObjects;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;
using Remotion.Web.ExecutionEngine;

namespace Remotion.Data.UnitTests.DomainObjects.Web.WxeTransactedFunctionIntegrationTests.WxeFunctions
{
  [Serializable]
  public class DomainObjectHandleParameterTestTransactedFunction : DelegateExecutingTransactedFunction
  {
    public DomainObjectHandleParameterTestTransactedFunction (
        ITransactionMode transactionMode,
        Action<WxeContext, DomainObjectHandleParameterTestTransactedFunction> testDelegate,
        IDomainObjectHandle<ClassWithAllDataTypes> inParameter)
      : base (transactionMode, (ctx, f) => testDelegate (ctx, (DomainObjectHandleParameterTestTransactedFunction) f), inParameter)
    {
    }

    [WxeParameter (1, false, WxeParameterDirection.In)]
    public IDomainObjectHandle<ClassWithAllDataTypes> InParameter
    {
      get { return (IDomainObjectHandle<ClassWithAllDataTypes>) Variables["InParameter"]; }
      set { Variables["InParameter"] = value; }
    }

    [WxeParameter (2, false, WxeParameterDirection.Out)]
    public IDomainObjectHandle<ClassWithAllDataTypes> OutParameter
    {
      get { return (IDomainObjectHandle<ClassWithAllDataTypes>) Variables["OutParameter"]; }
      set { Variables["OutParameter"] = value; }
    }
  }
}
