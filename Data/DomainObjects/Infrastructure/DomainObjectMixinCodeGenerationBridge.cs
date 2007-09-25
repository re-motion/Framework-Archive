using System;
using System.Runtime.Serialization;
using Rubicon.Mixins;
using Rubicon.Utilities;

namespace Rubicon.Data.DomainObjects.Infrastructure
{
  /// <summary>
  /// Forms a bridge between domain objects and mixins by supporting generation and deserialization of mixed domain objects.
  /// </summary>
  /// <remarks>All of the methods in this class are tolerant agains non-mixed types, i.e. they will perform default/no-op actions if a non-mixed
  /// type (or object) is passed to them rather than throwing exceptions.</remarks>
  public static class DomainObjectMixinCodeGenerationBridge
  {
    public static Type GetConcreteType (Type domainObjectType)
    {
      ArgumentUtility.CheckNotNull ("domainObjectType", domainObjectType);
      return TypeFactory.GetConcreteType (domainObjectType);
    }

    public static void PrepareUnconstructedInstance (DomainObject instance)
    {
      ArgumentUtility.CheckNotNull ("instance", instance);
      
      IMixinTarget instanceAsMixinTarget = instance as IMixinTarget;
      if (instanceAsMixinTarget != null)
        TypeFactory.InitializeUnconstructedInstance (instanceAsMixinTarget);
    }

    public static IObjectReference BeginDeserialization (Type baseType, SerializationInfo info, StreamingContext context)
    {
      ArgumentUtility.CheckNotNull ("baseType", baseType);
      ArgumentUtility.CheckNotNull ("info", info);

      IObjectReference objectReference = ObjectFactory.BeginDeserialization (baseType, info, context);

      Assertion.IsNotNull (objectReference);
      Assertion.IsTrue (baseType.IsAssignableFrom (objectReference.GetRealObject(context).GetType ()));
      return objectReference;
    }

    public static void FinishDeserialization (IObjectReference objectReference)
    {
      ArgumentUtility.CheckNotNull ("objectReference", objectReference);
      ObjectFactory.FinishDeserialization (objectReference);
    }

    public static void OnDomainObjectCreated (DomainObject instance)
    {
      ArgumentUtility.CheckNotNull ("instance", instance);
      NotifyDomainObjectMixins (instance, delegate (IDomainObjectMixin mixin) { mixin.OnDomainObjectCreated (); });
    }

    public static void OnDomainObjectLoaded (DomainObject instance, LoadMode loadMode)
    {
      ArgumentUtility.CheckNotNull ("instance", instance);
      NotifyDomainObjectMixins (instance, delegate (IDomainObjectMixin mixin) { mixin.OnDomainObjectLoaded (loadMode); });
    }

    private static void NotifyDomainObjectMixins (DomainObject instance, Proc<IDomainObjectMixin> notifier)
    {
      IMixinTarget instanceAsMixinTarget = instance as IMixinTarget;
      if (instanceAsMixinTarget != null)
      {
        foreach (object mixin in instanceAsMixinTarget.Mixins)
        {
          IDomainObjectMixin mixinAsDomainObjectMixin = mixin as IDomainObjectMixin;
          if (mixinAsDomainObjectMixin != null)
            notifier (mixinAsDomainObjectMixin);
        }
      }
    }
  }
}