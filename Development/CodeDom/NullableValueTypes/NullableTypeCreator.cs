using System;
using System.CodeDom;
using System.Runtime.Serialization;

using Rubicon.NullableValueTypes;
using Rubicon.Development.CodeDom;

namespace Rubicon.Development.CodeDom.NullableValueTypes
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

    string unqualifiedValueTypeName = valueTypeName;
    int posLastDot = valueTypeName.LastIndexOfAny (new char[] {'.', '+'});
    if (posLastDot >= 0)
      unqualifiedValueTypeName = unqualifiedValueTypeName.Substring (posLastDot + 1);

    CodeTypeReference valueTypeReference = new CodeTypeReference (valueTypeName);
    CodeTypeReference nullableTypeReference = new CodeTypeReference (nullableTypeName);
    CodeTypeReferenceExpression nullableTypeReferenceExpression = new CodeTypeReferenceExpression (nullableTypeReference);

    CodeTypeDeclaration nullableTypeDeclaration = provider.CreateStructWithConstructors (nullableTypeName);
    nullableTypeDeclaration.IsStruct = true;
    nullableTypeDeclaration.BaseTypes.Add (typeof (INaNullable));
    nullableTypeDeclaration.BaseTypes.Add (typeof (IComparable));
    nullableTypeDeclaration.BaseTypes.Add (typeof (ISerializable));
    nullableTypeDeclaration.BaseTypes.Add (typeof (IFormattable));

    // add field: private <ValueType> _value;
    CodeMemberField fieldValue = new CodeMemberField (valueTypeReference, "_value");
    fieldValue.Attributes = MemberAttributes.Private;
    nullableTypeDeclaration.Members.Add (fieldValue);

    // add field: private bool _isNotNull;
    CodeMemberField fieldIsNotNull = new CodeMemberField (typeof (bool), "_isNotNull");
    fieldIsNotNull.Attributes = MemberAttributes.Private;
    nullableTypeDeclaration.Members.Add (fieldIsNotNull);

    // add static property: public static NullableType Null 
    CodeMemberProperty propertyNull = new CodeMemberProperty ();
    propertyNull.Name = "Null";
    propertyNull.Type = nullableTypeReference;
    propertyNull.Attributes = MemberAttributes.Static | MemberAttributes.Public;
    propertyNull.HasGet = true;
    propertyNull.GetStatements.Add ( new CodeMethodReturnStatement (
        new CodeObjectCreateExpression (nullableTypeReference, new CodePrimitiveExpression (true))));
    provider.AddSummaryComment (propertyNull, "Represents a null value that can be assigned to this type.");
    nullableTypeDeclaration.Members.Add (propertyNull);

    // add static field: public static readonly string NullString = "null";
    CodeMemberField fieldNullString = new CodeMemberField (typeof (string), "NullString");
    fieldNullString.Attributes = MemberAttributes.Public | MemberAttributes.Const;
    fieldNullString.InitExpression = new CodePrimitiveExpression ("null");
    provider.AddSummaryComment (fieldNullString, "The string representation of a null value.");
    provider.AddValueComment (fieldNullString, "The value is \"null\".");
    nullableTypeDeclaration.Members.Add (fieldNullString);

    // add ctor: public <NullableType> (<ValueType> value)
    CodeConstructor ctorValueType = provider.CreateStructConstructor ();
    ctorValueType.Attributes = MemberAttributes.Public;
    ctorValueType.Parameters.Add (new CodeParameterDeclarationExpression (valueTypeReference, "value"));
    ctorValueType.Statements.Add (new CodeAssignStatement (
        new CodeFieldReferenceExpression (new CodeThisReferenceExpression(), "_isNotNull"),
        new CodePrimitiveExpression (true)));
    ctorValueType.Statements.Add (new CodeAssignStatement (
        new CodeFieldReferenceExpression (new CodeThisReferenceExpression(), "_value"),
        new CodeVariableReferenceExpression ("value")));
    provider.AddSummaryComment (ctorValueType, "Creates a new instance with the specified value.");
    nullableTypeDeclaration.Members.Add (ctorValueType);

    // add ctor: private NullableType (bool isNull)
    CodeConstructor ctorBool = provider.CreateStructConstructor ();
    ctorBool.Attributes = MemberAttributes.Private;
    ctorBool.Parameters.Add (new CodeParameterDeclarationExpression (typeof (bool), "isNull"));
    ctorBool.Statements.Add (new CodeAssignStatement (
        new CodeFieldReferenceExpression (new CodeThisReferenceExpression(), "_isNotNull"),
        provider.CreateUnaryOperatorExpression (
            CodeUnaryOperatorType.BooleanNot,
            new CodeArgumentReferenceExpression ("isNull"))));
    ctorBool.Statements.Add (new CodeAssignStatement (
        new CodeFieldReferenceExpression (new CodeThisReferenceExpression(), "_value"),
        new CodePrimitiveExpression (0)));
    nullableTypeDeclaration.Members.Add (ctorBool);
    
    // add method: public int CompareTo (object obj)
    CodeMemberMethod methodCompareToObject = new CodeMemberMethod ();
    methodCompareToObject.Name = "CompareTo";
    methodCompareToObject.Attributes = MemberAttributes.Public | MemberAttributes.Final | MemberAttributes.Overloaded;
    methodCompareToObject.ImplementationTypes.Add (typeof (IComparable));
    methodCompareToObject.ReturnType = new CodeTypeReference (typeof (int));
    methodCompareToObject.Parameters.Add (new CodeParameterDeclarationExpression (typeof (object), "obj"));
    methodCompareToObject.Statements.Add (
        new CodeConditionStatement (                                            
            new CodeBinaryOperatorExpression (
                new CodeVariableReferenceExpression ("obj"),                                                          
                CodeBinaryOperatorType.IdentityEquality,                                                              
                new CodePrimitiveExpression (null)),                                                                  
            new CodeConditionStatement (                                                                          
                new CodePropertyReferenceExpression (new CodeThisReferenceExpression(), "IsNull"),                    
                new CodeStatement[] { new CodeMethodReturnStatement (new CodePrimitiveExpression (0)) },          
                new CodeStatement[] { new CodeMethodReturnStatement (new CodePrimitiveExpression (1)) } ) 
            ));   
    methodCompareToObject.Statements.Add (
        new CodeConditionStatement (
            provider.CreateUnaryOperatorExpression ( // WORKAROUND: CodeBinaryOperatorType.IdentityInequality does not work for VB
                CodeUnaryOperatorType.BooleanNot, 
                new CodeBinaryOperatorExpression (
                    new CodeMethodInvokeExpression (
                        new CodeVariableReferenceExpression ("obj"),
                        "GetType"),
                    CodeBinaryOperatorType.IdentityEquality,
                    new CodeTypeOfExpression (nullableTypeReference))),
            new CodeThrowExceptionStatement (new CodeObjectCreateExpression (typeof (ArgumentException), new CodePrimitiveExpression ("obj"))) 
            ));
    methodCompareToObject.Statements.Add (
        new CodeMethodReturnStatement (
            new CodeMethodInvokeExpression (
                new CodeThisReferenceExpression(),
                "CompareTo", 
                new CodeCastExpression (nullableTypeReference, new CodeVariableReferenceExpression ("obj")))));                                     
    provider.AddSummaryComment (methodCompareToObject, "Returns -1 if the current value is less than the specified argument, 0 if it is equal and 1 if it is greater. Null and null references are considered equal.");
    nullableTypeDeclaration.Members.Add (methodCompareToObject);
    
    // add method: public int CompareTo (<NullableType> val)
    CodeMemberMethod methodCompareToNullableType = new CodeMemberMethod ();
    methodCompareToNullableType.Name = "CompareTo";
    methodCompareToNullableType.Attributes = MemberAttributes.Public | MemberAttributes.Final | MemberAttributes.Overloaded;
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
            new CodeMethodReturnStatement (new CodePrimitiveExpression (1))));
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
    provider.AddSummaryComment (methodCompareToNullableType, "Returns -1 if the current value is less than the specified argument, 0 if it is equal and 1 if it is greater. Null references are considered equal.");
    nullableTypeDeclaration.Members.Add (methodCompareToNullableType);

    // add method: public override string ToString()
    CodeMemberMethod methodToStringVoid = new CodeMemberMethod ();
    methodToStringVoid.Name = "ToString";
    methodToStringVoid.Attributes = MemberAttributes.Public | MemberAttributes.Overloaded | MemberAttributes.Override;
    methodToStringVoid.ReturnType = new CodeTypeReference (typeof (string));
    methodToStringVoid.Statements.Add (
        new CodeConditionStatement (
            new CodePropertyReferenceExpression (new CodeThisReferenceExpression(), "IsNull"),
            new CodeMethodReturnStatement ( new CodeFieldReferenceExpression (nullableTypeReferenceExpression, "NullString"))));
    methodToStringVoid.Statements.Add (
        new CodeMethodReturnStatement (
            new CodeMethodInvokeExpression (
                new CodeFieldReferenceExpression (new CodeThisReferenceExpression(), "_value"),
                "ToString")));
    provider.AddSummaryComment (methodToStringVoid, "Returns a String that represents the current value.");
    nullableTypeDeclaration.Members.Add (methodToStringVoid);
    
    // add method: public string ToString (string format, IFormatProvider formatProvider)
    CodeMemberMethod methodToStringFormatFormatProvider = new CodeMemberMethod ();
    methodToStringFormatFormatProvider.Name = "ToString";
    methodToStringFormatFormatProvider.Attributes = MemberAttributes.Public | MemberAttributes.Final | MemberAttributes.Overloaded;
    methodToStringFormatFormatProvider.ImplementationTypes.Add (typeof (IFormattable));
    methodToStringFormatFormatProvider.ReturnType = new CodeTypeReference (typeof (string));
    methodToStringFormatFormatProvider.Parameters.Add (new CodeParameterDeclarationExpression (typeof (string), "format"));
    methodToStringFormatFormatProvider.Parameters.Add (new CodeParameterDeclarationExpression (typeof (IFormatProvider), "formatProvider"));
    methodToStringFormatFormatProvider.Statements.Add (
        new CodeConditionStatement (
            new CodePropertyReferenceExpression (new CodeThisReferenceExpression(), "IsNull"),
            new CodeMethodReturnStatement ( new CodeFieldReferenceExpression (nullableTypeReferenceExpression, "NullString"))));
    methodToStringFormatFormatProvider.Statements.Add (
        new CodeMethodReturnStatement (
            new CodeMethodInvokeExpression (
                new CodeFieldReferenceExpression (new CodeThisReferenceExpression(), "_value"),
                "ToString",
                new CodeVariableReferenceExpression ("format"),
                new CodeVariableReferenceExpression ("formatProvider"))));
    provider.AddSummaryComment (methodToStringFormatFormatProvider, "Returns a String that represents the current value.");
    nullableTypeDeclaration.Members.Add (methodToStringFormatFormatProvider);
    
    // add property: public bool IsNull
    CodeMemberProperty propertyIsNull = new CodeMemberProperty ();
    propertyIsNull.Name = "IsNull";
    propertyIsNull.Attributes = MemberAttributes.Public | MemberAttributes.Final;
    propertyIsNull.ImplementationTypes.Add (typeof (System.Data.SqlTypes.INullable));
    propertyIsNull.Type = new CodeTypeReference (typeof (bool));
    propertyIsNull.HasGet = true;
    propertyIsNull.GetStatements.Add (
        new CodeMethodReturnStatement (
            provider.CreateUnaryOperatorExpression (
                CodeUnaryOperatorType.BooleanNot, 
                new CodeFieldReferenceExpression (new CodeThisReferenceExpression(), "_isNotNull"))));
    provider.AddSummaryComment (propertyIsNull, "Returns true if the current value is Null.");
    nullableTypeDeclaration.Members.Add (propertyIsNull);

    // add property: public <ValueType> Value
    CodeMemberProperty propertyValue = new CodeMemberProperty ();
    propertyValue.Name = "Value";
    propertyValue.Attributes = MemberAttributes.Public | MemberAttributes.Final;
    propertyValue.Type = valueTypeReference;
    propertyValue.HasGet = true;
    propertyValue.GetStatements.Add (
        new CodeConditionStatement (
            new CodePropertyReferenceExpression (new CodeThisReferenceExpression(), "IsNull"),
            new CodeThrowExceptionStatement (
                new CodeMethodInvokeExpression (
                    new CodeTypeReferenceExpression (typeof (NaNullValueException)), 
                    "AccessingMember", 
                    new CodePrimitiveExpression ("Value")))));
    propertyValue.GetStatements.Add (
        new CodeMethodReturnStatement (new CodeFieldReferenceExpression (new CodeThisReferenceExpression(), "_value")));
    provider.AddSummaryComment (propertyValue, "Returns the value if the current value is not Null.");
    provider.AddExceptionComment (propertyValue, typeof (NaNullValueException), "The current value is Null.");
    nullableTypeDeclaration.Members.Add (propertyValue);
    
    // add method: public void GetObjectData (SerializationInfo info, StreamingContext context)
    CodeMemberMethod methodGetObjectData = new CodeMemberMethod ();
    methodGetObjectData.Name = "GetObjectData";
    methodGetObjectData.Attributes = MemberAttributes.Public | MemberAttributes.Final;
    methodGetObjectData.ImplementationTypes.Add (typeof (ISerializable));
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
    provider.AddSummaryComment (methodGetObjectData, "Stores the current value in a serialization stream.");
    nullableTypeDeclaration.Members.Add (methodGetObjectData);

    // add ctor: private <NullableType> (SerializationInfo info, StreamingContext context)
    CodeConstructor ctorSerialization = provider.CreateStructConstructor ();
    ctorSerialization.Attributes = MemberAttributes.Private;
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
    
    // add method: public override bool Equals (object obj)
    CodeMemberMethod methodEqualsObject = new CodeMemberMethod();
    methodEqualsObject.Name = "Equals";
    methodEqualsObject.Attributes = MemberAttributes.Public | MemberAttributes.Override | MemberAttributes.Overloaded;
    methodEqualsObject.ReturnType = new CodeTypeReference (typeof (bool));
    methodEqualsObject.Parameters.Add (new CodeParameterDeclarationExpression (typeof (object), "obj"));
    methodEqualsObject.Statements.Add (
        new CodeConditionStatement (
            new CodeBooleanOrExpression (
                new CodeBinaryOperatorExpression (
                    new CodeArgumentReferenceExpression ("obj"),
                    CodeBinaryOperatorType.IdentityEquality,
                    new CodePrimitiveExpression (null)),
                provider.CreateUnaryOperatorExpression ( // WORKAROUND: CodeBinaryOperatorType.IdentityInequality does not work for VB
                    CodeUnaryOperatorType.BooleanNot,
                    new CodeBinaryOperatorExpression (
                        new CodeMethodInvokeExpression (
                            new CodeArgumentReferenceExpression ("obj"),
                            "GetType"),
                        CodeBinaryOperatorType.IdentityEquality,
                        new CodeTypeOfExpression (nullableTypeReference)))),
            new CodeMethodReturnStatement (new CodePrimitiveExpression (false))));
    methodEqualsObject.Statements.Add (
        new CodeMethodReturnStatement (
            new CodeMethodInvokeExpression (
              nullableTypeReferenceExpression,
              "Equals",
              new CodeThisReferenceExpression (),
              new CodeCastExpression (nullableTypeReference, new CodeArgumentReferenceExpression ("obj")))));
    provider.AddSummaryComment (methodEqualsObject, "Returns true if the value of the current object is equal to the specified object.");
    nullableTypeDeclaration.Members.Add (methodEqualsObject);
    
    // add method: public bool Equals (<NullableType> value)
    CodeMemberMethod methodEqualsNullableType = new CodeMemberMethod();
    methodEqualsNullableType.Name = "Equals";
    methodEqualsNullableType.Attributes = MemberAttributes.Public | MemberAttributes.Final | MemberAttributes.Overloaded;
    methodEqualsNullableType.ReturnType = new CodeTypeReference (typeof (bool));
    methodEqualsNullableType.Parameters.Add (new CodeParameterDeclarationExpression (nullableTypeReference, "value"));
    methodEqualsNullableType.Statements.Add (
        new CodeMethodReturnStatement (
            new CodeMethodInvokeExpression (
              nullableTypeReferenceExpression,
              "Equals",
              new CodeThisReferenceExpression (),
              new CodeArgumentReferenceExpression ("value"))));
    provider.AddSummaryComment (methodEqualsNullableType, "Returns true if the value of the current object is equal to the specified object.");
    nullableTypeDeclaration.Members.Add (methodEqualsNullableType);
    
    // add static method: public static bool Equals (<NullableType> x, <NullableType> y)
    CodeMemberMethod methodStaticEquals = new CodeMemberMethod();
    methodStaticEquals.Name = "Equals";
    methodStaticEquals.Attributes = MemberAttributes.Public | MemberAttributes.Static | MemberAttributes.Overloaded;
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
    provider.AddSummaryComment (methodStaticEquals, "Returns true if the values of the specified objects are equal.");
    nullableTypeDeclaration.Members.Add (methodStaticEquals);

    // add method: public override int GetHashCode()
    CodeMemberMethod methodGetHashCode = new CodeMemberMethod();
    methodGetHashCode.Name = "GetHashCode";
    methodGetHashCode.Attributes = MemberAttributes.Public | MemberAttributes.Override;
    methodGetHashCode.ReturnType = new CodeTypeReference (typeof (int));
    methodGetHashCode.Statements.Add (
        new CodeConditionStatement (
            new CodePropertyReferenceExpression (new CodeThisReferenceExpression(), "IsNull"),
            new CodeMethodReturnStatement (new CodePrimitiveExpression (0))));
    methodGetHashCode.Statements.Add (
        new CodeMethodReturnStatement (
            new CodeMethodInvokeExpression (
                new CodeFieldReferenceExpression (new CodeThisReferenceExpression(), "_value"),
                "GetHashCode")));
    provider.AddSummaryComment (methodGetHashCode, "Returns the hash code for this instance.");
    nullableTypeDeclaration.Members.Add (methodGetHashCode);

    // add static method: public static object ToBoxed<ValueType> (<NullableType> value)
    CodeMemberMethod methodToBoxedValueType = new CodeMemberMethod ();
    methodToBoxedValueType.Name = "ToBoxed" + unqualifiedValueTypeName;
    methodToBoxedValueType.Attributes = MemberAttributes.Public | MemberAttributes.Static;
    methodToBoxedValueType.ReturnType = new CodeTypeReference (typeof (object));
    methodToBoxedValueType.Parameters.Add (new CodeParameterDeclarationExpression (nullableTypeReference, "value"));
    methodToBoxedValueType.Statements.Add (new CodeConditionStatement (
        new CodePropertyReferenceExpression (new CodeArgumentReferenceExpression ("value"), "IsNull"),
        new CodeStatement[] { 
            new CodeMethodReturnStatement (new CodePrimitiveExpression (null)) 
        },
        new CodeStatement[] { 
            new CodeMethodReturnStatement (
                new CodeFieldReferenceExpression ( new CodeArgumentReferenceExpression ("value"), "_value")) 
        } ));
    provider.AddSummaryComment (methodToBoxedValueType, "Creates a boxed instance of the underlying value type, or a null reference if the current instance is Null.");
    nullableTypeDeclaration.Members.Add (methodToBoxedValueType);

    // add static method: public static object ToBoxedInt32 (<NullableType> value)
    CodeMemberMethod methodToBoxedInt32 = new CodeMemberMethod ();
    methodToBoxedInt32.Name = "ToBoxedInt32";
    methodToBoxedInt32.Attributes = MemberAttributes.Public | MemberAttributes.Static;
    methodToBoxedInt32.ReturnType = new CodeTypeReference (typeof (object));
    methodToBoxedInt32.Parameters.Add (new CodeParameterDeclarationExpression (nullableTypeReference, "value"));
    methodToBoxedInt32.Statements.Add (new CodeConditionStatement (
        new CodePropertyReferenceExpression (new CodeArgumentReferenceExpression ("value"), "IsNull"),
        new CodeStatement[] { 
            new CodeMethodReturnStatement (new CodePrimitiveExpression (null)) 
        },
        new CodeStatement[] { 
            new CodeMethodReturnStatement (
                new CodeCastExpression (
                    typeof (Int32),
                    new CodeFieldReferenceExpression ( new CodeArgumentReferenceExpression ("value"), "_value"))) 
        } ));
    provider.AddSummaryComment (methodToBoxedInt32, "Creates a boxed instance of the underlying value type, or a null reference if the current instance is Null.");
    nullableTypeDeclaration.Members.Add (methodToBoxedInt32);

    // add static method: public static NullableType FromBoxedValueType (object value)
    CodeMemberMethod methodFromBoxedValueType = new CodeMemberMethod ();
    methodFromBoxedValueType.Name = "FromBoxed" + unqualifiedValueTypeName;
    methodFromBoxedValueType.Attributes = MemberAttributes.Public | MemberAttributes.Static;
    methodFromBoxedValueType.ReturnType = nullableTypeReference;
    methodFromBoxedValueType.Parameters.Add (new CodeParameterDeclarationExpression (typeof (object), "value"));
    methodFromBoxedValueType.Statements.Add (new CodeConditionStatement (
        new CodeBinaryOperatorExpression (
            new CodeArgumentReferenceExpression ("value"),
            CodeBinaryOperatorType.IdentityEquality,
            new CodePrimitiveExpression (null)),
        new CodeMethodReturnStatement ( new CodePropertyReferenceExpression (nullableTypeReferenceExpression, "Null"))));
    methodFromBoxedValueType.Statements.Add (new CodeConditionStatement (
        provider.CreateUnaryOperatorExpression (
            CodeUnaryOperatorType.BooleanNot,
            new CodeBinaryOperatorExpression (
                new CodeMethodInvokeExpression ( new CodeArgumentReferenceExpression ("value"), "GetType"),
                CodeBinaryOperatorType.IdentityEquality,
                new CodeTypeOfExpression (valueTypeReference))),
        new CodeThrowExceptionStatement (
            new CodeObjectCreateExpression (
                typeof (ArgumentException), 
                new CodePrimitiveExpression ("Must be a " + valueTypeName + " value."), 
                new CodePrimitiveExpression ("value")))));
    methodFromBoxedValueType.Statements.Add (new CodeMethodReturnStatement (
        new CodeObjectCreateExpression (
            nullableTypeReference,
            new CodeCastExpression (valueTypeReference, new CodeArgumentReferenceExpression ("value")))));
    provider.AddSummaryComment (methodFromBoxedValueType, "Creates a new instance from a boxed value type or a null reference.");
    nullableTypeDeclaration.Members.Add (methodFromBoxedValueType);

    // add static method: public static NullableType FromBoxedInt32 (object value)
    CodeMemberMethod methodFromBoxedInt32 = new CodeMemberMethod ();
    methodFromBoxedInt32.Name = "FromBoxedInt32";
    methodFromBoxedInt32.Attributes = MemberAttributes.Public | MemberAttributes.Static;
    methodFromBoxedInt32.ReturnType = nullableTypeReference;
    methodFromBoxedInt32.Parameters.Add (new CodeParameterDeclarationExpression (typeof (object), "value"));
    methodFromBoxedInt32.Statements.Add (new CodeConditionStatement (
        new CodeBinaryOperatorExpression (
            new CodeArgumentReferenceExpression ("value"),
            CodeBinaryOperatorType.IdentityEquality,
            new CodePrimitiveExpression (null)),
        new CodeMethodReturnStatement ( new CodePropertyReferenceExpression (nullableTypeReferenceExpression, "Null"))));
    methodFromBoxedInt32.Statements.Add (new CodeConditionStatement (
        provider.CreateUnaryOperatorExpression (
            CodeUnaryOperatorType.BooleanNot,
            new CodeBinaryOperatorExpression (
                new CodeMethodInvokeExpression ( new CodeArgumentReferenceExpression ("value"), "GetType"),
                CodeBinaryOperatorType.IdentityEquality,
                new CodeTypeOfExpression (typeof (Int32)))),
        new CodeThrowExceptionStatement (
            new CodeObjectCreateExpression (
                typeof (ArgumentException), 
                new CodePrimitiveExpression ("Must be a System.Int32 value."), 
                new CodePrimitiveExpression ("value")))));
    methodFromBoxedInt32.Statements.Add (new CodeMethodReturnStatement (
        new CodeObjectCreateExpression (
            nullableTypeReference,
            new CodeCastExpression (
                valueTypeReference, 
                new CodeCastExpression (
                    new CodeTypeReference (typeof (Int32)),
                    new CodeArgumentReferenceExpression ("value"))))));
    provider.AddSummaryComment (methodFromBoxedInt32, "Creates a new instance from a boxed System.Int32 value or a null reference.");
    nullableTypeDeclaration.Members.Add (methodFromBoxedInt32);

    if (provider.SupportsOperatorOverriding)
    {
      // add operator: public static bool operator == (<NullableType> x, <NullableType> y)
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
      provider.AddSummaryComment (operatorEquality, "Returns true if the values of the specified objects are equal.");
      nullableTypeDeclaration.Members.Add (operatorEquality);

      // add operator: public static bool operator != (<NullableType> x, <NullableType> y)
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
      provider.AddSummaryComment (operatorInequality, "Returns true if the values of the specified objects are not equal.");
      nullableTypeDeclaration.Members.Add (operatorInequality);
    }
    
    if (provider.SupportsCastingOperators)
    {
      // add operator: public static implicit operator <NullableType> (<ValueType> value)
      CodeStatementCollection operatorCastFromValueTypeStatements = new CodeStatementCollection();
      operatorCastFromValueTypeStatements.Add (
          new CodeMethodReturnStatement (
              new CodeObjectCreateExpression (nullableTypeReference, new CodeArgumentReferenceExpression ("value"))));
      CodeTypeMember operatorCastFromValueType = provider.CreateCastingOperator (
          valueTypeName, nullableTypeName, "value", operatorCastFromValueTypeStatements, 
          MemberAttributes.Public | MemberAttributes.Static, CodeCastOperatorKind.Implicit);
      provider.AddSummaryComment (operatorCastFromValueType, "Implicitly casts to an instance of this type from its underlying value type.");
      nullableTypeDeclaration.Members.Add (operatorCastFromValueType);
      
      // add operator: public static explicit operator <ValueType> (<NullableType> value)
      CodeStatementCollection operatorCastToValueTypeStatements = new CodeStatementCollection();
      operatorCastToValueTypeStatements.Add (
          new CodeMethodReturnStatement (
              new CodePropertyReferenceExpression (new CodeArgumentReferenceExpression ("value"), "Value")));
      CodeTypeMember operatorCastToValueType = provider.CreateCastingOperator ( 
          nullableTypeName, valueTypeName, "value", operatorCastToValueTypeStatements, 
          MemberAttributes.Public | MemberAttributes.Static, CodeCastOperatorKind.Explicit);
      provider.AddSummaryComment (operatorCastToValueType, "Explicitly casts an instance of this type to its underlying value type.");
      provider.AddExceptionComment (operatorCastToValueType, typeof (NaNullValueException), "The current value is Null.");
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
