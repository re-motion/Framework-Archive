using System;
using System.Collections.Generic;
using System.Text;

using NUnit.Framework;
using Rubicon.Collections;
using System.Security.Principal;

namespace Rubicon.Core.UnitTests.Collections
{
  [TestFixture]
  public class TupelAbcdTest
  {
    [Test]
    public void Initialize ()
    {
      Tupel<int, string, double, DateTime> tupel = new Tupel<int, string, double, DateTime> (1, "X", 2.5, new DateTime (2006, 7, 17, 11, 15, 10));

      Assert.AreEqual (1, tupel.A);
      Assert.AreEqual ("X", tupel.B);
    }

    [Test]
    public void Equals_WithNull ()
    {
      Tupel<int, string, double, DateTime> left = new Tupel<int, string, double, DateTime> (1, "X", 2.5,new DateTime (2006, 7, 17, 11, 15, 10));

      Assert.IsFalse (left.Equals (null));
    }

    [Test]
    public void Equals_WithSame ()
    {
      Tupel<int, string, double, DateTime> tupel = new Tupel<int, string, double, DateTime> (1, "X", 2.5, new DateTime (2006, 7, 17, 11, 15, 10));

      Assert.IsTrue (tupel.Equals (tupel));
    }

    [Test]
    public void Equals_WithEqual ()
    {
      Tupel<int, string, double, DateTime> left = new Tupel<int, string, double, DateTime> (1, "X", 2.5, new DateTime (2006, 7, 17, 11, 15, 10));
      Tupel<int, string, double, DateTime> right = new Tupel<int, string, double, DateTime> (1, "X", 2.5, new DateTime (2006, 7, 17, 11, 15, 10));

      Assert.IsTrue (left.Equals (right));
      Assert.IsTrue (right.Equals (left));
    }

    [Test]
    public void Equals_WithDifferentA ()
    {
      Tupel<int, string, double, DateTime> left = new Tupel<int, string, double, DateTime> (1, "X", 2.5, new DateTime (2006, 7, 17, 11, 15, 10));
      Tupel<int, string, double, DateTime> right = new Tupel<int, string, double, DateTime> (-1, "X", 2.5, new DateTime (2006, 7, 17, 11, 15, 10));

      Assert.IsFalse (left.Equals (right));
      Assert.IsFalse (right.Equals (left));
    }

    [Test]
    public void Equals_WithDiffentB ()
    {
      Tupel<int, string, double, DateTime> left = new Tupel<int, string, double, DateTime> (1, "X", 2.5, new DateTime (2006, 7, 17, 11, 15, 10));
      Tupel<int, string, double, DateTime> right = new Tupel<int, string, double, DateTime> (1, "A", 2.5, new DateTime (2006, 7, 17, 11, 15, 10));

      Assert.IsFalse (left.Equals (right));
      Assert.IsFalse (right.Equals (left));
    }

    [Test]
    public void Equals_WithDiffentC ()
    {
      Tupel<int, string, double, DateTime> left = new Tupel<int, string, double, DateTime> (1, "X", 2.5, new DateTime (2006, 7, 17, 11, 15, 10));
      Tupel<int, string, double, DateTime> right = new Tupel<int, string, double, DateTime> (1, "X", -2.5, new DateTime (2006, 7, 17, 11, 15, 10));

      Assert.IsFalse (left.Equals (right));
      Assert.IsFalse (right.Equals (left));
    }

    [Test]
    public void Equals_WithDiffentD ()
    {
      Tupel<int, string, double, DateTime> left = new Tupel<int, string, double, DateTime> (1, "X", 2.5, new DateTime (2006, 7, 17, 11, 15, 10));
      Tupel<int, string, double, DateTime> right = new Tupel<int, string, double, DateTime> (1, "X", 2.5, new DateTime (2005, 7, 17, 11, 15, 10));

      Assert.IsFalse (left.Equals (right));
      Assert.IsFalse (right.Equals (left));
    }

    [Test]
    public void EqualsObject_WithEqual ()
    {
      Tupel<int, string, double, DateTime> left = new Tupel<int, string, double, DateTime> (1, "X", 2.5, new DateTime (2006, 7, 17, 11, 15, 10));
      Tupel<int, string, double, DateTime> right = new Tupel<int, string, double, DateTime> (1, "X", 2.5, new DateTime (2006, 7, 17, 11, 15, 10));

      Assert.IsTrue (left.Equals ((object) right));
    }

    [Test]
    public void EqualsObject_WithNull ()
    {
      Tupel<int, string, double, DateTime> left = new Tupel<int, string, double, DateTime> (1, "X", 2.5, new DateTime (2006, 7, 17, 11, 15, 10));

      Assert.IsFalse (left.Equals ((object) null));
    }

    [Test]
    public void EqualsObject_WithObject ()
    {
      Tupel<int, string, double, DateTime> left = new Tupel<int, string, double, DateTime> (1, "X", 2.5, new DateTime (2006, 7, 17, 11, 15, 10));

      Assert.IsFalse (left.Equals (new object ()));
    }

    [Test]
    public void TestGetHashCode ()
    {
      Tupel<int, string, double, DateTime> left = new Tupel<int, string, double, DateTime> (1, "X", 2.5, new DateTime (2006, 7, 17, 11, 15, 10));
      Tupel<int, string, double, DateTime> right = new Tupel<int, string, double, DateTime> (1, "X", 2.5, new DateTime (2006, 7, 17, 11, 15, 10));

      Assert.AreEqual (left.GetHashCode (), right.GetHashCode ());
    }

  }
}