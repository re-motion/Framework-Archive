using System;
using Rubicon.Utilities;

namespace Rubicon.Mixins.Context
{
  [Serializable]
  public class ClassContextEventArgs : EventArgs
  {
    public readonly ClassContext ClassContext;

    public ClassContextEventArgs (ClassContext classContext)
    {
      ArgumentUtility.CheckNotNull ("classContext", classContext);
      ClassContext = classContext;
    }
  }
}