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
/// <para>
///   Extended code providers extend the functionality of .NET CodeDOM code providers. In order to generate
///   code for a specific language, a subclass of ExtendedCodeProvider must be implemented that extends the
///   functionality of CodeDOM code providers.
/// <para></para>
///   <b>Note to inheritors:</b>
/// <para></para>
///   Inheritors must at least call the base class constructor with a valid CodeDomProvider.
/// <para></para>
///   If the specific language supports custom casting operators, override <see cref="SupportsCastingOperators"/> 
///   and <see cref="CreateCastingOperator"/>. 
/// </para>
/// </remarks>
public abstract class ExtendedCodeProvider
{
  // types

  /// <summary>
  /// Specifies one of the two kinds of casting operators.
  /// </summary>
  public enum CastOperatorKind { Implicit, Explicit }

  /// <summary>
  /// This class is used to create a mapping table for <c>AppendMemberAttributeString</c>.
  /// <seealso cref="AppendMemberAttributeString"/>
  /// </summary>
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

  /// <summary>
  /// Creates an <c>ExtendedCodeProvider</c> using a <c>CodeDomProvider</c> and a matching generator.
  /// </summary>
	public ExtendedCodeProvider (CodeDomProvider provider, ICodeGenerator generator)
	{
    if (provider == null) throw new ArgumentNullException ("provider");
    if (generator == null) throw new ArgumentNullException ("generator");
    Provider = provider;
    Generator = generator;
	}

  /// <summary>
  /// Creates an <c>ExtendedCodeProvider</c> using a <c>CodeDomProvider</c>.
  /// </summary>
	public ExtendedCodeProvider (CodeDomProvider provider)
	{
    if (provider == null) throw new ArgumentNullException ("provider");
    Provider = provider;
    Generator = provider.CreateGenerator ();
	}

  // properties and methods

  /// <summary>
  /// Specifies whether the current provider supports casting operators.
  /// </summary>
  /// <value>The default implementation always returns <c>false</c>.</value>
  public virtual bool SupportsCastingOperators
  {
    get { return false; }
  }

  /// <summary>
  /// For derived classes that support casting operators, this method creates implicit or explicit casting operators.
  /// </summary>
  /// <param name="fromType">Type that the method casts from.</param>
  /// <param name="toType">Type that the method casts to.</param>
  /// <param name="statements">Statements that perform the conversion, ending with a <c>CodeMethodReturnStatement</c>.</param>
  /// <param name="attributes">Method attributes that define access and scope. Must be static.</param>
  /// <param name="castOperatorKind"><c>Implicit</c> to create an implicit casting operator, <c>Explicit</c> otherwise.</param>
  /// <returns>A <c>CodeTypeMember</c> object that can be appended to a CodeDOM type object.</returns>
  /// <exception cref="NotSupportedException">The default implementation always throws this exception.</exception>
  public virtual CodeTypeMember CreateCastingOperator (
      string fromType, string toType, CodeStatementCollection statements, 
      MemberAttributes attributes, CastOperatorKind castOperatorKind)
  {
    throw new NotSupportedException (this.GetType().FullName + " does not support casting operators.");
  }

  /// <summary>
  /// Specifies whether the provider supports documentation comments.
  /// </summary>
  /// <value>The default implementation always returns false.</value>
  public virtual bool SupportsDocumentationComments
  {
    get { return false; }
  }

  /// <summary>
  /// Adds a documentation comment if the provider supports it, a normal comment otherwise.
  /// </summary>
  /// <param name="item">The source code item the comment is to be attached to.</param>
  /// <param name="elementName">The documentation comments element name.</param>
  /// <param name="elementArguments">Optional. A string containing the XML attributes for this element.</param>
  /// <param name="alternativeHeadline">Optional. A string that is written as a headline if the provider does not support documentation comments.</param>
  /// <param name="description">The text of the comment.</param>
  public virtual void AddDocumentationComment (
      CodeTypeMember item, string elementName, string elementArguments, string alternativeHeadline, string description)
  {
    StringBuilder sb = new StringBuilder();
    if (SupportsDocumentationComments)
    {
      sb.Append ("<");
      sb.Append (elementName);
      if (elementArguments != null && elementArguments.Length > 0)
      {
        sb.Append (" ");
        sb.Append (elementArguments);
      }
      sb.Append (">");
      sb.Append (description.Replace ("<", "&lt;").Replace(">", "&gt;"));
      sb.Append ("</");
      sb.Append (elementName);
      sb.Append (">");
      item.Comments.Add (new CodeCommentStatement (sb.ToString(), true));
    }
    else
    {
      if (alternativeHeadline != null && alternativeHeadline.Length > 0)
      {
        sb.Append (alternativeHeadline);
        sb.Append (":\n");                                        
      }
      sb.Append (description);
      item.Comments.Add (new CodeCommentStatement (sb.ToString(), false));
    }
  }

  /// <summary>
  /// Adds a <c>summary</c> documentation comment.
  /// </summary>
  public virtual void AddSummaryComment (CodeTypeMember item, string summary)
  {
    AddDocumentationComment (item, "summary", null, null, summary);
  }

  /// <summary>
  /// Adds a <c>remarks</c> documentation comment.
  /// </summary>
  public virtual void AddRemarksComment (CodeTypeMember item, string remarks)
  {
    AddDocumentationComment (item, "remarks", null, null, remarks);
  }

  /// <summary>
  /// Adds a <c>param</c> documentation comment.
  /// </summary>
  public virtual void AddParameterComment (CodeTypeMember item, string parameterName, string description)
  {
    AddDocumentationComment (item, "param", "name=\"" + parameterName + "\"", "Parameter " + parameterName, description);
  }

  /// <summary>
  /// This method returns a valid identifier name for the CodeDOM provider.
  /// </summary>
  /// <param name="name">The identifier.</param>
  /// <returns>The parameter <c>name</c> itself.</returns>
  /// <remarks>Override this method only if the CodeDOM provider you are using does not correctly escape all identifiers, as is
  /// the case with the C# provider that does not escape the keyword <c>params</c>.
  /// </remarks>
  public virtual string GetValidName (string name)
  {
    return name;
  }

  /// <summary>
  /// If implemented by a derived class, modifies the specified <c>CompilerParameters</c> to create an XML documentation file.
  /// </summary>
  public virtual void AddOptionCreateXmlDocumentation (CompilerParameters parameters, string xmlFilename)
  {
  }

  /// <summary>
  /// Writes the language-specific keywords that correspond to the values of the <c>attribute</c> parameter.
  /// </summary>
  /// <remarks>
  /// This method aids in the implementation of <see cref="CreateCastingOperator"/>.
  /// </remarks>
  /// <param name="sb">The StringBuilder object to write to.</param>
  /// <param name="mappings">An array of <see cref="MemberAttributeKeywordMapping"/> mappings that define the language-specific keywords.</param>
  /// <param name="attributes">The concrete attributes that are to be written.</param>
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
