﻿// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2008 rubicon informationstechnologie gmbh, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// version 3.0 as published by the Free Software Foundation.
// 
// This framework is distributed in the hope that it will be useful, 
// but WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
// GNU Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public License
// along with this framework; if not, see http://www.gnu.org/licenses.
// 
using System;

namespace Remotion.Mixins.Samples.Tutorial.T01_Configuration
{
  namespace ExtendsSamples
  {
    [Extends (typeof (MyClass))]
    public class MyMixin
    {
    }

    public class MyClass
    {
    }

    public class File
    {
    }

    [Extends (typeof (File))]
    public class NumberedFileMixin : INumberedFile
    {
      public string GetFileNumber ()
      {
        return Guid.NewGuid ().ToString();
      }
    }

    public interface INumberedFile
    {
      string GetFileNumber ();
    }
  }
}