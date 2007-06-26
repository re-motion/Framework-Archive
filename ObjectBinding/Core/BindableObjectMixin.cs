using System;
using Rubicon.Mixins;

namespace Rubicon.ObjectBinding
{
  public class BindableObjectMixin : Mixin<object>, IBusinessObject
  {
    private IBusinessObjectClass _businessObjectClass;

    public BindableObjectMixin ()
    {
    }

    /// <summary> Gets the <see cref="IBusinessObjectClass"/> of this business object. </summary>
    /// <value> An <see cref="IBusinessObjectClass"/> instance acting as the business object's type. </value>
    public IBusinessObjectClass BusinessObjectClass
    {
      get { return _businessObjectClass; }
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
      throw new NotImplementedException();
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
      throw new NotImplementedException();
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
      throw new NotImplementedException();
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
      throw new NotImplementedException();
    }

    /// <overloads> Gets the string representation of the value accessed through the specified property.  </overloads>
    /// <summary> 
    ///   Gets the string representation of the value accessed through the specified 
    ///   <see cref="IBusinessObjectProperty"/>.
    /// </summary>
    /// <param name="property"> 
    ///   The <see cref="IBusinessObjectProperty"/> used to access the value. Must not be <see langword="null"/>.
    /// </param>
    /// <returns> The string representation of the property value for the <paramref name="property"/> parameter. </returns>
    /// <exception cref="Exception"> 
    ///   Thrown if the <paramref name="property"/> is not part of this business object's class. 
    /// </exception>
    public string GetPropertyString (IBusinessObjectProperty property)
    {
      throw new NotImplementedException();
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
      throw new NotImplementedException();
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
      throw new NotImplementedException();
    }

    protected override void OnInitialized ()
    {
      base.OnInitialized ();

      _businessObjectClass = new BindableObjectClass (Configuration.BaseClass.Type, BindableObjectProvider.Instance);
    }
  }
}