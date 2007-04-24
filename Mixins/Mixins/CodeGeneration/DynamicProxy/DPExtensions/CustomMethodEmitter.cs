using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using Castle.DynamicProxy.Generators.Emitters;

namespace Mixins.CodeGeneration.DynamicProxy.DPExtensions
{
  public class CustomMethodEmitter : IAttributableEmitter
  {
    private MethodEmitter _innerEmitter;

    public CustomMethodEmitter (AbstractTypeEmitter parentEmitter, string name, MethodAttributes attributes)
    {
      _innerEmitter = parentEmitter.CreateMethod (name, attributes);
    }

    public void AddCustomAttribute (CustomAttributeBuilder customAttribute)
    {
      _innerEmitter.MethodBuilder.SetCustomAttribute (customAttribute);
    }

    public void CopyParametersAndReturnTypeFrom (MethodInfo method, AbstractTypeEmitter parentEmitter)
    {
      _innerEmitter.CopyParametersAndReturnTypeFrom (method, parentEmitter);
    }

    public MethodBuilder MethodBuilder
    {
      get { return _innerEmitter.MethodBuilder; }
    }

    public MethodEmitter InnerEmitter
    {
      get { return _innerEmitter; }
    }
  }
}
