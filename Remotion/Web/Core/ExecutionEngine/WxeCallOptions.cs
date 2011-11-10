// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) rubicon IT GmbH, www.rubicon.eu
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
using Remotion.Utilities;

namespace Remotion.Web.ExecutionEngine
{
  /// <summary>
  /// The <see cref="WxeCallOptions"/> type represents the most generic of options for executing a <see cref="WxeFunction"/>, namely it only controls
  /// whether the useer should be presented with a perma-URL to the <see cref="WxeFunction"/>.
  /// Use the derived types if you require additional control over the <see cref="WxeFunction"/>'s execution.
  /// </summary>
  [Serializable]
  public class WxeCallOptions
  {
    private readonly WxePermaUrlOptions _permaUrlOptions;

    public WxeCallOptions ()
        : this (WxePermaUrlOptions.Null)
    {
    }

    public WxeCallOptions (WxePermaUrlOptions permaUrlOptions)
    {
      ArgumentUtility.CheckNotNull ("permaUrlOptions", permaUrlOptions);

      _permaUrlOptions = permaUrlOptions;
    }

    public virtual void Dispatch (IWxeExecutor executor, WxeFunction function, WxeCallArguments handler)
    {
      ArgumentUtility.CheckNotNull ("executor", executor);
      ArgumentUtility.CheckNotNull ("function", function);

      executor.ExecuteFunction (function, _permaUrlOptions);
    }

    public WxePermaUrlOptions PermaUrlOptions
    {
      get { return _permaUrlOptions; }
    }
  }
}
