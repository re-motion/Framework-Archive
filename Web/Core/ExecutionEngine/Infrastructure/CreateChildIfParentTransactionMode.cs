/* Copyright (C) 2005 - 2008 rubicon informationstechnologie gmbh
 *
 * This program is free software: you can redistribute it and/or modify it under 
 * the terms of the re:motion license agreement in license.txt. If you did not 
 * receive it, please visit http://www.re-motion.org/licensing.
 * 
 * Unless otherwise provided, this software is distributed on an "AS IS" basis, 
 * WITHOUT WARRANTY OF ANY KIND, either express or implied. 
 */

using System;
using Remotion.Data;
using Remotion.Utilities;

namespace Remotion.Web.ExecutionEngine.Infrastructure
{
  //TODO: Doc
  [Serializable]
  public class CreateChildIfParentTransactionMode<TTransactionFactory> : ITransactionMode
      where TTransactionFactory: ITransactionFactory, new()
  {
    private readonly bool _autoCommit;

    public CreateChildIfParentTransactionMode (bool autoCommit)
    {
      _autoCommit = autoCommit;
    }

    public virtual TransactionStrategyBase CreateTransactionStrategy (WxeFunction2 function)
    {
      ArgumentUtility.CheckNotNull ("function", function);

      var transactionFactory = new TTransactionFactory();
      return new RootTransactionStrategy (_autoCommit, transactionFactory.CreateRootTransaction(), NullTransactionStrategy.Null, function);
    }

    public bool AutoCommit
    {
      get { return _autoCommit; }
    }
  }
}