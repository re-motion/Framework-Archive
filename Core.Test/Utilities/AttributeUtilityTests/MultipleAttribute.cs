using System;

namespace Remotion.Core.UnitTests.Utilities.AttributeUtilityTests
{
  [AttributeUsage(AttributeTargets.All, Inherited = true, AllowMultiple = true)]
  public class MultipleAttribute : Attribute, ICustomAttribute
  { }
}