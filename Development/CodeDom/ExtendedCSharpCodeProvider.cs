using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.IO;
using System.Text;

namespace Rubicon.CooNet.Gen
{

public class ExtendedCSharpCodeProvider: ExtendedCodeProvider
{
  // constants

  private static readonly MemberAttributeKeywordMapping[] c_memberAttributeKeywordMappings = new MemberAttributeKeywordMapping[] {
      new MemberAttributeKeywordMapping (MemberAttributes.Abstract, MemberAttributes.ScopeMask, "abstract"),
      new MemberAttributeKeywordMapping (MemberAttributes.Const, MemberAttributes.ScopeMask, "const"),
      new MemberAttributeKeywordMapping (MemberAttributes.Assembly, MemberAttributes.AccessMask, "internal"),
      new MemberAttributeKeywordMapping (MemberAttributes.Family, MemberAttributes.AccessMask, "protected"),
      new MemberAttributeKeywordMapping (MemberAttributes.FamilyOrAssembly, MemberAttributes.AccessMask, "internal protected"),
      new MemberAttributeKeywordMapping (MemberAttributes.Final, MemberAttributes.ScopeMask, "sealed"),
      new MemberAttributeKeywordMapping (MemberAttributes.New, MemberAttributes.VTableMask, "new"),
      new MemberAttributeKeywordMapping (MemberAttributes.Override, (MemberAttributes) 0, "override"),
      new MemberAttributeKeywordMapping (MemberAttributes.Private, MemberAttributes.AccessMask, "private"),
      new MemberAttributeKeywordMapping (MemberAttributes.Public, MemberAttributes.AccessMask, "public"),
      new MemberAttributeKeywordMapping (MemberAttributes.Static, MemberAttributes.ScopeMask, "static") };

  // construction and disposal

  public ExtendedCSharpCodeProvider ()
    : base (new Microsoft.CSharp.CSharpCodeProvider())
  {
  }

  // properties and methods

  public override bool SupportsCastingOperators
  {
    get { return true; }
  }

  public override CodeTypeMember CreateCastingOperator ( 
      string fromType, string toType, CodeStatementCollection statements, 
      MemberAttributes attributes, Rubicon.CooNet.Gen.ExtendedCodeProvider.CastOperatorKind castOperatorKind)
  {
    StringBuilder sb = new StringBuilder ();

    AppendMemberAttributeString (sb, c_memberAttributeKeywordMappings, attributes);
    if (castOperatorKind == CastOperatorKind.Implicit)
      sb.Append (" implicit operator ");
    else
      sb.Append (" explicit operator ");

    sb.Append (toType);
    sb.Append (" (");
    sb.Append (fromType);
    sb.Append (" obj) {");

    StringWriter writer = new StringWriter (sb);
    // CodeGeneratorOptions options  = new CodeGeneratorOptions ();

    foreach (CodeStatement statement in statements)
      Generator.GenerateCodeFromStatement (statement, writer, null);

    sb.Append ("\n}");

    return new CodeSnippetTypeMember (sb.ToString());
  }

  public override bool SupportsDocumentationComments
  {
    get { return true; }
  }

  public override void AddOptionCreateXmlDocumentation(CompilerParameters parameters, string xmlFilename)
  {
    string option = "/doc:" + xmlFilename + " /nowarn:1591";
    if (parameters.CompilerOptions == null || parameters.CompilerOptions.Length == 0)
      parameters.CompilerOptions = option;
    else
      parameters.CompilerOptions += " " + option;
  }

  public override string GetValidName (string name)
  {
    if (name == "params")
      return "@" + name;
    return name;
  }

  public override bool IsCaseSensitive
  {
    get { return true; }
  }

  public override CodeExpression CreateUnaryOperatorExpression (CodeUnaryOperatorType operatorType, CodeExpression expression)
  {
    StringBuilder sb = new StringBuilder();
    switch (operatorType)
    {
      case CodeUnaryOperatorType.BooleanNot:
        sb.Append ("(! (");
      case CodeUnaryOperatorType.Negate:
        sb.Append ("(- (");
      case CodeUnaryOperatorType.Plus:
        sb.Append ("(+ (");
      case CodeUnaryOperatorType.OnesComplement:
        sb.Append ("(~ (");
    }
    StringWriter writer = new StringWriter (sb);
    Generator.GenerateCodeFromExpression (expression, writer, null);
    sb.Append ("))");
    return new CodeSnippetExpression (sb.ToString());
  }
}

}
