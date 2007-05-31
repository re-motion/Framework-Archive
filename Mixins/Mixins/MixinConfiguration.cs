using System;
using Mixins.Utilities.Singleton;
using Mixins.Context;

namespace Mixins
{
  public class MixinConfiguration : IDisposable
  {
    private static CallContextSingleton<ApplicationContext> _activeContext =
        new CallContextSingleton<ApplicationContext> ("Mixins.MixinConfiguration._activeContext",
        delegate { return DefaultContextBuilder.BuildDefaultContext(); });

    public static bool HasActiveContext
    {
      get { return _activeContext.HasCurrent; }
    }

    public static ApplicationContext ActiveContext
    {
      get { return _activeContext.Current; }
    }

    public static void SetActiveContext(ApplicationContext context)
    {
      _activeContext.SetCurrent (context);
    }

    private ApplicationContext _previousContext = null;
    private bool _disposed;

    public MixinConfiguration (ApplicationContext temporaryContext)
    {
      if (MixinConfiguration.HasActiveContext)
        _previousContext = MixinConfiguration.ActiveContext;

      MixinConfiguration.SetActiveContext (temporaryContext);
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
