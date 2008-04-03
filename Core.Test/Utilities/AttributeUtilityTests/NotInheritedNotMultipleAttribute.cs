using System;

namespace Remotion.Core.UnitTests.Utilities.AttributeUtilityTests
{
  [AttributeUsage (AttributeTargets.All, Inherited = false, AllowMultiple = false)]
  public class NotInheritedNotMultipleAttribute : Attribute, ICustomAttribute
  { }
}