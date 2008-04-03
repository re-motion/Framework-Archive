using System;
using NUnit.Framework;
using Remotion.Reflection;
using System.Reflection;

[assembly: Remotion.Core.UnitTests.Reflection.TestMarker]
[assembly: NonApplicationAssembly]

namespace Remotion.Core.UnitTests.Reflection
{
  public class TestMarkerAttribute : Attribute { }

  [TestFixture]
  public class AssemblyFinderFilterTest
  {
    [SetUp]
    public void SetUp ()
    {
      ApplicationAssemblyFinderFilter.Instance.Reset ();
    }

    [TearDown]
    public void TearDown ()
    {
      ApplicationAssemblyFinderFilter.Instance.Reset ();
    }

    [Test]
    public void RegexConsidering_SimpleName ()
    {
      RegexAssemblyFinderFilter filter = new RegexAssemblyFinderFilter ("^Remotion.*$", RegexAssemblyFinderFilter.MatchTargetKind.SimpleName);
      Assert.AreEqual ("^Remotion.*$", filter.MatchExpressionString);
      Assert.IsTrue (filter.ShouldConsiderAssembly (typeof (AssemblyFinderFilterTest).Assembly.GetName ()));
      Assert.IsFalse (filter.ShouldConsiderAssembly (typeof (TestFixtureAttribute).Assembly.GetName ()));
      Assert.IsFalse (filter.ShouldConsiderAssembly (typeof (object).Assembly.GetName ()));
      Assert.IsFalse (filter.ShouldConsiderAssembly (new AssemblyName ("this is not a Remotion assembly")));
    }

    [Test]
    public void RegexConsidering_FullName ()
    {
      RegexAssemblyFinderFilter filter = new RegexAssemblyFinderFilter (typeof (object).Assembly.FullName,
          RegexAssemblyFinderFilter.MatchTargetKind.FullName);
      Assert.IsTrue (filter.MatchExpressionString.StartsWith ("mscorlib"));
      Assert.IsFalse (filter.ShouldConsiderAssembly (typeof (AssemblyFinderFilterTest).Assembly.GetName ()));
      Assert.IsFalse (filter.ShouldConsiderAssembly (typeof (TestFixtureAttribute).Assembly.GetName ()));
      Assert.IsTrue (filter.ShouldConsiderAssembly (typeof (object).Assembly.GetName ()));
      Assert.IsFalse (filter.ShouldConsiderAssembly (new AssemblyName ("this is not mscorlib")));
    }

    [Test]
    public void RegexInclusion_AlwaysTrue ()
    {
      RegexAssemblyFinderFilter filter = new RegexAssemblyFinderFilter ("spispopd", RegexAssemblyFinderFilter.MatchTargetKind.SimpleName);
      Assert.AreEqual ("spispopd", filter.MatchExpressionString);
      Assert.IsTrue (filter.ShouldIncludeAssembly (typeof (AssemblyFinderFilterTest).Assembly));
      Assert.IsTrue (filter.ShouldIncludeAssembly (typeof (TestFixtureAttribute).Assembly));
      Assert.IsTrue (filter.ShouldIncludeAssembly (typeof (object).Assembly));
      Assert.IsTrue (filter.ShouldIncludeAssembly (typeof (Uri).Assembly));
    }

    [Test]
    public void ApplicationAssemblyMatchExpression ()
    {
      ApplicationAssemblyFinderFilter filter = ApplicationAssemblyFinderFilter.Instance;
      Assert.AreEqual (@"^((mscorlib)|(System)|(System\..*)|(Microsoft\..*)|(Remotion\..*\.Generated\..*))$", 
          filter.SystemAssemblyMatchExpression);
    }

    [Test]
    public void ApplicationAssemblyConsidering ()
    {
      ApplicationAssemblyFinderFilter filter = ApplicationAssemblyFinderFilter.Instance;
      Assert.IsTrue (filter.ShouldConsiderAssembly (typeof (AssemblyFinderFilterTest).Assembly.GetName ()));
      Assert.IsTrue (filter.ShouldConsiderAssembly (typeof (TestFixtureAttribute).Assembly.GetName ()));
      Assert.IsTrue (filter.ShouldConsiderAssembly (typeof (AssemblyFinder).Assembly.GetName()));

      Assert.IsFalse (filter.ShouldConsiderAssembly (typeof (object).Assembly.GetName ()));
      Assert.IsFalse (filter.ShouldConsiderAssembly (new AssemblyName ("System")));
      Assert.IsFalse (filter.ShouldConsiderAssembly (new AssemblyName ("Microsoft.Something.Whatever")));
      Assert.IsFalse (filter.ShouldConsiderAssembly (new AssemblyName ("Remotion.Mixins.Generated.Unsigned")));
      Assert.IsFalse (filter.ShouldConsiderAssembly (new AssemblyName ("Remotion.Mixins.Generated.Signed")));
      Assert.IsFalse (filter.ShouldConsiderAssembly (new AssemblyName ("Remotion.Data.DomainObjects.Generated.Signed")));
      Assert.IsFalse (filter.ShouldConsiderAssembly (new AssemblyName ("Remotion.Data.DomainObjects.Generated.Unsigned")));
    }

    [Test]
    public void AddIgnoredAssembly ()
    {
      ApplicationAssemblyFinderFilter filter = ApplicationAssemblyFinderFilter.Instance;
      Assert.IsTrue (filter.ShouldConsiderAssembly (typeof (AssemblyFinder).Assembly.GetName ()));
      filter.AddIgnoredAssembly (typeof (AssemblyFinder).Assembly.GetName().Name);
      Assert.IsFalse (filter.ShouldConsiderAssembly (typeof (AssemblyFinder).Assembly.GetName ()));
    }

    [Test]
    public void ApplicationAssemblyInclusion_DependsOnAttribute ()
    {
      ApplicationAssemblyFinderFilter filter = ApplicationAssemblyFinderFilter.Instance;
      Assert.IsFalse (filter.ShouldIncludeAssembly (typeof (AssemblyFinderFilterTest).Assembly));
      Assert.IsTrue (filter.ShouldIncludeAssembly (typeof (TestFixtureAttribute).Assembly));
      Assert.IsTrue (filter.ShouldIncludeAssembly (typeof (AssemblyFinder).Assembly));
      Assert.IsTrue (filter.ShouldIncludeAssembly (typeof (object).Assembly));
      Assert.IsTrue (filter.ShouldIncludeAssembly (typeof (Uri).Assembly));
    }

    [Test]
    public void AttributeConsidering ()
    {
      AttributeAssemblyFinderFilter filter = new AttributeAssemblyFinderFilter (typeof (SerializableAttribute)); // attribute type doesn't matter here
      Assert.IsTrue (filter.ShouldConsiderAssembly (typeof (AssemblyFinderFilterTest).Assembly.GetName()));
      Assert.IsTrue (filter.ShouldConsiderAssembly (typeof (TestFixtureAttribute).Assembly.GetName()));
      Assert.IsTrue (filter.ShouldConsiderAssembly (typeof (object).Assembly.GetName()));
      Assert.IsTrue (filter.ShouldConsiderAssembly (new AssemblyName ("name does not matter")));
    }

    [Test]
    public void AttributeInclusion ()
    {
      AttributeAssemblyFinderFilter filter = new AttributeAssemblyFinderFilter (typeof (TestMarkerAttribute));
      Assert.IsTrue (filter.ShouldIncludeAssembly (typeof (AssemblyFinderFilterTest).Assembly));
      Assert.IsFalse (filter.ShouldIncludeAssembly (typeof (TestFixtureAttribute).Assembly));
      Assert.IsFalse (filter.ShouldIncludeAssembly (typeof (object).Assembly));
      Assert.IsFalse (filter.ShouldIncludeAssembly (typeof (Uri).Assembly));

      filter = new AttributeAssemblyFinderFilter (typeof (CLSCompliantAttribute));
      Assert.IsTrue (filter.ShouldIncludeAssembly (typeof (AssemblyFinder).Assembly));
      Assert.IsFalse (filter.ShouldIncludeAssembly (typeof (AssemblyFinderFilterTest).Assembly));
      Assert.IsTrue (filter.ShouldIncludeAssembly (typeof (TestFixtureAttribute).Assembly));
      Assert.IsTrue (filter.ShouldIncludeAssembly (typeof (object).Assembly));
      Assert.IsTrue (filter.ShouldIncludeAssembly (typeof (Uri).Assembly));

      filter = new AttributeAssemblyFinderFilter (typeof (SerializableAttribute));
      Assert.IsFalse (filter.ShouldIncludeAssembly (typeof (AssemblyFinderFilterTest).Assembly));
      Assert.IsFalse (filter.ShouldIncludeAssembly (typeof (TestFixtureAttribute).Assembly));
      Assert.IsFalse (filter.ShouldIncludeAssembly (typeof (object).Assembly));
      Assert.IsFalse (filter.ShouldIncludeAssembly (typeof (Uri).Assembly));
    }
  }
}