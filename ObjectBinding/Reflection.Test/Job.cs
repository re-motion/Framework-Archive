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
public class Job: ReflectionBusinessObject
{
  public static Job GetObject (Guid id)
  {
    ReflectionBusinessObject obj = ReflectionBusinessObjectStorage.GetObject (typeof (Job), id);
    if (obj == null)
      return null;
    return (Job) obj;
  }

  public static Job CreateObject ()
  {
    return (Job) ReflectionBusinessObjectStorage.CreateObject (typeof (Job));
  }

  public static Job CreateObject (Guid id)
  {
    return (Job) ReflectionBusinessObjectStorage.CreateObject (typeof (Job), id);
  }

  private string _title;
  private DateTime _startDate;
  private DateTime _endDate;

  public Job()
	{}

  [XmlAttribute]
  public string Title
  {
    get { return _title; }
    set { _title = value; }
  }

  [XmlAttribute (DataType="date")]
  public DateTime StartDate
  {
    get { return _startDate; }
    set { _startDate = value; }
  }

  [XmlAttribute (DataType="date")]
  public DateTime EndDate
  {
    get { return _endDate; }
    set { _endDate = value; }
  }

  public override string DisplayName
  {
    get { return Title; }
  }

  public override string ToString()
  {
    return DisplayName;
  }
}
}
