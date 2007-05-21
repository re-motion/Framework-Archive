using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Rubicon.Utilities;
using ReflectionUtility=Mixins.Utilities.ReflectionUtility;

namespace Mixins.Definitions.Building
{
  /// <summary>
  /// MemberChain takes an enumeration of members and orders them according to the hierarchy of their declaring types. This can be used
  /// to find out what members override or shadow other members of the same name and signature.
  /// </summary>
  /// <typeparam name="T">The member type to be chained, must be assignable from <see cref="MemberInfo"/>.</typeparam>
  public class MemberChain<T> where T : MemberInfo
  {
    private LinkedList<T> _chain = new LinkedList<T>();

    public MemberChain (IEnumerable<T> members)
    {
      ArgumentUtility.CheckNotNull ("members", members);

      foreach (T member in members)
        Insert (member);
      Assertion.DebugAssert (IsSorted());
    }

    private bool IsSorted()
    {
      LinkedListNode<T> current = _chain.First;
      while (current != null && current.Next != null)
      {
        if (!current.Value.DeclaringType.IsAssignableFrom (current.Next.Value.DeclaringType))
          return false;
        current = current.Next;
      }
      return true;
    }

    private void Insert (T member)
    {
      LinkedListNode<T> position = _chain.First;

      while (position != null && position.Value.DeclaringType.IsAssignableFrom (member.DeclaringType))
        position = position.Next;

      if (position != null)
        _chain.AddBefore (position, member);
      else
        _chain.AddLast (member);
    }

    public bool IsOverridden (T member)
    {
      ArgumentUtility.CheckNotNull ("member", member);
      LinkedListNode<T> respectiveNode = _chain.Find (member);
      if (respectiveNode == null)
      {
        string message = string.Format (
            "The chain of members does not contain member {0}.{1}.",
            member.DeclaringType.FullName,
            member.Name);
        throw new ArgumentException (message, "member");
      }
      else
        return IsOverridden (respectiveNode);
    }

    private bool IsOverridden (LinkedListNode<T> node)
    {
      Assertion.Assert (node != null);
      return node.Next != null && (ReflectionUtility.IsVirtualMember(node.Next.Value) && !ReflectionUtility.IsNewSlotMember (node.Next.Value));
    }

    public IEnumerable<T> GetSortedMembers()
    {
      return _chain;
    }

    public IEnumerable<T> GetNonOverriddenMembers()
    {
      LinkedListNode<T> current = _chain.First;
      while (current != null)
      {
        if (!IsOverridden (current))
          yield return current.Value;
        current = current.Next;
      }
    }
  }
}
