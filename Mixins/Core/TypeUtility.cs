using System;
using System.Collections.Generic;
using Rubicon.Mixins.Context;
using Rubicon.Utilities;

namespace Rubicon.Mixins
{
  /// <summary>
  /// Provides a central point for reflectively working with mixin targets and generated concrete types.
  /// </summary>
  public static class TypeUtility
  {
    /// <summary>
    /// Determines whether the given <paramref name="type"/> is a type generated by the mixin infrastructure.
    /// </summary>
    /// <param name="type">The type to be checked.</param>
    /// <returns>
    /// True if <paramref name="type"/> was generated by the mixin infrastructure; otherwise, false.
    /// </returns>
    public static bool IsGeneratedType (Type type)
    {
      ArgumentUtility.CheckNotNull ("type", type);
      return typeof (IMixinTarget).IsAssignableFrom (type);
    }

    /// <summary>
    /// Gets the concrete type for a given <paramref name="baseType"/> which contains all mixins currently configured for the type.
    /// </summary>
    /// <param name="baseType">The base type for which to retrieve a concrete type.</param>
    /// <returns>The <paramref name="baseType"/> itself if there are no mixins configured for the type or if the type itself is a generated type;
    /// otherwise, a generated type containing all the mixins currently configured for <paramref name="baseType"/>.</returns>
    public static Type GetConcreteType (Type baseType)
    {
      ArgumentUtility.CheckNotNull ("baseType", baseType);
      if (IsGeneratedType (baseType))
        return baseType;
      else
        return TypeFactory.GetConcreteType (baseType, GenerationPolicy.GenerateOnlyIfConfigured);
    }

    /// <summary>
    /// Determines whether the given <paramref name="typeToAssign"/> would be assignable to <paramref name="baseOrInterface"/> after all mixins
    /// currently configured for the type have been taken into account.
    /// </summary>
    /// <param name="baseOrInterface">The base or interface to assign to.</param>
    /// <param name="typeToAssign">The type to check for assignment compatibility to <paramref name="baseOrInterface"/>.</param>
    /// <returns>
    /// True if the type returned by <see cref="GetConcreteType"/> for <paramref name="typeToAssign"/> is the same as, derived from, or an
    /// implementation of <paramref name="baseOrInterface"/>; otherwise, false.
    /// </returns>
    public static bool IsAssignableFrom (Type baseOrInterface, Type typeToAssign)
    {
      ArgumentUtility.CheckNotNull ("baseOrInterface", baseOrInterface);
      ArgumentUtility.CheckNotNull ("typeToAssign", typeToAssign);

      return baseOrInterface.IsAssignableFrom (GetConcreteType (typeToAssign));
    }

    /// <summary>
    /// Determines whether the specified <paramref name="type"/> is associated with any mixins.
    /// </summary>
    /// <param name="type">The type to check for mixins.</param>
    /// <returns>
    /// True if the specified type is a generated type containing any mixins or a base type for which there are mixins currently configured;
    /// otherwise, false.
    /// </returns>
    public static bool HasMixins (Type type)
    {
      ArgumentUtility.CheckNotNull ("type", type);

      ClassContext classContext = GetConcreteClassContext(type);
      return classContext != null && classContext.MixinCount > 0;
    }

    /// <summary>
    /// Determines whether the specified <paramref name="typeToCheck"/> is associated with a mixin of the given <paramref name="mixinType"/>.
    /// </summary>
    /// <param name="typeToCheck">The type to check.</param>
    /// <param name="mixinType">The mixin type to check for.</param>
    /// <returns>
    /// True if the specified type is a generated type containing a mixin of the given <paramref name="mixinType"/> or a base type currently
    /// configured with such a mixin; otherwise, false.
    /// </returns>
    /// <remarks>This method checks for the exact mixin type, it does not take assignability or generic type instantiations into account. If the
    /// check should be broadened to include these properties, <see cref="GetAscribableMixinType"/> should be used.</remarks>
    public static bool HasMixin (Type typeToCheck, Type mixinType)
    {
      ArgumentUtility.CheckNotNull ("typeToCheck", typeToCheck);
      ArgumentUtility.CheckNotNull ("mixinType", mixinType);

      ClassContext classContext = GetConcreteClassContext (typeToCheck);
      return classContext != null && classContext.ContainsMixin (mixinType);
    }

    /// <summary>
    /// Determines whether the specified <paramref name="typeToCheck"/> is associated with a mixin that can be ascribed to the given
    /// <paramref name="mixinType"/> and returns the respective mixin type.
    /// </summary>
    /// <param name="typeToCheck">The type to check.</param>
    /// <param name="mixinType">The mixin type to check for.</param>
    /// <returns>
    /// The exact mixin type if the specified type is a generated type containing a mixin that can be ascribed to <paramref name="mixinType"/> or a
    /// base type currently configured with such a mixin; otherwise <see langword="null"/>.
    /// </returns>
    public static Type GetAscribableMixinType (Type typeToCheck, Type mixinType)
    {
      ArgumentUtility.CheckNotNull ("typeToCheck", typeToCheck);
      ArgumentUtility.CheckNotNull ("mixinType", mixinType);

      foreach (Type configuredMixinType in GetMixinTypes (typeToCheck))
      {
        if (ReflectionUtility.CanAscribe (configuredMixinType, mixinType))
          return configuredMixinType;
      }
      return null;
    }

    private static ClassContext GetConcreteClassContext (Type type)
    {
      return Mixin.GetMixinConfigurationFromConcreteType (GetConcreteType (type));
    }

    /// <summary>
    /// Gets the mixin types associated with the given <paramref name="type"/>.
    /// </summary>
    /// <param name="type">The type to check for mixin types.</param>
    /// <returns>The mixins included in <paramref name="type"/> if it is a generated type; otherwise the mixins currently configured for
    /// <paramref name="type"/>.</returns>
    public static IEnumerable<Type> GetMixinTypes (Type type)
    {
      ArgumentUtility.CheckNotNull ("type", type);

      ClassContext classContext = GetConcreteClassContext (type);
      if (classContext != null)
      {
        foreach (MixinContext mixinContext in classContext.Mixins)
          yield return mixinContext.MixinType;
      }
    }

    /// <summary>
    /// Creates an instance of the type returned by <see cref="GetConcreteType"/> for the given <paramref name="type"/>.
    /// </summary>
    /// <param name="type">The type for whose concrete type to create an instance.</param>
    /// <param name="args">The arguments to be passed to the constructor.</param>
    /// <returns>An instance of the type returned by <see cref="GetConcreteType"/> for <paramref name="type"/> created via a constructor taking the
    /// specified <paramref name="args"/>.</returns>
    public static object CreateInstance (Type type, params object[] args)
    {
      ArgumentUtility.CheckNotNull ("type", type);
      ArgumentUtility.CheckNotNull ("args", args);

      return Activator.CreateInstance (GetConcreteType (type), args);
    }
  }
}
