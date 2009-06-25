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
using System.Linq;
using System.Reflection;
using Castle.DynamicProxy;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Development.UnitTesting;
using Remotion.Scripting.UnitTests.TestDomain;

namespace Remotion.Scripting.UnitTests
{
  [TestFixture]
  public class ForwardingProxyBuilderTest
  {
    private ModuleScope _moduleScope;


    [TestFixtureTearDown]
    public void TestFixtureTearDown ()
    {
      if (_moduleScope != null)
      {
        if (_moduleScope.StrongNamedModule != null)
          SaveAndVerifyModuleScopeAssembly (true);
        if (_moduleScope.WeakNamedModule != null)
          SaveAndVerifyModuleScopeAssembly (false);
      }
    }

    public ModuleScope ModuleScope
    {
      get
      {
        if (_moduleScope == null)
        {
          const string name = "Remotion.Scripting.CodeGeneration.Generated.ForwardingProxyBuilderTest";
          const string nameSigned = name + ".Signed";
          const string nameUnsigned = name + ".Unsigned";
          const string ext = ".dll";
          _moduleScope = new ModuleScope (true, nameSigned, nameSigned + ext, nameUnsigned, nameUnsigned + ext);
        }
        return _moduleScope;
      }
    }


    [Test]
    public void BuildProxyType ()
    {
      var proxyBuilder = new ForwardingProxyBuilder ("ForwardingProxyBuilder_BuildProxyTypeTest", ModuleScope, typeof (Proxied), new Type[0]);
      Type proxyType = proxyBuilder.BuildProxyType ();

      // Create proxy instance, initializing it with class to be proxied
      var proxied = new Proxied ();
      object proxy = Activator.CreateInstance (proxyType, proxied);

      FieldInfo proxiedFieldInfo = proxy.GetType ().GetField ("_proxied", BindingFlags.NonPublic | BindingFlags.Instance);
      Assert.That (proxiedFieldInfo.GetValue (proxy), Is.EqualTo (proxied));
      Assert.That (proxiedFieldInfo.IsInitOnly, Is.True);
      Assert.That (proxiedFieldInfo.IsPrivate, Is.True);
    }


    [Test]
    public void AddForwardingExplicitInterfaceMethod ()
    {
      var proxyBuilder = new ForwardingProxyBuilder ("AddForwardingExplicitInterfaceMethod",
        ModuleScope, typeof (ProxiedChild), new[] { typeof (IAmbigous1) });
      
      var methodInfo = typeof (IAmbigous1).GetMethod ("StringTimes");
      proxyBuilder.AddForwardingExplicitInterfaceMethod (methodInfo);

      // Create proxy instance, initializing it with class to be proxied
      var proxied = new ProxiedChild ();
      object proxy = proxyBuilder.CreateInstance (proxied);

      Assert.That (((IAmbigous1) proxied).StringTimes ("aBc", 4), Is.EqualTo ("aBcaBcaBcaBc"));
      Assert.That (((IAmbigous1) proxy).StringTimes ("aBc", 4), Is.EqualTo ("aBcaBcaBcaBc"));
    }


    [Test]
    public void AddForwardingImplicitInterfaceMethod ()
    {
      var proxyBuilder = new ForwardingProxyBuilder ("AddForwardingImplicitInterfaceMethod",
        ModuleScope, typeof (Proxied), new[] { typeof (IProxiedGetName) });
      proxyBuilder.AddForwardingImplicitInterfaceMethod (typeof (IProxiedGetName).GetMethod ("GetName"));
      Type proxyType = proxyBuilder.BuildProxyType ();

      // Create proxy instance, initializing it with class to be proxied
      var proxied = new Proxied ();
      object proxy = Activator.CreateInstance (proxyType, proxied);

      Assert.That (proxy.GetType ().GetInterfaces (), Is.EquivalentTo ((new[] { typeof (IProxiedGetName) })));
      Assert.That (((IProxiedGetName) proxy).GetName (), Is.EqualTo ("Implementer.IProxiedGetName"));
      Assert.That (proxy.GetType ().GetMethod ("GetName").Invoke (proxy, new object[0]), Is.EqualTo ("Implementer.IProxiedGetName"));
    }


    [Test]
    public void AddForwardingMethod ()
    {
      var proxyBuilder = new ForwardingProxyBuilder ("AddForwardingMethod", ModuleScope, typeof (Proxied), new Type[0]);
      var methodInfo = typeof (Proxied).GetMethod ("PrependName");
      proxyBuilder.AddForwardingMethod (methodInfo);
      Type proxyType = proxyBuilder.BuildProxyType ();

      var proxied = new Proxied ("The name");
      object proxy = Activator.CreateInstance (proxyType, proxied);

      // Check calling proxied method through reflection
      Assert.That (methodInfo.Invoke (proxied, new object[] { "is Smith" }), Is.EqualTo ("The name is Smith"));

      var proxyMethodInfo = proxy.GetType ().GetMethod ("PrependName");
      AssertMethodInfoEqual (proxyMethodInfo, methodInfo);
      Assert.That (proxyMethodInfo.Invoke (proxy, new object[] { "is Smith" }), Is.EqualTo ("The name is Smith"));
    }


    [Test]
    public void AddForwardingProperty ()
    {
      var proxyBuilder = new ForwardingProxyBuilder ("AddForwardingProperty",
        ModuleScope, typeof (ProxiedChildGeneric<ProxiedChild, double>), new Type[0]);
      var propertyInfo = typeof (ProxiedChildGeneric<ProxiedChild, double>).GetProperty ("MutableName");
      proxyBuilder.AddForwardingProperty (propertyInfo);

      // Create proxy instance, initializing it with class to be proxied
      var proxied = new ProxiedChildGeneric<ProxiedChild, double> ("PCG", new ProxiedChild ("PC"), 123.456);
      object proxy = proxyBuilder.CreateInstance (proxied);

      Assert.That (proxied.MutableName, Is.EqualTo ("PCG"));
      var proxyPropertyInfo = proxy.GetType ().GetProperty ("MutableName");
      AssertPropertyInfoEqual (proxyPropertyInfo, propertyInfo);
      Assert.That (proxyPropertyInfo.GetValue (proxy, null), Is.EqualTo ("PCG"));

      proxied.MutableName = "PCG_Changed";
      Assert.That (proxyPropertyInfo.GetValue (proxy, null), Is.EqualTo ("PCG_Changed"));

      proxyPropertyInfo.SetValue (proxy, "PCG_Changed_Proxy", null);
      Assert.That (proxied.MutableName, Is.EqualTo ("PCG_Changed_Proxy"));
      Assert.That (proxyPropertyInfo.GetValue (proxy, null), Is.EqualTo ("PCG_Changed_Proxy"));
    }


    [Test]
    public void AddForwardingProperty_MethodWithVariableNumberOfParameters ()
    {
      var proxyBuilder = new ForwardingProxyBuilder ("AddForwardingProperty_MethodWithVariableNumberOfParameters", ModuleScope, typeof (Proxied), new Type[0]);
      var methodInfo = typeof (Proxied).GetMethod ("Sum");
      proxyBuilder.AddForwardingMethod (methodInfo);
      Type proxyType = proxyBuilder.BuildProxyType ();

      // Create proxy instance, initializing it with class to be proxied
      var proxied = new Proxied ("ProxiedProxySumTest");
      object proxy = Activator.CreateInstance (proxyType, proxied);

      // Check calling proxied method through reflection
      const string resultExpected = "ProxiedProxySumTest: 12";
      Assert.That (proxied.Sum (3, 4, 5), Is.EqualTo (resultExpected));
      Assert.That (methodInfo.Invoke (proxied, new object[] { new int[] { 3, 4, 5 } }), Is.EqualTo (resultExpected));

      var proxyMethodInfo = proxy.GetType ().GetMethod ("Sum");
      AssertMethodInfoEqual (proxyMethodInfo, methodInfo);
      Assert.That (proxyMethodInfo.Invoke (proxy, new object[] { new int[] { 3, 4, 5 } }), Is.EqualTo (resultExpected));

      Assert.That (proxyMethodInfo.Invoke (proxy, new object[] { new int[] { } }), Is.EqualTo ("ProxiedProxySumTest: 0"));
      Assert.That (proxyMethodInfo.Invoke (proxy, new object[] { new int[] { 1 } }), Is.EqualTo ("ProxiedProxySumTest: 1"));
      Assert.That (proxyMethodInfo.Invoke (proxy, new object[] { new int[] { 1000, 100, 10, 1 } }), Is.EqualTo ("ProxiedProxySumTest: 1111"));
    }


    [Test]
    public void AddForwardingMethod_GenericClass ()
    {
      var proxyBuilder = new ForwardingProxyBuilder ("AddForwardingMethod_GenericClass",
        ModuleScope, typeof (ProxiedChildGeneric<ProxiedChild, double>), new Type[0]);
      var methodInfo = typeof (ProxiedChildGeneric<ProxiedChild, double>).GetMethod ("ToStringKebap");
      proxyBuilder.AddForwardingMethod (methodInfo);

      // Create proxy instance, initializing it with class to be proxied
      var proxied = new ProxiedChildGeneric<ProxiedChild, double> ("PCG", new ProxiedChild ("PC"), 123.456);
      object proxy = proxyBuilder.CreateInstance (proxied);

      var proxyMethodInfo = proxy.GetType ().GetMethod ("ToStringKebap");
      AssertMethodInfoEqual (proxyMethodInfo, methodInfo);
      Assert.That (proxyMethodInfo.Invoke (proxy, new object[] { 9800 }), Is.EqualTo ("PCG_[Proxied: PC]_123.456_9800"));
    }


    [Test]
    public void AddForwardingProperty_ReadOnlyProperty ()
    {
      Type proxiedType = typeof (Proxied);
      var proxyBuilder = new ForwardingProxyBuilder ("AddForwardingProperty_ReadOnlyProperty", ModuleScope, proxiedType, new Type[0]);
      var propertyInfo = proxiedType.GetProperty ("ReadonlyName");
      proxyBuilder.AddForwardingProperty (propertyInfo);
      Type proxyType = proxyBuilder.BuildProxyType ();
      var proxyPropertyInfo = proxyType.GetProperty ("ReadonlyName");
      Assert.That (proxyPropertyInfo.CanRead, Is.True);
      Assert.That (proxyPropertyInfo.CanWrite, Is.False);
    }


    [Test]
    public void AddForwardingProperty_WriteOnlyProperty ()
    {
      Type proxiedType = typeof (Proxied);
      var proxyBuilder = new ForwardingProxyBuilder ("AddForwardingProperty_WriteOnlyProperty", ModuleScope, proxiedType, new Type[0]);
      var propertyInfo = proxiedType.GetProperty ("WriteonlyName");
      proxyBuilder.AddForwardingProperty (propertyInfo);
      Type proxyType = proxyBuilder.BuildProxyType ();
      var proxyPropertyInfo = proxyType.GetProperty ("WriteonlyName");
      Assert.That (proxyPropertyInfo.CanRead, Is.False);
      Assert.That (proxyPropertyInfo.CanWrite, Is.True);
    }





    public void AssertMethodInfoEqual (MethodInfo methodInfo0, MethodInfo methodInfo1)
    {
      Assert.That (methodInfo0, Is.Not.Null);
      Assert.That (methodInfo1, Is.Not.Null);
      Assert.That (methodInfo0.Name, Is.EqualTo (methodInfo1.Name));
      Assert.That (methodInfo0.ReturnType, Is.EqualTo (methodInfo1.ReturnType));
      var parameterTypes0 = methodInfo0.GetParameters ().Select (pi => pi.ParameterType);
      var parameterTypes1 = methodInfo1.GetParameters ().Select (pi => pi.ParameterType);
      Assert.That (parameterTypes0.ToList (), Is.EquivalentTo (parameterTypes1.ToList ()));
    }

    public void AssertPropertyInfoEqual (PropertyInfo propertyInfo0, PropertyInfo propertyInfo1)
    {
      Assert.That (propertyInfo0, Is.Not.Null);
      Assert.That (propertyInfo1, Is.Not.Null);
      Assert.That (propertyInfo0.Name, Is.EqualTo (propertyInfo1.Name));
      Assert.That (propertyInfo0.CanRead, Is.EqualTo (propertyInfo1.CanRead));
      Assert.That (propertyInfo0.CanWrite, Is.EqualTo (propertyInfo1.CanWrite));
      Assert.That (propertyInfo0.PropertyType, Is.EqualTo (propertyInfo1.PropertyType));
    }


    private void SaveAndVerifyModuleScopeAssembly (bool strongNamed)
    {
      var path = _moduleScope.SaveAssembly (strongNamed);
      PEVerifier.VerifyPEFile (path);
    }
  }
}