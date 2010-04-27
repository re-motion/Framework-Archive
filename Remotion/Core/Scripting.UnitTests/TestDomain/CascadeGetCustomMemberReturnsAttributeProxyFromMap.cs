﻿using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Remotion.Collections;

namespace Remotion.Scripting.UnitTests.TestDomain
{
  public class CascadeGetCustomMemberReturnsAttributeProxyFromMap : Cascade
  {
    protected readonly Dictionary<Tuple<Type, string>, object> _attributeProxyMap = new Dictionary<Tuple<Type, string>, object> ();

    public CascadeGetCustomMemberReturnsAttributeProxyFromMap (int nrChildren)
    {
      --nrChildren;
      Name = "C" + nrChildren;
      if (nrChildren > 0)
      {
        Child = new CascadeGetCustomMemberReturnsAttributeProxyFromMap (nrChildren);
      }
    }

    public void AddAttributeProxy (string name, object proxied, ScriptContext scriptContext)
    {
      var type = this.GetType ();
      ScriptContext.SwitchAndHoldScriptContext (scriptContext);
      var attributeNameProxy = ScriptContext.GetAttributeProxy (proxied, name);
      _attributeProxyMap[new Tuple<Type, string> (type,name)] = attributeNameProxy;
      ScriptContext.ReleaseScriptContext (scriptContext);
    }

    [SpecialName]
    public object GetCustomMember (string name)
    {
      var type = this.GetType();
      return _attributeProxyMap[new Tuple<Type, string> (type, name)];
    }
  }
}