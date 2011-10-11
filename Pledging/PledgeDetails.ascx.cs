namespace ArenaWeb.UserControls.Contributions
{
	using System;
    using System.Collections;
	using System.Collections.Specialized;
    using System.Configuration;
    using System.Data;
	using System.Data.SqlClient;
	using System.Drawing;
	using System.Text;	
	using System.Web;
	using System.Web.UI;
    using System.Web.UI.HtmlControls;
	using System.Web.UI.WebControls;
	
	using Arena.Contributions;
    using Arena.Core;
    using Arena.DataLayer.Contributions;
    using Arena.DataLayer.Core;
   	using Arena.Enums;
    using Arena.Exceptions;
    using Arena.Metric;
	using Arena.Portal;
	using Arena.Portal.UI;
	using Arena.Utility;
	using ComponentArt.Charting;

	/// <summary>
	///	Summary description for RequestDetail.
	/// </summary>
	public partial class PledgeDetails : PortalControl
	{
		#region Module Settings
        
		[PageSetting("Person Detail Page", "The page that is used for displaying person details.", true)]
		public string PersonDetailPageIDSetting {get{return Setting("PersonDetailPageID", "", true);}}

        #endregion

        #region Private Variables

        private int fundId = -1;
        private double minPledgeAmount = 0;
        private double maxPledgeAmount = 0;
        private double minPledgeComplete = 0;
        private double maxPledgeComplete = 0;
        private bool includeNoPledge = false;		

        #endregion

        #region Events

        protected void Page_Load(object sender, System.EventArgs e)
		{
            // get filtered input
            try
            {
                if (ddFundList.Items.Count != 0)
                    fundId = Int32.Parse(ddFundList.SelectedValue);

                if (tbFilterMinPledge.Text.Trim().Length != 0)
                    minPledgeAmount = Convert.ToDouble(tbFilterMinPledge.Text);

                if (tbFilterMaxPledge.Text.Trim().Length != 0)
                    maxPledgeAmount = Convert.ToDouble(tbFilterMaxPledge.Text);

                if (tbFilterMinPercent.Text.Trim().Length != 0)
                    minPledgeComplete = Convert.ToDouble(tbFilterMinPercent.Text) / 100;

                if (tbFilterMaxPercent.Text.Trim().Length != 0)
                    maxPledgeComplete = Convert.ToDouble(tbFilterMaxPercent.Text) / 100;

				includeNoPledge = cbIncludeNoPledge.Checked;
            }
            catch
            { }
            
            if (!Page.IsPostBack)
            {
                // load funds
                FundCollection funds = new FundCollection(CurrentOrganization.OrganizationID);
                foreach (Fund fund in funds)
                    if (fund.Active)
                        ddFundList.Items.Add(new ListItem(fund.FundName, fund.FundId.ToString()));

            }
		}

        private void dgPledges_Rebind(object source, System.EventArgs e)
        {
            ShowPledges();
        }

        private void btnApply_Click(object sender, EventArgs e)
        {
            ShowPledges();
        }

        #endregion

        #region Private Methods

        private void ShowPledges()
        {
            DataTable DataSource = new PledgeData().GetPledgeListByFundId_DT(fundId, includeNoPledge, minPledgeAmount, maxPledgeAmount, minPledgeComplete, maxPledgeComplete);

            dgPledges.ItemType = "Pledges";
            dgPledges.ItemBgColor = CurrentPortalPage.Setting("ItemBgColor", string.Empty, false);
            dgPledges.ItemAltBgColor = CurrentPortalPage.Setting("ItemAltBgColor", string.Empty, false);
            dgPledges.ItemMouseOverColor = CurrentPortalPage.Setting("ItemMouseOverColor", string.Empty, false);
            dgPledges.AddEnabled = false;
            dgPledges.MoveEnabled = false;
            dgPledges.DeleteEnabled = false;
            dgPledges.EditEnabled = false;
            dgPledges.MergeEnabled = true;
            dgPledges.MailEnabled = false;
            dgPledges.ExportEnabled = true;
            dgPledges.AllowSorting = true;
            dgPledges.DataSource = DataSource;
            dgPledges.DataBind();
        }

        #endregion

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
			this.btnApply.Click +=new EventHandler(btnApply_Click);
            this.dgPledges.ReBind += new DataGridReBindEventHandler(this.dgPledges_Rebind);
		}

		#endregion
	}
}