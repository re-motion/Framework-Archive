using System;
using Rubicon.Mixins;
using Rubicon.Mixins.Context;
using Rubicon.Utilities;

namespace Rubicon.ObjectBinding.BindableObject
{
  internal class DesignModeBindableObjectProvider : BindableObjectProvider
  {
    public DesignModeBindableObjectProvider ()
    {
    }

    public override BindableObjectClass GetBindableObjectClass (Type type)
    {
      ArgumentUtility.CheckNotNull ("type", type);

      using (MixinConfiguration.ScopedReplace (CreateApplicationContextFromType(type)))
      {
        return base.GetBindableObjectClass (type);
      }
    }

    private ApplicationContext CreateApplicationContextFromType (Type type)
    {
      ApplicationContextBuilder applicationContextBuilder = new ApplicationContextBuilder (null);
      applicationContextBuilder.AddType (type);
      return applicationContextBuilder.BuildContext();
    }
  }
}