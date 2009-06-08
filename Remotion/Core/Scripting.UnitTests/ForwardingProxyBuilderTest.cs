// Copyright (C) 2005 - 2009 rubicon informationstechnologie gmbh
// All rights reserved.
//
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Castle.DynamicProxy;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;

namespace Remotion.Scripting.UnitTests
{
  [TestFixture]
  public class ForwardingProxyBuilderTest
  {
    private ModuleScope _moduleScope;

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
    public void ForwardingProxyBuilder_BuildProxyTypeTest ()
    {
      var proxyBuilder = new ForwardingProxyBuilder ("CreateForwardingProxyHullCtorTestClass", ModuleScope, typeof (Proxied), new Type[0]);
      Type proxyType = proxyBuilder.BuildProxyType ();

      // Create proxy instance, initializing it with class to be proxied
      var proxied = new Proxied ();
      object proxy = Activator.CreateInstance (proxyType, proxied);

      FieldInfo proxiedFieldInfo = proxy.GetType ().GetField ("_proxied", BindingFlags.NonPublic | BindingFlags.Instance);
      Assert.That (proxiedFieldInfo.GetValue (proxy), Is.EqualTo (proxied));
      Assert.That (proxiedFieldInfo.IsInitOnly, Is.True);
      Assert.That (proxiedFieldInfo.IsPrivate, Is.True);
    }  
  }


  public interface IProxiedGetName
  {
    string GetName ();
  }


  public class Proxied : IProxiedGetName
  {
    private string _name;

    public Proxied ()
      : this ("default")
    {
    }

    public Proxied (string name)
    {
      _name = name;
    }

    public string Name
    {
      get { return _name; }
    }

    public string MutableName
    {
      get { return _name; }
      set { _name = value; }
    }

    public string ReadonlyName
    {
      get { return _name; }
    }

    public string WriteonlyName
    {
      set { _name = value; }
    }

    public string GetName ()
    {
      return "Implementer.IProxiedGetName";
    }

    public string Sum (params int[] numbers)
    {
      return Name + ": " + numbers.Sum ();
    }

    public string PrependName (string text)
    {
      return Name + " " + text;
    }

    public override string ToString ()
    {
      return "[Proxied: " + Name + "]";
    }
  }
}