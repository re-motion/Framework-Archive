using System;
using System.IO;

using NUnit.Framework;

namespace Rubicon.Data.DomainObjects.UnitTests.Resources
{
// TODO Review: Entire class
public sealed class ResourceManager
{
  // types

  // static members and constants

  public static byte[] GetResource (string resourceID)
  {
    Type resourceManagerType = typeof (ResourceManager);
    using (Stream resourceStream = resourceManagerType.Assembly.GetManifestResourceStream (resourceID))
    {
      byte[] buffer = new byte[resourceStream.Length];
      resourceStream.Read (buffer, 0, buffer.Length);
      return buffer;
    }
  }

  public static byte[] GetImage1 ()
  {
    return GetResource ("Rubicon.Data.DomainObjects.UnitTests.Resources.Image1.png");
  }

  public static byte[] GetImage2 ()
  {
    return GetResource ("Rubicon.Data.DomainObjects.UnitTests.Resources.Image2.png");
  }

  public static byte[] GetImageLarger1MB ()
  {
    return GetResource ("Rubicon.Data.DomainObjects.UnitTests.Resources.ImageLarger1MB.bmp");
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

    if (actual == null)
      Assert.Fail ("Actual array is null, but expected array is not null.");

    Assert.AreEqual (expected.Length, actual.Length, "Array lengths differ.");

    for (int i = 0; i < expected.Length; i++)
    { 
      string assertionMessage = string.Format ("Array's differ at position {0}. Additional information: {1}", i.ToString (), message);
      Assert.AreEqual (expected[i], actual[i], assertionMessage);
    }
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
    AreEqual (GetImage1 (), actual, message);
  }

  public static void IsEqualToImage2 (byte[] actual, string message)
  {
    AreEqual (GetImage2 (), actual, message);
  }

  public static void IsEqualToImageLarger1MB (byte[] actual, string message)
  {
    AreEqual (GetImageLarger1MB (), actual, message);
  }

  public static void IsEmptyImage (byte[] actual, string message)
  {
    AreEqual (new byte[0], actual, message);
  }

  // member fields

  // construction and disposing

  private ResourceManager ()
  {
  }

  // methods and properties

}
}
