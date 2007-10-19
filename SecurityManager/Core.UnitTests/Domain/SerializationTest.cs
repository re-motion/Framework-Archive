using System;
using NUnit.Framework;
using Rubicon.Collections;
using Rubicon.Data.DomainObjects.ObjectBinding;
using Rubicon.ObjectBinding;
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
      using (ClientTransaction.NewTransaction ().EnterNonDiscardingScope ())
      {
        CheckDomainObjectSerializability<AccessControlEntry>();
        CheckDomainObjectSerializability<AccessControlList> ();
        CheckDomainObjectSerializability<Permission> ();
        CheckDomainObjectSerializability<StateCombination> ();
        CheckDomainObjectSerializability<StateUsage> ();
        CheckDomainObjectSerializability<AbstractRoleDefinition> ();
        CheckDomainObjectSerializability<AccessTypeDefinition> ();
        CheckDomainObjectSerializability<AccessTypeReference> ();
        CheckDomainObjectSerializability<Culture> ("DE-DE");
        CheckDomainObjectSerializability<LocalizedName> ("foo", Culture.NewObject ("DE-DE"), SecurableClassDefinition.NewObject ());
        CheckDomainObjectSerializability<SecurableClassDefinition> ();
        CheckDomainObjectSerializability<StateDefinition> ();
        CheckDomainObjectSerializability<StatePropertyDefinition> ();
        CheckDomainObjectSerializability<StatePropertyReference> ();
        CheckDomainObjectSerializability<Group> ();
        CheckDomainObjectSerializability<GroupType> ();
        CheckDomainObjectSerializability<GroupTypePosition> ();
        CheckDomainObjectSerializability<Position> ();
        CheckDomainObjectSerializability<Role> ();
        CheckDomainObjectSerializability<Tenant> ();
        CheckDomainObjectSerializability<User> ();
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
      
      Tuple<T, ClientTransaction> deserializedTuple = Serializer.SerializeAndDeserialize (Tuple.NewTuple (t, ClientTransaction.Current));
      T deserializedT = deserializedTuple.A;
      Assert.IsNotNull (deserializedT);

      IBusinessObject bindableOriginal = (IBusinessObject) t;
      IBusinessObject bindableDeserialized = (IBusinessObject) deserializedT;

      foreach (IBusinessObjectProperty property in bindableOriginal.BusinessObjectClass.GetPropertyDefinitions ())
      {
        Assert.IsNotNull (bindableDeserialized.BusinessObjectClass.GetPropertyDefinition (property.Identifier));
        
        object value = null;
        bool propertyCanBeRetrieved;
        try
        {
          value = bindableOriginal.GetProperty (property);
          propertyCanBeRetrieved = true;
        }
        catch (Exception)
        {
          propertyCanBeRetrieved = false;
        }

        if (propertyCanBeRetrieved)
        {
          object newValue;
          using (deserializedTuple.B.EnterNonDiscardingScope())
          {
            newValue = bindableDeserialized.GetProperty (property);
          }
          if (value != null && typeof (DomainObject).IsAssignableFrom (property.PropertyType))
            Assert.AreEqual (((DomainObject)value).ID, ((DomainObject)newValue).ID);
          else
            Assert.AreEqual (value, newValue);
        }
      }
    }
  }
}