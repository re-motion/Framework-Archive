using System;
using System.Collections.Generic;
using System.Text;

using Rubicon.Utilities;
using Rubicon.Data.DomainObjects;
using Rubicon.SecurityManager.Domain.OrganizationalStructure;

namespace Rubicon.SecurityManager.UnitTests.Domain.OrganizationalStructure
{
  public class OrganisationalStructureTestHelper
  {
    private ClientTransaction _transaction;
    private OrganizationalStructureFactory _factory;

    public OrganisationalStructureTestHelper ()
    {
      _transaction = new ClientTransaction ();
      _factory = new OrganizationalStructureFactory ();
    }

    public ClientTransaction Transaction
    {
      get { return _transaction; }
    }

    public Client CreateClient (ClientTransaction transaction, string name)
    {
      Client client = new Client (transaction);
      client.Name = name;

      return client;
    }

    public Group CreateGroup (ClientTransaction transaction, string name, string uniqueIdentifier, Group parent, Client client)
    {
      Group group = _factory.CreateGroup (transaction);
      group.Name = name;
      group.Parent = parent;
      group.Client = client;
      group.UniqueIdentifier = uniqueIdentifier;

      return group;
    }
    
    public User CreateUser (ClientTransaction transaction, string userName, string firstName, string lastName, string title, Group group, Client client)
    {
      User user = _factory.CreateUser (transaction);
      user.UserName = userName;
      user.FirstName = firstName;
      user.LastName = lastName;
      user.Title = title;
      user.Client = client;
      user.Group = group;

      return user;
    }
  }
}