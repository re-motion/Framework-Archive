using System;
using System.Collections.Generic;
using System.Reflection;
using Rubicon.ObjectBinding.BindableObject;

namespace Rubicon.ObjectBinding.UnitTests.Core.BindableObject.TestDomain
{
  public class UseSpecialMetadataFactoryAttribute : UseCustomMetadataFactoryAttribute
  {
    public override IMetadataFactory GetFactoryInstance ()
    {
      return SpecialMetadataFactory.Instance;
    }
  }

  public class SpecialMetadataFactory : DefaultMetadataFactory
  {
    public static readonly new SpecialMetadataFactory Instance = new SpecialMetadataFactory ();

    private class SpecialPropertyFinder : IPropertyFinder
    {
      private readonly Type _targetType;

      public SpecialPropertyFinder (Type targetType)
      {
        _targetType = targetType;
      }

      public IEnumerable<PropertyInfo> GetPropertyInfos ()
      {
        return _targetType.GetProperties (BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
      }
    }

    public override IPropertyFinder CreatePropertyFinder (Type targetType)
    {
      return new SpecialPropertyFinder (targetType);
    }
  }
}