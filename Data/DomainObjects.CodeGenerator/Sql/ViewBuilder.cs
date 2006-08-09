using System;
using System.Collections.Generic;
using System.Text;
using Rubicon.Data.DomainObjects.Mapping;
using Rubicon.Utilities;
using Rubicon.Data.DomainObjects.Persistence.Rdbms;

namespace Rubicon.Data.DomainObjects.CodeGenerator.Sql
{
  public class ViewBuilder
  {
    // types

    // static members and constants

    // member fields

    private StringBuilder _createViewBuilder;
    private StringBuilder _dropViewBuilder;

    // construction and disposing

    public ViewBuilder ()
    {
      _createViewBuilder = new StringBuilder ();
      _dropViewBuilder = new StringBuilder ();
    }

    // methods and properties

    public string GetCreateViewScript ()
    {
      return _createViewBuilder.ToString ();
    }

    public string GetDropViewScript ()
    {
      return _dropViewBuilder.ToString ();
    }

    public void AddViews (ClassDefinitionCollection classDefinitions)
    {
      foreach (ClassDefinition classDefinition in classDefinitions)
        AddView (classDefinition);
    }

    public void AddView (ClassDefinition classDefinition)
    {
      ArgumentUtility.CheckNotNull ("classDefinition", classDefinition);

      if (classDefinition.GetEntityName () != null)
        AddViewForConcreteClass (classDefinition);
      else
        AddViewForAbstractClass (classDefinition);
    }

    private void AddViewForConcreteClass (ClassDefinition classDefinition)
    {
      AddDropViewStatement (classDefinition);
      AppendCreateViewSeparator ();

      _createViewBuilder.AppendFormat (
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

    private void AddViewForAbstractClass (ClassDefinition classDefinition)
    {
      ClassDefinitionCollection concreteClasses = GetConcreteClasses (classDefinition);
      if (concreteClasses.Count != 0)
      {
        List<PropertyDefinition> allPropertyDefinitions = GetAllPropertyDefinitions (classDefinition);
        string classIDListForWhereClause = GetClassIDList (GetClassDefinitionsForWhereClause (classDefinition));

        AddDropViewStatement (classDefinition);

        AppendCreateViewSeparator ();

        _createViewBuilder.AppendFormat (
            "CREATE VIEW [{0}].[{1}] ([ID], [ClassID], [Timestamp], {2})\n"
            + "  WITH SCHEMABINDING AS\n",
            SqlFileBuilder.DefaultSchema,
            GetViewName (classDefinition),
            GetColumnList (allPropertyDefinitions));

        int numberOfSelects = 0;
        foreach (ClassDefinition tableRootClass in concreteClasses)
        {
          if (numberOfSelects > 0)
            _createViewBuilder.AppendFormat ("  UNION ALL\n");

          _createViewBuilder.AppendFormat (
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
          _createViewBuilder.Append ("  WITH CHECK OPTION\n");
      }
    }

    private ClassDefinitionCollection GetConcreteClasses (ClassDefinition abstractClassDefinition)
    {
      ArgumentUtility.CheckNotNull ("abstractClassDefinition", abstractClassDefinition);

      ClassDefinitionCollection concreteClasses = new ClassDefinitionCollection (false);
      FillConcreteClasses (abstractClassDefinition, concreteClasses);
      return concreteClasses;
    }

    private void FillConcreteClasses (ClassDefinition classDefinition, ClassDefinitionCollection concreteClasses)
    {
      if (classDefinition.GetEntityName () != null)
      {
        concreteClasses.Add (classDefinition);
        return;
      }

      foreach (ClassDefinition derivedClass in classDefinition.DerivedClasses)
        FillConcreteClasses (derivedClass, concreteClasses);
    }

    private void AppendCreateViewSeparator ()
    {
      if (_createViewBuilder.Length != 0)
        _createViewBuilder.Append ("GO\n\n");
    }

    private void AddDropViewStatement (ClassDefinition classDefinition)
    {
      if (_dropViewBuilder.Length != 0)
        _dropViewBuilder.Append ("\n");

      _dropViewBuilder.AppendFormat ("IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Views WHERE TABLE_NAME = '{0}')\n"
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

    private ClassDefinitionCollection GetClassDefinitionsForWhereClause (ClassDefinition classDefinition)
    {
      ClassDefinitionCollection classDefinitionsForWhereClause = new ClassDefinitionCollection (false);

      if (classDefinition.GetEntityName () != null)
        classDefinitionsForWhereClause.Add (classDefinition);

      FillClassDefinitionsForWhereClauseWithDerivedClasses (classDefinition, classDefinitionsForWhereClause);

      return classDefinitionsForWhereClause;
    }

    private void FillClassDefinitionsForWhereClauseWithDerivedClasses (
        ClassDefinition classDefinition, 
        ClassDefinitionCollection classDefinitionsForWhereClause)
    {
      foreach (ClassDefinition derivedClass in classDefinition.DerivedClasses)
      {
        if (derivedClass.GetEntityName () != null)
          classDefinitionsForWhereClause.Add (derivedClass);
  
        FillClassDefinitionsForWhereClauseWithDerivedClasses (derivedClass, classDefinitionsForWhereClause);
      }
    }

    private bool IsPartOfInheritanceBranch (ClassDefinition classDefinitionOfBranch, ClassDefinition classDefinitionToEvaluate)
    {
      if (classDefinitionOfBranch == classDefinitionToEvaluate)
        return true;

      ClassDefinition baseClass = classDefinitionOfBranch.BaseClass;
      while (baseClass != null)
      {
        if (baseClass == classDefinitionToEvaluate)
          return true;
        baseClass = baseClass.BaseClass;
      }

      return IsDerivedClass (classDefinitionOfBranch, classDefinitionToEvaluate);
    }

    private bool IsDerivedClass (ClassDefinition classDefinition, ClassDefinition classDefinitionToEvaluate)
    {
      if (classDefinition.DerivedClasses.Contains (classDefinitionToEvaluate))
        return true;

      foreach (ClassDefinition derivedClasses in classDefinition.DerivedClasses)
      {
        if (IsDerivedClass (derivedClasses, classDefinitionToEvaluate))
          return true;
      }

      return false;
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

    private List<PropertyDefinition> GetAllPropertyDefinitions (ClassDefinition classDefinition)
    {
      List<PropertyDefinition> allPropertyDefinitions = new List<PropertyDefinition> ();
      FillAllPropertyDefinitionsFromBaseClasses (classDefinition, allPropertyDefinitions);

      foreach (PropertyDefinition propertyDefinitionInDerivedClass in classDefinition.MyPropertyDefinitions)
        allPropertyDefinitions.Add (propertyDefinitionInDerivedClass);

      FillAllPropertyDefinitionsFromDerivedClasses (classDefinition, allPropertyDefinitions);

      return allPropertyDefinitions;
    }

    private void FillAllPropertyDefinitionsFromBaseClasses (ClassDefinition classDefinition, List<PropertyDefinition> allPropertyDefinitions)
    {
      if (classDefinition.BaseClass == null)
        return;

      FillAllPropertyDefinitionsFromBaseClasses (classDefinition.BaseClass, allPropertyDefinitions);

      foreach (PropertyDefinition propertyDefinitionInDerivedClass in classDefinition.BaseClass.MyPropertyDefinitions)
        allPropertyDefinitions.Add (propertyDefinitionInDerivedClass);
    }

    private void FillAllPropertyDefinitionsFromDerivedClasses (ClassDefinition classDefinition, List<PropertyDefinition> allPropertyDefinitions)
    {
      foreach (ClassDefinition derivedClass in classDefinition.DerivedClasses)
      {
        foreach (PropertyDefinition propertyDefinitionInDerivedClass in derivedClass.MyPropertyDefinitions)
          allPropertyDefinitions.Add (propertyDefinitionInDerivedClass);

        FillAllPropertyDefinitionsFromDerivedClasses (derivedClass, allPropertyDefinitions);
      }
    }

    private string GetViewName (ClassDefinition classDefinition)
    {
      return classDefinition.ID + "View";
    }
  }
}
