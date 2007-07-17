using System;
using System.Windows.Forms;
using Rubicon.Utilities;

namespace Rubicon.ObjectBinding.Design.BindableObject
{
  public class SearchFieldController : ControllserBase
  {
    public enum SearchIcons
    {
      [EnumDescription ("VS_Search.bmp")]
      Search = 0
    }

    private readonly TextBox _searchField;
    private readonly Button _searchButton;

    public SearchFieldController (TextBox searchField, Button searchButton)
    {
      ArgumentUtility.CheckNotNull ("searchField", searchField);
      ArgumentUtility.CheckNotNull ("searchButton", searchButton);

      _searchField = searchField;
      _searchButton = searchButton;
      _searchButton.ImageList = CreateImageList (SearchIcons.Search);
      _searchButton.ImageKey = SearchIcons.Search.ToString();
    }

    public TextBox SearchField
    {
      get { return _searchField; }
    }

    public Button SearchButton
    {
      get { return _searchButton; }
    }
  }
}