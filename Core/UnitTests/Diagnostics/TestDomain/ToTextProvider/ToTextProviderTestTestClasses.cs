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
using Remotion.Development.UnitTesting;
using Remotion.Development.UnitTesting.ObjectMother;

namespace Remotion.UnitTests.Diagnostics.TestDomain.ToTextProvider
{
  public class TestSimple
  {
    public TestSimple ()
    {
      Name = "ABC abc";
      Int = 54321;
    }

    public TestSimple (string name, int i)
    {
      Name = name;
      Int = i;
    }

    public string Name { get; set; }
    public int Int { get; set; }

    public override string ToString ()
    {
      return String.Format ("((TestSimple) Name:{0},Int:{1})", Name, Int);
    }
  }

  public class TestSimple2
  {
    public TestSimple2 ()
    {
      PubProp = "%public_property%";
      PrivateProp = "*private*";
      Dev.Null = privateField; // get rid of warning
    }

    public string PubProp { get; set; }
    private string PrivateProp { get; set; }

    public string pubField = "%public_field%";
    private string privateField = "*private_field*";
  }

  public interface ITestName
  {
    string Name { get; }
  }

  public interface ITestInt
  {
    int Int { get; }
  }

  public interface ITestListListString
  {
    List<List<string>> ListListString { get; }
  }


  public class Test : ITestName, ITestListListString, ITestInt
  {
    public Test ()
    {
      Name = "DefaultName";
      Int = 1234567;
    }

    public Test (string name, int i0)
    {
      Name = name;
      Int = i0;
      ListListString = new List<List<string>> ();

      Dev.Null = _privateFieldString;
      Dev.Null = _privateFieldListList;
    }

    public string Name { get; set; }
    public int Int { get; set; }
    public LinkedList<string> LinkedListString { get; set; }
    public List<List<string>> ListListString { get; set; }
    public Object[][][] Array3D { get; set; }
    public Object[,] RectangularArray2D { get; set; }
    public Object[, ,] RectangularArray3D { get; set; }

    private string _privateFieldString = "FieldString text";
    private List<List<string>> _privateFieldListList = Development.UnitTesting.ObjectMother.List.New (Development.UnitTesting.ObjectMother.List.New ("private", "field"), List.New ("list of", "list"));
  }

  public class TestChild : Test
  {
    public TestChild ()
    {
      Name = "Child Name";
      Int = 22222;
    }
  }

  public class TestChildChild : TestChild
  {
    public TestChildChild ()
    {
      Name = "CHILD CHILD NAME";
      Int = 333333333;
    }
  }


  public interface ITest2Name
  {
    string Name { get; }
  }

  public class Test2 : ITest2Name
  {
    public Test2 ()
    {
      Name = "DefaultName";
      Int = 1234567;
    }

    public Test2 (string name, int i0)
    {
      Name = name;
      Int = i0;
      ListListString = new List<List<string>> ();
    }

    public string Name { get; set; }
    public int Int { get; set; }
    public LinkedList<string> LinkedListString { get; set; }
    public List<List<string>> ListListString { get; set; }
    public Object[][][] Array3D { get; set; }
    public Object[,] RectangularArray2D { get; set; }
    public Object[, ,] RectangularArray3D { get; set; }

    public override string ToString ()
    {
      return String.Format ("<Name->{0}><Int->{1}><ListListString->{2}>", Name, Int, ListListString);
    }
  }
}