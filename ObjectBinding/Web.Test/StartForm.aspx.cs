using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using Rubicon.Web.ExecutionEngine;

using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Diagnostics;
using Rubicon.NullableValueTypes;
using System.Text.RegularExpressions;

namespace OBWTest
{
public class StartForm : System.Web.UI.Page
{
  protected System.Web.UI.WebControls.Button Button1;
  protected Rubicon.Web.UI.Controls.HtmlHeadContents HtmlHeadContents;

	#region Web Form Designer generated code
	override protected void OnInit(EventArgs e)
	{
		//
		// CODEGEN: This call is required by the ASP.NET Web Form Designer.
		//
		InitializeComponent();
		base.OnInit(e);
	}
	
	/// <summary>
	/// Required method for Designer support - do not modify
	/// the contents of this method with the code editor.
	/// </summary>
	private void InitializeComponent()
	{    
    this.Load += new System.EventHandler(this.Page_Load);

  }
	#endregion

	private void Page_Load(object sender, System.EventArgs e)
	{
  }

  private void SerializeTestFunction()
  {
    BinaryFormatter formatter = new BinaryFormatter();
    Stream stream = new MemoryStream();
    formatter.Serialize(stream, new object[] {new SerializeTestFunction()});
    stream.Position = 0;
    object[] deserialized = (object[]) formatter.Deserialize(stream);
  }

  private void SerializeTestNaTypes()
  {
    NaS1[] nas1 = new NaS1[1000];
    for (int i = 0; i < nas1.Length; i++)
      nas1[i] = new NaS1();

    NaS2[] nas2 = new NaS2[1000];
    for (int i = 0; i < nas2.Length; i++)
      nas2[i] = new NaS2();

    object[] naTypes = new object[18000];
    for (int i = 0; i < naTypes.Length; i+=18)
    {
      naTypes[i+0] = new NaBoolean(true);
      naTypes[i+1] = new NaByte(7);
      naTypes[i+2] = new NaDateTime(100000000);
      naTypes[i+3] = new NaDecimal(100000000);
      naTypes[i+4] = new NaDouble(1000000000);
      naTypes[i+5] = new NaInt16(10000);
      naTypes[i+6] = new NaInt32(10000000);
      naTypes[i+7] = new NaInt64(10000000000);
      naTypes[i+8] = new NaSingle(100000000);

      naTypes[i+9] = NaBoolean.Null;
      naTypes[i+10] = NaByte.Null;
      naTypes[i+11] = NaDateTime.Null;
      naTypes[i+12] = NaDecimal.Null;
      naTypes[i+13] = NaDouble.Null;
      naTypes[i+14] = NaInt16.Null;
      naTypes[i+15] = NaInt32.Null;
      naTypes[i+16] = NaInt64.Null;
      naTypes[i+17] = NaSingle.Null;
    }

    long start;
    long end;

    Debug.WriteLine("");

    BinaryFormatter nas1formatter = new BinaryFormatter();
    Stream nas1Stream = new MemoryStream();
    start = DateTime.Now.Ticks;
    nas1formatter.Serialize(nas1Stream, nas1);
    end = DateTime.Now.Ticks;
    Debug.WriteLine (string.Format ("Serializing Nullables in class interface version: {0} ms", (end - start)/10000));

    BinaryFormatter nas2formatter = new BinaryFormatter();
    Stream nas2Stream = new MemoryStream();
    start = DateTime.Now.Ticks;
    nas2formatter.Serialize(nas2Stream, nas2);
    end = DateTime.Now.Ticks;
    Debug.WriteLine (string.Format ("Serializing Nullables in class attribute version: {0} ms", (end - start)/10000));

    nas1Stream.Position = 0;
    start = DateTime.Now.Ticks;
    NaS1[] nas1Deserialized = (NaS1[]) nas1formatter.Deserialize(nas1Stream);
    end = DateTime.Now.Ticks;
    Debug.WriteLine (string.Format ("Deserializing Nullables in class interface version: {0} ms", (end - start)/10000));

    nas2Stream.Position = 0;
    start = DateTime.Now.Ticks;
    NaS2[] nas2Deserialized = (NaS2[]) nas2formatter.Deserialize(nas2Stream);
    end = DateTime.Now.Ticks;
    Debug.WriteLine (string.Format ("Deserializing Nullables in class attribute: {0} ms", (end - start)/10000));

    BinaryFormatter naTypesformatter = new BinaryFormatter();
    Stream naTypesStream = new MemoryStream();
    start = DateTime.Now.Ticks;
    naTypesformatter.Serialize(naTypesStream, naTypes);
    end = DateTime.Now.Ticks;
    Debug.WriteLine (string.Format ("Serializing Nullables: {0} ms", (end - start)/10000));

    naTypesStream.Position = 0;
    start = DateTime.Now.Ticks;
    object[] naTypesDeserialized = (object[]) naTypesformatter.Deserialize(naTypesStream);
    end = DateTime.Now.Ticks;
    Debug.WriteLine (string.Format ("Deserializing Nullables {0} ms", (end - start)/10000));

    Debug.WriteLine("");
  }

  private void SerializeTest()
  {
    S1[] s1 = new S1[100000];
    for (int i = 0; i < s1.Length; i++)
      s1[i] = new S1 (i, "test test test test", "test test test test", "test test test test");

    S2[] s2 = new S2[100000];
    for (int i = 0; i < s1.Length; i++)
      s2[i] = new S2 (i, "test test test test", "test test test test", "test test test test");

    long start;
    long end;

    Debug.WriteLine("");

    BinaryFormatter s1formatter = new BinaryFormatter();
    Stream s1Stream = new MemoryStream();
    start = DateTime.Now.Ticks;
    s1formatter.Serialize(s1Stream, s1);
    end = DateTime.Now.Ticks;
    Debug.WriteLine (string.Format ("Serializing Interface version: {0} ms", (end - start)/10000));
    
    BinaryFormatter s2formatter = new BinaryFormatter();
    Stream s2Stream = new MemoryStream();
    start = DateTime.Now.Ticks;
    s2formatter.Serialize(s2Stream, s2);
    end = DateTime.Now.Ticks;
    Debug.WriteLine (string.Format ("Serializing Attribute version: {0} ms", (end - start)/10000));

    s1Stream.Position = 0;
    start = DateTime.Now.Ticks;
    S1[] s1Deserialized = (S1[]) s1formatter.Deserialize(s1Stream);
    end = DateTime.Now.Ticks;
    Debug.WriteLine (string.Format ("Deserializing Interface version: {0} ms", (end - start)/10000));

    s2Stream.Position = 0;
    start = DateTime.Now.Ticks;
    S2[] s2Deserialized = (S2[]) s2formatter.Deserialize(s2Stream);
    end = DateTime.Now.Ticks;
    Debug.WriteLine (string.Format ("Deserializing Attribute version: {0} ms", (end - start)/10000));

    Debug.WriteLine("");
  }
}

[Serializable]
public class SerializeTestFunction: WxeFunction
{
  private void Step1()
  {
  }
}

[Serializable]
public class NaS1: ISerializable
{
  private NaBoolean _naBoolean;
  private NaByte _naByte;
  private NaDateTime _naDateTime;
  private NaDecimal _naDecimal;
  private NaDouble _naDouble;
  private NaInt16 _naInt16;
  private NaInt32 _naInt32;
  private NaInt64 _naInt64;
  private NaSingle _naSingle;

  private NaBoolean _naBooleanNull;
  private NaByte _naByteNull;
  private NaDateTime _naDateTimeNull;
  private NaDecimal _naDecimalNull;
  private NaDouble _naDoubleNull;
  private NaInt16 _naInt16Null;
  private NaInt32 _naInt32Null;
  private NaInt64 _naInt64Null;
  private NaSingle _naSingleNull;

  public NaS1 ()
  {
    _naBoolean = true;
    _naByte = 7;
    _naDateTime = new DateTime (2000, 12, 31);
    _naDecimal = 1000000m;
    _naDouble = 10000000;
    _naInt16 = 10000;
    _naInt32 = 10000000;
    _naInt64 = 1000000000000000;
    _naSingle = 10000000000;

    _naBooleanNull = NaBoolean.Null;
    _naByteNull = NaByte.Null;
    _naDateTimeNull = NaDateTime.Null;
    _naDecimalNull = NaDecimal.Null;
    _naDoubleNull = NaDouble.Null;
    _naInt16Null = NaInt16.Null;
    _naInt32Null = NaInt32.Null;
    _naInt64Null = NaInt64.Null;
    _naSingleNull = NaSingle.Null;
  }

  protected NaS1 (SerializationInfo info, StreamingContext context)
  {
    _naBoolean = (NaBoolean)info.GetValue ("_naBoolean", typeof (NaBoolean));
    _naByte = (NaByte)info.GetValue ("_naByte", typeof (NaByte));
    _naDateTime = (NaDateTime)info.GetValue ("_naDateTime", typeof (NaDateTime));
    _naDouble = (NaDouble)info.GetValue ("_naDouble", typeof (NaDouble));
    _naInt16 = (NaInt16)info.GetValue ("_naInt16", typeof (NaInt16));
    _naInt32 = (NaInt32)info.GetValue ("_naInt32", typeof (NaInt32));
    _naInt64 = (NaInt64)info.GetValue ("_naInt64", typeof (NaInt64));
    _naSingle = (NaSingle)info.GetValue ("_naSingle", typeof (NaSingle));

    _naBooleanNull = (NaBoolean)info.GetValue ("_naBooleanNull", typeof (NaBoolean));
    _naByteNull = (NaByte)info.GetValue ("_naByteNull", typeof (NaByte));
    _naDateTimeNull = (NaDateTime)info.GetValue ("_naDateTimeNull", typeof (NaDateTime));
    _naDoubleNull = (NaDouble)info.GetValue ("_naDoubleNull", typeof (NaDouble));
    _naInt16Null = (NaInt16)info.GetValue ("_naInt16Null", typeof (NaInt16));
    _naInt32Null = (NaInt32)info.GetValue ("_naInt32Null", typeof (NaInt32));
    _naInt64Null = (NaInt64)info.GetValue ("_naInt64Null", typeof (NaInt64));
    _naSingleNull = (NaSingle)info.GetValue ("_naSingleNull", typeof (NaSingle));
  }

  void ISerializable.GetObjectData (SerializationInfo info, StreamingContext context)
  {
    info.AddValue ("_naBoolean", _naBoolean);
    info.AddValue ("_naByte", _naByte);
    info.AddValue ("_naDateTime", _naDateTime);
    info.AddValue ("_naDouble", _naDouble);
    info.AddValue ("_naInt16", _naInt16);
    info.AddValue ("_naInt32", _naInt32);
    info.AddValue ("_naInt64", _naInt64);
    info.AddValue ("_naSingle", _naSingle);

    info.AddValue ("_naBooleanNull", _naBooleanNull);
    info.AddValue ("_naByteNull", _naByteNull);
    info.AddValue ("_naDateTimeNull", _naDateTimeNull);
    info.AddValue ("_naDoubleNull", _naDoubleNull);
    info.AddValue ("_naInt16Null", _naInt16Null);
    info.AddValue ("_naInt32Null", _naInt32Null);
    info.AddValue ("_naInt64Null", _naInt64Null);
    info.AddValue ("_naSingleNull", _naSingleNull);
  }
}

[Serializable]
public class NaS2
{
  private NaBoolean _naBoolean;
  private NaByte _naByte;
  private NaDateTime _naDateTime;
  private NaDecimal _naDecimal;
  private NaDouble _naDouble;
  private NaInt16 _naInt16;
  private NaInt32 _naInt32;
  private NaInt64 _naInt64;
  private NaSingle _naSingle;

  private NaBoolean _naBooleanNull;
  private NaByte _naByteNull;
  private NaDateTime _naDateTimeNull;
  private NaDecimal _naDecimalNull;
  private NaDouble _naDoubleNull;
  private NaInt16 _naInt16Null;
  private NaInt32 _naInt32Null;
  private NaInt64 _naInt64Null;
  private NaSingle _naSingleNull;

  public NaS2 ()
  {
    _naBoolean = true;
    _naByte = 7;
    _naDateTime = new DateTime (2000, 12, 31);
    _naDecimal = 1000000m;
    _naDouble = 10000000;
    _naInt16 = 10000;
    _naInt32 = 10000000;
    _naInt64 = 1000000000000000;
    _naSingle = 10000000000;

    _naBooleanNull = NaBoolean.Null;
    _naByteNull = NaByte.Null;
    _naDateTimeNull = NaDateTime.Null;
    _naDecimalNull = NaDecimal.Null;
    _naDoubleNull = NaDouble.Null;
    _naInt16Null = NaInt16.Null;
    _naInt32Null = NaInt32.Null;
    _naInt64Null = NaInt64.Null;
    _naSingleNull = NaSingle.Null;
  }
}

[Serializable]
public class S1: ISerializable
{
  private int _int1;
  private string _string1;
  private string _string2;
  private string _string3;

  public S1 (int int1, string string1, string string2, string string3)
  {
    _int1 = int1;
    _string1 = string1;
    _string2 = string3; 
    _string2 = string3;
  }

  protected S1 (SerializationInfo info, StreamingContext context)
  {
    _int1 = info.GetInt32 ("_int1");
    _string1 = info.GetString ("_string1");
    _string2 = info.GetString ("_string2");
    _string3 = info.GetString ("_string3");
  }

  void ISerializable.GetObjectData (SerializationInfo info, StreamingContext context)
  {
    info.AddValue ("_int1", _int1);
    info.AddValue ("_string1", _string1);
    info.AddValue ("_string2", _string2);
    info.AddValue ("_string3", _string3);
  }

  public int P1 {get{return 0;} set {}}
  public int P2 {get{return 0;} set {}}
  public int P3 {get{return 0;} set {}}
  public int P4 {get{return 0;} set {}}
  public int P5 {get{return 0;} set {}}
  public int P6 {get{return 0;} set {}}

  public int M1 (int i1, int i2, int i3) { return 0; }
  public int M2 (int i1, int i2, int i3) { return 0; }
  public int M3 (int i1, int i2, int i3) { return 0; }
  public int M4 (int i1, int i2, int i3) { return 0; }
  public int M5 (int i1, int i2, int i3) { return 0; }
  public int M6 (int i1, int i2, int i3) { return 0; }
}

[Serializable]
public class S2
{
  private int _int1;
  private string _string1;
  private string _string2;
  private string _string3;

  public S2 (int int1, string string1, string string2, string string3)
  {
    _int1 = int1;
    _string1 = string1;
    _string2 = string3; 
    _string3 = string3;
  }

  protected S2 (SerializationInfo info, StreamingContext context)
  {
  }

  private void GetObjectData (SerializationInfo info, StreamingContext context){}

  public int P1 {get{return 0;} set {}}
  public int P2 {get{return 0;} set {}}
  public int P3 {get{return 0;} set {}}
  public int P4 {get{return 0;} set {}}
  public int P5 {get{return 0;} set {}}
  public int P6 {get{return 0;} set {}}

  public int M1 (int i1, int i2, int i3) { return 0; }
  public int M2 (int i1, int i2, int i3) { return 0; }
  public int M3 (int i1, int i2, int i3) { return 0; }
  public int M4 (int i1, int i2, int i3) { return 0; }
  public int M5 (int i1, int i2, int i3) { return 0; }
  public int M6 (int i1, int i2, int i3) { return 0; }
}
}
