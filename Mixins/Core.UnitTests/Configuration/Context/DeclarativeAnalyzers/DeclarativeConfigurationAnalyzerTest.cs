using System;
using NUnit.Framework;
using Rhino.Mocks;
using Rubicon.Mixins.Context.DeclarativeAnalyzers;
using Rubicon.Mixins.Context.FluentBuilders;

namespace Rubicon.Mixins.UnitTests.Configuration.Context.DeclarativeAnalyzers
{
  [TestFixture]
  public class DeclarativeConfigurationAnalyzerTest
  {
    private MockRepository _mockRepository;
    private MixinConfigurationBuilder _configurationBuilderMock;
    private ExtendsAnalyzer _extendsAnalyzerMock;
    private UsesAnalyzer _usesAnalyzerMock;
    private CompleteInterfaceAnalyzer _completeInterfaceAnalyzerMock;

    [SetUp]
    public void SetUp ()
    {
      _mockRepository = new MockRepository();
      _configurationBuilderMock = _mockRepository.CreateMock<MixinConfigurationBuilder> ((MixinConfiguration) null);

      _extendsAnalyzerMock = _mockRepository.CreateMock<ExtendsAnalyzer> (_configurationBuilderMock);
      _usesAnalyzerMock = _mockRepository.CreateMock<UsesAnalyzer> (_configurationBuilderMock);
      _completeInterfaceAnalyzerMock = _mockRepository.CreateMock<CompleteInterfaceAnalyzer> (_configurationBuilderMock);
    }

    [Test]
    public void Analyze ()
    {
      Type[] types = new Type[] { typeof (object), typeof (string) };

      using (_mockRepository.Ordered ())
      {
        _extendsAnalyzerMock.Analyze (typeof (object)); // expectation
        _extendsAnalyzerMock.Analyze (typeof (string)); // expectation
        _usesAnalyzerMock.Analyze (typeof (object)); // expectation
        _usesAnalyzerMock.Analyze (typeof (string)); // expectation
        _completeInterfaceAnalyzerMock.Analyze (typeof (object)); // expectation
        _completeInterfaceAnalyzerMock.Analyze (typeof (string)); // expectation
      }

      _mockRepository.ReplayAll();

      DeclarativeConfigurationAnalyzer analyzer = new DeclarativeConfigurationAnalyzer (_configurationBuilderMock,
          _extendsAnalyzerMock, _usesAnalyzerMock, _completeInterfaceAnalyzerMock);
      analyzer.Analyze (types);

      _mockRepository.VerifyAll();
    }
  }
}