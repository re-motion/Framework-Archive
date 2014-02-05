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
using System.Linq;
using log4net;
using log4net.Appender;
using log4net.Config;
using NUnit.Framework;
using Remotion.Data.DomainObjects.Validation;
using Remotion.Globalization;
using Remotion.Reflection;
using Remotion.ServiceLocation;
using Remotion.Validation.Globalization;
using Remotion.Validation.Implementation;
using Remotion.Validation.Merging;
using Remotion.Validation.MetaValidation;
using Remotion.Validation.Mixins.Implementation;
using Remotion.Validation.Providers;

namespace Remotion.Validation.IntegrationTests
{
  [TestFixture]
  public abstract class IntegrationTestBase
  {
    protected FluentValidatorBuilder ValidationBuilder;
    protected MemoryAppender MemoryAppender;
    protected bool ShowLogOutput;

    [SetUp]
    public virtual void SetUp ()
    {
      MemoryAppender = new MemoryAppender();
      BasicConfigurator.Configure (MemoryAppender);

      var memberInfoNameResolver = SafeServiceLocator.Current.GetInstance<IMemberInformationNameResolver>();
      var memberInformationGlobalizationService = SafeServiceLocator.Current.GetInstance<IMemberInformationGlobalizationService>();

      ValidationBuilder = new FluentValidatorBuilder (
          new AggregatingValidationCollectorProvider (
              new MixedInvolvedTypeProviderDecorator (
                  InvolvedTypeProvider.Create (
                      types => types.OrderBy (t => t.Name),
                      SafeServiceLocator.Current.GetInstance<ICompoundValidationTypeFilter>())),
              new IValidationCollectorProvider[]
              {
                  new DomainObjectAttributesBasedValidationCollectorProvider(),
                  new ValidationAttributesBasedCollectorProvider(),
                  new ApiBasedComponentValidationCollectorProvider (new DiscoveryServiceBasedTypeCollectorReflector())
              }),
          new DiagnosticOutputRuleMergeDecorator (
              new OrderPrecedenceValidationCollectorMerger (new PropertyValidatorExtractorFactory()),
              new FluentValidationValidatorFormatterDecorator (new DefaultValidatorFormatter())),
          new MetaRulesValidatorFactory (mi => new DefaultSystemMetaValidationRulesProvider (mi)),
          new CompoundValidationRuleGlobalizationService (
              new IValidationRuleGlobalizationService[]
              {
                  new PropertyDisplayNameGlobalizationService (memberInformationGlobalizationService),
                  new ValidationRuleGlobalizationService (new DefaultMessageEvaluator(), GetValidatorGlobalizationService())
              }),
          memberInfoNameResolver);
    }

    [TearDown]
    public void TearDown ()
    {
      if (ShowLogOutput)
      {
        var logEvents = MemoryAppender.GetEvents().Reverse().ToArray();
        Console.WriteLine (logEvents.Skip (1).First().RenderedMessage);
        Console.WriteLine (logEvents.First().RenderedMessage);
      }

      MemoryAppender.Clear();
      LogManager.ResetConfiguration();

      Assert.That (LogManager.GetLogger (typeof (DiagnosticOutputRuleMergeDecorator)).IsDebugEnabled, Is.False);
    }

    protected virtual IValidatorGlobalizationService GetValidatorGlobalizationService ()
    {
      return new NullMessageValidatorGlobalizationService();
    }
  }
}