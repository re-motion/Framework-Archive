using System;
using System.Collections.Generic;
using Rubicon;
using Rubicon.Utilities;

namespace Mixins.Utilities.Serialization
{
  using PreparationFunction = Func<object, object>;
  using FixupFunction = Func<object, object>;
  using System.Reflection;

  public class SerializationFixupRegistry
  {
    private static Dictionary<Type, PreparationFunction> s_preparationFunctions;
    private static Dictionary<Type, FixupFunction> s_fixupFunctions;

    static SerializationFixupRegistry()
    {
      s_preparationFunctions = new Dictionary<Type, Func<object, object>>();
      s_fixupFunctions = new Dictionary<Type, Func<object, object>>();

      AddFixupFunctions (typeof (MethodInfo), MethodInfoFixupData.PrepareMethodInfo, MethodInfoFixupData.FixupMethodInfo);
      AddFixupFunctions (typeof (PropertyInfo), PropertyInfoFixupData.PreparePropertyInfo, PropertyInfoFixupData.FixupPropertyInfo);
      AddFixupFunctions (typeof (EventInfo), EventInfoFixupData.PrepareEventInfo, EventInfoFixupData.FixupEventInfo);
      AddFixupFunctions (typeof (Type), TypeFixupData.PrepareType, TypeFixupData.FixupType);
      AddFixupFunctions (typeof (Delegate), DelegateFixupData.PrepareDelegate, DelegateFixupData.FixupDelegate);
    }

    private static void AddFixupFunctions (Type dataType, PreparationFunction preparationFunction, FixupFunction fixupFunction)
    {
      ArgumentUtility.CheckNotNull ("dataType", dataType);
      ArgumentUtility.CheckNotNull ("preparationFunction", preparationFunction);
      ArgumentUtility.CheckNotNull ("fixupFunction", fixupFunction);

      s_preparationFunctions.Add (dataType, preparationFunction);
      s_fixupFunctions.Add (dataType, fixupFunction);
    }

    public static PreparationFunction GetPreparationFunction (Type dataType)
    {
      ArgumentUtility.CheckNotNull ("dataType", dataType);

      while (dataType != null)
      {
        if (s_preparationFunctions.ContainsKey (dataType))
          return s_preparationFunctions[dataType];
        dataType = dataType.BaseType;
      }

      return null;
    }

    public static FixupFunction GetFixupFunction (Type dataType)
    {
      ArgumentUtility.CheckNotNull ("dataType", dataType);

      while (dataType != null)
      {
        if (s_fixupFunctions.ContainsKey (dataType))
          return s_fixupFunctions[dataType];
        dataType = dataType.BaseType;
      }

      return null;
    }
  }
}
