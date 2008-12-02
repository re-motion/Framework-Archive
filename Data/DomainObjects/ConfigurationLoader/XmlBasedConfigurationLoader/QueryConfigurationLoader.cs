/* Copyright (C) 2005 - 2008 rubicon informationstechnologie gmbh
 *
 * This program is free software: you can redistribute it and/or modify it under 
 * the terms of the re:motion license agreement in license.txt. If you did not 
 * receive it, please visit http://www.re-motion.org/licensing.
 * 
 * Unless otherwise provided, this software is distributed on an "AS IS" basis, 
 * WITHOUT WARRANTY OF ANY KIND, either express or implied. 
 */

using System;
using System.IO;
using System.Xml;
using System.Xml.Schema;
using Remotion.Data.DomainObjects.Configuration;
using Remotion.Data.DomainObjects.Persistence.Configuration;
using Remotion.Data.DomainObjects.Queries.Configuration;
using Remotion.Data.DomainObjects.Schemas;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.ConfigurationLoader.XmlBasedConfigurationLoader
{
  public class QueryConfigurationLoader : BaseFileLoader
  {
    // types

    // static members and constants

    // member fields

    // construction and disposing

    //TODO: resolve parameter
    public QueryConfigurationLoader (string configurationFile)
    {
      try
      {
        base.Initialize (
            configurationFile,
            SchemaLoader.Queries,
            true,
            PrefixNamespace.QueryConfigurationNamespace);
      }
      catch (ConfigurationException e)
      {
        throw CreateQueryConfigurationException (
            e, "Error while reading query configuration: {0} File: '{1}'.", e.Message, Path.GetFullPath (configurationFile));
      }
      catch (XmlSchemaException e)
      {
        throw CreateQueryConfigurationException (
            e, "Error while reading query configuration: {0} File: '{1}'.", e.Message, Path.GetFullPath (configurationFile));
      }
      catch (XmlException e)
      {
        throw CreateQueryConfigurationException (
            e, "Error while reading query configuration: {0} File: '{1}'.", e.Message, Path.GetFullPath (configurationFile));
      }
    }

    // methods and properties

    public QueryDefinitionCollection GetQueryDefinitions ()
    {
      QueryDefinitionCollection queries = new QueryDefinitionCollection();
      FillQueryDefinitions (queries);
      return queries;
    }

    private void FillQueryDefinitions (QueryDefinitionCollection queries)
    {
      XmlNodeList queryNodeList = Document.SelectNodes (FormatXPath ("{0}:queries/{0}:query"), NamespaceManager);

      foreach (XmlNode queryNode in queryNodeList)
        queries.Add (GetQueryDefinition (queryNode));
    }

    private QueryDefinition GetQueryDefinition (XmlNode queryNode)
    {
      string queryID = queryNode.SelectSingleNode ("@id", NamespaceManager).InnerText;
      string queryTypeAsString = queryNode.SelectSingleNode ("@type", NamespaceManager).InnerText;
      QueryType queryType = (QueryType) Enum.Parse (typeof (QueryType), queryTypeAsString, true);

      XmlNode node = queryNode.SelectSingleNode (FormatXPath ("{0}:storageGroupType"), NamespaceManager);
      string storageProviderID;
      if (node != null)
        storageProviderID = GetStorageProviderID (node.InnerText);
      else
        storageProviderID = GetStorageProviderID (null);

      string statement = queryNode.SelectSingleNode (FormatXPath ("{0}:statement"), NamespaceManager).InnerText;

      Type collectionType = LoaderUtility.GetOptionalType (queryNode, FormatXPath ("{0}:collectionType"), NamespaceManager);

      if (queryType == QueryType.Scalar && collectionType != null)
        throw CreateQueryConfigurationException ("A scalar query '{0}' must not specify a collectionType.", queryID);

      return new QueryDefinition (queryID, storageProviderID, statement, queryType, collectionType);
    }

    private string FormatXPath (string xPath)
    {
      return NamespaceManager.FormatXPath (xPath, PrefixNamespace.QueryConfigurationNamespace.Uri);
    }

    //TODO: COMMONS-842
    //TODO: Move type resolving to storagegrouplist and unify with ClassReflector
    //TODO: Test for DefaultStorageProvider
    private string GetStorageProviderID (string storageGroupName)
    {
      var defaultStorageProviderDefinition = DomainObjectsConfiguration.Current.Storage.DefaultStorageProviderDefinition;
      if (storageGroupName == null)
      {
        //TODO COMMONS-783: Test exception
        if (defaultStorageProviderDefinition == null)
          throw new InvalidOperationException ("Missing default storage provider.\r\nFile: " + ConfigurationFile);
        return defaultStorageProviderDefinition.Name;
      }

      Type storageGroupType = TypeUtility.GetType (storageGroupName, true);
      string canonicalStorageGroupName = TypeUtility.GetPartialAssemblyQualifiedName (storageGroupType);
      StorageGroupElement storageGroup = DomainObjectsConfiguration.Current.Storage.StorageGroups[canonicalStorageGroupName];
      if (storageGroup == null)
      {
        //TODO COMMONS-783: Test exception
        if (defaultStorageProviderDefinition == null)
          throw new InvalidOperationException ("Missing default storage provider.\r\nFile: " + ConfigurationFile);
        return defaultStorageProviderDefinition.Name;
      }
      return storageGroup.StorageProviderName;
    }

    private QueryConfigurationException CreateQueryConfigurationException (string message, params object[] args)
    {
      return CreateQueryConfigurationException (null, message, args);
    }

    private QueryConfigurationException CreateQueryConfigurationException (Exception inner, string message, params object[] args)
    {
      return new QueryConfigurationException (string.Format (message, args), inner);
    }
  }
}