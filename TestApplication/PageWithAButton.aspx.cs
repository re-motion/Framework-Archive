using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Rubicon.Web.ExecutionEngine;

namespace TestApplication
{
    public partial class PageWithAButton : Page
    {

        protected override void OnInit(EventArgs e)
        {
            submitButton.Click += submitButton_OnClick;
            base.OnInit(e);
        }

        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void submitButton_OnClick (object sender, EventArgs e)
        {
            nameLabel.Text = nameTextBox.Text;
            displayNamePanel.Visible = !string.IsNullOrEmpty (nameLabel.Text);
        }
    
    
    }
}