using System;

namespace Rubicon.Core.UnitTests.Utilities
{
  [AttributeUsage (AttributeTargets.All, Inherited = true, AllowMultiple = true)]
  public class InheritedAttribute : Attribute, ICustomAttribute
  { }
}