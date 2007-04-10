using System;
using Mixins.CodeGeneration;

namespace Mixins.UnitTests.Mixins
{
  public class ObjectFactory
  {
    private TypeFactory _typeFactory;

    public ObjectFactory(TypeFactory typeFactory)
    {
      _typeFactory = typeFactory;
    }

    public T Create<T> (params object[] args)
    {
      return (T) Create (typeof (T), args);
    }

    public object Create (Type t, params object[] args)
    {
      Type concreteType = _typeFactory.GetConcreteType (t);
      return Activator.CreateInstance (concreteType, args);
    }
  }
}
