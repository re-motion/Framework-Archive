// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2008 rubicon informationstechnologie gmbh, www.rubicon.eu
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
using Remotion.Data;

namespace Remotion.Web.UnitTests.ExecutionEngine.TestFunctions
{
  public class TestTransactionScope : ITransactionScope
  {
    private static TestTransactionScope _currentScope;

    private readonly TestTransaction _scopedTransaction;
    private readonly TestTransactionScope _previousScope;
    private bool _left = false;

    public TestTransactionScope (TestTransaction scopedTransaction)
    {
      _scopedTransaction = scopedTransaction;
      _previousScope = _currentScope;
      CurrentScope = this;
    }

    public TestTransaction ScopedTransaction
    {
      get { return _scopedTransaction; }
    }

    public static TestTransactionScope CurrentScope
    {
      get { return _currentScope; }
      set
      {
        _currentScope = value;
        TestTransaction.Current = value != null ? value.ScopedTransaction : null;
      }
    }

    ITransaction ITransactionScope.ScopedTransaction
    {
      get { return ScopedTransaction; }
    }

    bool ITransactionScope.IsActiveScope
    {
      get { return CurrentScope == this; }
    }

    public void Leave ()
    {
      if (_left)
        throw new InvalidOperationException ("Has already been left.");
      CurrentScope = _previousScope;
      _left = true;
    }
  }
}
