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
using System.Web;
using Microsoft.Practices.ServiceLocation;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.ObjectBinding.Web.UI.Controls.BocListImplementation;
using Remotion.ObjectBinding.Web.UI.Controls.BocListImplementation.Rendering;
using Rhino.Mocks;

namespace Remotion.ObjectBinding.UnitTests.Web.UI.Controls.BocListImplementation.Rendering
{
  [TestFixture]
  public class BocColumnRendererArrayBuilderTest
  {
    private StubColumnDefinition _stubColumnDefinition;
    private IServiceLocator _serviceLocatorStub;
    private HttpContextBase _httpContextStub;
    private IBocList _bocListStub;

    [SetUp]
    public void SetUp ()
    {
      _serviceLocatorStub = MockRepository.GenerateStub<IServiceLocator>();
      _httpContextStub = MockRepository.GenerateStub<HttpContextBase>();
      _bocListStub = MockRepository.GenerateStub<IBocList>();

      _stubColumnDefinition = new StubColumnDefinition ();
    }
    
    [Test]
    public void GetColumnRenderers ()
    {
      var builder = new BocColumnRendererArrayBuilder (new[] { _stubColumnDefinition }, _serviceLocatorStub);

      var bocColumnRenderers = builder.CreateColumnRenderers (_httpContextStub, _bocListStub);

      Assert.That (bocColumnRenderers.Length, Is.EqualTo (1));
      Assert.That (bocColumnRenderers[0].ColumnRenderer, Is.TypeOf(typeof(StubColumnRenderer)));
      Assert.That (bocColumnRenderers[0].ColumnDefinition, Is.SameAs(_stubColumnDefinition));
      Assert.That (bocColumnRenderers[0].ColumnIndex, Is.EqualTo(0));
      Assert.That (((StubColumnRenderer) bocColumnRenderers[0].ColumnRenderer).Context, Is.SameAs(_httpContextStub));
      Assert.That (((StubColumnRenderer) bocColumnRenderers[0].ColumnRenderer).List, Is.SameAs (_bocListStub));
    }
  }
}