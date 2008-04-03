using System;
using System.Collections.Generic;
using System.Text;
using Remotion.Utilities;
using System.Reflection;

namespace Remotion.Data.DomainObjects.Legacy.CodeGenerator
{
  //TODO: add support for closed generics
  public class TypeName : IComparable
  {
    // types

    // static members and constants

    // member fields

    private string _assemblyQualifiedName;
    private string _fullName;
    private string _name;
    private string _namespace;
    private AssemblyName _assemblyName;
    private TypeName _declaringTypeName;

    // construction and disposing

    public TypeName (string fullTypeName, string assemblyName) : this (Assembly.CreateQualifiedName (assemblyName, fullTypeName))
    {
    }

    public TypeName (string assemblyQualifiedName)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("assemblyQualifiedName", assemblyQualifiedName);

      CheckAssemblyQualifiedName (assemblyQualifiedName);

      _assemblyQualifiedName = assemblyQualifiedName;
      _fullName = GetFullName (assemblyQualifiedName);
      _assemblyName = CreateAssemblyName (assemblyQualifiedName);
      _declaringTypeName = GetDeclaringTypeName (_fullName, _assemblyName);
      _name = GetName (_fullName, IsNested);
      _namespace = _fullName.Substring (0, _fullName.LastIndexOf ('.'));
    }

    private void CheckAssemblyQualifiedName (string assemblyQualifiedName)
    {
      //TODO ES: validate no invalid characters are contained 
      //TODO ES: check that valid TypeName is present before a '+' sign
      if (assemblyQualifiedName.IndexOf (',') == -1)
      {
        throw CreateArgumentException (
          "assemblyQualifiedName",
          "The assemblyQualifiedName '{0}' does not contain an assembly name.",
          assemblyQualifiedName);
      }
    }

    private string GetFullName (string assemblyQualifiedName)
    {
      int indexOfFirstKomma = assemblyQualifiedName.IndexOf (',');
      string fullName = assemblyQualifiedName.Substring (0, indexOfFirstKomma).Trim ();

      if (fullName == string.Empty)
        throw CreateArgumentException ("assemblyQualifiedName", "The assemblyQualifiedName '{0}' does contains an empty type name.", assemblyQualifiedName);
      return fullName;
    }

    private string GetName (string fullName, bool isNested)
    {
      if (isNested)
        return fullName.Substring (fullName.LastIndexOf ('+') + 1);
      else
        return fullName.Substring (fullName.LastIndexOf ('.') + 1);
    }

    private AssemblyName CreateAssemblyName (string assemblyQualifiedName)
    {
      int indexOfFirstKomma = assemblyQualifiedName.IndexOf (',');
      string assemblyFullName = assemblyQualifiedName.Substring (indexOfFirstKomma + 1).Trim ();

      if (assemblyFullName == string.Empty)
      {
        throw CreateArgumentException (
          "assemblyQualifiedName",
          "The assemblyQualifiedName '{0}' does contains an empty assembly name.",
          assemblyQualifiedName);
      }

      return new AssemblyName (assemblyFullName);
    }

    private TypeName GetDeclaringTypeName (string fullName, AssemblyName assemblyName)
    {
      int indexOfPlusSign = fullName.IndexOf ("+");

      if (indexOfPlusSign == -1)
        return null;

      return new TypeName (fullName.Remove (indexOfPlusSign), assemblyName.FullName);
    }

    // methods and properties

    public string AssemblyQualifiedName
    {
      get { return _assemblyQualifiedName; }
    }

    public string FullName
    {
      get { return _fullName; }
    }

    public string Name
    {
      get { return _name; }
    }

    public string Namespace
    {
      get { return _namespace; }
    }

    public AssemblyName AssemblyName
    {
      get { return _assemblyName; }
    }

    public bool IsNested
    {
      get { return _declaringTypeName != null; }
    }

    public TypeName DeclaringTypeName
    {
      get { return _declaringTypeName; }
    }

    private ArgumentException CreateArgumentException (string parameterName, string format, params string[] args)
    {
      return new ArgumentException (string.Format (format, args), parameterName);
    }

    #region IComparable Members

    public int CompareTo (object value)
    {
      if (value == null)
        return 1;

      if (!(value is TypeName))
      {
        throw CreateArgumentException (
            "value",
            "Argument 'value' must be of type '{0}' but was of type '{1}'.",
            typeof (TypeName).FullName,
            value.GetType ().FullName);
      }

      TypeName otherTypeName = (TypeName) value;

      int fullNameCompareResult = FullName.CompareTo (otherTypeName.FullName);
      if (fullNameCompareResult != 0)
        return fullNameCompareResult;

      return AssemblyName.FullName.CompareTo (otherTypeName.AssemblyName.FullName);
    }

    public int CompareTo (TypeName value)
    {
      return CompareTo ((object) value);
    }

    #endregion
  }
}
