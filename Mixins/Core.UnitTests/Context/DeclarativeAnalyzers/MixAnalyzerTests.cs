using System;
using NUnit.Framework;
using Rhino.Mocks;
using Rhino.Mocks.Interfaces;
using Remotion.Mixins.Context.DeclarativeAnalyzers;
using Remotion.Mixins.Context.FluentBuilders;

namespace Remotion.Mixins.UnitTests.Context.DeclarativeAnalyzers
{
  [TestFixture]
  public class MixAnalyzerTests
  {
    private MockRepository _mockRepository;
    private MixinConfigurationBuilder _configurationBuilderMock;
    private MixAnalyzer _analyzer;

   [SetUp]
    public void SetUp ()
    {
      _mockRepository = new MockRepository();
      _configurationBuilderMock = _mockRepository.CreateMock<MixinConfigurationBuilder>((MixinConfiguration) null);
      _analyzer = new MixAnalyzer(_configurationBuilderMock);
    }

    [Test]
    public void AnalyzeMixAttribute ()
    {
      MixAttribute attribute = new MixAttribute (typeof (object), typeof (float));
      attribute.SuppressedMixins = new Type[] { typeof (int) };
      attribute.AdditionalDependencies = new Type[] { typeof (string) };

      Expect
          .Call (_configurationBuilderMock.AddMixinToClass (typeof (object), typeof (float), attribute.AdditionalDependencies,
          attribute.SuppressedMixins))
          .Return (null);

      _mockRepository.ReplayAll ();
      _analyzer.AnalyzeMixAttribute (attribute);
      _mockRepository.VerifyAll ();
    }

    [Test]
    [ExpectedException (typeof (ConfigurationException), ExpectedMessage = "Supper?")]
    public void AnalyzeMixAttribute_InvalidOperation ()
    {
      MixAttribute attribute = new MixAttribute (typeof (object), typeof (float));
      attribute.SuppressedMixins = new Type[] { typeof (int) };
      attribute.AdditionalDependencies = new Type[] { typeof (string) };

      Expect
          .Call (
          _configurationBuilderMock.AddMixinToClass (
              typeof (object),
              typeof (float),
              attribute.AdditionalDependencies,
              attribute.SuppressedMixins))
          .Throw (new InvalidOperationException("Supper?"));

      _mockRepository.ReplayAll ();
      _analyzer.AnalyzeMixAttribute (attribute);
    }

    [Test]
    public void Analyze ()
    {
      MixAnalyzer analyzer = _mockRepository.CreateMock<MixAnalyzer> (_configurationBuilderMock);

      analyzer.Analyze (typeof (MixAnalyzerTests).Assembly);
      LastCall.CallOriginalMethod (OriginalCallOptions.CreateExpectation);

      MixAttribute[] attributes =
          (MixAttribute[]) typeof (MixAnalyzerTests).Assembly.GetCustomAttributes (typeof (MixAttribute), false);
      analyzer.AnalyzeMixAttribute (attributes[0]); // expectation
      analyzer.AnalyzeMixAttribute (attributes[1]); // expectation
      analyzer.AnalyzeMixAttribute (attributes[2]); // expectation

      _mockRepository.ReplayAll ();
      analyzer.Analyze (typeof (MixAnalyzerTests).Assembly);
      _mockRepository.VerifyAll ();
    }
  }
}