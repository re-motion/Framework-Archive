using System;
using System.Xml.Serialization;
using System.IO;
using Rubicon.NullableValueTypes;
using Rubicon.ObjectBinding.Reflection;
using Rubicon.ObjectBinding;

namespace OBWTest
{

[XmlType]
public class Person: ReflectionBusinessObject, IBusinessObjectWithIdentity
{
  public static Person LoadFromXml (string fileName)
  {
    XmlSerializer serializer = new XmlSerializer (typeof (Person));

    using (FileStream stream = new FileStream (fileName, FileMode.Open, FileAccess.Read))
    {
      return (Person) serializer.Deserialize (stream);
    }
  }

  private string _firstName;
  private string _lastName;
  private DateTime _dateOfBirth;
  private int _height;
  private NaInt32 _income = 1;
  private Gender _gender;
  private MarriageStatus _marriageStatus;
  private Person _partner;

  public void SaveToXml (string fileName)
  {
    XmlSerializer serializer = new XmlSerializer (typeof (Person));

    using (FileStream stream = new FileStream (fileName, FileMode.Create, FileAccess.Write))
    {
      serializer.Serialize (stream, this);
    }
  }

  [XmlAttribute]
  public string FirstName
  {
    get { return _firstName; }
    set { _firstName = value; }
  }

  [XmlAttribute]
  public string LastName
  {
    get { return _lastName; }
    set { _lastName = value; }
  }

  [XmlAttribute]
  public DateTime DateOfBirth
  {
    get { return _dateOfBirth; }
    set { _dateOfBirth = value; }
  }

  [XmlAttribute]
  public int Height
  {
    get { return _height; }
    set { _height = value; }
  }

  [XmlElement]
  public NaInt32 Income
  {
    get { return _income; }
    set { _income = value; }
  }

  [XmlAttribute]
  public Gender Gender
  {
    get { return _gender; }
    set { _gender = value; }
  }

  [XmlAttribute]
  public MarriageStatus MarriageStatus
  {
    get { return _marriageStatus; }
    set { _marriageStatus = value; }
  }

  public Person Partner
  {
    get { return _partner; }
    set { _partner = value; }
  }

  public string DisplayName
  {
    get
    {
      return LastName + ", " + FirstName;
    }
  }

  public string UniqueIdentifier
  {
    get
    {
      // TODO:  Add Person.UniqueIdentifier getter implementation
      return null;
    }
  }
}

public enum Gender
{
  Male,
  Female
}

public enum MarriageStatus
{
  Married,
  Single, 
  Divorced
}

}
