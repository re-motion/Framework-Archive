using System;
using System.Diagnostics;
using System.Reflection;
using Rubicon.Utilities;

namespace Rubicon.Mixins.Definitions
{
  [DebuggerDisplay("{AttributeType}")]
  public class AttributeDefinition: IVisitableDefinition
  {
    private IAttributableDefinition _declaringDefinition;
    private CustomAttributeData _data;

    public AttributeDefinition (IAttributableDefinition declaringDefinition, CustomAttributeData data)
    {
      _declaringDefinition = declaringDefinition;
      _data = data;
    }

    public CustomAttributeData Data
    {
      get { return _data;}
    }

    public Type AttributeType
    {
      get { return _data.Constructor.DeclaringType; }
    }

    public void Accept (IDefinitionVisitor visitor)
    {
      ArgumentUtility.CheckNotNull ("visitor", visitor);
      visitor.Visit (this);
    }

    public string FullName
    {
      get { return _data.Constructor.DeclaringType.FullName; }
    }

    public IVisitableDefinition Parent
    {
      get { return DeclaringDefinition as IVisitableDefinition; }
    }

    public IAttributableDefinition DeclaringDefinition
    {
      get { return _declaringDefinition; }
    }
  }
}