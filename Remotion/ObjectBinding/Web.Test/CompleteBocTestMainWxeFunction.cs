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
using Remotion.Web.ExecutionEngine;
using Remotion.Web.ExecutionEngine.Infrastructure;

namespace OBWTest
{

[Serializable]
public class CompleteBocTestMainWxeFunction: WxeFunction
{
  public CompleteBocTestMainWxeFunction ()
    : base (new NoneTransactionMode ())
  {
    ReturnUrl = "StartForm.aspx";
    Variables["id"] = new Guid(0,0,0,0,0,0,0,0,0,0,1).ToString();
  }

  // steps

  private WxeStep Step1 = new WxePageStep ("CompleteBocTestForm.aspx");
  private WxeStep Step2 = new WxePageStep ("CompleteBocTestUserControlForm.aspx");
  private WxeStep Step3 = new WxePageStep ("PersonDetailsForm.aspx");
}

}
