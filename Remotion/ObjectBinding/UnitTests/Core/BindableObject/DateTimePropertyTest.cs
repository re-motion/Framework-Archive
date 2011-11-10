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
using NUnit.Framework;
using Remotion.ObjectBinding.BindableObject;
using Remotion.ObjectBinding.BindableObject.Properties;
using Remotion.ObjectBinding.UnitTests.Core.TestDomain;

namespace Remotion.ObjectBinding.UnitTests.Core.BindableObject
{
  [TestFixture]
  public class DateTimePropertyTest : TestBase
  {
    private BindableObjectProvider _businessObjectProvider;

    public override void SetUp ()
    {
      base.SetUp ();

      _businessObjectProvider = CreateBindableObjectProviderWithStubBusinessObjectServiceFactory ();
    }

    [Test]
    public void Initialize_DateProperty ()
    {
      IBusinessObjectDateTimeProperty property = new DateProperty (
          GetPropertyParameters (GetPropertyInfo (typeof (ClassWithAllDataTypes), "Date"), _businessObjectProvider));

      Assert.That (property.Type, Is.EqualTo (DateTimeType.Date));
    }

    [Test]
    public void Initialize_DateTimeProperty ()
    {
      IBusinessObjectDateTimeProperty property = new DateTimeProperty (
          GetPropertyParameters (GetPropertyInfo (typeof (ClassWithAllDataTypes), "DateTime"), _businessObjectProvider));

      Assert.That (property.Type, Is.EqualTo (DateTimeType.DateTime));
    }
  }
}
