using System;

namespace Rubicon.CooNet.Gen
{

public class ExtendedVBCodeProvider: ExtendedCodeProvider
{
	public ExtendedVBCodeProvider()
    : base (new Microsoft.VisualBasic.VBCodeProvider())
	{
	}

  public override string GetValidName(string name)
  {
    if (name == "ObjectClass")
      return "[" + name + "]";
    return name;
  }

  public override bool IsCaseSensitive
  {
    get { return false; }
  }
}

}
