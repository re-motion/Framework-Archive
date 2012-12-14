using System;

using Rubicon.Utilities;

namespace Rubicon.Data.DomainObjects.Persistence
{
public class RdbmsExpression
{
  // types

  // static members and constants

  private static string[] s_invalidCharacterSequences = new string[]
  {
    ";",
    "--",
    "\n",
    "\r",
    "/*",
    "*/"
  };

  // member fields

  private string _text;

  // construction and disposing

  public RdbmsExpression (string text)
  {
    ArgumentUtility.CheckNotNullOrEmpty ("text", text);

    _text = text;
  }

  // methods and properties

  public string Text
  {
    get { return _text; }
  }

  public void Check ()
  {
    foreach (string invalidCharacterSequence in s_invalidCharacterSequences)
    {
      if (_text.IndexOf (invalidCharacterSequence) >= 0)
      {
        throw CreateRdbmsExpressionSecurityException (
            "For security reasons the character sequence '{0}' must not be used in an RDBMS expression.", 
            invalidCharacterSequence);
      }
    }
  }

  private RdbmsExpressionSecurityException CreateRdbmsExpressionSecurityException (
      string message, 
      params object[] args)
  {
    return new RdbmsExpressionSecurityException (string.Format (message, args));
  }
}
}
