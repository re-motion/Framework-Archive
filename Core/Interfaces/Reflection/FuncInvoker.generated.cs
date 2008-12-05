//------------------------------------------------------------------------------
// This code was generated by a tool.
//
// Changes to this file may cause incorrect behavior and will be lost if 
// the code is regenerated.
//
//------------------------------------------------------------------------------
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

namespace Remotion.Reflection
{
  public partial interface IFuncInvoker<TResult>
  {


    TResult With ();

    TResult With<A1, A2> (A1 a1, A2 a2);

    TResult With<A1, A2, A3> (A1 a1, A2 a2, A3 a3);

    TResult With<A1, A2, A3, A4> (A1 a1, A2 a2, A3 a3, A4 a4);

    TResult With<A1, A2, A3, A4, A5> (A1 a1, A2 a2, A3 a3, A4 a4, A5 a5);

    TResult With<A1, A2, A3, A4, A5, A6> (A1 a1, A2 a2, A3 a3, A4 a4, A5 a5, A6 a6);

    TResult With<A1, A2, A3, A4, A5, A6, A7> (A1 a1, A2 a2, A3 a3, A4 a4, A5 a5, A6 a6, A7 a7);

    TResult With<A1, A2, A3, A4, A5, A6, A7, A8> (A1 a1, A2 a2, A3 a3, A4 a4, A5 a5, A6 a6, A7 a7, A8 a8);

    TResult With<A1, A2, A3, A4, A5, A6, A7, A8, A9> (A1 a1, A2 a2, A3 a3, A4 a4, A5 a5, A6 a6, A7 a7, A8 a8, A9 a9);

    TResult With<A1, A2, A3, A4, A5, A6, A7, A8, A9, A10> (A1 a1, A2 a2, A3 a3, A4 a4, A5 a5, A6 a6, A7 a7, A8 a8, A9 a9, A10 a10);

    TResult With<A1, A2, A3, A4, A5, A6, A7, A8, A9, A10, A11> (A1 a1, A2 a2, A3 a3, A4 a4, A5 a5, A6 a6, A7 a7, A8 a8, A9 a9, A10 a10, A11 a11);

    TResult With<A1, A2, A3, A4, A5, A6, A7, A8, A9, A10, A11, A12> (A1 a1, A2 a2, A3 a3, A4 a4, A5 a5, A6 a6, A7 a7, A8 a8, A9 a9, A10 a10, A11 a11, A12 a12);

    TResult With<A1, A2, A3, A4, A5, A6, A7, A8, A9, A10, A11, A12, A13> (A1 a1, A2 a2, A3 a3, A4 a4, A5 a5, A6 a6, A7 a7, A8 a8, A9 a9, A10 a10, A11 a11, A12 a12, A13 a13);

    TResult With<A1, A2, A3, A4, A5, A6, A7, A8, A9, A10, A11, A12, A13, A14> (A1 a1, A2 a2, A3 a3, A4 a4, A5 a5, A6 a6, A7 a7, A8 a8, A9 a9, A10 a10, A11 a11, A12 a12, A13 a13, A14 a14);

    TResult With<A1, A2, A3, A4, A5, A6, A7, A8, A9, A10, A11, A12, A13, A14, A15> (A1 a1, A2 a2, A3 a3, A4 a4, A5 a5, A6 a6, A7 a7, A8 a8, A9 a9, A10 a10, A11 a11, A12 a12, A13 a13, A14 a14, A15 a15);

    TResult With<A1, A2, A3, A4, A5, A6, A7, A8, A9, A10, A11, A12, A13, A14, A15, A16> (A1 a1, A2 a2, A3 a3, A4 a4, A5 a5, A6 a6, A7 a7, A8 a8, A9 a9, A10 a10, A11 a11, A12 a12, A13 a13, A14 a14, A15 a15, A16 a16);

    TResult With<A1, A2, A3, A4, A5, A6, A7, A8, A9, A10, A11, A12, A13, A14, A15, A16, A17> (A1 a1, A2 a2, A3 a3, A4 a4, A5 a5, A6 a6, A7 a7, A8 a8, A9 a9, A10 a10, A11 a11, A12 a12, A13 a13, A14 a14, A15 a15, A16 a16, A17 a17);
  }

  public partial struct FuncInvokerWrapper<TResult> : IFuncInvoker<TResult>
  {


    public TResult With ()
    {
      return PerformAfterAction (_invoker.With ());
    }


    public TResult With<A1, A2> (A1 a1, A2 a2)
    {
      return PerformAfterAction (_invoker.With (a1, a2));
    }


    public TResult With<A1, A2, A3> (A1 a1, A2 a2, A3 a3)
    {
      return PerformAfterAction (_invoker.With (a1, a2, a3));
    }


    public TResult With<A1, A2, A3, A4> (A1 a1, A2 a2, A3 a3, A4 a4)
    {
      return PerformAfterAction (_invoker.With (a1, a2, a3, a4));
    }


    public TResult With<A1, A2, A3, A4, A5> (A1 a1, A2 a2, A3 a3, A4 a4, A5 a5)
    {
      return PerformAfterAction (_invoker.With (a1, a2, a3, a4, a5));
    }


    public TResult With<A1, A2, A3, A4, A5, A6> (A1 a1, A2 a2, A3 a3, A4 a4, A5 a5, A6 a6)
    {
      return PerformAfterAction (_invoker.With (a1, a2, a3, a4, a5, a6));
    }


    public TResult With<A1, A2, A3, A4, A5, A6, A7> (A1 a1, A2 a2, A3 a3, A4 a4, A5 a5, A6 a6, A7 a7)
    {
      return PerformAfterAction (_invoker.With (a1, a2, a3, a4, a5, a6, a7));
    }


    public TResult With<A1, A2, A3, A4, A5, A6, A7, A8> (A1 a1, A2 a2, A3 a3, A4 a4, A5 a5, A6 a6, A7 a7, A8 a8)
    {
      return PerformAfterAction (_invoker.With (a1, a2, a3, a4, a5, a6, a7, a8));
    }


    public TResult With<A1, A2, A3, A4, A5, A6, A7, A8, A9> (A1 a1, A2 a2, A3 a3, A4 a4, A5 a5, A6 a6, A7 a7, A8 a8, A9 a9)
    {
      return PerformAfterAction (_invoker.With (a1, a2, a3, a4, a5, a6, a7, a8, a9));
    }


    public TResult With<A1, A2, A3, A4, A5, A6, A7, A8, A9, A10> (A1 a1, A2 a2, A3 a3, A4 a4, A5 a5, A6 a6, A7 a7, A8 a8, A9 a9, A10 a10)
    {
      return PerformAfterAction (_invoker.With (a1, a2, a3, a4, a5, a6, a7, a8, a9, a10));
    }


    public TResult With<A1, A2, A3, A4, A5, A6, A7, A8, A9, A10, A11> (A1 a1, A2 a2, A3 a3, A4 a4, A5 a5, A6 a6, A7 a7, A8 a8, A9 a9, A10 a10, A11 a11)
    {
      return PerformAfterAction (_invoker.With (a1, a2, a3, a4, a5, a6, a7, a8, a9, a10, a11));
    }


    public TResult With<A1, A2, A3, A4, A5, A6, A7, A8, A9, A10, A11, A12> (A1 a1, A2 a2, A3 a3, A4 a4, A5 a5, A6 a6, A7 a7, A8 a8, A9 a9, A10 a10, A11 a11, A12 a12)
    {
      return PerformAfterAction (_invoker.With (a1, a2, a3, a4, a5, a6, a7, a8, a9, a10, a11, a12));
    }


    public TResult With<A1, A2, A3, A4, A5, A6, A7, A8, A9, A10, A11, A12, A13> (A1 a1, A2 a2, A3 a3, A4 a4, A5 a5, A6 a6, A7 a7, A8 a8, A9 a9, A10 a10, A11 a11, A12 a12, A13 a13)
    {
      return PerformAfterAction (_invoker.With (a1, a2, a3, a4, a5, a6, a7, a8, a9, a10, a11, a12, a13));
    }


    public TResult With<A1, A2, A3, A4, A5, A6, A7, A8, A9, A10, A11, A12, A13, A14> (A1 a1, A2 a2, A3 a3, A4 a4, A5 a5, A6 a6, A7 a7, A8 a8, A9 a9, A10 a10, A11 a11, A12 a12, A13 a13, A14 a14)
    {
      return PerformAfterAction (_invoker.With (a1, a2, a3, a4, a5, a6, a7, a8, a9, a10, a11, a12, a13, a14));
    }


    public TResult With<A1, A2, A3, A4, A5, A6, A7, A8, A9, A10, A11, A12, A13, A14, A15> (A1 a1, A2 a2, A3 a3, A4 a4, A5 a5, A6 a6, A7 a7, A8 a8, A9 a9, A10 a10, A11 a11, A12 a12, A13 a13, A14 a14, A15 a15)
    {
      return PerformAfterAction (_invoker.With (a1, a2, a3, a4, a5, a6, a7, a8, a9, a10, a11, a12, a13, a14, a15));
    }


    public TResult With<A1, A2, A3, A4, A5, A6, A7, A8, A9, A10, A11, A12, A13, A14, A15, A16> (A1 a1, A2 a2, A3 a3, A4 a4, A5 a5, A6 a6, A7 a7, A8 a8, A9 a9, A10 a10, A11 a11, A12 a12, A13 a13, A14 a14, A15 a15, A16 a16)
    {
      return PerformAfterAction (_invoker.With (a1, a2, a3, a4, a5, a6, a7, a8, a9, a10, a11, a12, a13, a14, a15, a16));
    }


    public TResult With<A1, A2, A3, A4, A5, A6, A7, A8, A9, A10, A11, A12, A13, A14, A15, A16, A17> (A1 a1, A2 a2, A3 a3, A4 a4, A5 a5, A6 a6, A7 a7, A8 a8, A9 a9, A10 a10, A11 a11, A12 a12, A13 a13, A14 a14, A15 a15, A16 a16, A17 a17)
    {
      return PerformAfterAction (_invoker.With (a1, a2, a3, a4, a5, a6, a7, a8, a9, a10, a11, a12, a13, a14, a15, a16, a17));
    }

  }
}
