using System;
using System.Collections.Generic;
using Rubicon.Mixins;
using Rubicon.ObjectBinding.BindableObject.Properties;
using Rubicon.Utilities;

namespace Rubicon.ObjectBinding.BindableObject
{
  //TODO: doc
  public class BindableObjectClass : IBusinessObjectClass
  {
    private readonly Type _type;
    private readonly BindableObjectProvider _businessObjectProvider;
    private readonly PropertyCollection _properties = new PropertyCollection();

    public BindableObjectClass (Type type, BindableObjectProvider businessObjectProvider)
    {
      //TODO: Check for value type
      ArgumentUtility.CheckNotNull ("type", type);
      CheckTypeForBindableObjectMixin (type);
      ArgumentUtility.CheckNotNull ("businessObjectProvider", businessObjectProvider);

      _type = type;
      _businessObjectProvider = businessObjectProvider;
    }

    /// <summary> Returns the <see cref="IBusinessObjectProperty"/> for the passed <paramref name="propertyIdentifier"/>. </summary>
    /// <param name="propertyIdentifier"> 
    ///   A <see cref="String"/> uniquely identifying an <see cref="IBusinessObjectProperty"/> in this
    ///   business object class.
    /// </param>
    /// <returns> Returns the <see cref="IBusinessObjectProperty"/> or <see langword="null"/>. </returns>
    /// <remarks> 
    ///   It is not specified wheter an exception is thrown or <see langword="null"/> is returned if the 
    ///   <see cref="IBusinessObjectProperty"/> could not be found.
    /// </remarks>
    public IBusinessObjectProperty GetPropertyDefinition (string propertyIdentifier)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("propertyIdentifier", propertyIdentifier);

      if (!HasPropertyDefinition (propertyIdentifier))
      {
        throw new KeyNotFoundException (
            string.Format ("The property '{0}' was not found on business object class '{1}'.", propertyIdentifier, Identifier));
      }

      return _properties[propertyIdentifier];
    }

    //TODO: doc
    public bool HasPropertyDefinition (string propertyIdentifier)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("propertyIdentifier", propertyIdentifier);

      return _properties.Contains (propertyIdentifier);
    }

    /// <summary> 
    ///   Returns the <see cref="IBusinessObjectProperty"/> instances defined for this business object class.
    /// </summary>
    /// <returns> An array of <see cref="IBusinessObjectProperty"/> instances.</returns>
    public IBusinessObjectProperty[] GetPropertyDefinitions ()
    {
      return _properties.ToArray();
    }

    /// <summary> Gets the <see cref="IBusinessObjectProvider"/> for this business object class. </summary>
    /// <value> An instance of the <see cref="IBusinessObjectProvider"/> type.</value>
    public IBusinessObjectProvider BusinessObjectProvider
    {
      get { return _businessObjectProvider; }
    }

    /// <summary>
    ///   Gets a flag that specifies whether a referenced object of this business object class needs to be 
    ///   written back to its container if some of its values have changed.
    /// </summary>
    /// <value> <see langword="true"/> if the <see cref="IBusinessObject"/> must be reassigned to its container. </value>
    /// <example>
    ///   The following pseudo code shows how this value affects the binding behaviour.
    ///   <code><![CDATA[
    ///   Address address = person.Address;
    ///   address.City = "Vienna";
    ///   // the RequiresWriteBack property of the 'Address' business object class specifies 
    ///   // whether the following statement is required:
    ///   person.Address = address;
    ///   ]]></code>
    /// </example>
    public bool RequiresWriteBack
    {
      get { return false; }
    }

    /// <summary> Gets the identifier (i.e. the type name) for this business object class. </summary>
    /// <value> 
    ///   A string that uniquely identifies the business object class within the business object model. 
    /// </value>
    public string Identifier
    {
      get { return TypeUtility.GetPartialAssemblyQualifiedName (_type); }
    }

    public Type Type
    {
      get { return _type; }
    }

    internal void SetProperties (IList<PropertyBase> properties)
    {
      ArgumentUtility.CheckNotNull ("properties", properties);

      foreach (PropertyBase property in properties)
        _properties.Add (property);
    }

    protected virtual void CheckTypeForBindableObjectMixin (Type type)
    {
      CheckTypeForBindableObjectMixin<BindableObjectMixin, IBusinessObject> (type);
    }

    protected void CheckTypeForBindableObjectMixin<TBindableObjectMixin, TBusinessObjectInterface> (Type type)
    {
      if (!MixinConfiguration.ActiveContext.ContainsClassContext (type.IsGenericType ? type.GetGenericTypeDefinition() : type)
          || !TypeFactory.GetActiveConfiguration (type).Mixins.ContainsKey (typeof (TBindableObjectMixin)))
      {
        throw new ArgumentException (
            string.Format (
                "Type '{0}' does not implement the '{1}' interface via the '{2}'.",
                type.FullName,
                typeof (TBusinessObjectInterface).FullName,
                typeof (TBindableObjectMixin).FullName),
            "type");
      }
    }
  }
}