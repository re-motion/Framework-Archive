using System;
using System.Collections.Generic;
using System.Text;
using Mixins;
using Samples.PhotoStuff;
using Samples.PhotoStuff.Variant1;
using NUnit.Framework;
using System.Reflection;

namespace Samples.UnitTests.PhotoStuff
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
