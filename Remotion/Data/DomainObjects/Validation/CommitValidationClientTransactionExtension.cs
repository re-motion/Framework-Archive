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
using System.Collections.ObjectModel;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Validation
{
  /// <summary>
  /// Handles commit validation for <see cref="ClientTransaction"/> instances.
  /// </summary>
  /// <remarks>
  /// Currently, this extension only checks that all mandatory relations are set.
  /// </remarks>
  public class CommitValidationClientTransactionExtension : ClientTransactionExtensionBase
  {
    private readonly Func<ClientTransaction, IDomainObjectValidator> _validatorFactory;

    public static string DefaultKey
    {
      get { return typeof (CommitValidationClientTransactionExtension).FullName; }
    }

    public Func<ClientTransaction, IDomainObjectValidator> ValidatorFactory
    {
      get { return _validatorFactory; }
    }

    public CommitValidationClientTransactionExtension (Func<ClientTransaction, IDomainObjectValidator> validatorFactory)
      : this (validatorFactory, DefaultKey)
    {
    }

    protected CommitValidationClientTransactionExtension (Func<ClientTransaction, IDomainObjectValidator> validatorFactory, string key)
        : base (key)
    {
      ArgumentUtility.CheckNotNull ("validatorFactory", validatorFactory);
      ArgumentUtility.CheckNotNullOrEmpty ("key", key);

      _validatorFactory = validatorFactory;
    }

    public override void CommitValidate (ClientTransaction clientTransaction, ReadOnlyCollection<DomainObject> changedDomainObjects)
    {
      var validator = _validatorFactory (clientTransaction);
      foreach (var domainObject in changedDomainObjects)
        validator.Validate (domainObject);
    }
  }
}