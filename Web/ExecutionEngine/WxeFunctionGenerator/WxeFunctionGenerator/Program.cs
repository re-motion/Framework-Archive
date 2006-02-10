using System;
using System.Reflection;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.CodeDom;
using System.CodeDom.Compiler;
using Rubicon.Text.CommandLine;
using Rubicon.Web.ExecutionEngine;

namespace WxeFunctionGenerator
{
  public enum Language
  {
    CSharp, VB
  }

  public class Arguments
  {
    [CommandLineStringArgument (false, 
        Description = "File name of the assembly containing the ASP.NET page classes", 
        Placeholder = "assemblyfile")]
    public string AssemblyFile;

    [CommandLineStringArgument (false,
        Description = "Output file",
        Placeholder = "outputfile")]
    public string OutputFile;

    [CommandLineStringArgument ("language", true, 
        Description = "Language (default is CSharp)",
        Placeholder = "{CSharp|VB}")]
    public Language Language = Language.CSharp;
  }

  class Program
  {
    static int Main (string[] args)
    {
      Arguments arguments;
      CommandLineClassParser parser = new CommandLineClassParser (typeof (Arguments));
      try
      {
        arguments = (Arguments) parser.Parse (args);
      }
      catch (CommandLineArgumentException)
      {
        string appName = System.IO.Path.GetFileName (System.Environment.GetCommandLineArgs ()[0]);
        Console.Error.WriteLine ("Usage: " + parser.GetAsciiSynopsis (appName, 79));
        return 1;
      }

      CodeDomProvider provider;
      switch (arguments.Language)
      {
        case Language.CSharp:
          provider = new Microsoft.CSharp.CSharpCodeProvider ();
          break;
        case Language.VB:
          provider = new Microsoft.VisualBasic.VBCodeProvider ();
          break;
        default:
          throw new Exception ();
      }

      CodeCompileUnit unit = new CodeCompileUnit ();

      Assembly assembly = Assembly.LoadFrom (arguments.AssemblyFile);
      foreach (Type type in assembly.GetTypes ())
      {
        object[] pageAttributes = type.GetCustomAttributes (typeof (WxeFunctionPageAttribute), false);
        if (pageAttributes != null && pageAttributes.Length > 0)
        {
          WxeFunctionPageAttribute pageAttribute = (WxeFunctionPageAttribute) pageAttributes[0];
          GenerateClass (unit, type, pageAttribute);
        }
      }

			using (TextWriter writer = new StreamWriter (arguments.OutputFile, false, Encoding.Unicode))
      {
	  		CodeGeneratorOptions options = new CodeGeneratorOptions ();
        ICodeGenerator generator = provider.CreateGenerator (arguments.OutputFile);
			  generator.GenerateCodeFromCompileUnit (unit, writer, options);
      }

      return 0;
    }

    static void GenerateClass (CodeCompileUnit unit, Type type, WxeFunctionPageAttribute pageAttribute)
    {

      WxePageParameterAttribute[] parameterAttributes = (WxePageParameterAttribute[]) type.GetCustomAttributes (
          typeof (WxePageParameterAttribute), false);
      //WxePageParameterAttribute[] parameterAttributes = null;
      //if (untypedParameterAttributes != null)

      CodeNamespace ns = new CodeNamespace (type.Namespace);
      unit.Namespaces.Add (ns);

      CodeTypeDeclaration partialPageClass = new CodeTypeDeclaration (type.Name);
      ns.Types.Add (partialPageClass);
      partialPageClass.IsPartial = true;
      partialPageClass.Attributes = MemberAttributes.Public;

      CodeTypeDeclaration functionClass = new CodeTypeDeclaration (type.Name + "Function");
      ns.Types.Add (functionClass);
      functionClass.Attributes = MemberAttributes.Public;
      functionClass.BaseTypes.Add (pageAttribute.BaseClass);

      foreach (WxePageParameterAttribute parameterAttribute in parameterAttributes)
      {
        CodeMemberProperty pageProperty = new CodeMemberProperty ();
        partialPageClass.Members.Add (pageProperty);
        pageProperty.Name = parameterAttribute.Name;
        pageProperty.Type = new CodeTypeReference (parameterAttribute.Type);
        pageProperty.Attributes = MemberAttributes.Public | MemberAttributes.Final;

        CodeMemberProperty functionProperty = new CodeMemberProperty ();
        functionClass.Members.Add (functionProperty);
        functionProperty.Name = parameterAttribute.Name;
        functionProperty.Type = new CodeTypeReference (parameterAttribute.Type);
        functionProperty.Attributes = MemberAttributes.Public | MemberAttributes.Final;

        // <variable> := Variables["<parameterName>"
        CodeExpression variable = new CodeIndexerExpression (
            new CodePropertyReferenceExpression (new CodeThisReferenceExpression (), "Variables"),
            new CodePrimitiveExpression (parameterAttribute.Name));

        // <getStatement> := get { return (<type>) <variable>]; }
        CodeStatement getStatement = new CodeMethodReturnStatement (
            new CodeCastExpression (
                new CodeTypeReference (parameterAttribute.Type),
                variable));

        // <setStatement> := set { <variable> = value; }
        CodeStatement setStatement = new CodeAssignStatement (
            variable,
            new CodePropertySetValueReferenceExpression ());

        // add get/set accessors according to parameter direction
        if (parameterAttribute.Direction != WxeParameterDirection.Out) // In or InOut
        {
          pageProperty.GetStatements.Add (getStatement);
          functionProperty.SetStatements.Add (setStatement);
        }

        if (parameterAttribute.Direction != WxeParameterDirection.In) // Out or InOut
        {
          pageProperty.SetStatements.Add (setStatement);
          functionProperty.GetStatements.Add (getStatement);
        }

        // [WxeParameter (...)
        CodeAttributeDeclaration wxeParameterAttribute = new CodeAttributeDeclaration (
            new CodeTypeReference (typeof (WxeParameterAttribute)));
        functionProperty.CustomAttributes.Add (wxeParameterAttribute);
        wxeParameterAttribute.Arguments.Add (new CodeAttributeArgument (new CodePrimitiveExpression (parameterAttribute.Index)));
        wxeParameterAttribute.Arguments.Add (new CodeAttributeArgument (new CodePrimitiveExpression (parameterAttribute.Required)));
        wxeParameterAttribute.Arguments.Add (new CodeAttributeArgument (
            new CodeFieldReferenceExpression (
                new CodeTypeReferenceExpression (typeof (WxeParameterDirection)), 
                parameterAttribute.Direction.ToString())));
      }

      // add PageStep to WXE function
      CodeMemberField step1 = new CodeMemberField (typeof (WxePageStep), "Step1");
      functionClass.Members.Add (step1);
      step1.InitExpression = new CodeObjectCreateExpression (
          new CodeTypeReference (typeof (WxePageStep)),
          new CodePrimitiveExpression (pageAttribute.AspxPageName));

      // add constructors to WXE function

      // ctor () {}
      CodeConstructor defaultCtor = new CodeConstructor ();
      functionClass.Members.Add (defaultCtor);
      defaultCtor.Attributes = MemberAttributes.Public;

      // ctor (object[] args): base (args) {}
      // TODO: add "params" support!
      CodeConstructor untypedCtor = new CodeConstructor ();
      functionClass.Members.Add (untypedCtor);
      untypedCtor.Attributes = MemberAttributes.Public;
      untypedCtor.Parameters.Add (new CodeParameterDeclarationExpression (
          new CodeTypeReference (typeof (object[])),
          "args"));
      untypedCtor.BaseConstructorArgs.Add (new CodeArgumentReferenceExpression ("args"));

      // ctor (<type1> inarg1, <type2> inarg2, ...): base (inarg1, inarg2, ...) {}
      CodeConstructor typedCtor = new CodeConstructor ();
      typedCtor.Attributes = MemberAttributes.Public;
      foreach (WxePageParameterAttribute parameterAttribute in parameterAttributes)
      {
        if (parameterAttribute.Direction == WxeParameterDirection.Out)
          break;

        typedCtor.Parameters.Add (new CodeParameterDeclarationExpression (
            new CodeTypeReference (parameterAttribute.Type),
            parameterAttribute.Name));

        typedCtor.BaseConstructorArgs.Add (new CodeArgumentReferenceExpression (parameterAttribute.Name));
      }
      if (typedCtor.Parameters.Count > 0)
        functionClass.Members.Add (typedCtor);
    }
  }
}
