// This file is part of the re-motion Core Framework (www.re-motion.org)
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
using Remotion.ServiceLocation;

namespace Remotion.UnitTests.ServiceLocation.TestDomain
{
  public interface ITestConstructorInjectionWithThreeParameters
  {
  }

  [ConcreteImplementation (typeof (ITestConstructorInjectionWithThreeParameters))]
  public class TestConstructorInjectionWithThreeParameters : ITestConstructorInjectionWithThreeParameters
  {
    public readonly ITestConstructorInjectionWithOneParameter Param1;
    public readonly ITestConstructorInjectionWithOneParameter Param2;
    public readonly ITestSingletonConcreteImplementationAttributeType Param3;

    public TestConstructorInjectionWithThreeParameters (
        ITestConstructorInjectionWithOneParameter param1,
        ITestConstructorInjectionWithOneParameter param2,
        ITestSingletonConcreteImplementationAttributeType param3)
    {
      Param1 = param1;
      Param2 = param2;
      Param3 = param3;
    }
  }
}