using System;
using System.Reflection;
using Rubicon.ObjectBinding.BindableObject;

namespace Rubicon.Data.DomainObjects.ObjectBinding
{
  internal class BindableDomainObjectPropertyFinder : ReflectionBasedPropertyFinder
  {
    public BindableDomainObjectPropertyFinder (Type targetType)
        : base (targetType)
    {
    }

    protected override bool PropertyFilter (MemberInfo memberInfo, object filterCriteria)
    {
      return base.PropertyFilter (memberInfo, filterCriteria) && memberInfo.DeclaringType != typeof (DomainObject);
    }
  }
}