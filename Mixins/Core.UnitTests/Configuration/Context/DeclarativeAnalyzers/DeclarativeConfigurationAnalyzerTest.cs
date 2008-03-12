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
      Type[] extenders = new Type[] { typeof (object), typeof (string) };
      Type[] users = new Type[] { typeof (int), typeof (double) };
      Type[] completeInterfaces = new Type[] { typeof (IServiceProvider), typeof (ICloneable) };

      using (_mockRepository.Ordered ())
      {
        _extendsAnalyzerMock.Analyze (typeof (object)); // expectation
        _extendsAnalyzerMock.Analyze (typeof (string)); // expectation
        _usesAnalyzerMock.Analyze (typeof (int)); // expectation
        _usesAnalyzerMock.Analyze (typeof (double)); // expectation
        _completeInterfaceAnalyzerMock.Analyze (typeof (IServiceProvider)); // expectation
        _completeInterfaceAnalyzerMock.Analyze (typeof (ICloneable)); // expectation
      }

      _mockRepository.ReplayAll();

      DeclarativeConfigurationAnalyzer analyzer = new DeclarativeConfigurationAnalyzer (_configurationBuilderMock,
          _extendsAnalyzerMock, _usesAnalyzerMock, _completeInterfaceAnalyzerMock);
      analyzer.Analyze (extenders, users, completeInterfaces);

      _mockRepository.VerifyAll();
    }
  }
}