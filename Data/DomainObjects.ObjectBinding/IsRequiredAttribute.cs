using System;

namespace Rubicon.Data.DomainObjects.ObjectBinding
{
// TODO Doc: 
[AttributeUsage (AttributeTargets.Field | AttributeTargets.Property)]
public class IsRequiredAttribute : Attribute
{
  private bool _isRequired;

	public IsRequiredAttribute (bool isRequired)
	{
    _isRequired = isRequired;
	}

  public bool IsRequired
  {
    get { return _isRequired; }
    set { _isRequired = value; }
  }
}
}
