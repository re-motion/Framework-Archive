using System;
using Rubicon.Utilities;

namespace Rubicon
{
  /// <summary>Provides a standard implementation of the double checked locking pattern.</summary>
  /// <typeparam name="T">The type encapsulated by the <see cref="DoubleCheckedLockingContainer{T}"/>.</typeparam>
  /// <remarks>Initialize the container during the construction of the parent object and assign the value using the <see cref="Value"/> property.</remarks>
  /// <threadsafety static="true" instance="true" />
  public class DoubleCheckedLockingContainer<T>
      where T : class
  {
    private T _value = null;
    private readonly Func<T> _defaultFactory;
    private readonly object _sync = new object();

    /// <summary>Initializes a new instance of the <see cref="DoubleCheckedLockingContainer{T}"/> type.</summary>
    /// <param name="defaultFactory">The delegate used to create the default value in case the value is <see langword="null" />.</param>
    public DoubleCheckedLockingContainer (Func<T> defaultFactory)
    {
      ArgumentUtility.CheckNotNull ("defaultFactory", defaultFactory);
      _defaultFactory = defaultFactory;
    }

    /// <summary>Gets or sets the object encapsulated by the <see cref="DoubleCheckedLockingContainer{T}"/>.</summary>
    /// <value>
    /// The object assigned via the set accessor<br />or,<br />
    /// if the value is null, the object created by the <b>defaultFactory</b> assigned during the initialization of the container.
    /// </value>
    public T Value
    {
      get
      {
        if (_value == null)
        {
          lock (_sync)
          {
            if (_value == null)
              _value = _defaultFactory();
          }
        }
        return _value;
      }
      set
      {
        lock (_sync)
        {
          _value = value ?? _defaultFactory();
        }
      }
    }
  }
}