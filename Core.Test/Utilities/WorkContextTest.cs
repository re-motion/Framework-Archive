using System;
using Rubicon.Utilities;
using NUnit.Framework;

namespace Rubicon.Core.UnitTests.Utilities
{

[TestFixture]
public class WorkContextTest
{
  enum ThrowLocation { main_inside, main_outside, sub1_inside, sub1_outside, sub2_inside, sub2_outside, sub2_1_inside}

  // use inner catch

  [Test]
  public void TestCatchMainInside ()
  {
    Assertion.AssertEquals (
      "? main",
      PerformTest (ThrowLocation.main_inside, true));
  }

  [Test]
  public void TestCatchMainOutside ()
  {
    Assertion.AssertEquals (
      "",
      PerformTest (ThrowLocation.main_outside, true));
  }

  [Test]
  public void TestCatchSub1Inside()
  {
    Assertion.AssertEquals (
      "main\n" + 
      "? sub1",
      PerformTest (ThrowLocation.sub1_inside, true));
  }

  [Test]
  public void TestCatchSub1Outside()
  {
    Assertion.AssertEquals (
      "main",
      PerformTest (ThrowLocation.sub1_outside, true));
  }

  [Test]
  public void TestCatchSub2Inside()
  {
    Assertion.AssertEquals (
      "main\n" + 
      "? sub2",
      PerformTest (ThrowLocation.sub2_inside, true));
  }

  [Test]
  public void TestCatchSub2Outside()
  {
    Assertion.AssertEquals (
      "main",
      PerformTest (ThrowLocation.sub2_outside, true));
  }

  [Test]
  public void TestCatchSub2_1Inside()
  {
    Assertion.AssertEquals (
      "main\n" + 
      "? sub2\n" + 
      "? sub2.1",
      PerformTest (ThrowLocation.sub2_1_inside, true));
  }

  // do not use inner catch

  [Test]
  public void TestNoCatchMainInside ()
  {
    Assertion.AssertEquals (
      "? main",
      PerformTest (ThrowLocation.main_inside, false));
  }

  [Test]
  public void TestNoCatchMainOutside ()
  {
    Assertion.AssertEquals (
      "",
      PerformTest (ThrowLocation.main_outside, false));
  }

  [Test]
  public void TestNoCatchSub1Inside()
  {
    Assertion.AssertEquals (
      "? main\n" + 
      "? sub1",
      PerformTest (ThrowLocation.sub1_inside, false));
  }

  [Test]
  public void TestNoCatchSub1Outside()
  {
    Assertion.AssertEquals (
      "? main",
      PerformTest (ThrowLocation.sub1_outside, false));
  }

  [Test]
  public void TestNoCatchSub2Inside()
  {
    Assertion.AssertEquals (
      "? main\n" + 
      "? sub2",
      PerformTest (ThrowLocation.sub2_inside, false));
  }

  [Test]
  public void TestNoCatchSub2Outside()
  {
    Assertion.AssertEquals (
      "? main",
      PerformTest (ThrowLocation.sub2_outside, false));
  }

  [Test]
  public void TestNoCatchSub2_1Inside()
  {
    Assertion.AssertEquals (
      "? main\n" + 
      "? sub2\n" + 
      "? sub2.1",
      PerformTest (ThrowLocation.sub2_1_inside, false));
  }

  private string PerformTest (ThrowLocation location, bool catchInInnerHandler)
  {
    try
    {
      using (WorkContext ctxMain = WorkContext.EnterNew ("main"))
      {
        try
        {
          using (WorkContext ctxSub1 = WorkContext.EnterNew ("sub1"))
          {
            if (location == ThrowLocation.sub1_inside) throw new Exception (location.ToString());
            ctxSub1.Done();
          }
          if (location == ThrowLocation.sub1_outside) throw new Exception (location.ToString());
        }
        catch (Exception e)
        {
          if (!catchInInnerHandler)
            throw;
          Assertion.AssertEquals (location.ToString(), e.Message);
          return WorkContext.Stack.ToString();
        }
        try
        {
          using (WorkContext ctxSub2 = WorkContext.EnterNew ("sub2"))
          {
            using (WorkContext ctxSub2_1 = WorkContext.EnterNew ("sub2.1"))
            {
              if (location == ThrowLocation.sub2_1_inside) throw new Exception (location.ToString());
              ctxSub2_1.Done();
            }
            if (location == ThrowLocation.sub2_inside) throw new Exception (location.ToString());
            ctxSub2.Done();
          }
          if (location == ThrowLocation.sub2_outside) throw new Exception (location.ToString());
        }
        catch (Exception e)
        {
          if (!catchInInnerHandler)
            throw;
          Assertion.AssertEquals (location.ToString(), e.Message);
          return WorkContext.Stack.ToString();
        }
        if (location == ThrowLocation.main_inside) throw new Exception (location.ToString());
        ctxMain.Done();
      }
      if (location == ThrowLocation.main_outside) throw new Exception (location.ToString());
      return null;
    }
    catch (Exception e)
    {
      Assertion.AssertEquals (location.ToString(), e.Message);
      return WorkContext.Stack.ToString();
    }
  }
}

}
