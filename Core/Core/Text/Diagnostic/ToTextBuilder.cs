using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Remotion.Text.Diagnostic
{
  public class ToTextBuilder
  {
    /* Features:
     * Start-/End(class)
     * Start-/EndCollection(class)
     * Start-/EndCollectionDimension(class)
     * Start-/EndCollectionEntry(class): seperator
     * 
     * s ... append string
     * sf ... append formatted string
     * nl ... append newline
     * space, tab ... append whitespace
     * m ... named class member
     * c ... class
     * 
     * XML: Support text to be added to be processed to become XML compatible ("<" -> "&lt;" etc). Use CDATA ?
    */

    private StringBuilder _stringBuilder = new StringBuilder();
    private ToTextProvider _toTextProvider;


    public ToTextBuilder(ToTextProvider toTextProvider)
    {
      _toTextProvider = toTextProvider;
    }


    public ToTextBuilder AppendString (string s)
    {
      _stringBuilder.Append (s);
      return this;
    }

    public ToTextBuilder AppendNewLine ()
    {
      _stringBuilder.Append (System.Environment.NewLine);
      return this;
    }

    public ToTextBuilder AppendSpace ()
    {
      _stringBuilder.Append (" ");
      return this;
    }

    public ToTextBuilder AppendTabulator ()
    {
      _stringBuilder.Append ("\t");
      return this;
    }

    public ToTextBuilder AppendSeperator ()
    {
      _stringBuilder.Append (",");
      return this;
    }

    public ToTextBuilder AppendComma ()
    {
      _stringBuilder.Append (",");
      return this;
    }

    public ToTextBuilder AppendColon ()
    {
      _stringBuilder.Append (":");
      return this;
    }

    public ToTextBuilder AppendSemiColon ()
    {
      _stringBuilder.Append (";");
      return this;
    }


    private ToTextBuilder AppendObjectToString (object o)
    {
      _stringBuilder.Append (o.ToString());
      return this;
    }


    public ToTextBuilder AppendMember (string name, Object o)
    {
      //throw new System.NotImplementedException ();
      _stringBuilder.Append (name);
      _stringBuilder.Append (":");
      _toTextProvider.ToText (o, this);
      return this;
    }

    public ToTextBuilder AppendEnumerable (IEnumerable collection)
    {
      const string sequenceStart = "{";
      const string sequenceSeperator = ",";
      const string sequenceEnd = "}";

      Append (sequenceStart);
      bool insertSeperator = false; // no seperator before first element
      foreach (Object element in collection)
      {
        if (insertSeperator)
        {
          Append (sequenceSeperator);
        }
        else
        {
          insertSeperator = true;
        }

        ToText (element);
      }
      Append (sequenceEnd);
      return this;
    }



    private class ArrayToTextProcessor : OuterProduct.ProcessorBase
    {
      protected readonly Array _array;
      private ToTextBuilder _toTextBuilder;
      //public readonly StringBuilder _result = new StringBuilder ();

      public ArrayToTextProcessor (Array rectangularArray, ToTextBuilder toTextBuilder)
      {
        _array = rectangularArray;
        _toTextBuilder = toTextBuilder;
      }

      public override bool DoBeforeLoop ()
      {
        if (ProcessingState.IsInnermostLoop)
        {
          _toTextBuilder.s (ProcessingState.IsFirstLoopElement ? "" : ",");
          _toTextBuilder.ToText (_array.GetValue (ProcessingState.DimensionIndices));
        }
        else
        {
          _toTextBuilder.s (ProcessingState.IsFirstLoopElement ? "" : ",");
          _toTextBuilder.s ("{");
        }
        return true;
      }

      public override bool DoAfterLoop ()
      {
        if (!ProcessingState.IsInnermostLoop)
        {
          _toTextBuilder.s ("}");
        }
        return true;
      }
    }


    public ToTextBuilder AppendArray (Array array)
    {
      var outerProduct = new OuterProduct (array);
      AppendString ("{");
      var processor = new ArrayToTextProcessor (array, this);
      outerProduct.ProcessOuterProduct (processor);
      AppendString ("}");
      return this;
    }



    public ToTextBuilder AppendToText (Object o)
    {
      _toTextProvider.ToText (o, this);
      return this;
    }

    public ToTextBuilder Append (string s)
    {
      return AppendString (s);
    }


    public ToTextBuilder ToTextString (string s)
    {
      return AppendString (s);
    }

    public ToTextBuilder sf (string format, params object[] paramArray)
    {
      return AppendString (string.Format (format, paramArray));
    }


    public ToTextBuilder s (string s)
    {
      return AppendString (s);
    }


    public ToTextBuilder nl 
    {
      get { AppendNewLine (); return this; }
    }

    public ToTextBuilder space
    {
      get { AppendSpace (); return this; }
    }

    public ToTextBuilder tab
    {
      get { AppendTabulator (); return this; }
    }

    public ToTextBuilder seperator
    {
      get { AppendSeperator (); return this; }
    }

    public ToTextBuilder comma
    {
      get { AppendComma (); return this; }
    }

    public ToTextBuilder colon
    {
      get { AppendColon (); return this; }
    }

    public ToTextBuilder semicolon
    {
      get { AppendSemiColon (); return this; }
    }


    public ToTextBuilder tt (Object o)
    {
      return AppendToText (o);
    }

    public ToTextBuilder m (Object o)
    {
      return AppendToText (o);
    }

    public ToTextBuilder m(string name, Object o)
    {
      return AppendMember (name, o);
    }

    public ToTextBuilder ts (object o)
    {
      return AppendObjectToString (o);
    }

    public ToTextBuilder collection (IEnumerable collection)
    {
      return AppendEnumerable (collection);
    }


    public ToTextBuilder array (Array array)
    {
      return AppendArray (array);
    }



    public override string ToString ()
    {
      return _stringBuilder.ToString();
    }

    public ToTextBuilder ToText (object o)
    {
      //throw new NotImplementedException();
      _toTextProvider.ToText(o, this);
      return this;
    }


  }
}