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

    [CommandLineFlagArgument ("verbose", false,
        Description = "Verbose error information (default is off)")]
    public bool Verbose;
  }

  class Program
  {
    static int Main (string[] args)
    {
      // parse arguments / show usage info
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

      try
      {
        // select correct CodeDOM provider
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

        // generate classes for each [WxePageFunction] class
        CodeCompileUnit unit = new CodeCompileUnit ();

        Assembly assembly = Assembly.LoadFrom (arguments.AssemblyFile);
        foreach (Type type in assembly.GetTypes ())
        {
          object[] pageAttributes = type.GetCustomAttributes (typeof (WxePageFunctionAttribute), false);
          if (pageAttributes != null && pageAttributes.Length > 0)
          {
            WxePageFunctionAttribute pageAttribute = (WxePageFunctionAttribute) pageAttributes[0];
            GenerateClass (unit, type, pageAttribute);
          }
        }

        if (unit.Namespaces.Count == 0)
        {
          Console.Error.WriteLine ("No classes with [WxePageFunction] attribute found. Possible mismatch of "
           + "assembly versions: check that wxegen.exe and Rubicon.Web.dll versions match. Generation aborted.");
          return 1;
        }

        // write generated code
			  using (TextWriter writer = new StreamWriter (arguments.OutputFile, false, Encoding.Unicode))
        {
          Console.WriteLine ("Writing classes to " + arguments.OutputFile);
          CodeGeneratorOptions options = new CodeGeneratorOptions ();
          ICodeGenerator generator = provider.CreateGenerator (arguments.OutputFile);
			    generator.GenerateCodeFromCompileUnit (unit, writer, options);
        }

        return 0;
      }
      catch (Exception e)
      {
        // write error info accpording to /verbose option
        Console.Error.WriteLine ("Execution aborted: {0}",  e.Message);
        if (arguments.Verbose)
        {
          Console.Error.WriteLine ("Detailed exception information:");
          for (Exception current = e; current != null; current = current.InnerException)
          {
            Console.Error.WriteLine ("{0}: {1}", e.GetType().FullName, e.Message);
            Console.Error.WriteLine (e.StackTrace);
          }
        }
        return 1;
      }
    }

    // generate output classes for [WxePageFunction] page class
    static void GenerateClass (CodeCompileUnit unit, Type type, WxePageFunctionAttribute pageAttribute)
    {
      CodeNamespace ns = new CodeNamespace (type.Namespace);
      unit.Namespaces.Add (ns);

      #if NET11
        // for ASP.NET 1.1, generate a "<page>Variables" class for each page that allows access to parameters and 
        // local variables from page code. This class must explicitly be referenced from the page class:
        // <class>Variables Variables { get { return ((<class>Function) CurrentFunction).PageVariables; } }
        CodeTypeDeclaration variablesClass = new CodeTypeDeclaration (type.Name + "Variables");
        ns.Types.Add (variablesClass);
        variablesClass.Attributes = MemberAttributes.Assembly;

        // private readonly NameObjectCollection Variables;
        CodeMemberField variablesField = new CodeMemberField (
            typeof (Rubicon.Collections.NameObjectCollection), "Variables");
        variablesClass.Members.Add (variablesField);

        // public AutoPageVariables (NameObjectCollection variables)
        CodeConstructor variableClassCtor = new CodeConstructor ();
        variablesClass.Members.Add (variableClassCtor);
        variableClassCtor.Attributes = MemberAttributes.Public;
        variableClassCtor.Parameters.Add (new CodeParameterDeclarationExpression (
            new CodeTypeReference (typeof (Rubicon.Collections.NameObjectCollection)),
            "variables"));
        // { Variables = variables; }
        variableClassCtor.Statements.Add (new CodeAssignStatement (
            new CodeFieldReferenceExpression (new CodeThisReferenceExpression (), "Variables"),
            new CodeArgumentReferenceExpression ("variables")));
      #else
        // for ASP.NET above 1.1, generate a partial class for the page that allows access to parameters and
        // local variables from page code
        CodeTypeDeclaration partialPageClass = new CodeTypeDeclaration (type.Name);
        ns.Types.Add (partialPageClass);
        partialPageClass.IsPartial = true;
        partialPageClass.Attributes = MemberAttributes.Public;

        // add Return() method as alias for ExecuteNextStep()
        CodeMemberMethod returnMethod = new CodeMemberMethod ();
        partialPageClass.Members.Add (returnMethod);
        returnMethod.Name = "Return";
        returnMethod.Attributes = MemberAttributes.Family | MemberAttributes.Final;
        CodeExpression executeNextStep = new CodeMethodInvokeExpression (
            new CodeThisReferenceExpression (),
            "ExecuteNextStep",
            new CodeExpression[0]);
        returnMethod.Statements.Add (executeNextStep);

        //// add Return (outPar1, outPar2, ...) method 
        //// -- removed (unneccessary, possibly confusing)
        //CodeMemberMethod returnParametersMethod = new CodeMemberMethod ();
        //foreach (WxePageParameterAttribute parameterAttribute in type.GetCustomAttributes (typeof (WxePageParameterAttribute), false))
        //{
        //  if (parameterAttribute.Direction != WxeParameterDirection.In)
        //  {
        //    returnParametersMethod.Parameters.Add (new CodeParameterDeclarationExpression (
        //        new CodeTypeReference (parameterAttribute.Type),
        //        parameterAttribute.Name));
        //    returnParametersMethod.Statements.Add (new CodeAssignStatement (
        //        new CodePropertyReferenceExpression (new CodeThisReferenceExpression (), parameterAttribute.Name),
        //        new CodeArgumentReferenceExpression (parameterAttribute.Name)));
        //  }
        //}
        //if (returnParametersMethod.Parameters.Count > 0)
        //{
        //  partialPageClass.Members.Add (returnParametersMethod);
        //  returnParametersMethod.Name = "Return";
        //  returnParametersMethod.Attributes = MemberAttributes.Family | MemberAttributes.Final;
        //  returnParametersMethod.Statements.Add (executeNextStep);
        //}
      #endif

      // generate a WxeFunction class
      CodeTypeDeclaration functionClass = new CodeTypeDeclaration (type.Name + "Function");
      ns.Types.Add (functionClass);
      functionClass.Attributes = MemberAttributes.Public;
      functionClass.BaseTypes.Add (pageAttribute.BaseClass);

      // generate local variables in partial/variables class, and
      // generate function parameters in partial/variables class and function class
      foreach (WxePageVariableAttribute variableAttribute in type.GetCustomAttributes (typeof (WxePageVariableAttribute), false))
      {
        CodeMemberProperty localProperty = new CodeMemberProperty ();
        localProperty.Name = variableAttribute.Name;
        localProperty.Type = new CodeTypeReference (variableAttribute.Type);
        #if NET11
          variablesClass.Members.Add (localProperty);
          localProperty.Attributes = MemberAttributes.Public | MemberAttributes.Final;
        #else 
          partialPageClass.Members.Add (localProperty);
          localProperty.Attributes = MemberAttributes.Family | MemberAttributes.Final;
        #endif

        WxePageParameterAttribute parameterAttribute = variableAttribute as WxePageParameterAttribute;
        CodeMemberProperty functionProperty = null;
        if (parameterAttribute != null)
        {
          functionProperty = new CodeMemberProperty ();
          functionClass.Members.Add (functionProperty);
          functionProperty.Name = parameterAttribute.Name;
          functionProperty.Type = new CodeTypeReference (parameterAttribute.Type);
          functionProperty.Attributes = MemberAttributes.Public | MemberAttributes.Final;
        }

        // <variable> := Variables["<parameterName>"]
        CodeExpression variable = new CodeIndexerExpression (
            new CodePropertyReferenceExpression (new CodeThisReferenceExpression (), "Variables"),
            new CodePrimitiveExpression (variableAttribute.Name));

        // <getStatement> := get { return (<type>) <variable>; }
        CodeStatement getStatement = new CodeMethodReturnStatement (
            new CodeCastExpression (
                new CodeTypeReference (variableAttribute.Type),
                variable));

        // <setStatement> := set { <variable> = value; }
        CodeStatement setStatement = new CodeAssignStatement (
            variable,
            new CodePropertySetValueReferenceExpression ());

        if (parameterAttribute != null)
        {
          // add get/set accessors according to parameter direction
          if (parameterAttribute.Direction != WxeParameterDirection.Out)
          {
            // In or InOut: get from page, set from function
            localProperty.GetStatements.Add (getStatement);
            functionProperty.SetStatements.Add (setStatement);
          }

          if (parameterAttribute.Direction != WxeParameterDirection.In)
          {
            // Out or InOut: set from page, get from function
            localProperty.SetStatements.Add (setStatement);
            functionProperty.GetStatements.Add (getStatement);
          }
        }
        else
        {
          // variables always have get and set, and are only added to the local variable collection
          localProperty.GetStatements.Add (getStatement);
          localProperty.SetStatements.Add (setStatement);
        }

        if (functionProperty != null)
        {
          // add attribute [WxeParameter (Index, [Required,] Direction)
          CodeAttributeDeclaration wxeParameterAttribute = new CodeAttributeDeclaration (
              new CodeTypeReference (typeof (WxeParameterAttribute)));
          functionProperty.CustomAttributes.Add (wxeParameterAttribute);
          wxeParameterAttribute.Arguments.Add (new CodeAttributeArgument (
              new CodePrimitiveExpression (parameterAttribute.Index)));
          if (!parameterAttribute.Required.IsNull)
          {
            wxeParameterAttribute.Arguments.Add (new CodeAttributeArgument (
                new CodePrimitiveExpression (parameterAttribute.Required.Value)));
          }
          wxeParameterAttribute.Arguments.Add (new CodeAttributeArgument (
              new CodeFieldReferenceExpression (
                  new CodeTypeReferenceExpression (typeof (WxeParameterDirection)),
                  parameterAttribute.Direction.ToString ())));
        }
      }

      #if NET11
        // AutoPageVariables PageVariables
        CodeMemberProperty pageVariablesProperty = new CodeMemberProperty ();
        functionClass.Members.Add (pageVariablesProperty);
        pageVariablesProperty.Name = "PageVariables";
        pageVariablesProperty.Type = new CodeTypeReference (variablesClass.Name);
        pageVariablesProperty.Attributes = MemberAttributes.Public | MemberAttributes.Final;
        // get { return new <variablesClass> (Variables); }
        pageVariablesProperty.GetStatements.Add (new CodeMethodReturnStatement (
            new CodeObjectCreateExpression (
                new CodeTypeReference (variablesClass.Name),
                new CodeExpression[] {
                    new CodePropertyReferenceExpression (new CodeThisReferenceExpression (), "Variables") 
                })));
      #endif

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

      // ctor (params object[] args): base (args) {}
      CodeConstructor untypedCtor = new CodeConstructor ();
      functionClass.Members.Add (untypedCtor);
      untypedCtor.Attributes = MemberAttributes.Public;
      CodeParameterDeclarationExpression untypedParameters = new CodeParameterDeclarationExpression (
          new CodeTypeReference (typeof (object[])),
          "args");
      untypedParameters.CustomAttributes.Add (new CodeAttributeDeclaration ("System.ParamArrayAttribute"));
      untypedCtor.Parameters.Add (untypedParameters);
      untypedCtor.BaseConstructorArgs.Add (new CodeArgumentReferenceExpression ("args"));

      // ctor (<type1> inarg1, <type2> inarg2, ...): base (inarg1, inarg2, ...) {}
      CodeConstructor typedCtor = new CodeConstructor ();
      typedCtor.Attributes = MemberAttributes.Public;
      foreach (WxePageParameterAttribute parameterAttribute in type.GetCustomAttributes (typeof (WxePageParameterAttribute), false))
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
