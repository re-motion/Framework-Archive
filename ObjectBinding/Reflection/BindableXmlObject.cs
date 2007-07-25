using System;
using System.Xml.Serialization;
using Rubicon.Mixins;
using Rubicon.ObjectBinding.BindableObject;

namespace Rubicon.ObjectBinding.Reflection
{
  [Serializable]
  [BindableObjectWithIdentity]
  [GetObjectServiceType (typeof (XmlReflectionBusinessObjectStorageProvider))]
  public abstract class BindableXmlObject
  {
    protected static T GetObject<T> (Guid id)
      where T:BindableXmlObject
    {
      return (T) XmlReflectionBusinessObjectStorageProvider.Current.GetObject (typeof (T), id);
    }

    protected static T CreateObject<T> ()
       where T : BindableXmlObject
    {
      return XmlReflectionBusinessObjectStorageProvider.Current.CreateObject<T> ();
    }

    protected static T CreateObject<T> (Guid id)
         where T : BindableXmlObject
    {
      return XmlReflectionBusinessObjectStorageProvider.Current.CreateObject<T> (id);
    }
  
    internal Guid _id;

    protected BindableXmlObject ()
    {
    }

    [XmlIgnore]
    public Guid ID
    {
      get { return _id; }
    }

    [XmlIgnore]
    [Override]
    public virtual string DisplayName
    {
      get { return GetType().FullName; }
    }

    [XmlIgnore]
    [Override]
    public string UniqueIdentifier
    {
      get { return _id.ToString(); }
    }

    public void SaveObject ()
    {
      XmlReflectionBusinessObjectStorageProvider.Current.SaveObject (this);
    }
  }
}