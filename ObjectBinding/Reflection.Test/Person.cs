using System;
using System.Xml.Serialization;
using System.IO;
using Rubicon.ObjectBinding.Reflection;

namespace BocTest
{

[XmlType]
public class Person: ReflectionBusinessObject
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
}

}
