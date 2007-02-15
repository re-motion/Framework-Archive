using System;
using NUnit.Framework;
using Rubicon.Collections;

namespace Rubicon.Core.UnitTests.Collections
{
  [TestFixture]
  public class TupelAbcdTest
  {
    [Test]
    public void Initialize ()
    {
      Tuple<int, string, double, DateTime> tupel = new Tuple<int, string, double, DateTime> (1, "X", 2.5, new DateTime (2006, 7, 17, 11, 15, 10));

      Assert.AreEqual (1, tupel.A);
      Assert.AreEqual ("X", tupel.B);
    }

    [Test]
    public void Equals_WithNull ()
    {
      Tuple<int, string, double, DateTime> left = new Tuple<int, string, double, DateTime> (1, "X", 2.5,new DateTime (2006, 7, 17, 11, 15, 10));

      Assert.IsFalse (left.Equals (null));
    }

    [Test]
    public void Equals_WithSame ()
    {
      Tuple<int, string, double, DateTime> tupel = new Tuple<int, string, double, DateTime> (1, "X", 2.5, new DateTime (2006, 7, 17, 11, 15, 10));

      Assert.IsTrue (tupel.Equals (tupel));
    }

    [Test]
    public void Equals_WithEqual ()
    {
      Tuple<int, string, double, DateTime> left = new Tuple<int, string, double, DateTime> (1, "X", 2.5, new DateTime (2006, 7, 17, 11, 15, 10));
      Tuple<int, string, double, DateTime> right = new Tuple<int, string, double, DateTime> (1, "X", 2.5, new DateTime (2006, 7, 17, 11, 15, 10));

      Assert.IsTrue (left.Equals (right));
      Assert.IsTrue (right.Equals (left));
    }

    [Test]
    public void Equals_WithDifferentA ()
    {
      Tuple<int, string, double, DateTime> left = new Tuple<int, string, double, DateTime> (1, "X", 2.5, new DateTime (2006, 7, 17, 11, 15, 10));
      Tuple<int, string, double, DateTime> right = new Tuple<int, string, double, DateTime> (-1, "X", 2.5, new DateTime (2006, 7, 17, 11, 15, 10));

      Assert.IsFalse (left.Equals (right));
      Assert.IsFalse (right.Equals (left));
    }

    [Test]
    public void Equals_WithDiffentB ()
    {
      Tuple<int, string, double, DateTime> left = new Tuple<int, string, double, DateTime> (1, "X", 2.5, new DateTime (2006, 7, 17, 11, 15, 10));
      Tuple<int, string, double, DateTime> right = new Tuple<int, string, double, DateTime> (1, "A", 2.5, new DateTime (2006, 7, 17, 11, 15, 10));

      Assert.IsFalse (left.Equals (right));
      Assert.IsFalse (right.Equals (left));
    }

    [Test]
    public void Equals_WithDiffentC ()
    {
      Tuple<int, string, double, DateTime> left = new Tuple<int, string, double, DateTime> (1, "X", 2.5, new DateTime (2006, 7, 17, 11, 15, 10));
      Tuple<int, string, double, DateTime> right = new Tuple<int, string, double, DateTime> (1, "X", -2.5, new DateTime (2006, 7, 17, 11, 15, 10));

      Assert.IsFalse (left.Equals (right));
      Assert.IsFalse (right.Equals (left));
    }

    [Test]
    public void Equals_WithDiffentD ()
    {
      Tuple<int, string, double, DateTime> left = new Tuple<int, string, double, DateTime> (1, "X", 2.5, new DateTime (2006, 7, 17, 11, 15, 10));
      Tuple<int, string, double, DateTime> right = new Tuple<int, string, double, DateTime> (1, "X", 2.5, new DateTime (2005, 7, 17, 11, 15, 10));

      Assert.IsFalse (left.Equals (right));
      Assert.IsFalse (right.Equals (left));
    }

    [Test]
    public void EqualsObject_WithEqual ()
    {
      Tuple<int, string, double, DateTime> left = new Tuple<int, string, double, DateTime> (1, "X", 2.5, new DateTime (2006, 7, 17, 11, 15, 10));
      Tuple<int, string, double, DateTime> right = new Tuple<int, string, double, DateTime> (1, "X", 2.5, new DateTime (2006, 7, 17, 11, 15, 10));

      Assert.IsTrue (left.Equals ((object) right));
    }

    [Test]
    public void EqualsObject_WithNull ()
    {
      Tuple<int, string, double, DateTime> left = new Tuple<int, string, double, DateTime> (1, "X", 2.5, new DateTime (2006, 7, 17, 11, 15, 10));

      Assert.IsFalse (left.Equals ((object) null));
    }

    [Test]
    public void EqualsObject_WithObject ()
    {
      Tuple<int, string, double, DateTime> left = new Tuple<int, string, double, DateTime> (1, "X", 2.5, new DateTime (2006, 7, 17, 11, 15, 10));

      Assert.IsFalse (left.Equals (new object ()));
    }

    [Test]
    public void TestGetHashCode ()
    {
      Tuple<int, string, double, DateTime> left = new Tuple<int, string, double, DateTime> (1, "X", 2.5, new DateTime (2006, 7, 17, 11, 15, 10));
      Tuple<int, string, double, DateTime> right = new Tuple<int, string, double, DateTime> (1, "X", 2.5, new DateTime (2006, 7, 17, 11, 15, 10));

      Assert.AreEqual (left.GetHashCode (), right.GetHashCode ());
    }

  }
}