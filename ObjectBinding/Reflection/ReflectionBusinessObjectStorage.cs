using System;
using System.Collections;
using System.Xml.Serialization;
using System.IO;
using Rubicon.Utilities;

namespace Rubicon.ObjectBinding.Reflection
{

public class ReflectionBusinessObjectStorage
{
  private static string _rootPath = null;

  private static Hashtable _identityMap = new Hashtable(); 

  public static string RootPath 
  {
    get { return _rootPath; }
    set { _rootPath = value; }
  }
  
  public static ReflectionBusinessObject GetObject (Type concreteType, Guid id)
  {
    if (id == Guid.Empty)
      return null;

    ReflectionBusinessObject obj = GetFromIdentityMap (id);
    if (obj != null)
      return obj;

    XmlSerializer serializer = new XmlSerializer (concreteType);

    string typeDir = Path.Combine (_rootPath, concreteType.FullName);
    string fileName = Path.Combine (typeDir, id.ToString());
    if (! File.Exists (fileName))
      return null;
    
    using (FileStream stream = new FileStream (fileName, FileMode.Open, FileAccess.Read))
    {
      obj = (ReflectionBusinessObject) serializer.Deserialize (stream);
      obj._id = id;
      AddToIdentityMap (obj);
      return obj;
    }
  }

  public static Guid GetID (ReflectionBusinessObject obj)
  {
    if (obj == null)
      return Guid.Empty;
    else
      return obj.ID;
  }

  public static void SaveObject (ReflectionBusinessObject obj)
  {
    ArgumentUtility.CheckNotNull ("obj", obj);

    Type concreteType = obj.GetType();
    XmlSerializer serializer = new XmlSerializer (concreteType);

    string typeDir = Path.Combine (_rootPath, concreteType.FullName);
    if (! Directory.Exists (typeDir))
      Directory.CreateDirectory (typeDir);

    string fileName = Path.Combine (typeDir, obj.ID.ToString());

    using (FileStream stream = new FileStream (fileName, FileMode.Create, FileAccess.Write))
    {
      serializer.Serialize (stream, obj);
    }
  }

  public static ReflectionBusinessObject CreateObject (Type concreteType)
  {
    ReflectionBusinessObject obj = (ReflectionBusinessObject) Activator.CreateInstance (concreteType);
    obj._id = Guid.NewGuid();
    AddToIdentityMap (obj);
    return obj;
  }

  private static void AddToIdentityMap (ReflectionBusinessObject obj)
  {
    WeakReference reference = new WeakReference (obj, false);
    _identityMap.Add (obj.ID, reference);
  }

  private static ReflectionBusinessObject GetFromIdentityMap (Guid id)
  {
    WeakReference reference = (WeakReference) _identityMap[id];
    if (reference == null)
      return null;
    return (ReflectionBusinessObject) reference.Target;
  }
}

}
