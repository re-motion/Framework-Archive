using System.Collections.Generic;
using Rubicon.Data.DomainObjects;

namespace Rubicon.Data.DomainObjects.Transport
{
  /// <summary>
  /// Implements a strategy to import a set of transported <see cref="DomainObject"/> instances from a byte array. The imported objects
  /// should be wrapped as <see cref="TransportItem"/> property holders, the <see cref="DomainObjectImporter"/> class creates 
  /// <see cref="DomainObject"/> instances from those holders and synchronizes them with the database.
  /// </summary>
  /// <remarks>
  /// Supply an implementation of this interface to <see cref="DomainObjectTransporter.LoadTransportData(byte[],IImportStrategy)"/>. The strategy
  /// should match the <see cref="IExportStrategy"/> supplied to <see cref="DomainObjectTransporter.GetBinaryTransportData(IExportStrategy)"/>.
  /// </remarks>
  public interface IImportStrategy
  {
    /// <summary>
    /// Imports the specified data.
    /// </summary>
    /// <param name="data">The data to be imported.</param>
    /// <returns>A stream of <see cref="TransportItem"/> values representing <see cref="DomainObject"/> instances.</returns>
    /// <exception cref="TransportationException">The data could not be imported using this strategy.</exception>
    IEnumerable<TransportItem> Import (byte[] data);
  }
}