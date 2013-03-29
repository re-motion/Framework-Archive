// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (c) rubicon IT GmbH, www.rubicon.eu
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
using System.Linq;
using NUnit.Framework;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model;
using Remotion.Data.DomainObjects.Queries;
using Remotion.Data.UnitTests.DomainObjects.Core.MixedDomains.TestDomain;
using Remotion.Data.UnitTests.DomainObjects.Core.MixedDomains.TestDomain.ConcreteInheritance;
using Remotion.Data.UnitTests.DomainObjects.Core.MixedDomains.TestDomain.SingleInheritance;
using Remotion.Data.UnitTests.DomainObjects.Factories;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;
using Remotion.Data.UnitTests.DomainObjects.TestDomain.TableInheritance;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Linq.IntegrationTests
{
  [TestFixture]
  public class UpcastIntegrationTest : IntegrationTestBase
  {
    private TableInheritanceDomainObjectIDs _concreteObjectIDs;

    public override void SetUp ()
    {
      base.SetUp ();
      _concreteObjectIDs = new TableInheritanceDomainObjectIDs (Configuration);
    }

    [Test]
    public void AccessingProperties_OfDerivedClass ()
    {
      var queryWithSingleTableInheritance =
          from c in QueryFactory.CreateLinqQuery<Company>()
          where
              c is Customer
              && (((Customer) c).Type == Customer.CustomerType.Standard || ((Customer) c).Orders.Select (o => o.ID).Contains (DomainObjectIDs.Order4))
          select c;
      CheckQueryResult (queryWithSingleTableInheritance, DomainObjectIDs.Customer1, DomainObjectIDs.Customer4);

      var queryWithConcreteTableInheritance =
          from fsi in QueryFactory.CreateLinqQuery<TIFileSystemItem>()
          where
              fsi is TIFolder
              && (((TIFolder) fsi).CreatedAt == new DateTime (2006, 2, 1) && ((TIFolder) fsi).FileSystemItems.Any (i => i.Name == "Datei im Root"))
          select fsi;
      CheckQueryResult (queryWithConcreteTableInheritance, _concreteObjectIDs.FolderRoot);
    }

    [Test]
    public void AccessingMixinProperties_OfDerivedClass ()
    {
      var queryWithSingleTableInheritance =
          from obj in QueryFactory.CreateLinqQuery<SingleInheritanceBaseClass> ()
          where
              (obj is SingleInheritanceFirstDerivedClass || obj is SingleInheritanceSecondDerivedClass) 
                  && (((ISingleInheritancePersistentMixin) obj).PersistentProperty == "dummy")
          select obj;
      CheckQueryResult (queryWithSingleTableInheritance);

      var queryWithConcreteTableInheritance =
          from obj in QueryFactory.CreateLinqQuery<ConcreteInheritanceBaseClass> ()
          where
              (obj is ConcreteInheritanceFirstDerivedClass || obj is ConcreteInheritanceSecondDerivedClass)
              && (((IConcreteInheritancePersistentMixin) obj).PersistentProperty == "dummy")
          select obj;
      CheckQueryResult (queryWithConcreteTableInheritance);
    }
 }
}