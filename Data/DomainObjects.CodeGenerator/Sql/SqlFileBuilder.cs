using System;
using System.Collections.Generic;
using System.Text;
using Rubicon.Data.DomainObjects.Mapping;
using Rubicon.Utilities;
using Rubicon.Data.DomainObjects.Persistence.Configuration;
using Rubicon.Data.DomainObjects.Persistence.Rdbms;

namespace Rubicon.Data.DomainObjects.CodeGenerator.Sql
{
  public class SqlFileBuilder
  {
    // types

    // static members and constants

    private const string s_dropForeignKeyConstraintsFormat = "DECLARE @statement nvarchar (4000)\n"
        + "SET @statement = ''\n"
        + "SELECT @statement = @statement + 'ALTER TABLE [' + t.name + '] DROP CONSTRAINT [' + fk.name + ']; ' \n"
        + "    FROM sysobjects fk INNER JOIN sysobjects t ON fk.parent_obj = t.id \n"
        + "    WHERE fk.xtype = 'F' AND t.name IN ({0})\n"
        + "    ORDER BY t.name, fk.name\n"
        + "exec sp_executesql @statement\n";

    private const string s_dropTableFormat = "IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Tables WHERE TABLE_NAME = '{0}')\n"
          + "  DROP TABLE [{0}]\n";

    private const string s_scriptFormat = "USE {0}\n"
        + "GO\n\n"
        + "-- Drop foreign keys of all tables that will be created below\n"
        + "{1}GO\n\n"
        + "-- Drop all tables that will be created below\n"
        + "{2}GO\n\n"
        + "-- Create all tables\n"
        + "{3}GO\n\n"
        + "-- Create constraints for tables that were created above\n"
        + "{4}GO\n";

    // member fields

    private MappingConfiguration _mappingConfiguration;
    private StorageProviderConfiguration _storageProviderConfiguration;
    private string _storageProviderID;

    private ClassDefinitionCollection _tableRootClasses;

    // construction and disposing

    public SqlFileBuilder (
        MappingConfiguration mappingConfiguration,
        StorageProviderConfiguration storageProviderConfiguration,
        string storageProviderID)
    {
      ArgumentUtility.CheckNotNull ("mappingConfiguration", mappingConfiguration);
      ArgumentUtility.CheckNotNull ("storageProviderConfiguration", storageProviderConfiguration);
      ArgumentUtility.CheckNotNullOrEmpty ("storageProviderID", storageProviderID);

      _mappingConfiguration = mappingConfiguration;
      _storageProviderConfiguration = storageProviderConfiguration;
      _storageProviderID = storageProviderID;
      _tableRootClasses = GetTableRootClassesOfThisStorageProvider ();
    }

    // methods and properties

    public MappingConfiguration MappingConfiguration
    {
      get { return _mappingConfiguration; }
    }

    public StorageProviderConfiguration StorageProviderConfiguration
    {
      get { return _storageProviderConfiguration; }
    }

    public string StorageProviderID
    {
      get { return _storageProviderID; }
    }

    public ClassDefinitionCollection TableRootClasses
    {
      get { return _tableRootClasses; }
    }

    public string GetScript ()
    {
      return string.Format (s_scriptFormat, 
          GetDatabaseName (), GetDropForeignKeyScript (), GetDropTableScript (), GetCreateTableScript (), GetAddConstraintScript ());
    }

    public List<string> GetEntityNames (ClassDefinitionCollection classDefinitions)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("classDefinitions", classDefinitions);

      List<string> entityNames = new List<string> ();

      foreach (ClassDefinition classDefinition in classDefinitions)
      {
        string entityName = classDefinition.GetEntityName ();
        if (entityName != null && !entityNames.Contains (entityName))
          entityNames.Add (entityName);
      }

      return entityNames;
    }

    public string GetDropForeignKeyScript ()
    {
      List<string> entityNames = GetEntityNames (TableRootClasses);

      return string.Format (s_dropForeignKeyConstraintsFormat, "'" + string.Join ("', '", entityNames.ToArray ()) + "'");
    }

    public string GetDatabaseName ()
    {
      RdbmsProviderDefinition provider = (RdbmsProviderDefinition) _storageProviderConfiguration.StorageProviderDefinitions[_storageProviderID];
      return GetDatabasenameFromConnectionString (provider.ConnectionString);
    }

    public string GetAddConstraintScript ()
    {
      StringBuilder addConstraintScriptBuilder = new StringBuilder ();

      foreach (ClassDefinition classDefinition in _tableRootClasses)
      {
        string script = GetAddConstraintScript (classDefinition);
        if (script.Length != 0)
        {
          if (addConstraintScriptBuilder.Length != 0)
            addConstraintScriptBuilder.Append ("\n");

          addConstraintScriptBuilder.Append (script);
        }
      }
      return addConstraintScriptBuilder.ToString ();
    }

    public string GetAddConstraintScript (ClassDefinition classDefinition)
    {
      ArgumentUtility.CheckNotNull ("classDefinition", classDefinition);

      string constraints = string.Empty;
      foreach (IRelationEndPointDefinition relationEndPoint in GetRelationEndPoints (classDefinition))
      {
        string constraint = GetConstraint (relationEndPoint);

        if (constraints.Length != 0 && constraint.Length != 0)
          constraints += ",\n" + constraint;
        else
          constraints += constraint;
      }

      if (constraints == string.Empty)
        return string.Empty;

      return string.Format ("ALTER TABLE [{0}] ADD\n{1}\n", classDefinition.MyEntityName, constraints);
    }

    public string GetConstraint (IRelationEndPointDefinition relationEndPoint)
    {
      ArgumentUtility.CheckNotNull ("relationEndPoint", relationEndPoint);

      if (relationEndPoint.IsNull)
        return string.Empty;

      ClassDefinition oppositeClassDefinition = relationEndPoint.ClassDefinition.GetMandatoryOppositeClassDefinition (relationEndPoint.PropertyName);

      if (!HasConstraint (relationEndPoint, oppositeClassDefinition))
        return string.Empty;

      PropertyDefinition propertyDefinition = relationEndPoint.ClassDefinition.GetMandatoryPropertyDefinition (relationEndPoint.PropertyName);

      return string.Format ("  CONSTRAINT [FK_{0}] FOREIGN KEY ([{1}]) REFERENCES [{2}] ([ID])",
          relationEndPoint.RelationDefinition.ID,
          propertyDefinition.ColumnName,
          oppositeClassDefinition.MyEntityName);
    }

    private List<IRelationEndPointDefinition> GetRelationEndPoints (ClassDefinition classDefinition)
    {
      IRelationEndPointDefinition[] relationEndPointDefinitions = classDefinition.GetRelationEndPointDefinitions ();

      List<IRelationEndPointDefinition> allRelationEndPointDefinitions = new List<IRelationEndPointDefinition> ();
      if (classDefinition.BaseClass != null)
        allRelationEndPointDefinitions.AddRange (classDefinition.BaseClass.GetRelationEndPointDefinitions ());

      FillAllRelationEndPointDefinitions (classDefinition, allRelationEndPointDefinitions);

      return allRelationEndPointDefinitions;
    }

    private void FillAllRelationEndPointDefinitions (ClassDefinition classDefinition, List<IRelationEndPointDefinition> allRelationEndPointDefinitions)
    {
      foreach (RelationDefinition relationDefinition in classDefinition.MyRelationDefinitions)
      {
        foreach (IRelationEndPointDefinition relationEndPointDefinition in relationDefinition.EndPointDefinitions)
        {
          if (relationEndPointDefinition.ClassDefinition == classDefinition)
            allRelationEndPointDefinitions.Add (relationEndPointDefinition);
        }
      }

      foreach (ClassDefinition derivedClass in classDefinition.DerivedClasses)
        FillAllRelationEndPointDefinitions (derivedClass, allRelationEndPointDefinitions);
    }

    private string GetDatabasenameFromConnectionString (string connectionString)
    {
      string temp = connectionString.Substring (connectionString.IndexOf ("Initial Catalog=") + 16);
      return temp.Substring (0, temp.IndexOf (";"));
    }

    private ClassDefinitionCollection GetTableRootClassesOfThisStorageProvider ()
    {
      ClassDefinitionCollection inheritanceRootClasses = MappingConfiguration.ClassDefinitions.GetInheritanceRootClasses ();

      ClassDefinitionCollection allTableRootClasses = new ClassDefinitionCollection (false);
      foreach (ClassDefinition inheritanceRootClass in inheritanceRootClasses)
      {
        if (inheritanceRootClass.StorageProviderID == _storageProviderID)
          FillAllTableRootClasses (inheritanceRootClass, allTableRootClasses);
      }

      return allTableRootClasses;
    }

    private void FillAllTableRootClasses (ClassDefinition classDefinition, ClassDefinitionCollection allTableRootClasses)
    {
      if (classDefinition.GetEntityName () != null)
      {
        allTableRootClasses.Add (classDefinition);
        return;
      }

      foreach (ClassDefinition derivedClass in classDefinition.DerivedClasses)
        FillAllTableRootClasses (derivedClass, allTableRootClasses);
    }

    private bool HasConstraint (IRelationEndPointDefinition relationEndPoint, ClassDefinition oppositeClassDefinition)
    {
      if (relationEndPoint.IsVirtual)
        return false;

      if (oppositeClassDefinition.StorageProviderID != relationEndPoint.ClassDefinition.StorageProviderID)
        return false;

      if (oppositeClassDefinition.GetEntityName () == null)
        return false;

      return true;
    }

    private string GetCreateTableScript ()
    {
      CreateTableBuilder createTableBuilder = new CreateTableBuilder ();

      StringBuilder scriptBuilder = new StringBuilder ();

      foreach (ClassDefinition classDefinition in _tableRootClasses)
      {
        if (scriptBuilder.Length != 0)
          scriptBuilder.Append ("\n");

        scriptBuilder.Append (createTableBuilder.GetCreateTableStatement (classDefinition));
      }

      return scriptBuilder.ToString ();
    }

    public string GetDropTableScript ()
    {
      StringBuilder scriptBuilder = new StringBuilder ();
      foreach (ClassDefinition classDefinition in _tableRootClasses)
      {
        if (scriptBuilder.Length != 0)
          scriptBuilder.Append ("\n");

        scriptBuilder.AppendFormat (s_dropTableFormat, classDefinition.MyEntityName);
      }
      return scriptBuilder.ToString ();
    }
  }
}
