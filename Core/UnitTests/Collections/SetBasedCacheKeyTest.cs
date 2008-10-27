/* Copyright (C) 2005 - 2008 rubicon informationstechnologie gmbh
 *
 * This program is free software: you can redistribute it and/or modify it under 
 * the terms of the re:motion license agreement in license.txt. If you did not 
 * receive it, please visit http://www.re-motion.org/licensing.
 * 
 * Unless otherwise provided, this software is distributed on an "AS IS" basis, 
 * WITHOUT WARRANTY OF ANY KIND, either express or implied. 
 */

using System;
using System.Collections.Generic;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Collections;

namespace Remotion.UnitTests.Collections
{
  [TestFixture]
  public class SetBasedCacheKeyTest
  {
    [Test]
    public void Count_Implicit_NoElements ()
    {
      var key = new SetBasedCacheKey<int> ();
      Assert.That (key.Count, Is.EqualTo (0));
    }

    [Test]
    public void Count_Explicit_NoElements ()
    {
      var key = new SetBasedCacheKey<int> (new List<int>());
      Assert.That (key.Count, Is.EqualTo (0));
    }

    [Test]
    public void Count_Explicit_ThreeElements ()
    {
      var key = new SetBasedCacheKey<int> (1, 2, 3);
      Assert.That (key.Count, Is.EqualTo (3));
    }

    [Test]
    public void Count_Explicit_ThreeSimilarElements ()
    {
      var key = new SetBasedCacheKey<int> (1, 1, 1);
      Assert.That (key.Count, Is.EqualTo (1));
    }

    [Test]
    public void GetHashCode_Implicit_NoElements()
    {
      var emptyKey = new SetBasedCacheKey<int> ();
      Assert.That (emptyKey.GetHashCode (), Is.EqualTo (0));
    }

    [Test]
    public void GetHashCode_Explicit_NoElements ()
    {
      var emptyKey = new SetBasedCacheKey<int> (new List<int> ());
      Assert.That (emptyKey.GetHashCode (), Is.EqualTo (0));
    }

    [Test]
    public void GetHashCode_OneElement_Equal ()
    {
      var key1 = new SetBasedCacheKey<int> (1);
      var key2 = new SetBasedCacheKey<int> (1);

      Assert.That (key1.GetHashCode (), Is.EqualTo (key2.GetHashCode ()));
    }

    [Test]
    public void GetHashCode_OneElement_Different ()
    {
      var key1 = new SetBasedCacheKey<int> (1);
      var key2 = new SetBasedCacheKey<int> (2);

      Assert.That (key1.GetHashCode (), Is.Not.EqualTo (key2.GetHashCode ()));
    }

    [Test]
    public void GetHashCode_ThreeElements_Equal ()
    {
      var key1 = new SetBasedCacheKey<int> (1, 2, 3);
      var key2 = new SetBasedCacheKey<int> (1, 2, 3);

      Assert.That (key1.GetHashCode (), Is.EqualTo (key2.GetHashCode ()));
    }

    [Test]
    public void GetHashCode_ThreeElements_OrderAgnostic ()
    {
      var key1 = new SetBasedCacheKey<int> (1, 2, 3);
      var key2 = new SetBasedCacheKey<int> (3, 1, 2);

      Assert.That (key1.GetHashCode (), Is.EqualTo (key2.GetHashCode ()));
    }

    [Test]
    public void GetHashCode_ThreeElements_Different ()
    {
      var key1 = new SetBasedCacheKey<int> (1, 2, 3);
      var key2 = new SetBasedCacheKey<int> (1, 2, 4);

      Assert.That (key1.GetHashCode (), Is.Not.EqualTo (key2.GetHashCode ()));
    }

    [Test]
    public void GetHashCode_SimilarElements_Equal ()
    {
      var key1 = new SetBasedCacheKey<int> (1, 1, 2, 2);
      var key2 = new SetBasedCacheKey<int> (1, 2);

      Assert.That (key1.GetHashCode (), Is.EqualTo (key2.GetHashCode ()));
    }

    [Test]
    public void GetHashCode_DifferentElementNumber_Different ()
    {
      var key1 = new SetBasedCacheKey<int> (1, 2, 3);
      var key2 = new SetBasedCacheKey<int> (1, 2);

      Assert.That (key1.GetHashCode (), Is.Not.EqualTo (key2.GetHashCode ()));
    }

    [Test]
    public void Equals_Implicit_NoElements ()
    {
      var emptyKey1 = new SetBasedCacheKey<int> ();
      var emptyKey2 = new SetBasedCacheKey<int> ();
      Assert.That (emptyKey1, Is.EqualTo (emptyKey2));
    }

    [Test]
    public void Equals_Explicit_NoElements ()
    {
      var emptyKey1 = new SetBasedCacheKey<int> (new List<int> ());
      var emptyKey2 = new SetBasedCacheKey<int> (new List<int> ());
      Assert.That (emptyKey1, Is.EqualTo (emptyKey2));
    }

    [Test]
    public void Equals_Mixed_NoElements ()
    {
      var emptyKey1 = new SetBasedCacheKey<int> (new List<int> ());
      var emptyKey2 = new SetBasedCacheKey<int> ();
      Assert.That (emptyKey1, Is.EqualTo (emptyKey2));
    }

    [Test]
    public void Equals_OneElement_Equal ()
    {
      var key1 = new SetBasedCacheKey<int> (1);
      var key2 = new SetBasedCacheKey<int> (1);

      Assert.That (key1, Is.EqualTo (key2));
    }

    [Test]
    public void Equals_OneElement_Different ()
    {
      var key1 = new SetBasedCacheKey<int> (1);
      var key2 = new SetBasedCacheKey<int> (2);

      Assert.That (key1, Is.Not.EqualTo (key2));
    }

    [Test]
    public void Equals_ThreeElements_Equal ()
    {
      var key1 = new SetBasedCacheKey<int> (1, 2, 3);
      var key2 = new SetBasedCacheKey<int> (1, 2, 3);

      Assert.That (key1, Is.EqualTo (key2));
    }

    [Test]
    public void Equals_ThreeElements_OrderAgnostic ()
    {
      var key1 = new SetBasedCacheKey<int> (1, 2, 3);
      var key2 = new SetBasedCacheKey<int> (3, 1, 2);

      Assert.That (key1, Is.EqualTo (key2));
    }

    [Test]
    public void Equals_ThreeElements_Different ()
    {
      var key1 = new SetBasedCacheKey<int> (1, 2, 3);
      var key2 = new SetBasedCacheKey<int> (1, 2, 4);

      Assert.That (key1, Is.Not.EqualTo (key2));
    }

    [Test]
    public void Equals_ThreeSimilarElements_Equal ()
    {
      var key1 = new SetBasedCacheKey<int> (1, 1, 2);
      var key2 = new SetBasedCacheKey<int> (1, 2, 2);

      Assert.That (key1, Is.EqualTo (key2));
    }

    [Test]
    public void Equals_SimilarElements_Equal ()
    {
      var key1 = new SetBasedCacheKey<int> (1, 1, 2, 2);
      var key2 = new SetBasedCacheKey<int> (1, 2);

      Assert.That (key1, Is.EqualTo (key2));
    }

    [Test]
    public void Equals_DifferentElementNumber_Different ()
    {
      var key1 = new SetBasedCacheKey<int> (1, 2, 3);
      var key2 = new SetBasedCacheKey<int> (1, 2);

      Assert.That (key1, Is.Not.EqualTo (key2));
    }

    [Test]
    public void Equals_DifferentObject_Different ()
    {
      var key1 = new SetBasedCacheKey<int> (1);
      Assert.That (key1, Is.Not.EqualTo (1));
    }
  }
}