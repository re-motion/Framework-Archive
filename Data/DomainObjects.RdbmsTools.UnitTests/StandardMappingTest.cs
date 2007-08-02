using System;
using NUnit.Framework;
using Rubicon.Data.DomainObjects.Configuration;
using Rubicon.Data.DomainObjects.Mapping;
using Rubicon.Data.DomainObjects.Persistence.Configuration;
using Rubicon.Data.DomainObjects.RdbmsTools.UnitTests.TestDomain;

namespace Rubicon.Data.DomainObjects.RdbmsTools.UnitTests
{
  public class StandardMappingTest
  {
    private ClassDefinition _orderItemClass;
    private ClassDefinition _orderClass;
    private ClassDefinition _companyClass;
    private ClassDefinition _customerClass;
    private ClassDefinition _partnerClass;
    private ClassDefinition _abstractWithoutConcreteClass;
    private ClassDefinition _concreteClass;
    private ClassDefinition _derivedClass;
    private ClassDefinition _secondDerivedClass;
    private ClassDefinition _derivedOfDerivedClass;
    private ClassDefinition _ceoClass;
    private ClassDefinition _classWithRelations;
    private ClassDefinition _classWithoutProperties;

    [TestFixtureSetUp]
    public virtual void TextFixtureSetUp ()
    {
    }

    [SetUp]
    public virtual void SetUp()
    {
      _orderItemClass = MappingConfiguration.ClassDefinitions.GetMandatory (typeof (OrderItem));
      _orderClass = MappingConfiguration.ClassDefinitions.GetMandatory (typeof (Order));
      _companyClass = MappingConfiguration.ClassDefinitions.GetMandatory (typeof (Company));
      _customerClass = MappingConfiguration.ClassDefinitions.GetMandatory (typeof (Customer));
      _partnerClass = MappingConfiguration.ClassDefinitions.GetMandatory (typeof (Partner));
      _abstractWithoutConcreteClass = MappingConfiguration.ClassDefinitions.GetMandatory (typeof (AbstractWithoutConcreteClass));
      _concreteClass = MappingConfiguration.ClassDefinitions.GetMandatory (typeof (ConcreteClass));
      _derivedClass = MappingConfiguration.ClassDefinitions.GetMandatory (typeof (DerivedClass));
      _secondDerivedClass = MappingConfiguration.ClassDefinitions.GetMandatory (typeof (SecondDerivedClass));
      _derivedOfDerivedClass = MappingConfiguration.ClassDefinitions.GetMandatory (typeof (DerivedOfDerivedClass));
      _ceoClass = MappingConfiguration.ClassDefinitions.GetMandatory (typeof (Ceo));
      _classWithRelations = MappingConfiguration.ClassDefinitions.GetMandatory (typeof (ClassWithRelations));
      _classWithoutProperties = MappingConfiguration.ClassDefinitions.GetMandatory (typeof (ClassWithoutProperties));
    }

    [TearDown]
    public virtual void TearDown()
    {
    }

    protected PersistenceConfiguration StorageConfiguration
    {
      get { return DomainObjectsConfiguration.Current.Storage; }
    }

    protected MappingConfiguration MappingConfiguration
    {
      get { return MappingConfiguration.Current; }
    }

    protected ClassDefinition OrderItemClass
    {
      get { return _orderItemClass; }
    }

    protected ClassDefinition OrderClass
    {
      get { return _orderClass; }
    }

    protected ClassDefinition CompanyClass
    {
      get { return _companyClass; }
    }

    protected ClassDefinition CustomerClass
    {
      get { return _customerClass; }
    }

    protected ClassDefinition PartnerClass
    {
      get { return _partnerClass; }
    }

    public ClassDefinition AbstractWithoutConcreteClass
    {
      get { return _abstractWithoutConcreteClass; }
    }

    protected ClassDefinition ConcreteClass
    {
      get { return _concreteClass; }
    }

    public ClassDefinition DerivedClass
    {
      get { return _derivedClass; }
    }

    protected ClassDefinition SecondDerivedClass
    {
      get { return _secondDerivedClass; }
    }

    protected ClassDefinition DerivedOfDerivedClass
    {
      get { return _derivedOfDerivedClass; }
    }

    protected ClassDefinition CeoClass
    {
      get { return _ceoClass; }
    }

    protected ClassDefinition ClassWithRelations
    {
      get { return _classWithRelations; }
    }

    protected ClassDefinition ClassWithoutProperties
    {
      get { return _classWithoutProperties; }
    }
  }
}