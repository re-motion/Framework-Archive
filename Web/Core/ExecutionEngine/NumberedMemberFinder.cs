using System;
using System.Reflection;

namespace Rubicon.Web.ExecutionEngine
{

/// <summary>
///   Finds and orders type members using a prefix/number pattern.
/// </summary>
/// <remarks>
///   This class is used by WxeStep subclasses that use numbered elements, like Step1, Step2, ... or Catch1, Catch2 etc.
///   Underlines may be appended to separate member names from class names in case the class itself is called Step&lt;n;gt;
/// </remarks>
internal class NumberedMemberFinder
{
  /// <summary>
  ///   Returnes an orderd array of member infos.
  /// </summary>
  public static MemberInfo[] FindMembers (Type type, string prefix, MemberTypes memberType, BindingFlags bindingFlags)
  {
    NumberedMemberFinder finder = new NumberedMemberFinder (prefix);
    MemberInfo[] members = type.FindMembers (memberType, bindingFlags, new MemberFilter (finder.PrefixMemberFilter), null);

    int[] numbers = new int[members.Length];
    for (int i = 0; i < members.Length; ++i)
      numbers[i] = finder.GetStepNumber (members[i].Name);
    Array.Sort (numbers, members);

    return members;
  }

  private string _prefix;

	private NumberedMemberFinder (string prefix)
	{
    _prefix = prefix;
	}

  private bool PrefixMemberFilter (MemberInfo member, object param)
  {
    return GetStepNumber (member.Name) != -1;
  }

  private int GetStepNumber (string memberName)
  {
    if (! memberName.StartsWith (_prefix))
      return -1;
    string numStr = memberName.TrimEnd('_').Substring (_prefix.Length);
    if (numStr.Length == 0)
      return -1;
    double num;
    if (! double.TryParse (numStr, System.Globalization.NumberStyles.Integer, null, out num))
      return -1;
    return (int) num;
  }
}

}
