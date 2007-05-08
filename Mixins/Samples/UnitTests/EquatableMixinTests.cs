using System;
using NUnit.Framework;
using Mixins;
using Mixins.CodeGeneration;
using System.Reflection;

namespace Samples.UnitTests
{
  [TestFixture]
  public class EquatableMixinTests
  {
    [ApplyMixin(typeof (EquatableMixin<C>))]
    public class C
    {
      public int I;
      public string S;
      public bool B;
    }

    [Test]
    public void ImplementsEquatable()
    {
      using (new CurrentTypeFactoryScope (Assembly.GetExecutingAssembly ()))
      {
        C c = new C();
        Assert.IsFalse (c is IEquatable<C>);

        C c2 = ObjectFactory.Create<C>().With();
        Assert.IsTrue (c2 is IEquatable<C>);
      }
    }

    [Test]
    [Ignore("TODO: Implement overriding")]
    public void EqualsRespectsMembers ()
    {
      using (new CurrentTypeFactoryScope (Assembly.GetExecutingAssembly ()))
      {
        C c = ObjectFactory.Create<C> ().With ();
        C c2 = ObjectFactory.Create<C> ().With ();
        Assert.AreEqual (c, c2);

        c2.S = "foo";
        Assert.AreNotEqual (c, c2);
        c2.I = 5;
        c2.B = true;
        Assert.AreNotEqual (c, c2);
        c.S = "foo";
        Assert.AreNotEqual (c, c2);
        c.I = 5;
        Assert.AreNotEqual (c, c2);
        c.B = true;
        Assert.AreEqual (c, c2);
      }
    }
  }
}
