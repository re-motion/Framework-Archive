using System;
using System.Reflection;
using Rubicon.Utilities;

namespace Rubicon.Mixins
{
  /// <summary>
  /// When applied to a mixin, specifies a class whose custom attributes should be added to the mixin's target class. This is useful when a mixin
  /// should add certain attributes without itself exposing those attributes.
  /// </summary>
  [AttributeUsage (AttributeTargets.Class | AttributeTargets.Method | AttributeTargets.Property, AllowMultiple = true, Inherited = true)]
  public class CopyCustomAttributesAttribute : Attribute
  {
    private readonly Type _attributeSourceType;
    private readonly string _attributeSourceMemberName;

    public CopyCustomAttributesAttribute (Type attributeSourceType)
    {
      ArgumentUtility.CheckNotNull ("attributeSource", attributeSourceType);
      _attributeSourceType = attributeSourceType;
      _attributeSourceMemberName = null;
    }

    public CopyCustomAttributesAttribute (Type attributeSourceType, string attributeSourceMemberName)
    {
      ArgumentUtility.CheckNotNull ("attributeSource", attributeSourceType);
      ArgumentUtility.CheckNotNullOrEmpty ("attributeSourceMemberName", attributeSourceMemberName);

      _attributeSourceType = attributeSourceType;
      _attributeSourceMemberName = attributeSourceMemberName;
    }

    public Type AttributeSourceType
    {
      get { return _attributeSourceType; }
    }

    public string AttributeSourceMemberName
    {
      get { return _attributeSourceMemberName; }
    }

    public object AttributeSourceName
    {
      get
      {
        return AttributeSourceMemberName != null ? AttributeSourceType.FullName + "." + AttributeSourceMemberName : AttributeSourceType.FullName;
      }
    }

    public MemberInfo GetAttributeSource (MemberTypes memberType)
    {
      if (AttributeSourceMemberName == null)
        return AttributeSourceType;
      else
      {
        MemberInfo[] members =
            AttributeSourceType.GetMember (AttributeSourceMemberName, memberType, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

        if (members.Length == 0)
          return null;
        else if (members.Length == 1)
          return members[0];
        else
        {
          throw new AmbiguousMatchException (
              string.Format (
                  "The source member string {0} matches several members on type {1}.",
                  AttributeSourceMemberName,
                  AttributeSourceType.FullName));
        }
      }
    }
  }
}