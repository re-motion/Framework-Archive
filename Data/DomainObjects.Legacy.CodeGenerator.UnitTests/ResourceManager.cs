using System;
using System.IO;
using Assertion=Remotion.Utilities.Assertion;

namespace Remotion.Data.DomainObjects.Legacy.CodeGenerator.UnitTests
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
      Assertion.IsNotNull (stream, "Resource '{0}.{1}' was not found", resourceManagerType.Namespace, resourceID);

      return stream;
    }
  }
}