// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
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
using Remotion.Data.Linq;
using Remotion.Data.Linq.EagerFetching;
using Remotion.Data.Linq.SqlBackend.SqlPreparation;
using Remotion.Data.Linq.SqlBackend.SqlPreparation.ResultOperatorHandlers;
using Remotion.Data.Linq.SqlBackend.SqlStatementModel;

namespace Remotion.Data.DomainObjects.Linq
{
  /// <summary>
  /// Implements <see cref="IResultOperatorHandler"/> for classes derived from <see cref="FetchRequestBase"/>. Since those result operators cannot
  /// be translated to SQL, but must be removed by the LINQ provider _before_ the query is translated to SQL, the <see cref="FetchResultOperatorHandler"/>
  /// always throws a <see cref="NotSupportedException"/>.
  /// </summary>
  public class FetchResultOperatorHandler : ResultOperatorHandler<FetchRequestBase>
  {
    public override void HandleResultOperator (FetchRequestBase resultOperator, SqlStatementBuilder sqlStatementBuilder, UniqueIdentifierGenerator generator, ISqlPreparationStage stage, ISqlPreparationContext context)
    {
      throw new NotSupportedException (
          "The fetch query operator methods must be the last query operators in a LINQ query. All calls to Where, Select, Take, etc. must go before "
          + "the fetch operators."
          + Environment.NewLine
          + Environment.NewLine
          + "E.g., instead of 'QueryFactory.CreateLinqQuery<Order>().FetchMany (o => o.OrderItems).Where (o => o.OrderNumber > 1)', "
          + "write 'QueryFactory.CreateLinqQuery<Order>().Where (o => o.OrderNumber > 1).FetchMany (o => o.OrderItems)'.");
    }
  }
}