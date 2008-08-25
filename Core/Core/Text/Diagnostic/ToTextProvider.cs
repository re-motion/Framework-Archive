using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Text;
using Remotion.Collections;
using Remotion.Utilities;

namespace Remotion.Text.Diagnostic
{
  /// <summary>
  /// Provides conversion of arbitray objects into human readable text form (<c>ToText</c> method) 
  /// using a fallback cascade starting with registered external object-to-text-conversion-handlers.
  /// 
  /// The conversion is done through the following mechanisms, in order of precedence:
  /// <list type="number">
  /// <item>Handler for object type registered (see <see cref="RegisterHandler{T}"/>)</item>
  /// <item>Implements <see cref="IToTextHandler"/> (i.e. object supplies <c>ToText</c> method)</item>
  /// <item>Is a string or character (see <see cref="UseAutomaticStringEnclosing"/> and <see cref="UseAutomaticCharEnclosing"/> respectively)</item>
  /// <item>Is a primitive: Floating point numbers are alway output formatted US style.</item>
  /// <item>Is a (rectangular) array (arrays have are be treted seperately to prevent them from from being handled as IEnumerable)</item>
  /// <item>Implements IEnumerable</item>
  /// <item>Log instance members through reflection (see <see cref="UseAutomaticObjectToText"/>)</item>
  /// <item>If all of the above fail, the object's <c>ToString</c> method is called</item>
  /// </list>
  /// 
  /// </summary>
  public class ToTextProvider
  {
    private interface IToTextHandlerExternal
    {
      void ToText (Object obj, ToTextBuilder toTextBuilder);
    }

    private class ToTextHandlerExternal<T> : IToTextHandlerExternal
    {
      private readonly Action<T, ToTextBuilder> _handler;

      public ToTextHandlerExternal (Action<T, ToTextBuilder> handler)
      {
        _handler = handler;
      }

      public void ToText (object obj, ToTextBuilder toTextBuilder)
      {
        _handler ((T) obj, toTextBuilder);
      }
    }


    private readonly Dictionary<Type, IToTextHandlerExternal> _typeHandlerMap = new Dictionary<Type, IToTextHandlerExternal> ();
    private static readonly NumberFormatInfo _numberFormatInfoUS = new CultureInfo ("en-US", false).NumberFormat;

    // Define a cache instance (dictionary syntax)
    private static readonly InterlockedCache<Tuple<Type, BindingFlags>, MemberInfo[]> memberInfoCache = new InterlockedCache<Tuple<Type, BindingFlags>, MemberInfo[]>();
    
    private bool _automaticObjectToText = true;
    private bool _automaticStringEnclosing = true;
    private bool _automaticCharEnclosing = true;

    private bool _emitPublicProperties = true;
    private bool _emitPublicFields = true;
    private bool _emitPrivateProperties = true;
    private bool _emitPrivateFields = true;


    public bool UseAutomaticObjectToText
    {
      get { return _automaticObjectToText; }
      set { _automaticObjectToText = value; }
    }

    public bool UseAutomaticStringEnclosing
    {
      get { return _automaticStringEnclosing; }
      set { _automaticStringEnclosing = value; }
    }

    public bool UseAutomaticCharEnclosing
    {
      get { return _automaticCharEnclosing; }
      set { _automaticCharEnclosing = value; }
    }

    public bool EmitPublicProperties
    {
      get { return _emitPublicProperties; }
      set { _emitPublicProperties = value; }
    }

    public bool EmitPublicFields
    {
      get { return _emitPublicFields; }
      set { _emitPublicFields = value; }
    }

    public bool EmitPrivateProperties
    {
      get { return _emitPrivateProperties; }
      set { _emitPrivateProperties = value; }
    }

    public bool EmitPrivateFields
    {
      get { return _emitPrivateFields; }
      set { _emitPrivateFields = value; }
    }

    public void SetAutomaticObjectToTextEmit (bool emitPublicProperties, bool emitPublicFields, bool emitPrivateProperties, bool emitPrivateFields)
    {
      _emitPublicProperties = emitPublicProperties;
      _emitPublicFields = emitPublicFields;
      _emitPrivateProperties = emitPrivateProperties;
      _emitPrivateFields = emitPrivateFields;
    }



    public string ToTextString (object obj)
    {
      var toTextBuilder = new ToTextBuilder (this);
      return toTextBuilder.ToText (obj).ToString ();
    }

    public void ToText (object obj, ToTextBuilder toTextBuilder)
    {
      Assertion.IsNotNull (toTextBuilder);

      // Handle Cascade:
      // *) Is null
      // *) Type handler registered
      // *) Is string (Treat seperately to prevent from being treated as IEnumerable)
      // *) Is primitive: To prevent them from being handled through reflection
      // *) Is rectangular array (Treat seperately to prevent from being treated as 1D-collection by IEnumerable)
      // *) Implements IToTextHandler
      // *) If !IsInterface: Base type handler registered (recursive)
      // *) Implements IEnumerable ("is container")
      // *) If enabled: Log properties through reflection
      // *) ToString()

      // TODO Functionality:
      // * Automatic call stack indentation

      if (obj == null)
      {
        Log ("null");
        toTextBuilder.AppendString ("null");
        return;
      }

      Type type = obj.GetType ();

      Log (type.ToString ());

      IToTextHandlerExternal handler = null;
      handler = GetHandler(type);


      if (handler != null)
      {
        //handler.DynamicInvoke (obj, toTextBuilder);
        handler.ToText (obj, toTextBuilder);
      }
      else if (type == typeof (string))
      {
        string s= (string) obj;
        if (UseAutomaticStringEnclosing)
        {
          toTextBuilder.Append ('"');
          toTextBuilder.AppendString (s);
          toTextBuilder.Append ('"');
        }
        else
        {
          toTextBuilder.AppendString(s);
        }
      }
      else if (obj is IToTextHandler)
      {
        ((IToTextHandler) obj).ToText (toTextBuilder);
      }
      else if (obj is Type) 
      {
        // Catch Type|s here to avoid endless recursion in AutomaticObjectToText below.
        toTextBuilder.AppendString (obj.ToString ());
      }
      else if (type.IsPrimitive)
      {
        if (type == typeof (char))
        {
          char c = (char) obj;
          if (UseAutomaticCharEnclosing)
          {
            toTextBuilder.Append ('\'');
            toTextBuilder.Append (c);
            toTextBuilder.Append ('\'');
          }
          else
          {
            toTextBuilder.Append (c);
          }
        }
        else if (type == typeof (Single)) 
        {
          // Make sure floating point numbers are emitted with '.' comma character (non-localized)
          // to avoid problems with comma as an e.g. sequence seperator character.
          // Since ToText is to be used for debug output, localizing everything to the common
          // IT norm of using US syntax (except for dates) makes sense.
          toTextBuilder.AppendString (((Single) obj).ToString (_numberFormatInfoUS));
        }
        else if (type == typeof (Double))
        {
          toTextBuilder.AppendString (((Double) obj).ToString (_numberFormatInfoUS));
        }
        else
        {
          // Emit primitives who have no registered specific handler without further processing.
          toTextBuilder.Append (obj);
        }
      }
      else if (type.IsArray)
      {
        ArrayToText ((Array) obj, toTextBuilder);
      }
      else if (type.GetInterface ("IEnumerable") != null)
      {
        CollectionToText ((IEnumerable) obj, toTextBuilder);
      }
      else if (_automaticObjectToText)
      {
        //AutomaticObjectToText (obj, toTextBuilder, true, true, true, true);
        AutomaticObjectToText (obj, toTextBuilder, EmitPublicProperties, EmitPublicFields, EmitPrivateProperties, EmitPrivateFields);
      }
      else
      {
        toTextBuilder.AppendString (obj.ToString ());
      }
    }

    private IToTextHandlerExternal GetHandler (Type type)
    {
      IToTextHandlerExternal handler = GetHandlerWithBaseClassFallback(type, 0, 1);
      return handler;
    }

    private IToTextHandlerExternal GetHandlerWithBaseClassFallback (Type type, int recursionDepth, int recursionDepthMax)
    {
      if (recursionDepth >= recursionDepthMax)
      {
        return null;
      }

      IToTextHandlerExternal handler;
      _typeHandlerMap.TryGetValue (type, out handler);

      if (handler != null)
      {
        return handler;
      }

      Type baseType = type.BaseType;
      if(baseType == null)
      {
        return null;
      }

      return GetHandlerWithBaseClassFallback (baseType, recursionDepth + 1, recursionDepthMax);
    }


    public void RegisterHandler<T> (Action<T, ToTextBuilder> handler)
    {
      _typeHandlerMap.Add (typeof (T),new ToTextHandlerExternal<T> (handler));
    }

    public void ClearHandlers ()
    {
      _typeHandlerMap.Clear ();
    }



    public static void CollectionToText (IEnumerable collection, ToTextBuilder toTextBuilder)
    {
      toTextBuilder.AppendEnumerable(collection);
    }


    public static void ArrayToText (Array array, ToTextBuilder toTextBuilder)
    {
      toTextBuilder.AppendArray(array);
    }



    private static object GetValue (object obj, Type type, MemberInfo memberInfo)
    {
      object value = null;
      if (memberInfo is PropertyInfo)
      {
        value = ((PropertyInfo)memberInfo).GetValue (obj, null);
      }
      else if (memberInfo is FieldInfo)
      {
        value = ((FieldInfo) memberInfo).GetValue (obj);
      }
      else
      {
        throw new System.NotImplementedException ();
      }
      return value;
    }


    private static void AutomaticObjectToTextProcessMemberInfos (string message, Object obj, BindingFlags bindingFlags, 
      MemberTypes memberTypeFlags, ToTextBuilder toTextBuilder)
    {
      Type type = obj.GetType ();

      // Cache the member info result
      MemberInfo[] memberInfos = memberInfoCache.GetOrCreateValue (new Tuple<Type, BindingFlags> (type, bindingFlags), tuple => tuple.A.GetMembers (tuple.B));

      foreach (var memberInfo in memberInfos)
      {
        if ((memberInfo.MemberType & memberTypeFlags) != 0)
        {
          string name = memberInfo.Name;

          // Skip backing fields
          bool processMember = !name.Contains("k__");

          if (processMember)
          {
            object value = GetValue(obj, type, memberInfo);
            // AppendMember ToText|s value
            toTextBuilder.AppendMember(name, value);
          }
        }
      }
    }



    public void AutomaticObjectToText (object obj, ToTextBuilder toTextBuilder, 
      bool emitPublicProperties, bool emitPublicFields, bool emitPrivateProperties, bool emitPrivateFields)
    {
      Type type = obj.GetType ();

      toTextBuilder.beginInstance(type);

      if (emitPublicProperties)
      {
        AutomaticObjectToTextProcessMemberInfos (
            "Public Properties", obj, BindingFlags.Instance | BindingFlags.Public, MemberTypes.Property, toTextBuilder);
      }
      if (emitPublicFields)
      {
        AutomaticObjectToTextProcessMemberInfos ("Public Fields", obj, BindingFlags.Instance | BindingFlags.Public, MemberTypes.Field, toTextBuilder);
      }
      if (emitPrivateProperties)
      {
        AutomaticObjectToTextProcessMemberInfos ("Non Public Properties", obj, BindingFlags.Instance | BindingFlags.NonPublic, MemberTypes.Property, toTextBuilder);
      }
      if (emitPrivateFields)
      {
        AutomaticObjectToTextProcessMemberInfos ("Non Public Fields", obj, BindingFlags.Instance | BindingFlags.NonPublic, MemberTypes.Field, toTextBuilder);
      }
      toTextBuilder.endInstance();
    }



    private static void Log (string s)
    {
      Console.WriteLine ("[To]: " + s);
    }

    private static void LogVariables (string format, params object[] parameterArray)
    {
      Log (String.Format (format, parameterArray));
    }

 
  }
}

