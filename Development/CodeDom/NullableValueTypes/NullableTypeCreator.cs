using System;
using System.CodeDom;
using System.Runtime.Serialization;

namespace Rubicon.Data.NullableValueTypes.CodeDom
{

public sealed class NullableTypeCreator
{
  // static members

  public static CodeTypeDeclaration CreateNullableType (Type valueType, string nullableTypeName)
  {
    CodeTypeReference valueTypeReference = new CodeTypeReference (valueType);

    CodeTypeDeclaration nullableTypeDeclaration = new CodeTypeDeclaration (nullableTypeName);
    nullableTypeDeclaration.IsStruct = true;
    nullableTypeDeclaration.BaseTypes.Add (typeof (INaNullable));
    nullableTypeDeclaration.BaseTypes.Add (typeof (IComparable));
    nullableTypeDeclaration.BaseTypes.Add (typeof (ISerializable));
    nullableTypeDeclaration.BaseTypes.Add (typeof (IFormattable));

    // add member: private <valueType> _value;
    CodeMemberField fieldValue = new CodeMemberField (valueType, "_value");
    fieldValue.Attributes = MemberAttributes.Private;
    nullableTypeDeclaration.Members.Add (fieldValue);

    // add member: private bool _isNotNull;
    CodeMemberField fieldIsNotNull = new CodeMemberField (typeof (bool), "_isNotNull");
    fieldIsNotNull.Attributes = MemberAttributes.Private;
    nullableTypeDeclaration.Members.Add (fieldIsNotNull);

    // add member: public <nullableType> (<valueType> value)
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
    methodCompareToObject.Attributes = MemberAttributes.Public;
    methodCompareToObject.ReturnType = new CodeTypeReference (typeof (int));
    methodCompareToObject.Parameters.Add (new CodeParameterDeclarationExpression (typeof (object), "obj"));
    methodCompareToObject.Statements.Add (new CodeConditionStatement (                                            
        new CodeBinaryOperatorExpression (
            new CodeVariableReferenceExpression ("obj"),                                                          
            CodeBinaryOperatorType.IdentityEquality,                                                              
            new CodePrimitiveExpression (null)),                                                                  
        new CodeStatement[] {                                                                                     
            new CodeConditionStatement (                                                                          
                new CodePropertyReferenceExpression (CodeThisReferenceExpression(), "IsNull"),                    
                new CodeStatement[] { new CodeMethodReturnStatement (new CodePrimitiveExpression (0)) },          
                new CodeStatement[] { new CodeMethodReturnStatement (new CodePrimitiveExpression (1)) } ) } ));   
    methodCompareToObject.Statements.Add (new CodeConditionStatement (
        new CodeBinaryOperatorExpression (
            
                                      
    
    // add member: public int CompareTo (<nullableType> val)
    
    // add member: public override string ToString()
    
    // add member: public string ToString (string format, IFormatProvider formatProvider)
    
    // add member: public bool IsNull
    
    // add member: public <valueType> Value
    
    // add member: public void GetObjectData (SerializationInfo info, StreamingContext context)
    
    // add member: public <nullableType> (SerializationInfo info, StreamingContext context)
    
    // add member: public override bool Equals (object obj)
    
    // add member: public bool Equals (<nullableType> value)
    
    // add member: public static bool Equals (<nullableType> x, <nullableType> y)
    
    // add member: public override int GetHashCode()
    
    // add member: public static bool operator == (<nullableType> x, <nullableType> y)
    
    // add member: public static bool operator != (<nullableType> x, <nullableType> y)
    
    // add member: public static implicit operator <nullableType> (<valueType> value)
    
    // add member: public static explicit operator <valueType> (<nullableType> value)

    return naEnumDeclaration;
  }

  public static CodeTypeDeclaration CreateNullableEnumType (Type enumType, string nullableTypeName)
  {
    if (! enumType.IsEnum) throw new ArgumentException ("Argument must be an enumeration type", "enumType");

    return CreateNullableType (enumType, nullableTypeName);
  }

  // construction and disposal

	private NullableTypeCreator ()
	{
	}
}

}
