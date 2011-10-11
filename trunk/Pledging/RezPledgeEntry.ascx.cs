namespace ArenaWeb.UserControls.Custom.cotr
{

    using System;
    using System.Data.SqlClient;
    using System.Data;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.Linq;
    using System.Web;
    using System.Web.UI;
    using System.Web.UI.WebControls;
    using Arena.DataLayer.Core;
    using Arena.Payment;
    using Arena.Exceptions;
    using Arena.Core.Communications;
    using Arena.Portal;
    using Arena.Contributions;
    using Arena.Core;

    using DataLayer;

    public partial class RezPledgeEntry : PortalControl
    {
        #region Module Settings

        [PageSetting("Person Search Page", "The page that is used to search for people.", false, 16)]
        public string PersonSearchPageSetting { get { return Setting("PersonSearchPage", "16", false); } }

        [PageSetting("Person Detail Page", "The page that should be used to display the person's details.", false, 7)]
        public string PersonDetailPageSetting { get { return Setting("PersonDetailPage", "7", false); } }

        [GatewayAccountSetting("ACH Payment Gateway Name", "The name of the Payment Gateway to use for ACH Transactions", false, AccountType.ACH)]
        public string ACHPaymentGatewayNameSetting { get { return Setting("ACHPaymentGatewayName", "", false); } }

        [ListFromSqlSetting("Default Fund", "The default fund that will be selected on page load.", true, "", "SELECT [cf].[fund_id], [cf].[fund_name] FROM [dbo].[ctrb_fund] AS cf ORDER BY [cf].[fund_order]")]
        public string DefaultFundSetting { get { return Setting("DefaultFund", "", true); } }

        [ListFromSqlSetting("Designated Campital Funds", "Funds selected here will be designated as capital funds. Capital funds will not be stored with a campus designation.", true, "", "SELECT [cf].[fund_id], [cf].[fund_name] FROM [dbo].[ctrb_fund] AS cf ORDER BY [cf].[fund_order]", ListSelectionMode.Multiple)]
        public string CapitalFundsSetting { get { return Setting("CapitalFunds", "", true); } }

        #endregion

        // Private member variable to indicate if logged in user has permission to this module
        private bool _editEnabled = false;

        // Private member variable set to the pledge ID we are working with
        private int _pledgeId = -1;
        
        /// <summary>
        /// Runs upon page load
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            _editEnabled = CurrentModule.Permissions.Allowed(Arena.Security.OperationType.Edit, CurrentUser);
            _pledgeId = !string.IsNullOrEmpty(Request.QueryString["pledge"]) ? Convert.ToInt32(Request.QueryString["pledge"]) : -1;

            // Put a bit of space at the top of the <asp:Panel ID="DataPanel"> control
            DataPanel.Attributes.Add("style", "margin-top:1em;");

            // Add onfocus event to tbFrequencyCount that calculates totals
            tbFrequencyCount.Attributes.Add("onfocus", "getTimeSpans()");

            // If not postback, set up form
            if (!Page.IsPostBack)
            {
                // Construct a fund collection to populate the fund drop down list
                FundCollection funds = new FundCollection(CurrentOrganization.OrganizationID);
                ddlFund.DataSource = funds;
                ddlFund.DataTextField = "FundName";
                ddlFund.DataValueField = "FundId";
                ddlFund.DataBind();
                ddlFund.Items.Insert(0, new ListItem("", "-1"));

                // Set the selected fund to the defined module setting
                ddlFund.SelectedValue = DefaultFundSetting;

                // Construct campus drop down list from Arena lookup type 102, which is campus designations
                LookupCollection campuses = new LookupCollection(102);

                // Bind campus lookup key / values pairs to dropdown datasource
                ddlCampus.DataSource = campuses;
                ddlCampus.DataValueField = "LookupID";
                ddlCampus.DataTextField = "Value";
                ddlCampus.DataBind();
                ddlCampus.Items.Insert(0, new ListItem("", "-1"));            

                // Bind frequency dictionary to frequency dropdown datasource.
                rblPledgeFrequency.DataSource = PledgeFrequency.FrequencyNames;
                rblPledgeFrequency.DataValueField = "Key";
                rblPledgeFrequency.DataTextField = "Value";
                rblPledgeFrequency.DataBind();
                rblPledgeFrequency.SelectedIndex = 2;

                foreach( ListItem li in rblPledgeFrequency.Items )
                {   
                    //if( "RadioButton".Equals(ctrl.GetType().ToString() ) )
                    //    ((RadioButton)ctrl).Attributes["onfocus"] = "getTimeSpans()"; 
                    li.Attributes["onfocus"] = "getTimeSpans()";  
                    
                }

                // Initially set focus on name text box
                tbName.Focus();

            }
        }

        /// <summary>
        /// Searches for person
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnSearchPerson_Click(object sender, EventArgs e)
        {
            string firstName = "";
            string secondName = "";
            string thirdName = "";
            int envelopeNumber = -1;
            DataTable dt = null;

            // If integer is provided then search for person by their envelope number?
            if (int.TryParse(tbName.Text, out envelopeNumber))
            {
                dt = new PersonData().GetPersonByEnvelopeNumber_DT(envelopeNumber);
            }
            else // Else parse the input by first, middle, lastname variations
            {
                if (tbName.Text.IndexOf(",") > -1)
                {
                    thirdName = tbName.Text.Substring(0, tbName.Text.IndexOf(","));
                    firstName = tbName.Text.Substring(tbName.Text.IndexOf(",") + 1);
                }
                else if (tbName.Text.IndexOf(" ") > -1 && tbName.Text.IndexOf(" ") != tbName.Text.LastIndexOf(" "))
                {
                    //three spaces
                    firstName = tbName.Text.Substring(0, tbName.Text.IndexOf(" "));
                    secondName = tbName.Text.Substring(tbName.Text.IndexOf(" "), tbName.Text.LastIndexOf(" "));
                    thirdName = tbName.Text.Substring(tbName.Text.LastIndexOf(" "));
                }
                else if (tbName.Text.IndexOf(" ") > -1)
                {
                    firstName = tbName.Text.Substring(0, tbName.Text.IndexOf(" "));
                    thirdName = tbName.Text.Substring(tbName.Text.IndexOf(" "));
                }
                else
                {
                    thirdName = tbName.Text;
                }
                dt = new PersonData().GetPersonListBySearch_DT(firstName.Trim(), secondName.Trim(), thirdName.Trim());
            }

            lblUpdated.Text = "";
            dgPeople.DataSource = dt;
            dgPeople.DataBind();
            dgPeople.Visible = true;
            if (dt.Rows.Count == 1)
            {
                ihPersonList.Text = dt.Rows[0]["person_id"].ToString();
                dgPeople.Items[0].Style.Add("background-color", "#fefebb");
                dgPeople.Items[0].Attributes.Remove("onmouseover");
                dgPeople.Items[0].Attributes.Remove("onmouseout");
                RadioButton r = (RadioButton)dgPeople.Items[0].FindControl("rbChoose");
                r.Checked = true;

                // Set the form focus depending on whether campus is seleted or visible, or start date has already been entered
                if (ddlCampus.SelectedValue.Equals("-1") && ddlCampus.Visible == true)
                    ddlCampus.Focus();
                else if (tbStartDate.Text == "")
                    tbStartDate.Focus();
                else
                    tbFrequencyCount.Focus();

            }
            else if (dgPeople.Items.Count > 0)
            {
                ScriptManager.GetCurrent(Page).SetFocus(dgPeople.Items[0].FindControl("rbChoose"));
            }
            else
            {
                dgPeople.Visible = false;
                tbName.Focus();
                lblPerson.Text = "There are no matches for the name entered. Please try your search again.";
            }
        }

        /// <summary>
        /// Updating datagrid link to open in new window
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void dgPeople_ItemDataBound(object sender, DataGridItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                HyperLink hl = (HyperLink)e.Item.Cells[2].Controls[0];
                hl.TabIndex = -1;
                hl.Target = "_blank";
            }
        }

        /// <summary>
        /// Radio button clicked to choose record in the search results
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void rbChoose_CheckedChanged(object sender, EventArgs e)
        {
            RadioButton rb = (RadioButton)sender;
            DataGridItem dgi = (DataGridItem)rb.Parent.BindingContainer;
            int personID = Int32.Parse(dgPeople.Items[dgi.ItemIndex].Cells[0].Text);
            foreach (DataGridItem i in dgPeople.Items)
            {
                i.Style.Remove("background-color");
                i.Style.Remove("border");

                // Uncheck all other radio buttons
                ((RadioButton)i.FindControl("rbChoose")).Checked = false;
            }

            // Check the button that was clicked
            ((RadioButton)dgi.FindControl("rbChoose")).Checked = true;
            
            // Set the styling of the checked button to be highlighted
            dgPeople.Items[dgi.ItemIndex].Style.Add("background-color", "#fefebb");
            dgPeople.Items[dgi.ItemIndex].Style.Add("border", "1px solid #000000");
            dgPeople.Items[dgi.ItemIndex].Attributes.Remove("onmouseover");
            dgPeople.Items[dgi.ItemIndex].Attributes.Remove("onmouseout");
            Person p = new Person(personID);

            // Need to fix this so that it's in the updatepanel, so that the ajax call can update the value properly
            ihPersonList.Text = p.PersonID.ToString();

            // Set the form focus depending on whether campus is seleted or visible, or start date has already been entered
            if ( ddlCampus.SelectedValue.Equals("-1") && ddlCampus.Visible == true )
                ddlCampus.Focus();
            else if (tbStartDate.Text == "")
                tbStartDate.Focus();
            else
                tbFrequencyCount.Focus();
        }

        /// <summary>
        /// Update button was clicked to save pledge information
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnUpdate_Click(object sender, EventArgs e)
        {
            // Create person object based on the person that is selected
            Person person = new Person(GetSelectedPersonId());

            // Load a pledge collection object to determine if person already has a pledge against the fund selected
            PledgeCollection pledges = new PledgeCollection();
            pledges.LoadByPersonAndFundId(GetSelectedPersonId(), Convert.ToInt32(ddlFund.SelectedValue));

            DataLayer.PledgeData pledgeData = new DataLayer.PledgeData(-1);
           
            if (pledges.Count > 0)
            {
                lblUpdated.Text = "";
                lblError.Text = string.Format("Error: {0} already has a {1:c} pledge for the {2} fund.", Utilities.PersonLink(person, Convert.ToInt32(PersonDetailPageSetting)), pledges[0].Amount, pledges[0].Fund.FundName);
            }
            else
            {
                // Create pledge data object and save to database
                pledgeData.PersonID = GetSelectedPersonId();
                pledgeData.CampusLUID = Convert.ToInt32(ddlCampus.SelectedValue);
                pledgeData.FundID = Convert.ToInt32(ddlFund.SelectedValue);
                pledgeData.StartDate = tbStartDate.SelectedDate;
                pledgeData.EndDate = tbEndDate.SelectedDate;
                pledgeData.FrequencyAmount = Convert.ToDecimal(tbFrequencyAmt.Text);
                pledgeData.FrequencyCount = Convert.ToInt32(tbFrequencyCount.Text);
                pledgeData.TotalAmount = Convert.ToDecimal(tbAmount.Text);
                pledgeData.SavePledge(CurrentUser.Identity.Name);

                // Clear any errors being displayed, display a confirmation text that the pledge was stored, and reset the form appropriately
                lblError.Text = "";
                lblUpdated.Text = string.Format(@"Added {0:c} pledge to the <span style=""font-weight:bold;"">{1}</span> fund for {2}", pledgeData.TotalAmount, ddlFund.SelectedItem.Text, Utilities.PersonLink(person, Convert.ToInt32(PersonDetailPageSetting)));
                tbFrequencyAmt.Text = tbFrequencyCount.Text = tbAmount.Text = "";
                dgPeople.Visible = false;
                tbName.Text = "";
                tbName.Focus();
    
            }

        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            // If cancel is clicked then just reset parts of the form and set focus on the name search again
            dgPeople.Visible = false;
            tbFrequencyCount.Text = tbFrequencyAmt.Text = tbAmount.Text = "";
            tbName.Text = "";
            tbName.Focus();

        }

        /// <summary>
        /// Retrieves person_id selected in the tbName search
        /// </summary>
        /// <returns></returns>
        private int GetSelectedPersonId()
        {
            if (ihPersonList.Text != string.Empty)
            {
                foreach (string id in ihPersonList.Text.Split(','))
                {
                    return Convert.ToInt32(id.Trim());
                }
            }
            return -1;
        }

        /// <summary>
        /// Fires when Fund selected item changes and hides campus elements if necessary
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void ddlFund_OnChange(object sender, EventArgs e)
        {
            string selectedFund = ddlFund.SelectedValue;
            string[] capitalFunds = CapitalFundsSetting.ToString().Split(new char[1] { ',' });

            // if selected fund is in the string array of capitalFunds, then hide campus settings
            if (capitalFunds.Contains(selectedFund))
            {
                lblCampus.Visible = false;
                ddlCampus.Visible = false;
                ddlCampus.SelectedValue = "-1";
                rfvCampus.Enabled = false;
            }
            else // otherwise make campus settings visible
            {
                lblCampus.Visible = true;
                ddlCampus.Visible = true;
                rfvCampus.Enabled = true;
            }
        }

        protected void rblPledgeFrequency_OnPreRender(object sender, EventArgs e)
        {
            base.OnPreRender(e);
        }
    }
}