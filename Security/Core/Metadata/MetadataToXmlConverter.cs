using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using Rubicon.Utilities;

namespace Rubicon.Security.Metadata
{
  public class MetadataToXmlConverter
  {
    private delegate XmlNode CreateInfoNodeDelegate<T> (XmlDocument document, T info);

    public const string MetadataXmlNamespace = "http://www.rubicon-it.com/Security/Metadata/1.0";

    private MetadataCache _cache;

    public MetadataToXmlConverter (MetadataCache cache)
    {
      ArgumentUtility.CheckNotNull ("cache", cache);
      _cache = cache;
    }

    public XmlDocument Convert()
    {
      XmlDocument document = new XmlDocument();
      XmlElement rootElement = document.CreateElement ("securityMetadata", MetadataXmlNamespace);

      AppendCollection (document, rootElement, "classes", _cache.GetSecurableClassInfos (), CreateClassNode);
      AppendCollection (document, rootElement, "stateProperties", _cache.GetStatePropertyInfos (), CreateStatePropertyNode);
      AppendCollection (document, rootElement, "accessTypes", _cache.GetAccessTypes (), CreateAccessTypeNode);
      AppendCollection (document, rootElement, "abstractRoles", _cache.GetAbstractRoles (), CreateAbstractRoleNode);

      document.AppendChild (rootElement);
      return document;
    }

    private void AppendCollection<T> (
        XmlDocument document, 
        XmlElement parentElement, 
        string collectionElementName, 
        List<T> infos, 
        CreateInfoNodeDelegate<T> createInfoNodeDelegate)
    {
      if (infos.Count > 0)
      {
        XmlElement collectionElement = document.CreateElement (collectionElementName, MetadataXmlNamespace);

        foreach (T info in infos)
          collectionElement.AppendChild (createInfoNodeDelegate (document, info));

        parentElement.AppendChild (collectionElement);
      }
    }

    private XmlElement CreateAccessTypeNode (XmlDocument document, EnumValueInfo accessTypeInfo)
    {
      XmlElement accessTypeElement = document.CreateElement ("accessType", MetadataXmlNamespace);
      AppendEnumValueInfoAttributes (document, accessTypeInfo, accessTypeElement);
      return accessTypeElement;
    }

    private XmlElement CreateAbstractRoleNode (XmlDocument document, EnumValueInfo abstractRoleInfo)
    {
      XmlElement abstractRoleElement = document.CreateElement ("abstractRole", MetadataXmlNamespace);
      AppendEnumValueInfoAttributes (document, abstractRoleInfo, abstractRoleElement);
      return abstractRoleElement;
    }

    private void AppendEnumValueInfoAttributes (XmlDocument document, EnumValueInfo enumValueInfo, XmlElement enumValueElement)
    {
      XmlAttribute enumValueIDAttribute = document.CreateAttribute ("id");
      enumValueIDAttribute.Value = enumValueInfo.ID;

      XmlAttribute enumValueNameAttribute = document.CreateAttribute ("name");
      enumValueNameAttribute.Value = enumValueInfo.Name;

      XmlAttribute enumValueValueAttribute = document.CreateAttribute ("value");
      enumValueValueAttribute.Value = enumValueInfo.Value.ToString ();

      enumValueElement.Attributes.Append (enumValueIDAttribute);
      enumValueElement.Attributes.Append (enumValueNameAttribute);
      enumValueElement.Attributes.Append (enumValueValueAttribute);
    }

    private XmlElement CreateStatePropertyNode (XmlDocument document, StatePropertyInfo propertyInfo)
    {
      XmlElement propertyElement = document.CreateElement ("stateProperty", MetadataXmlNamespace);

      XmlAttribute propertyIdAttribute = document.CreateAttribute ("id");
      propertyIdAttribute.Value = propertyInfo.ID;
      
      XmlAttribute propertyNameAttribute = document.CreateAttribute ("name");
      propertyNameAttribute.Value = propertyInfo.Name;
      
      propertyElement.Attributes.Append (propertyIdAttribute);
      propertyElement.Attributes.Append (propertyNameAttribute);
      
      foreach (EnumValueInfo enumValueInfo in propertyInfo.Values)
        propertyElement.AppendChild (CreateStatePropertyValueNode (document, enumValueInfo));
      
      return propertyElement;
    }

    private XmlElement CreateStatePropertyValueNode (XmlDocument document, EnumValueInfo enumValueInfo)
    {
      XmlElement propertyValueElement = document.CreateElement ("state", MetadataXmlNamespace);

      XmlAttribute propertyValueNameAttribute = document.CreateAttribute ("name");
      propertyValueNameAttribute.Value = enumValueInfo.Name;
      
      XmlAttribute propertyValueValueAttribute = document.CreateAttribute ("value");
      propertyValueValueAttribute.Value = enumValueInfo.Value.ToString ();
      
      propertyValueElement.Attributes.Append (propertyValueNameAttribute);
      propertyValueElement.Attributes.Append (propertyValueValueAttribute);
      
      return propertyValueElement;
    }

    private XmlElement CreateClassNode (XmlDocument document, SecurableClassInfo classInfo)
    {
      XmlElement classElement = document.CreateElement ("class", MetadataXmlNamespace);
      
      XmlAttribute classIdAttribute = document.CreateAttribute ("id");
      classIdAttribute.Value = classInfo.ID;
      
      XmlAttribute classNameAttribute = document.CreateAttribute ("name");
      classNameAttribute.Value = classInfo.Name;
      
      classElement.Attributes.Append (classIdAttribute);
      classElement.Attributes.Append (classNameAttribute);

      if (classInfo.BaseClass != null)
      {
        XmlAttribute baseClassAttribute = document.CreateAttribute ("base");
        baseClassAttribute.Value = classInfo.BaseClass.ID;

        classElement.Attributes.Append (baseClassAttribute);
      }

      AppendCollection (document, classElement, "derivedClasses", classInfo.DerivedClasses, CreateDerivedClassRefElement);
      AppendCollection (document, classElement, "stateProperties", classInfo.Properties, CreateStatePropertyRefElement);
      AppendCollection (document, classElement, "accessTypes", classInfo.AccessTypes, CreateAccessTypeRefElement);

      return classElement;
    }

    private XmlElement CreateDerivedClassRefElement (XmlDocument document, SecurableClassInfo derivedClassInfo)
    {
      return CreateRefElement (document, "classRef", derivedClassInfo.ID);
    }

    private XmlElement CreateStatePropertyRefElement (XmlDocument document, StatePropertyInfo propertyInfo)
    {
      return CreateRefElement (document, "statePropertyRef", propertyInfo.ID);
    }

    private XmlElement CreateAccessTypeRefElement (XmlDocument document, EnumValueInfo accessTypeInfo)
    {
      return CreateRefElement (document, "accessTypeRef", accessTypeInfo.ID);
    }

    private static XmlElement CreateRefElement (XmlDocument document, string elementName, string idText)
    {
      XmlElement refElement = document.CreateElement (elementName, MetadataXmlNamespace);

      XmlText idTextNode = document.CreateTextNode (idText);
      refElement.AppendChild (idTextNode);

      return refElement;
    }
  }
}
