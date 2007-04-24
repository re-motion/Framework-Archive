using System;
using System.IO;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;

namespace Rubicon.Data.DomainObjects.UnitTests.Resources
{
  public static class ResourceManager
  {
    public const string DomainObjectsConfigurationWithFakeMappingLoader = "DomainObjectsConfigurationWithFakeMappingLoader.xml";
    public const string DomainObjectsConfigurationWithCustomSectionGroupName = "DomainObjectsConfigurationWithCustomSectionGroupName.xml";
    public const string DomainObjectsConfigurationWithMinimumSettings = "DomainObjectsConfigurationWithMinimumSettings.xml";
    
    public static byte[] GetResource (string resourceID)
    {
      using (Stream resourceStream = GetResourceStream (resourceID))
      {
        byte[] buffer = new byte[resourceStream.Length];
        resourceStream.Read (buffer, 0, buffer.Length);
        return buffer;
      }
    }

    public static Stream GetResourceStream (string resourceID)
    {
      Type resourceManagerType = typeof (ResourceManager);
      Stream stream = resourceManagerType.Assembly.GetManifestResourceStream (resourceManagerType, resourceID);
      Rubicon.Utilities.Assertion.Assert (stream != null, "Resource '{0}.{1}' was not found", resourceManagerType.Namespace, resourceID);

      return stream;
    }

    public static byte[] GetImage1()
    {
      return GetResource ("Image1.png");
    }

    public static byte[] GetImage2()
    {
      return GetResource ("Image2.png");
    }

    public static byte[] GetImageLarger1MB()
    {
      return GetResource ("ImageLarger1MB.bmp");
    }

    public static void IsEqualToImage1 (byte[] actual)
    {
      IsEqualToImage1 (actual, null);
    }

    public static void IsEqualToImage2 (byte[] actual)
    {
      IsEqualToImage2 (actual, null);
    }

    public static void IsEqualToImageLarger1MB (byte[] actual)
    {
      IsEqualToImageLarger1MB (actual, null);
    }

    public static void IsEmptyImage (byte[] actual)
    {
      IsEmptyImage (actual, null);
    }

    public static void IsEqualToImage1 (byte[] actual, string message)
    {
      AreEqual (GetImage1(), actual, message);
    }

    public static void IsEqualToImage2 (byte[] actual, string message)
    {
      AreEqual (GetImage2(), actual, message);
    }

    public static void IsEqualToImageLarger1MB (byte[] actual, string message)
    {
      AreEqual (GetImageLarger1MB(), actual, message);
    }

    public static void IsEmptyImage (byte[] actual, string message)
    {
      AreEqual (new byte[0], actual, message);
    }

    public static void AreEqual (byte[] expected, byte[] actual)
    {
      AreEqual (expected, actual, null);
    }

    public static void AreEqual (byte[] expected, byte[] actual, string message)
    {
      if (expected == actual)
        return;

      if (expected == null)
        Assert.Fail ("Expected array is null, but actual array is not null.");
      
      Assert.That (actual, Is.EqualTo (expected));
    }
  }
}