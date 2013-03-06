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

namespace Remotion.Data.DomainObjects.DataManagement.RelationEndPoints
{
  /// <summary>
  /// Provides common checks needed by relation end points when they are assigned new values.
  /// </summary>
  public static class RelationEndPointValueChecker
  {
    public static void CheckClientTransaction (IRelationEndPoint endPoint, DomainObject domainObject, string exceptionFormatString)
    {
      // TODO 5447: Compare root transactions.
      if (domainObject != null && !endPoint.ClientTransaction.IsEnlisted (domainObject))
      {
        var formattedMessage = String.Format (
            exceptionFormatString, 
            domainObject.ID, 
            endPoint.Definition.PropertyName, 
            endPoint.ObjectID);
        throw new ClientTransactionsDifferException (formattedMessage + " The objects do not belong to the same ClientTransaction.");
      }
    }
  }
}