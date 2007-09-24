using System;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Rubicon.ObjectBinding.BindableObject;
using Rubicon.ObjectBinding.BindableObject.Properties;
using Rubicon.ObjectBinding.UnitTests.Core.BindableObject.TestDomain;

namespace Rubicon.ObjectBinding.UnitTests.Core.BindableObject
{
  [TestFixture]
  public class ClassReflectorTest : TestBase
  {
    private BindableObjectProvider _businessObjectProvider;
    private Type _type;
    private ClassReflector _classReflector;

    public override void SetUp ()
    {
      base.SetUp();

      _type = typeof (DerivedBusinessObjectClass);
      _businessObjectProvider = new BindableObjectProvider();
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
      BindableObjectClass bindableObjectClass = _classReflector.GetMetadata();

      Assert.That (bindableObjectClass, Is.InstanceOfType (typeof (IBusinessObjectClass)));
      Assert.That (bindableObjectClass.Type, Is.SameAs (_type));
      Assert.That (bindableObjectClass.GetPropertyDefinitions().Length, Is.EqualTo (1));
      Assert.That (bindableObjectClass.GetPropertyDefinitions()[0].Identifier, Is.EqualTo ("Public"));
      Assert.That (((PropertyBase) bindableObjectClass.GetPropertyDefinitions()[0]).PropertyInfo.DeclaringType, Is.SameAs (_type));
      Assert.That (bindableObjectClass.GetPropertyDefinitions()[0].BusinessObjectProvider, Is.SameAs (_businessObjectProvider));
    }

    [Test]
    public void GetMetadata_ForBindableObjectWithIdentity ()
    {
      ClassReflector classReflector = new ClassReflector (typeof (ClassWithIdentity), _businessObjectProvider);
      BindableObjectClass bindableObjectClass = classReflector.GetMetadata();

      Assert.That (bindableObjectClass, Is.InstanceOfType (typeof (IBusinessObjectClassWithIdentity)));
      Assert.That (bindableObjectClass.Type, Is.SameAs (typeof (ClassWithIdentity)));
      Assert.That (bindableObjectClass.GetPropertyDefinitions()[0].BusinessObjectProvider, Is.SameAs (_businessObjectProvider));
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage = "Type 'Rubicon.ObjectBinding.UnitTests.Core.BindableObject.TestDomain."
        + "ClassWithManualIdentity' does not implement the 'Rubicon.ObjectBinding.IBusinessObject' interface via the 'Rubicon.ObjectBinding."
        + "BindableObject.BindableObjectMixinBase`1'.\r\nParameter name: type")]
    public void GetMetadata_ForBindableObjectWithManualIdentity ()
    {
      ClassReflector classReflector = new ClassReflector (typeof (ClassWithManualIdentity), _businessObjectProvider);
      classReflector.GetMetadata ();
    }

    [Test]
    public void GetMetadata_FromCache ()
    {
      ClassReflector otherClassReflector = new ClassReflector (_type, _businessObjectProvider);
      Assert.That (otherClassReflector.GetMetadata(), Is.SameAs (_classReflector.GetMetadata()));
    }
  }
}