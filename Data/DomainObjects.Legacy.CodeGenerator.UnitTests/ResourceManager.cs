using System;
using System.IO;
using Assertion=Rubicon.Utilities.Assertion;

namespace Rubicon.Data.DomainObjects.Legacy.CodeGenerator.UnitTests
{
  public static class ResourceManager
  {    
    public static string GetResourceString (string resourceID)
    {
      using (StreamReader streamReader = new StreamReader (GetResourceStream (resourceID)))
      {
        return streamReader.ReadToEnd();
      }
    }

    public static Stream GetResourceStream (string resourceID)
    {
      Type resourceManagerType = typeof (ResourceManager);
      Stream stream = resourceManagerType.Assembly.GetManifestResourceStream (resourceManagerType, resourceID);
      Assertion.Assert (stream != null, "Resource '{0}.{1}' was not found", resourceManagerType.Namespace, resourceID);

      return stream;
    }
  }
}