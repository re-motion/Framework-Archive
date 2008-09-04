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

namespace Remotion.Diagnostics.ToText
{
  internal class ToTextBuilderArrayToTextProcessor : OuterProductProcessorBase
  {
    protected readonly Array _array;
    private readonly ToTextBuilder _toTextBuilder;

    public ToTextBuilderArrayToTextProcessor (Array rectangularArray, ToTextBuilder toTextBuilder)
    {
      _array = rectangularArray;
      _toTextBuilder = toTextBuilder;
    }

    public override bool DoBeforeLoop ()
    {
      if (ProcessingState.IsInnermostLoop)
      {
        _toTextBuilder.AppendToText (_array.GetValue (ProcessingState.DimensionIndices));
      }
      else
      {
        _toTextBuilder.AppendSequenceBegin (_toTextBuilder.ArrayPrefix, _toTextBuilder.ArrayFirstElementPrefix, _toTextBuilder.ArrayOtherElementPrefix, _toTextBuilder.ArrayElementPostfix, _toTextBuilder.ArrayPostfix);
      }
      return true;
    }



    public override bool DoAfterLoop ()
    {
      if (!ProcessingState.IsInnermostLoop)
      {
        _toTextBuilder.AppendSequenceEnd ();
      }
      return true;
    }



  }
}