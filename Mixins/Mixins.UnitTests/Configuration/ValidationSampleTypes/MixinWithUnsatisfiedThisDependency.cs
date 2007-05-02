using System;
using System.Collections.Generic;
using System.Text;

namespace Mixins.UnitTests.Configuration.ValidationSampleTypes
{
  class MixinWithUnsatisfiedThisDependency : Mixin<IServiceProvider>
  {
  }
}
