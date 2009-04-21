// This file is part of re-strict (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
// 
// This program is free software; you can redistribute it and/or modify
// it under the terms of the GNU Affero General Public License version 3.0 
// as published by the Free Software Foundation.
// 
// This program is distributed in the hope that it will be useful, 
// but WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
// GNU Affero General Public License for more details.
// 
// You should have received a copy of the GNU Affero General Public License
// along with this program; if not, see http://www.gnu.org/licenses.
// 
// Additional permissions are listed in the file re-motion_exceptions.txt.
// 
using System;
using Remotion.Data.DomainObjects;
using Remotion.SecurityManager.Domain;
using Remotion.Web.ExecutionEngine;

namespace Remotion.SecurityManager.Clients.Web.WxeFunctions
{
  [Serializable]
  public abstract class BaseTransactedFunction : WxeFunction
  {
    protected BaseTransactedFunction ()
        : this (WxeTransactionMode.CreateRootWithAutoCommit)
    {
    }

    protected BaseTransactedFunction (ITransactionMode transactionMode, params object[] args)
        : base (transactionMode, args)
    {
      Initialize();
    }

    public ObjectID TenantID
    {
      get { return (!SecurityManagerPrincipal.Current.IsNull) ? SecurityManagerPrincipal.Current.Tenant.ID : null; }
    }

    public bool HasUserCancelled
    {
      get { return (ExceptionHandler.Exception != null && ExceptionHandler.Exception.GetType() == typeof (WxeUserCancelException)); }
    }

    protected virtual void Initialize ()
    {
      ExceptionHandler.SetCatchExceptionTypes (typeof (WxeUserCancelException));
    }
  }
}
