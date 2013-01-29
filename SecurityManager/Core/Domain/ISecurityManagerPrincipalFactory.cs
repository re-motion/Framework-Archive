// This file is part of re-strict (www.re-motion.org)
// Copyright (c) rubicon IT GmbH, www.rubicon.eu
// 
// This program is free software; you can redistribute it and/or modify
// it under the terms of the GNU Affero General Public License version 3.0 
// as published by the Free Software Foundation.
// 
// This program is distributed in the hope that it will be useful, 
// but WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
// GNU Affero General Public License for more details.
// 
// You should have received a copy of the GNU Affero General Public License
// along with this program; if not, see http://www.gnu.org/licenses.
// 
// Additional permissions are listed in the file re-motion_exceptions.txt.
// 
using System;
using Remotion.Data.DomainObjects;
using Remotion.SecurityManager.Domain.OrganizationalStructure;
using Remotion.ServiceLocation;

namespace Remotion.SecurityManager.Domain
{
  /// <summary>
  /// Declares factory methods for creating an instance for the <see cref="ISecurityManagerPrincipal"/> interface.
  /// </summary>
  /// <seealso cref="SecurityManagerPrincipalFactory"/>
  [ConcreteImplementation (typeof (SecurityManagerPrincipalFactory), Lifetime = LifetimeKind.Singleton)]
  public interface ISecurityManagerPrincipalFactory
  {
    /// <summary>
    /// Instantiates a threadsafe implementation of the <see cref="ISecurityManagerPrincipal"/> interface.
    /// </summary>
    /// <param name="tenantID">The <see cref="ObjectID"/> of the <see cref="Tenant"/>. Must not be <see langword="null" />.</param>
    /// <param name="userID">The <see cref="ObjectID"/> of the <see cref="User"/>. Must not be <see langword="null" />.</param>
    /// <param name="substitutionID">The <see cref="ObjectID"/> of the <see cref="Substitution"/>.</param>
    /// <returns>A threadsafe implementation of the <see cref="ISecurityManagerPrincipal"/> interface.</returns>
    ISecurityManagerPrincipal CreateWithLocking (IDomainObjectHandle<Tenant> tenantID, IDomainObjectHandle<User> userID, IDomainObjectHandle<Substitution> substitutionID);
  }
}