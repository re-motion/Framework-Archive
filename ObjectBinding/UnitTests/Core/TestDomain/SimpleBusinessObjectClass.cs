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

namespace Remotion.ObjectBinding.UnitTests.Core.TestDomain
{
  [BindableObject]
  [Serializable]
  public class SimpleBusinessObjectClass
  {
    private string _string;

    public SimpleBusinessObjectClass ()
    {
    }

    public string String
    {
      get { return _string; }
      set { _string = value; }
    }

    public string StringWithoutGetter
    {
      set { _string = value; }
    }

    public string StringWithoutSetter
    {
      set { _string = value; }
    }
  }
}
