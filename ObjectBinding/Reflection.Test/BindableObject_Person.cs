using System;
using Rubicon.ObjectBinding;

namespace OBRTest
{
  [BindableObject]
  public class BindableObject_Person
  {
    private string _firstName;
    private string _lastName;
    private DateTime _dateOfBirth;
    private int _height;
    private double? _income = 1000.50f;
    private Gender _gender;
    private MarriageStatus _marriageStatus;
    private DateTime _dateOfDeath;
    private bool _deceased = false;

    public BindableObject_Person ()
    {
    }

    public string FirstName
    {
      get { return _firstName; }
      set { _firstName = value; }
    }

    public string LastName
    {
      get { return _lastName; }
      set { _lastName = value; }
    }

    public DateTime DateOfBirth
    {
      get { return _dateOfBirth; }
      set { _dateOfBirth = value; }
    }

    public int Height
    {
      get { return _height; }
      set { _height = value; }
    }

    public double? Income
    {
      get { return _income; }
      set { _income = value; }
    }

    public Gender Gender
    {
      get { return _gender; }
      set { _gender = value; }
    }

    public MarriageStatus MarriageStatus
    {
      get { return _marriageStatus; }
      set { _marriageStatus = value; }
    }

    public DateTime DateOfDeath
    {
      get { return _dateOfDeath; }
      set { _dateOfDeath = value; }
    }

    public bool Deceased
    {
      get { return _deceased; }
      set { _deceased = value; }
    }
  }
}