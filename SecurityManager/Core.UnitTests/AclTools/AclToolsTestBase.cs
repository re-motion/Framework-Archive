/* Copyright (C) 2005 - 2008 rubicon informationstechnologie gmbh
 *
 * This program is free software: you can redistribute it and/or modify it under 
 * the terms of the re:motion license agreement in license.txt. If you did not 
 * receive it, please visit http://www.re-motion.org/licensing.
 * 
 * Unless otherwise provided, this software is distributed on an "AS IS" basis, 
 * WITHOUT WARRANTY OF ANY KIND, either express or implied. 
 */

using NUnit.Framework;
using Remotion.SecurityManager.Domain.AccessControl;
using Remotion.SecurityManager.Domain.Metadata;
using Remotion.SecurityManager.Domain.OrganizationalStructure;
using Remotion.SecurityManager.UnitTests.Domain.AccessControl;

namespace Remotion.SecurityManager.UnitTests.AclTools
{
  public class AclToolsTestBase : DomainTest
  {
    private AccessControlTestHelper _testHelper;

    private Tenant _tenant;
    private Group _group;
    private Position _position;
    private Role _role;
    private User _user;

    public AccessControlTestHelper TestHelper
    {
      get { return _testHelper; }
    }

    public Tenant Tenant
    {
      get { return _tenant; }
    }

    public Group Group
    {
      get { return _group; }
    }

    public Position Position
    {
      get { return _position; }
    }

    public Role Role
    {
      get { return _role; }
    }

    public User User
    {
      get { return _user; }
    }

    public override void SetUp ()
    {
      base.SetUp ();
      _testHelper = new AccessControlTestHelper ();
      TestHelper.Transaction.EnterNonDiscardingScope ();

      _tenant = TestHelper.CreateTenant ("Da Tenant");
      _group = TestHelper.CreateGroup ("Da Group", null, Tenant);
      _position = TestHelper.CreatePosition ("Supreme Being");
      _role = TestHelper.CreateRole (User, Group, Position);
      _user  = TestHelper.CreateUser ("DaUs", "Da", "Usa", "Dr.", Group, Tenant);
    }

    private User CreateUser (Tenant userTenant, Group userGroup)
    {
      return TestHelper.CreateUser ("JoDo", "John", "Doe", "Prof.", userGroup, userTenant);
    }
  }
}