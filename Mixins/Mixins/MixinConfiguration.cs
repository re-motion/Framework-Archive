using System;
using System.Reflection;
using Mixins.Utilities.Singleton;
using Mixins.Context;

namespace Mixins
{
  public class MixinConfiguration : IDisposable
  {
    private static CallContextSingleton<ApplicationContext> _activeContext =
        new CallContextSingleton<ApplicationContext> ("Mixins.MixinConfiguration._activeContext",
        delegate { return ApplicationContextBuilder.BuildDefault(); });

    public static bool HasActiveContext
    {
      get { return _activeContext.HasCurrent; }
    }

    public static ApplicationContext ActiveContext
    {
      get { return _activeContext.Current; }
    }

    public static void SetActiveContext (ApplicationContext context)
    {
      _activeContext.SetCurrent (context);
    }

    private static ApplicationContext PeekActiveContext
    {
      get { return MixinConfiguration.HasActiveContext ? MixinConfiguration.ActiveContext : null; }
    }

    private ApplicationContext _previousContext = null;
    private bool _disposed;

    public MixinConfiguration (ApplicationContext newActiveContext)
    {
      _previousContext = MixinConfiguration.PeekActiveContext;
      MixinConfiguration.SetActiveContext (newActiveContext);
    }

    public MixinConfiguration (Type baseType, params Type[] mixinTypes)
        : this (new ClassContext (baseType, mixinTypes))
    {
    }

    public MixinConfiguration (params ClassContext[] classContexts)
        : this (ApplicationContextBuilder.BuildFromClasses (MixinConfiguration.PeekActiveContext, classContexts))
    {
    }

    public MixinConfiguration (params Assembly[] assemblies)
        : this (ApplicationContextBuilder.BuildFromAssemblies (MixinConfiguration.PeekActiveContext, assemblies))
    {
    }

    public void Dispose ()
    {
      if (!_disposed)
      {
        MixinConfiguration.SetActiveContext (_previousContext);
        _disposed = true;
      }
    }
  }
}
