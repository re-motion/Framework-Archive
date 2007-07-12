using System;
using System.Collections.Generic;
using System.Text;
using Rubicon.Mixins.Definitions;

namespace Rubicon.Mixins.CodeGeneration
{
  public class GuidNameProvider : INameProvider
  {
    public static readonly GuidNameProvider Instance = new GuidNameProvider ();

    private GuidNameProvider ()
    {
    }
    
    public string GetNewTypeName (ClassDefinitionBase configuration)
    {
      return string.Format ("{0}_Concrete_{1}", configuration.FullName, Guid.NewGuid ().ToString ("N"));
    }
  }
}
