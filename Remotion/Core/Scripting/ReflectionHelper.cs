// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// version 3.0 as published by the Free Software Foundation.
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
using Castle.DynamicProxy;

namespace Remotion.Scripting
{
  /// <summary>
  /// Helps with typical reflection tasks such as creating a ModuleScope.
  /// </summary>
  public static class ReflectionHelper
  {
    public static ModuleScope CreateModuleScope (string namePostfix, bool savePhysicalAssembly)
    {
      string name = "Remotion.CodeGeneration.Generated." + namePostfix;
      string nameSigned = name + ".Signed";
      string nameUnsigned = name + ".Unsigned";
      const string ext = ".dll";
      return new ModuleScope (savePhysicalAssembly, nameSigned, nameSigned + ext, nameUnsigned, nameUnsigned + ext);
    }
  }
}