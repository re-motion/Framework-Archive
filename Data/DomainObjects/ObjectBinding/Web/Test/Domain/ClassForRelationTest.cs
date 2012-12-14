using System;

using Rubicon.Globalization;
using Rubicon.NullableValueTypes;

using Rubicon.Data.DomainObjects;
using Rubicon.Data.DomainObjects.ObjectBinding;

namespace Rubicon.Data.DomainObjects.ObjectBinding.Web.Test.Domain
{
[MultiLingualResources ("Rubicon.Data.DomainObjects.ObjectBinding.Web.Test.Globalization.ClassForRelationTest")]
public class ClassForRelationTest : BindableDomainObject
{
  // types

  // static members and constants
  
  public static new ClassForRelationTest GetObject (ObjectID id)
  {
    return (ClassForRelationTest) BindableDomainObject.GetObject (id);
  }

  // member fields

  // construction and disposing
  
  public ClassForRelationTest ()
  {
  }

  public ClassForRelationTest (ClientTransaction clientTransaction) : base (clientTransaction)
  {
  }

  protected ClassForRelationTest (DataContainer dataContainer) : base (dataContainer)
  {
  }

  // methods and properties

  public string Name
  {
    get { return DataContainer.GetString ("Name"); }
    set { DataContainer.SetValue ("Name", value); }
  } 

  public override string DisplayName
  {
    get { return Name; }
  }
}
}