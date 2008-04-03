using System;
using System.IO;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Legacy.CodeGenerator
{
  public class EnumBuilder : CodeFileBuilder
  {
    // types

    // static members and constants

    public static void Build (string filename, TypeName typeName, bool enumDescriptionResourceAttribute)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("filename", filename);
      ArgumentUtility.CheckNotNull ("typeName", typeName);

      using (TextWriter writer = new StreamWriter (filename))
      {
        Build (writer, typeName, enumDescriptionResourceAttribute);
      }
    }

    public static void Build (TextWriter writer, TypeName typeName, bool enumDescriptionResourceAttribute)
    {
      ArgumentUtility.CheckNotNull ("writer", writer);
      ArgumentUtility.CheckNotNull ("typeName", typeName);

      EnumBuilder builder = new EnumBuilder (writer);
      builder.Build (typeName, enumDescriptionResourceAttribute);
    }

    #region Templates

    private static readonly string s_enum =
        "  public enum %enumname%\r\n"
        + "  {\r\n"
        + "    DummyEntry = 0\r\n"
        + "  }\r\n";

    #endregion

    // member fields

    // construction and disposing

    private EnumBuilder (TextWriter writer)
      : base (writer)
    {
    }

    // methods and properties

    private void Build (TypeName typeName, bool enumDescriptionResourceAttribute)
    {
      BeginNamespace (typeName.Namespace);

      if (enumDescriptionResourceAttribute)
        WriteEnumDescriptionResourceAttribute (typeName);

      WriteEnum (typeName.Name);
      EndNamespace ();
      FinishFile ();
    }

    private void WriteEnum (string enumName)
    {
      Write (ReplaceTag (s_enum, s_enumTag, enumName));
    }
  }
}
