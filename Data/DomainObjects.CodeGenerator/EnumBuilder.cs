using System;
using System.IO;
using Rubicon.Utilities;

namespace Rubicon.Data.DomainObjects.CodeGenerator
{

public class EnumBuilder
{
  private class Builder: CodeFileBuilder
  {
    public Builder (TextWriter writer) : base (writer)
    {
    }

    public void Build (Type type, bool enumDescriptionResourceAttribute)
    {
      BeginNamespace (type.Namespace);

      if (enumDescriptionResourceAttribute)
        WriteEnumDescriptionResourceAttribute (type);

      WriteEnum (type.Name);
      EndNamespace ();
      FinishFile ();
    }
  }

  public static void Build (string filename, Type type, bool enumDescriptionResourceAttribute)
  {
    ArgumentUtility.CheckNotNullOrEmpty ("filename", filename);

    using (TextWriter writer = new StreamWriter (filename))
    {
      Build (writer, type, enumDescriptionResourceAttribute);
    }
  }

  public static void Build (TextWriter writer, Type type, bool enumDescriptionResourceAttribute)
  {
    ArgumentUtility.CheckNotNull ("writer", writer);
    ArgumentUtility.CheckNotNull ("type", type);

    Builder builder = new Builder (writer);
    builder.Build (type, enumDescriptionResourceAttribute);
  }

  private EnumBuilder()
  {
  }
}
}
