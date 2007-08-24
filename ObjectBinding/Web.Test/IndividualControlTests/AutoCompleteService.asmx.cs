using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Web.Services;
using Rubicon.ObjectBinding;
using Rubicon.ObjectBinding.Sample;
using Rubicon.ObjectBinding.Web;

namespace OBWTest.IndividualControlTests
{
  /// <summary>
  /// Summary description for AutoCompleteService
  /// </summary>
  [WebService (Namespace = "http://tempuri.org/")]
  [WebServiceBinding (ConformsTo = WsiProfiles.BasicProfile1_1)]
  [ToolboxItem (false)]
  [System.Web.Script.Services.ScriptService]
  public class AutoCompleteService : System.Web.Services.WebService
  {
    #region Values

    private static string[] s_values = new string[]
        {
            "sdfg",
            "sdfgh",
            "sdfghj",
            "sdfghjk",
            "sdfghjkl",
            "sdfg 0qqqqwwww",
            "sdfg 1qqqqwwww",
            "sdfg 2qqqqwwww",
            "sdfg 3qqqqwwww",
            "sdfg 4qqqqwwww",
            "sdfg 5qqqqwwww",
            "sdfg 7qqqqwwww",
            "sdfg 8qqqqwwww",
            "sdfg 9qqqqwwww",
            "sdfg q",
            "sdfg qq",
            "sdfg qqq",
            "sdfg qqqq",
            "sdfg qqqqq",
            "sdfg qqqqqq",
            "sdfg qqqqqqq",
            "sdfg qqqqqqqq",
            "sdfg qqqqqqqqq",
            "sdfg qqqqqqqqqq",
            "sdfg qqqqqqqqqqq",
            "access control list (ACL)",
            "ADO.NET",
            "aggregate event",
            "alpha channel",
            "anchoring",
            "antialiasing",
            "application base",
            "application domain (AppDomain)",
            "application manifest",
            "application state",
            "ASP.NET",
            "ASP.NET application services database",
            "ASP.NET mobile controls",
            "ASP.NET mobile Web Forms",
            "ASP.NET page",
            "ASP.NET server control",
            "ASP.NET Web application",
            "assembly",
            "assembly cache",
            "assembly manifest",
            "assembly metadata",
            "assertion (Assert)",
            "association class",
            "ASSOCIATORS OF",
            "asynchronous method",
            "attribute",
            "authentication",
            "authorization",
            "autopostback",
            "bounds",
            "boxing",
            "C#",
            "card",
            "catalog",
            "CCW",
            "chevron",
            "chrome",
            "cHTML",
            "CIM",
            "CIM Object Manager",
            "CIM schema",
            "class",
            "client area",
            "client coordinates",
            "clip",
            "closed generic type",
            "CLR",
            "CLS",
            "CLS-compliant",
            "code access security",
            "code-behind class",
            "code-behind file",
            "code-behind page",
            "COM callable wrapper (CCW)",
            "COM interop",
            "Common Information Model (CIM)",
            "common language runtime",
            "common language runtime host",
            "Common Language Specification (CLS)",
            "common object file format (COFF)",
            "common type system (CTS)",
            "comparison evaluator",
            "composite control",
            "configuration file",
            "connection",
            "connection point",
            "constraint",
            "constructed generic type",
            "constructed type",
            "consumer",
            "container",
            "container control",
            "content page",
            "context",
            "context property",
            "contract",
            "control state",
            "cross-page posting",
            "CTS",
            "custom attribute (Attribute)",
            "custom control"
        };

    #endregion

    [WebMethod]
    public BusinessObjectWithIdentityProxy[] GetPersonList (
        string prefixText,
        int? completionSetCount,
        string businessObjectClass,
        string businessObjectProperty,
        string businessObjectID,
        string args)
    {
      if (prefixText.Equals ("throw", StringComparison.OrdinalIgnoreCase))
        throw new Exception ("Test Exception");

      List<BusinessObjectWithIdentityProxy> persons = new List<BusinessObjectWithIdentityProxy>();
      foreach (Person person in XmlReflectionBusinessObjectStorageProvider.Current.GetObjects (typeof (Person)))
        persons.Add (new BusinessObjectWithIdentityProxy ((IBusinessObjectWithIdentity) person));

      foreach (string value in s_values)
        persons.Add (new BusinessObjectWithIdentityProxy ("invalid", value));

      List<BusinessObjectWithIdentityProxy> filteredPersons =
          persons.FindAll (
              delegate (BusinessObjectWithIdentityProxy person) { return person.DisplayName.StartsWith (prefixText, StringComparison.OrdinalIgnoreCase); });

      return filteredPersons.ToArray();
    }
  }
}