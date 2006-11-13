using System;
using System.Collections.Generic;
using System.Text;
using Rubicon.Data.DomainObjects.Mapping;
using Rubicon.Utilities;
using System.Collections;

namespace Rubicon.Data.DomainObjects.CodeGenerator.Sql.SqlServer
{
  public class ConstraintBuilder : ConstraintBuilderBase
  {
    // types

    // static members and constants

    // member fields

    private Hashtable _constraintNamesUsed;

    // construction and disposing

    public ConstraintBuilder ()
    {
      _constraintNamesUsed = new Hashtable ();
    }

    // methods and properties

    public override void AddToDropConstraintScript (List<string> entityNamesForDropConstraintScript, StringBuilder dropConstraintStringBuilder)
    {
      ArgumentUtility.CheckNotNullOrItemsNull ("entityNamesForDropConstraintScript", entityNamesForDropConstraintScript);
      ArgumentUtility.CheckNotNull ("dropConstraintStringBuilder", dropConstraintStringBuilder);

      dropConstraintStringBuilder.AppendFormat ("DECLARE @statement nvarchar (4000)\n"
          + "SET @statement = ''\n"
          + "SELECT @statement = @statement + 'ALTER TABLE [{0}].[' + t.name + '] DROP CONSTRAINT [' + fk.name + ']; ' \n"
          + "    FROM sysobjects fk INNER JOIN sysobjects t ON fk.parent_obj = t.id \n"
          + "    WHERE fk.xtype = 'F' AND t.name IN ('{1}')\n"
          + "    ORDER BY t.name, fk.name\n"
          + "exec sp_executesql @statement\n",
          SqlFileBuilder.DefaultSchema,
          string.Join ("', '", entityNamesForDropConstraintScript.ToArray ()));
    }

    public override void AddToCreateConstraintScript (ClassDefinition classDefinition, StringBuilder createConstraintStringBuilder)
    {
      ArgumentUtility.CheckNotNull ("classDefinition", classDefinition);
      ArgumentUtility.CheckNotNull ("createConstraintStringBuilder", createConstraintStringBuilder);

      string constraints = GetConstraints (classDefinition);
      if (constraints.Length != 0)
      {
        createConstraintStringBuilder.AppendFormat ("ALTER TABLE [{0}].[{1}] ADD\n{2}\n",
            SqlFileBuilder.DefaultSchema,
            classDefinition.MyEntityName,
            constraints);
      }
    }

    public override string GetConstraint (IRelationEndPointDefinition relationEndPoint, PropertyDefinition propertyDefinition, ClassDefinition oppositeClassDefinition)
    {
      ArgumentUtility.CheckNotNull ("relationEndPoint", relationEndPoint);
      ArgumentUtility.CheckNotNull ("propertyDefinition", propertyDefinition);
      ArgumentUtility.CheckNotNull ("oppositeClassDefinition", oppositeClassDefinition);

      return string.Format ("  CONSTRAINT [FK_{0}] FOREIGN KEY ([{1}]) REFERENCES [{2}].[{3}] ([ID])",
          GetUniqueConstraintName (relationEndPoint),
          propertyDefinition.ColumnName,
          SqlFileBuilder.DefaultSchema,
          oppositeClassDefinition.GetEntityName ());
    }


    protected override string ConstraintSeparator
    {
      get { return ",\n"; }
    }

    private string GetUniqueConstraintName (IRelationEndPointDefinition relationEndPoint)
    {
      int i = 1;

      string constraintName = relationEndPoint.RelationDefinition.ID;
      while (_constraintNamesUsed.ContainsKey (constraintName))
      {
        constraintName = string.Format ("{0}_{1}", relationEndPoint.RelationDefinition.ID, i);
        i++;
      }

      _constraintNamesUsed.Add (constraintName, constraintName);
      return constraintName;
    }
  }
}
