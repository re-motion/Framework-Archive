using System;

namespace Rubicon.Data.DomainObjects.ObjectBinding
{
/// <summary>
/// Specifies if a property or field is read only.
/// </summary>
[AttributeUsage (AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
public sealed class IsReadOnlyAttribute : Attribute
{
  public IsReadOnlyAttribute ()
	{
	}
}
}
