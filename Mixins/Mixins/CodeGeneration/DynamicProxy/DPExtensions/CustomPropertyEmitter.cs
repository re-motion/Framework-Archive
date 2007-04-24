using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using Castle.DynamicProxy.Generators.Emitters;

namespace Mixins.CodeGeneration.DynamicProxy.DPExtensions
{
  public class CustomPropertyEmitter : IAttributableEmitter
  {
    private PropertyBuilder builder;
    private MethodEmitter getMethod;
    private MethodEmitter setMethod;

    public CustomPropertyEmitter (AbstractTypeEmitter parentTypeEmitter, String name, PropertyAttributes attributes, Type propertyType, Type[] indexParameters)
		{
			builder = parentTypeEmitter.TypeBuilder.DefineProperty(name, attributes, propertyType, indexParameters);
		}

    public MethodEmitter GetMethod
    {
      get { return getMethod; }
      set { getMethod = value; }
    }

    public MethodEmitter SetMethod
    {
      get { return setMethod; }
      set { setMethod = value; }
    }

    public void Generate ()
    {
      if (setMethod != null)
      {
        setMethod.Generate ();
        builder.SetSetMethod (setMethod.MethodBuilder);
      }

      if (getMethod != null)
      {
        getMethod.Generate ();
        builder.SetGetMethod (getMethod.MethodBuilder);
      }
    }

    public void AddCustomAttribute (CustomAttributeBuilder customAttribute)
    {
      builder.SetCustomAttribute (customAttribute);
    }
  }
}
