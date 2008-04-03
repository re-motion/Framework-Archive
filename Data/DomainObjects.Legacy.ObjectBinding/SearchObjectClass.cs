using System;
using Remotion.ObjectBinding;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.ObjectBinding
{
/// <summary>
/// The implementation of <see cref="Remotion.ObjectBinding.IBusinessObjectClass"/> for the <see cref="BindableSearchObject"/>.
/// </summary>
/// <remarks>
///   <para>
///     See the documentation of <see cref="Remotion.ObjectBinding.IBusinessObjectClassWithIdentity"/> for further information.
///   </para>
///   <para>
///     This class is used with <see cref="BindableSearchObject"/> and <see cref="SearchObjectDataSource"/>.
///   </para>
/// </remarks>
public class SearchObjectClass : IBusinessObjectClass
{
  private BusinessObjectClassReflector _classReflector;

  /// <summary>
  /// Instantiates a new object.
  /// </summary>
  /// <param name="type">The type that the object should represent.</param>
	public SearchObjectClass (Type type)
	{
    ArgumentUtility.CheckNotNull ("type", type);

    ReflectionPropertyFactory propertyFactory = new ReflectionPropertyFactory (this);
    _classReflector = new BusinessObjectClassReflector (type, propertyFactory);
  }

  #region IBusinessObjectClass Members

  /// <summary>
  /// Gets <see langword="false"/>.
  /// </summary>
  public bool RequiresWriteBack
  {
    get { return false; }
  }

  /// <summary>
  /// Gets the full name of the class that is represented by the object.
  /// </summary>
  /// <value>The full name of the class that is represented by the object.</value>
  public string Identifier
  {
    get { return _classReflector.Identifier; }
  }

  /// <summary>
  /// Returns an instance of <see cref="SearchObjectProvider"/>.
  /// </summary>
  public IBusinessObjectProvider BusinessObjectProvider
  {
    get { return SearchObjectProvider.Instance; }
  }

  /// <summary>
  /// Returns an <see cref="Remotion.ObjectBinding.IBusinessObjectProperty"/> representing the given <paramref name="propertyIdentifier"/>.
  /// </summary>
  /// <param name="propertyIdentifier">The name of the property.</param>
  /// <returns>An instance of <see cref="BaseProperty"/> or derived type representing the given <paramref name="propertyIdentifier"/>, or <see langword="null"/> if not found.</returns>
  public IBusinessObjectProperty GetPropertyDefinition (string propertyIdentifier)
  {
    return _classReflector.GetPropertyDefinition (propertyIdentifier);
  }

  /// <summary>
  /// Returns an array of <see cref="Remotion.ObjectBinding.IBusinessObjectProperty"/> for all properties of the type that was passed in the constructor.
  /// </summary>
  /// <returns>An array of instances of <see cref="BaseProperty"/> or a derived type for each property. If no properties can be found, an empty array is returned.</returns>
  public IBusinessObjectProperty[] GetPropertyDefinitions ()
  {
    return _classReflector.GetPropertyDefinitions ();
  }

  #endregion
}
}
