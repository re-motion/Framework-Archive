using System;
using System.Collections.Generic;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Web.Hosting;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using Rubicon.Reflection;
using Rubicon.Utilities;
using Rubicon.Mixins;
using System.Reflection;
using System.Diagnostics;

namespace WebApplicationSample
{
  public class MixinAwareVirtualFile : VirtualFile
  {
    private readonly Dictionary<string, Type> _baseNameToConcreteTypeMap;
    private string _physicalPath;

    public MixinAwareVirtualFile(string virtualPath, Dictionary<string, Type> baseNameToConcreteTypeMap)
      : base(virtualPath)
    {
      _baseNameToConcreteTypeMap = baseNameToConcreteTypeMap;
      _physicalPath = HostingEnvironment.MapPath(virtualPath);
    }

    public override System.IO.Stream Open()
    {
      using (FileStream fileStream = new FileStream(_physicalPath, FileMode.Open, FileAccess.Read, FileShare.Read))
      {
        using (StreamReader streamReader = new StreamReader(fileStream))
        {
          string fileContent = streamReader.ReadToEnd();

          fileContent = AdjustPageDirectiveToInheritFromMixin(fileContent);

          //// Replaces classname in "Inherits" attribute with the classname in "MixedPage" attribute if existing
          //Regex pageDirectiveRegex = new Regex("(^<%@[ ]*Page[ ]+.*Inherits=\")(.+?)(\"[ ]+.*)(MixedPage=\"(.*)\")(.*\n)");
          //fileContent = pageDirectiveRegex.Replace(fileContent, "$1$5$3$6");

          MemoryStream memoryStream = new MemoryStream();

          StreamWriter streamWriter = new StreamWriter(memoryStream);
          streamWriter.Write(fileContent);
          streamWriter.Flush();

          memoryStream.Position = 0;
          return memoryStream;
        }
      }
    }

    //private struct RegisterDirective
    //{
    //  string Namespace;
    //  string TagPrefix;
    //  string Assembly;
    //  string TagName;
    //  string Src;
    //}
    //private string AdjustControlsToInheritFromMixin(string fileContent)
    //{
    //  // regex to parse register directives -> all the attributes are available as named indexer
    //  //^<%@[ ]*Register.+?((Namespace="(?<Namespace>.+?)".+?)|(TagPrefix="(?<TagPrefix>.+?)".+?)|(TagName="(?<TagName>.+?)".+?)|(Assembly="(?<Assembly>.+?)".+?)|(Src="(?<Src>.+?)".+?))*[ ]?%>\r\n


    //  // TODO: solve generic (add single register attributes to regex)
    //  Regex parseRegisterDirectives = new Regex("^<%@[ ]*Register.+?((Namespace=\"(?<Namespace>.+?)\".+?)|(TagPrefix=\"(?<TagPrefix>.+?)\".+?)|(TagName=\"(?<TagName>.+?)\".+?)|(Assembly=\"(?<Assembly>.+?)\".+?)|(Src=\"(?<Src>.+?)\".+?))*[ ]?%>\r\n");
    //  MatchCollection matches = parseRegisterDirectives.Matches (fileContent);

    //  foreach (Match match in matches)
    //  {

    //  }
    //}

    private string AdjustPageDirectiveToInheritFromMixin(string fileContent)
    {
      Regex pageDirectiveGetPageTypeRegex = new Regex("^<%@[ ]*Page[ ]+.*Inherits=\"(.+?)\"[ ]+.*\n");
      Match match = pageDirectiveGetPageTypeRegex.Match(fileContent);

      if (match != null)
      {
        string pageTypeName = match.Groups[1].Value;
        Trace.WriteLine ("Editing base page type '" + pageTypeName + "'", "INFO");

        if (_baseNameToConcreteTypeMap.ContainsKey (pageTypeName))
        {
          Type concreteType = _baseNameToConcreteTypeMap[pageTypeName];
          Regex pageDirectiveReplacePageTypeRegex = new Regex ("(^<%@[ ]*Page[ ]+.*Inherits=\")(.+?)(\"[ ]+.*\n)");
          fileContent = pageDirectiveReplacePageTypeRegex.Replace (fileContent, "$1" + concreteType.AssemblyQualifiedName + "$3");
          Trace.WriteLine ("Replaced '" + pageTypeName + "' with '" + concreteType.AssemblyQualifiedName + "'", "INFO");
        }
      }
      return fileContent;
    }
  }
}
