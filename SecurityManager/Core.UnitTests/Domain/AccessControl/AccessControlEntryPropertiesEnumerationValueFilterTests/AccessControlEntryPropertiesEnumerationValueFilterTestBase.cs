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
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Data.DomainObjects;
using Remotion.ObjectBinding;
using Remotion.ObjectBinding.BindableObject;
using Remotion.SecurityManager.Domain.AccessControl;

namespace Remotion.SecurityManager.UnitTests.Domain.AccessControl.AccessControlEntryPropertiesEnumerationValueFilterTests
{
  public class AccessControlEntryPropertiesEnumerationValueFilterTestBase : DomainTest
  {
    private IEnumerationValueFilter _filter;
    public override void SetUp ()
    {
      base.SetUp ();

      ClientTransaction.CreateRootTransaction().EnterNonDiscardingScope();

      _filter = new AccessControlEntryPropertiesEnumerationValueFilter();
    }

    protected IEnumerationValueFilter Filter
    {
      get { return _filter; }
    }

    protected IEnumerationValueInfo CreateEnumValueInfo (Enum enumValue)
    {
      return new EnumerationValueInfo (enumValue, "ID", "Name", true);
    }

    protected IEnumerationValueInfo CreateEnumValueInfo_Disabled (Enum enumValue)
    {
      return new EnumerationValueInfo (enumValue, "ID", "Name", false);
    }

    protected AccessControlEntry CreateAceForStateless ()
    {
      var ace = AccessControlEntry.NewObject ();
      ace.AccessControlList = StatelessAccessControlList.NewObject ();

      return ace;
    }

    protected AccessControlEntry CreateAceForStateful ()
    {
      var ace = AccessControlEntry.NewObject ();
      ace.AccessControlList = StatefulAccessControlList.NewObject ();

      return ace;
    }

    protected IBusinessObjectEnumerationProperty GetPropertyDefinition (AccessControlEntry ace, string propertyName)
    {
      var property = (IBusinessObjectEnumerationProperty) ((IBusinessObject) ace).BusinessObjectClass.GetPropertyDefinition (propertyName);
      Assert.That (property, Is.Not.Null, propertyName);
      return property;
    }
  }
}
