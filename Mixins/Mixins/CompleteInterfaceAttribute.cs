using System;

namespace Mixins
{
  /// <summary>
  /// Indicates that an interface acts as a complete interface for a class accessed and instantiated via <see cref="TypeFactory"/> and
  /// <see cref="ObjectFactory"/>.
  /// </summary>
  /// <remarks>
  /// <para>
  /// A complete interface is an interface which is added to a mixin target type in order to provide an API incorporating both the members originally
  /// on the target class and those members introduced by the mixin. For example, if a target class provides the methods A and B and a mixin adds the
  /// methods C and D, users of the class can only ever use either A and B or C and D without casting. By implementing a complete interface that
  /// provides methods A, B, C, and D, users of the class can employ the full API in a simple way.
  /// </para>
  /// <para>
  /// All methods specified by the complete interface must either be implemented on the target type or introduced via a mixin.
  /// </para>
  /// <para>
  /// This interface can be applied multiple times if an interface is to be a complete interface for multiple target types, and it is not inherited
  /// (i.e. an interface inheriting from a complete interface does not automatically constitute a complete interface for the same type).
  /// </para>
  /// </remarks>
  /// <example>
  /// <code>
  /// public class MyMixinTarget
  /// {
  ///   public void A() { Console.WriteLine ("A"); }
  ///   public void B() { Console.WriteLine ("B"); }
  /// }
  /// 
  /// [Extends (typeof (MyMixinTarget))]
  /// public class MyMixin : Mixin&lt;MyMixinTarget&gt;
  /// {
  ///   public void C() { Console.WriteLine ("D"); }
  ///   public void D() { Console.WriteLine ("D"); }
  /// }
  /// 
  /// [CompleteInterface (typeof (MyMixinTarget))]
  /// public interface ICMyMixinTargetMyMixin
  /// {
  ///   void A();
  ///   void B();
  ///   void C();
  ///   void D();
  /// }
  /// </code>
  /// Complete interfaces can also be defined by inheriting from existing interfaces rather than spelling all the methods out explicitly.
  /// </example>
  [AttributeUsage (AttributeTargets.Interface, AllowMultiple = true, Inherited = false)]
  public class CompleteInterfaceAttribute : Attribute
  {
    private Type _targetType;

    /// <summary>
    /// Initializes a new instance of the <see cref="CompleteInterfaceAttribute"/> class.
    /// </summary>
    /// <param name="targetType">Target type for which this interface constitutes a complete interface.</param>
    public CompleteInterfaceAttribute (Type targetType)
    {
      _targetType = targetType;
    }

    /// <summary>
    /// Gets the target tyoe for which this interface constitutes a complete interface.
    /// </summary>
    /// <value>The target type of this complete interface.</value>
    public Type TargetType
    {
      get { return _targetType; }
    }
  }
}
