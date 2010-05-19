// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// as published by the Free Software Foundation; either version 2.1 of the 
// License, or (at your option) any later version.
// 
// re-motion is distributed in the hope that it will be useful, 
// but WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
// GNU Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public License
// along with re-motion; if not, see http://www.gnu.org/licenses.
// 
using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Development.UnitTesting;
using Remotion.Development.UnitTesting.ObjectMother;
using Remotion.Scripting.UnitTests.Diagnostics.ToText;
using Remotion.Scripting.UnitTests.Diagnostics.ToText.Infrastructure;
using Remotion.Scripting.UnitTests.Diagnostics.ToText.Infrastructure.ToTextProviderHandler;
using Remotion.Scripting.UnitTests.Diagnostics.ToTextUnitTests.TestDomain.ToTextProvider;

namespace Remotion.Scripting.UnitTests.Diagnostics.ToTextUnitTests
{

  [TestFixture]
  public class ToTextProviderTest
  {
    private static string ToText (ToTextProvider toText, object o)
    {
      var toTextBuilder = new ToTextBuilder (toText);
      toText.ToText (o, toTextBuilder);
      return toTextBuilder.ToString ();
    }


    [Test]
    public void ObjectTest ()
    {
      ToTextProvider toText = CreateTextProvider();

      toText.Settings.UseAutomaticObjectToText = false;

      Object o = 5711;
      Assert.That (ToText (toText, o), Is.EqualTo (o.ToString()));

      Object o2 = new object();
      Assert.That (ToText (toText, o2), Is.EqualTo (o2.ToString()));
    }

    [Test]
    public void FallbackToStringTest ()
    {
      FallbackToStringTestSingleType (new object());
      FallbackToStringTestSingleType (new Test ("xxx", 123));

      var test2 = new Test2 ("aBc", 789);
      FallbackToStringTestSingleType (test2);
    }

    private void FallbackToStringTestSingleType<T> (T t)
    {
      ToTextProvider toText = CreateTextProvider();
      toText.Settings.UseAutomaticObjectToText = false;
      Assert.That (ToText (toText, t), Is.EqualTo (t.ToString()));
    }


    private IToTextBuilder NamedSequenceBegin (IToTextBuilder toTextBuilder, string name)
    {
      toTextBuilder.WriteSequenceLiteralBegin (name, "[", "", "", ";", "]");
      return toTextBuilder;
    }

    private void RegisterHandlers ()
    {
      ToTextProvider toText = CreateTextProvider();
      toText.RegisterSpecificTypeHandler<Object> ((x, ttb) => NamedSequenceBegin(ttb,"Object").e (x.ToString ()).se());
      toText.RegisterSpecificTypeHandler<Test> ((x, ttb) => NamedSequenceBegin (ttb, "Test").e (x.Name).e (x.Int).se ());
    }

    private void InitTestInstanceContainer (Test test)
    {
      test.ListListString.Add (new List<string>() { "Aaa", "Bbb", "Ccc", "Ddd" });
      test.ListListString.Add (new List<string>() { "R1 C0", "R1 C1" });
      test.ListListString.Add (new List<string>() { "R2 C0", "R2 C1", "R2 C2", "R2 C3", "R2 C4" });
      test.ListListString.Add (new List<string>() { "R3 C0", "R3 C1", "R3 C2" });
    }

    [Test]
    public void RegisteredHandlerTest ()
    {
      ToTextProvider toText = CreateTextProvider();
      toText.RegisterSpecificTypeHandler<Object> ((x, ttb) => NamedSequenceBegin (ttb, "Object").e (x.ToString ()).se ());
      Object o = new object();
      string toTextO = ToText (toText, o);
      //Log ("toTextO=" + toTextO);
      Assert.That (toTextO, Is.EqualTo (String.Format ("[Object: {0}]", o.ToString())));

      toText.RegisterSpecificTypeHandler<Test> ((x, ttb) => NamedSequenceBegin (ttb, "Test").e (x.Name).e (x.Int).se ());
      var test = new Test ("That's not my name", 179);
      string toTextTest = ToText (toText, test);
      //Log ("toTextTest=" + toTextTest);
      Assert.That (toTextTest, Is.EqualTo ("[Test: That's not my name;179]"));
    }

    [Test]
    public void NullTest ()
    {
      ToTextProvider toText = CreateTextProvider();
      Object o = null;
      Assert.That (ToText (toText, o), Is.EqualTo ("null"));
    }

    [Test]
    public void IntToStringFallbackTest ()
    {
      ToTextProvider toText = CreateTextProvider();
      int i = 908;
      Assert.That (ToText (toText, i), Is.EqualTo ("908"));
    }




    [Test]
    public void StringHandlerTest ()
    {
      ToTextProvider toText = CreateTextProvider();
      //toText.RegisterSpecificTypeHandler<String> ((x, ttb) => ttb.s ("\"").ts (x).s ("\""));
      toText.RegisterSpecificTypeHandler<String> ((x, ttb) => ttb.s ("\"").ts (x).s ("\""));
      string toTextTest = ToText (toText, "Some text");
      //Log ("[StringHandlerTest] toTextTest=" + toTextTest);
      Assert.That (toTextTest, Is.EqualTo ("\"Some text\""));
    }

    [Test]
    public void CharHandlerTest ()
    {
      ToTextProvider toText = CreateTextProvider();
      toText.RegisterSpecificTypeHandler<char> ((x, ttb) => ttb.s ("'").ts (x).s ("'"));
      string toTextTest = ToText (toText, 'x');
      //Log ("[CharHandlerTest] toTextTest=" + toTextTest);
      Assert.That (toTextTest, Is.EqualTo ("'x'"));
    }



    [Test]
    public void UseAutomaticObjectToTextTest ()
    {
      ToTextProvider toText = CreateTextProvider();
      var testSimple = new TestSimple();

      toText.Settings.UseAutomaticObjectToText = true;
      var resultAutomaticObjectToText = ToText (toText, testSimple);
      //Log (resultAutomaticObjectToText);

      Assert.That (resultAutomaticObjectToText, NUnit.Framework.SyntaxHelpers.Text.Contains ("ABC abc"));
      Assert.That (resultAutomaticObjectToText, NUnit.Framework.SyntaxHelpers.Text.Contains ("54321"));

      toText.Settings.UseAutomaticObjectToText = false;
      Assert.That (ToText (toText, testSimple), Is.EqualTo (testSimple.ToString()));
    }


    [Test]
    public void AutomaticObjectToTextConfigureTest ()
    {
      ToTextProvider toText = CreateTextProvider();
      var testSimple2 = new TestSimple2();

      toText.Settings.UseAutomaticObjectToText = true;

      toText.Settings.SetAutomaticObjectToTextEmit (true, true, true, true);
      var resultAutomaticObjectToText = ToText (toText, testSimple2);
      //Log (resultAutomaticObjectToText);

      Assert.That (
          resultAutomaticObjectToText,
          Is.EqualTo ("[TestSimple2 PubProp=%public_property%,pubField=%public_field%,PrivateProp=*private*,privateField=*private_field*]"));

      toText.Settings.SetAutomaticObjectToTextEmit (true, true, true, false);
      Assert.That (
          ToText (toText, testSimple2), Is.EqualTo ("[TestSimple2 PubProp=%public_property%,pubField=%public_field%,PrivateProp=*private*]"));

      toText.Settings.SetAutomaticObjectToTextEmit (true, true, false, true);
      Assert.That (
          ToText (toText, testSimple2), Is.EqualTo ("[TestSimple2 PubProp=%public_property%,pubField=%public_field%,privateField=*private_field*]"));

      toText.Settings.SetAutomaticObjectToTextEmit (true, false, true, true);
      Assert.That (
          ToText (toText, testSimple2), Is.EqualTo ("[TestSimple2 PubProp=%public_property%,PrivateProp=*private*,privateField=*private_field*]"));

      toText.Settings.SetAutomaticObjectToTextEmit (false, true, true, true);
      Assert.That (
          ToText (toText, testSimple2), Is.EqualTo ("[TestSimple2 pubField=%public_field%,PrivateProp=*private*,privateField=*private_field*]"));
    }


    [Test]
    public void ClearHandlersTest ()
    {
      ToTextProvider toText = CreateTextProvider();
      toText.Settings.UseAutomaticObjectToText = false;
      toText.RegisterSpecificTypeHandler<Object> ((x, ttb) => ttb.s ("[ClearHandlersTest]").ts (x));
      var o = new object();
      string toTextTest = ToText (toText, o);
      //Log ("[ClearHandlersTest] toTextTest=" + toTextTest);
      Assert.That (ToText (toText, o), Is.EqualTo ("[ClearHandlersTest]" + o));
      toText.ClearSpecificTypeHandlers();
      Assert.That (ToText (toText, o), Is.EqualTo (o.ToString()));
    }

    [Test]
    public void StringNotEnumerableTest ()
    {
      ToTextProvider toText = CreateTextProvider();
      string s = "Wow look at the size of that thing";
      string toTextTest = ToText (toText, s);
      const string toTextTestExpected = "Wow look at the size of that thing";
      Assert.That (toTextTest, Is.EqualTo (toTextTestExpected));
    }


    [Test]
    public void NullEnumerableTest ()
    {
      ToTextProvider toText = CreateTextProvider();
      //toText.RegisterSpecificTypeHandler<Test> ((x, ttb) => ttb.sf ("[Test: {0};{1};{2}]", ToText (toText, x.Name),ToText(toText,x.Int), ToText(toText,x.LinkedListString)));
      toText.RegisterSpecificTypeHandler<Test> ((x, ttb) => ttb.s ("[Test: ").e (x.Name).s(";").e (x.Int).s(";").e (x.LinkedListString).s ("]"));
      var test = new Test ("That's not my name", 179);
      test.LinkedListString = null;
      string toTextTest = ToText (toText, test);
      const string toTextTestExpected = "[Test: That's not my name;179;null]";
      Assert.That (toTextTest, Is.EqualTo (toTextTestExpected));
    }

    [Test]
    public void EmptyEnumerableTest ()
    {
      ToTextProvider toText = CreateTextProvider();
      toText.RegisterSpecificTypeHandler<Test> (
          (x, ttb) => NamedSequenceBegin (ttb, "Test").e (ToText (toText, x.Name)).e (ToText (toText, x.Int)).e (ToText (toText, x.ListListString)).se ());
      var test = new Test ("That's not my name", 179);
      test.LinkedListString = new LinkedList<string>();
      string toTextTest = ToText (toText, test);
      const string toTextTestExpected = "[Test: That's not my name;179;{}]";
      Assert.That (toTextTest, Is.EqualTo (toTextTestExpected));
    }

    [Test]
    public void OneDimensionalEnumerableTest ()
    {
      ToTextProvider toText = CreateTextProvider();
      toText.RegisterSpecificTypeHandler<Test> (
          (x, ttb) => NamedSequenceBegin(ttb,"Test").e(ToText (toText, x.Name)).e(ToText (toText, x.Int)).e(ToText (toText, x.LinkedListString)).se());
          //(x, ttb) => ttb.sf ("[Test: {0};{1};{2}]", ToText (toText, x.Name), ToText (toText, x.Int), ToText (toText, x.LinkedListString)));
      var test = new Test ("That's not my name", 179);
      string[] linkedListInit = { "A1", "A2", "A3", "A4", "A5" };
      test.LinkedListString = new LinkedList<string> (linkedListInit);
      //LogVariables ("LinkedListString.Count={0}", test.LinkedListString.Count);
      string toTextTest = ToText (toText, test);
      const string toTextTestExpected = "[Test: That's not my name;179;{A1,A2,A3,A4,A5}]";
      Log (toTextTest);
      Assert.That (toTextTest, Is.EqualTo (toTextTestExpected));
    }


    [Test]
    public void TwoDimensionalEnumerableTest ()
    {
      ToTextProvider toText = CreateTextProvider();
      toText.RegisterSpecificTypeHandler<Test> (
          (x, ttb) => NamedSequenceBegin (ttb, "Test").e (ToText (toText, x.Name)).e (ToText (toText, x.Int)).e (ToText (toText, x.ListListString)).se ());
          //(x, ttb) => ttb.sf ("[Test: {0};{1};{2}]", ToText (toText, x.Name), ToText (toText, x.Int), ToText (toText, x.ListListString)));
      var test = new Test ("That's not my name", 179);
      test.ListListString = new List<List<string>>() { new List<string>() { "A1", "A2" }, new List<string>() { "B1", "B2", "B3" } };
      string toTextTest = ToText (toText, test);
      Log ("toTextTest=" + toTextTest);
      Assert.That (toTextTest, Is.EqualTo ("[Test: That's not my name;179;{{A1,A2},{B1,B2,B3}}]"));
    }


    [Test]
    public void ThreeDimensionalEnumerableTest ()
    {
      ToTextProvider toText = CreateTextProvider();
      toText.RegisterSpecificTypeHandler<Test> (
          (x, ttb) => NamedSequenceBegin (ttb, "Test").e (ToText (toText, x.Name)).e (ToText (toText, x.Int)).e (ToText (toText, x.Array3D)).se ());
      var test = new Test ("That's not my name", 179);
      test.Array3D = new Object[][][] { new Object[][] { new Object[] { 91, 82, 73, 64 } } };
      string toTextTest = ToText (toText, test);
      Log ("toTextTest=" + toTextTest);
      Assert.That (toTextTest, Is.EqualTo ("[Test: That's not my name;179;{{{91,82,73,64}}}]"));
    }



    [Test]
    public void IGroupingTest ()
    {
      ToTextProvider toText = CreateTextProvider ();

      var list = ListMother.New("Abakus","Ameise","Banane","Zentaur","Zeiserl");
      var grouping = from s in list
              group s by s[0]
              into g 
              select g;

      string toTextTest = ToText (toText, grouping);
      const string toTextTestExpected = "{(A,{Abakus,Ameise}),(B,{Banane}),(Z,{Zentaur,Zeiserl})}";
      Assert.That (toTextTest, Is.EqualTo (toTextTestExpected));
    }    
    
    
    
    [Test]
    public void RectangularArrayTest ()
    {
      ToTextProvider toText = CreateTextProvider();
      toText.RegisterSpecificTypeHandler<Test> (
          (x, ttb) => NamedSequenceBegin (ttb, "Test").e (ToText (toText, x.Name)).e (ToText (toText, x.Int)).e (ToText (toText, x.RectangularArray2D)).se ());
          //(x, ttb) => ttb.sf ("[Test: {0};{1};{2}]", ToText (toText, x.Name), ToText (toText, x.Int), ToText (toText, x.RectangularArray2D)));
      var test = new Test ("That's not my name", 179);
      test.RectangularArray2D = new Object[,] { { 1, 3, 5 }, { 7, 11, 13 } };
      string toTextTest = ToText (toText, test);
      Log ("toTextTest=" + toTextTest);
      Assert.That (toTextTest, Is.EqualTo ("[Test: That's not my name;179;{{1,3,5},{7,11,13}}]"));
    }

    [Test]
    [Ignore]
    public void ComplexArrayTest ()
    {
      ToTextProvider toText = CreateTextProvider ();
      toText.RegisterSpecificTypeHandler<Test> (
          (x, ttb) => NamedSequenceBegin (ttb, "Test").e (x.RectangularArray3D).se ());
      toText.RegisterSpecificTypeHandler<Test2> (
          (x, ttb) => NamedSequenceBegin (ttb, "Test2").e (x.RectangularArray2D).se ());
      var test = new Test ("That's not my name", 179);
      var at2 = new Test2[3, 3, 3];

      for (int i0 = 0; i0 < 2; i0++)
      {
        for (int i1 = 0; i1 < 2; i1++)
        {
          for (int i2 = 0; i2 < 2; i2++)
          {
            var test2 = new Test2 (String.Format ("{0}-{1}-{2}", i0, i1, i2), i0 ^ i1 ^ i2);
            test2.RectangularArray2D = new string[,] { { "A" + i0, "B" + 1 }, { "C" + i2, "D" } };
            at2[i0, i1, i2] = test2;
          }
        }
      }
      test.RectangularArray3D = new Object[,,]
                                {
                                    {
                                        { at2[0, 0, 0], at2[0, 0, 1] }
                                    },
                                };
      string toTextTest = ToText (toText, test);
      Log ("toTextTest=" + toTextTest);
    }


    [Test]
    //[Ignore]
    public void RectangularArrayTest2 ()
    {
      ToTextProvider toText = CreateTextProvider();
      toText.RegisterSpecificTypeHandler<Test> (
          (x, ttb) => NamedSequenceBegin (ttb, "Test").e (x.Name).e (x.Int).e (x.RectangularArray3D).se ());
      toText.RegisterSpecificTypeHandler<Test2> (
          (x, ttb) => NamedSequenceBegin (ttb, "Test2").e (x.Name).e (x.Int).e (x.RectangularArray2D).se ());
      var test = new Test ("That's not my name", 179);
      var at2 = new Test2[3,3,3];

      for (int i0 = 0; i0 < 2; i0++)
      {
        for (int i1 = 0; i1 < 2; i1++)
        {
          for (int i2 = 0; i2 < 2; i2++)
          {
            var test2 = new Test2 (String.Format ("{0}-{1}-{2}", i0, i1, i2), i0 ^ i1 ^ i2);
            test2.RectangularArray2D = new string[,] { { "A" + i0, "B" + 1 }, { "C" + i2, "D" } };
            at2[i0, i1, i2] = test2;
          }
        }
      }
      test.RectangularArray3D = new Object[,,]
                                {
                                    {
                                        { at2[0, 0, 0], at2[0, 0, 1] }, { at2[0, 1, 0], at2[0, 1, 1] },
                                    },
                                    {
                                        { at2[1, 0, 0], at2[1, 0, 1] }, { at2[1, 1, 0], at2[1, 1, 1] },
                                    },
                                };
      string toTextTest = ToText (toText, test);
      Log ("toTextTest=" + toTextTest);
      Assert.That (
          toTextTest,
          Is.EqualTo (
              "[Test: That's not my name;179;{{{[Test2: 0-0-0;0;{{A0,B1},{C0,D}}],[Test2: 0-0-1;1;{{A0,B1},{C1,D}}]},{[Test2: 0-1-0;1;{{A0,B1},{C0,D}}],[Test2: 0-1-1;0;{{A0,B1},{C1,D}}]}},{{[Test2: 1-0-0;1;{{A1,B1},{C0,D}}],[Test2: 1-0-1;0;{{A1,B1},{C1,D}}]},{[Test2: 1-1-0;0;{{A1,B1},{C0,D}}],[Test2: 1-1-1;1;{{A1,B1},{C1,D}}]}}}]"));
    }


    [Test]
    public void AutomaticStringEnclosingTest ()
    {
      ToTextProvider toText = CreateTextProvider();
      var test = "ABC";
      toText.Settings.UseAutomaticStringEnclosing = false;
      Assert.That (ToText (toText, test), Is.EqualTo ("ABC"));
      toText.Settings.UseAutomaticStringEnclosing = true;
      Assert.That (ToText (toText, test), Is.EqualTo ("\"ABC\""));
    }

    [Test]
    public void AutomaticCharEnclosingTest ()
    {
      ToTextProvider toText = CreateTextProvider();
      var test = 'A';
      toText.Settings.UseAutomaticCharEnclosing = false;
      Assert.That (ToText (toText, test), Is.EqualTo ("A"));
      toText.Settings.UseAutomaticCharEnclosing = true;
      Assert.That (ToText (toText, test), Is.EqualTo ("'A'"));
    }


    private void ToTextProviderEnableAutomatics (ToTextProvider toText, bool enable)
    {
      toText.Settings.UseAutomaticObjectToText = enable;
      toText.Settings.UseAutomaticStringEnclosing = enable;
      toText.Settings.UseAutomaticCharEnclosing = enable;
    }

    [Test]
    public void AutomaticObjectToTextTest ()
    {
      ToTextProvider toText = CreateTextProvider();
      ToTextProviderEnableAutomatics (toText, true);
      var test = new Test ("That's not my name", 179);
      test.Array3D = new Object[][][] { new Object[][] { new Object[] { 91, 82, 73, 64 } } };
      test.LinkedListString = new LinkedList<string>();
      string toTextTest = ToText (toText, test);
      const string toTextTestExpected = "[Test Name=\"That's not my name\",Int=179,LinkedListString={},ListListString={},Array3D={{{91,82,73,64}}},RectangularArray2D=null,RectangularArray3D=null,_privateFieldString=\"FieldString text\",_privateFieldListList={{\"private\",\"field\"},{\"list of\",\"list\"}}]";
      Assert.That (toTextTest, Is.EqualTo (toTextTestExpected));
    }


    private void TypeToTextTestDo ()
    {
      ToTextProvider toText = CreateTextProvider();
      toText.Settings.UseAutomaticObjectToText = true;
      var type = typeof (object).GetType();
      string result = toText.ToTextString (type);
    }


    [Test]
    public void RuntimeTypeIsTypeTest ()
    {
      Assert.That (typeof (Type).IsAssignableFrom (typeof (object).GetType()), Is.True);
    }


    [Test]
    public void TypeToTextNoEndlessRecursionTest ()
    {
      //bool timesOut = ThreadRunner.RunTimesOutAfterSeconds (TypeToTextTestDo, 0.1);
      bool timesOut = ThreadRunner.WithSecondsTimeout (TypeToTextTestDo, 0.1).Run();
      Assert.That (timesOut, Is.False);
    }

    [Test]
    public void FloatingPointToTextTest ()
    {
      ToTextProvider toText = CreateTextProvider();

      double d = 1.2345678901234567890;
      Double D = d;

      float f = (float) d;
      Single F = f;

      Assert.That (toText.ToTextString (d), Is.EqualTo ("1.23456789012346"));
      Assert.That (toText.ToTextString (D), Is.EqualTo ("1.23456789012346"));
      Assert.That (toText.ToTextString (f), Is.EqualTo ("1.234568"));
      Assert.That (toText.ToTextString (F), Is.EqualTo ("1.234568"));
    }


    [Test]
    public void UseParentHandlerTest ()
    {
      ToTextProvider toText = CreateTextProvider();

      toText.Settings.UseParentHandlers = true;
      
      var testChildChild = new TestChildChild();

      toText.Settings.ParentHandlerSearchUpToRoot = false;
      toText.Settings.ParentHandlerSearchDepth = 2;
      toText.RegisterSpecificTypeHandler<Test> ((o, ttb) => ttb.s ("Test"));
      Assert.That (toText.ToTextString (testChildChild), Is.EqualTo ("Test"));
      toText.Settings.ParentHandlerSearchDepth = 1;
      Assert.That (toText.ToTextString (testChildChild), Is.EqualTo (testChildChild.ToString()));

      toText.Settings.ParentHandlerSearchDepth = 1;
      toText.RegisterSpecificTypeHandler<TestChild> ((o, ttb) => ttb.s ("TestChild"));
      toText.Settings.ParentHandlerSearchDepth = 0;
      Assert.That (toText.ToTextString (testChildChild), Is.EqualTo (testChildChild.ToString()));

      toText.Settings.ParentHandlerSearchDepth = 10;
      toText.RegisterSpecificTypeHandler<TestChildChild> ((o, ttb) => ttb.s ("TestChildChild"));
      Assert.That (toText.ToTextString (testChildChild), Is.EqualTo ("TestChildChild"));

      toText.ClearSpecificTypeHandlers();
      toText.RegisterSpecificTypeHandler<Test> ((o, ttb) => ttb.s ("Test"));
      toText.Settings.ParentHandlerSearchDepth = 0;
      toText.Settings.ParentHandlerSearchUpToRoot = true;
      Assert.That (toText.ToTextString (testChildChild), Is.EqualTo ("Test"));
      toText.Settings.ParentHandlerSearchUpToRoot = false;
      Assert.That (toText.ToTextString (testChildChild), Is.EqualTo (testChildChild.ToString()));
    }

    [Test]
    public void UseParentHandlerSwitchTest ()
    {
      ToTextProvider toText = CreateTextProvider ();
      var testChild = new TestChild ();

      toText.RegisterSpecificTypeHandler<Test> ((o, ttb) => ttb.s ("Test"));
      toText.Settings.UseParentHandlers = true;
      toText.Settings.ParentHandlerSearchUpToRoot = true;
      toText.Settings.ParentHandlerSearchDepth = 1000;
      Assert.That (toText.ToTextString (testChild), Is.EqualTo ("Test"));
      toText.Settings.UseParentHandlers = false;
      Assert.That (toText.ToTextString (testChild), Is.EqualTo (testChild.ToString ()));
    }


    [Test]
    public void InterfaceTest ()
    {
      ToTextProvider toText = CreateTextProvider ();
      toText.Settings.UseInterfaceHandlers = true;

      toText.RegisterSpecificInterfaceHandlerWithLowestPriority<ITestName> ((o, ttb) => ttb.e ("Name", o.Name));

      Test testInterface = new Test ("�berbauer",3589);
      var result = toText.ToTextString (testInterface);
      Log (result);
      Assert.That (result, Is.EqualTo ("Name=�berbauer"));
    }

    [Test]
    public void InterfaceDisabledTest ()
    {
      ToTextProvider toText = CreateTextProvider ();
      toText.Settings.UseInterfaceHandlers = false;
      toText.RegisterSpecificInterfaceHandlerWithLowestPriority<ITestName> ((o, ttb) => ttb.e ("Name", o.Name));

      Test testInterface = new Test ("�berbauer", 3589);
      var result = toText.ToTextString (testInterface);
      Log (result);
      Assert.That (result, Is.EqualTo (testInterface.ToString()));
    }

    [Test]
    public void InterfacePriorityTest ()
    {
      ToTextProvider toText = CreateTextProvider ();
      toText.Settings.UseInterfaceHandlers = true;

      toText.RegisterSpecificInterfaceHandlerWithLowestPriority<ITestInt> ((o, ttb) => ttb.e ("Int", o.Int));
      toText.RegisterSpecificInterfaceHandlerWithLowestPriority<ITestName> ((o, ttb) => ttb.e ("Name", o.Name));

      Test testInterface = new Test ("�berbauer", 3589);
      var result = toText.ToTextString (testInterface);
      Log (result);
      Assert.That (result, Is.EqualTo ("Int=3589"));
    }

    [Test]
    public void InterfacePriority2Test ()
    {
      ToTextProvider toText = CreateTextProvider ();
      toText.Settings.UseInterfaceHandlers = true;

      toText.RegisterSpecificInterfaceHandlerWithLowestPriority<ITestInt> ((o, ttb) => ttb.e ("Int", o.Int));
      toText.RegisterSpecificInterfaceHandlerWithLowestPriority<ITestName> ((o, ttb) => ttb.e ("Name", o.Name));
      toText.RegisterSpecificInterfaceHandlerWithLowestPriority<ITest2Name> ((o, ttb) => ttb.e ("Name2", o.Name));
      toText.RegisterSpecificInterfaceHandlerWithLowestPriority<ITestListListString> ((o, ttb) => ttb.e ("ListListString", o.ListListString));

      Test2 test2Interface = new Test2 ("�berbauer", 3589);
      var result = toText.ToTextString (test2Interface);
      Log (result);
      Assert.That (result, Is.EqualTo ("Name2=�berbauer"));
    }


    [Test]
    public void InterfaceAppendFirstPriorityTest ()
    {
      ToTextProvider toText = CreateTextProvider ();
      toText.Settings.UseInterfaceHandlers = true;

      toText.RegisterSpecificInterfaceHandlerWithHighestPriority<ITestName> ((o, ttb) => ttb.e ("Name", o.Name));
      toText.RegisterSpecificInterfaceHandlerWithHighestPriority<ITestInt> ((o, ttb) => ttb.e ("Int", o.Int));

      Test testInterface = new Test ("�berbauer", 3589);
      var result = toText.ToTextString (testInterface);
      Log (result);
      Assert.That (result, Is.EqualTo ("Int=3589"));
    }


    [Test]
    public void ToTextProviderNullHandlerTest ()
    {
      ToTextProvider toText = CreateTextProvider ();

      var handler = new ToTextProviderNullHandler();
      var parameters = CreateToTextParameters(null);
      var feedback = new ToTextProviderHandlerFeedback();
      handler.ToTextIfTypeMatches (parameters,feedback);
      Assert.That (feedback.Handled, Is.EqualTo (true));
      var result = parameters.ToTextBuilder.ToString();
      Log (result);
      Assert.That (result, Is.EqualTo ("null"));
    }



    [Test]
    public void ToTextProviderExtendedCtorInterfaceOnlyTest ()
    {
      var test = new Test ();
      var interfaceHandlerMap = new ToTextSpecificHandlerMap<IToTextSpecificInterfaceHandler> ();
      interfaceHandlerMap[typeof (ITestName)] = new ToTextSpecificInterfaceHandlerWrapper<ITestName> ((t, ttb) => ttb.s ("[ToTextProviderExtendedCtorTest:ITestName]"), 0);

      var toTextProvider = new ToTextProvider (null, interfaceHandlerMap);
      TextProviderSetAutomatics (toTextProvider, false);
      toTextProvider.Settings.UseInterfaceHandlers = true;

      Assert.That (toTextProvider.ToTextString (test), Is.EqualTo ("[ToTextProviderExtendedCtorTest:ITestName]"));
    }

    [Test]
    public void ToTextProviderExtendedCtorTest ()
    {
      var typeHandlerMap = new ToTextSpecificHandlerMap<IToTextSpecificTypeHandler> ();
      var test = new Test ();
      typeHandlerMap[test.GetType ()] = new ToTextSpecificTypeHandlerWrapper<Test> ((t, ttb) => ttb.s ("[ToTextProviderExtendedCtorTest:Test]"));

      var toTextProvider = new ToTextProvider (typeHandlerMap, null);
      TextProviderSetAutomatics (toTextProvider, false);
      toTextProvider.Settings.UseInterfaceHandlers = true;

      Assert.That (toTextProvider.ToTextString (test), Is.EqualTo ("[ToTextProviderExtendedCtorTest:Test]"));

    }




    [Test]
    public void OutputComplexitySequenceTest ()
    {
      ToTextProvider toText = CreateTextProvider ();
      toText.RegisterSpecificTypeHandler<Test> (
        (x, ttb) => ttb.sb ().e (x.Name).nl ().e (x.Int).nl ().writeIfFull.e (x.RectangularArray3D).se ());

      To.ToTextProvider.RegisterSpecificTypeHandler<Test> (
        (x, ttb) => ttb.sb ().e (x.Name).nl ().e (x.Int).nl ().writeIfFull.e (x.RectangularArray3D).se ());


      toText.RegisterSpecificTypeHandler<Test2> (
        (x, ttb) => NamedSequenceBegin (ttb, "Test2").e (x.Name).e (x.Int).e (x.RectangularArray2D).se ());
      var test = new Test ("That's not my name", 179);
      var at2 = new Test2[3, 3, 3];

      for (int i0 = 0; i0 < 2; i0++)
      {
        for (int i1 = 0; i1 < 2; i1++)
        {
          for (int i2 = 0; i2 < 2; i2++)
          {
            var test2 = new Test2 (String.Format ("{0}-{1}-{2}", i0, i1, i2), i0 ^ i1 ^ i2);
            test2.RectangularArray2D = new string[,] { { "A" + i0, "B" + 1 }, { "C" + i2, "D" } };
            at2[i0, i1, i2] = test2;
          }
        }
      }
      test.RectangularArray3D = new Object[,,]
                                {
                                    {
                                        { at2[0, 0, 0], at2[0, 0, 1] }, { at2[0, 1, 0], at2[0, 1, 1] },
                                    },
                                    {
                                        { at2[1, 0, 0], at2[1, 0, 1] }, { at2[1, 1, 0], at2[1, 1, 1] },
                                    },
                                };



      var toTextBuilder = new ToTextBuilder (toText);
      toTextBuilder.SetOutputComplexityToSkeleton ();
      toText.ToText (test, toTextBuilder);
      string toTextTest = toTextBuilder.ToString ();
      Log ("toTextTest=" + toTextTest);
    }



    [Test]
    public void BigBozoTest ()
    {
      ToTextProvider toText = CreateTextProvider ();
      toText.RegisterSpecificTypeHandler<Test> (
        (x, ttb) => ttb.sb().e (x.Name).nl().e (x.Int).nl().writeIfFull.e (x.RectangularArray3D).se ());
      toText.RegisterSpecificTypeHandler<Test2> (
        (x, ttb) => NamedSequenceBegin (ttb, "Test2").e (x.Name).e (x.Int).e (x.RectangularArray2D).se ());
      var test = new Test ("That's not my name", 179);
      var at2 = new Test2[3, 3, 3];

      for (int i0 = 0; i0 < 2; i0++)
      {
        for (int i1 = 0; i1 < 2; i1++)
        {
          for (int i2 = 0; i2 < 2; i2++)
          {
            var test2 = new Test2 (String.Format ("{0}-{1}-{2}", i0, i1, i2), i0 ^ i1 ^ i2);
            test2.RectangularArray2D = new string[,] { { "A" + i0, "B" + 1 }, { "C" + i2, "D" } };
            at2[i0, i1, i2] = test2;
          }
        }
      }
      test.RectangularArray3D = new Object[,,]
                                {
                                    {
                                        { at2[0, 0, 0], at2[0, 0, 1] }, { at2[0, 1, 0], at2[0, 1, 1] },
                                    },
                                    {
                                        { at2[1, 0, 0], at2[1, 0, 1] }, { at2[1, 1, 0], at2[1, 1, 1] },
                                    },
                                };



      var toTextBuilder = new ToTextBuilder (toText);
      toTextBuilder.SetOutputComplexityToSkeleton();
      toText.ToText (test, toTextBuilder);
      string toTextTest = toTextBuilder.ToString ();
      Log ("toTextTest=" + toTextTest);
    }

    private enum ToTextProviderTestEnum
    {
      Start = 3,
      Stop = 17
    }

    [Test]
    public void EnumTest ()
    {
      ToTextProvider toText = CreateTextProvider ();
      var enumVal = ToTextProviderTestEnum.Stop;
      var result = ToText (toText, enumVal);
      //Log (result);
      Assert.That (ToText (toText, ToTextProviderTestEnum.Start), Is.EqualTo ("Start"));
      Assert.That (result, Is.EqualTo ("Stop"));
    }




    public static void TextProviderSetAutomatics (ToTextProvider toTextProvider, bool enable)
    {
      toTextProvider.Settings.UseAutomaticObjectToText = enable;
      toTextProvider.Settings.UseAutomaticStringEnclosing = enable;
      toTextProvider.Settings.UseAutomaticCharEnclosing = enable;
      toTextProvider.Settings.UseInterfaceHandlers = enable;
    }

    public static ToTextProvider CreateTextProvider ()
    {
      var toTextProvider = new ToTextProvider ();

      TextProviderSetAutomatics (toTextProvider, false);
      return toTextProvider;
    }

    public static ToTextParameters CreateToTextParameters (object obj)
    {
      var toText = CreateTextProvider();
      var toTextBuilder = new ToTextBuilder (toText);
      Type type = (obj != null) ? obj.GetType() : null;
      var parameters = new ToTextParameters () { Object = obj, Type = type, ToTextBuilder = toTextBuilder };
      return parameters;
    }


    private void Log (string s)
    {
      Console.WriteLine (s);
    }
  }


  static class ToTextTestExtensionMethods
  {
    public static IToTextBuilder ts (this IToTextBuilder toTextBuilder, object obj)
    {
      toTextBuilder.s (obj.ToString ());
      return toTextBuilder;
    }
  }

}
