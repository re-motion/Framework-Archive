using System;
using NUnit.Framework;
using Rubicon.Text;

namespace Rubicon.Core.UnitTests.Text
{

[TestFixture]
public class IdentifierGeneratorTest
{
  [Test]
  [ExpectedException (typeof (InvalidOperationException))]
	public void TestUseTemplateGenerator()
	{
    IdentifierGenerator idGen = IdentifierGenerator.CStyle;
    idGen.GetUniqueIdentifier ("some name");
  }

  [Test]
  [ExpectedException (typeof (InvalidOperationException))]
	public void TestChangeUseCaseSensitiveNamesAfterGeneratingUniqueIdentifier()
	{
    IdentifierGenerator idGen = IdentifierGenerator.CStyle.Clone();
    idGen.GetUniqueIdentifier ("some name");
    idGen.UseCaseSensitiveNames = false;
  }

  [Test]
	public void TestCStyle()
	{
    IdentifierGenerator idGen = IdentifierGenerator.CStyle.Clone();

    CheckValidIdentifier (idGen, "myid", "myid");
    CheckValidIdentifier (idGen, "3myid", "_myid");
    CheckValidIdentifier (idGen, "_myid3", "_myid3");
    CheckValidIdentifier (idGen, "myid�", "myid_");

    CheckUniqueIdentifier (idGen, "my�id", "my_id");
    CheckUniqueIdentifier (idGen, "My�id", "My_id");
    CheckUniqueIdentifier (idGen, "my�id", "my_id_1");
    CheckUniqueIdentifier (idGen, "my�id", "my_id_2");
    CheckUniqueIdentifier (idGen, "my�id", "my_id");
    CheckUniqueIdentifier (idGen, "my�id", "my_id_1");
	}

  [Test]
	public void TestUniqueObjects()
	{
    IdentifierGenerator idGen = IdentifierGenerator.CStyle.Clone();
    object o1 = new object(), o2 = new object(), o3 = new object();

    CheckUniqueIdentifier (idGen, o1, "my_id", "my_id");
    CheckUniqueIdentifier (idGen, o2, "my_id", "my_id_1");
    CheckUniqueIdentifier (idGen, o3, "my_id", "my_id_2");
    CheckUniqueIdentifier (idGen, o1, "xxx", "my_id");
    CheckUniqueIdentifier (idGen, o2, "xxx", "my_id_1");
    CheckUniqueIdentifier (idGen, o3, "xxx", "my_id_2");
	}

  [Test]
	public void TestHtmlStyle()
	{
    IdentifierGenerator idGen = IdentifierGenerator.HtmlStyle.Clone();
    idGen.UseCaseSensitiveNames = false;

    CheckValidIdentifier (idGen, "myid", "myid");
    CheckValidIdentifier (idGen, "_myid", "myid");
    CheckValidIdentifier (idGen, "myid3", "myid3");
    CheckValidIdentifier (idGen, "myid�", "myid_");

    CheckUniqueIdentifier (idGen, "my�id", "my_id");
    CheckUniqueIdentifier (idGen, "My�id", "my_id");
    CheckUniqueIdentifier (idGen, "my�id", "my_id_1");
    CheckUniqueIdentifier (idGen, "my�id", "my_id_2");
    CheckUniqueIdentifier (idGen, "my�id", "my_id");
    CheckUniqueIdentifier (idGen, "my�id", "my_id_1");
	}

  [Test]
	public void TestXmlStyle()
	{
    IdentifierGenerator idGen = IdentifierGenerator.XmlStyle.Clone();

    CheckValidIdentifier (idGen, "myid", "myid");
    CheckValidIdentifier (idGen, "_myid", "_myid");
    CheckValidIdentifier (idGen, "-myid", "_myid");
    CheckValidIdentifier (idGen, "myid3", "myid3");
    CheckValidIdentifier (idGen, "myid�", "myid_");

    CheckUniqueIdentifier (idGen, "my�id", "my_id");
    CheckUniqueIdentifier (idGen, "My�id", "My_id_1");
    CheckUniqueIdentifier (idGen, "my�id", "my_id_2");
    CheckUniqueIdentifier (idGen, "my�id", "my_id_3");
    CheckUniqueIdentifier (idGen, "my�id", "my_id");
    CheckUniqueIdentifier (idGen, "my�id", "my_id_2");
	}

  public void CheckValidIdentifier (IdentifierGenerator idGen, string uniqueName, string expectedIdentifier)
  {
    Assertion.AssertEquals (expectedIdentifier, idGen.GetValidIdentifier (uniqueName));
  }

  public void CheckUniqueIdentifier (IdentifierGenerator idGen, string uniqueName, string expectedIdentifier)
  {
    Assertion.AssertEquals (expectedIdentifier, idGen.GetUniqueIdentifier (uniqueName));
  }

  public void CheckUniqueIdentifier (IdentifierGenerator idGen, object uniqueObject, string name, string expectedIdentifier)
  {
    Assertion.AssertEquals (expectedIdentifier, idGen.GetUniqueIdentifier (uniqueObject, name));
  }
}

}
