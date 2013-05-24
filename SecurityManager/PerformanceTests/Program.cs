﻿using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using Microsoft.Practices.ServiceLocation;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Queries;
using Remotion.Logging;
using Remotion.Security;
using Remotion.SecurityManager.Domain;
using Remotion.SecurityManager.Domain.AccessControl;
using Remotion.SecurityManager.Domain.AccessControl.AccessEvaluation;
using Remotion.SecurityManager.Domain.OrganizationalStructure;
using Remotion.ServiceLocation;
using Remotion.Utilities;

namespace Remotion.SecurityManager.PerformanceTests
{
  class Program
  {
    static void Main (string[] args)
    {
      var defaultServiceLocator = new DefaultServiceLocator();

      defaultServiceLocator.Register (typeof (Remotion.Data.DomainObjects.IClientTransactionExtensionFactory), typeof (Remotion.Data.DomainObjects.UberProfIntegration.LinqToSqlExtensionFactory), LifetimeKind.Singleton);
      defaultServiceLocator.Register (typeof (Remotion.Data.DomainObjects.Tracing.IPersistenceExtensionFactory), typeof (Remotion.Data.DomainObjects.UberProfIntegration.LinqToSqlExtensionFactory), LifetimeKind.Singleton);

      ServiceLocator.SetLocatorProvider (() => defaultServiceLocator);

      LogManager.Initialize();

      SecurityService provider = new SecurityService("SecurityManager", new NameValueCollection());
      var context =
          new SimpleSecurityContext (
             "ActaNova.Federal.Domain.File, ActaNova.Federal.Domain",
              "ServiceUser",
              string.Empty,
              "SystemTenant",
              false,
              new Dictionary<string, EnumWrapper> { { "CommonFileState", EnumWrapper.Get ("Work|ActaNova.Domain.CommonFile+CommonFileStateType, ActaNova.Domain") } },
              new EnumWrapper[0]);
      ISecurityPrincipal user = new SecurityPrincipal ("ServiceUser", null, null, null);
      MappingConfiguration.Current.GetTypeDefinitions();
      QueryFactory.CreateLinqQuery<Tenant>(); // Takes about 200ms
      using (ClientTransaction.CreateRootTransaction().EnterDiscardingScope())
      {
        (from t in QueryFactory.CreateLinqQuery<Tenant>()
         select new { Key = t.UniqueIdentifier, Value = t.ID.GetHandle<Tenant>() }).ToList(); // takes about 180ms for first Linq query in application.
      }

      Console.WriteLine ("Initializing query cache...");
      new SecurityContextRepository (new RevisionProvider()).GetTenant ("SystemTenant");
      new SecurityPrincipalRepository (new RevisionProvider()).GetUser ("ServiceUser");
      Console.WriteLine ("Query cache initialized");
      Console.WriteLine();

      using (StopwatchScope.CreateScope ("First access check took {elapsed:ms} ms."))
      {
        provider.GetAccess (context, user);
      }
      Console.WriteLine ("Init done");
      //Console.ReadKey();

      Stopwatch stopwatch = Stopwatch.StartNew();
      int dummy = 0;
      int count = 1;
      for (int i = 0; i < count; i++)
      {
        dummy += provider.GetAccess (context, user).Length;
      }
      stopwatch.Stop();
      Trace.Write (dummy);
      Console.WriteLine ("Time taken: {0}ms", ((decimal)stopwatch.ElapsedMilliseconds)/count);
      //Console.ReadKey();
    }
  }
}
