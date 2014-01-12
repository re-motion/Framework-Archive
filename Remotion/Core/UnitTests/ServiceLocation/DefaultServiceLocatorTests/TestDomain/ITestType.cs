﻿// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (c) rubicon IT GmbH, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// as published by the Free Software Foundation; either version 2.1 of the 
// License, or (at your option) any later version.
// 
// re-motion is distributed in the hope that it will be useful, 
// but WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
// GNU Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public License
// along with re-motion; if not, see http://www.gnu.org/licenses.
// 

using System;
using System.Collections.Generic;
using System.Linq;

namespace Remotion.UnitTests.ServiceLocation.DefaultServiceLocatorTests.TestDomain
{
  public interface ITestType
  {
  }

  public class TestImplementation1 : ITestType
  {
  }

  public class TestImplementation2 : ITestType
  {
  }

  public class TestCompound : ITestType
  {
    public IEnumerable<ITestType> CompoundRegistrations { get; private set; }

    public TestCompound (IEnumerable<ITestType> compoundRegistrations)
    {
      CompoundRegistrations = compoundRegistrations.ToArray();
    }
  }


  public class TestCompoundWithAdditionalConstructorParameters : ITestType
  {
    public StubService StubService { get; private set; }

    public IEnumerable<ITestType> CompoundRegistrations { get; private set; }

    public TestCompoundWithAdditionalConstructorParameters (StubService stubService, IEnumerable<ITestType> compoundRegistrations)
    {
      StubService = stubService;
      CompoundRegistrations = compoundRegistrations.ToArray();
    }
  }
}