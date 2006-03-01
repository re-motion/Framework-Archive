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

    public void Build (Type type)
    {
      BeginNamespace (type.Namespace);
      WriteEnum (type.Name);
      EndNamespace ();
      FinishFile ();
    }
  }

  public static void Build (string filename, Type type)
  {
    ArgumentUtility.CheckNotNullOrEmpty ("filename", filename);

    using (TextWriter writer = new StreamWriter (filename))
    {
      Build (writer, type);
    }
  }

  public static void Build (TextWriter writer, Type type)
  {
    ArgumentUtility.CheckNotNull ("writer", writer);
    ArgumentUtility.CheckNotNull ("type", type);

    Builder builder = new Builder (writer);
    builder.Build (type);
  }

  private EnumBuilder()
  {
  }
}
}
