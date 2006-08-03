using System;
using System.Collections.Generic;
using Rubicon.Data.DomainObjects;
using Rubicon.Web.ExecutionEngine;
using Rubicon.Web.UI.Globalization;
using Rubicon.ObjectBinding.Web.UI.Controls;
using WebSample;
using WebSample.Classes;
using WebSample.WxeFunctions;

namespace WebSample.UI
{
	public partial class EditPersonForm : BasePage
	{
		private bool _isSaved;

		protected void Page_Load(object sender, EventArgs e)
		{
			CurrentObject.BusinessObject = Function.Person;
			CurrentObject.LoadValues(IsPostBack);

			foreach (DataEditUserControl control in DataEditUserControls)
			{
				control.DataSource.BusinessObject = Function.Person;
				control.LoadValues(IsPostBack);
			}
		}

		protected EditPersonFunction Function
		{
			get { return (EditPersonFunction)CurrentFunction; }
		}

		private List<DataEditUserControl> DataEditUserControls
		{
			get
			{
				List<DataEditUserControl> list = new List<DataEditUserControl>();

				foreach (System.Web.UI.Control control in EditPersonView.Controls)
				{
					DataEditUserControl dataEditUserControl = control as DataEditUserControl;

					if (dataEditUserControl != null)
						list.Add(dataEditUserControl);
				}

				return list;
			}
		}

		protected void SaveButton_Click(object sender, EventArgs e)
		{
			EnsurePostLoadInvoked();
			EnsureValidatableControlsInitialized();

			bool isValid = CurrentObject.Validate();

			foreach (DataEditUserControl control in DataEditUserControls)
				isValid &= control.Validate();

			if (isValid)
			{
				foreach (DataEditUserControl control in DataEditUserControls)
					control.SaveValues(false);

				CurrentObject.SaveValues(false);
				ClientTransaction.Current.Commit();
				_isSaved = true;

				this.ExecuteNextStep();
			}
		}

		protected void CancelButton_Click(object sender, EventArgs e)
		{
			this.ExecuteNextStep();
		}

		protected override void OnUnload(EventArgs e)
		{
			base.OnUnload(e);

			if (!_isSaved)
			{
				foreach (DataEditUserControl control in DataEditUserControls)
					control.SaveValues(true);

				CurrentObject.SaveValues(true);
			}
		}
	}
}
