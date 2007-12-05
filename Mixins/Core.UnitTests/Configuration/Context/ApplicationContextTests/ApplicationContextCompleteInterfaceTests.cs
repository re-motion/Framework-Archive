using System;
using System.Collections.Generic;
using Rubicon.Mixins.UnitTests.SampleTypes;
using NUnit.Framework;
using Rubicon.Mixins.Context;

namespace Rubicon.Mixins.UnitTests.Configuration.Context.ApplicationContextTests
{
  [TestFixture]
  public class ApplicationContextCompleteInterfaceTests
  {
    [Test]
    public void RegisterAndResolveCompleteInterface ()
    {
      ApplicationContext ac = new ApplicationContext ();
      ClassContext cc = new ClassContext (typeof (BaseType2));
      ac.AddClassContext (cc);
      cc.AddCompleteInterface (typeof (IBaseType2));
      Assert.IsNull (ac.ResolveInterface (typeof (IBaseType2)));
      ac.RegisterInterface (typeof (IBaseType2), cc);
      Assert.AreSame (cc, ac.ResolveInterface (typeof (IBaseType2)));

      ac.GetOrAddClassContext (typeof (BaseType3));
      ac.RegisterInterface (typeof (IBaseType31), typeof (BaseType3));
      Assert.AreSame (ac.GetClassContext (typeof (BaseType3)), ac.ResolveInterface (typeof (IBaseType31)));
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage = "The argument is not an interface.", MatchType = MessageMatch.Contains)]
    public void RegisterCompleteInterfaceThrowsOnNonInterface ()
    {
      ApplicationContext ac = new ApplicationContext ();
      ClassContext cc = new ClassContext (typeof (BaseType2));
      ac.AddClassContext (cc);
      ac.RegisterInterface (typeof (BaseType2), cc);
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage = "The class context hasn't been added to this application context.",
        MatchType = MessageMatch.Contains)]
    public void RegisterCompleteInterfaceThrowsOnInvalidClassContext1 ()
    {
      ApplicationContext ac = new ApplicationContext ();
      ClassContext cc = new ClassContext (typeof (BaseType2));
      ac.RegisterInterface (typeof (IBaseType2), cc);
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage = "The class context hasn't been added to this application context.",
        MatchType = MessageMatch.Contains)]
    public void RegisterCompleteInterfaceThrowsOnInvalidClassContext2 ()
    {
      ApplicationContext ac = new ApplicationContext ();
      ClassContext cc = new ClassContext (typeof (BaseType2));
      ac.AddClassContext (cc);
      ac.RegisterInterface (typeof (IBaseType2), new ClassContext (typeof (BaseType2)));
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage = "There is no class context for the given type",
        MatchType = MessageMatch.Contains)]
    public void RegisterCompleteInterfaceThrowsOnInvalidClassContext3 ()
    {
      ApplicationContext ac = new ApplicationContext ();
      ac.RegisterInterface (typeof (IBaseType2), typeof (BaseType2));
    }

    [Test]
    public void RemoveClassContextCausesInterfaceRegistrationToBeRemoved ()
    {
      ApplicationContext ac = new ApplicationContext ();
      ClassContext cc = new ClassContext (typeof (BaseType2));
      ac.AddClassContext (cc);
      ac.RegisterInterface (typeof (IBaseType2), cc);
      Assert.IsNotNull (ac.ResolveInterface (typeof (IBaseType2)));
      Assert.AreSame (cc, ac.ResolveInterface (typeof (IBaseType2)));
      ac.RemoveClassContext (typeof (BaseType2));
      Assert.IsNull (ac.ResolveInterface (typeof (IBaseType2)));
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "already been associated with a class context",
        MatchType = MessageMatch.Contains)]
    public void RegisterCompleteInterfaceThrowsOnDuplicateInterface ()
    {
      ApplicationContext ac = new ApplicationContext ();
      ClassContext cc = new ClassContext (typeof (BaseType2));
      ac.AddClassContext (cc);
      ac.RegisterInterface (typeof (IBaseType2), cc);
      ac.RegisterInterface (typeof (IBaseType2), cc);
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage = "The argument is not an interface.", MatchType = MessageMatch.Contains)]
    public void ResolveCompleteInterfaceThrowsOnNonInterface ()
    {
      ApplicationContext ac = new ApplicationContext ();
      ac.ResolveInterface (typeof (BaseType2));
    }
  }
}