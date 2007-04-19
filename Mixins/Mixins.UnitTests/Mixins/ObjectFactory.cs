using System;
using Mixins.CodeGeneration;
using Rubicon.Reflection;
using Mixins.Definitions;
using System.Reflection;

namespace Mixins.UnitTests.Mixins
{
  public class ObjectFactory
  {
    private TypeFactory _typeFactory;

    public ObjectFactory(TypeFactory typeFactory)
    {
      _typeFactory = typeFactory;
    }

    public IInvokeWith<T> Create<T> ()
    {
      const BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;

      Type concreteType = _typeFactory.GetConcreteType (typeof (T));
      GetDelegateWith<T> constructionDelegateCreator = new CachedGetDelegateWith<T, Type> (
          concreteType,
          delegate (Type[] argumentTypes, Type delegateType)
          {
            return ConstructorWrapper.CreateConstructorDelegate (concreteType, bindingFlags, null, CallingConventions.Any, argumentTypes, null, delegateType);
          });
      return new InvokeWith<T> (constructionDelegateCreator);
    }

    public object Create (Type t, params object[] args)
    {
      Type concreteType = _typeFactory.GetConcreteType (t);
      return Activator.CreateInstance (concreteType, args);
    }
  }
}
