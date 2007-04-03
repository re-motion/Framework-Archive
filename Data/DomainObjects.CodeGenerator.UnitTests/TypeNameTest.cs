using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;

namespace Rubicon.Data.DomainObjects.CodeGenerator.UnitTests
{
  [TestFixture]
  public class TypeNameTest
  {
    // types

    // static members and constants

    // member fields

    private string _assemblyQualifiedName;
    private TypeName _typeName;
    private TypeName _typeNameOfNestedType;

    // construction and disposing

    public TypeNameTest ()
    {
    }

    // methods and properties

    [SetUp]
    public void SetUp ()
    {
      _assemblyQualifiedName = "Rubicon.Data.DomainObjects.CodeGenerator.UnitTests.TypeNameTest, Rubicon.Data.DomainObjects.CodeGenerator.UnitTests";
      _typeName = new TypeName (_assemblyQualifiedName);

      _typeNameOfNestedType = new TypeName (
          "Rubicon.Data.DomainObjects.CodeGenerator.UnitTests.TypeNameTest+NestedType, Rubicon.Data.DomainObjects.CodeGenerator.UnitTests");
    }

    [Test]
    public void Initialize ()
    {
      Assert.AreEqual (_assemblyQualifiedName, _typeName.AssemblyQualifiedName);
    }

    [Test]
    public void InitializeWithParts ()
    {
      TypeName typeName = new TypeName (
          "Rubicon.Data.DomainObjects.CodeGenerator.UnitTests.TypeNameTest", 
          "Rubicon.Data.DomainObjects.CodeGenerator.UnitTests");

      Assert.AreEqual (_assemblyQualifiedName, typeName.AssemblyQualifiedName);
    }

    [Test]
    public void FullName ()
    {
      Assert.AreEqual ("Rubicon.Data.DomainObjects.CodeGenerator.UnitTests.TypeNameTest", _typeName.FullName);
    }

    [Test]
    public void FullNameIsTrimmed ()
    {
      TypeName typeName = new TypeName (
          "Rubicon.Data.DomainObjects.CodeGenerator.UnitTests.TypeNameTest , Rubicon.Data.DomainObjects.CodeGenerator.UnitTests");

      Assert.AreEqual ("Rubicon.Data.DomainObjects.CodeGenerator.UnitTests.TypeNameTest", typeName.FullName);
    }

    [Test]
    public void AssemblyName ()
    {
      Assert.IsNotNull (_typeName.AssemblyName);
      Assert.AreEqual ("Rubicon.Data.DomainObjects.CodeGenerator.UnitTests", _typeName.AssemblyName.FullName);
    }

    [Test]
    public void IsNested ()
    {
      Assert.IsFalse (_typeName.IsNested);
      Assert.IsTrue (_typeNameOfNestedType.IsNested);
    }

    [Test]
    public void DeclaringTypeName ()
    {
      Assert.IsNull (_typeName.DeclaringTypeName);

      Assert.IsNotNull (_typeNameOfNestedType.DeclaringTypeName);
      Assert.AreEqual (_assemblyQualifiedName, _typeNameOfNestedType.DeclaringTypeName.AssemblyQualifiedName);
    }

    [Test]
    public void Name ()
    {
      Assert.AreEqual ("TypeNameTest", _typeName.Name);
      Assert.AreEqual ("NestedType", _typeNameOfNestedType.Name);
    }

    [Test]
    public void Namespace ()
    {
      Assert.AreEqual ("Rubicon.Data.DomainObjects.CodeGenerator.UnitTests", _typeName.Namespace);
      Assert.AreEqual ("Rubicon.Data.DomainObjects.CodeGenerator.UnitTests", _typeNameOfNestedType.Namespace);
    }

    [Test]
    public void ImplementsIComparable ()
    {
      Assert.IsNotNull (_typeName as IComparable);
    }

    [Test]
    public void CompareTo ()
    {
      Assert.AreEqual (0, _typeName.CompareTo (_typeName));
      Assert.AreEqual (1, _typeName.CompareTo (null));

      TypeName typeName1 = new TypeName ("Namespace.TypeName1, AssemblyName");
      TypeName typeName2 = new TypeName ("Namespace.TypeName2, AssemblyName");
      
      Assert.AreEqual (-1, typeName1.CompareTo (typeName2));
      Assert.AreEqual (1, typeName2.CompareTo (typeName1));

      TypeName typeName3 = new TypeName ("Namespace.TypeName, AssemblyName1");
      TypeName typeName4 = new TypeName ("Namespace.TypeName, AssemblyName2");

      Assert.AreEqual (-1, typeName3.CompareTo (typeName4));
      Assert.AreEqual (1, typeName4.CompareTo (typeName3));

      TypeName typeName5 = new TypeName ("Namespace.TypeName1, AssemblyName2");
      TypeName typeName6 = new TypeName ("Namespace.TypeName2, AssemblyName1");

      Assert.AreEqual (-1, typeName5.CompareTo (typeName6));
      Assert.AreEqual (1, typeName6.CompareTo (typeName5));

      TypeName typeName7 = new TypeName ("Namespace.TypeName , AssemblyName");
      TypeName typeName8 = new TypeName ("Namespace.TypeName,AssemblyName");

      Assert.AreEqual (0, typeName7.CompareTo (typeName8));
      Assert.AreEqual (0, typeName8.CompareTo (typeName7));
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), 
        ExpectedMessage = "Argument 'value' must be of type 'Rubicon.Data.DomainObjects.CodeGenerator.TypeName' but was of type 'System.String'.\r\n"
        + "Parameter name: value")]
    public void CompareToWithInvalidArgumentType ()
    {
      _typeName.CompareTo ("someText");
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), 
        ExpectedMessage = "The assemblyQualifiedName 'Rubicon.Data.DomainObjects.CodeGenerator.UnitTests.TypeNameTest' does not contain an assembly name.\r\n"
        + "Parameter name: assemblyQualifiedName")]
    public void InitializeWithMissingAssemblyName ()
    {
      new TypeName ("Rubicon.Data.DomainObjects.CodeGenerator.UnitTests.TypeNameTest");
    }

    [Test]
    [ExpectedException (typeof (ArgumentException),
        ExpectedMessage = "The assemblyQualifiedName 'Rubicon.Data.DomainObjects.CodeGenerator.UnitTests.TypeNameTest,' does contains an empty assembly name.\r\n"
        + "Parameter name: assemblyQualifiedName")]
    public void InitializeWithEmptyAssemblyName ()
    {
      new TypeName ("Rubicon.Data.DomainObjects.CodeGenerator.UnitTests.TypeNameTest,");
    }

    [Test]
    [ExpectedException (typeof (ArgumentException),
        ExpectedMessage = "The assemblyQualifiedName 'Rubicon.Data.DomainObjects.CodeGenerator.UnitTests.TypeNameTest,  ' does contains an empty assembly name.\r\n"
        + "Parameter name: assemblyQualifiedName")]
    public void InitializeWithWhiteSpaceAssemblyName ()
    {
      new TypeName ("Rubicon.Data.DomainObjects.CodeGenerator.UnitTests.TypeNameTest,  ");
    }

    [Test]
    [ExpectedException (typeof (ArgumentException),
        ExpectedMessage = "The assemblyQualifiedName ', Rubicon.Data.DomainObjects.CodeGenerator.UnitTests' does contains an empty type name.\r\n"
        + "Parameter name: assemblyQualifiedName")]
    public void InitializeWithEmptyTypeName ()
    {
      new TypeName (", Rubicon.Data.DomainObjects.CodeGenerator.UnitTests");
    }

    [Test]
    [ExpectedException (typeof (ArgumentException),
        ExpectedMessage = "The assemblyQualifiedName '  , Rubicon.Data.DomainObjects.CodeGenerator.UnitTests' does contains an empty type name.\r\n"
        + "Parameter name: assemblyQualifiedName")]
    public void InitializeWithWhiteSpaceTypeName ()
    {
      new TypeName ("  , Rubicon.Data.DomainObjects.CodeGenerator.UnitTests");
    }
  }
}
