using System;
using NUnit.Framework;
using Rubicon.Development.UnitTesting;

namespace Rubicon.Web.Test.UI.Controls
{

[TestFixture]
public class EmailAddressValidatorTest
{
  private EmailAddressValidatorMock _validator;

  [SetUp]
  public virtual void SetUp()
  {
    _validator = new EmailAddressValidatorMock();
  }

	[Test]
  public void MatchValidUserPartAllLowerCase()
  {
    string text = @"jdoe";
    bool result = _validator.IsMatchUserPart (emailAddress);
    Assert.AreEqual (true, result);
  }

	[Test]
  public void MatchValidUserPartWithCapitalCharacterTheInFront()
  {
    string text = @"Jdoe";
    bool result = _validator.IsMatchUserPart (emailAddress);
    Assert.AreEqual (true, result);
  }

	[Test]
  public void MatchValidUserPartWithCapitalCharacterInTheMiddle()
  {
    string text = @"jDoe";
    bool result = _validator.IsMatchUserPart (emailAddress);
    Assert.AreEqual (true, result);
  }

	[Test]
  public void MatchValidUserPartWithCapitalCharacterInTheEnd()
  {
    string text = @"jdoE";
    bool result = _validator.IsMatchUserPart (emailAddress);
    Assert.AreEqual (true, result);
  }

	[Test]
  public void MatchValidUserPartWithNumberInTheFront()
  {
    string text = @"2jdoe";
    bool result = _validator.IsMatchUserPart (emailAddress);
    Assert.AreEqual (true, result);
  }

	[Test]
  public void MatchValidUserPartWithNumberInTheMiddle()
  {
    string text = @"j2doe";
    bool result = _validator.IsMatchUserPart (emailAddress);
    Assert.AreEqual (true, result);
  }

	[Test]
  public void MatchValidUserPartWithNumberInTheEnd()
  {
    string text = @"jdoe2";
    bool result = _validator.IsMatchUserPart (emailAddress);
    Assert.AreEqual (true, result);
  }

	[Test]
  public void MatchValidUserPartWithUnderscoreInTheFront()
  {
    string text = @"_jdoe";
    bool result = _validator.IsMatchUserPart (emailAddress);
    Assert.AreEqual (true, result);
  }

	[Test]
  public void MatchValidUserPartWithUnderscoreInTheMiddle()
  {
    string text = @"j_doe";
    bool result = _validator.IsMatchUserPart (emailAddress);
    Assert.AreEqual (true, result);
  }

	[Test]
  public void MatchValidUserPartWithUnderscoreInTheEnd()
  {
    string text = @"jdoe_";
    bool result = _validator.IsMatchUserPart (emailAddress);
    Assert.AreEqual (true, result);
  }

	[Test]
  public void MatchValidUserPartWithUmlautInTheFront()
  {
    string text = @"äjdoe";
    bool result = _validator.IsMatchUserPart (emailAddress);
    Assert.AreEqual (true, result);
  }

	[Test]
  public void MatchValidUserPartWithUmlautInTheMiddle()
  {
    string text = @"jädoe";
    bool result = _validator.IsMatchUserPart (emailAddress);
    Assert.AreEqual (true, result);
  }

	[Test]
  public void MatchValidUserPartWithUmlautInTheEnd()
  {
    string text = @"jdoeä";
    bool result = _validator.IsMatchUserPart (emailAddress);
    Assert.AreEqual (true, result);
  }

	[Test]
  public void MatchValidUserPartWithHyphen()
  {
    string text = @"j-doe";
    bool result = _validator.IsMatchUserPart (emailAddress);
    Assert.AreEqual (true, result);
  }

	[Test]
  public void MatchValidUserPartWithDot()
  {
    string text = @"j.doe";
    bool result = _validator.IsMatchUserPart (emailAddress);
    Assert.AreEqual (true, result);
  }

	[Test]
  public void MatchEmailAddressValid2()
  {
    string emailAddress = @"john.doe@provider.net";
    bool result = _validator.IsMatch (emailAddress);
    Assert.AreEqual (true, result);
  }

	[Test]
  public void MatchEmailAddressValid3()
  {
    string emailAddress = @"john-doe@provider.net";
    bool result = _validator.IsMatch (emailAddress);
    Assert.AreEqual (true, result);
  }

	[Test]
  public void MatchEmailAddressValid4()
  {
    string emailAddress = @"jdoe@national.provider.net";
    bool result = _validator.IsMatch (emailAddress);
    Assert.AreEqual (true, result);
  }

	[Test]
  public void MatchEmailAddressValid5()
  {
    string emailAddress = @"jdoe@national-provider.net";
    bool result = _validator.IsMatch (emailAddress);
    Assert.AreEqual (true, result);
  }

	[Test]
  public void MatchEmailAddressValid6()
  {
    string emailAddress = @"jdoe@national-provider.at";
    bool result = _validator.IsMatch (emailAddress);
    Assert.AreEqual (true, result);
  }

	[Test]
  public void MatchEmailAddressValid7()
  {
    string emailAddress = @"jdoe@national-provider.abcdefghi";
    bool result = _validator.IsMatch (emailAddress);
    Assert.AreEqual (true, result);
  }

	[Test]
  public void MatchEmailAddressValid8()
  {
    string emailAddress = @"2jdoe@nationalprovider.net";
    bool result = _validator.IsMatch (emailAddress);
    Assert.AreEqual (true, result);
  }

	[Test]
  public void MatchEmailAddressValid9()
  {
    string emailAddress = @"jdoe@2nationalprovider.net";
    bool result = _validator.IsMatch (emailAddress);
    Assert.AreEqual (true, result);
  }

	[Test]
  public void MatchEmailAddressValid10()
  {
    string emailAddress = @"j_doe@nationalprovider.net";
    bool result = _validator.IsMatch (emailAddress);
    Assert.AreEqual (true, result);
  }

	[Test]
  public void MatchEmailAddressValid11()
  {
    string emailAddress = @"jdoe@national_provider.net";
    bool result = _validator.IsMatch (emailAddress);
    Assert.AreEqual (true, result);
  }

	[Test]
  public void MatchEmailAddressValid12()
  {
    string emailAddress = @"jdoe@nat_ional.prov_ider.net";
    bool result = _validator.IsMatch (emailAddress);
    Assert.AreEqual (true, result);
  }

	[Test]
  public void MatchEmailAddressMissing1()
  {
    string emailAddress = @"jdoenationalprovider.net";
    bool result = _validator.IsMatch (emailAddress);
    Assert.AreEqual (false, result);
  }

	[Test]
  public void MatchEmailAddressMissing2()
  {
    string emailAddress = @"jdoe@nationalprovidernet";
    bool result = _validator.IsMatch (emailAddress);
    Assert.AreEqual (false, result);
  }

	[Test]
  public void MatchEmailAddressMissing3()
  {
    string emailAddress = @"jdoe@nationalprovider.n";
    bool result = _validator.IsMatch (emailAddress);
    Assert.AreEqual (false, result);
  }

	[Test]
  public void MatchEmailAddressMissing4()
  {
    string emailAddress = @"jdoe@nationalprovider.";
    bool result = _validator.IsMatch (emailAddress);
    Assert.AreEqual (false, result);
  }

	[Test]
  public void MatchEmailAddressMissing5()
  {
    string emailAddress = @"@nationalprovider.";
    bool result = _validator.IsMatch (emailAddress);
    Assert.AreEqual (false, result);
  }

	[Test]
  public void MatchEmailAddressInvalid1()
  {
    string emailAddress = @"jdoe@nationalprovider-net";
    bool result = _validator.IsMatch (emailAddress);
    Assert.AreEqual (false, result);
  }

	[Test]
  public void MatchEmailAddressInvalid2()
  {
    string emailAddress = @"_jdoe@nationalprovider.net";
    bool result = _validator.IsMatch (emailAddress);
    Assert.AreEqual (false, result);
  }

	[Test]
  public void MatchEmailAddressInvalid3()
  {
    string emailAddress = @"jdoe@_nationalprovider.net";
    bool result = _validator.IsMatch (emailAddress);
    Assert.AreEqual (false, result);
  }

	[Test]
  public void MatchEmailAddressInvalid4()
  {
    string emailAddress = @"jdoe_@nationalprovider.net";
    bool result = _validator.IsMatch (emailAddress);
    Assert.AreEqual (false, result);
  }

	[Test]
  public void MatchEmailAddressInvalid5()
  {
    string emailAddress = @"jdoe@nationalprovider_.net";
    bool result = _validator.IsMatch (emailAddress);
    Assert.AreEqual (false, result);
  }

	[Test]
  public void MatchEmailAddressInvalid6()
  {
    string emailAddress = @" jdoe@nationalprovider.net";
    bool result = _validator.IsMatch (emailAddress);
    Assert.AreEqual (false, result);
  }

	[Test]
  public void MatchEmailAddressInvalid7()
  {
    string emailAddress = @"jdoe@nationalprovider.net ";
    bool result = _validator.IsMatch (emailAddress);
    Assert.AreEqual (false, result);
  }

	[Test]
  public void MatchEmailAddressInvalid8()
  {
    string emailAddress = @"jdöe@nationalprovider.net";
    bool result = _validator.IsMatch (emailAddress);
    Assert.AreEqual (false, result);
  }

	[Test]
  public void MatchEmailAddressInvalid9()
  {
    string emailAddress = @"jdoe@nätionalprovider.net";
    bool result = _validator.IsMatch (emailAddress);
    Assert.AreEqual (false, result);
  }
}

}
