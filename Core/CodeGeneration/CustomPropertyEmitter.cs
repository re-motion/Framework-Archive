using System;
using System.Reflection;
using System.Reflection.Emit;
using Castle.DynamicProxy.Generators.Emitters.SimpleAST;

namespace Rubicon.CodeGeneration
{
  public class CustomPropertyEmitter : IAttributableEmitter
  {
    public readonly CustomClassEmitter DeclaringType;
    public readonly PropertyBuilder PropertyBuilder;

    private readonly Type _propertyType;
    private readonly string _name;
    private readonly Type[] _indexParameters;

    private CustomMethodEmitter _getMethod;
    private CustomMethodEmitter _setMethod;

    public CustomPropertyEmitter (
        CustomClassEmitter declaringType, string name, PropertyAttributes attributes, bool hasThis, Type propertyType, Type[] indexParameters)
    {
      DeclaringType = declaringType;
      _name = name;

      _propertyType = propertyType;
      _indexParameters = indexParameters;
    
      // TODO: As soon as the overload below is publicly available, use it
      // CallingConventions callingConvention = hasThis ? CallingConventions.HasThis : CallingConventions.Standard;
      // PropertyBuilder = DeclaringType.TypeBuilder.DefineProperty (name, attributes, callingConvention, propertyType, null, null, indexParameters,
      //    null, null);
      PropertyBuilder = DeclaringType.TypeBuilder.DefineProperty (name, attributes, propertyType, indexParameters);
    }

    public Type PropertyType
    {
      get { return _propertyType; }
    }

    public Type[] IndexParameters
    {
      get { return _indexParameters; }
    }

    public CustomMethodEmitter GetMethod
    {
      get { return _getMethod; }
      set
      {
        if (value != null)
        {
          _getMethod = value;
          PropertyBuilder.SetGetMethod (_getMethod.MethodBuilder);
        }
        else
          throw new ArgumentException ("Due to limitations in Reflection.Emit, property accessors cannot be set to null.", "value");
      }
    }

    public CustomMethodEmitter SetMethod
    {
      get { return _setMethod; }
      set
      {
        if (value != null)
        {
          _setMethod = value;
          PropertyBuilder.SetSetMethod (_setMethod.MethodBuilder);
        }
        else
          throw new ArgumentException ("Due to limitations in Reflection.Emit, property accessors cannot be set to null.", "value");
      }
    }

    public string Name
    {
      get { return _name; }
    }

    public void AddCustomAttribute (CustomAttributeBuilder customAttribute)
    {
      PropertyBuilder.SetCustomAttribute (customAttribute);
    }

    public void ImplementPropertyWithField (FieldReference backingField)
    {
      if (GetMethod != null)
        GetMethod.AddStatement (new ReturnStatement (backingField));
      if (SetMethod != null)
      {
        SetMethod.AddStatement (
            new AssignStatement (backingField, SetMethod.ArgumentReferences[0].ToExpression()));
        SetMethod.ImplementByReturningVoid();
      }
    }

    public CustomMethodEmitter CreateGetMethod ()
    {
      if (GetMethod != null)
        throw new InvalidOperationException ("Thís property already has a getter method");
      else
      {
        CustomMethodEmitter method =
            DeclaringType.CreateMethod ("get_" + Name, MethodAttributes.Public | MethodAttributes.SpecialName);
        method.SetReturnType (PropertyType);
        method.SetParameterTypes (IndexParameters);
        GetMethod = method;
        return method;
      }
    }

    public CustomMethodEmitter CreateSetMethod ()
    {
      if (SetMethod != null)
        throw new InvalidOperationException ("Thís property already has a setter method");
      else
      {
        CustomMethodEmitter method =
            DeclaringType.CreateMethod ("set_" + Name, MethodAttributes.Public | MethodAttributes.SpecialName);
        Type[] setterParameterTypes = new Type[IndexParameters.Length + 1];
        IndexParameters.CopyTo (setterParameterTypes, 0);
        setterParameterTypes[IndexParameters.Length] = PropertyType;
        method.SetParameterTypes (setterParameterTypes);
        SetMethod = method;
        return method;
      }
    }
  }
}