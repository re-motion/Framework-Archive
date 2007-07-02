using System;

namespace Rubicon.ObjectBinding.UnitTests.BindableObject.TestDomain
{
  [BindableObject]
  public class ClassWithAllDataTypes
  {
    private bool _boolean;
    private byte _byte;
    private decimal _decimal;
    private double _double;
    private TestEnum _enum;
    private short _int16;
    private int _int32;
    private long _int64;
    private float _single;
    private string _string;

    public ClassWithAllDataTypes ()
    {
    }

    public bool Boolean
    {
      get { return _boolean; }
      set { _boolean = value; }
    }

    public byte Byte
    {
      get { return _byte; }
      set { _byte = value; }
    }

    public TestEnum Enum
    {
      get { return _enum; }
      set { _enum = value; }
    }

    public decimal Decimal
    {
      get { return _decimal; }
      set { _decimal = value; }
    }

    public double Double
    {
      get { return _double; }
      set { _double = value; }
    }

    public short Int16
    {
      get { return _int16; }
      set { _int16 = value; }
    }

    public int Int32
    {
      get { return _int32; }
      set { _int32 = value; }
    }

    public long Int64
    {
      get { return _int64; }
      set { _int64 = value; }
    }

    public float Single
    {
      get { return _single; }
      set { _single = value; }
    }

    public string String
    {
      get { return _string; }
      set { _string = value; }
    }
  }
}