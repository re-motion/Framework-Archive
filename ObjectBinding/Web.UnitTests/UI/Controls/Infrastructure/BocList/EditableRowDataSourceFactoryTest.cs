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
public class ModifiableRowDataSourceFactoryTest
{
  // types

  // static members and constants

  // member fields

  private TypeWithString _value;
  private ModifiableRowDataSourceFactory _factory;

  // construction and disposing

  public ModifiableRowDataSourceFactoryTest ()
  {
  }

  // methods and properties

  [SetUp] 
  public virtual void SetUp()
  {
    _value = new TypeWithString ();

    _factory = new ModifiableRowDataSourceFactory ();
  }

  [Test]
  public void Create ()
  {
    IBusinessObjectReferenceDataSource dataSource = _factory.Create (_value);

    Assert.IsNotNull (dataSource);
    Assert.AreSame (_value, dataSource.BusinessObject);
  }
}

}
