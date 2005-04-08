using System;
using System.Text;

namespace Rubicon.Utilities
{

/// <summary>
/// Builds a string adding separators between appended strings.
/// </summary>
public class SeparatedStringBuilder
{
  private string _separator;
  private StringBuilder _stringBuilder;

	public SeparatedStringBuilder (string separator, int capacity)
	{
    _stringBuilder = new StringBuilder (capacity);
    _separator = separator;
	}

  public SeparatedStringBuilder (string separator)
  {
    _stringBuilder = new StringBuilder ();
    _separator = separator;
  }

  public void Append (string s)
  {
    AppendSeparator();
    _stringBuilder.Append (s);
  }

  public void AppendFormat (string format, params object[] args)
  {
    AppendSeparator();
    _stringBuilder.AppendFormat (format, args);
  }

  private void AppendSeparator()
  {
    if (_stringBuilder.Length > 0)
      _stringBuilder.Append (_separator);
  }

  public override string ToString()
  {
    return _stringBuilder.ToString();
  }

}

}
