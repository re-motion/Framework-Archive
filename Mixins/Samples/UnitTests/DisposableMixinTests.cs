using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using Mixins;
using Mixins.CodeGeneration;
using System.Reflection;

namespace Samples.UnitTests
{
  [TestFixture]
  public class DisposableMixinTests
  {
    public class Data
    {
      public bool ManagedCalled = false;
      public bool UnmanagedCalled = false;
    }

    [Uses (typeof (DisposableMixin))]
    public class C
    {
      public Data Data = new Data();

      [Override]
      public void CleanupManagedResources()
      {
        Data.ManagedCalled = true;
      }

      [Override]
      public void CleanupUnmanagedResources ()
      {
        Data.UnmanagedCalled = true;
      }
    }

    [Test]
    public void DisposeCallsAllCleanupMethods()
    {
      DisposableMixinTests.C c = ObjectFactory.Create<C>().With();
      Data data = c.Data;

      Assert.IsFalse (data.ManagedCalled);
      Assert.IsFalse (data.UnmanagedCalled);
      
      using ((IDisposable)c)
      {
        Assert.IsFalse (data.ManagedCalled);
        Assert.IsFalse (data.UnmanagedCalled);
      }
      Assert.IsTrue (data.ManagedCalled);
      Assert.IsTrue (data.UnmanagedCalled);
      GC.KeepAlive (c);
    }

    [Test]
    public void GCCallsAllUnmanagedCleanup ()
    {
      DisposableMixinTests.C c = ObjectFactory.Create<C> ().With ();
      Data data = c.Data;

      Assert.IsFalse (data.ManagedCalled);
      Assert.IsFalse (data.UnmanagedCalled);

      GC.KeepAlive (c);
      c = null;

      GC.Collect (2, GCCollectionMode.Forced);
      GC.WaitForPendingFinalizers();

      Assert.IsFalse (data.ManagedCalled);
      Assert.IsTrue (data.UnmanagedCalled);
    }
  }
}