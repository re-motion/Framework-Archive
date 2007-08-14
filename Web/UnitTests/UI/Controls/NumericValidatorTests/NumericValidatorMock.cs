using System;
using System.Web.UI;
using Rubicon.Utilities;
using Rubicon.Web.UI.Controls;

namespace Rubicon.Web.UnitTests.UI.Controls.NumericValidatorTests
{
  public class NumericValidatorMock : NumericValidator
  {
    private readonly Control _namingContainer;

    public NumericValidatorMock (Control namingContainer)
    {
      ArgumentUtility.CheckNotNull ("namingContainer", namingContainer);
      _namingContainer = namingContainer;
    }

    public override Control NamingContainer
    {
      get { return _namingContainer; }
    }
  }
}