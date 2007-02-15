using System;
using System.Collections;
using System.ComponentModel;
using System.Reflection;
using Rubicon.ObjectBinding;
using Rubicon.Utilities;

namespace Rubicon.Data.DomainObjects.ObjectBinding
{
/// <summary>
/// A class for reflection based creation of IBusinessObjectProperties of a type.
/// </summary>
public class BusinessObjectClassReflector
{
  private Type _businessObjectClassType;
  private ReflectionPropertyFactory _propertyFactory;

  /// <summary>
  /// Instantiates a new object.
  /// </summary>
  /// <param name="businessObjectClassType">The type to reflect. Must not be <see langword="null"/>.</param>
  /// <param name="propertyFactory">The factory to use for creating the properties.</param>
  /// <exception cref="System.ArgumentNullException">
  ///   <paramref name="businessObjectClassType"/> is <see langword="null"/>. <br /> - or - <br />
  ///   <paramref name="propertyFactory"/> is <see langword="null"/>.
  /// </exception>
	public BusinessObjectClassReflector (Type businessObjectClassType, ReflectionPropertyFactory propertyFactory)
	{
    ArgumentUtility.CheckNotNull ("businessObjectClassType", businessObjectClassType);
    ArgumentUtility.CheckNotNull ("propertyFactory", propertyFactory);

    _businessObjectClassType = businessObjectClassType;
    _propertyFactory = propertyFactory;
  }

  /// <summary>
  /// Gets the type that is reflected.
  /// </summary>
  /// <value>The type that is reflected.</value>
  public Type BusinessObjectClassType
  {
    get { return _businessObjectClassType; }
  }

  /// <summary>
  /// Gets the full name of the class that is reflected.
  /// </summary>
  /// <value>The full name of the class that is reflected.</value>
  public string Identifier
  {
    get { return _businessObjectClassType.FullName; }
  }

  /// <summary>
  /// Returns an <see cref="Rubicon.ObjectBinding.IBusinessObjectProperty"/> representing the given <paramref name="propertyIdentifier"/>.
  /// </summary>
  /// <param name="propertyIdentifier">The name of the property.</param>
  /// <returns>An instance of <see cref="BaseProperty"/> or derived type representing the given <paramref name="propertyIdentifier"/>, or <see langword="null"/> if not found.</returns>
  // TODO: is a ArgumentUtility.CheckNotNull missing?
  public IBusinessObjectProperty GetPropertyDefinition (string propertyIdentifier)
  {
    PropertyInfo propertyInfo = _businessObjectClassType.GetProperty (propertyIdentifier);
    return propertyInfo == null ? null : _propertyFactory.CreateProperty (propertyInfo);
  }

  /// <summary>
  /// Returns an array of <see cref="Rubicon.ObjectBinding.IBusinessObjectProperty"/> for all properties of the type <see cref="BusinessObjectClassType"/>.
  /// </summary>
  /// <returns>An array of instances of <see cref="BaseProperty"/> or a derived type for each property. If no properties can be found, an empty array is returned.</returns>
  public IBusinessObjectProperty[] GetPropertyDefinitions ()
  {
    PropertyInfo[] propertyInfos = _businessObjectClassType.GetProperties ();
    if (propertyInfos == null)
      return new IBusinessObjectProperty[0];

    ArrayList properties = new ArrayList();
    foreach (PropertyInfo propertyInfo in propertyInfos)
    {
      if (UsePropertyInBusinessObject (propertyInfo))
        properties.Add (_propertyFactory.CreateProperty (propertyInfo));
    }

    return (IBusinessObjectProperty[]) properties.ToArray (typeof (IBusinessObjectProperty));
  }

  private bool UsePropertyInBusinessObject (PropertyInfo propertyInfo)
  {
    EditorBrowsableAttribute[] editorBrowsableAttributes = (EditorBrowsableAttribute[]) propertyInfo.GetCustomAttributes (
        typeof (EditorBrowsableAttribute), true);

    if (editorBrowsableAttributes.Length == 1)
    {
      EditorBrowsableAttribute editorBrowsableAttribute = editorBrowsableAttributes[0];
      if (editorBrowsableAttribute.State == EditorBrowsableState.Never)
        return false;
    }

    //  Prevents the display of the indexers declared in BusinessObject.
    //  Adding "EditorBrowsable (EditorBrowsableState.Never)" to BusinessObject 
    //  might not be the best solution until the final way of hiding properties is established
    if (propertyInfo.Name == "Item")
      return false;

    return true;
  }
}
}
