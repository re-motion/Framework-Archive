using System;
using System.Collections.Generic;
using System.Text;

using Rubicon.Utilities;

namespace Rubicon.ObjectBinding
{
  public static class ObjectBindingUtilities
  {
    /// <summary>
    ///   Returns the <see cref="IBusinessObjectWithIdentity.DisplayName"/> for the <paramref name="businessObject"/> or the 
    ///   <see cref="IBusinessObjectProvider.GetNotAccessiblePropertyStringPlaceHolder"/> if the <see cref="IBusinessObjectWithIdentity.DisplayName"/>
    ///   property is not accessible.
    /// </summary>
    /// <param name="businessObject"> The <see cref="IBusinessObjectWithIdentity"/> to get the display name for. </param>
    /// <returns> 
    ///   A <see cref="String"/> containing the <see cref="IBusinessObjectWithIdentity.DisplayName"/> 
    ///   or the <see cref="IBusinessObjectProvider.GetNotAccessiblePropertyStringPlaceHolder"/>.
    /// </returns>
    public static string GetDisplayName (IBusinessObjectWithIdentity businessObject)
    {
      ArgumentUtility.CheckNotNull ("businessObject", businessObject);

      IBusinessObjectClass businessObjectClass = businessObject.BusinessObjectClass;
      IBusinessObjectProperty displayNameProperty = businessObjectClass.GetPropertyDefinition ("DisplayName");
      if (displayNameProperty.IsAccessible (businessObjectClass, businessObject))
        return businessObject.DisplayName;

      return businessObjectClass.BusinessObjectProvider.GetNotAccessiblePropertyStringPlaceHolder ();
    }
  }
}