using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Text;

namespace Rubicon.CooNet.Gen
{
/// <summary>
/// Base class for extended code providers.
/// </summary>
/// <remarks>
/// Extended code providers extend the functionality of .NET CodeDOM code providers. In order to generate
/// code for a specific language, a subclass of ExtendedCodeProvider must be implemented that extends the
/// functionality of CodeDOM code providers.
/// </remarks>
public abstract class ExtendedCodeProvider
{
  // types

  public enum CastOperatorKind { Implicit, Explicit }

  protected class MemberAttributeKeywordMapping
  {
    public MemberAttributeKeywordMapping (MemberAttributes attribute, MemberAttributes mask, string keyword)
    {
      Attribute = attribute;
      Mask = mask;
      Keyword = keyword;
    }
    public readonly MemberAttributes Attribute;
    public readonly MemberAttributes Mask;
    public readonly string Keyword;

    public bool IsSet (MemberAttributes concreteAttributes)
    {
      if (Mask != (MemberAttributes) 0)
        return (concreteAttributes & Mask) == Attribute;
      else
        return (concreteAttributes & Attribute) == Attribute;
    }
  }

  // fields

  public readonly CodeDomProvider Provider;
  public readonly ICodeGenerator Generator;

  // construction and disposal

	public ExtendedCodeProvider (CodeDomProvider provider)
	{
    Provider = provider;
    Generator = provider.CreateGenerator ();
	}

  // properties and methods

  public virtual bool SupportsCastingOperators
  {
    get { return false; }
  }

  public virtual CodeTypeMember CreateCastingOperator (
      string fromType, string toType, CodeStatementCollection statements, 
      MemberAttributes attributes, CastOperatorKind castOperatorKind)
  {
    throw new NotSupportedException (this.GetType().FullName + " does not support casting operators.");
  }

  public virtual string GetValidName (string name)
  {
    return name;
  }

  protected static void AppendMemberAttributeString (
      StringBuilder sb, MemberAttributeKeywordMapping[] mappings, MemberAttributes attributes)
  {
    foreach (MemberAttributeKeywordMapping mapping in mappings)
    {
      if (mapping.IsSet (attributes))
      {
        if (sb.Length > 0)
          sb.Append (" ");
        sb.Append (mapping.Keyword);
      }
    }
  }
}

}
