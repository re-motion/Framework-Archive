using System;
using Rubicon.Utilities;

namespace Rubicon.ObjectBinding.BindableObject
{
  //TODO: doc
  [AttributeUsage (AttributeTargets.Class | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
  public sealed class BindableObjectSearchServiceTypeAttribute : Attribute
  {
    private readonly Type _type;

    public BindableObjectSearchServiceTypeAttribute (Type type)
    {
      ArgumentUtility.CheckNotNullAndTypeIsAssignableFrom ("type", type, typeof (ISearchAvailableObjectsService));
      _type = type;
    }

    public Type Type
    {
      get { return _type; }
    }
  }
}