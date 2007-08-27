//------------------------------------------------------------------------------
// This code was generated by a tool.
//
// Changes to this file may cause incorrect behavior and will be lost if 
// the code is regenerated.
//
//------------------------------------------------------------------------------
using System;
using Rubicon.Utilities;
using System.Reflection;

namespace Rubicon.Reflection
{
  /// <summary>
  /// This interface allows invokers with fixed arguments to be returned without references to their generic argument types. 
  /// </summary>
  /// <remarks>
  /// <p>Note that casting a struct like <see cref="FuncInvoker{TResult}"/> to an interface is a boxing operation, thus creating an object on the
  /// heap and garbage collecting it later. For very performance-critical scenarios, it be better to avoid this and accept the references to 
  /// the invoker's generic argument types.</p>
  /// <p>It is recommended to wrap this interface within a <see cref="FuncInvokerWrapper{TResult}"/>, because returning an interface could lead to 
  /// ambigous castings if the final call to <see cref="With()"/> is missing, while using structs will usually lead to a compile-time error as 
  /// expected.</p>
  /// </remarks>
  /// <typeparam name="TResult"> Return type of the method that will be invoked. </typeparam>
  public partial interface IFuncInvoker<TResult>
  {


    TResult With ();

    TResult With<A1, A2> (A1 a1, A2 a2);

    TResult With<A1, A2, A3> (A1 a1, A2 a2, A3 a3);

    TResult With<A1, A2, A3, A4> (A1 a1, A2 a2, A3 a3, A4 a4);

    TResult With<A1, A2, A3, A4, A5> (A1 a1, A2 a2, A3 a3, A4 a4, A5 a5);

    TResult With<A1, A2, A3, A4, A5, A6> (A1 a1, A2 a2, A3 a3, A4 a4, A5 a5, A6 a6);

    TResult With<A1, A2, A3, A4, A5, A6, A7> (A1 a1, A2 a2, A3 a3, A4 a4, A5 a5, A6 a6, A7 a7);
  }

  /// <summary>
  /// Used to wrap an <see cref="IFuncInvoker{TResult}"/> object rather than returning it directly.
  /// </summary>
  /// <typeparam name="TResult"> Return type of the method that will be invoked. </typeparam>
  public partial struct FuncInvokerWrapper<TResult> : IFuncInvoker<TResult>
  {


    public TResult With ()
    {
      return PerformAfterAction (_invoker.With ());
    }


    public TResult With<A1, A2> (A1 a1, A2 a2)
    {
      return PerformAfterAction (_invoker.With (a1, a2));
    }


    public TResult With<A1, A2, A3> (A1 a1, A2 a2, A3 a3)
    {
      return PerformAfterAction (_invoker.With (a1, a2, a3));
    }


    public TResult With<A1, A2, A3, A4> (A1 a1, A2 a2, A3 a3, A4 a4)
    {
      return PerformAfterAction (_invoker.With (a1, a2, a3, a4));
    }


    public TResult With<A1, A2, A3, A4, A5> (A1 a1, A2 a2, A3 a3, A4 a4, A5 a5)
    {
      return PerformAfterAction (_invoker.With (a1, a2, a3, a4, a5));
    }


    public TResult With<A1, A2, A3, A4, A5, A6> (A1 a1, A2 a2, A3 a3, A4 a4, A5 a5, A6 a6)
    {
      return PerformAfterAction (_invoker.With (a1, a2, a3, a4, a5, a6));
    }


    public TResult With<A1, A2, A3, A4, A5, A6, A7> (A1 a1, A2 a2, A3 a3, A4 a4, A5 a5, A6 a6, A7 a7)
    {
      return PerformAfterAction (_invoker.With (a1, a2, a3, a4, a5, a6, a7));
    }

  }


  public partial struct FuncInvoker<TResult> : IFuncInvoker<TResult>
  {
    private DelegateSelector _delegateSelector;


    private const int c_argCount = 0;

    public FuncInvoker (DelegateSelector delegateSelector)
    {
      _delegateSelector = delegateSelector;
    }

#pragma warning disable 162 // disable unreachable code warning. 
    private Type[] GetValueTypes (Type[] valueTypes)
    {
      if (c_argCount == 0)
        return valueTypes;
      Type[] fixedArgTypes = new Type[] {  };
      return ArrayUtility.Combine (fixedArgTypes, valueTypes);
    }

    private object[] GetValues (object[] values)
    {
      if (c_argCount == 0)
        return values;
      object[] fixedArgs = new object[] {  };
      return ArrayUtility.Combine (fixedArgs, values);
    }
#pragma warning restore 162

    public TResult Invoke (Type[] valueTypes, object[] values)
    {
      InvokerUtility.CheckInvokeArguments (valueTypes, values);
      return (TResult) GetDelegate (GetValueTypes (valueTypes)).DynamicInvoke (GetValues (values));
    }

    public TResult Invoke (object[] values)
    {
      Type[] valueTypes = InvokerUtility.GetValueTypes (values);
      return (TResult) GetDelegate (GetValueTypes (valueTypes)).DynamicInvoke (GetValues (values));
    }

    public Delegate GetDelegate (params Type[] parameterTypes)
    {
      return GetDelegate (FuncDelegates.MakeClosedType (typeof (TResult), parameterTypes));
    }

    public TDelegate GetDelegate<TDelegate> ()
    {
      return (TDelegate) (object) GetDelegate (typeof (TDelegate));
    }

    public Delegate GetDelegate (Type delegateType)
    {
      return _delegateSelector (delegateType);
    }

    public TResult With ()
    {
      return GetDelegateWith () ();
    }

    public Func<TResult> GetDelegateWith ()
    {
      return GetDelegate<Func<TResult>> ();
    }
  }

  public partial struct FuncInvoker<TFixedArg1, TFixedArg2, TResult> : IFuncInvoker<TResult>
  {
    private DelegateSelector _delegateSelector;

    private TFixedArg1 _fixedArg1;
    private TFixedArg2 _fixedArg2;

    private const int c_argCount = 2;

    public FuncInvoker (DelegateSelector delegateSelector, TFixedArg1 fixedArg1, TFixedArg2 fixedArg2)
    {
      _delegateSelector = delegateSelector;
      _fixedArg1 = fixedArg1;
      _fixedArg2 = fixedArg2;
    }

#pragma warning disable 162 // disable unreachable code warning. 
    private Type[] GetValueTypes (Type[] valueTypes)
    {
      if (c_argCount == 0)
        return valueTypes;
      Type[] fixedArgTypes = new Type[] { typeof (TFixedArg1), typeof (TFixedArg2) };
      return ArrayUtility.Combine (fixedArgTypes, valueTypes);
    }

    private object[] GetValues (object[] values)
    {
      if (c_argCount == 0)
        return values;
      object[] fixedArgs = new object[] { _fixedArg1, _fixedArg2 };
      return ArrayUtility.Combine (fixedArgs, values);
    }
#pragma warning restore 162

    public TResult Invoke (Type[] valueTypes, object[] values)
    {
      InvokerUtility.CheckInvokeArguments (valueTypes, values);
      return (TResult) GetDelegate (GetValueTypes (valueTypes)).DynamicInvoke (GetValues (values));
    }

    public TResult Invoke (object[] values)
    {
      Type[] valueTypes = InvokerUtility.GetValueTypes (values);
      return (TResult) GetDelegate (GetValueTypes (valueTypes)).DynamicInvoke (GetValues (values));
    }

    public Delegate GetDelegate (params Type[] parameterTypes)
    {
      return GetDelegate (FuncDelegates.MakeClosedType (typeof (TResult), parameterTypes));
    }

    public TDelegate GetDelegate<TDelegate> ()
    {
      return (TDelegate) (object) GetDelegate (typeof (TDelegate));
    }

    public Delegate GetDelegate (Type delegateType)
    {
      return _delegateSelector (delegateType);
    }

    public TResult With ()
    {
      return GetDelegateWith () (_fixedArg1, _fixedArg2);
    }

    public Func<TFixedArg1, TFixedArg2, TResult> GetDelegateWith ()
    {
      return GetDelegate<Func<TFixedArg1, TFixedArg2, TResult>> ();
    }
  }

  public partial struct FuncInvoker<TFixedArg1, TFixedArg2, TFixedArg3, TResult> : IFuncInvoker<TResult>
  {
    private DelegateSelector _delegateSelector;

    private TFixedArg1 _fixedArg1;
    private TFixedArg2 _fixedArg2;
    private TFixedArg3 _fixedArg3;

    private const int c_argCount = 3;

    public FuncInvoker (DelegateSelector delegateSelector, TFixedArg1 fixedArg1, TFixedArg2 fixedArg2, TFixedArg3 fixedArg3)
    {
      _delegateSelector = delegateSelector;
      _fixedArg1 = fixedArg1;
      _fixedArg2 = fixedArg2;
      _fixedArg3 = fixedArg3;
    }

#pragma warning disable 162 // disable unreachable code warning. 
    private Type[] GetValueTypes (Type[] valueTypes)
    {
      if (c_argCount == 0)
        return valueTypes;
      Type[] fixedArgTypes = new Type[] { typeof (TFixedArg1), typeof (TFixedArg2), typeof (TFixedArg3) };
      return ArrayUtility.Combine (fixedArgTypes, valueTypes);
    }

    private object[] GetValues (object[] values)
    {
      if (c_argCount == 0)
        return values;
      object[] fixedArgs = new object[] { _fixedArg1, _fixedArg2, _fixedArg3 };
      return ArrayUtility.Combine (fixedArgs, values);
    }
#pragma warning restore 162

    public TResult Invoke (Type[] valueTypes, object[] values)
    {
      InvokerUtility.CheckInvokeArguments (valueTypes, values);
      return (TResult) GetDelegate (GetValueTypes (valueTypes)).DynamicInvoke (GetValues (values));
    }

    public TResult Invoke (object[] values)
    {
      Type[] valueTypes = InvokerUtility.GetValueTypes (values);
      return (TResult) GetDelegate (GetValueTypes (valueTypes)).DynamicInvoke (GetValues (values));
    }

    public Delegate GetDelegate (params Type[] parameterTypes)
    {
      return GetDelegate (FuncDelegates.MakeClosedType (typeof (TResult), parameterTypes));
    }

    public TDelegate GetDelegate<TDelegate> ()
    {
      return (TDelegate) (object) GetDelegate (typeof (TDelegate));
    }

    public Delegate GetDelegate (Type delegateType)
    {
      return _delegateSelector (delegateType);
    }

    public TResult With ()
    {
      return GetDelegateWith () (_fixedArg1, _fixedArg2, _fixedArg3);
    }

    public Func<TFixedArg1, TFixedArg2, TFixedArg3, TResult> GetDelegateWith ()
    {
      return GetDelegate<Func<TFixedArg1, TFixedArg2, TFixedArg3, TResult>> ();
    }
  }



    public partial struct FuncInvoker<TResult>
    {


        public TResult With<A1> (A1 a1)
        {
          return GetDelegateWith<A1> () (a1);
        }

        public Func<A1, TResult> GetDelegateWith<A1> ()
        {
          return GetDelegate<Func<A1, TResult>> ();
        }

        public TResult With<A1, A2> (A1 a1, A2 a2)
        {
          return GetDelegateWith<A1, A2> () (a1, a2);
        }

        public Func<A1, A2, TResult> GetDelegateWith<A1, A2> ()
        {
          return GetDelegate<Func<A1, A2, TResult>> ();
        }

        public TResult With<A1, A2, A3> (A1 a1, A2 a2, A3 a3)
        {
          return GetDelegateWith<A1, A2, A3> () (a1, a2, a3);
        }

        public Func<A1, A2, A3, TResult> GetDelegateWith<A1, A2, A3> ()
        {
          return GetDelegate<Func<A1, A2, A3, TResult>> ();
        }

        public TResult With<A1, A2, A3, A4> (A1 a1, A2 a2, A3 a3, A4 a4)
        {
          return GetDelegateWith<A1, A2, A3, A4> () (a1, a2, a3, a4);
        }

        public Func<A1, A2, A3, A4, TResult> GetDelegateWith<A1, A2, A3, A4> ()
        {
          return GetDelegate<Func<A1, A2, A3, A4, TResult>> ();
        }

        public TResult With<A1, A2, A3, A4, A5> (A1 a1, A2 a2, A3 a3, A4 a4, A5 a5)
        {
          return GetDelegateWith<A1, A2, A3, A4, A5> () (a1, a2, a3, a4, a5);
        }

        public Func<A1, A2, A3, A4, A5, TResult> GetDelegateWith<A1, A2, A3, A4, A5> ()
        {
          return GetDelegate<Func<A1, A2, A3, A4, A5, TResult>> ();
        }

        public TResult With<A1, A2, A3, A4, A5, A6> (A1 a1, A2 a2, A3 a3, A4 a4, A5 a5, A6 a6)
        {
          return GetDelegateWith<A1, A2, A3, A4, A5, A6> () (a1, a2, a3, a4, a5, a6);
        }

        public Func<A1, A2, A3, A4, A5, A6, TResult> GetDelegateWith<A1, A2, A3, A4, A5, A6> ()
        {
          return GetDelegate<Func<A1, A2, A3, A4, A5, A6, TResult>> ();
        }

        public TResult With<A1, A2, A3, A4, A5, A6, A7> (A1 a1, A2 a2, A3 a3, A4 a4, A5 a5, A6 a6, A7 a7)
        {
          return GetDelegateWith<A1, A2, A3, A4, A5, A6, A7> () (a1, a2, a3, a4, a5, a6, a7);
        }

        public Func<A1, A2, A3, A4, A5, A6, A7, TResult> GetDelegateWith<A1, A2, A3, A4, A5, A6, A7> ()
        {
          return GetDelegate<Func<A1, A2, A3, A4, A5, A6, A7, TResult>> ();
        }
    }

    public partial struct FuncInvoker<TFixedArg1, TResult>
    {


        public TResult With<A1, A2> (A1 a1, A2 a2)
        {
          return GetDelegateWith<A1, A2> () (_fixedArg1, a1, a2);
        }

        public Func<TFixedArg1, A1, A2, TResult> GetDelegateWith<A1, A2> ()
        {
          return GetDelegate<Func<TFixedArg1, A1, A2, TResult>> ();
        }

        public TResult With<A1, A2, A3> (A1 a1, A2 a2, A3 a3)
        {
          return GetDelegateWith<A1, A2, A3> () (_fixedArg1, a1, a2, a3);
        }

        public Func<TFixedArg1, A1, A2, A3, TResult> GetDelegateWith<A1, A2, A3> ()
        {
          return GetDelegate<Func<TFixedArg1, A1, A2, A3, TResult>> ();
        }

        public TResult With<A1, A2, A3, A4> (A1 a1, A2 a2, A3 a3, A4 a4)
        {
          return GetDelegateWith<A1, A2, A3, A4> () (_fixedArg1, a1, a2, a3, a4);
        }

        public Func<TFixedArg1, A1, A2, A3, A4, TResult> GetDelegateWith<A1, A2, A3, A4> ()
        {
          return GetDelegate<Func<TFixedArg1, A1, A2, A3, A4, TResult>> ();
        }

        public TResult With<A1, A2, A3, A4, A5> (A1 a1, A2 a2, A3 a3, A4 a4, A5 a5)
        {
          return GetDelegateWith<A1, A2, A3, A4, A5> () (_fixedArg1, a1, a2, a3, a4, a5);
        }

        public Func<TFixedArg1, A1, A2, A3, A4, A5, TResult> GetDelegateWith<A1, A2, A3, A4, A5> ()
        {
          return GetDelegate<Func<TFixedArg1, A1, A2, A3, A4, A5, TResult>> ();
        }

        public TResult With<A1, A2, A3, A4, A5, A6> (A1 a1, A2 a2, A3 a3, A4 a4, A5 a5, A6 a6)
        {
          return GetDelegateWith<A1, A2, A3, A4, A5, A6> () (_fixedArg1, a1, a2, a3, a4, a5, a6);
        }

        public Func<TFixedArg1, A1, A2, A3, A4, A5, A6, TResult> GetDelegateWith<A1, A2, A3, A4, A5, A6> ()
        {
          return GetDelegate<Func<TFixedArg1, A1, A2, A3, A4, A5, A6, TResult>> ();
        }

        public TResult With<A1, A2, A3, A4, A5, A6, A7> (A1 a1, A2 a2, A3 a3, A4 a4, A5 a5, A6 a6, A7 a7)
        {
          return GetDelegateWith<A1, A2, A3, A4, A5, A6, A7> () (_fixedArg1, a1, a2, a3, a4, a5, a6, a7);
        }

        public Func<TFixedArg1, A1, A2, A3, A4, A5, A6, A7, TResult> GetDelegateWith<A1, A2, A3, A4, A5, A6, A7> ()
        {
          return GetDelegate<Func<TFixedArg1, A1, A2, A3, A4, A5, A6, A7, TResult>> ();
        }
    }

    public partial struct FuncInvoker<TFixedArg1, TFixedArg2, TResult>
    {


        public TResult With<A1> (A1 a1)
        {
          return GetDelegateWith<A1> () (_fixedArg1, _fixedArg2, a1);
        }

        public Func<TFixedArg1, TFixedArg2, A1, TResult> GetDelegateWith<A1> ()
        {
          return GetDelegate<Func<TFixedArg1, TFixedArg2, A1, TResult>> ();
        }

        public TResult With<A1, A2> (A1 a1, A2 a2)
        {
          return GetDelegateWith<A1, A2> () (_fixedArg1, _fixedArg2, a1, a2);
        }

        public Func<TFixedArg1, TFixedArg2, A1, A2, TResult> GetDelegateWith<A1, A2> ()
        {
          return GetDelegate<Func<TFixedArg1, TFixedArg2, A1, A2, TResult>> ();
        }

        public TResult With<A1, A2, A3> (A1 a1, A2 a2, A3 a3)
        {
          return GetDelegateWith<A1, A2, A3> () (_fixedArg1, _fixedArg2, a1, a2, a3);
        }

        public Func<TFixedArg1, TFixedArg2, A1, A2, A3, TResult> GetDelegateWith<A1, A2, A3> ()
        {
          return GetDelegate<Func<TFixedArg1, TFixedArg2, A1, A2, A3, TResult>> ();
        }

        public TResult With<A1, A2, A3, A4> (A1 a1, A2 a2, A3 a3, A4 a4)
        {
          return GetDelegateWith<A1, A2, A3, A4> () (_fixedArg1, _fixedArg2, a1, a2, a3, a4);
        }

        public Func<TFixedArg1, TFixedArg2, A1, A2, A3, A4, TResult> GetDelegateWith<A1, A2, A3, A4> ()
        {
          return GetDelegate<Func<TFixedArg1, TFixedArg2, A1, A2, A3, A4, TResult>> ();
        }

        public TResult With<A1, A2, A3, A4, A5> (A1 a1, A2 a2, A3 a3, A4 a4, A5 a5)
        {
          return GetDelegateWith<A1, A2, A3, A4, A5> () (_fixedArg1, _fixedArg2, a1, a2, a3, a4, a5);
        }

        public Func<TFixedArg1, TFixedArg2, A1, A2, A3, A4, A5, TResult> GetDelegateWith<A1, A2, A3, A4, A5> ()
        {
          return GetDelegate<Func<TFixedArg1, TFixedArg2, A1, A2, A3, A4, A5, TResult>> ();
        }

        public TResult With<A1, A2, A3, A4, A5, A6> (A1 a1, A2 a2, A3 a3, A4 a4, A5 a5, A6 a6)
        {
          return GetDelegateWith<A1, A2, A3, A4, A5, A6> () (_fixedArg1, _fixedArg2, a1, a2, a3, a4, a5, a6);
        }

        public Func<TFixedArg1, TFixedArg2, A1, A2, A3, A4, A5, A6, TResult> GetDelegateWith<A1, A2, A3, A4, A5, A6> ()
        {
          return GetDelegate<Func<TFixedArg1, TFixedArg2, A1, A2, A3, A4, A5, A6, TResult>> ();
        }

        public TResult With<A1, A2, A3, A4, A5, A6, A7> (A1 a1, A2 a2, A3 a3, A4 a4, A5 a5, A6 a6, A7 a7)
        {
          return GetDelegateWith<A1, A2, A3, A4, A5, A6, A7> () (_fixedArg1, _fixedArg2, a1, a2, a3, a4, a5, a6, a7);
        }

        public Func<TFixedArg1, TFixedArg2, A1, A2, A3, A4, A5, A6, A7, TResult> GetDelegateWith<A1, A2, A3, A4, A5, A6, A7> ()
        {
          return GetDelegate<Func<TFixedArg1, TFixedArg2, A1, A2, A3, A4, A5, A6, A7, TResult>> ();
        }
    }

    public partial struct FuncInvoker<TFixedArg1, TFixedArg2, TFixedArg3, TResult>
    {


        public TResult With<A1> (A1 a1)
        {
          return GetDelegateWith<A1> () (_fixedArg1, _fixedArg2, _fixedArg3, a1);
        }

        public Func<TFixedArg1, TFixedArg2, TFixedArg3, A1, TResult> GetDelegateWith<A1> ()
        {
          return GetDelegate<Func<TFixedArg1, TFixedArg2, TFixedArg3, A1, TResult>> ();
        }

        public TResult With<A1, A2> (A1 a1, A2 a2)
        {
          return GetDelegateWith<A1, A2> () (_fixedArg1, _fixedArg2, _fixedArg3, a1, a2);
        }

        public Func<TFixedArg1, TFixedArg2, TFixedArg3, A1, A2, TResult> GetDelegateWith<A1, A2> ()
        {
          return GetDelegate<Func<TFixedArg1, TFixedArg2, TFixedArg3, A1, A2, TResult>> ();
        }

        public TResult With<A1, A2, A3> (A1 a1, A2 a2, A3 a3)
        {
          return GetDelegateWith<A1, A2, A3> () (_fixedArg1, _fixedArg2, _fixedArg3, a1, a2, a3);
        }

        public Func<TFixedArg1, TFixedArg2, TFixedArg3, A1, A2, A3, TResult> GetDelegateWith<A1, A2, A3> ()
        {
          return GetDelegate<Func<TFixedArg1, TFixedArg2, TFixedArg3, A1, A2, A3, TResult>> ();
        }

        public TResult With<A1, A2, A3, A4> (A1 a1, A2 a2, A3 a3, A4 a4)
        {
          return GetDelegateWith<A1, A2, A3, A4> () (_fixedArg1, _fixedArg2, _fixedArg3, a1, a2, a3, a4);
        }

        public Func<TFixedArg1, TFixedArg2, TFixedArg3, A1, A2, A3, A4, TResult> GetDelegateWith<A1, A2, A3, A4> ()
        {
          return GetDelegate<Func<TFixedArg1, TFixedArg2, TFixedArg3, A1, A2, A3, A4, TResult>> ();
        }

        public TResult With<A1, A2, A3, A4, A5> (A1 a1, A2 a2, A3 a3, A4 a4, A5 a5)
        {
          return GetDelegateWith<A1, A2, A3, A4, A5> () (_fixedArg1, _fixedArg2, _fixedArg3, a1, a2, a3, a4, a5);
        }

        public Func<TFixedArg1, TFixedArg2, TFixedArg3, A1, A2, A3, A4, A5, TResult> GetDelegateWith<A1, A2, A3, A4, A5> ()
        {
          return GetDelegate<Func<TFixedArg1, TFixedArg2, TFixedArg3, A1, A2, A3, A4, A5, TResult>> ();
        }

        public TResult With<A1, A2, A3, A4, A5, A6> (A1 a1, A2 a2, A3 a3, A4 a4, A5 a5, A6 a6)
        {
          return GetDelegateWith<A1, A2, A3, A4, A5, A6> () (_fixedArg1, _fixedArg2, _fixedArg3, a1, a2, a3, a4, a5, a6);
        }

        public Func<TFixedArg1, TFixedArg2, TFixedArg3, A1, A2, A3, A4, A5, A6, TResult> GetDelegateWith<A1, A2, A3, A4, A5, A6> ()
        {
          return GetDelegate<Func<TFixedArg1, TFixedArg2, TFixedArg3, A1, A2, A3, A4, A5, A6, TResult>> ();
        }

        public TResult With<A1, A2, A3, A4, A5, A6, A7> (A1 a1, A2 a2, A3 a3, A4 a4, A5 a5, A6 a6, A7 a7)
        {
          return GetDelegateWith<A1, A2, A3, A4, A5, A6, A7> () (_fixedArg1, _fixedArg2, _fixedArg3, a1, a2, a3, a4, a5, a6, a7);
        }

        public Func<TFixedArg1, TFixedArg2, TFixedArg3, A1, A2, A3, A4, A5, A6, A7, TResult> GetDelegateWith<A1, A2, A3, A4, A5, A6, A7> ()
        {
          return GetDelegate<Func<TFixedArg1, TFixedArg2, TFixedArg3, A1, A2, A3, A4, A5, A6, A7, TResult>> ();
        }
    }
}
