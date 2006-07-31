using System;
using System.Collections.Generic;
using System.Text;

using Rubicon.Data.DomainObjects;
using Rubicon.SecurityManager.Domain.Metadata;

namespace Rubicon.SecurityManager.UnitTests.Domain.Metadata
{
  public class MetadataTestHelper
  {
    public const string Confidentiality_NormalName = "Normal";
    public const int Confidentiality_NormalValue = 0;
    public const string Confidentiality_ConfidentialName = "Confidential";
    public const int Confidentiality_ConfidentialValue = 1;
    public const string Confidentiality_PrivateName = "Private";
    public const int Confidentiality_PrivateValue = 2;

    public const string State_NewName = "New";
    public const int State_NewValue = 0;
    public const string State_NormalName = "Normal";
    public const int State_NormalValue = 1;
    public const string State_ArchivedName = "Archived";
    public const int State_ArchivedValue = 2;

    private ClientTransaction _transaction;

    public MetadataTestHelper ()
    {
      _transaction = new ClientTransaction ();
    }

    public ClientTransaction Transaction
    {
      get { return _transaction; }
    }

    public AbstractRoleDefinition CreateClerkAbstractRole (int index)
    {
      AbstractRoleDefinition role = new AbstractRoleDefinition (_transaction, new Guid ("00000003-0001-0000-0000-000000000000"), "Clerk|Rubicon.Security.UnitTests.TestDomain.DomainAbstractRoles, Rubicon.Security.UnitTests.TestDomain", 0);
      role.Index = index;

      return role;
    }

    public AbstractRoleDefinition CreateSecretaryAbstractRole (int index)
    {
      AbstractRoleDefinition role = new AbstractRoleDefinition (_transaction, new Guid ("00000003-0002-0000-0000-000000000000"), "Secretary|Rubicon.Security.UnitTests.TestDomain.DomainAbstractRoles, Rubicon.Security.UnitTests.TestDomain", 1);
      role.Index = index;

      return role;
    }

    public AbstractRoleDefinition CreateAdministratorAbstractRole (int index)
    {
      AbstractRoleDefinition role = new AbstractRoleDefinition (_transaction, new Guid ("00000004-0001-0000-0000-000000000000"), "Administrator|Rubicon.Security.UnitTests.TestDomain.SpecialAbstractRoles, Rubicon.Security.UnitTests.TestDomain", 0);
      role.Index = index;

      return role;
    }

    public AccessTypeDefinition CreateAccessTypeCreate (int index)
    {
      AccessTypeDefinition type = new AccessTypeDefinition (_transaction, new Guid ("1d6d25bc-4e85-43ab-a42d-fb5a829c30d5"), "Create|Rubicon.Security.GeneralAccessTypes, Rubicon.Security", 0);
      type.Index = index;

      return type;
    }

    public AccessTypeDefinition CreateAccessTypeRead (int index)
    {
      AccessTypeDefinition type = new AccessTypeDefinition (_transaction, new Guid ("62dfcd92-a480-4d57-95f1-28c0f5996b3a"), "Read|Rubicon.Security.GeneralAccessTypes, Rubicon.Security", 1);
      type.Index = index;

      return type;
    }

    public AccessTypeDefinition CreateAccessTypeEdit (int index)
    {
      AccessTypeDefinition type = new AccessTypeDefinition (_transaction, new Guid ("11186122-6de0-4194-b434-9979230c41fd"), "Edit|Rubicon.Security.GeneralAccessTypes, Rubicon.Security", 2);
      type.Index = index;
      
      return type;
    }

    public StatePropertyDefinition CreateConfidentialityProperty (int index)
    {
      StatePropertyDefinition property = new StatePropertyDefinition (_transaction, new Guid ("00000000-0000-0000-0001-000000000001"), "Confidentiality");
      property.Index = index;
      property.AddState (Confidentiality_NormalName, Confidentiality_NormalValue);
      property.AddState (Confidentiality_ConfidentialName, Confidentiality_ConfidentialValue);
      property.AddState (Confidentiality_PrivateName, Confidentiality_PrivateValue);

      return property;
    }

    public StatePropertyDefinition CreateFileStateProperty (int index)
    {
      StatePropertyDefinition property = new StatePropertyDefinition (_transaction, new Guid ("00000000-0000-0000-0002-000000000001"), "State");
      property.Index = index;
      property.AddState (State_NewName, State_NewValue);
      property.AddState (State_NormalName, State_NormalValue);
      property.AddState (State_ArchivedName, State_ArchivedValue);

      return property;
    }

    public StateDefinition CreateConfidentialState ()
    {
      return CreateState (Confidentiality_ConfidentialName, Confidentiality_ConfidentialValue);
    }

    public StateDefinition CreatePrivateState ()
    {
      return CreateState (Confidentiality_PrivateName, Confidentiality_PrivateValue);
    }

    public StatePropertyDefinition CreateNewStateProperty (string name)
    {
      return new StatePropertyDefinition (_transaction, Guid.NewGuid (), name);
    }

    public StateDefinition CreateState (string name, int value)
    {
      StateDefinition state = new StateDefinition (_transaction, name, value);
      state.Index = value;

      return state;
    }
  }
}
