using System;
using Rubicon.Web.UI.Controls;

namespace Rubicon.Web.UntTests.UI.Controls
{

public class EmailAddressValidatorMock: EmailAddressValidator
{
	public new bool IsMatchComplete (string text)
  {
    return base.IsMatchComplete (text);
  }

	public new bool IsMatchUserPart (string text)
  {
    return base.IsMatchUserPart (text);
  }

	public new bool IsMatchDomainPart (string text)
  {
    return base.IsMatchDomainPart (text);
  }
}

}
