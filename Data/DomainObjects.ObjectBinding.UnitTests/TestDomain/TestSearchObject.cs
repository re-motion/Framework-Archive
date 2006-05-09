using System;
using System.Text;
using System.ComponentModel;

using Rubicon.Data.DomainObjects.Queries;
using Rubicon.Data.DomainObjects.Queries.Configuration;
using Rubicon.Data.DomainObjects.ObjectBinding;

namespace Rubicon.Data.DomainObjects.ObjectBinding.UnitTests.TestDomain
{
[Serializable]
public class TestSearchObject : BindableSearchObject
{
  private string _stringProperty;
  private string _readOnlyStringProperty = "test";

  public string StringProperty
  {
    get { return _stringProperty; }
    set { _stringProperty = value; }
  }

  public string ReadOnlyStringProperty
  {
    get { return _readOnlyStringProperty; }
  }

  public override IQuery CreateQuery()
  {
    throw new NotImplementedException ();
  }
}
}
