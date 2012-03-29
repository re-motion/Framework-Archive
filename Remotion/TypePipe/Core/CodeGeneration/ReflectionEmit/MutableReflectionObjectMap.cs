// Copyright (c) rubicon IT GmbH, www.rubicon.eu
//
// See the NOTICE file distributed with this work for additional information
// regarding copyright ownership.  rubicon licenses this file to you under 
// the Apache License, Version 2.0 (the "License"); you may not use this 
// file except in compliance with the License.  You may obtain a copy of the 
// License at
//
//   http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software 
// distributed under the License is distributed on an "AS IS" BASIS, WITHOUT 
// WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.  See the 
// License for the specific language governing permissions and limitations
// under the License.
// 
using System;
using System.Collections.Generic;
using System.Reflection;
using Remotion.TypePipe.CodeGeneration.ReflectionEmit.BuilderAbstractions;
using Remotion.TypePipe.CodeGeneration.ReflectionEmit.LambdaCompilation;
using Remotion.Utilities;
using Remotion.Collections;

namespace Remotion.TypePipe.CodeGeneration.ReflectionEmit
{
  /// <summary>
  /// Maps mutable reflection objects to their associated <code>Reflection.Emit</code> builder objects, which can be used for code generation
  /// by <see cref="ILGeneratorDecorator"/>.
  /// </summary>
  [CLSCompliant(false)]
  public class MutableReflectionObjectMap
  {
    private readonly Dictionary<Type, ITypeBuilder> _mappedTypes = new Dictionary<Type, ITypeBuilder> ();
    private readonly Dictionary<ConstructorInfo, IConstructorBuilder> _mappedConstructorInfos = new Dictionary<ConstructorInfo, IConstructorBuilder> ();
    private readonly Dictionary<FieldInfo, IFieldBuilder> _mappedFieldInfos = new Dictionary<FieldInfo, IFieldBuilder> ();

    public void AddMapping (Type mappedType, ITypeBuilder typeBuilder)
    {
      ArgumentUtility.CheckNotNull ("mappedType", mappedType);
      ArgumentUtility.CheckNotNull ("typeBuilder", typeBuilder);

      AddMapping (_mappedTypes, mappedType, typeBuilder);
    }

    public void AddMapping (ConstructorInfo mappedConstructorInfo, IConstructorBuilder constructorBuilder)
    {
      ArgumentUtility.CheckNotNull ("mappedConstructorInfo", mappedConstructorInfo);
      ArgumentUtility.CheckNotNull ("constructorBuilder", constructorBuilder);

      AddMapping (_mappedConstructorInfos, mappedConstructorInfo, constructorBuilder);
    }

    public void AddMapping (FieldInfo mappedFieldInfo, IFieldBuilder fieldBuilder)
    {
      ArgumentUtility.CheckNotNull ("mappedFieldInfo", mappedFieldInfo);
      ArgumentUtility.CheckNotNull ("fieldBuilder", fieldBuilder);

      AddMapping (_mappedFieldInfos, mappedFieldInfo, fieldBuilder);
    }

    public ITypeBuilder GetBuilder (Type mappedType)
    {
      ArgumentUtility.CheckNotNull ("mappedType", mappedType);

      return GetBuilder (_mappedTypes, mappedType);
    }

    public IConstructorBuilder GetBuilder (ConstructorInfo mappedConstructorInfo)
    {
      ArgumentUtility.CheckNotNull ("mappedConstructorInfo", mappedConstructorInfo);

      return GetBuilder (_mappedConstructorInfos, mappedConstructorInfo);
    }

    public IFieldBuilder GetBuilder (FieldInfo mappedFieldInfo)
    {
      ArgumentUtility.CheckNotNull ("mappedFieldInfo", mappedFieldInfo);

      return GetBuilder (_mappedFieldInfos, mappedFieldInfo);
    }

    private void AddMapping<TKey, TValue> (Dictionary<TKey, TValue> mapping, TKey mappedItem, TValue builder)
    {
      string itemNameType = typeof (TKey).Name;
      if (mapping.ContainsKey (mappedItem))
      {
        var message = string.Format ("{0} is already mapped.", itemNameType);
        throw new ArgumentException (message, "mapped" + itemNameType);
      }

      mapping.Add (mappedItem, builder);
    }

    private TValue GetBuilder<TKey, TValue> (Dictionary<TKey, TValue> mapping, TKey mappedItem)
    {
      return mapping.GetValueOrDefault (mappedItem);
    }
  }
}