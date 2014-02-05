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
using System.Linq.Expressions;
using System.Reflection;
using Remotion.Reflection;
using Remotion.Utilities;

namespace Remotion.Validation.Utilities
{
  public static class ExpressionExtensions
  {
    public static IPropertyInformation ExtractDynamicMemberInfo<TValidatedType, TProperty> (this Expression<Func<TValidatedType, TProperty>> propertyExpression)
    {
      ArgumentUtility.CheckNotNull ("propertyExpression", propertyExpression);

      var memberExpression = propertyExpression.Body as MemberExpression;
      if (memberExpression == null)
        throw new ArgumentException (string.Format ("Expression not a MemberExpresssion: {0}", propertyExpression), "propertyExpression");

      var property = memberExpression.Member as PropertyInfo;
      if (property == null)
        throw new ArgumentException (string.Format ("Expression not a Property: {0}", propertyExpression), "propertyExpression");

      var getMethod = property.GetGetMethod (true);
      if (getMethod.IsStatic)
        throw new ArgumentException (string.Format ("Expression cannot be static: {0}", propertyExpression), "propertyExpression");

      Type realType = memberExpression.Expression.Type;
      if (realType == null)
        throw new ArgumentException (string.Format ("Expression has no DeclaringType: {0}", propertyExpression), "propertyExpression");

      return PropertyInfoAdapter.Create(realType.GetProperty (property.Name));
    }
  }
}