using System;

namespace Mixins.CodeGeneration
{
  [AttributeUsage (AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
  public class ExtensionFieldAttribute : Attribute
  {

  }
}
