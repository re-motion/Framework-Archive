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
using System.Data;
using NUnit.Framework;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Persistence.Rdbms;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model.Building;
using Remotion.Data.UnitTests.DomainObjects.Core.TableInheritance.TestDomain;
using Remotion.Data.UnitTests.DomainObjects.Factories;
using Rhino.Mocks;

namespace Remotion.Data.UnitTests.DomainObjects.Core.TableInheritance
{
  [TestFixture]
  public class OppositeClassDefinitionRetrieverTest : SqlProviderBaseTest
  {
    private MockRepository _mockRepository;
    private IDataReader _readerMock;
    private ClassDefinition _regionDefinition;
    private ClassDefinition _customerDefinition;
    private ClassDefinition _fileSystemItemDefinition;
    private ClassDefinition _folderDefinition;
    private ClassDefinition _specificFolderDefinition;
    private PropertyDefinition _parentFolderPropertyDefinition;
    private PropertyDefinition _regionPropertyDefinition;

    public override void SetUp ()
    {
      base.SetUp ();

      _mockRepository = new MockRepository ();
      _readerMock = _mockRepository.StrictMock<IDataReader> ();

      _regionDefinition = MappingConfiguration.Current.GetTypeDefinition (typeof (Region));
      _customerDefinition = MappingConfiguration.Current.GetTypeDefinition (typeof (Customer));
      _fileSystemItemDefinition = MappingConfiguration.Current.GetTypeDefinition (typeof (FileSystemItem));
      _folderDefinition = MappingConfiguration.Current.GetTypeDefinition (typeof (Folder));
      _specificFolderDefinition = MappingConfiguration.Current.GetTypeDefinition (typeof (SpecificFolder));

      _parentFolderPropertyDefinition = _fileSystemItemDefinition.GetMandatoryPropertyDefinition (ReflectionMappingHelper.GetPropertyName (typeof (FileSystemItem), "ParentFolder"));
      _regionPropertyDefinition = _customerDefinition.GetMandatoryPropertyDefinition (ReflectionMappingHelper.GetPropertyName (typeof (Customer), "Region"));

      OppositeClassDefinitionRetriever.ResetCache ();
    }

    public override void TearDown ()
    {
      OppositeClassDefinitionRetriever.ResetCache();
      base.TearDown();
    }

    [Test]
    public void GetMandatoryOppositeClassDefinition_InheritanceHierarchy_NullClassID ()
    {
      OppositeClassDefinitionRetriever retriever = new OppositeClassDefinitionRetriever(_fileSystemItemDefinition, _parentFolderPropertyDefinition, new ReflectionBasedStorageNameProvider());
      SetupReaderForInheritanceHierarchy(true, true);
      _mockRepository.ReplayAll();

      ClassDefinition opposite = retriever.GetMandatoryOppositeClassDefinition (_readerMock, 0);
      Assert.AreSame (_folderDefinition, opposite);

      _mockRepository.VerifyAll ();
    }

    [Test]
    [ExpectedException (typeof (RdbmsProviderException), ExpectedMessage = "Incorrect database value encountered. Column 'ParentFolderIDClassID' of "
        + "entity '' must not contain null.")]
    public void GetMandatoryOppositeClassDefinition_InheritanceHierarchy_NullClassID_NonNullObjectID ()
    {
      OppositeClassDefinitionRetriever retriever = new OppositeClassDefinitionRetriever (_fileSystemItemDefinition, _parentFolderPropertyDefinition, new ReflectionBasedStorageNameProvider());
      SetupReaderForInheritanceHierarchy (false, true);
      _mockRepository.ReplayAll ();

      retriever.GetMandatoryOppositeClassDefinition (_readerMock, 0);
    }

    [Test]
    public void GetMandatoryOppositeClassDefinition_InheritanceHierarchy_NonNullClassID ()
    {
      OppositeClassDefinitionRetriever retriever = new OppositeClassDefinitionRetriever (_fileSystemItemDefinition, _parentFolderPropertyDefinition, new ReflectionBasedStorageNameProvider());
      SetupReaderForInheritanceHierarchy (false, false);

      _mockRepository.ReplayAll ();

      ClassDefinition opposite = retriever.GetMandatoryOppositeClassDefinition (_readerMock, 0);
      Assert.AreSame (_specificFolderDefinition, opposite);

      _mockRepository.VerifyAll ();
    }

    [Test]
    [ExpectedException (typeof (RdbmsProviderException), ExpectedMessage = "Incorrect database value encountered. Column 'ParentFolderIDClassID' of "
        + "entity '' must not contain a value.")]
    public void GetMandatoryOppositeClassDefinition_InheritanceHierarchy_NonNullClassID_NullObjectID ()
    {
      OppositeClassDefinitionRetriever retriever = new OppositeClassDefinitionRetriever (_fileSystemItemDefinition, _parentFolderPropertyDefinition, new ReflectionBasedStorageNameProvider());
      SetupReaderForInheritanceHierarchy (true, false);
      _mockRepository.ReplayAll ();

      retriever.GetMandatoryOppositeClassDefinition (_readerMock, 0);
    }

    [Test]
    [ExpectedException (typeof (RdbmsProviderException), ExpectedMessage = "Incorrect database format encountered. Entity '' must have column "
        + "'ParentFolderIDClassID' defined, because opposite class 'TI_Folder' is part of an inheritance hierarchy.")]
    public void GetMandatoryOppositeClassDefinition_NoOrdinal_DataReaderThrowsIndexOutOfRangeException ()
    {
      OppositeClassDefinitionRetriever retriever = new OppositeClassDefinitionRetriever (_fileSystemItemDefinition, _parentFolderPropertyDefinition, new ReflectionBasedStorageNameProvider());
      Expect.Call (_readerMock.GetOrdinal ("ParentFolderIDClassID")).Throw (new IndexOutOfRangeException ());
      _mockRepository.ReplayAll ();

      retriever.GetMandatoryOppositeClassDefinition (_readerMock, 0);
    }

    [Test]
    [ExpectedException (typeof (RdbmsProviderException), ExpectedMessage = "Incorrect database format encountered. Entity '' must have column "
        + "'ParentFolderIDClassID' defined, because opposite class 'TI_Folder' is part of an inheritance hierarchy.")]
    public void GetMandatoryOppositeClassDefinition_NoOrdinal_DataReaderReturnsMinusOne ()
    {
      OppositeClassDefinitionRetriever retriever = new OppositeClassDefinitionRetriever (_fileSystemItemDefinition, _parentFolderPropertyDefinition, new ReflectionBasedStorageNameProvider());
      Expect.Call (_readerMock.GetOrdinal ("ParentFolderIDClassID")).Return (-1);
      _mockRepository.ReplayAll ();

      retriever.GetMandatoryOppositeClassDefinition (_readerMock, 0);
    }

    [Test]
    public void GetMandatoryOppositeClassDefinition_NoInheritanceHierarchy_NoOrdinal_DataReaderThrowsIndexOutOfRangeException ()
    {
      OppositeClassDefinitionRetriever retriever = new OppositeClassDefinitionRetriever (_customerDefinition, _regionPropertyDefinition, new ReflectionBasedStorageNameProvider());
      Expect.Call (_readerMock.GetOrdinal ("RegionIDClassID")).Throw (new IndexOutOfRangeException());
      _mockRepository.ReplayAll ();

      ClassDefinition opposite = retriever.GetMandatoryOppositeClassDefinition (_readerMock, 0);
      Assert.AreSame (_regionDefinition, opposite);

      _mockRepository.VerifyAll ();
    }

    [Test]
    public void GetMandatoryOppositeClassDefinition_NoInheritanceHierarchy_NoOrdinal_DataReaderReturnsMinusOne ()
    {
      OppositeClassDefinitionRetriever retriever = new OppositeClassDefinitionRetriever (_customerDefinition, _regionPropertyDefinition, new ReflectionBasedStorageNameProvider());
      Expect.Call (_readerMock.GetOrdinal ("RegionIDClassID")).Return (-1);
      _mockRepository.ReplayAll ();

      ClassDefinition opposite = retriever.GetMandatoryOppositeClassDefinition (_readerMock, 0);
      Assert.AreSame (_regionDefinition, opposite);

      _mockRepository.VerifyAll ();
    }

    [Test]
    public void GetMandatoryOppositeClassDefinition_NoInheritanceHierarchy_NoOrdinal_CheckCache_DataReaderThrowsIndexOutOfRangeException ()
    {
      OppositeClassDefinitionRetriever retriever1 = new OppositeClassDefinitionRetriever (_customerDefinition, _regionPropertyDefinition, new ReflectionBasedStorageNameProvider());
      OppositeClassDefinitionRetriever retriever2 = new OppositeClassDefinitionRetriever (_customerDefinition, _regionPropertyDefinition, new ReflectionBasedStorageNameProvider());
      Expect.Call (_readerMock.GetOrdinal ("RegionIDClassID")).Throw (new IndexOutOfRangeException ());
      _mockRepository.ReplayAll ();

      ClassDefinition opposite1 = retriever1.GetMandatoryOppositeClassDefinition (_readerMock, 0);
      ClassDefinition opposite2 = retriever1.GetMandatoryOppositeClassDefinition (_readerMock, 0);
      ClassDefinition opposite3 = retriever2.GetMandatoryOppositeClassDefinition (_readerMock, 0);
      Assert.AreSame (_regionDefinition, opposite1);
      Assert.AreSame (_regionDefinition, opposite2);
      Assert.AreSame (_regionDefinition, opposite3);

      _mockRepository.VerifyAll ();
    }

    [Test]
    public void GetMandatoryOppositeClassDefinition_NoInheritanceHierarchy_NoOrdinal_CheckCache_DataReaderReturnsMinusOne ()
    {
      OppositeClassDefinitionRetriever retriever1 = new OppositeClassDefinitionRetriever (_customerDefinition, _regionPropertyDefinition, new ReflectionBasedStorageNameProvider());
      OppositeClassDefinitionRetriever retriever2 = new OppositeClassDefinitionRetriever (_customerDefinition, _regionPropertyDefinition, new ReflectionBasedStorageNameProvider());
      Expect.Call (_readerMock.GetOrdinal ("RegionIDClassID")).Return (-1);
      _mockRepository.ReplayAll ();

      ClassDefinition opposite1 = retriever1.GetMandatoryOppositeClassDefinition (_readerMock, 0);
      ClassDefinition opposite2 = retriever1.GetMandatoryOppositeClassDefinition (_readerMock, 0);
      ClassDefinition opposite3 = retriever2.GetMandatoryOppositeClassDefinition (_readerMock, 0);
      Assert.AreSame (_regionDefinition, opposite1);
      Assert.AreSame (_regionDefinition, opposite2);
      Assert.AreSame (_regionDefinition, opposite3);

      _mockRepository.VerifyAll ();
    }

    [Test]
    [ExpectedException (typeof (RdbmsProviderException), ExpectedMessage = "Incorrect database format encountered. Entity 'TableInheritance_Person' "
        + "must not contain column 'RegionIDClassID', because opposite class 'TI_Region' is not part of an inheritance hierarchy.")]
    public void GetMandatoryOppositeClassDefinition_NoInheritanceHierarchy_WithOrdinal ()
    {
      OppositeClassDefinitionRetriever retriever = new OppositeClassDefinitionRetriever (_customerDefinition, _regionPropertyDefinition, new ReflectionBasedStorageNameProvider());
      Expect.Call (_readerMock.GetOrdinal ("RegionIDClassID")).Return (1);
      _mockRepository.ReplayAll ();

      retriever.GetMandatoryOppositeClassDefinition (_readerMock, 0);
    }

    [Test]
    public void GetMandatoryOppositeClassDefinition_NoInheritanceHierarchy_WithOrdinal_CheckCache ()
    {
      OppositeClassDefinitionRetriever retriever1 = new OppositeClassDefinitionRetriever (_customerDefinition, _regionPropertyDefinition, new ReflectionBasedStorageNameProvider());
      OppositeClassDefinitionRetriever retriever2 = new OppositeClassDefinitionRetriever (_customerDefinition, _regionPropertyDefinition, new ReflectionBasedStorageNameProvider());
      Expect.Call (_readerMock.GetOrdinal ("RegionIDClassID")).Return (1);
      _mockRepository.ReplayAll ();

      CheckThrow (retriever1);
      CheckThrow (retriever1);
      CheckThrow (retriever2);

      _mockRepository.VerifyAll ();
    }

    private void CheckThrow (OppositeClassDefinitionRetriever retriever1)
    {
      try
      {
        retriever1.GetMandatoryOppositeClassDefinition (_readerMock, 0);
        Assert.Fail ("Expected RdbmsProviderException");
      }
      catch (RdbmsProviderException)
      {
      }
    }

    private void SetupReaderForInheritanceHierarchy (bool objectIDNull, bool classIDNull)
    {
      using (_mockRepository.Ordered ())
      {
        Expect.Call (_readerMock.GetOrdinal ("ParentFolderIDClassID")).Return (1);

        // consistency checks
        Expect.Call (_readerMock.IsDBNull (0)).Return (objectIDNull);
        if (objectIDNull)
          Expect.Call (_readerMock.IsDBNull (1)).Return (classIDNull);

        Expect.Call (_readerMock.IsDBNull (0)).Return (objectIDNull);
        if (!objectIDNull)
          Expect.Call (_readerMock.IsDBNull (1)).Return (classIDNull);

        // decision about whether to return the default one
        Expect.Call (_readerMock.IsDBNull (1)).Return (classIDNull);

        if (!classIDNull)
          Expect.Call (_readerMock.GetString (1)).Return (_specificFolderDefinition.ID);
      }
    }    

  }
}
