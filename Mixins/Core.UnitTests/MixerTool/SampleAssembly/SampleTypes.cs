using System;

namespace Remotion.Mixins.UnitTests.MixerTool.SampleAssembly
{
  public class BaseType
  {
  }

  [Extends (typeof (BaseType))]
  public class Mixin
  {
  }
}