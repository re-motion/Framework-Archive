using System;
using System.Collections.Generic;
using System.Text;

using NUnit.Framework;
using Rubicon.Collections;
using System.Security.Principal;

namespace Rubicon.Core.UnitTests.Collections
{
  [TestFixture]
  public class TupelAbcTest
  {
    [Test]
    public void Initialize ()
    {
      Tuple<int, string, double> tupel = new Tuple<int, string, double> (1, "X", 2.5);

      Assert.AreEqual (1, tupel.A);
      Assert.AreEqual ("X", tupel.B);
    }

    [Test]
    public void Equals_WithNull ()
    {
      Tuple<int, string, double> left = new Tuple<int, string, double> (1, "X", 2.5);

      Assert.IsFalse (left.Equals (null));
    }

    [Test]
    public void Equals_WithSame ()
    {
      Tuple<int, string, double> tupel = new Tuple<int, string, double> (1, "X", 2.5);

      Assert.IsTrue (tupel.Equals (tupel));
    }

    [Test]
    public void Equals_WithEqual ()
    {
      Tuple<int, string, double> left = new Tuple<int, string, double> (1, "X", 2.5);
      Tuple<int, string, double> right = new Tuple<int, string, double> (1, "X", 2.5);

      Assert.IsTrue (left.Equals (right));
      Assert.IsTrue (right.Equals (left));
    }

    [Test]
    public void Equals_WithDifferentA ()
    {
      Tuple<int, string, double> left = new Tuple<int, string, double> (1, "X", 2.5);
      Tuple<int, string, double> right = new Tuple<int, string, double> (-1, "X", 2.5);

      Assert.IsFalse (left.Equals (right));
      Assert.IsFalse (right.Equals (left));
    }

    [Test]
    public void Equals_WithDiffentB ()
    {
      Tuple<int, string, double> left = new Tuple<int, string, double> (1, "X", 2.5);
      Tuple<int, string, double> right = new Tuple<int, string, double> (1, "A", 2.5);

      Assert.IsFalse (left.Equals (right));
      Assert.IsFalse (right.Equals (left));
    }

    [Test]
    public void Equals_WithDiffentC ()
    {
      Tuple<int, string, double> left = new Tuple<int, string, double> (1, "X", 2.5);
      Tuple<int, string, double> right = new Tuple<int, string, double> (1, "X", -2.5);

      Assert.IsFalse (left.Equals (right));
      Assert.IsFalse (right.Equals (left));
    }

    [Test]
    public void EqualsObject_WithEqual ()
    {
      Tuple<int, string, double> left = new Tuple<int, string, double> (1, "X", 2.5);
      Tuple<int, string, double> right = new Tuple<int, string, double> (1, "X", 2.5);

      Assert.IsTrue (left.Equals ((object) right));
    }

    [Test]
    public void EqualsObject_WithNull ()
    {
      Tuple<int, string, double> left = new Tuple<int, string, double> (1, "X", 2.5);

      Assert.IsFalse (left.Equals ((object) null));
    }

    [Test]
    public void EqualsObject_WithObject ()
    {
      Tuple<int, string, double> left = new Tuple<int, string, double> (1, "X", 2.5);

      Assert.IsFalse (left.Equals (new object ()));
    }

    [Test]
    public void TestGetHashCode ()
    {
      Tuple<int, string, double> left = new Tuple<int, string, double> (1, "X", 2.5);
      Tuple<int, string, double> right = new Tuple<int, string, double> (1, "X", 2.5);

      Assert.AreEqual (left.GetHashCode (), right.GetHashCode ());
    }

  }
}