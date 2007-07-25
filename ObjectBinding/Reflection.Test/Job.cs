using System;
using System.Xml.Serialization;
using Rubicon.ObjectBinding.Reflection;

namespace OBRTest
{
  [XmlType]
  [Serializable]
  public class Job : BindableXmlObject
  {
    public static Job GetObject (Guid id)
    {
      Job job = GetObject<Job> (id);
      if (job == null)
        return null;
      return job;
    }

    public static Job CreateObject ()
    {
      return CreateObject<Job>();
    }

    public static Job CreateObject (Guid id)
    {
      return CreateObject<Job> (id);
    }

    private string _title;
    private DateTime _startDate;
    private DateTime _endDate;

    protected Job ()
    {
    }

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

    public override string ToString ()
    {
      return DisplayName;
    }
  }
}