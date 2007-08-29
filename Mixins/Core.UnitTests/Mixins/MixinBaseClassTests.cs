using System;
using Rubicon.Mixins.UnitTests.SampleTypes;
using NUnit.Framework;

namespace Rubicon.Mixins.UnitTests.Mixins
{
  [TestFixture]
  public class MixinBaseClassTests : MixinBaseTest
  {
    public class MixinWithOnInitialize1 : Mixin<object>
    {
      public object ThisValue;

      public MixinWithOnInitialize1()
      {
        try
        {
          object t = This;
          Assert.Fail("Expected InvalidOperationException.");
        }
        catch (InvalidOperationException)
        {
          // good
        }
        catch (Exception e)
        {
          Assert.Fail ("Expected InvalidOperationException, but was: " + e);
        }
        Assert.IsNull (ThisValue);
      }

      protected override void OnInitialized ()
      {
        Assert.IsNotNull (This);
        ThisValue = This;
        base.OnInitialized();
      }
    }

    [Test]
    public void ThisAccessInCtorAndInitialize()
    {
      BaseType1 bt1 = CreateMixedObject<BaseType1> (typeof (MixinWithOnInitialize1)).With();
      MixinWithOnInitialize1 mixin = Mixin.Get<MixinWithOnInitialize1> (bt1);
      Assert.IsNotNull (mixin);
      Assert.IsNotNull (mixin.ThisValue);
    }

    public class MixinWithOnInitialize2 : Mixin<object, IBaseType2>
    {
      public object ThisValue;
      public object BaseValue;

      public MixinWithOnInitialize2 ()
      {
        try
        {
          object t = This;
          Assert.Fail ("Expected InvalidOperationException.");
        }
        catch (InvalidOperationException)
        {
          // good
        }
        catch (Exception e)
        {
          Assert.Fail ("Expected InvalidOperationException, but was: " + e);
        }

        try
        {
          object t = Base;
          Assert.Fail ("Expected InvalidOperationException.");
        }
        catch (InvalidOperationException)
        {
          // good
        }
        catch (Exception e)
        {
          Assert.Fail ("Expected InvalidOperationException, but was: " + e);
        }

        Assert.IsNull (ThisValue);
        Assert.IsNull (BaseValue);
      }

      protected override void OnInitialized ()
      {
        Assert.IsNotNull (This);
        Assert.IsNotNull (Base);
        ThisValue = This;
        BaseValue = Base;
        base.OnInitialized ();
      }
    }

    [Test]
    public void BaseAccessInCtorAndInitialize ()
    {
      BaseType2 bt2 = CreateMixedObject<BaseType2> (typeof (MixinWithOnInitialize2)).With ();
      MixinWithOnInitialize2 mixin = Mixin.Get<MixinWithOnInitialize2> (bt2);
      Assert.IsNotNull (mixin);
      Assert.IsNotNull (mixin.ThisValue);
      Assert.IsNotNull (mixin.BaseValue);
    }
  }
}
