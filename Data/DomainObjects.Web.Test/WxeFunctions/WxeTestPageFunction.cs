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

using Remotion.Utilities;
using Remotion.Web.ExecutionEngine;

namespace Remotion.Data.DomainObjects.Web.Test.WxeFunctions
{
[Serializable]
public class WxeTestPageFunction : WxeFunction
{
  // types

  // static members and constants

  // member fields

  // construction and disposing

  public WxeTestPageFunction ()
    :base (WxeTransactionMode.CreateRoot)
  {
  }

  // methods and properties

  public ClientTransaction CurrentClientTransaction
  {
    get { return (ClientTransaction) Variables["CurrentClientTransaction"]; }
    set { Variables["CurrentClientTransaction"] = value;}
  }

  private WxePageStep Step1 = new WxePageStep ("WxeTestPage.aspx");
}
}
