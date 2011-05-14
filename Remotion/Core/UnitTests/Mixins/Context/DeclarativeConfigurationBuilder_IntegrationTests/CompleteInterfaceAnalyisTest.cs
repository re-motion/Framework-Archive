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
using NUnit.Framework;
using Remotion.Mixins.Context;
using Remotion.UnitTests.Mixins.TestDomain;

namespace Remotion.UnitTests.Mixins.Context.DeclarativeConfigurationBuilder_IntegrationTests
{
  [TestFixture]
  public class CompleteInterfaceAnalyisTest
  {
    [Test]
    public void CompleteInterface_ViaAttribute ()
    {
      var result = new DeclarativeConfigurationBuilder (null)
          .AddType (typeof (ClassWithCompleteInterface))
          .AddType (typeof (ClassWithCompleteInterface.ICompleteInterface))
          .BuildConfiguration ();

      var classContext = result.GetContext (typeof (ClassWithCompleteInterface));
      Assert.That (classContext.CompleteInterfaces, Has.Member (typeof (ClassWithCompleteInterface.ICompleteInterface)));
    }

    [Test]
    [Ignore ("TODO 3536")]
    public void CompleteInterface_ViaAttribute_Derived ()
    {
      var result = new DeclarativeConfigurationBuilder (null)
          .AddType (typeof (ClassWithCompleteInterface))
          .AddType (typeof (ClassWithCompleteInterface.ICompleteInterface))
          .AddType (typeof (DerivedClassWithCompleteInterface))
          .BuildConfiguration ();

      var baseClassContext = result.GetContext (typeof (ClassWithCompleteInterface));
      Assert.That (baseClassContext.CompleteInterfaces, Has.Member (typeof (ClassWithCompleteInterface.ICompleteInterface)));

      var derivedClassContext = result.GetContext (typeof (DerivedClassWithCompleteInterface));
      Assert.That (derivedClassContext.CompleteInterfaces, Has.Member (typeof (ClassWithCompleteInterface.ICompleteInterface)));
    }

    [Test]
    public void CompleteInterface_ViaIHasCompleteInterface ()
    {
      var result = new DeclarativeConfigurationBuilder (null).AddType (typeof (ClassWithHasCompleteInterfaces)).BuildConfiguration ();

      var classContext = result.ResolveCompleteInterface (typeof (ClassWithHasCompleteInterfaces.ICompleteInterface1));
      Assert.That (classContext, Is.Not.Null);
      Assert.That (classContext.Type, Is.SameAs (typeof (ClassWithHasCompleteInterfaces)));
      Assert.That (classContext.CompleteInterfaces, Has.Member(typeof (ClassWithHasCompleteInterfaces.ICompleteInterface1)));

      var classContext2 = result.ResolveCompleteInterface (typeof (ClassWithHasCompleteInterfaces.ICompleteInterface2));
      Assert.That (classContext2, Is.SameAs (classContext));
      Assert.That (classContext2.CompleteInterfaces, Has.Member(typeof (ClassWithHasCompleteInterfaces.ICompleteInterface2)));
    }

    [Test]
    public void CompleteInterface_ViaIHasCompleteInterface_ViaGenericBaseClass ()
    {
      var result = new DeclarativeConfigurationBuilder (null).AddType (typeof (ClassDerivedFromBaseClassWithHasComleteInterface)).BuildConfiguration ();

      var classContext = result.GetContext (typeof (ClassDerivedFromBaseClassWithHasComleteInterface));
      Assert.That (classContext.CompleteInterfaces, Has.Member (typeof (ClassDerivedFromBaseClassWithHasComleteInterface.ICompleteInterface)));
    }

    [Test]
    [Ignore ("TODO 3536")]
    public void CompleteInterface_ViaIHasCompleteInterface_Derived ()
    {
      var result = new DeclarativeConfigurationBuilder (null)
          .AddType (typeof (ClassDerivedFromBaseClassWithHasComleteInterface))
          .AddType (typeof (DerivedClassDerivedFromBaseClassWithHasComleteInterface)).BuildConfiguration ();

      var baseClassContext = result.GetContext (typeof (ClassDerivedFromBaseClassWithHasComleteInterface));
      Assert.That (baseClassContext.CompleteInterfaces, Has.Member (typeof (ClassDerivedFromBaseClassWithHasComleteInterface.ICompleteInterface)));

      var derivedClassContext = result.GetContext (typeof (DerivedClassDerivedFromBaseClassWithHasComleteInterface));
      Assert.That (derivedClassContext.CompleteInterfaces, Has.Member (typeof (ClassDerivedFromBaseClassWithHasComleteInterface.ICompleteInterface)));
    }
  }
}