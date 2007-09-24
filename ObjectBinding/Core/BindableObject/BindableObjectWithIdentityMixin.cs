using System;
using Rubicon.Mixins;
using Rubicon.Mixins.Context;
using Rubicon.Utilities;

namespace Rubicon.ObjectBinding.BindableObject
{
  //TODO: doc
  [Serializable]
  public abstract class BindableObjectWithIdentityMixin : BindableObjectMixin, IBusinessObjectWithIdentity
  {
    public static new bool HasMixin (Type targetType)
    {
      ArgumentUtility.CheckNotNull ("targetType", targetType);
      return HasMixin (targetType, MixinConfiguration.ActiveContext);
    }

    internal static new bool HasMixin (Type targetType, ApplicationContext applicationContext)
    {
      ArgumentUtility.CheckNotNull ("targetType", targetType);
      ArgumentUtility.CheckNotNull ("applicationContext", applicationContext);

      return HasMixin (targetType, typeof (BindableObjectWithIdentityMixin), applicationContext);
    }

    public BindableObjectWithIdentityMixin ()
    {
    }

    public abstract string UniqueIdentifier { get; }
  }
}