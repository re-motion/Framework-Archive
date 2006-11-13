using System;
using System.Collections.Generic;
using System.Text;
using Rubicon.Data.DomainObjects.Mapping;
using Rubicon.Utilities;
using Rubicon.Data.DomainObjects.Persistence.Rdbms;

namespace Rubicon.Data.DomainObjects.CodeGenerator.Sql.SqlServer
{
  public class ViewBuilder : ViewBuilderBase
  {
    // types

    // static members and constants

    // member fields

    // construction and disposing

    public ViewBuilder ()
    {
    }

    // methods and properties

    public override string CreateViewSeparator
    {
      get { return "GO\n\n"; }
    }

    public override void AddViewForConcreteClassToCreateViewScript (ClassDefinition classDefinition, StringBuilder createViewStringBuilder)
    {
      ArgumentUtility.CheckNotNull ("classDefinition", classDefinition);
      ArgumentUtility.CheckNotNull ("createViewStringBuilder", createViewStringBuilder); 

      createViewStringBuilder.AppendFormat (
          "CREATE VIEW [{0}].[{1}] ([ID], [ClassID], [Timestamp], {2})\n"
          + "  WITH SCHEMABINDING AS\n"
          + "  SELECT [ID], [ClassID], [Timestamp], {2}\n"
          + "    FROM [{0}].[{3}]\n"
          + "    WHERE [ClassID] IN ({4})\n"
          + "  WITH CHECK OPTION\n",
          SqlFileBuilder.DefaultSchema,
          GetViewName (classDefinition),
          GetColumnList (GetAllPropertyDefinitions (classDefinition)),
          classDefinition.GetEntityName (),
          GetClassIDList (GetClassDefinitionsForWhereClause (classDefinition)));
    }

    public override void AddViewForAbstractClassToCreateViewScript (
        ClassDefinition classDefinition, 
        ClassDefinitionCollection concreteClasses, 
        StringBuilder createViewStringBuilder)
    {
      ArgumentUtility.CheckNotNull ("classDefinition", classDefinition);
      ArgumentUtility.CheckNotNullOrEmpty ("concreteClasses", concreteClasses);
      ArgumentUtility.CheckNotNull ("createViewStringBuilder", createViewStringBuilder);

      List<PropertyDefinition> allPropertyDefinitions = GetAllPropertyDefinitions (classDefinition);
      string classIDListForWhereClause = GetClassIDList (GetClassDefinitionsForWhereClause (classDefinition));

      createViewStringBuilder.AppendFormat (
          "CREATE VIEW [{0}].[{1}] ([ID], [ClassID], [Timestamp], {2})\n"
          + "  WITH SCHEMABINDING AS\n",
          SqlFileBuilder.DefaultSchema,
          GetViewName (classDefinition),
          GetColumnList (allPropertyDefinitions));

      int numberOfSelects = 0;
      foreach (ClassDefinition tableRootClass in concreteClasses)
      {
        if (numberOfSelects > 0)
          createViewStringBuilder.AppendFormat ("  UNION ALL\n");

        createViewStringBuilder.AppendFormat (
            "  SELECT [ID], [ClassID], [Timestamp], {0}\n"
            + "    FROM [{1}].[{2}]\n"
            + "    WHERE [ClassID] IN ({3})\n",
            GetColumnListForUnionSelect (tableRootClass, allPropertyDefinitions),
            SqlFileBuilder.DefaultSchema,
            tableRootClass.MyEntityName,
            classIDListForWhereClause);

        numberOfSelects++;
      }
      if (numberOfSelects == 1)
        createViewStringBuilder.Append ("  WITH CHECK OPTION\n");
    }

    public override void AddToDropViewScript (ClassDefinition classDefinition, StringBuilder dropViewStringBuilder)
    {
      ArgumentUtility.CheckNotNull ("classDefinition", classDefinition);
      ArgumentUtility.CheckNotNull ("dropViewStringBuilder", dropViewStringBuilder);

      dropViewStringBuilder.AppendFormat ("IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Views WHERE TABLE_NAME = '{0}' AND TABLE_SCHEMA = '{1}')\n"
          + "  DROP VIEW [{1}].[{0}]\n",
          GetViewName (classDefinition),
          SqlFileBuilder.DefaultSchema);
    }

    private string GetColumnListForUnionSelect (ClassDefinition classDefinitionForUnionSelect, List<PropertyDefinition> allPropertyDefinitions)
    {
      StringBuilder stringBuilder = new StringBuilder ();

      foreach (PropertyDefinition propertyDefinition in allPropertyDefinitions)
      {
        if (stringBuilder.Length != 0)
          stringBuilder.Append (", ");

        if (IsPartOfInheritanceBranch (classDefinitionForUnionSelect, propertyDefinition.ClassDefinition))
        {
          stringBuilder.AppendFormat ("[{0}]", propertyDefinition.ColumnName);

          if (TableBuilder.HasClassIDColumn (propertyDefinition))
            stringBuilder.AppendFormat (", [{0}]", RdbmsProvider.GetClassIDColumnName (propertyDefinition.ColumnName));
        }
        else
        {
          stringBuilder.Append ("null");

          if (TableBuilder.HasClassIDColumn (propertyDefinition))
            stringBuilder.Append (", null");
        }
      }
      return stringBuilder.ToString ();
    }

    private string GetColumnList (List<PropertyDefinition> propertyDefinitions)
    {
      StringBuilder stringBuilder = new StringBuilder ();
      foreach (PropertyDefinition propertyDefinition in propertyDefinitions)
      {
        if (stringBuilder.Length != 0)
          stringBuilder.Append (", ");

        stringBuilder.AppendFormat ("[{0}]", propertyDefinition.ColumnName);

        if (TableBuilder.HasClassIDColumn (propertyDefinition))
          stringBuilder.AppendFormat (", [{0}]", RdbmsProvider.GetClassIDColumnName (propertyDefinition.ColumnName));
      }
      return stringBuilder.ToString ();
    }

    private string GetClassIDList (ClassDefinitionCollection classDefinitionCollection)
    {
      StringBuilder classIDListBuilder = new StringBuilder ();
      foreach (ClassDefinition classDefinition in classDefinitionCollection)
      {
        if (classIDListBuilder.Length != 0)
          classIDListBuilder.Append (", ");

        classIDListBuilder.AppendFormat ("'{0}'", classDefinition.ID);
      }
      return classIDListBuilder.ToString ();
    }

    private string GetViewName (ClassDefinition classDefinition)
    {
      return classDefinition.ID + "View";
    }
  }
}
