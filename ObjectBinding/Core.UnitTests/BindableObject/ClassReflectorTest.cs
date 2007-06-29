using System;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Rubicon.ObjectBinding.BindableObject;
using Rubicon.ObjectBinding.UnitTests.BindableObject.TestDomain;

namespace Rubicon.ObjectBinding.UnitTests.BindableObject
{
  [TestFixture]
  public class ClassReflectorTest
  {
    private BindableObjectProvider _businessObjectProvider;
    private Type _type;
    private ClassReflector _classReflector;

    [SetUp]
    public void SetUp ()
    {
      _type = typeof (SimpleClass);
      _businessObjectProvider = new BindableObjectProvider ();
      _classReflector = new ClassReflector (_type, _businessObjectProvider);
    }

    [Test]
    public void Initialize ()
    {
      Assert.That (_classReflector.Type, Is.SameAs (_type));
      Assert.That (_classReflector.BusinessObjectProvider, Is.SameAs (_businessObjectProvider));
    }

    [Test]
    public void GetMetadata ()
    {
      BindableObjectClass bindableObjectClass = _classReflector.GetMetadata ();

      Assert.That (bindableObjectClass.Type, Is.SameAs (typeof (SimpleClass)));
      Assert.That (bindableObjectClass.GetPropertyDefinitions()[0].Identifier, Is.EqualTo ("String"));
      Assert.That (bindableObjectClass.GetPropertyDefinitions ()[0].BusinessObjectProvider, Is.SameAs (_businessObjectProvider));
    }


    [Test]
    public void GetMetadata_FromCache ()
    {
      ClassReflector otherClassReflector = new ClassReflector (_type, _businessObjectProvider);
      Assert.That (otherClassReflector.GetMetadata(), Is.SameAs (_classReflector.GetMetadata ()));
    }
  }
}