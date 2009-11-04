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
using System.Linq;
using NUnit.Framework;
using Remotion.Data.DomainObjects.Configuration;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Queries.Configuration;
using System.Reflection;
using Remotion.Logging;

namespace Remotion.Data.DomainObjects.PerformanceTests
{
  /// <summary>
  /// This class is the root to execute a single test in the profiler.
  /// Switch the project type to a ConsoleApplication in order to use it.
  /// </summary>
  public class Root
  {
    // types

    // static members and constants

    // member fields

    // construction and disposing

    private Root()
    {
    }

    // methods and properties

    [STAThread]
    public static void Main (string[] args)
    {
      LogManager.Initialize ();

      //LoadObjectsTest test1 = new LoadObjectsTest();
      //test1.TestFixtureSetUp();

      //// Have all xml files loaded, so if the code is instrumented by a profiler, 
      //// the loading does not falsify the method run times during the first call of GetObject.
      //MappingConfiguration mapping = MappingConfiguration.Current;
      //QueryConfiguration queryConfiguration = DomainObjectsConfiguration.Current.Query;

      //test1.SetUp();
      //test1.LoadObjectsOverRelationTest();
      //test1.TearDown();

      //test1.SetUp ();
      //test1.LoadObjectsOverRelationWithAbstractBaseClass ();
      //test1.TearDown ();

      //test1.TestFixtureTearDown();

      var test2 = new SerializationTest();
      test2.TestFixtureSetUp();

      var testMethods = from m in test2.GetType().GetMethods (BindingFlags.Public | BindingFlags.Instance)
                        where m.IsDefined (typeof (TestAttribute), true) && !m.IsDefined (typeof (IgnoreAttribute), true)
                        orderby m.Name
                        select m;
      foreach (MethodInfo testMethod in testMethods)
      {
        test2.SetUp ();
        testMethod.Invoke (test2, new object[0]);
        test2.TearDown ();
      }

      test2.TestFixtureTearDown();

      Console.ReadLine();
    }
  }
}
