using System;
using Rubicon.Mixins.Context;
using Rubicon.Utilities;

namespace Rubicon.Mixins.MixerTool
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