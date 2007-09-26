using System;
using Rubicon.ObjectBinding.BindableObject;

namespace Rubicon.Data.DomainObjects.ObjectBinding
{
  [AttributeUsage (AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
  public class UseBindableDomainObjectMetadataFactoryAttribute : UseCustomMetadataFactoryAttribute
  {
    public override IMetadataFactory GetFactoryInstance ()
    {
      return BindableDomainObjectMetadataFactory.Instance;
    }
  }
}