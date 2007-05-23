using System;
using System.Reflection;
using System.Runtime.Serialization;
using Rubicon;

namespace Mixins.Utilities.Serialization
{
  [Serializable]
  public abstract class SafeISerializableBase : ISerializable
  {
    public SafeISerializableBase ()
    {
    }

    protected SafeISerializableBase (SerializationInfo info, StreamingContext context)
    {
      object[] data = (object[]) info.GetValue ("data", typeof (object[]));
      string[] fixupTypeNames = (string[]) info.GetValue ("fixupTypeNames", typeof (string[]));
      MemberInfo[] members = GetSerializableMembers ();

      FixupData (data, fixupTypeNames);
      FormatterServices.PopulateObjectMembers (this, members, data);
    }

    public void GetObjectData (SerializationInfo info, StreamingContext context)
    {
      MemberInfo[] members = GetSerializableMembers();
      object[] data = FormatterServices.GetObjectData (this, members);

      string[] fixupTypeNames = PrepareFixups (data);
      info.AddValue ("data", data);
      info.AddValue ("fixupTypeNames", fixupTypeNames);
    }

    protected virtual MemberInfo[] GetSerializableMembers ()
    {
      return FormatterServices.GetSerializableMembers (GetType ());
    }

    private string[] PrepareFixups (object[] data)
    {
      string[] fixupTypeNames = new string[data.Length];
      for (int i = 0; i < data.Length; ++i)
      {
        if (data[i] != null)
        {
          Func<object, object> fixupPreparationFunction = SerializationFixupRegistry.GetPreparationFunction (data[i].GetType());
          if (fixupPreparationFunction != null)
          {
            fixupTypeNames[i] = data[i].GetType().AssemblyQualifiedName;
            data[i] = fixupPreparationFunction (data[i]);
          }
        }
      }
      return fixupTypeNames;
    }

    private void FixupData (object[] data, string[] fixupTypeNames)
    {
      for (int i = 0; i < data.Length; ++i)
      {
        if (fixupTypeNames[i] != null)
        {
          Type fixupType = Type.GetType (fixupTypeNames[i]);
          if (fixupType == null)
            throw new SerializationException (string.Format ("Invalid serialization data: type {0} cannot be found.", fixupTypeNames[i]));

          Func<object, object> fixupFunction = SerializationFixupRegistry.GetFixupFunction (fixupType);
          data[i] = fixupFunction(data[i]);
        }
      }
    }
  }
}
