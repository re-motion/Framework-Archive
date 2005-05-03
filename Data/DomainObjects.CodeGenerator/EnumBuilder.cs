using System;

using Rubicon.Utilities;

namespace Rubicon.Data.DomainObjects.CodeGenerator
{
public class EnumBuilder : CodeFileBuilder
{
  // types

  // static members and constants

  // member fields
  private Type _type;

  // construction and disposing

  public EnumBuilder (string fileName, Type type) : base (fileName)
  {
    ArgumentUtility.CheckNotNull ("type", type);

    _type = type;
  }

  // methods and properties

  public override void Build()
  {
    OpenFile ();

    BeginNamespace (_type.Namespace);

    WriteEnum (_type.Name);

    EndNamespace ();

    CloseFile ();
  }

}
}
