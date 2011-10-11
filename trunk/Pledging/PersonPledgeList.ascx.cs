namespace ArenaWeb.UserControls.Custom.cotr
{
	using System;
	using System.Data;
	using System.Drawing;
    using System.Text;
	using System.Web;
	using System.Web.UI;
    using System.Web.UI.HtmlControls;
	using System.Web.UI.WebControls;
	
    using Arena.Contributions;
    using Arena.Core;
    using Arena.DataLayer.Contributions;
	using Arena.Enums;
    using Arena.Exceptions;
	using Arena.Portal;
	using Arena.Portal.UI;
	using Arena.Security;
    using DataLayer;
	
	/// <summary>
	///	User control for the Person Pledge List.
	/// </summary>
	public partial class PersonPledgeList : PortalControl
	{
        #region Module Settings

        #endregion

        #region Private Properties

        private Person person = null;
        private bool _useShelby = false;

        #endregion

        protected void Page_Load(object sender, System.EventArgs e)
		{
            _useShelby = Convert.ToBoolean(CurrentOrganization.Settings["UseShelbyV5Contributions"]);

            LoadPerson();

            if (!Page.IsPostBack)
                ShowList();
        }

		public void ShowList()
		{
			bool EditEnabled = CurrentModule.Permissions.Allowed(OperationType.Edit, CurrentUser);

            // Set up the dgPledges DataGrid
            dgPledges.ItemType = "Pledge";
			dgPledges.ItemBgColor = CurrentPortalPage.Setting("ItemBgColor", string.Empty, false);
			dgPledges.ItemAltBgColor = CurrentPortalPage.Setting("ItemAltBgColor", string.Empty, false);
			dgPledges.ItemMouseOverColor = CurrentPortalPage.Setting("ItemMouseOverColor", string.Empty, false);
			dgPledges.AddEnabled = false;
			dgPledges.DeleteEnabled = EditEnabled;
			dgPledges.EditEnabled = EditEnabled;
			dgPledges.MergeEnabled = false;
			dgPledges.MailEnabled = false;
			dgPledges.ExportEnabled = true;

            dgPledges.DataSource = new DataLayer.PledgeData().GetPledgeListByPersonID_DT(person.PersonID);
			dgPledges.DataBind();

            if (EditEnabled)
            {
                pnlAdd.Visible = true;
                FundCollection funds = new FundCollection(CurrentOrganization.OrganizationID, _useShelby);

                ddlFunds.Items.Clear();
                foreach (Fund fund in funds)
                    if (fund.Active && fund.CanPledge)
                        ddlFunds.Items.Add(new ListItem(fund.FundName, fund.FundId.ToString()));
            }
            else
            {
                pnlAdd.Visible = false;
            }
        }

        void dgPledges_ItemDataBound(object sender, DataGridItemEventArgs e)
        {
            ListItemType li = (ListItemType)e.Item.ItemType;
            if (li == ListItemType.Item || li == ListItemType.AlternatingItem || li == ListItemType.EditItem)
            {
                DataRowView row = (DataRowView)e.Item.DataItem;
                
                Label lblPersonName = (Label)e.Item.FindControl("lblPersonName");
                if ((int)row["giving_person_id"] != person.PersonID)
                    lblPersonName.Text = string.Format("<a href='default.aspx?page={0}&pagetab=3167&guid={1}'>{2}</a>",
                        CurrentPortalPage.PortalPageID.ToString(),
                        row["giving_person_guid"].ToString(),
                        row["person_name"].ToString());
                else
                    lblPersonName.Text = person.FullName;

                if ((decimal)row["pledge_balance"] > 0)
                    e.Item.Cells[7].ForeColor = Color.Red;

                if (li == ListItemType.EditItem)
                {
                    Fund fund = new Fund((int)row["fund_id"]);

                    DateTextBox tbStartDate = (DateTextBox)e.Item.FindControl("tbStartDate");
                    if (fund != null && tbStartDate != null && fund.StartDate != DateTime.Parse("1/1/1900") && fund.EndDate != DateTime.Parse("1/1/1900"))
                    {
                        tbStartDate.MinimumDate = fund.StartDate;
                        tbStartDate.MaximumDate = fund.EndDate;
                        tbStartDate.InvalidValueMessage = string.Format("Start Date must be a valid date between {0} and {1}",
                            fund.StartDate.ToShortDateString(),
                            fund.EndDate.ToShortDateString());
                    }

                    DateTextBox tbEndDate = (DateTextBox)e.Item.FindControl("tbEndDate");
                    if (fund != null && tbStartDate != null && fund.StartDate != DateTime.Parse("1/1/1900") && fund.EndDate != DateTime.Parse("1/1/1900"))
                    {
                        tbEndDate.MinimumDate = fund.StartDate;
                        tbEndDate.MaximumDate = fund.EndDate;
                        tbEndDate.InvalidValueMessage = string.Format("End Date must be a valid date between {0} and {1}",
                            fund.StartDate.ToShortDateString(),
                            fund.EndDate.ToShortDateString());
                    }
                }
            }
        }

        void btnAddPledge_Click(object sender, EventArgs e)
        {
			int fundId = Int32.Parse(ddlFunds.SelectedValue);
			PledgeCollection pledges = new PledgeCollection();
			pledges.LoadByPersonAndFundId(person.PersonID, fundId);
			if (pledges.Count > 0)
			{
				lblError.Text = string.Format("<br />Error: {0} already has a {1:c} pledge for the {2} fund.", person.NickName, pledges[0].Amount, pledges[0].Fund.FundName);
			}
			else
			{
				lblError.Text = string.Empty;
				Fund fund = new Fund(fundId);

				DateTime startDate = DateTime.Today;
				if (DateTime.Today > fund.EndDate || DateTime.Today < fund.StartDate)
					startDate = fund.StartDate;

				new Arena.DataLayer.Contributions.PledgeData().SavePledge(
					-1,
					person.PersonID,
					fund.FundId,
					startDate,
					fund.EndDate,
					0,
					CurrentUser.Identity.Name);
			}

            ShowList();
        }

        private void dgPledges_Rebind(object source, System.EventArgs e)
		{
			ShowList();
		}

        private void dgPledges_UpdateCommand(object source, DataGridCommandEventArgs e)
        {
            if (Page.IsValid)
            {
                TextBox tbAmount = (TextBox)e.Item.FindControl("tbAmount");
                DateTextBox tbStartDate = (DateTextBox)e.Item.FindControl("tbStartDate");
                DateTextBox tbEndDate = (DateTextBox)e.Item.FindControl("tbEndDate");

                Pledge pledge = new Pledge(Int32.Parse(e.Item.Cells[0].Text));
                Fund fund = new Fund(pledge.FundId);

                decimal amount = 0;
                DateTime startDate = fund.StartDate;
                DateTime endDate = fund.EndDate;

                if (tbAmount.Text.Trim() != string.Empty)
                    amount = decimal.Parse(tbAmount.Text.Trim());

                if (tbStartDate.Text.Trim() != string.Empty)
                    startDate = tbStartDate.SelectedDate;

                if (tbEndDate.Text.Trim() != string.Empty)
                    endDate = tbEndDate.SelectedDate;

                pledge.Amount = amount;
                pledge.PledgeBeginDate = startDate;
                pledge.PledgeEndDate = endDate;

                pledge.Save(CurrentUser.Identity.Name);

                dgPledges.EditItemIndex = -1;
                ShowList();
            }
        }

        void dgPledges_DeleteCommand(object source, DataGridCommandEventArgs e)
        {
            int pledgeID = Int32.Parse(e.Item.Cells[0].Text);
            Arena.DataLayer.Contributions.PledgeData data = new Arena.DataLayer.Contributions.PledgeData();

            data.DeletePledge(pledgeID);

            ShowList();
        }

        public void LoadPerson()
        {
			if (ArenaContext.Current.SelectedPerson != null)
				person = ArenaContext.Current.SelectedPerson;

            if (person == null)
            {
                string[] keys;
                keys = Request.QueryString.AllKeys;
                foreach (string key in keys)
                {
                    switch (key.ToUpper())
                    {
                        case "PERSON":
                            try { person = new Person(Int32.Parse(Request.QueryString.Get(key))); }
                            catch { }
                            break;
                        case "GUID":
                            try { person = new Person(new Guid(Request.QueryString.Get(key))); }
                            catch { }
                            break;
                    }
                }

                if (person == null || person.PersonID == -1)
                    throw new ModuleException(CurrentPortalPage, CurrentModule, "The PersonPledgeList module requires a numeric Person ID or valid Person GUID");
            }
        }

        #region Web Form Designer generated code
		override protected void OnInit(EventArgs e)
		{
			//
			// CODEGEN: This call is required by the ASP.NET Web Form Designer.
			//
			InitializeComponent();
			base.OnInit(e);
		}
		
		/// <summary>
		///		Required method for Designer support - do not modify
		///		the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            dgPledges.ItemDataBound += new DataGridItemEventHandler(dgPledges_ItemDataBound);
			dgPledges.ReBind += new DataGridReBindEventHandler(dgPledges_Rebind);
            dgPledges.DeleteCommand += new DataGridCommandEventHandler(dgPledges_DeleteCommand);
            dgPledges.UpdateCommand += new DataGridCommandEventHandler(dgPledges_UpdateCommand);
            btnAddPledge.Click += new EventHandler(btnAddPledge_Click);
        }

		#endregion

	}
}