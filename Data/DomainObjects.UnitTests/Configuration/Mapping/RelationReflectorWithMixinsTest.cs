using System;
using System.Collections.Generic;
using System.Reflection;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Rubicon.Data.DomainObjects.ConfigurationLoader.ReflectionBasedConfigurationLoader;
using Rubicon.Data.DomainObjects.Mapping;
using Rubicon.Data.DomainObjects.UnitTests.MixedDomains.SampleTypes;
using Rubicon.Data.DomainObjects.UnitTests.TestDomain.ReflectionBasedMappingSample;

namespace Rubicon.Data.DomainObjects.UnitTests.Configuration.Mapping
{
  [TestFixture]
  public class RelationReflectorWithMixinsTest : StandardMappingTest
  {
    private ReflectionBasedClassDefinition _mixinTargetClassDefinition;
    private ReflectionBasedClassDefinition _relatedClassDefinition;
    private ClassDefinitionCollection _classDefinitions;
    private RelationDefinitionCollection _relationDefinitions;

    public override void SetUp ()
    {
      base.SetUp();

      _mixinTargetClassDefinition = 
          CreateReflectionBasedClassDefinition (typeof (TargetClassForPersistentMixin), typeof (MixinAddingPersistentProperties));
      _relatedClassDefinition = 
          CreateReflectionBasedClassDefinition (typeof (RelationTargetForPersistentMixin), typeof (MixinAddingPersistentProperties));

      _classDefinitions = new ClassDefinitionCollection();
      _classDefinitions.Add (_mixinTargetClassDefinition);
      _classDefinitions.Add (_relatedClassDefinition);

      _relationDefinitions = new RelationDefinitionCollection();
    }

    [Test]
    public void GetMetadata_Mixed_Unidirectional ()
    {
      PropertyInfo propertyInfo = typeof (MixinAddingPersistentProperties).GetProperty ("UnidirectionalRelationProperty");
      PropertyReflector propertyReflector = new PropertyReflector (_mixinTargetClassDefinition, propertyInfo);
      PropertyDefinition propertyDefinition = propertyReflector.GetMetadata ();
      _mixinTargetClassDefinition.MyPropertyDefinitions.Add (propertyDefinition);
      RelationReflector relationReflector = new RelationReflector (_mixinTargetClassDefinition, propertyInfo);

      RelationDefinition actualRelationDefinition = relationReflector.GetMetadata (_classDefinitions, _relationDefinitions);

      Assert.AreEqual (
          "Rubicon.Data.DomainObjects.UnitTests.MixedDomains.SampleTypes.MixinAddingPersistentProperties.UnidirectionalRelationProperty",
          actualRelationDefinition.ID);

      Assert.IsInstanceOfType (typeof (RelationEndPointDefinition), actualRelationDefinition.EndPointDefinitions[0]);
      RelationEndPointDefinition endPointDefinition = (RelationEndPointDefinition) actualRelationDefinition.EndPointDefinitions[0];
      Assert.AreEqual (propertyDefinition, endPointDefinition.PropertyDefinition);
      Assert.AreSame (_mixinTargetClassDefinition, endPointDefinition.ClassDefinition);
      Assert.AreSame (actualRelationDefinition, endPointDefinition.RelationDefinition);
      Assert.That (_mixinTargetClassDefinition.MyRelationDefinitions, List.Contains (actualRelationDefinition));

      Assert.IsInstanceOfType (typeof (AnonymousRelationEndPointDefinition), actualRelationDefinition.EndPointDefinitions[1]);
      AnonymousRelationEndPointDefinition oppositeEndPointDefinition =
          (AnonymousRelationEndPointDefinition) actualRelationDefinition.EndPointDefinitions[1];
      Assert.AreSame (_relatedClassDefinition, oppositeEndPointDefinition.ClassDefinition);
      Assert.AreSame (actualRelationDefinition, oppositeEndPointDefinition.RelationDefinition);
      Assert.That (_relatedClassDefinition.MyRelationDefinitions, List.Not.Contains (actualRelationDefinition));

      Assert.That (_relationDefinitions.Count, Is.EqualTo (1));
      Assert.That (_relationDefinitions, List.Contains (actualRelationDefinition));
    }

    [Test]
    public void GetMetadata_Mixed_BidirectionalOneToOne ()
    {
      PropertyInfo propertyInfo = typeof (MixinAddingPersistentProperties).GetProperty ("RelationProperty");
      PropertyReflector propertyReflector = new PropertyReflector (_mixinTargetClassDefinition, propertyInfo);
      PropertyDefinition propertyDefinition = propertyReflector.GetMetadata ();
      _mixinTargetClassDefinition.MyPropertyDefinitions.Add (propertyDefinition);
      RelationReflector relationReflector = new RelationReflector (_mixinTargetClassDefinition, propertyInfo);

      RelationDefinition actualRelationDefinition = relationReflector.GetMetadata (_classDefinitions, _relationDefinitions);

      Assert.AreEqual (
          "Rubicon.Data.DomainObjects.UnitTests.MixedDomains.SampleTypes.MixinAddingPersistentProperties.RelationProperty",
          actualRelationDefinition.ID);

      Assert.IsInstanceOfType (typeof (RelationEndPointDefinition), actualRelationDefinition.EndPointDefinitions[0]);
      RelationEndPointDefinition endPointDefinition = (RelationEndPointDefinition) actualRelationDefinition.EndPointDefinitions[0];
      Assert.AreEqual (propertyDefinition, endPointDefinition.PropertyDefinition);
      Assert.AreSame (_mixinTargetClassDefinition, endPointDefinition.ClassDefinition);
      Assert.AreSame (actualRelationDefinition, endPointDefinition.RelationDefinition);
      Assert.That (_mixinTargetClassDefinition.MyRelationDefinitions, List.Contains (actualRelationDefinition));

      Assert.IsInstanceOfType (typeof (VirtualRelationEndPointDefinition), actualRelationDefinition.EndPointDefinitions[1]);
      VirtualRelationEndPointDefinition oppositeEndPointDefinition =
          (VirtualRelationEndPointDefinition) actualRelationDefinition.EndPointDefinitions[1];
      Assert.AreSame (_relatedClassDefinition, oppositeEndPointDefinition.ClassDefinition);
      Assert.AreEqual (
          "Rubicon.Data.DomainObjects.UnitTests.MixedDomains.SampleTypes.RelationTargetForPersistentMixin.RelationProperty1",
          oppositeEndPointDefinition.PropertyName);
      Assert.AreSame (typeof (TargetClassForPersistentMixin), oppositeEndPointDefinition.PropertyType);
      Assert.AreSame (actualRelationDefinition, oppositeEndPointDefinition.RelationDefinition);
      Assert.That (_relatedClassDefinition.MyRelationDefinitions, List.Contains (actualRelationDefinition));

      Assert.That (_relationDefinitions.Count, Is.EqualTo (1));
      Assert.That (_relationDefinitions, List.Contains (actualRelationDefinition));
    }

    [Test]
    public void GetMetadata_Mixed_BidirectionalOneToOne_VirtualEndPoint ()
    {
      PropertyInfo propertyInfo = typeof (MixinAddingPersistentProperties).GetProperty ("VirtualRelationProperty");
      RelationReflector relationReflector = new RelationReflector (_mixinTargetClassDefinition, propertyInfo);

      Assert.IsNull (relationReflector.GetMetadata (_classDefinitions, _relationDefinitions));
      Assert.That (_relationDefinitions.Count, Is.EqualTo (0));
    }

    [Test]
    public void GetMetadata_Mixed_BidirectionalOneToMany ()
    {
      PropertyInfo propertyInfo = typeof (MixinAddingPersistentProperties).GetProperty ("CollectionPropertyNSide");
      PropertyReflector propertyReflector = new PropertyReflector (_mixinTargetClassDefinition, propertyInfo);
      PropertyDefinition propertyDefinition = propertyReflector.GetMetadata ();
      _mixinTargetClassDefinition.MyPropertyDefinitions.Add (propertyDefinition);
      RelationReflector relationReflector = new RelationReflector (_mixinTargetClassDefinition, propertyInfo);

      RelationDefinition actualRelationDefinition = relationReflector.GetMetadata (_classDefinitions, _relationDefinitions);

      Assert.AreEqual (
          "Rubicon.Data.DomainObjects.UnitTests.MixedDomains.SampleTypes.MixinAddingPersistentProperties.CollectionPropertyNSide",
          actualRelationDefinition.ID);

      Assert.IsInstanceOfType (typeof (RelationEndPointDefinition), actualRelationDefinition.EndPointDefinitions[0]);
      RelationEndPointDefinition endPointDefinition = (RelationEndPointDefinition) actualRelationDefinition.EndPointDefinitions[0];
      Assert.AreEqual (propertyDefinition, endPointDefinition.PropertyDefinition);
      Assert.AreSame (_mixinTargetClassDefinition, endPointDefinition.ClassDefinition);
      Assert.AreSame (actualRelationDefinition, endPointDefinition.RelationDefinition);
      Assert.That (_mixinTargetClassDefinition.MyRelationDefinitions, List.Contains (actualRelationDefinition));

      Assert.IsInstanceOfType (typeof (VirtualRelationEndPointDefinition), actualRelationDefinition.EndPointDefinitions[1]);
      VirtualRelationEndPointDefinition oppositeEndPointDefinition =
          (VirtualRelationEndPointDefinition) actualRelationDefinition.EndPointDefinitions[1];
      Assert.AreSame (_relatedClassDefinition, oppositeEndPointDefinition.ClassDefinition);
      Assert.AreEqual (
          "Rubicon.Data.DomainObjects.UnitTests.MixedDomains.SampleTypes.RelationTargetForPersistentMixin.RelationProperty4",
          oppositeEndPointDefinition.PropertyName);
      Assert.AreSame (typeof (ObjectList<TargetClassForPersistentMixin>), oppositeEndPointDefinition.PropertyType);
      Assert.AreSame (actualRelationDefinition, oppositeEndPointDefinition.RelationDefinition);
      Assert.That (_relatedClassDefinition.MyRelationDefinitions, List.Contains (actualRelationDefinition));

      Assert.That (_relationDefinitions.Count, Is.EqualTo (1));
      Assert.That (_relationDefinitions, List.Contains (actualRelationDefinition));
    }

    [Test]
    public void GetMetadata_Mixed_BidirectionalOneToMany_VirtualEndPoint ()
    {
      PropertyInfo propertyInfo = typeof (MixinAddingPersistentProperties).GetProperty ("CollectionProperty1Side");
      RelationReflector relationReflector = new RelationReflector (_mixinTargetClassDefinition, propertyInfo);

      Assert.IsNull (relationReflector.GetMetadata (_classDefinitions, _relationDefinitions));
      Assert.That (_relationDefinitions.Count, Is.EqualTo (0));
    }

    private ReflectionBasedClassDefinition CreateReflectionBasedClassDefinition (Type type, params Type[] mixins)
    {
      return new ReflectionBasedClassDefinition (type.Name, type.Name, "TestDomain", type, false, new List<Type> (mixins));
    }
  }
}