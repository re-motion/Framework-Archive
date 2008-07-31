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
using NUnit.Framework;
using Remotion.Security;
using Remotion.Web.ExecutionEngine;
using Remotion.Web.UI;
using Remotion.Development.Web.UnitTesting.Configuration;

namespace Remotion.Web.UnitTests.UI.Controls.WebButtonTests
{
  public class BaseTest : WebControlTest
  {
    [TearDown]
    public override void TearDown ()
    {
      base.TearDown ();

      WebConfigurationMock.Current = null;
      AdapterRegistry.Instance.SetAdapter (typeof (IWebSecurityAdapter), null);
      AdapterRegistry.Instance.SetAdapter (typeof (IWxeSecurityAdapter), null);
    }
  }
}
