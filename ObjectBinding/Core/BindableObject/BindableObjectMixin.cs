using System;
using Rubicon.Mixins;
using Rubicon.Mixins.Context;
using Rubicon.Utilities;

namespace Rubicon.ObjectBinding.BindableObject
{
  //TODO: doc
  [Serializable]
  public class BindableObjectMixin : BindableObjectMixinBase<object>
  {
    public static bool HasMixin (Type targetType)
    {
      ArgumentUtility.CheckNotNull ("targetType", targetType);
      return HasMixin (targetType, MixinConfiguration.ActiveContext);
    }

    internal static bool HasMixin (Type targetType, ApplicationContext applicationContext)
    {
      ArgumentUtility.CheckNotNull ("targetType", targetType);
      ArgumentUtility.CheckNotNull ("applicationContext", applicationContext);

      return HasMixin (targetType, typeof (BindableObjectMixin), applicationContext);
    }

    public static bool IncludesMixin (Type concreteType)
    {
      ArgumentUtility.CheckNotNull ("concreteType", concreteType);
      return IncludesMixin (concreteType, typeof (BindableObjectMixin), MixinConfiguration.ActiveContext);
    }

    public BindableObjectMixin ()
    {
    }

    protected override BindableObjectClass InitializeBindableObjectClass()
    {
      return BindableObjectProvider.Current.GetBindableObjectClass (Configuration.TargetClass.Type);
    }
  }
}