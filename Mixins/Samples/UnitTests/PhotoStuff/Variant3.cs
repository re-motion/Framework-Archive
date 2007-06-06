using System;
using System.Collections.Generic;
using System.Text;
using Mixins;
using Samples.PhotoStuff;
using Samples.PhotoStuff.Variant3;
using NUnit.Framework;
using System.Reflection;

namespace Samples.UnitTests.PhotoStuff
{
  [TestFixture]
  public class Variant3
  {
    [Test]
    public void StoredMembers()
    {
      using (new MixinConfiguration (Assembly.GetExecutingAssembly()))
      {
        Photo photo = ObjectFactory.Create<Photo>().With();
        Assert.IsNotNull (photo.Document);
        PropertyInfo[] properties = Array.FindAll (photo.GetType().GetProperties (BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance),
            delegate (PropertyInfo pi)
            {
              return pi.IsDefined (typeof (StoredAttribute), false);
            });

        Mixins.CodeGeneration.ConcreteTypeBuilder.Current.Scope.SaveAssembly ();
        Assert.AreEqual (2, properties.Length);
      }
    }

    [Test]
    public void InitializeWithConcreteDocument()
    {
      using (new MixinConfiguration (Assembly.GetExecutingAssembly ()))
      {
        Document doc = new Document();
        doc.CreatedAt = new DateTime (2006, 01, 01);
        Photo photo = ObjectFactory.CreateWithMixinInstances<Photo>(doc).With ();
        Assert.IsNotNull (photo.Document);
        Assert.AreEqual (new DateTime (2006, 01, 01), photo.Document.CreatedAt);
      }
    }
  }
}
