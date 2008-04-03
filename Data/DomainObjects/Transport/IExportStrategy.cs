namespace Remotion.Data.DomainObjects.Transport
{
  /// <summary>
  /// Implements a strategy to export a set of <see cref="DomainObject"/> instances to a byte array. The exported objects are
  /// wrapped as <see cref="TransportItem"/> property holders by the <see cref="DomainObjectTransporter"/> class.
  /// </summary>
  /// <remarks>
  /// Supply an implementation of this interface to <see cref="DomainObjectTransporter.GetBinaryTransportData(IExportStrategy)"/>. The strategy
  /// must match the <see cref="IImportStrategy"/> supplied to <see cref="DomainObjectTransporter.LoadTransportData(byte[],IImportStrategy)"/>.
  /// </remarks>
  public interface IExportStrategy
  {
    /// <summary>
    /// Exports the specified transported objects.
    /// </summary>
    /// <param name="transportedObjects">The objects to be exported.</param>
    /// <returns>A byte array representing the transported objects.</returns>
    /// <exception cref="TransportationException">The objects could not be exported using this strategy.</exception>
    byte[] Export (TransportItem[] transportedObjects);
  }
}