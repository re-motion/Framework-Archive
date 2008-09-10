/* Copyright (C) 2005 - 2008 rubicon informationstechnologie gmbh
 *
 * This program is free software: you can redistribute it and/or modify it under 
 * the terms of the re:motion license agreement in license.txt. If you did not 
 * receive it, please visit http://www.re-motion.org/licensing.
 * 
 * Unless otherwise provided, this software is distributed on an "AS IS" basis, 
 * WITHOUT WARRANTY OF ANY KIND, either express or implied. 
 */

using System;
using System.Collections;
using System.Xml;
using Remotion.Utilities;

namespace Remotion.Diagnostics.ToText
{
  public class ToTextBuilderXml : ToTextBuilderBase
  {
    //public DisableableXmlWriter XmlWriter { get; private set; }
    private readonly DisableableXmlWriter _disableableWriter;

    public ToTextBuilderXml (ToTextProvider toTextProvider, XmlWriter xmlWriter)
      : base (toTextProvider)
    {
      _disableableWriter = new DisableableXmlWriter (xmlWriter);
    }

    // TODO: Implement 


    //public override bool UseMultiLine
    //{
    //  get { throw new System.NotImplementedException(); }
    //  set { throw new System.NotImplementedException(); }
    //}

    public override bool Enabled
    {
      get { return _disableableWriter.Enabled; }
      set { _disableableWriter.Enabled = value; }
    }

    public override IToTextBuilderBase WriteTheFollowingIfComplexityLevelIsGreaterThanOrEqualTo (ToTextBuilderOutputComplexityLevel complexityLevel)
    {
      // TODO: Derive DisableableWriter interface, move to base class
      _disableableWriter.Enabled = (OutputComplexity >= complexityLevel) ? true : false;
      return this;
    }

    public override string CheckAndConvertToString ()
    {
      Assertion.IsFalse (IsInSequence);
      return _disableableWriter.ToString ();
    }

    public override IToTextBuilderBase Flush ()
    {
      _disableableWriter.Flush();
      return this;
    }

    public override IToTextBuilderBase WriteNewLine ()
    {
      _disableableWriter.WriteStartElement ("br");
      _disableableWriter.WriteEndElement();
      return this;
    }

    public override IToTextBuilderBase WriteSequenceLiteralBegin (string name, string sequencePrefix, string elementPrefix, string elementPostfix, string separator, string sequencePostfix)
    {
      throw new System.NotSupportedException ("ToTextBuilderXml does not support literal sequences.");
    }

    protected override IToTextBuilderBase SequenceBegin ()
    {
      return SequenceXmlBegin(null,null);
    }

    //protected override IToTextBuilderBase WriteObjectToString (object obj)
    //{
    //  _disableableWriter.WriteValue (obj.ToString ());
    //  return this;
    //}

    //protected override IToTextBuilderBase SequenceBegin (string name, string sequencePrefix, string elementPrefix, string elementPostfix, string separator, string sequencePostfix)
    //{
    //  throw new System.NotImplementedException();
    //}

    protected IToTextBuilderBase SequenceXmlBegin (string name, string sequenceType)
    {
      //ArgumentUtility.CheckNotNull ("name",name);
      //ArgumentUtility.CheckNotNull ("sequenceType",sequenceType);
      
      BeforeNewSequence ();
      SequenceState = new SequenceStateHolder () { Name = name, SequenceType = sequenceType };
      _disableableWriter.WriteStartElement ("seq");
      _disableableWriter.WriteAttributeIfNotEmpty ("name", SequenceState.Name);
      _disableableWriter.WriteAttributeIfNotEmpty ("type", SequenceState.SequenceType);
      return this;
    }


    //protected override void SequenceBeginWritePart (string name, string sequencePrefix, string elementPrefix, string elementPostfix, string separator, string sequencePostfix)
    //protected override void SequenceBeginWritePart (SequenceStateHolder sequenceState)
    //{
    //  _disableableWriter.WriteStartElement ("seq");
    //  //_disableableWriter.Write (SequenceState.SequencePrefix);
    //  string name = sequenceState.Name;
    //  if (name.Length > 0)
    //  {
    //    _disableableWriter.WriteAttribute ("name",name);
    //  }
    //}

    public override IToTextBuilderBase WriteSequenceBegin ()
    {
      return SequenceBegin();
    }

    public override IToTextBuilderBase WriteRawStringUnsafe (string s)
    {
      _disableableWriter.WriteValue (s);
      return this;
    }

    public override IToTextBuilderBase WriteRawStringEscapedUnsafe (string s)
    {
      return WriteRawStringUnsafe(s);
    }

    //public override IToTextBuilderBase sEsc (string s)
    //{
    //  throw new System.NotImplementedException();
    //}

    public override IToTextBuilderBase WriteRawCharUnsafe (char c)
    {
      _disableableWriter.WriteValue (c);
      return this;
    }

    public override IToTextBuilderBase WriteEnumerable (IEnumerable collection)
    {
      SequenceXmlBegin (collection.GetType ().Name, "enumerable");
      foreach (Object element in collection)
      {
        WriteElement (element);
      }
      SequenceEnd ();
      return this;
    }

    public override IToTextBuilderBase WriteArray (Array array)
    {
      throw new System.NotImplementedException ();
      //  var outerProduct = new OuterProductIndexGenerator (array);

      //  SequenceBegin ("array", "", "", "", "", "");
      //  var processor = new ToTextBuilderArrayToTextProcessor (array, this);
      //  outerProduct.ProcessOuterProduct (processor);
      //  SequenceEnd ();

      //  return this;
    }

    public override IToTextBuilderBase WriteSequenceArrayBegin ()
    {
      throw new System.NotImplementedException();
    }

    public override IToTextBuilderBase LowLevelWrite (object obj)
    {
      _disableableWriter.WriteValue (obj);
      return this;
    }

    public override IToTextBuilderBase WriteInstanceBegin (Type type)
    {
      //throw new System.NotImplementedException ();
      SequenceXmlBegin (type.Name, "instance");
      return this;
    }

    protected override void SequenceEnd ()
    {
      SequenceXmlEnd ();
    }

    private void SequenceXmlEnd ()
    {
      //throw new NotImplementedException();
      Assertion.IsTrue (IsInSequence);
      _disableableWriter.WriteEndElement ();

      SequenceState = sequenceStack.Pop ();

      AfterWriteElement ();
    }


    protected override void BeforeWriteElement ()
    {
      _disableableWriter.WriteStartElement ("e");
    }

    protected override void AfterWriteElement ()
    {
      _disableableWriter.WriteEndElement ();
    }


    protected override IToTextBuilderBase WriteMemberRaw (string name, Object obj)
    {
      //throw new System.NotImplementedException ();
      //SequenceBegin ("", name + "=", "", "", "", "");
      //toTextProvider.ToText (obj, this);
      //SequenceEnd ();
      _disableableWriter.WriteStartElement ("var");
      _disableableWriter.WriteAttribute ("name", name);
      WriteElement (obj);
      _disableableWriter.WriteEndElement ();
      return this;
    }
  }
}
