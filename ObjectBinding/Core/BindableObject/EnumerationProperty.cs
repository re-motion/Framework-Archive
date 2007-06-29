using System;
using System.Reflection;

namespace Rubicon.ObjectBinding.BindableObject
{
  //TODO: doc
  public class EnumerationProperty : PropertyBase, IBusinessObjectEnumerationProperty
  {
    public EnumerationProperty (BindableObjectProvider businessObjectProvider, PropertyInfo propertyInfo, IListInfo listInfo, bool isRequired)
        : base (businessObjectProvider, propertyInfo, listInfo, isRequired)
    {
    }

    // methods and properties
    /// <summary> Returns a list of all the enumeration's values. </summary>
    /// <returns> 
    ///   A list of <see cref="IEnumerationValueInfo"/> objects encapsulating the values defined in the enumeration. 
    /// </returns>
    public IEnumerationValueInfo[] GetAllValues ()
    {
      throw new NotImplementedException();
    }

    /// <summary> Returns a list of the enumeration's values that can be used in the current context. </summary>
    /// <returns> 
    ///   A list of <see cref="IEnumerationValueInfo"/> objects encapsulating the enabled values in the enumeration. 
    /// </returns>
    /// <remarks> CLS type enums do not inherently support the disabling of its values. </remarks>
    public IEnumerationValueInfo[] GetEnabledValues (IBusinessObject businessObject)
    {
      throw new NotImplementedException();
    }

    /// <overloads> Returns a specific enumeration value. </overloads>
    /// <summary> Returns a specific enumeration value. </summary>
    /// <param name="value"> The enumeration value to return the <see cref="IEnumerationValueInfo"/> for. </param>
    /// <returns> The <see cref="IEnumerationValueInfo"/> object for the provided <paramref name="value"/>. </returns>
    public IEnumerationValueInfo GetValueInfoByValue (object value)
    {
      throw new NotImplementedException();
    }

    /// <summary> Returns a specific enumeration value. </summary>
    /// <param name="identifier"> 
    ///   The string identifying the  enumeration value to return the <see cref="IEnumerationValueInfo"/> for. 
    /// </param>
    /// <returns> The <see cref="IEnumerationValueInfo"/> object for the provided <paramref name="identifier"/>. </returns>
    public IEnumerationValueInfo GetValueInfoByIdentifier (string identifier)
    {
      throw new NotImplementedException();
    }
  }
}