using System;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Rhino.Mocks;
using Rubicon.Data.DomainObjects.UnitTests.ObjectBinding.TestDomain;
using Rubicon.Mixins;
using Rubicon.ObjectBinding;
using Rubicon.ObjectBinding.BindableObject;

namespace Rubicon.Data.DomainObjects.UnitTests.ObjectBinding.BindableDomainObjectMixinTest
{
  [TestFixture]
  public class GetPropertyString : ObjectBindingBaseTest
  {
    private IBusinessObject _businessObject;
    private MockRepository _mockRepository;
    private IBusinessObjectStringFormatterService _mockStringFormatterService;
    private IBusinessObjectProperty _property;

    public override void SetUp ()
    {
      base.SetUp ();

      _mockRepository = new MockRepository();
      _mockStringFormatterService = _mockRepository.CreateMock<IBusinessObjectStringFormatterService>();
      BindableObjectProvider provider = new BindableObjectProvider();
      provider.AddService (typeof (IBusinessObjectStringFormatterService), _mockStringFormatterService);
      BindableObjectProvider.SetCurrent (provider);
      
      _businessObject = (IBusinessObject) BindableDomainObject.NewObject();

      _property = _businessObject.BusinessObjectClass.GetPropertyDefinition ("Name");
      Assert.That (
          _property, Is.Not.Null, "Property 'Name' was not found on BusinessObjectClass '{0}'", _businessObject.BusinessObjectClass.Identifier);

      BindableObjectProvider.SetCurrent (new BindableObjectProvider());
    }

    [Test]
    public void FromProperty ()
    {
      Expect.Call (_mockStringFormatterService.GetPropertyString (_businessObject, _property, "TheFormatString")).Return ("TheStringValue");
      _mockRepository.ReplayAll();

      string actual = _businessObject.GetPropertyString (_property, "TheFormatString");
      
      _mockRepository.VerifyAll();
      Assert.That (actual, Is.EqualTo ("TheStringValue"));
    }

    [Test]
    public void FromIdentifier ()
    {
      Expect.Call (_mockStringFormatterService.GetPropertyString (_businessObject, _property, null)).Return ("TheStringValue");
      _mockRepository.ReplayAll ();

      string actual = _businessObject.GetPropertyString ("Name");

      _mockRepository.VerifyAll ();
      Assert.That (actual, Is.EqualTo ("TheStringValue"));
    }
  }
}