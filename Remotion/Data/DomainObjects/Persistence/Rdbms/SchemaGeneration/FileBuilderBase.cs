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
using System.IO;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Persistence.Configuration;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Persistence.Rdbms.SchemaGeneration
{
  public abstract class FileBuilderBase
  {
    // types

    // static members and constants

    public static void Build (MappingConfiguration mappingConfiguration, StorageConfiguration storageConfiguration, string outputPath)
    {
      ArgumentUtility.CheckNotNull ("mappingConfiguration", mappingConfiguration);
      ArgumentUtility.CheckNotNull ("storageConfiguration", storageConfiguration);
      ArgumentUtility.CheckNotNull ("outputPath", outputPath);

      if (outputPath != string.Empty && !Directory.Exists (outputPath))
        Directory.CreateDirectory (outputPath);

      bool createMultipleFiles = storageConfiguration.StorageProviderDefinitions.Count > 1;
      foreach (StorageProviderDefinition storageProviderDefinition in storageConfiguration.StorageProviderDefinitions)
      {
        var rdbmsProviderDefinition = storageProviderDefinition as RdbmsProviderDefinition;
        if (rdbmsProviderDefinition != null)
          Build (mappingConfiguration, rdbmsProviderDefinition, GetFileName (rdbmsProviderDefinition, outputPath, createMultipleFiles));
      }
    }

    public static void Build (MappingConfiguration mappingConfiguration, RdbmsProviderDefinition rdbmsProviderDefinition, string fileName)
    {
      ArgumentUtility.CheckNotNull ("mappingConfiguration", mappingConfiguration);
      ArgumentUtility.CheckNotNull ("rdbmsProviderDefinition", rdbmsProviderDefinition);

      var classDefinitionsForStorageProvider = GetClassesInStorageProvider (mappingConfiguration.ClassDefinitions, rdbmsProviderDefinition);
      var fileBuilder = rdbmsProviderDefinition.Factory.CreateSchemaFileBuilder(rdbmsProviderDefinition);

      var script = fileBuilder.GetScript (classDefinitionsForStorageProvider);
      File.WriteAllText (fileName, script);
    }

    public static ClassDefinitionCollection GetClassesInStorageProvider (
        ClassDefinitionCollection classDefinitions, RdbmsProviderDefinition rdbmsProviderDefinition)
    {
      var classes = new ClassDefinitionCollection (false);
      foreach (ClassDefinition currentClass in classDefinitions)
      {
        if (currentClass.StorageEntityDefinition.StorageProviderDefinition.Name == rdbmsProviderDefinition.Name)
          classes.Add (currentClass);
      }

      return classes;
    }

    public static string GetFileName (StorageProviderDefinition storageProviderDefinition, string outputPath, bool multipleStorageProviders)
    {
      ArgumentUtility.CheckNotNull ("storageProviderDefinition", storageProviderDefinition);
      ArgumentUtility.CheckNotNull ("outputPath", outputPath);

      string fileName;
      if (multipleStorageProviders)
        fileName = string.Format ("SetupDB_{0}.sql", storageProviderDefinition.Name);
      else
        fileName = "SetupDB.sql";

      return Path.Combine (outputPath, fileName);
    }

    private readonly RdbmsProviderDefinition _rdbmsProviderDefinition;

    protected FileBuilderBase (RdbmsProviderDefinition rdbmsProviderDefinition)
    {
      ArgumentUtility.CheckNotNull ("rdbmsProviderDefinition", rdbmsProviderDefinition);

      _rdbmsProviderDefinition = rdbmsProviderDefinition;
    }

    public RdbmsProviderDefinition RdbmsProviderDefinition
    {
      get { return _rdbmsProviderDefinition; }
    }

    public abstract string GetScript (ClassDefinitionCollection classDefinitions);

    protected void CheckClassDefinitions (ClassDefinitionCollection classDefinitions)
    {
      foreach (ClassDefinition classDefinition in classDefinitions)
      {
        if (classDefinition.StorageEntityDefinition.StorageProviderDefinition.Name != _rdbmsProviderDefinition.Name)
        {
          throw new ArgumentException (
              string.Format (
                  "Class '{0}' has storage provider '{1}' defined, but storage provider '{2}' is required.",
                  classDefinition.ID,
                  classDefinition.StorageEntityDefinition.StorageProviderDefinition.Name,
                  _rdbmsProviderDefinition.Name));
        }
      }
    }
  }
}