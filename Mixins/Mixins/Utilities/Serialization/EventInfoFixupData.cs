using System;
using System.Reflection;
using Rubicon.Utilities;

namespace Mixins.Utilities.Serialization
{
  [Serializable]
  public class EventInfoFixupData
  {
    private const BindingFlags _bindingFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static;

    private string _typeName;
    private string _eventName;

    public EventInfoFixupData (EventInfo eventInfo)
    {
      ArgumentUtility.CheckNotNull ("eventInfo", eventInfo);

      _typeName = eventInfo.DeclaringType.AssemblyQualifiedName;
      _eventName = eventInfo.Name;
    }

    public EventInfo GetEventInfo ()
    {
      Type declaringType = Type.GetType (_typeName);
      EventInfo eventInfo = declaringType.GetEvent (_eventName, _bindingFlags);
      Assertion.Assert (eventInfo != null);
      return eventInfo;
    }

    internal static object PrepareEventInfo (object data)
    {
      EventInfo eventInfo = data as EventInfo;
      if (eventInfo == null)
        throw new ArgumentException ("Invalid data object - EventInfo expected.", "data");
      return new EventInfoFixupData (eventInfo);
    }

    internal static object FixupEventInfo (object data)
    {
      EventInfoFixupData fixupData = data as EventInfoFixupData;
      if (fixupData == null)
        throw new ArgumentException ("Invalid data object - EventInfoFixupData expected.", "data");
      return fixupData.GetEventInfo ();
    }

  }
}