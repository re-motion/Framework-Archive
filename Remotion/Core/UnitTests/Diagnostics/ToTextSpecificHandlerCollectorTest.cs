// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// version 3.0 as published by the Free Software Foundation.
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
using System.Collections;
using System.ComponentModel.Design;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Development.UnitTesting.Logging;
using Remotion.Diagnostics.ToText;
using Remotion.Diagnostics.ToText.Infrastructure;
using Remotion.Reflection;
using Remotion.Utilities;

namespace Remotion.UnitTests.Diagnostics
{
  [TestFixture]
  public class ToTextSpecificHandlerCollectorTest
  {
    private readonly ISimpleLogger log = SimpleLogger.CreateForConsole (false);

    private class TestSimpleSimple
    {
      public TestSimpleSimple ()
      {
        Name = "I like it";
      }

      public string Name { get; set; }

      public override string ToString ()
      {
        return String.Format ("((TestSimple) Name:{0})", Name);
      }
    }

    private class TestSimple : ITestSimpleName, ITestSimpleInt
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

    private interface ITestSimpleName
    {
      string Name { get; set; }
    }

    private interface ITestSimpleInt
    {
      int Int { get; set; }
    }


    //[ToTextHandlerTargetType (typeof (TestSimple))]
    [ToTextSpecificHandler]
    class TestSimpleToTextSpecificTypeHandler : ToTextSpecificTypeHandler<TestSimple>
    {
      public override void ToText (TestSimple accessControlEntry, IToTextBuilder toTextBuilder)
      {
        toTextBuilder.sbLiteral ("[", ",", "]").e ("TestSimple").e (accessControlEntry.Name).e (accessControlEntry.Int).se ();
      }
    }

    [ToTextSpecificHandler]
    class TestSimpleSimpleToTextSpecificTypeHandler : ToTextSpecificTypeHandler<TestSimpleSimple>
    {
      public override void ToText (TestSimpleSimple accessControlEntry, IToTextBuilder toTextBuilder)
      {
        //toTextBuilder.WriteInstanceBegin (typeof (TestSimple)).e ("name", t.Name).WriteInstanceEnd ();
        //toTextBuilder.sbLiteral ("[", ",", "]").s ("name=").e (t.Name).se ();
        toTextBuilder.sbLiteral ("[", ",", "]").e ("TestSimpleSimple").e (accessControlEntry.Name).se ();
      }
    }



    [ToTextSpecificHandler]
    class ITestSimpleIntToTextSpecificInterfaceHandler : ToTextSpecificInterfaceHandler<ITestSimpleInt>
    {
      public override void ToText (ITestSimpleInt t, IToTextBuilder toTextBuilder)
      {
        toTextBuilder.sbLiteral ("[", ",", "]").e ("TestSimple").e (t.Int).se ();
      }
    }

    [ToTextSpecificHandler]
    class ITestSimpleNameToTextSpecificInterfaceHandler : ToTextSpecificInterfaceHandler<ITestSimpleName>
    {
      public override void ToText (ITestSimpleName t, IToTextBuilder toTextBuilder)
      {
        toTextBuilder.sbLiteral ("[", ",", "]").e ("TestSimple").e (t.Name).se ();
      }
    }


    private ToTextSpecificHandlerAttribute RetrieveTextHandlerAttribute<T> ()
    {
      return AttributeUtility.GetCustomAttribute<ToTextSpecificHandlerAttribute> (typeof (T), false);
    }

    private ToTextSpecificHandlerAttribute RetrieveTextHandlerAttribute (Type type)
    {
      return AttributeUtility.GetCustomAttribute<ToTextSpecificHandlerAttribute> (type, false);
    }


    [Test]
    public void RetrieveTestSimpleToTextSpecificTypeHandlerAttributeTest ()
    {
      ToTextSpecificHandlerAttribute attribute = RetrieveTextHandlerAttribute<TestSimpleToTextSpecificTypeHandler>();
      Assert.That (attribute, Is.Not.Null);
      //Assert.That (attribute.Type, Is.EqualTo (typeof (TestSimple)));
    }



    [Test]
    public void SimpleToTextSpecificTypeHandlerAttributeTest ()
    {
      const bool excludeGlobalTypes = true;
      ICollection types = ContextAwareTypeDiscoveryUtility.GetTypeDiscoveryService().GetTypes (typeof (IToTextSpecificTypeHandler), excludeGlobalTypes);

      foreach (Type type in types)
      {
        log.It ("type=" + type);

        if (type == typeof (TestSimpleToTextSpecificTypeHandler)) {
          Assert.That (RetrieveTextHandlerAttribute (type), Is.Not.Null);
          log.It ("attribute found");
          Type baseType = type.BaseType;
          Assert.That (baseType.IsGenericType, Is.True);
          log.It ("baseType.Name=" + baseType.Name);
          Type[] genericArguments = baseType.GetGenericArguments();
          log.Item ("List type arguments ({0}):", genericArguments.Length);
          Assert.That (genericArguments.Length, Is.EqualTo (1));
          Type handledType = genericArguments[0];
          Assert.That (handledType, Is.EqualTo(typeof(TestSimple)));
        }
      }
    }



    [Test]
    public void DiscoverMultipleHandlersTest ()
    {
      var handlerCollector = new ToTextSpecificHandlerCollector ();

      var handlerMap = handlerCollector.CollectTypeHandlers();

      IToTextSpecificTypeHandler simpleHandler;
      handlerMap.TryGetValue (typeof (TestSimple), out simpleHandler);
      Assert.That (simpleHandler, Is.Not.Null);
      Assert.That (simpleHandler is TestSimpleToTextSpecificTypeHandler, Is.True);

      IToTextSpecificTypeHandler simpleSimpleHandler;
      handlerMap.TryGetValue (typeof (TestSimpleSimple), out simpleSimpleHandler);
      Assert.That (simpleSimpleHandler, Is.Not.Null);
      Assert.That (simpleSimpleHandler is TestSimpleSimpleToTextSpecificTypeHandler, Is.True);
    }

    [Test]
    public void DiscoverInterfaceHandlersTest ()
    {
      var handlerCollector = new ToTextSpecificHandlerCollector ();

      var handlerMap = handlerCollector.CollectInterfaceHandlers ();

      //foreach (var pair in handlerMap)
      //{
      //  Log (pair.Key + " " + pair.Value);
      //}

      Assert.That (handlerMap.Count, Is.GreaterThan (0));

      IToTextSpecificInterfaceHandler iTestSimpleIntHandler;
      handlerMap.TryGetValue (typeof (ITestSimpleInt), out iTestSimpleIntHandler);
      Assert.That (iTestSimpleIntHandler, Is.Not.Null);
      Assert.That (iTestSimpleIntHandler is ITestSimpleIntToTextSpecificInterfaceHandler, Is.True);

      IToTextSpecificInterfaceHandler iTestSimpleNameHandler;
      handlerMap.TryGetValue (typeof (ITestSimpleName), out iTestSimpleNameHandler);
      Assert.That (iTestSimpleNameHandler, Is.Not.Null);
      Assert.That (iTestSimpleNameHandler is ITestSimpleNameToTextSpecificInterfaceHandler, Is.True);
    }





    // Logging
    private void LogVariables (string format, params object[] parameters)
    {
      log.Item (format, parameters);
    }
    
    private void Log (string s)
    {
      log.It (s);
    }
  }
}
