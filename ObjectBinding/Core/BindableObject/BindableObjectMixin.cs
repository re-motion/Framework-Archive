using System;
using System.Runtime.Serialization;
using Rubicon.Mixins;
using Rubicon.Mixins.CodeGeneration;
using Rubicon.Mixins.Context;
using Rubicon.Mixins.Definitions;
using Rubicon.ObjectBinding.BindableObject.Properties;
using Rubicon.Utilities;

namespace Rubicon.ObjectBinding.BindableObject
{
  //TODO: doc
  [Serializable]
  public class BindableObjectMixin : Mixin<object>, IBusinessObject
  {
    internal static bool HasMixin (Type targetType)
    {
      ArgumentUtility.CheckNotNull ("targetType", targetType);
      return HasMixin (targetType, typeof (BindableObjectMixin), MixinConfiguration.ActiveContext);
    }

    internal static bool HasMixin (Type targetType, ApplicationContext applicationContext)
    {
      ArgumentUtility.CheckNotNull ("targetType", targetType);
      ArgumentUtility.CheckNotNull ("applicationContext", applicationContext);

      return HasMixin (targetType, typeof (BindableObjectMixin), applicationContext);
    }

    internal static bool HasMixin (Type targetType, Type mixinType, ApplicationContext applicationContext)
    {
      if (applicationContext.ContainsClassContext (targetType.IsGenericType ? targetType.GetGenericTypeDefinition() : targetType))
      {
        foreach (MixinDefinition mixin in TypeFactory.GetConfiguration (targetType, applicationContext).Mixins)
        {
          if (mixinType.IsAssignableFrom (mixin.Type))
            return true;
        }
      }
      return false;
    }

    internal static bool IncludesMixin (Type concreteType)
    {
      ArgumentUtility.CheckNotNull ("concreteType", concreteType);
      return IncludesMixin (concreteType, typeof (BindableObjectMixin), MixinConfiguration.ActiveContext);
    }

    internal static bool IncludesMixin (Type concreteType, Type mixinType, ApplicationContext applicationContext)
    {
      ConcreteMixedTypeAttribute[] attributes = AttributeUtility.GetCustomAttributes<ConcreteMixedTypeAttribute> (concreteType, false);
      if (attributes.Length > 0)
        return HasMixin (attributes[0].BaseType, mixinType, applicationContext);

      return false;
    }

    [NonSerialized]
    private BindableObjectClass _bindableObjectClass;

    public BindableObjectMixin ()
    {
    }

    /// <overloads> Gets the value accessed through the specified property. </overloads>
    /// <summary> Gets the value accessed through the specified <see cref="IBusinessObjectProperty"/>. </summary>
    /// <param name="property"> The <see cref="IBusinessObjectProperty"/> used to access the value. </param>
    /// <returns> The property value for the <paramref name="property"/> parameter. </returns>
    /// <exception cref="Exception">
    ///   Thrown if the <paramref name="property"/> is not part of this business object's class. 
    /// </exception>
    public object GetProperty (IBusinessObjectProperty property)
    {
      PropertyBase propertyBase = ArgumentUtility.CheckNotNullAndType<PropertyBase> ("property", property);

      object nativeValue;

      //TODO: catch and wrap the TargetException
      nativeValue = propertyBase.PropertyInfo.GetValue (This, new object[0]);

      return propertyBase.ConvertFromNativePropertyType (nativeValue);
    }

    /// <summary>
    ///   Gets the value accessed through the <see cref="IBusinessObjectProperty"/> identified by the passed 
    ///   <paramref name="propertyIdentifier"/>. 
    /// </summary>
    /// <param name="propertyIdentifier"> 
    ///   A <see cref="String"/> identifing the <see cref="IBusinessObjectProperty"/> used to access the value. 
    /// </param>
    /// <returns> The property value for the <paramref name="propertyIdentifier"/> parameter. </returns>
    /// <exception cref="Exception"> 
    ///   Thrown if the <see cref="IBusinessObjectProperty"/> identified through the <paramref name="propertyIdentifier"/>
    ///   is not part of this business object's class. 
    /// </exception>
    public object GetProperty (string propertyIdentifier)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("propertyIdentifier", propertyIdentifier);
      return GetProperty (_bindableObjectClass.GetPropertyDefinition (propertyIdentifier));
    }

    /// <overloads> Sets the value accessed through the specified property. </overloads>
    /// <summary> Sets the value accessed through the specified <see cref="IBusinessObjectProperty"/>. </summary>
    /// <param name="property"> 
    ///   The <see cref="IBusinessObjectProperty"/> used to access the value. Must not be <see langword="null"/>.
    /// </param>
    /// <param name="value"> The new value for the <paramref name="property"/> parameter. </param>
    /// <exception cref="Exception"> 
    ///   Thrown if the <paramref name="property"/> is not part of this business object's class. 
    /// </exception>
    public void SetProperty (IBusinessObjectProperty property, object value)
    {
      PropertyBase propertyBase = ArgumentUtility.CheckNotNullAndType<PropertyBase> ("property", property);

      object nativeValue = propertyBase.ConvertToNativePropertyType (value);

      //TODO: catch and wrap the TargetException
      propertyBase.PropertyInfo.SetValue (This, nativeValue, new object[0]);
    }

    /// <summary>
    ///   Sets the value accessed through the <see cref="IBusinessObjectProperty"/> identified by the passed 
    ///   <paramref name="propertyIdentifier"/>. 
    /// </summary>
    /// <param name="propertyIdentifier"> 
    ///   A <see cref="String"/> identifing the <see cref="IBusinessObjectProperty"/> used to access the value. 
    /// </param>
    /// <param name="value"> 
    ///   The new value for the <see cref="IBusinessObjectProperty"/> identified by the 
    ///   <paramref name="propertyIdentifier"/> parameter. 
    /// </param>
    /// <exception cref="Exception"> 
    ///   Thrown if the <see cref="IBusinessObjectProperty"/> identified by the <paramref name="propertyIdentifier"/>
    ///   is not part of this business object's class. 
    /// </exception>
    public void SetProperty (string propertyIdentifier, object value)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("propertyIdentifier", propertyIdentifier);
      SetProperty (_bindableObjectClass.GetPropertyDefinition (propertyIdentifier), value);
    }

    /// <summary> 
    ///   Gets the formatted string representation of the value accessed through the specified 
    ///   <see cref="IBusinessObjectProperty"/>.
    /// </summary>
    /// <param name="property"> 
    ///   The <see cref="IBusinessObjectProperty"/> used to access the value. Must not be <see langword="null"/>.
    /// </param>
    /// <param name="format"> The format string applied by the <b>ToString</b> method. </param>
    /// <returns> The string representation of the property value for the <paramref name="property"/> parameter.  </returns>
    /// <exception cref="Exception"> 
    ///   Thrown if the <paramref name="property"/> is not part of this business object's class. 
    /// </exception>
    public string GetPropertyString (IBusinessObjectProperty property, string format)
    {
      IBusinessObjectStringFormatterService stringFormatterService =
          (IBusinessObjectStringFormatterService)
          BusinessObjectClass.BusinessObjectProvider.GetService (typeof (IBusinessObjectStringFormatterService));
      return stringFormatterService.GetPropertyString ((IBusinessObject) This, property, format);
    }

    /// <summary> 
    ///   Gets the string representation of the value accessed through the <see cref="IBusinessObjectProperty"/> 
    ///   identified by the passed <paramref name="propertyIdentifier"/>.
    /// </summary>
    /// <param name="propertyIdentifier"> 
    ///   A <see cref="String"/> identifing the <see cref="IBusinessObjectProperty"/> used to access the value. 
    /// </param>
    /// <returns> 
    ///   The string representation of the property value for the <see cref="IBusinessObjectProperty"/> identified by the 
    ///   <paramref name="propertyIdentifier"/> parameter. 
    /// </returns>
    /// <exception cref="Exception"> 
    ///   Thrown if the <paramref name="propertyIdentifier"/> is not part of this business object's class. 
    /// </exception>
    public string GetPropertyString (string propertyIdentifier)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("propertyIdentifier", propertyIdentifier);
      return GetPropertyString (_bindableObjectClass.GetPropertyDefinition (propertyIdentifier), null);
    }

    /// <summary> Gets the <see cref="BindableObjectClass"/> of this business object. </summary>
    /// <value> An <see cref="BindableObjectClass"/> instance acting as the business object's type. </value>
    public BindableObjectClass BusinessObjectClass
    {
      get { return _bindableObjectClass; }
    }

    /// <summary> Gets the <see cref="IBusinessObjectClass"/> of this business object. </summary>
    /// <value> An <see cref="IBusinessObjectClass"/> instance acting as the business object's type. </value>
    IBusinessObjectClass IBusinessObject.BusinessObjectClass
    {
      get { return BusinessObjectClass; }
    }

    /// <summary> Gets the human readable representation of this <see cref="IBusinessObject"/>. </summary>
    /// <value> The default implementation returns the <see cref="BusinessObjectClass"/>'s <see cref="IBusinessObjectClass.Identifier"/>. </value>
    public virtual string DisplayName
    {
      get { return _bindableObjectClass.Identifier; }
    }

    /// <summary>
    ///   Gets the value of <see cref="DisplayName"/> if it is accessible and otherwise falls back to the <see cref="string"/> returned by
    ///   <see cref="IBusinessObjectProvider.GetNotAccessiblePropertyStringPlaceHolder"/>.
    /// </summary>
    public string DisplayNameSafe
    {
      get
      {
        if (!_bindableObjectClass.HasPropertyDefinition ("DisplayName"))
          return DisplayName;

        IBusinessObjectProperty displayNameProperty = _bindableObjectClass.GetPropertyDefinition ("DisplayName");
        if (displayNameProperty.IsAccessible (_bindableObjectClass, (IBusinessObject) This))
          return DisplayName;

        return _bindableObjectClass.BusinessObjectProvider.GetNotAccessiblePropertyStringPlaceHolder();
      }
    }

    protected override void OnInitialized ()
    {
      base.OnInitialized();

      InitializeBindableObjectClass();
    }

    [OnDeserialized]
    private void OnDeserialization (StreamingContext context)
    {
      InitializeBindableObjectClass();
    }

    private void InitializeBindableObjectClass ()
    {
      _bindableObjectClass = BindableObjectProvider.Current.GetBindableObjectClass (Configuration.TargetClass.Type);
    }
  }
}