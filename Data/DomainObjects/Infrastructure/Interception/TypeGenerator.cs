using System;
using System.Reflection;
using Castle.DynamicProxy;
using Rubicon.Utilities;
using Rubicon.CodeGeneration;

namespace Rubicon.Data.DomainObjects.Infrastructure.Interception
{
  internal class TypeGenerator
  {
    private readonly CustomClassEmitter _classEmitter;

    public TypeGenerator (Type baseType, ModuleScope scope)
    {
      ArgumentUtility.CheckNotNull ("baseType", baseType);
      ArgumentUtility.CheckNotNull ("scope", scope);

      string typeName = baseType.Name;
      _classEmitter = new CustomClassEmitter (scope, typeName, baseType, Type.EmptyTypes, TypeAttributes.Public | TypeAttributes.Abstract);
    }

    public Type BuildType (Type type)
    {
      return _classEmitter.BuildType ();
    }
  }
}