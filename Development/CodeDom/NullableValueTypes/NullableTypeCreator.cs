using System;
using System.CodeDom;
using System.Runtime.Serialization;

using Rubicon.Data.NullableValueTypes;
using Rubicon.CodeDom;

namespace Rubicon.CodeDom.NullableValueTypes
{

public sealed class NullableTypeCreator
{
  // static members

  public static CodeTypeDeclaration CreateNullableType (
      string valueTypeName, string nullableTypeName, ExtendedCodeProvider provider, 
      CodeStatementCollection serializationReadStatements, CodeStatementCollection serializationWriteStatements)
  {
    if (valueTypeName == null) throw new ArgumentException ("valueTypeName");
    if (nullableTypeName == null) throw new ArgumentException ("nullableTypeName");
    if (provider == null) throw new ArgumentException ("provider");
    if (serializationReadStatements == null) throw new ArgumentException ("serializationReadStatements");
    if (serializationWriteStatements == null) throw new ArgumentException ("serializationWriteStatements");

    CodeTypeReference valueTypeReference = new CodeTypeReference (valueTypeName);
    CodeTypeReference nullableTypeReference = new CodeTypeReference (nullableTypeName);
    CodeTypeReferenceExpression nullableTypeReferenceExpression = new CodeTypeReferenceExpression (nullableTypeReference);

    CodeTypeDeclaration nullableTypeDeclaration = new CodeTypeDeclaration (nullableTypeName);
    nullableTypeDeclaration.IsStruct = true;
    nullableTypeDeclaration.BaseTypes.Add (typeof (INaNullable));
    nullableTypeDeclaration.BaseTypes.Add (typeof (IComparable));
    nullableTypeDeclaration.BaseTypes.Add (typeof (ISerializable));
    nullableTypeDeclaration.BaseTypes.Add (typeof (IFormattable));

    // add member: private <ValueType> _value;
    CodeMemberField fieldValue = new CodeMemberField (valueTypeReference, "_value");
    fieldValue.Attributes = MemberAttributes.Private;
    nullableTypeDeclaration.Members.Add (fieldValue);

    // add member: private bool _isNotNull;
    CodeMemberField fieldIsNotNull = new CodeMemberField (typeof (bool), "_isNotNull");
    fieldIsNotNull.Attributes = MemberAttributes.Private;
    nullableTypeDeclaration.Members.Add (fieldIsNotNull);

    // add member: public static readonly string NullString = "null";
    CodeMemberField fieldNullString = new CodeMemberField (typeof (string), "NullString");
    fieldNullString.Attributes = MemberAttributes.Public | MemberAttributes.Const;
    fieldNullString.InitExpression = new CodePrimitiveExpression ("null");
    nullableTypeDeclaration.Members.Add (fieldNullString);

    // add member: public <NullableType> (<ValueType> value)
    CodeConstructor ctorValueType = new CodeConstructor ();
    ctorValueType.Attributes = MemberAttributes.Public;
    ctorValueType.Parameters.Add (new CodeParameterDeclarationExpression (valueTypeReference, "value"));
    ctorValueType.Statements.Add (new CodeAssignStatement (
        new CodeFieldReferenceExpression (new CodeThisReferenceExpression(), "_isNotNull"),
        new CodePrimitiveExpression (true)));
    ctorValueType.Statements.Add (new CodeAssignStatement (
        new CodeFieldReferenceExpression (new CodeThisReferenceExpression(), "_value"),
        new CodeVariableReferenceExpression ("value")));
    nullableTypeDeclaration.Members.Add (ctorValueType);
    
    // add member: public int CompareTo (object obj)
    CodeMemberMethod methodCompareToObject = new CodeMemberMethod ();
    methodCompareToObject.Name = "CompareTo";
    methodCompareToObject.Attributes = MemberAttributes.Public | MemberAttributes.Final;
    methodCompareToObject.ReturnType = new CodeTypeReference (typeof (int));
    methodCompareToObject.Parameters.Add (new CodeParameterDeclarationExpression (typeof (object), "obj"));
    methodCompareToObject.Statements.Add (
        new CodeConditionStatement (                                            
            new CodeBinaryOperatorExpression (
                new CodeVariableReferenceExpression ("obj"),                                                          
                CodeBinaryOperatorType.IdentityEquality,                                                              
                new CodePrimitiveExpression (null)),                                                                  
            new CodeStatement[] {                                                                                     
                new CodeConditionStatement (                                                                          
                    new CodePropertyReferenceExpression (new CodeThisReferenceExpression(), "IsNull"),                    
                    new CodeStatement[] { new CodeMethodReturnStatement (new CodePrimitiveExpression (0)) },          
                    new CodeStatement[] { new CodeMethodReturnStatement (new CodePrimitiveExpression (1)) } ) 
            } ));   
    methodCompareToObject.Statements.Add (
        new CodeConditionStatement (
            new CodeBinaryOperatorExpression (
                new CodeMethodInvokeExpression (
                    new CodeVariableReferenceExpression ("obj"),
                    "GetType"),
                CodeBinaryOperatorType.IdentityInequality,
                new CodeTypeOfExpression (nullableTypeReference)),
            new CodeStatement[] {
                new CodeThrowExceptionStatement (new CodeObjectCreateExpression (typeof (ArgumentException), new CodePrimitiveExpression ("obj"))) 
            } ));
    methodCompareToObject.Statements.Add (
        new CodeMethodReturnStatement (
            new CodeMethodInvokeExpression (
                new CodeThisReferenceExpression(),
                "CompareTo", 
                new CodeCastExpression (nullableTypeReference, new CodeVariableReferenceExpression ("obj")))));                                     
    nullableTypeDeclaration.Members.Add (methodCompareToObject);
    
    // add member: public int CompareTo (<NullableType> val)
    CodeMemberMethod methodCompareToNullableType = new CodeMemberMethod ();
    methodCompareToNullableType.Name = "CompareTo";
    methodCompareToNullableType.Attributes = MemberAttributes.Public | MemberAttributes.Final;
    methodCompareToNullableType.ReturnType = new CodeTypeReference (typeof (int));
    methodCompareToNullableType.Parameters.Add (new CodeParameterDeclarationExpression (nullableTypeReference, "val"));
    methodCompareToNullableType.Statements.Add (
        new CodeConditionStatement (
            new CodeBooleanAndExpression (
                new CodePropertyReferenceExpression (new CodeThisReferenceExpression(), "IsNull"),
                new CodePropertyReferenceExpression (new CodeVariableReferenceExpression ("val"), "IsNull")),
            new CodeMethodReturnStatement (new CodePrimitiveExpression (0))));
    methodCompareToNullableType.Statements.Add (
        new CodeConditionStatement (
            new CodePropertyReferenceExpression (new CodeThisReferenceExpression(), "IsNull"),
            new CodeMethodReturnStatement (new CodePrimitiveExpression (-1))));
    methodCompareToNullableType.Statements.Add (
        new CodeConditionStatement (
            new CodePropertyReferenceExpression (new CodeVariableReferenceExpression ("val"), "IsNull"),
            new CodeStatement[] { new CodeMethodReturnStatement (new CodePrimitiveExpression (1)) } ));
    methodCompareToNullableType.Statements.Add (
        new CodeConditionStatement (
            new CodeBinaryOperatorExpression (
                new CodeFieldReferenceExpression (new CodeThisReferenceExpression(), "_value"),
                CodeBinaryOperatorType.LessThan,
                new CodeFieldReferenceExpression (new CodeVariableReferenceExpression ("val"), "_value")),
            new CodeMethodReturnStatement (new CodePrimitiveExpression (-1))));
    methodCompareToNullableType.Statements.Add (
        new CodeConditionStatement (
            new CodeBinaryOperatorExpression (
                new CodeFieldReferenceExpression (new CodeThisReferenceExpression(), "_value"),
                CodeBinaryOperatorType.GreaterThan,
                new CodeFieldReferenceExpression (new CodeVariableReferenceExpression ("val"), "_value")),
            new CodeMethodReturnStatement (new CodePrimitiveExpression (1))));
    methodCompareToNullableType.Statements.Add (
        new CodeMethodReturnStatement (new CodePrimitiveExpression (0)));
    nullableTypeDeclaration.Members.Add (methodCompareToNullableType);

    // add member: public override string ToString()
    CodeMemberMethod methodToStringVoid = new CodeMemberMethod ();
    methodToStringVoid.Name = "ToString";
    methodToStringVoid.Attributes = MemberAttributes.Override | MemberAttributes.Public;
    methodToStringVoid.ReturnType = new CodeTypeReference (typeof (string));
    methodToStringVoid.Statements.Add (
        new CodeConditionStatement (
            new CodePropertyReferenceExpression (new CodeThisReferenceExpression(), "IsNull"),
            new CodeStatement[] { 
                new CodeMethodReturnStatement ( new CodeFieldReferenceExpression (nullableTypeReferenceExpression, "NullString")) 
            } ));
    methodToStringVoid.Statements.Add (
        new CodeMethodReturnStatement (
            new CodeMethodInvokeExpression (
                new CodeFieldReferenceExpression (new CodeThisReferenceExpression(), "_value"),
                "ToString")));
    nullableTypeDeclaration.Members.Add (methodToStringVoid);
    
    // add member: public string ToString (string format, IFormatProvider formatProvider)
    CodeMemberMethod methodToStringFormatFormatProvider = new CodeMemberMethod ();
    methodToStringFormatFormatProvider.Name = "ToString";
    methodToStringFormatFormatProvider.Attributes = MemberAttributes.Public | MemberAttributes.Final;
    methodToStringFormatFormatProvider.ReturnType = new CodeTypeReference (typeof (string));
    methodToStringFormatFormatProvider.Parameters.Add (new CodeParameterDeclarationExpression (typeof (string), "format"));
    methodToStringFormatFormatProvider.Parameters.Add (new CodeParameterDeclarationExpression (typeof (IFormatProvider), "formatProvider"));
    methodToStringFormatFormatProvider.Statements.Add (
        new CodeConditionStatement (
            new CodePropertyReferenceExpression (new CodeThisReferenceExpression(), "IsNull"),
            new CodeStatement[] { 
                new CodeMethodReturnStatement ( new CodeFieldReferenceExpression (nullableTypeReferenceExpression, "NullString")) 
            } ));
    methodToStringFormatFormatProvider.Statements.Add (
        new CodeMethodReturnStatement (
            new CodeMethodInvokeExpression (
                new CodeFieldReferenceExpression (new CodeThisReferenceExpression(), "_value"),
                "ToString",
                new CodeVariableReferenceExpression ("format"),
                new CodeVariableReferenceExpression ("formatProvider"))));
    nullableTypeDeclaration.Members.Add (methodToStringFormatFormatProvider);
    
    // add member: public bool IsNull
    CodeMemberProperty propertyIsNull = new CodeMemberProperty ();
    propertyIsNull.Name = "IsNull";
    propertyIsNull.Attributes = MemberAttributes.Public | MemberAttributes.Final;
    propertyIsNull.Type = new CodeTypeReference (typeof (bool));
    propertyIsNull.HasGet = true;
    propertyIsNull.GetStatements.Add (
        new CodeMethodReturnStatement (
            provider.CreateUnaryOperatorExpression (
                CodeUnaryOperatorType.BooleanNot, 
                new CodeFieldReferenceExpression (new CodeThisReferenceExpression(), "_isNotNull"))));
    nullableTypeDeclaration.Members.Add (propertyIsNull);

    // add member: public <ValueType> Value
    CodeMemberProperty propertyValue = new CodeMemberProperty ();
    propertyValue.Name = "Value";
    propertyValue.Attributes = MemberAttributes.Public | MemberAttributes.Final;
    propertyValue.Type = valueTypeReference;
    propertyValue.HasGet = true;
    propertyValue.GetStatements.Add (
        new CodeConditionStatement (
            new CodePropertyReferenceExpression (new CodeThisReferenceExpression(), "IsNull"),
            new CodeStatement[] {
                new CodeThrowExceptionStatement (
                    new CodeMethodInvokeExpression (
                        new CodeTypeReferenceExpression (typeof (NaNullValueException)), 
                        "AccessingMember", 
                        new CodePrimitiveExpression ("Value")))
            } ));
    propertyValue.GetStatements.Add (
        new CodeMethodReturnStatement (new CodeFieldReferenceExpression (new CodeThisReferenceExpression(), "_value")));
    nullableTypeDeclaration.Members.Add (propertyValue);
    
    // add member: public void GetObjectData (SerializationInfo info, StreamingContext context)
    CodeMemberMethod methodGetObjectData = new CodeMemberMethod ();
    methodGetObjectData.Name = "GetObjectData";
    methodGetObjectData.Attributes = MemberAttributes.Public | MemberAttributes.Final;
    methodGetObjectData.Parameters.Add (new CodeParameterDeclarationExpression (typeof (SerializationInfo), "info"));
    methodGetObjectData.Parameters.Add (new CodeParameterDeclarationExpression (typeof (StreamingContext), "context"));
    methodGetObjectData.Statements.Add (
        new CodeMethodInvokeExpression (
            new CodeArgumentReferenceExpression ("info"),
            "AddValue",
            new CodePrimitiveExpression ("IsNull"),
            new CodePropertyReferenceExpression (new CodeThisReferenceExpression (), "IsNull")));
    foreach (CodeStatement writeStatement in serializationWriteStatements)
      methodGetObjectData.Statements.Add (writeStatement);
    nullableTypeDeclaration.Members.Add (methodGetObjectData);

    // add member: public <NullableType> (SerializationInfo info, StreamingContext context)
    CodeConstructor ctorSerialization = new CodeConstructor ();
    ctorSerialization.Attributes = MemberAttributes.Public;
    ctorSerialization.Parameters.Add (new CodeParameterDeclarationExpression (typeof (SerializationInfo), "info"));
    ctorSerialization.Parameters.Add (new CodeParameterDeclarationExpression (typeof (StreamingContext), "context"));
    ctorSerialization.Statements.Add (
        new CodeAssignStatement (
            new CodeFieldReferenceExpression (new CodeThisReferenceExpression(), "_isNotNull"),
            provider.CreateUnaryOperatorExpression (
                CodeUnaryOperatorType.BooleanNot,
                new CodeMethodInvokeExpression (
                    new CodeArgumentReferenceExpression ("info"),
                    "GetBoolean",
                    new CodePrimitiveExpression ("IsNull")))));
    foreach (CodeStatement readStatement in serializationReadStatements)
      ctorSerialization.Statements.Add (readStatement);
    nullableTypeDeclaration.Members.Add (ctorSerialization);
    
    // add member: public override bool Equals (object obj)
    CodeMemberMethod methodEqualsObject = new CodeMemberMethod();
    methodEqualsObject.Name = "Equals";
    methodEqualsObject.Attributes = MemberAttributes.Public | MemberAttributes.Override;
    methodEqualsObject.ReturnType = new CodeTypeReference (typeof (bool));
    methodEqualsObject.Parameters.Add (new CodeParameterDeclarationExpression (typeof (object), "obj"));
    methodEqualsObject.Statements.Add (
        new CodeConditionStatement (
            new CodeBooleanOrExpression (
                new CodeBinaryOperatorExpression (
                    new CodeArgumentReferenceExpression ("obj"),
                    CodeBinaryOperatorType.IdentityEquality,
                    new CodePrimitiveExpression (null)),
                new CodeBinaryOperatorExpression (
                    new CodeMethodInvokeExpression (
                        new CodeArgumentReferenceExpression ("obj"),
                        "GetType"),
                    CodeBinaryOperatorType.IdentityInequality,
                    new CodeTypeOfExpression (nullableTypeReference))),
            new CodeStatement[] {
                new CodeMethodReturnStatement (new CodePrimitiveExpression (false))
            } ));
    methodEqualsObject.Statements.Add (
        new CodeMethodReturnStatement (
            new CodeMethodInvokeExpression (
              nullableTypeReferenceExpression,
              "Equals",
              new CodeThisReferenceExpression (),
              new CodeCastExpression (nullableTypeReference, new CodeArgumentReferenceExpression ("obj")))));
    nullableTypeDeclaration.Members.Add (methodEqualsObject);
    
    // add member: public bool Equals (<NullableType> value)
    CodeMemberMethod methodEqualsNullableType = new CodeMemberMethod();
    methodEqualsNullableType.Name = "Equals";
    methodEqualsNullableType.Attributes = MemberAttributes.Public | MemberAttributes.Final;
    methodEqualsNullableType.ReturnType = new CodeTypeReference (typeof (bool));
    methodEqualsNullableType.Parameters.Add (new CodeParameterDeclarationExpression (nullableTypeReference, "value"));
    methodEqualsNullableType.Statements.Add (
        new CodeMethodReturnStatement (
            new CodeMethodInvokeExpression (
              nullableTypeReferenceExpression,
              "Equals",
              new CodeThisReferenceExpression (),
              new CodeArgumentReferenceExpression ("value"))));
    nullableTypeDeclaration.Members.Add (methodEqualsNullableType);
    
    // add member: public static bool Equals (<NullableType> x, <NullableType> y)
    CodeMemberMethod methodStaticEquals = new CodeMemberMethod();
    methodStaticEquals.Name = "Equals";
    methodStaticEquals.Attributes = MemberAttributes.Public | MemberAttributes.Static;
    methodStaticEquals.ReturnType = new CodeTypeReference (typeof (bool));
    methodStaticEquals.Parameters.Add (new CodeParameterDeclarationExpression (nullableTypeReference, "x"));
    methodStaticEquals.Parameters.Add (new CodeParameterDeclarationExpression (nullableTypeReference, "y"));    
    methodStaticEquals.Statements.Add (
        new CodeConditionStatement (
            new CodeBooleanAndExpression (
                new CodePropertyReferenceExpression (new CodeArgumentReferenceExpression ("x"), "IsNull"),
                new CodePropertyReferenceExpression (new CodeArgumentReferenceExpression ("y"), "IsNull")),
            new CodeMethodReturnStatement (new CodePrimitiveExpression (true))));
    methodStaticEquals.Statements.Add (
        new CodeConditionStatement (
            new CodeBooleanOrExpression (
                new CodePropertyReferenceExpression (new CodeArgumentReferenceExpression ("x"), "IsNull"),
                new CodePropertyReferenceExpression (new CodeArgumentReferenceExpression ("y"), "IsNull")),
            new CodeMethodReturnStatement (new CodePrimitiveExpression (false))));
    methodStaticEquals.Statements.Add (
        new CodeMethodReturnStatement (
            new CodeMethodInvokeExpression (
                new CodeFieldReferenceExpression (new CodeArgumentReferenceExpression ("x"), "_value"),
                "Equals",
                new CodeFieldReferenceExpression (new CodeArgumentReferenceExpression ("y"), "_value"))));
    nullableTypeDeclaration.Members.Add (methodStaticEquals);

    // add member: public override int GetHashCode()
    CodeMemberMethod methodGetHashCode = new CodeMemberMethod();
    methodGetHashCode.Name = "GetHashCode";
    methodGetHashCode.Attributes = MemberAttributes.Public | MemberAttributes.Override;
    methodGetHashCode.ReturnType = new CodeTypeReference (typeof (int));
    methodGetHashCode.Statements.Add (
        new CodeConditionStatement (
            new CodePropertyReferenceExpression (new CodeThisReferenceExpression(), "IsNull"),
            new CodeStatement[] { new CodeMethodReturnStatement (new CodePrimitiveExpression (0)) } ));
    methodGetHashCode.Statements.Add (
        new CodeMethodReturnStatement (
            new CodeMethodInvokeExpression (
                new CodeFieldReferenceExpression (new CodeThisReferenceExpression(), "_value"),
                "GetHashCode")));
    nullableTypeDeclaration.Members.Add (methodGetHashCode);

    if (provider.SupportsOperatorOverriding)
    {
      // add member: public static bool operator == (<NullableType> x, <NullableType> y)
      CodeStatementCollection operatorEqualityStatements = new CodeStatementCollection();
      operatorEqualityStatements.Add (
          new CodeMethodReturnStatement (
              new CodeMethodInvokeExpression (
                  nullableTypeReferenceExpression, 
                  "Equals", 
                  new CodeArgumentReferenceExpression ("x"), 
                  new CodeArgumentReferenceExpression ("y"))));
      CodeTypeMember operatorEquality = provider.CreateBinaryOperator (
          nullableTypeName, "x", "y", CodeOverridableOperatorType.Equality, "bool", 
          operatorEqualityStatements, MemberAttributes.Public | MemberAttributes.Static);
      nullableTypeDeclaration.Members.Add (operatorEquality);

      // add member: public static bool operator != (<NullableType> x, <NullableType> y)
      CodeStatementCollection operatorInequalityStatements = new CodeStatementCollection();
      operatorInequalityStatements.Add (
          new CodeMethodReturnStatement (
              provider.CreateUnaryOperatorExpression (
                  CodeUnaryOperatorType.BooleanNot,
                  new CodeMethodInvokeExpression (
                      nullableTypeReferenceExpression, 
                      "Equals", 
                      new CodeArgumentReferenceExpression ("x"), 
                      new CodeArgumentReferenceExpression ("y")))));
      CodeTypeMember operatorInequality = provider.CreateBinaryOperator (
          nullableTypeName, "x", "y", CodeOverridableOperatorType.Inequality, "bool", 
          operatorInequalityStatements, MemberAttributes.Public | MemberAttributes.Static);
      nullableTypeDeclaration.Members.Add (operatorInequality);
    }
    
    if (provider.SupportsCastingOperators)
    {
      // add member: public static implicit operator <NullableType> (<ValueType> value)
      CodeStatementCollection operatorCastFromValueTypeStatements = new CodeStatementCollection();
      operatorCastFromValueTypeStatements.Add (
          new CodeMethodReturnStatement (
              new CodeObjectCreateExpression (nullableTypeReference, new CodeArgumentReferenceExpression ("value"))));
      CodeTypeMember operatorCastFromValueType = provider.CreateCastingOperator (
          valueTypeName, nullableTypeName, "value", operatorCastFromValueTypeStatements, 
          MemberAttributes.Public | MemberAttributes.Static, CodeCastOperatorKind.Implicit);
      nullableTypeDeclaration.Members.Add (operatorCastFromValueType);
      
      // add member: public static explicit operator <ValueType> (<NullableType> value)
      CodeStatementCollection operatorCastToValueTypeStatements = new CodeStatementCollection();
      operatorCastToValueTypeStatements.Add (
          new CodeMethodReturnStatement (
              new CodePropertyReferenceExpression (new CodeArgumentReferenceExpression ("value"), "Value")));
      CodeTypeMember operatorCastToValueType = provider.CreateCastingOperator ( 
          nullableTypeName, valueTypeName, "value", operatorCastToValueTypeStatements, 
          MemberAttributes.Public | MemberAttributes.Static, CodeCastOperatorKind.Explicit);
      nullableTypeDeclaration.Members.Add (operatorCastToValueType);
    }

    return nullableTypeDeclaration;
  }

  public static CodeTypeDeclaration CreateNullableEnumType (
      string enumTypeName, string nullableTypeName, ExtendedCodeProvider provider)
  {
    CodeStatementCollection serializationWriteStatements = new CodeStatementCollection();
    serializationWriteStatements.Add (
        new CodeMethodInvokeExpression (
            new CodeArgumentReferenceExpression ("info"),
            "AddValue",
            new CodePrimitiveExpression ("Value"),
            new CodeFieldReferenceExpression (new CodeThisReferenceExpression (), "_value")));

    CodeStatementCollection serializationReadStatements = new CodeStatementCollection ();
    serializationReadStatements.Add (
        new CodeAssignStatement (
            new CodeFieldReferenceExpression (new CodeThisReferenceExpression(), "_value"),
            new CodeCastExpression (
                enumTypeName, 
                new CodeMethodInvokeExpression (
                    new CodeArgumentReferenceExpression ("info"),
                    "GetInt32",
                    new CodePrimitiveExpression ("Value")))));

    return CreateNullableType (enumTypeName, nullableTypeName, provider, serializationReadStatements, serializationWriteStatements);
  }

  // construction and disposal

	private NullableTypeCreator ()
	{
	}
}

}
