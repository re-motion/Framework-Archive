using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Xml;
using NUnit.Framework;

using Rubicon.Security.Metadata;
using Rubicon.Security.UnitTests.TestDomain;
using Rubicon.Security.UnitTests.XmlAsserter;

namespace Rubicon.Security.UnitTests.Metadata
{
  [TestFixture]
  public class MetadataToXmlConverterTest
  {
    private MetadataCache _cache;
    private MetadataToXmlConverter _converter;

    [SetUp]
    public void SetUp ()
    {
      _cache = new MetadataCache ();
      _converter = new MetadataToXmlConverter (_cache);
    }

    [Test]
    public void EmptyMetadata ()
    {
      XmlDocument document = _converter.Convert ();

      string expectedXml = @"<securityMetadata xmlns=""http://www.rubicon-it.com/Security/Metadata/1.0"" />";
      XmlAssert.AreDocumentsEqual (expectedXml, document);
    }

    [Test]
    public void OneEmptyClass ()
    {
      SecurableClassInfo classInfo = new SecurableClassInfo ();
      classInfo.ID = "4bbb1bab-8d37-40c0-918d-7a07cc7de44f";
      classInfo.Name = "File";
      _cache.AddSecurableClassInfo (typeof (File), classInfo);

      XmlDocument document = _converter.Convert ();

      string expectedXml = @"
          <securityMetadata xmlns=""http://www.rubicon-it.com/Security/Metadata/1.0"">
            <classes>
              <class id=""4bbb1bab-8d37-40c0-918d-7a07cc7de44f"" name=""File"" />
            </classes>
          </securityMetadata>";

      XmlAssert.AreDocumentsEqual (expectedXml, document);
    }

    [Test]
    public void OneStateProperty ()
    {
      StatePropertyInfo propertyInfo = new StatePropertyInfo ();
      propertyInfo.ID = "4bbb1bab-8d37-40c0-918d-7a07cc7de44f";
      propertyInfo.Name = "Confidentiality";
      propertyInfo.Values.Add (new EnumValueInfo (0, "Normal"));
      propertyInfo.Values.Add (new EnumValueInfo (1, "Confidential"));
      propertyInfo.Values.Add (new EnumValueInfo (2, "Private"));

      Type type = typeof (File);
      PropertyInfo property = type.GetProperty ("Confidentiality");
      _cache.AddStatePropertyInfo (property, propertyInfo);

      XmlDocument document = _converter.Convert ();

      string expectedXml = @"
          <securityMetadata xmlns=""http://www.rubicon-it.com/Security/Metadata/1.0"">
            <stateProperties>
              <stateProperty id=""4bbb1bab-8d37-40c0-918d-7a07cc7de44f"" name=""Confidentiality"">
                <state name=""Normal"" value=""0"" />
                <state name=""Confidential"" value=""1"" />
                <state name=""Private"" value=""2"" />
              </stateProperty>
            </stateProperties>
          </securityMetadata>";

      XmlAssert.AreDocumentsEqual (expectedXml, document);
    }

    [Test]
    public void OneAccessType ()
    {
      EnumValueInfo accessType = new EnumValueInfo (0, "Archive");
      accessType.ID = "4bbb1bab-8d37-40c0-918d-7a07cc7de44f";
      _cache.AddAccessType (DomainAccessType.Archive, accessType);

      XmlDocument document = _converter.Convert ();

      string expectedXml = @"
          <securityMetadata xmlns=""http://www.rubicon-it.com/Security/Metadata/1.0"">
            <accessTypes>
              <accessType id=""4bbb1bab-8d37-40c0-918d-7a07cc7de44f"" name=""Archive"" value=""0"" />
            </accessTypes>
          </securityMetadata>";

      XmlAssert.AreDocumentsEqual (expectedXml, document);
    }

    [Test]
    public void OneAbstractRole ()
    {
      EnumValueInfo abstractRole = new EnumValueInfo (0, "Administrator");
      abstractRole.ID = "00000004-0001-0000-0000-000000000000";
      _cache.AddAbstractRole (SpecialAbstractRole.Administrator, abstractRole);

      XmlDocument document = _converter.Convert ();

      string expectedXml = @"
          <securityMetadata xmlns=""http://www.rubicon-it.com/Security/Metadata/1.0"">
            <abstractRoles>
              <abstractRole id=""00000004-0001-0000-0000-000000000000"" name=""Administrator"" value=""0"" />
            </abstractRoles>
          </securityMetadata>";

      XmlAssert.AreDocumentsEqual (expectedXml, document);
    }

    [Test]
    public void MultipleClasses ()
    {
      SecurableClassInfo baseClassInfo = new SecurableClassInfo ();
      baseClassInfo.ID = "4bbb1bab-8d37-40c0-918d-7a07cc7de44f";
      baseClassInfo.Name = "File";
      _cache.AddSecurableClassInfo (typeof (File), baseClassInfo);

      SecurableClassInfo derivedClassInfo1 = new SecurableClassInfo ();
      derivedClassInfo1.ID = "00000000-0000-0000-0002-000000000000";
      derivedClassInfo1.Name = "PaperFile";
      _cache.AddSecurableClassInfo (typeof (PaperFile), derivedClassInfo1);

      SecurableClassInfo derivedClassInfo2 = new SecurableClassInfo ();
      derivedClassInfo2.ID = "118a9d5e-4f89-40af-ade5-e4613e4638d5";
      derivedClassInfo2.Name = "InputFile";
      _cache.AddSecurableClassInfo (typeof (SecurableClassInfo), derivedClassInfo2);

      XmlDocument document = _converter.Convert ();

      string expectedXml = @"
          <securityMetadata xmlns=""http://www.rubicon-it.com/Security/Metadata/1.0"">
            <classes>
              <class id=""4bbb1bab-8d37-40c0-918d-7a07cc7de44f"" name=""File"" />
              <class id=""00000000-0000-0000-0002-000000000000"" name=""PaperFile"" />
              <class id=""118a9d5e-4f89-40af-ade5-e4613e4638d5"" name=""InputFile"" />
            </classes>
          </securityMetadata>";

      XmlAssert.AreDocumentsEqual (expectedXml, document);
    }

    [Test]
    public void DerivedClasses ()
    {
      SecurableClassInfo baseClassInfo = new SecurableClassInfo ();
      baseClassInfo.ID = "4bbb1bab-8d37-40c0-918d-7a07cc7de44f";
      baseClassInfo.Name = "File";
      _cache.AddSecurableClassInfo (typeof (File), baseClassInfo);

      SecurableClassInfo derivedClassInfo1 = new SecurableClassInfo ();
      derivedClassInfo1.ID = "00000000-0000-0000-0002-000000000000";
      derivedClassInfo1.Name = "PaperFile";
      _cache.AddSecurableClassInfo (typeof (PaperFile), derivedClassInfo1);

      SecurableClassInfo derivedClassInfo2 = new SecurableClassInfo ();
      derivedClassInfo2.ID = "118a9d5e-4f89-40af-ade5-e4613e4638d5";
      derivedClassInfo2.Name = "InputFile";
      _cache.AddSecurableClassInfo (typeof (SecurableClassInfo), derivedClassInfo2);

      derivedClassInfo1.BaseClass = baseClassInfo;
      derivedClassInfo2.BaseClass = baseClassInfo;
      baseClassInfo.DerivedClasses.Add (derivedClassInfo1);
      baseClassInfo.DerivedClasses.Add (derivedClassInfo2);

      XmlDocument document = _converter.Convert ();

      string expectedXml = @"
          <securityMetadata xmlns=""http://www.rubicon-it.com/Security/Metadata/1.0"">
            <classes>
              <class id=""4bbb1bab-8d37-40c0-918d-7a07cc7de44f"" name=""File"">
                <derivedClasses>
                  <classRef>00000000-0000-0000-0002-000000000000</classRef>
                  <classRef>118a9d5e-4f89-40af-ade5-e4613e4638d5</classRef>
                </derivedClasses>
              </class>

              <class id=""00000000-0000-0000-0002-000000000000"" name=""PaperFile"" base=""4bbb1bab-8d37-40c0-918d-7a07cc7de44f"" />

              <class id=""118a9d5e-4f89-40af-ade5-e4613e4638d5"" name=""InputFile"" base=""4bbb1bab-8d37-40c0-918d-7a07cc7de44f"" />
            </classes>
          </securityMetadata>";

      XmlAssert.AreDocumentsEqual (expectedXml, document);
    }

    [Test]
    public void MultipleStateProperties ()
    {
      StatePropertyInfo propertyInfo1 = new StatePropertyInfo ();
      propertyInfo1.ID = "4bbb1bab-8d37-40c0-918d-7a07cc7de44f";
      propertyInfo1.Name = "Confidentiality";
      propertyInfo1.Values.Add (new EnumValueInfo (0, "Normal"));
      propertyInfo1.Values.Add (new EnumValueInfo (1, "Confidential"));
      propertyInfo1.Values.Add (new EnumValueInfo (2, "Private"));

      StatePropertyInfo propertyInfo2 = new StatePropertyInfo ();
      propertyInfo2.ID = "40749391-5c45-4fdd-a698-53a6cf167ae7";
      propertyInfo2.Name = "SomeEnum";
      propertyInfo2.Values.Add (new EnumValueInfo (0, "First"));
      propertyInfo2.Values.Add (new EnumValueInfo (1, "Second"));
      propertyInfo2.Values.Add (new EnumValueInfo (2, "Third"));

      Type type = typeof (File);
      PropertyInfo property1 = type.GetProperty ("Confidentiality");
      _cache.AddStatePropertyInfo (property1, propertyInfo1);
      PropertyInfo property2 = type.GetProperty ("SimpleEnum");
      _cache.AddStatePropertyInfo (property2, propertyInfo2);

      XmlDocument document = _converter.Convert ();

      string expectedXml = @"
          <securityMetadata xmlns=""http://www.rubicon-it.com/Security/Metadata/1.0"">
            <stateProperties>
              <stateProperty id=""4bbb1bab-8d37-40c0-918d-7a07cc7de44f"" name=""Confidentiality"">
                <state name=""Normal"" value=""0"" />
                <state name=""Confidential"" value=""1"" />
                <state name=""Private"" value=""2"" />
              </stateProperty>

              <stateProperty id=""40749391-5c45-4fdd-a698-53a6cf167ae7"" name=""SomeEnum"">
                <state name=""First"" value=""0"" />
                <state name=""Second"" value=""1"" />
                <state name=""Third"" value=""2"" />
              </stateProperty>
            </stateProperties>
          </securityMetadata>";

      XmlAssert.AreDocumentsEqual (expectedXml, document);
    }

    [Test]
    public void MultipleAccessTypes ()
    {
      EnumValueInfo accessType1 = new EnumValueInfo (0, "Archive");
      accessType1.ID = "4bbb1bab-8d37-40c0-918d-7a07cc7de44f";
      _cache.AddAccessType (DomainAccessType.Archive, accessType1);

      EnumValueInfo accessType2 = new EnumValueInfo (1, "Journalize");
      accessType2.ID = "c6995b9b-7fed-42df-a2d1-897600b00fb0";
      _cache.AddAccessType (DomainAccessType.Journalize, accessType2);

      XmlDocument document = _converter.Convert ();

      string expectedXml = @"
          <securityMetadata xmlns=""http://www.rubicon-it.com/Security/Metadata/1.0"">
            <accessTypes>
              <accessType id=""4bbb1bab-8d37-40c0-918d-7a07cc7de44f"" name=""Archive"" value=""0"" />
              <accessType id=""c6995b9b-7fed-42df-a2d1-897600b00fb0"" name=""Journalize"" value=""1"" />
            </accessTypes>
          </securityMetadata>";

      XmlAssert.AreDocumentsEqual (expectedXml, document);
    }

    [Test]
    public void MultipleAbstractRoles ()
    {
      EnumValueInfo abstractRole1 = new EnumValueInfo (0, "Administrator");
      abstractRole1.ID = "00000004-0001-0000-0000-000000000000";
      _cache.AddAbstractRole (SpecialAbstractRole.Administrator, abstractRole1);

      EnumValueInfo abstractRole2 = new EnumValueInfo (1, "PowerUser");
      abstractRole2.ID = "3b84739a-7f35-4224-989f-3d5b05047cbb";
      _cache.AddAbstractRole (SomeEnum.First, abstractRole2);

      XmlDocument document = _converter.Convert ();

      string expectedXml = @"
          <securityMetadata xmlns=""http://www.rubicon-it.com/Security/Metadata/1.0"">
            <abstractRoles>
              <abstractRole id=""00000004-0001-0000-0000-000000000000"" name=""Administrator"" value=""0"" />
              <abstractRole id=""3b84739a-7f35-4224-989f-3d5b05047cbb"" name=""PowerUser"" value=""1"" />
            </abstractRoles>
          </securityMetadata>";

      XmlAssert.AreDocumentsEqual (expectedXml, document);
    }

    [Test]
    public void ClassWithStateProperties ()
    {
      SecurableClassInfo classInfo = new SecurableClassInfo ();
      classInfo.ID = "4bbb1bab-8d37-40c0-918d-7a07cc7de44f";
      classInfo.Name = "File";
      _cache.AddSecurableClassInfo (typeof (File), classInfo);

      StatePropertyInfo propertyInfo = new StatePropertyInfo ();
      propertyInfo.ID = "4bbb1bab-8d37-40c0-918d-7a07cc7de44f";
      propertyInfo.Name = "Confidentiality";
      propertyInfo.Values.Add (new EnumValueInfo (0, "Normal"));
      propertyInfo.Values.Add (new EnumValueInfo (1, "Confidential"));
      propertyInfo.Values.Add (new EnumValueInfo (2, "Private"));

      Type type = typeof (File);
      PropertyInfo property = type.GetProperty ("Confidentiality");
      _cache.AddStatePropertyInfo (property, propertyInfo);

      classInfo.Properties.Add (propertyInfo);

      XmlDocument document = _converter.Convert ();

      string expectedXml = @"
          <securityMetadata xmlns=""http://www.rubicon-it.com/Security/Metadata/1.0"">
            <classes>
              <class id=""4bbb1bab-8d37-40c0-918d-7a07cc7de44f"" name=""File"">
                <stateProperties>
                  <statePropertyRef>4bbb1bab-8d37-40c0-918d-7a07cc7de44f</statePropertyRef>
                </stateProperties>
              </class>
            </classes>

            <stateProperties>
              <stateProperty id=""4bbb1bab-8d37-40c0-918d-7a07cc7de44f"" name=""Confidentiality"">
                <state name=""Normal"" value=""0"" />
                <state name=""Confidential"" value=""1"" />
                <state name=""Private"" value=""2"" />
              </stateProperty>
            </stateProperties>
          </securityMetadata>";

      XmlAssert.AreDocumentsEqual (expectedXml, document);
    }

    [Test]
    public void ClassWithAccessTypes ()
    {
      SecurableClassInfo classInfo = new SecurableClassInfo ();
      classInfo.ID = "4bbb1bab-8d37-40c0-918d-7a07cc7de44f";
      classInfo.Name = "File";
      _cache.AddSecurableClassInfo (typeof (File), classInfo);

      EnumValueInfo accessType1 = new EnumValueInfo (0, "Archive");
      accessType1.ID = "64d8f74e-685f-44ab-9705-1fda9ff836a4";
      _cache.AddAccessType (DomainAccessType.Archive, accessType1);

      EnumValueInfo accessType2 = new EnumValueInfo (1, "Journalize");
      accessType2.ID = "c6995b9b-7fed-42df-a2d1-897600b00fb0";
      _cache.AddAccessType (DomainAccessType.Journalize, accessType2);

      classInfo.AccessTypes.Add (accessType1);
      classInfo.AccessTypes.Add (accessType2);

      XmlDocument document = _converter.Convert ();

      string expectedXml = @"
          <securityMetadata xmlns=""http://www.rubicon-it.com/Security/Metadata/1.0"">
            <classes>
              <class id=""4bbb1bab-8d37-40c0-918d-7a07cc7de44f"" name=""File"">
                <accessTypes>
                  <accessTypeRef>64d8f74e-685f-44ab-9705-1fda9ff836a4</accessTypeRef>
                  <accessTypeRef>c6995b9b-7fed-42df-a2d1-897600b00fb0</accessTypeRef>
                </accessTypes>
              </class>
            </classes>

            <accessTypes>
              <accessType id=""64d8f74e-685f-44ab-9705-1fda9ff836a4"" name=""Archive"" value=""0"" />
              <accessType id=""c6995b9b-7fed-42df-a2d1-897600b00fb0"" name=""Journalize"" value=""1"" />
            </accessTypes>
          </securityMetadata>";

      XmlAssert.AreDocumentsEqual (expectedXml, document);
    }

    [Test]
    public void IntegrationTest ()
    {
      SecurableClassInfo classInfo = new SecurableClassInfo ();
      classInfo.ID = "4bbb1bab-8d37-40c0-918d-7a07cc7de44f";
      classInfo.Name = "File";
      _cache.AddSecurableClassInfo (typeof (File), classInfo);

      SecurableClassInfo derivedClassInfo = new SecurableClassInfo ();
      derivedClassInfo.ID = "ac101f66-6d1f-4002-b32b-f951db36582c";
      derivedClassInfo.Name = "PaperFile";
      _cache.AddSecurableClassInfo (typeof (PaperFile), derivedClassInfo);

      classInfo.DerivedClasses.Add (derivedClassInfo);
      derivedClassInfo.BaseClass = classInfo;
      
      StatePropertyInfo propertyInfo1 = new StatePropertyInfo ();
      propertyInfo1.ID = "d81b1521-ea06-4338-af6f-ff8510394efd";
      propertyInfo1.Name = "Confidentiality";
      propertyInfo1.Values.Add (new EnumValueInfo (0, "Normal"));
      propertyInfo1.Values.Add (new EnumValueInfo (1, "Confidential"));
      propertyInfo1.Values.Add (new EnumValueInfo (2, "Private"));

      StatePropertyInfo propertyInfo2 = new StatePropertyInfo ();
      propertyInfo2.ID = "40749391-5c45-4fdd-a698-53a6cf167ae7";
      propertyInfo2.Name = "SomeEnum";
      propertyInfo2.Values.Add (new EnumValueInfo (0, "First"));
      propertyInfo2.Values.Add (new EnumValueInfo (1, "Second"));
      propertyInfo2.Values.Add (new EnumValueInfo (2, "Third"));

      Type type = typeof (File);
      PropertyInfo property1 = type.GetProperty ("Confidentiality");
      _cache.AddStatePropertyInfo (property1, propertyInfo1);
      PropertyInfo property2 = type.GetProperty ("SimpleEnum");
      _cache.AddStatePropertyInfo (property2, propertyInfo2);

      classInfo.Properties.Add (propertyInfo1);
      derivedClassInfo.Properties.Add (propertyInfo1);
      derivedClassInfo.Properties.Add (propertyInfo2);

      EnumValueInfo accessType1 = new EnumValueInfo (0, "Archive");
      accessType1.ID = "64d8f74e-685f-44ab-9705-1fda9ff836a4";
      _cache.AddAccessType (DomainAccessType.Archive, accessType1);

      EnumValueInfo accessType2 = new EnumValueInfo (1, "Journalize");
      accessType2.ID = "c6995b9b-7fed-42df-a2d1-897600b00fb0";
      _cache.AddAccessType (DomainAccessType.Journalize, accessType2);

      classInfo.AccessTypes.Add (accessType1);
      derivedClassInfo.AccessTypes.Add (accessType1);
      derivedClassInfo.AccessTypes.Add (accessType2);

      EnumValueInfo abstractRole1 = new EnumValueInfo (0, "Administrator");
      abstractRole1.ID = "00000004-0001-0000-0000-000000000000";
      _cache.AddAbstractRole (SpecialAbstractRole.Administrator, abstractRole1);

      EnumValueInfo abstractRole2 = new EnumValueInfo (1, "PowerUser");
      abstractRole2.ID = "3b84739a-7f35-4224-989f-3d5b05047cbb";
      _cache.AddAbstractRole (SomeEnum.First, abstractRole2);

      XmlDocument document = _converter.Convert ();

      string expectedXml = @"
          <securityMetadata xmlns=""http://www.rubicon-it.com/Security/Metadata/1.0"">
            <classes>
              <class id=""4bbb1bab-8d37-40c0-918d-7a07cc7de44f"" name=""File"">
                <derivedClasses>
                  <classRef>ac101f66-6d1f-4002-b32b-f951db36582c</classRef>
                </derivedClasses>

                <stateProperties>
                  <statePropertyRef>d81b1521-ea06-4338-af6f-ff8510394efd</statePropertyRef>
                </stateProperties>

                <accessTypes>
                  <accessTypeRef>64d8f74e-685f-44ab-9705-1fda9ff836a4</accessTypeRef>
                </accessTypes>
              </class>

              <class id=""ac101f66-6d1f-4002-b32b-f951db36582c"" name=""PaperFile"" base=""4bbb1bab-8d37-40c0-918d-7a07cc7de44f"">
                <stateProperties>
                  <statePropertyRef>d81b1521-ea06-4338-af6f-ff8510394efd</statePropertyRef>
                  <statePropertyRef>40749391-5c45-4fdd-a698-53a6cf167ae7</statePropertyRef>
                </stateProperties>

                <accessTypes>
                  <accessTypeRef>64d8f74e-685f-44ab-9705-1fda9ff836a4</accessTypeRef>
                  <accessTypeRef>c6995b9b-7fed-42df-a2d1-897600b00fb0</accessTypeRef>
                </accessTypes>
              </class>
            </classes>

            <stateProperties>
              <stateProperty id=""d81b1521-ea06-4338-af6f-ff8510394efd"" name=""Confidentiality"">
                <state name=""Normal"" value=""0"" />
                <state name=""Confidential"" value=""1"" />
                <state name=""Private"" value=""2"" />
              </stateProperty>

              <stateProperty id=""40749391-5c45-4fdd-a698-53a6cf167ae7"" name=""SomeEnum"">
                <state name=""First"" value=""0"" />
                <state name=""Second"" value=""1"" />
                <state name=""Third"" value=""2"" />
              </stateProperty>
            </stateProperties>

            <accessTypes>
              <accessType id=""64d8f74e-685f-44ab-9705-1fda9ff836a4"" name=""Archive"" value=""0"" />
              <accessType id=""c6995b9b-7fed-42df-a2d1-897600b00fb0"" name=""Journalize"" value=""1"" />
            </accessTypes>

            <abstractRoles>
              <abstractRole id=""00000004-0001-0000-0000-000000000000"" name=""Administrator"" value=""0"" />
              <abstractRole id=""3b84739a-7f35-4224-989f-3d5b05047cbb"" name=""PowerUser"" value=""1"" />
            </abstractRoles>
          </securityMetadata>";

      XmlAssert.AreDocumentsEqual (expectedXml, document);
    }
  }
}
