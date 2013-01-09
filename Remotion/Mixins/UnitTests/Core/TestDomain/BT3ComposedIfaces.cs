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
namespace Remotion.Mixins.UnitTests.Core.TestDomain
{
// ReSharper disable PossibleInterfaceMemberAmbiguity - This is on purpose.
// ReSharper disable RedundantExtendsListEntry - This is on purpose.
  public interface ICBaseType3 : IBaseType31, IBaseType32, IBaseType33, IBaseType34, IBaseType35
// ReSharper restore RedundantExtendsListEntry
// ReSharper restore PossibleInterfaceMemberAmbiguity
  { }

  public interface ICBaseType3BT3Mixin4 : ICBaseType3, IBT3Mixin4
  { }
}