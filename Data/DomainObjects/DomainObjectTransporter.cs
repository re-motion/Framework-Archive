using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Rubicon.Data.DomainObjects
{
  /// <summary>
  /// Constitutes a collection of domain objects to be transported to another system.
  /// </summary>
  public class DomainObjectTransporter
  {
    /*/// <summary>
    /// Defines a method for filtering the objects loaded by <see cref="DomainObjectTransporter.LoadRecursive(ObjectID,TraversionController)"/> and for controlling how deeply
    /// object graph is processed.
    /// </summary>
    /// <param name="currentObject">The current object in the process of traversing the object graph. When the <see cref="TraversionController"/>
    /// is called for the first time, this parameter is the <see cref="ObjectID"/> passed to
    /// <see cref="DomainObjectTransporter.LoadRecursive(ObjectID,TraversionController)"/>.</param>
    /// <param name="continueTraversion">A parameter that can be set to <see langword="false"/> in order to stop traversing the related objects of
    /// <paramref name="currentObject"/>. The default is <see langword="true"/>.</param>
    /// <returns>True if the current object should be loaded; false otherwise.</returns>
    public delegate bool TraversionController (ObjectID currentObject, ref bool continueTraversion);
    */

    private ClientTransaction _transportTransaction = ClientTransaction.NewTransaction ();

    public int ObjectCount
    {
      get
      {
        return _transportTransaction.EnlistedDomainObjectCount;
      }
    }

    public IEnumerable<ObjectID> ObjectIDs
    {
      get
      {
        foreach (DomainObject domainObject in _transportTransaction.EnlistedDomainObjects)
          yield return domainObject.ID;
      }
    }

    public void Load (ObjectID objectID)
    {
      _transportTransaction.GetObject (objectID, false);
    }

    public void LoadWithRelatedObjects (ObjectID objectID)
    {
      DomainObject sourceObject = _transportTransaction.GetObject (objectID, false);
      using (_transportTransaction.EnterNonDiscardingScope ())
      {
        foreach (DomainObject domainObject in sourceObject.GetAllRelatedObjects ())
          Load (domainObject.ID); // explicitly call load rather than just implicitly loading it into the transaction for consistency
      }
    }

    public void LoadRecursive (ObjectID objectID)
    {
      DomainObject sourceObject = _transportTransaction.GetObject (objectID, false);
      using (_transportTransaction.EnterNonDiscardingScope ())
      {
        foreach (DomainObject domainObject in sourceObject.GetFlattenedRelatedObjectGraph())
          Load (domainObject.ID); // explicitly call load rather than just implicitly loading it into the transaction for consistency
      }
    }

    /*public void LoadRecursive (ObjectID objectID, TraversionController controller)
    {
      throw new NotImplementedException ();
    }*/

    public byte[] GetBinaryTransportData ()
    {
      using (MemoryStream dataStream = new MemoryStream ())
      {
        BinaryFormatter formatter = new BinaryFormatter ();
        formatter.Serialize (dataStream, _transportTransaction);
        return dataStream.ToArray ();
      }
    }
  }
}