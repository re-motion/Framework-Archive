using System;
using System.Collections.Generic;
using System.Text;

using Rubicon.Utilities;

namespace Rubicon.Security
{
  public sealed class SecurityFreeSection : IDisposable
  {
    // types

    // static members

    [ThreadStatic]
    private static int s_activeSectionCount = 0;

    public static bool IsActive
    {
      get { return s_activeSectionCount > 0; }
    }

    // member fields

    private bool _isDisposed = false;

    // construction and disposing

    public SecurityFreeSection ()
    {
      s_activeSectionCount++;
    }

    ~SecurityFreeSection ()
    {
      Dispose ();
    }

    void IDisposable.Dispose ()
    {
      Dispose ();
    }

    private void Dispose ()
    {
      if (!_isDisposed)
      {
        s_activeSectionCount--;
        _isDisposed = true;
        GC.SuppressFinalize (this);
      }
    }

    // methods and properties

    public void Leave ()
    {
      Dispose ();
    }
  }
}