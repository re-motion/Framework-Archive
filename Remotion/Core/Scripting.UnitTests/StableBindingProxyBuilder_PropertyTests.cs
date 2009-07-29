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
using Remotion.Diagnostics.ToText;
using Remotion.Scripting.UnitTests.TestDomain;

namespace Remotion.Scripting.UnitTests
{
  [TestFixture]
  public class StableBindingProxyBuilder_PropertyTests : StableBindingProxyBuilderTest
  {
    private const BindingFlags _nonPublicInstanceFlags = BindingFlags.Instance | BindingFlags.NonPublic;
    private const BindingFlags _publicInstanceFlags = BindingFlags.Instance | BindingFlags.Public;
    private const BindingFlags _allFlags = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;


    [Test]
    public void BuildProxyType_PublicProperty ()
    {
      //var knownBaseTypes = new[] { typeof (ProxiedChild) };
      var knownBaseTypes = new[] { typeof (ProxiedChildChild) };
      //var knownInterfaceTypes = new[] { typeof (IProperty) };
      var knownTypes = knownBaseTypes; //knownBaseTypes.Union (knownInterfaceTypes).ToArray ();
      var typeFilter = new TypeLevelTypeFilter (knownTypes);
      var proxiedType = typeof (ProxiedChildChildChild);
      var stableBindingProxyBuilder = new StableBindingProxyBuilder (proxiedType, typeFilter, CreateModuleScope ("BuildProxyType_PublicProperty"));
      var proxyType = stableBindingProxyBuilder.BuildProxyType ();

      // Create proxy instance, initializing it with class to be proxied
      var proxied = new ProxiedChildChildChild ("PC");
      object proxy = Activator.CreateInstance (proxyType, proxied);

      Assert.That (proxied.NameProperty, Is.EqualTo ("ProxiedChildChildChild::NameProperty PC"));

      To.ConsoleLine.e ("proxyType.GetAllProperties()", proxyType.GetAllProperties ()).nl ().e (proxyType.GetAllProperties ().Select (pi => pi.Attributes)).nl (2).e ("proxyType.GetAllMethods()", proxyType.GetAllMethods ());

      var proxyPropertyInfo = proxyType.GetProperty ("NameProperty", _publicInstanceFlags);

      Assert.That (proxyPropertyInfo, Is.Not.Null);
      Assert.That (proxyPropertyInfo.CanRead, Is.True);
      Assert.That (proxyPropertyInfo.CanWrite, Is.False);
      Assert.That (proxyPropertyInfo.GetValue (proxy, null), Is.EqualTo ("ProxiedChildChild::NameProperty PC"));
    }

    [Test]
    public void BuildProxyType_PublicProperty_Setter ()
    {
      var knownBaseTypes = new[] { typeof (ProxiedChild) };
      //var knownInterfaceTypes = new[] { typeof (IProperty) };
      var knownTypes = knownBaseTypes; //knownBaseTypes.Union (knownInterfaceTypes).ToArray ();
      var typeFilter = new TypeLevelTypeFilter (knownTypes);
      var proxiedType = typeof (ProxiedChildChildChild);
      var stableBindingProxyBuilder = new StableBindingProxyBuilder (proxiedType, typeFilter, CreateModuleScope ("BuildProxyType_PublicProperty"));
      var proxyType = stableBindingProxyBuilder.BuildProxyType ();

      // Create proxy instance, initializing it with class to be proxied
      var proxied = new ProxiedChildChildChild ("PC");
      object proxy = Activator.CreateInstance (proxyType, proxied);

      Assert.That (proxied.NameProperty, Is.EqualTo ("ProxiedChildChildChild::NameProperty PC"));

      To.ConsoleLine.e ("proxyType.GetAllProperties()", proxyType.GetAllProperties ()).nl ().e (proxyType.GetAllProperties ().Select (pi => pi.Attributes)).nl (2).e ("proxyType.GetAllMethods()", proxyType.GetAllMethods ());

      var proxyPropertyInfo = proxyType.GetProperty ("NameProperty", _publicInstanceFlags);

      Assert.That (proxyPropertyInfo, Is.Not.Null);
      Assert.That (proxyPropertyInfo.CanRead, Is.True);
      Assert.That (proxyPropertyInfo.CanWrite, Is.True);
      Assert.That (proxyPropertyInfo.GetValue (proxy, null), Is.EqualTo ("ProxiedChild::NameProperty PC"));

      proxyPropertyInfo.SetValue (proxy, "XXyyZZ", null);
      Assert.That (proxyPropertyInfo.GetValue (proxy, null), Is.EqualTo ("ProxiedChild::NameProperty XXyyZZ-ProxiedChild::NameProperty"));
    }


    [Test]
    public void BuildProxyType_ExplicitInterfaceProperty ()
    {
      var knownBaseTypes = new[] { typeof (ProxiedChild) };
      var knownInterfaceTypes = new[] { typeof (IProperty) };
      var knownTypes = knownBaseTypes.Union (knownInterfaceTypes).ToArray ();
      var typeFilter = new TypeLevelTypeFilter (knownTypes);
      var proxiedType = typeof (ProxiedChildChildChild);
      var stableBindingProxyBuilder = new StableBindingProxyBuilder (proxiedType, typeFilter, CreateModuleScope ("BuildProxyType_ExplicitInterfaceProperty"));
      var proxyType = stableBindingProxyBuilder.BuildProxyType ();

      // Create proxy instance, initializing it with class to be proxied
      var proxied = new ProxiedChildChildChild ("PC");

      object proxy = Activator.CreateInstance (proxyType, proxied);

      const string expectedPropertyValue = "ProxiedChild::IAmbigous1::MutableNameProperty PC";
      Assert.That (((IProperty) proxied).MutableNameProperty, Is.EqualTo (expectedPropertyValue));

      //To.ConsoleLine.e ("proxyType.GetAllProperties()", proxyType.GetAllProperties ()).nl ().e (proxyType.GetAllProperties ().Select(pi => pi.Attributes)).nl (2).e ("proxyType.GetAllMethods()", proxyType.GetAllMethods ());

      var proxyPropertyInfo = proxyType.GetProperty (
        "Remotion.Scripting.UnitTests.TestDomain.ProxiedChild.Remotion.Scripting.UnitTests.TestDomain.IProperty.MutableNameProperty", _nonPublicInstanceFlags);

      Assert.That (proxyPropertyInfo, Is.Not.Null);
      Assert.That (proxyPropertyInfo.GetValue (proxy, null), Is.EqualTo (expectedPropertyValue));
      //AssertPropertyInfoEqual (proxyPropertyInfo, propertyInfo);

      ((IProperty) proxied).MutableNameProperty = "aBc";
      const string expectedPropertyValue2 = "ProxiedChild::IAmbigous1::MutableNameProperty aBc-ProxiedChild::IAmbigous1::MutableNameProperty";
      Assert.That (((IProperty) proxied).MutableNameProperty, Is.EqualTo (expectedPropertyValue2));
      Assert.That (proxyPropertyInfo.GetValue (proxy, null), Is.EqualTo (expectedPropertyValue2));

      proxyPropertyInfo.SetValue (proxy,"XXyyZZ" ,null);
      const string expectedPropertyValue3 = "ProxiedChild::IAmbigous1::MutableNameProperty XXyyZZ-ProxiedChild::IAmbigous1::MutableNameProperty";
      Assert.That (((IProperty) proxied).MutableNameProperty, Is.EqualTo (expectedPropertyValue3));
      Assert.That (proxyPropertyInfo.GetValue (proxy, null), Is.EqualTo (expectedPropertyValue3));
    }



    //[Test]
    ////[Explicit]
    //public void BuildProxyType_AmbigousExplicitInterfaceProperties ()
    //{
    //  var knownBaseTypes = new[] { typeof (ProxiedChild) };
    //  var knownInterfaceTypes = new[] { typeof (IProperty) };
    //  var knownTypes = knownBaseTypes.Union (knownInterfaceTypes).ToArray ();
    //  var typeFilter = new TypeLevelTypeFilter (knownTypes);
    //  var proxiedType = typeof (ProxiedChildChildChild);
    //  var stableBindingProxyBuilder = new StableBindingProxyBuilder (proxiedType, typeFilter, CreateModuleScope ("BuildProxyType_ExplicitInterfaceProperty"));
    //  var proxyType = stableBindingProxyBuilder.BuildProxyType ();

    //  //var knownBaseTypeMethods = knownBaseTypes.SelectMany (t => t.GetMethods ()).Where (m => m.IsSpecialName == false);
    //  //var proxyMethods = proxyType.GetMethods (BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic).Where (m => m.IsSpecialName == false && m.DeclaringType != typeof (Object));
    //  //var interfaceMethods = knownInterfaceTypes.SelectMany (t => t.GetMethods (BindingFlags.Instance | BindingFlags.Public));

    //  ////To.ConsoleLine.e (knownBaseTypeMethods).nl (3).e (interfaceMethods).nl (3).e (proxyMethods).nl (3);

    //  //Assert.That (knownBaseTypeMethods.Count () + interfaceMethods.Count (), Is.EqualTo (proxyMethods.Count ()));


    //  // Create proxy instance, initializing it with class to be proxied
    //  var proxied = new ProxiedChildChildChild ("PC");

    //  object proxy = Activator.CreateInstance (proxyType, proxied);

    //  const string expectedPropertyValue = "ProxiedChild::IAmbigous1::MutableNameProperty PC";
    //  Assert.That (((IProperty) proxied).MutableNameProperty, Is.EqualTo (expectedPropertyValue));

    //  //To.ConsoleLine.e ("proxyType.GetAllProperties()", proxyType.GetAllProperties ()).nl ().e (proxyType.GetAllProperties ().Select (pi => pi.Attributes)).nl (2).e ("proxyType.GetAllMethods()", proxyType.GetAllMethods ());

    //  var proxyPropertyInfo = proxyType.GetProperty (
    //    "Remotion.Scripting.UnitTests.TestDomain.ProxiedChild.Remotion.Scripting.UnitTests.TestDomain.IProperty.MutableNameProperty", _nonPublicInstanceFlags);

    //  Assert.That (proxyPropertyInfo, Is.Not.Null);
    //  Assert.That (proxyPropertyInfo.GetValue (proxy, null), Is.EqualTo (expectedPropertyValue));
    //  //AssertPropertyInfoEqual (proxyPropertyInfo, propertyInfo);

    //}

  }
}