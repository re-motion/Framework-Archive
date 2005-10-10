using System;
using System.Collections;

namespace Rubicon.ObjectBinding
{

/// <summary> Provides functionality for business object providers. </summary>
/// <remarks>
///   A business object provider is able to retrieve services (e.g. the
///   <see cref="T:Rubicon.ObjectBinding.Web.IBusinessObjectWebUIService"/>) from the object model, as well as provide
///   functionality required by more than one of the business object components (<b>Class</b>, <b>Property</b>, and
///   <b>Object</b>).
///   <note type="inheritinfo">
///     If this interface is implemented using singletons, the singleton must be thread save.
///   </note>
///   <note type="inheritinfo">
///     You can use the abstract default implemtation (<see cref="BusinessObjectProvider"/>) as a base for implementing
///     the business object provider for your object model.
///   </note>
/// </remarks>
public interface IBusinessObjectProvider
{
  /// <summary> Retrieves the requested <see cref="IBusinessObjectService"/>. </summary>
  /// <param name="serviceType"> The type of <see cref="IBusinessObjectService"/> to get from the object model. </param>
  /// <returns> 
  ///   An instance if the <see cref="IBusinessObjectService"/> type or <see langword="null"/> if the sevice could not
  ///   be found or instantiated.
  ///  </returns>
  ///  <remarks>
  ///    <note type="inheritinfo">
  ///     If your object model does not support services, this method may always return null.
  ///    </note>
  ///  </remarks>
  IBusinessObjectService GetService (Type serviceType);

  /// <summary> 
  ///   Returns the <see cref="Char"/> to be used as a serparator when formatting the property path's identifier.
  /// </summary>
  /// <returns> A <see cref="Char"/> that is not used by the property's identifier. </returns>
  char GetPropertyPathSeparator();

  /// <summary> 
  ///   Creates a <see cref="BusinessObjectPropertyPath"/> from the passed <see cref="IBusinessObjectProperty"/> list.
  /// </summary>
  /// <param name="properties"> An array of <see cref="IBusinessObjectProperty"/> instances. </param>
  /// <returns> A new instance of the <see cref="BusinessObjectPropertyPath"/> type. </returns>
  BusinessObjectPropertyPath CreatePropertyPath (IBusinessObjectProperty[] properties);
}

/// <summary> The abstract default implementation of the <see cref="IBusinessObjectProvider"/> interface. </summary>
public abstract class BusinessObjectProvider: IBusinessObjectProvider
{
  /// <summary> The <see cref="IDictionary"/> used to store the references to the registered servies. </summary>
  /// <remarks>
  ///   <note type="inheritinfo">
  ///    If your object model does not support services, this property may always return <see langword="null"/>.
  ///   </note>
  /// </remarks>
  protected abstract IDictionary ServiceDictionary { get; }

  /// <summary> Retrieves the requested <see cref="IBusinessObjectService"/>. </summary>
  public IBusinessObjectService GetService (Type serviceType)
  {
    IDictionary serviceDictionary = ServiceDictionary;
    if (serviceDictionary != null)
      return (IBusinessObjectService) serviceDictionary[serviceType];
    return null;
  }

  /// <summary> Registers a new <see cref="IBusinessObjectService"/> with this <see cref="BusinessObjectProvider"/>. </summary>
  /// <param name="serviceType"> The type of the service to be registered. </param>
  /// <param name="service"> The <see cref="IBusinessObjectService"/> to register. </param>
  public void AddService (Type serviceType, IBusinessObjectService service)
  {
    IDictionary serviceDictionary = ServiceDictionary;
    if (serviceDictionary != null)
      serviceDictionary[serviceType] = service;
  }

  /// <summary> 
  ///   Returns the <see cref="Char"/> to be used as a serparator when formatting the property path's identifier.
  /// </summary>
  public virtual char GetPropertyPathSeparator()
  {
    return '.';
  }

  /// <summary> 
  ///   Creates a <see cref="BusinessObjectPropertyPath"/> from the passed <see cref="IBusinessObjectProperty"/> list.
  /// </summary>
  public virtual BusinessObjectPropertyPath CreatePropertyPath (IBusinessObjectProperty[] properties)
  {
    return new BusinessObjectPropertyPath (properties);
  }
}

}
