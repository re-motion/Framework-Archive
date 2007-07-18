using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Text.RegularExpressions;
using R = System.Text.RegularExpressions;

namespace Rubicon.Utilities
{
  /// <summary>
  /// Utility methods for handling types.
  /// </summary>
  public static class TypeUtility
  {
    /// <summary>
    ///   Converts abbreviated qualified type names into standard qualified type names.
    /// </summary>
    /// <remarks>
    ///   Abbreviated type names use the format <c>assemblyname::subnamespace.type</c>. For instance, the
    ///   abbreviated type name <c>"Rubicon.Web::Utilities.ControlHelper"</c> would result in the standard
    ///   type name <c>"Rubicon.Web.Utilities.ControlHelper, Rubicon.Web"</c>.
    /// </remarks>
    /// <param name="abbreviatedTypeName"> A standard or abbreviated type name. </param>
    /// <returns> A standard type name as expected by <see cref="Type.GetType"/>. </returns>
    public static string ParseAbbreviatedTypeName (string abbreviatedTypeName)
    {
      if (abbreviatedTypeName == null)
        return null;

      string typeNamePattern =                // <asm>::<type>
            @"(?<asm>[^\[\]\,]+)"             //    <asm> is the assembly part of the type name (before ::)
          + @"::" 
          + @"(?<type>[^\[\]\,]+)";           //    <type> is the partially qualified type name (after ::)

      string bracketPattern =                 // [...] (an optional pair of matching square brackets and anything in between)
            @"(?<br> \[          "            //    see "Mastering Regular Expressions" (O'Reilly) for how the construct "balancing group definition" 
          + @"  (                "            //    is used to match brackets: http://www.oreilly.com/catalog/regex2/chapter/ch09.pdf
          + @"      [^\[\]]      "
          + @"    |              "
          + @"      \[ (?<d>)    "            //    increment nesting counter <d>
          + @"    |              "
          + @"      \] (?<-d>)   "            //    decrement <d>
          + @"  )*               "
          + @"  (?(d)(?!))       "            //    ensure <d> is 0 before considering next match
          + @"\] )?              ";

      string strongNameParts =                // comma-separated list of name=value pairs
            @"(?<sn> (, \s* \w+ = [^,]+ )* )";

      string typePattern =                    // <asm>::<type>[...] (square brackets are optional)
            typeNamePattern 
          + bracketPattern;

      string openUnqualifiedPattern =         // requires the pattern to be preceded by [ or ,
            @"(?<= [\[,] )";   
      string closeUnqualifiedPattern =        // requires the pattern to be followed by ] or ,
            @"(?= [\],] )";

      string enclosedTypePattern =            // type within argument list
            openUnqualifiedPattern
          + typePattern
          + closeUnqualifiedPattern;

      string qualifiedTypePattern =           // <asm>::<type>[...], name=val, name=val ... (square brackets are optional)
            typePattern 
          + strongNameParts;

      string openQualifiedPattern =           // requires the pattern to be preceded by [[ or ,[
            @"(?<= [\[,] \[)";   
      string closeQualifiedPattern =          // requires the pattern to be followed by ]] or ],
            @"(?= \] [\],] )";

      string enclosedQualifiedTypePattern =   // qualified type within argument list
            openQualifiedPattern 
          + qualifiedTypePattern
          + closeQualifiedPattern;

      RegexOptions options = RegexOptions.ExplicitCapture | RegexOptions.IgnorePatternWhitespace; // | RegexOptions.Compiled
      string result = abbreviatedTypeName;
      string replace =          @"${asm}.${type}${br}, ${asm}";
      result = ReplaceRecursive (result, enclosedQualifiedTypePattern,  replace + "${sn}",   options);
      result = ReplaceRecursive (result, enclosedTypePattern,           "[" + replace + "]", options);
      result = Regex.Replace    (result, typePattern,                   replace,             options);
      return result;
    }

    private static string ReplaceRecursive (string input, string pattern, string replacement, RegexOptions options)
    {
      Regex regex = new Regex (pattern, options);
      string result = regex.Replace (input, replacement);
      while (result != input)
      {
        input = result;
        result = regex.Replace (input, replacement);
      }
      return result;
    }

    /// <summary>
    ///   Loads a type, optionally using an abbreviated type name as defined in <see cref="ParseAbbreviatedTypeName"/>.
    /// </summary>
    public static Type GetType (string abbreviatedTypeName)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("abbreviatedTypeName", abbreviatedTypeName);
      return Type.GetType (ParseAbbreviatedTypeName (abbreviatedTypeName));
    }

    /// <summary>
    ///   Loads a type, optionally using an abbreviated type name as defined in <see cref="ParseAbbreviatedTypeName"/>.
    /// </summary>
    public static Type GetType (string abbreviatedTypeName, bool throwOnError)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("abbreviatedTypeName", abbreviatedTypeName);
      return Type.GetType (ParseAbbreviatedTypeName (abbreviatedTypeName), throwOnError);
    }

    /// <summary>
    ///   Loads a type, optionally using an abbreviated type name as defined in <see cref="ParseAbbreviatedTypeName"/>.
    /// </summary>
    public static Type GetType (string abbreviatedTypeName, bool throwOnError, bool ignoreCase)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("abbreviatedTypeName", abbreviatedTypeName);
      return Type.GetType (ParseAbbreviatedTypeName (abbreviatedTypeName), throwOnError, ignoreCase);
    }

    public static string GetPartialAssemblyQualifiedName (Type type)
    {
      ArgumentUtility.CheckNotNull ("type", type);

      return type.FullName + ", " + type.Assembly.GetName().Name;
    }

    public static Type GetDesignModeType (string abbreviatedTypeName, ISite site, bool throwOnError)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("abbreviatedTypeName", abbreviatedTypeName);
      ArgumentUtility.CheckNotNull ("site", site);

      IDesignerHost designerHost = (IDesignerHost) site.GetService (typeof (IDesignerHost));
      Assertion.Assert (designerHost != null, "No IDesignerHost found.");
      Type type = designerHost.GetType (TypeUtility.ParseAbbreviatedTypeName (abbreviatedTypeName));
      if (type == null && throwOnError)
        throw new TypeLoadException (string.Format ("Could not load type '{0}'.", TypeUtility.ParseAbbreviatedTypeName (abbreviatedTypeName)));
      return type;
    }
  }
}
