using System;
using System.Collections.Generic;
using System.Text;

using Rubicon.Utilities;
using System.Configuration.Provider;

namespace Rubicon.Core.UnitTests.Configuration
{
  public class DerivedProviderMock : ProviderMock, IProvider
  {
    public DerivedProviderMock ()
    {
    }
  }
}