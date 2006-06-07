using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;

using Rubicon.Data.DomainObjects;
using Rubicon.Utilities;

namespace Rubicon.SecurityManager.Domain.UnitTests.Metadata
{
  [CLSCompliant (false)]
  public class DomainObjectCollectionContainsAsserter : AbstractAsserter
  {
    private DomainObjectFilterCriteria _filterCriteria;
    private DomainObjectCollection _actualCollection;

    public DomainObjectCollectionContainsAsserter (
        DomainObjectFilterCriteria filterCriteria,
        DomainObjectCollection actualCollection,
        string message,
        params object[] args)
      : base (message, args)
    {
      ArgumentUtility.CheckNotNull ("filterCriteria", filterCriteria);
      ArgumentUtility.CheckNotNull ("actualCollection", actualCollection);

      _filterCriteria = filterCriteria;
      _actualCollection = actualCollection;
    }

    public override bool Test ()
    {
      DomainObjectFilter filter = new DomainObjectFilter (_actualCollection);
      DomainObjectCollection result = filter.GetObjects (_filterCriteria);
      return result.Count > 0;
    }
  }
}
