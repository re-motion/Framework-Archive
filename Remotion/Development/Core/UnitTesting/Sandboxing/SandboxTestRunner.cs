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
using System.Reflection;
using System.Security;
using Remotion.Utilities;

namespace Remotion.Development.UnitTesting.Sandboxing
{
  /// <summary>
  /// <see cref="SandboxTestRunner"/> executes unit tests for the given types.
  /// </summary>
  public class SandboxTestRunner : MarshalByRefObject
  {
    public static TestFixtureResult[] RunTestFixturesInSandbox (IEnumerable<Type> testFixtureTypes, IPermission[] permissions, Assembly[] fullTrustAssemblies)
    {
      ArgumentUtility.CheckNotNull ("testFixtureTypes", testFixtureTypes);
      ArgumentUtility.CheckNotNull ("permissions", permissions);
      ArgumentUtility.CheckNotNull ("fullTrustAssemblies", fullTrustAssemblies);

      using (var sandbox = Sandbox.CreateSandbox (permissions, fullTrustAssemblies))
      {
        var runner = sandbox.CreateSandboxedInstance<SandboxTestRunner> (permissions);
        try
        {
          return runner.RunTestFixtures (testFixtureTypes);
        }
        catch (TargetInvocationException ex)
        {
          throw ex.InnerException.PreserveStackTrace();
        }
      }
    }

    public TestFixtureResult[] RunTestFixtures (IEnumerable<Type> testFixtureTypes)
    {
      if (testFixtureTypes == null)
        throw new ArgumentNullException ("testFixtureTypes"); // avoid ArgumentUtility, it doesn't support partial trust ATM

      return testFixtureTypes.Select (t => RunTestFixture (t)).ToArray();
    }

    public TestFixtureResult RunTestFixture (Type type)
    {
      if (type == null)
        throw new ArgumentNullException ("type"); // avoid ArgumentUtility, it doesn't support partial trust ATM

      var testFixtureInstance = Activator.CreateInstance (type);

      var setupMethod = type.GetMethods ().Where (m => IsDefined (m, "NUnit.Framework.SetUpAttribute")).SingleOrDefault ();
      var tearDownMethod = type.GetMethods ().Where (m => IsDefined (m, "NUnit.Framework.TearDownAttribute")).SingleOrDefault ();
      var testMethods = type.GetMethods ().Where (m => IsDefined (m, "NUnit.Framework.TestAttribute"));

      Exception exception;
      var testResults = new List<TestResult>();
      foreach (var testMethod in testMethods)
      {
        if (IsDefined (testMethod, "NUnit.Framework.IgnoreAttribute"))
        {
          testResults.Add (TestResult.CreateIgnored (testMethod));
          continue;
        }

        if (setupMethod!=null && !(TryInvokeMethod (setupMethod, testFixtureInstance, out exception)))
        {
          testResults.Add(TestResult.CreateFailedInSetUp (setupMethod, exception));
          continue;
        }
        
        if (IsDefined (testMethod, "NUnit.Framework.ExpectedExceptionAttribute"))
        {
          var exceptionType = (Type)GetAttribute (testMethod, "NUnit.Framework.ExpectedExceptionAttribute").ConstructorArguments[0].Value;
          if (!TryInvokeMethod (testMethod, testFixtureInstance, out exception) && exception.InnerException.GetType() == exceptionType)
            testResults.Add(TestResult.CreateSucceeded (testMethod));
          else
            testResults.Add (TestResult.CreateFailed (testMethod, exception));
        }
        else
        {
          if (TryInvokeMethod (testMethod, testFixtureInstance, out exception))
            testResults.Add(TestResult.CreateSucceeded (testMethod));
          else
            testResults.Add(TestResult.CreateFailed (testMethod, exception));
        }

        if (tearDownMethod!=null && !TryInvokeMethod (tearDownMethod, testFixtureInstance, out exception))
          testResults.Add (TestResult.CreateFailedInTearDown (tearDownMethod, exception));
      }
      return new TestFixtureResult(type, testResults.ToArray());
    }

    private bool TryInvokeMethod (MethodInfo method, object instance, out Exception exception)
    {
      exception = null;
      try
      {
        method.Invoke (instance, null);
      }
      catch (Exception ex)
      {
        exception = ex;
      }
      return exception==null;
    }

    private bool IsDefined (MemberInfo memberInfo, string attributeFullName)
    {
      var data = CustomAttributeData.GetCustomAttributes (memberInfo);
      return data.Any (attributeData => attributeData.Constructor.DeclaringType.FullName == attributeFullName);
    }

    private CustomAttributeData GetAttribute (MemberInfo memberInfo, string attributeName)
    {
      return CustomAttributeData.GetCustomAttributes (memberInfo).SingleOrDefault (ad => ad.Constructor.DeclaringType.FullName == attributeName);
    }
  }
}