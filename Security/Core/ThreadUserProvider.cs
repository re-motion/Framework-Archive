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
using System.Collections.Specialized;
using System.Security.Principal;
using System.Threading;
using Remotion.Configuration;

namespace Remotion.Security
{
  public class ThreadUserProvider: ExtendedProviderBase, IUserProvider
  {
    // types

    // static members

    // member fields

    // construction and disposing

    public ThreadUserProvider()
        : this ("Thread", new NameValueCollection())
    {
    }

    public ThreadUserProvider (string name, NameValueCollection config)
        : base (name, config)
    {
    }

    // methods and properties

    public IPrincipal GetUser()
    {
      return Thread.CurrentPrincipal;
    }

    bool INullObject.IsNull
    {
      get { return false; }
    }
  }
}
