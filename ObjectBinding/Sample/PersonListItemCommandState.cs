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
using System.Web.UI;
using Remotion.ObjectBinding;
using Remotion.ObjectBinding.Web.UI.Controls;

namespace Remotion.ObjectBinding.Sample
{
  public class PersonListItemCommandState: IBocListItemCommandState
  {
    public PersonListItemCommandState ()
    {
    }

    public bool IsEnabled(
        BocList list, 
        IBusinessObject businessObject, 
        BocCommandEnabledColumnDefinition columnDefinition)
    {
      return true;
    }
  }
}
