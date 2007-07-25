using System;
using System.ComponentModel;
using System.Xml.Serialization;
using Rubicon.ObjectBinding.Reflection;
using Rubicon.Utilities;

namespace OBRTest
{
  [XmlType]
  [Serializable]
  public class Person : BindableXmlObject
  {
    public static Person GetObject (Guid id)
    {
      Person person = GetObject<Person> (id);

      if (person == null)
        return null;

      if (person.PartnerID != Guid.Empty)
        person.Partner = Person.GetObject (person.PartnerID);

      return person;
    }

    public static Person CreateObject ()
    {
      return CreateObject<Person>();
    }

    public static Person CreateObject (Guid id)
    {
      return CreateObject<Person> (id);
    }

    private string _firstName;
    private string _lastName;
    private DateTime _dateOfBirth;
    private int _height;
    private double? _income = 1000.50f;
    private Gender _gender;
    private MarriageStatus _marriageStatus;
    private DateTime _dateOfDeath;
    private bool _deceased = false;
    private string[] _cv;
    private Guid _partnerID;
    private Guid[] _childIDs;
    private Guid[] _jobIDs;

    protected Person ()
    {
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
    public double? Income
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

    [XmlElement]
    public Guid PartnerID
    {
      get { return _partnerID; }
      set { _partnerID = value; }
    }

    [XmlIgnore]
    public Person Partner
    {
      get { return Person.GetObject (_partnerID); }
      set { _partnerID = (value != null) ? value.ID : Guid.Empty; }
    }

    [XmlElement]
    public Guid[] ChildIDs
    {
      get { return _childIDs; }
      set { _childIDs = value; }
    }

    [XmlIgnore]
    public Person[] Children
    {
      get
      {
        if (_childIDs == null)
          return new Person[0];

        Person[] children = new Person[_childIDs.Length];
        for (int i = 0; i < _childIDs.Length; i++)
          children[i] = Person.GetObject (_childIDs[i]);

        return children;
      }
      set
      {
        if (value != null)
        {
          ArgumentUtility.CheckNotNullOrItemsNull ("value", value);
          _childIDs = new Guid[value.Length];
          for (int i = 0; i < value.Length; i++)
            _childIDs[i] = value[i].ID;
        }
        else
        {
          _childIDs = new Guid[0];
        }
      }
    }

    [XmlElement]
    public Guid[] JobIDs
    {
      get { return _jobIDs; }
      set { _jobIDs = value; }
    }

    [XmlIgnore]
    public Job[] Jobs
    {
      get
      {
        if (_jobIDs == null)
          return new Job[0];

        Job[] jobs = new Job[_jobIDs.Length];
        for (int i = 0; i < _jobIDs.Length; i++)
          jobs[i] = Job.GetObject (_jobIDs[i]);

        return jobs;
      }
      set
      {
        if (value != null)
        {
          ArgumentUtility.CheckNotNullOrItemsNull ("value", value);
          _jobIDs = new Guid[value.Length];
          for (int i = 0; i < value.Length; i++)
            _jobIDs[i] = value[i].ID;
        }
        else
        {
          _jobIDs = new Guid[0];
        }
      }
    }

    [XmlAttribute (DataType="date")]
    public DateTime DateOfDeath
    {
      get { return _dateOfDeath; }
      set { _dateOfDeath = value; }
    }

    [XmlElement]
    public bool Deceased
    {
      get { return _deceased; }
      set { _deceased = value; }
    }

    [XmlElement]
    public string[] CV
    {
      get { return _cv; }
      set { _cv = value; }
    }

    public string CVString
    {
      get
      {
        if (_cv == null)
          return null;
        return string.Join ("<br/>", _cv);
      }
    }

    public override string DisplayName
    {
      get { return LastName + ", " + FirstName; }
    }

    public override string ToString ()
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