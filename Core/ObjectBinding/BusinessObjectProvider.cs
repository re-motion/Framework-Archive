using System;
using Remotion.Collections;
using Remotion.Utilities;

namespace Remotion.ObjectBinding
{
  /// <summary>The <see langword="abstract"/> default implementation of the <see cref="IBusinessObjectProvider"/> interface.</summary>
  public abstract class BusinessObjectProvider : IBusinessObjectProvider
  {
    /// <summary> Gets the <see cref="ICache{TKey,TValue}"/> used to store the references to the registered servies. </summary>
    /// <value>An object implementing <see cref="ICache{TKey,TValue}"/>. Must not retun <see langword="null" />.</value>
    /// <remarks>
    ///   <note type="inotes">
    ///    If your object model does not support services, this property should return an instance of type <see cref="NullCache{TKey,TValue}"/>.
    ///   </note>
    /// </remarks>
    protected abstract ICache<Type, IBusinessObjectService> ServiceCache { get; }

    /// <summary> Retrieves the requested <see cref="IBusinessObjectService"/>. Must not be <see langword="null" />.</summary>
    public IBusinessObjectService GetService (Type serviceType)
    {
      ArgumentUtility.CheckNotNullAndTypeIsAssignableFrom ("serviceType", serviceType, typeof (IBusinessObjectService));

      ICache<Type, IBusinessObjectService> serviceCache = ServiceCache;
      Assertion.IsNotNull (serviceCache, "The ServiceCache evaluated and returned null. It should return a null object instead.");
      IBusinessObjectService service;
      if (serviceCache.TryGetValue (serviceType, out service))
        return service;
      return null;
    }

    /// <summary> Retrieves the requested <see cref="IBusinessObjectService"/>. </summary>
    public T GetService<T> () where T: IBusinessObjectService
    {
      return (T) GetService (typeof (T));
    }

    /// <summary> Registers a new <see cref="IBusinessObjectService"/> with this <see cref="BusinessObjectProvider"/>. </summary>
    /// <param name="serviceType"> The type of the service to be registered. Must not be <see langword="null" />.</param>
    /// <param name="service"> The <see cref="IBusinessObjectService"/> to register. Must not be <see langword="null" />.</param>
    public void AddService (Type serviceType, IBusinessObjectService service)
    {
      ArgumentUtility.CheckNotNullAndTypeIsAssignableFrom ("serviceType", serviceType, typeof (IBusinessObjectService));
      ArgumentUtility.CheckNotNull ("service", service);

      ICache<Type, IBusinessObjectService> serviceCache = ServiceCache;
      Assertion.IsNotNull (serviceCache, "The ServiceCache evaluated and returned null. It should return a null object instead.");
      serviceCache.Add (serviceType, service);
    }

    /// <summary>Returns the <see cref="Char"/> to be used as a serparator when formatting the property path's identifier.</summary>
    public virtual char GetPropertyPathSeparator ()
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

    /// <summary> Returns a <see cref="String"/> to be used instead of the actual value if the property is not accessible. </summary>
    /// <returns> A <see cref="String"/> that can be easily distinguished from typical property values. </returns>
    public virtual string GetNotAccessiblePropertyStringPlaceHolder ()
    {
      return "�";
    }
  }
}