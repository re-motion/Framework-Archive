using System;

using NUnit.Framework;

using Rubicon.ObjectBinding.Reflection;
using Rubicon.ObjectBinding.Web;
using Rubicon.ObjectBinding.Web.UI.Controls;
using Rubicon.ObjectBinding.Web.UI.Controls.Infrastructure.BocList;
using Rubicon.ObjectBinding.Web.UnitTests.Domain;
using Rubicon.Utilities;

namespace Rubicon.ObjectBinding.Web.UnitTests.UI.Controls.Infrastructure.BocList
{

[TestFixture]
public class ModifiableRowControlFactoryTest
{
  // types

  // static members and constants

  // member fields

  private ReflectionBusinessObjectClass _stringValueClass;
  private BusinessObjectPropertyPath _stringValuePropertyPath;
  private BocSimpleColumnDefinition _stringValueColumn;

  private ModifiableRowControlFactory _factory;

  // construction and disposing

  public ModifiableRowControlFactoryTest ()
  {
  }

  // methods and properties

  [SetUp] 
  public virtual void SetUp()
  {
    _stringValueClass = new ReflectionBusinessObjectClass (typeof (TypeWithString));

    _stringValuePropertyPath = BusinessObjectPropertyPath.Parse (_stringValueClass, "FirstValue");

    _stringValueColumn = new BocSimpleColumnDefinition ();
    _stringValueColumn.PropertyPath = _stringValuePropertyPath;

    _factory = new ModifiableRowControlFactory();
  }

  [Test]
  public void CreateWithStringProperty ()
  {
    IBusinessObjectBoundModifiableWebControl control = _factory.Create (_stringValueColumn, 0);

    Assert.IsNotNull (control);
    Assert.IsTrue (control is BocTextValue);
  }
}

}
