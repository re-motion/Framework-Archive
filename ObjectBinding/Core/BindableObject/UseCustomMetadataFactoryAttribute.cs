using System;

namespace Rubicon.ObjectBinding.BindableObject
{
  [AttributeUsage (AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
  public abstract class UseCustomMetadataFactoryAttribute : Attribute
  {
    public abstract IMetadataFactory GetFactoryInstance ();
  }
}