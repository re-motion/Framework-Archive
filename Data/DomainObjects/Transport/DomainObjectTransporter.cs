using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using Rubicon.Data.DomainObjects;
using Rubicon.Data.DomainObjects.Persistence;
using Rubicon.Utilities;

namespace Rubicon.Data.DomainObjects.Transport
{
  /// <summary>
  /// Collects domain objects to be transported to another system.
  /// </summary>
  public class DomainObjectTransporter
  {
    /// <summary>
    /// Loads the data transported from another system into a <see cref="TransportedDomainObjects"/> container.
    /// </summary>
    /// <param name="data">The transported data to be loaded.</param>
    /// <returns>A container holding the objects loaded from the given data.</returns>
    /// <exception cref="ObjectNotFoundException">A referenced related object is not part of the transported data and does not exist on the
    /// target system.</exception>
    /// <remarks>
    /// Given a <see cref="DomainObjectTransporter"/>, the binary data can be retrieved from <see cref="GetBinaryTransportData"/>.
    /// </remarks>
    public static TransportedDomainObjects LoadTransportData (byte[] data)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("data", data);
      return new DomainObjectImporter (data).GetImportedObjects ();
    }

    private readonly ClientTransaction _transportTransaction = ClientTransaction.NewTransaction ();

    /// <summary>
    /// Gets the number of objects loaded into this transporter.
    /// </summary>
    /// <value>The number of loaded objects.</value>
    public int ObjectCount
    {
      get { return _transportTransaction.EnlistedDomainObjectCount; }
    }

    /// <summary>
    /// Gets the IDs of the objects loaded into this transporter.
    /// </summary>
    /// <value>The IDs of the loaded objects.</value>
    public IEnumerable<ObjectID> ObjectIDs
    {
      get
      {
        foreach (DomainObject domainObject in _transportTransaction.EnlistedDomainObjects)
          yield return domainObject.ID;
      }
    }

    /// <summary>
    /// Loads the object with the specified <see cref="ObjectID"/> into the transporter.
    /// </summary>
    /// <param name="objectID">The <see cref="ObjectID"/> of the object to load.</param>
    /// <remarks>
    /// <para>
    /// This method loads exactly the object with the given ID, it will not load any related objects.
    /// </para>
    /// <para>
    /// If an object has the foreign key side of a relationship and the related object is not loaded into this transporter, the relationship
    /// will still be transported. The related object must exist at the target system, otherwise an exception is thrown in
    /// <see cref="LoadTransportData"/>.
    /// </para>
    /// <para>
    /// If an object has the virtual side of a relationship and the related object is not loaded into this transporter, the relationship
    /// will not be transported. Its status after <see cref="LoadTransportData"/> depends on the objects at the target system. This
    /// also applies to the 1-side of a 1-to-n relationship because the n-side is the foreign key side.
    /// </para>
    /// </remarks>
    public void Load (ObjectID objectID)
    {
      ArgumentUtility.CheckNotNull ("objectID", objectID);
      _transportTransaction.GetObject (objectID, false);
    }

    /// <summary>
    /// Loads the object with the specified <see cref="ObjectID"/> plus all objects directly referenced by it into the transporter.
    /// Each object behaves as if it were loaded via <see cref="Load"/>.
    /// </summary>
    /// <param name="objectID">The <see cref="ObjectID"/> of the object which is to be loaded together with its related objects.</param>
    /// <seealso cref="DomainObject.GetAllRelatedObjects"/>
    public void LoadWithRelatedObjects (ObjectID objectID)
    {
      ArgumentUtility.CheckNotNull ("objectID", objectID);

      DomainObject sourceObject = _transportTransaction.GetObject (objectID, false);
      using (_transportTransaction.EnterNonDiscardingScope ())
      {
        foreach (DomainObject domainObject in sourceObject.GetAllRelatedObjects ())
          Load (domainObject.ID); // explicitly call load rather than just implicitly loading it into the transaction for consistency
      }
    }

    /// <summary>
    /// Loads the object with the specified <see cref="ObjectID"/> plus all objects directly or indirectly referenced by it into the
    /// transporter. Each object behaves as if it were loaded via <see cref="Load"/>.
    /// </summary>
    /// <param name="objectID">The <see cref="ObjectID"/> of the object which is to be loaded together with its related objects.</param>
    /// <seealso cref="DomainObject.GetFlattenedRelatedObjectGraph"/>
    public void LoadRecursive (ObjectID objectID)
    {
      ArgumentUtility.CheckNotNull ("objectID", objectID);

      DomainObject sourceObject = _transportTransaction.GetObject (objectID, false);
      using (_transportTransaction.EnterNonDiscardingScope ())
      {
        foreach (DomainObject domainObject in sourceObject.GetFlattenedRelatedObjectGraph())
          Load (domainObject.ID); // explicitly call load rather than just implicitly loading it into the transaction for consistency
      }
    }

    /// <summary>
    /// Gets a the objects loaded into this transporter (including their contents) in a binary format for transport to another system.
    /// At the target system, the data can be loaded via <see cref="LoadTransportData"/>.
    /// </summary>
    /// <returns>The loaded objects in a binary format.</returns>
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