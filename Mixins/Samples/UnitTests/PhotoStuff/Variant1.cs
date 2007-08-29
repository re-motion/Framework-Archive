using System;
using Rubicon.Mixins;
using Rubicon.Mixins.Samples.PhotoStuff;
using Rubicon.Mixins.Samples.PhotoStuff.Variant1;
using NUnit.Framework;
using System.Reflection;

namespace Rubicon.Mixins.Samples.UnitTests.PhotoStuff
{
  [TestFixture]
  public class Variant1
  {
    [Test]
    public void StoredMembers()
    {
      using (MixinConfiguration.ScopedExtend(Assembly.GetExecutingAssembly()))
      {
        Photo photo = ObjectFactory.Create<Photo>().With();
        Assert.IsNotNull (photo.Document);
        PropertyInfo[] properties = Array.FindAll (photo.GetType ().GetProperties (BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance),
            delegate (PropertyInfo pi)
            {
              return pi.IsDefined (typeof (StoredAttribute), false);
            });

        Assert.AreEqual (1, properties.Length);
      }
    }

    [Test]
    public void InitializeWithConcreteDocument()
    {
      using (MixinConfiguration.ScopedExtend(Assembly.GetExecutingAssembly ()))
      {
        Document doc = new Document();
        doc.CreatedAt = new DateTime (2006, 01, 01);
        Photo photo = ObjectFactory.CreateWithMixinInstances<Photo> (doc).With ();
        Assert.IsNotNull (photo.Document);
        Assert.AreEqual (new DateTime (2006, 01, 01), photo.Document.CreatedAt);
      }
    }
  }
}
