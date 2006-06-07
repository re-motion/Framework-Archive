using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Xml;

using Rubicon.Utilities;

namespace Rubicon.Xml
{
  public abstract class SchemaBase
  {
    protected abstract string SchemaFile { get; }
    public abstract string SchemaUri { get; }

    /// <summary> Gets an <see cref="XmlReader"/> reader for the schema embedded in the assembly. </summary>
    /// <exception cref="ApplicationException"> Thrown if the schema file could not be loaded. </exception>
    public XmlReader GetSchemaReader ()
    {
      Type type = GetType ();
      Assembly assembly = type.Assembly;

      Stream schemaStream = assembly.GetManifestResourceStream (type, SchemaFile);
      if (schemaStream == null)
        throw new ApplicationException (string.Format ("Error loading schema resource '{0}' from assembly '{1}'.", SchemaFile, assembly.FullName));

      return new XmlTextReader (schemaStream);
    }
  }
}