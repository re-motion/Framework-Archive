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
using System.IO;
using System.Xml;
using Remotion.Utilities;

namespace Remotion.Diagnostics.ToText
{
  public class ToTextBuilderXml : ToTextBuilderBase
  {
    //public DisableableXmlWriter XmlWriter { get; private set; }
    private readonly DisableableXmlWriter _disableableXmlWriter;

    public ToTextBuilderXml (ToTextProvider toTextProvider, XmlWriter xmlWriter)
      : base (toTextProvider)
    {
      _disableableXmlWriter = new DisableableXmlWriter (xmlWriter);
    }

    // TODO: Implement 

    #region Overrides of ToTextBuilderBase

    //public override bool UseMultiLine
    //{
    //  get { throw new System.NotImplementedException(); }
    //  set { throw new System.NotImplementedException(); }
    //}

    public override bool Enabled
    {
      get { return _disableableXmlWriter.Enabled; }
      set { _disableableXmlWriter.Enabled = value; }
    }

    //public override ToTextBuilder seperator
    //{
    //  get { throw new System.NotImplementedException(); }
    //}

    //public override ToTextBuilder comma
    //{
    //  get { throw new System.NotImplementedException(); }
    //}

    //public override ToTextBuilder colon
    //{
    //  get { throw new System.NotImplementedException(); }
    //}

    //public override ToTextBuilder semicolon
    //{
    //  get { throw new System.NotImplementedException(); }
    //}

    public override IToTextBuilderBase Append (object obj)
    {
      _disableableXmlWriter.WriteValue(obj);
      return this;
    }

    protected override void SequenceEnd ()
    {
      Assertion.IsTrue (IsInSequence);

      // TODO: Extract the next line into seperate method, move rest of implementation to base class.
      _disableableXmlWriter.WriteEndElement ();

      SequenceState = _sequenceStack.Pop ();
    }

    public override IToTextBuilderBase ToTextString (string s)
    {
      throw new System.NotImplementedException();
    }

    public override IToTextBuilderBase AppendTheFollowingIfComplexityLevelIsGreaterThanOrEqualTo (ToTextBuilderOutputComplexityLevel complexityLevel)
    {
      throw new System.NotImplementedException();
    }

    public override string CheckAndConvertToString ()
    {
      throw new System.NotImplementedException();
    }

    public override ToTextBuilder ToText (object obj)
    {
      throw new System.NotImplementedException();
    }

    public override IToTextBuilderBase Flush ()
    {
      throw new System.NotImplementedException();
    }

    public override IToTextBuilderBase sf (string format, params object[] paramArray)
    {
      throw new System.NotImplementedException();
    }

    public override ToTextBuilder AppendNewLine ()
    {
      throw new System.NotImplementedException();
    }

    public override ToTextBuilder nl ()
    {
      throw new System.NotImplementedException();
    }

    public override ToTextBuilder AppendSpace ()
    {
      throw new System.NotImplementedException();
    }

    public override IToTextBuilderBase space ()
    {
      throw new System.NotImplementedException();
    }

    public override ToTextBuilder AppendTabulator ()
    {
      throw new System.NotImplementedException();
    }

    public override ToTextBuilder tab ()
    {
      throw new System.NotImplementedException();
    }

    public override ToTextBuilder AppendSeperator ()
    {
      throw new System.NotImplementedException();
    }

    public override ToTextBuilder AppendComma ()
    {
      throw new System.NotImplementedException();
    }

    public override ToTextBuilder AppendColon ()
    {
      throw new System.NotImplementedException();
    }

    public override ToTextBuilder AppendSemiColon ()
    {
      throw new System.NotImplementedException();
    }

    public override IToTextBuilderBase AppendArray (Array array)
    {
      throw new System.NotImplementedException();
    }

    public override IToTextBuilderBase AppendString (string s)
    {
      throw new System.NotImplementedException();
    }

    public override ToTextBuilder AppendEscapedString (string s)
    {
      throw new System.NotImplementedException();
    }

    public override ToTextBuilder sEsc (string s)
    {
      throw new System.NotImplementedException();
    }

    public override IToTextBuilderBase AppendChar (char c)
    {
      throw new System.NotImplementedException();
    }

    public override IToTextBuilderBase AppendMember (string name, object obj)
    {
      throw new System.NotImplementedException();
    }

    public override IToTextBuilderBase AppendEnumerable (IEnumerable collection)
    {
      throw new System.NotImplementedException();
    }

    public override IToTextBuilderBase array (Array array)
    {
      throw new System.NotImplementedException();
    }

    protected override ToTextBuilder AppendMemberRaw (string name, object obj)
    {
      throw new System.NotImplementedException();
    }

    protected override ToTextBuilder AppendObjectToString (object obj)
    {
      throw new System.NotImplementedException();
    }

    protected override ToTextBuilder SequenceBegin (string sequencePrefix, string firstElementPrefix, string otherElementPrefix, string elementPostfix, string sequencePostfix)
    {
      throw new System.NotImplementedException();
    }

    protected override void BeforeAppendElement ()
    {
      throw new System.NotImplementedException();
    }

    protected override void AfterAppendElement ()
    {
      throw new System.NotImplementedException();
    }

    #endregion
  }
}