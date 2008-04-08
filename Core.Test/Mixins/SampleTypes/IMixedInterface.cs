using System;
using Remotion.Mixins;

namespace Remotion.Core.UnitTests.Mixins.SampleTypes
{
  [Uses (typeof (NullMixin))]
  public interface IMixedInterface { }
}