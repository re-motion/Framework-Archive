using System;

namespace Rubicon.CooNet.Gen
{

public class ExtendedVBCodeProvider: ExtendedCodeProvider
{
	public ExtendedVBCodeProvider()
    : base (new Microsoft.VisualBasic.VBCodeProvider())
	{
	}
}

}
