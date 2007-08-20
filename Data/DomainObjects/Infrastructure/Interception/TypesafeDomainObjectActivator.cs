using System;
using System.Reflection;
using Rubicon.Reflection;
using Rubicon.Text;
using Rubicon.Utilities;

namespace Rubicon.Data.DomainObjects.Infrastructure.Interception
{
  internal static class TypesafeDomainObjectActivator
  {
    public class ConstructorLookupInfo : TypesafeActivator.ConstructorLookupInfo
    {
      private readonly Type _baseType;

      public ConstructorLookupInfo (Type baseType, Type definingType, BindingFlags bindingFlags)
        : base (ArgumentUtility.CheckNotNull ("definingType", definingType), bindingFlags)
      {
        ArgumentUtility.CheckNotNull ("baseType", baseType);

        _baseType = baseType;
      }

      public override Delegate GetDelegate (Type delegateType)
      {
        try
        {
          return base.GetDelegate (delegateType);
        }
        catch (MissingMethodException ex)
        {
          Type[] parameterTypes = ConstructorWrapper.GetParameterTypes (delegateType);
          string message = string.Format ("Type {0} does not support the requested constructor with signature ({1}).",
              _baseType.FullName, SeparatedStringBuilder.Build (", ", parameterTypes, delegate (Type t) { return t.FullName; })); 
          throw new MissingMethodException (message, ex);
        }
      }
    }

    public static FuncInvoker<TMinimal> CreateInstance<TMinimal> (Type baseType, Type type, BindingFlags bindingFlags)
    {
      ArgumentUtility.CheckNotNullAndTypeIsAssignableFrom ("type", type, typeof (TMinimal));
      return new FuncInvoker<TMinimal> (new ConstructorLookupInfo (baseType, type, bindingFlags).GetDelegate);
    }
  }
}