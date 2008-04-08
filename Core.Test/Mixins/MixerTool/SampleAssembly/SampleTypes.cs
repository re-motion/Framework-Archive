using System;
using Remotion.Mixins;

namespace Remotion.Core.UnitTests.Mixins.MixerTool.SampleAssembly
{
  public class BaseType
  {
  }

  [Extends (typeof (BaseType))]
  public class Mixin
  {
  }
}