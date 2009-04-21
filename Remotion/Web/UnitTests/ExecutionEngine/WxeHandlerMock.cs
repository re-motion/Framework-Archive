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
using System.Web;
using Remotion.Web.ExecutionEngine;

namespace Remotion.Web.UnitTests.ExecutionEngine
{

/// <summary> Exposes non-public members of the <see cref="WxeHandler"/> type. </summary>
public class WxeHandlerMock: WxeHandler
{
  public new void CheckTimeoutConfiguration (HttpContext context)
  {
    base.CheckTimeoutConfiguration (context);
  }

  public new Type GetTypeByPath (string path)
  {
    return base.GetTypeByPath (path);
  }

  public new Type GetType (HttpContext context)
  {
    return base.GetType (context);
  }

  public new Type GetTypeByTypeName (string typeName)
  {
    return base.GetTypeByTypeName (typeName);
  }

  public new WxeFunctionState CreateNewFunctionState (HttpContext context, Type type)
  {
    return base.CreateNewFunctionState (context, type);
  }

  public new WxeFunctionState ResumeExistingFunctionState (HttpContext context, string functionToken)
  {
    return base.ResumeExistingFunctionState (context, functionToken);
  }

  public new void ProcessFunctionState (HttpContext context, WxeFunctionState functionState, bool isNewFunction)
  {
    base.ProcessFunctionState (context, functionState, isNewFunction);
  }

  public new void ExecuteFunctionState (HttpContext context, WxeFunctionState functionState, bool isNewFunction)
  {
    base.ExecuteFunctionState (context, functionState, isNewFunction);
  }

  public new virtual void ExecuteFunction (WxeFunction function, WxeContext context, bool isNew)
  {
    base.ExecuteFunction (function, context, isNew);
  }

  public new void CleanUpFunctionState (WxeFunctionState functionState)
  {
    base.CleanUpFunctionState (functionState);
  }

  public new void ProcessReturnUrl (HttpContext context, string returnUrl)
  {
    base.ProcessReturnUrl (context, returnUrl);
  }
}

}
