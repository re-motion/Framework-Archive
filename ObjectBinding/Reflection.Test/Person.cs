using System;
using System.Xml.Serialization;
using System.IO;
using System.ComponentModel;
using Rubicon.NullableValueTypes;
using Rubicon.ObjectBinding.Reflection;
using Rubicon.ObjectBinding;

namespace OBWTest
{

[XmlType]
public class Person: ReflectionBusinessObject
{
  public static Person GetObject (Guid id)
  {
    ReflectionBusinessObject obj = ReflectionBusinessObjectStorage.GetObject (typeof (Person), id);
    
    if (obj == null)
      return null;

    Person person = (Person) obj;
    
    if (person.PartnerID != Guid.Empty)
      person.Partner = (Person) ReflectionBusinessObjectStorage.GetObject (typeof (Person), person.PartnerID);

    return person;
  }

  public static Person CreateObject ()
  {
    return (Person) ReflectionBusinessObjectStorage.CreateObject (typeof (Person));
  }

  public static Person CreateObject (Guid id)
  {
    return (Person) ReflectionBusinessObjectStorage.CreateObject (typeof (Person), id);
  }
  
  private string _firstName;
  private string _lastName;
  private DateTime _dateOfBirth;
  private int _height;
  private NaInt32 _income = 1;
  private Gender _gender;
  private MarriageStatus _marriageStatus;
  private DateTime _dateofDeath_Date;

  private Guid _partnerID; 

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

  [XmlAttribute]
  [EditorBrowsable (EditorBrowsableState.Never)]
  public Guid PartnerID
  {
    get { return _partnerID; }
    set { _partnerID = value; }
  }

  [XmlIgnore]
  public Person Partner
  {
    get { return Person.GetObject (_partnerID); }
    set { _partnerID = ReflectionBusinessObjectStorage.GetID (value); }
  }

  [XmlAttribute]
  public ReflectionDate DateofDeath_Date
  {
    get { return _dateofDeath_Date; }
    set { _dateofDeath_Date = value; }
  }

  public override string DisplayName
  {
    get { return LastName + ", " + FirstName; }
  }

  public override string ToString()
  {
    return DisplayName;
  }


}

public enum Gender
{
  Male,
  Female,
  Disabled_UnknownGender
}

public enum MarriageStatus
{
  Married,
  Single, 
  Divorced,
  Disabled_Bigamist,
  Disabled_Polygamist,
}

}
