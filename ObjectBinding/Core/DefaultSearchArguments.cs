/* Copyright (C) 2005 - 2008 rubicon informationstechnologie gmbh
 *
 * This program is free software: you can redistribute it and/or modify it under 
 * the terms of the re:motion license agreement in license.txt. If you did not 
 * receive it, please visit http://www.re-motion.org/licensing.
 * 
 * Unless otherwise provided, this software is distributed on an "AS IS" basis, 
 * WITHOUT WARRANTY OF ANY KIND, either express or implied. 
 */

namespace Remotion.ObjectBinding
{
  public class DefaultSearchArguments: ISearchAvailableObjectsArguments
  {
    private readonly string _searchStatement;

    public DefaultSearchArguments (string searchStatement)
    {
      _searchStatement = searchStatement;
    }

    public string SearchStatement
    {
      get { return _searchStatement; }
    }
  }
}