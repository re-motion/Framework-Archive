using System;
using NUnit.Framework;
using Rubicon.SecurityManager.Domain;
using Rubicon.SecurityManager.Domain.AccessControl;
using Rubicon.SecurityManager.Domain.Metadata;
using Rubicon.SecurityManager.Domain.OrganizationalStructure;
using Rubicon.Data.DomainObjects;
using Rubicon.Development.UnitTesting;

namespace Rubicon.SecurityManager.UnitTests.Domain
{
  [TestFixture]
  public class SerializationTest
  {
    [Test]
    public void DomainObjectsAreSerializable ()
    {
      using (ClientTransaction.NewTransaction ().EnterScope ())
      {
        CheckDomainObjectSerializability<AccessControlEntry>();
        CheckDomainObjectSerializability<AccessControlList>();
        CheckDomainObjectSerializability<Permission>();
        CheckDomainObjectSerializability<StateCombination>();
        CheckDomainObjectSerializability<StateUsage>();
        CheckDomainObjectSerializability<AbstractRoleDefinition>();
        CheckDomainObjectSerializability<AccessTypeDefinition>();
        CheckDomainObjectSerializability<AccessTypeReference>();
        CheckDomainObjectSerializability<Culture> ("DE-DE");
        CheckDomainObjectSerializability<LocalizedName> ("foo", Culture.NewObject ("DE-DE"), SecurableClassDefinition.NewObject());
        CheckDomainObjectSerializability<SecurableClassDefinition>();
        CheckDomainObjectSerializability<StateDefinition>();
        CheckDomainObjectSerializability<StatePropertyDefinition>();
        CheckDomainObjectSerializability<StatePropertyReference>();
        CheckDomainObjectSerializability<Group>();
        CheckDomainObjectSerializability<GroupType>();
        CheckDomainObjectSerializability<GroupTypePosition>();
        CheckDomainObjectSerializability<Position>();
        CheckDomainObjectSerializability<Role>();
        CheckDomainObjectSerializability<Tenant>();
        CheckDomainObjectSerializability<User>();
      }
    }

    private void CheckDomainObjectSerializability<T> (params object[] args)
        where T : DomainObject
    {
      T t;
      try
      {
        t = (T) PrivateInvoke.InvokePublicStaticMethod (typeof (T), "NewObject", args);
      }
      catch (MethodNotFoundException)
      {
        t = (T) PrivateInvoke.InvokeNonPublicStaticMethod (typeof (T), "NewObject", args);
      }
      T deserializedT = Serializer.SerializeAndDeserialize (t);
      Assert.IsNotNull (deserializedT);
    }
  }
}