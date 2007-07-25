using System;
using System.Reflection;
using Rubicon.Data.DomainObjects;
using Rubicon.SecurityManager.Domain.Metadata;
using Rubicon.Utilities;

namespace Rubicon.SecurityManager.UnitTests.Domain.Metadata
{
  public class SecurableClassDefinitionWrapper
  {
    // types

    // static members

    // member fields

    private SecurableClassDefinition _securableClassDefinition;
    private PropertyInfo _accessTypeReferencesPropertyInfo;

    // construction and disposing

    public SecurableClassDefinitionWrapper (SecurableClassDefinition securableClassDefinition)
    {
      ArgumentUtility.CheckNotNull ("securableClassDefinition", securableClassDefinition);

      _securableClassDefinition = securableClassDefinition;
      _accessTypeReferencesPropertyInfo = ((object)_securableClassDefinition).GetType ().GetProperty (
          "AccessTypeReferences",
          BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.DeclaredOnly);
    }

    // methods and properties

    public SecurableClassDefinition SecurableClassDefinition
    {
      get { return _securableClassDefinition; }
    }

    public DomainObjectCollection AccessTypeReferences
    {
      get { return (DomainObjectCollection) _accessTypeReferencesPropertyInfo.GetValue (_securableClassDefinition, new object[0]); }
    }
  }
}