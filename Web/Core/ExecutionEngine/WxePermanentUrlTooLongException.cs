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
using System.Runtime.Serialization;

namespace Remotion.Web.ExecutionEngine
{

/// <summary>
///   This exception occurs when a permanent URL would exceed the maximum URL length.
/// </summary>
[Serializable]
public class WxePermanentUrlTooLongException: WxeException
{
	public WxePermanentUrlTooLongException (string message)
    : base (message)
	{
  }

  public WxePermanentUrlTooLongException (SerializationInfo info, StreamingContext context)
    : base (info, context)
  {
  }
}

}
