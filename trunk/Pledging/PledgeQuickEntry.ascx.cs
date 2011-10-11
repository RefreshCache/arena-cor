namespace ArenaWeb.UserControls.Custom.cotr
{
    using System;
    using System.Collections;
    using System.Configuration;
    using System.Data;
    using System.Data.SqlClient; //Added to try and solve problem
    using System.Linq;
    using System.Web;
    using System.Web.Security;
    using System.Web.UI;
    using System.Web.UI.HtmlControls;
    using System.Web.UI.WebControls;
    using System.Web.UI.WebControls.WebParts;
    using System.Xml.Linq;
    using Arena.Portal;
    using Arena.Contributions;
    using Arena.Core;
    using System.Collections.Specialized;
    using Arena.DataLayer.Core;
    using Arena.Payment;
    using Arena.Exceptions;
    using Arena.Core.Communications;
    using System.Text;
    using System.Collections.Generic;
    using DataLayer;

    public partial class PledgeQuickEntry : PortalControl
    {
        #region Module Settings

        [PageSetting("Person Search Page", "The page that is used to search for people.", false, 16)]
        public string PersonSearchPageSetting { get { return Setting("PersonSearchPage", "16", false); } }

        [PageSetting("Person Detail Page", "The page that should be used to display the person's details.", false, 7)]
        public string PersonDetailPageSetting { get { return Setting("PersonDetailPage", "7", false); } }

        [GatewayAccountSetting("ACH Payment Gateway Name", "The name of the Payment Gateway to use for ACH Transactions", false, AccountType.ACH)]
        public string ACHPaymentGatewayNameSetting { get { return Setting("ACHPaymentGatewayName", "", false); } }

        #endregion

        private bool _editEnabled = false;
        private int _pledgeId = -1;
        private GatewayAccount _achGatewayAcct = null;

        protected void Page_Init(object sender, EventArgs e)
        {
            if (ACHPaymentGatewayNameSetting.Trim() != string.Empty)
                try { _achGatewayAcct = new GatewayAccount(Convert.ToInt32(ACHPaymentGatewayNameSetting.Trim())); }
                catch { }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            _editEnabled = CurrentModule.Permissions.Allowed(Arena.Security.OperationType.Edit, CurrentUser);
            _pledgeId = !string.IsNullOrEmpty(Request.QueryString["pledge"]) ? Convert.ToInt32(Request.QueryString["pledge"]) : -1;

            if (!Page.IsPostBack)
            {
                SetDefaults();

                if (_achGatewayAcct != null)
                {
                    cbCreateRepeatingPayment.Visible = true;
                    cbCreateRepeatingPayment.Checked = (CurrentPerson.Settings["PledgeQuickEntry_CreateRepeatingPayment"] ?? string.Empty) == "True";
                    DisplayRepeatingPaymentSection();

                    // Load selected frequency types
                    Processor achProcessor = Processor.GetProcessorClass(_achGatewayAcct.PaymentProcessor);

                    ddlFrequency.Items.Clear();
                    Type enumType = typeof(PaymentFrequency);
                    System.Array values = Enum.GetValues(enumType);
                    foreach (object enumValue in values)
                    {
                        if ((PaymentFrequency)enumValue == PaymentFrequency.Every_Week ||
                            (PaymentFrequency)enumValue == PaymentFrequency.Once_a_Month ||
                            (PaymentFrequency)enumValue == PaymentFrequency.Every_Three_Months ||
                            (PaymentFrequency)enumValue == PaymentFrequency.Every_Year)
                        {
                            bool supported = true;
                            if (achProcessor != null && !achProcessor.FrequencySupported((PaymentFrequency)enumValue))
                                supported = false;

                            if (supported)
                            {
                                string itemValue = Enum.Format(enumType, enumValue, "D");
                                if (itemValue != "-1")
                                    ddlFrequency.Items.Add(new ListItem(Utilities.EnumName(enumType, enumValue), itemValue));
                            }
                        }
                    }
                }
            }

            // Construct campus drop down list from Arena lookup type 102, which is campus designations
            LookupCollection campuses = new LookupCollection(102);

            // Bind campus lookup key / values pairs to dropdown datasource
            ddlCampus.DataSource = campuses;
            ddlCampus.DataValueField = "LookupID";
            ddlCampus.DataTextField = "Value";
            ddlCampus.DataBind();

            // Bind frequency dictionary to frequency dropdown datasource.
            ddlPledgeFrequency.DataSource = PledgeFrequency.FrequencyNames;
            ddlPledgeFrequency.DataValueField = "Key";
            ddlPledgeFrequency.DataTextField = "Value";
            ddlPledgeFrequency.DataBind();

            // Add HTML tag attribute to tbAmount textbox, which calculates total amount
            tbAmount.Attributes.Add("onfocus", "calculateAmt()");
                       
        }

        private void SetDefaults()
        {
            FundCollection funds = new FundCollection(CurrentOrganization.OrganizationID);
            ddlFund.DataSource = funds;
            ddlFund.DataTextField = "FundName";
            ddlFund.DataValueField = "FundId";
            ddlFund.DataBind();
            ddlFund.Items.Insert(0, new ListItem("", "-1"));

            tbName.Focus();

            btnUpdate.Enabled = _editEnabled;

            if (_pledgeId != -1)
            {
                Pledge pledge = new Pledge(_pledgeId);
                if (pledge.PledgeId != -1)
                {
                    ddlFund.SelectedValue = pledge.FundId.ToString();
                    dtbStartDate.SelectedDate = pledge.PledgeBeginDate;
					dtbEndDate.SelectedDate = pledge.PledgeEndDate;
                    lblUpdated.Text = string.Format("Added {0:c} pledge to the {1} fund for {2}", pledge.Amount, pledge.Fund.FundName, Utilities.PersonLink(pledge.Person, Convert.ToInt32(PersonDetailPageSetting)));
                }
            }

            HyperLinkColumn hpl = (HyperLinkColumn)dgPeople.Columns[2];
            hpl.DataNavigateUrlFormatString = "~/default.aspx?page=" + PersonDetailPageSetting + "&guid={0}";
        }

        protected void btnUpdate_Click(object sender, EventArgs e)
        {
            PledgeCollection pledges = new PledgeCollection();
            pledges.LoadByPersonAndFundId(GetSelectedPersonId(), Convert.ToInt32(ddlFund.SelectedValue));
            if (pledges.Count > 0)
            {
                Person person = new Person(GetSelectedPersonId());
                lblError.Text = string.Format("Error: {0} already has a {1:c} pledge for the {2} fund.", Utilities.PersonLink(person, Convert.ToInt32(PersonDetailPageSetting)), pledges[0].Amount, pledges[0].Fund.FundName);
            }
            else if (dtbStartDate.SelectedDate > dtbEndDate.SelectedDate)
            {
                lblError.Text = "Start Date must be less than or equal to End Date";
            }
            else
            {
                Pledge pledge = new Pledge();
                pledge.PersonId = GetSelectedPersonId();
                pledge.FundId = Convert.ToInt32(ddlFund.SelectedValue);
                pledge.PledgeBeginDate = dtbStartDate.SelectedDate;
                pledge.PledgeEndDate = dtbEndDate.SelectedDate;
                pledge.Amount = Convert.ToDecimal(tbAmount.Text);
                //   pledge.Save(CurrentUser.Identity.Name);

                // Create custom pledge object
                DataLayer.PledgeData pledgeData = new DataLayer.PledgeData(_pledgeId);
                pledgeData.PersonID = GetSelectedPersonId();
                pledgeData.CampusLUID = Convert.ToInt32(ddlCampus.SelectedValue);
                pledgeData.FundID = Convert.ToInt32(ddlFund.SelectedValue);
                pledgeData.StartDate = dtbStartDate.SelectedDate;
                pledgeData.EndDate = dtbEndDate.SelectedDate;
                pledgeData.FrequencyAmount = Convert.ToDecimal(tbFrequencyAmt.Text);
                pledgeData.FrequencyCount = Convert.ToInt32(tbFrequencyCount.Text);
                pledgeData.TotalAmount = Convert.ToDecimal(tbAmount.Text);
                pledgeData.SavePledge(CurrentUser.Identity.Name);


                   if (cbCreateRepeatingPayment.Checked)
                {
                    CurrentPerson.Settings["PledgeQuickEntry_CreateRepeatingPayment"] = cbCreateRepeatingPayment.Checked.ToString();
                    
                    //Create ACH Repeating Payment
                    string comment = string.Format("F{0}:{1} ", pledge.FundId.ToString(), tbPaymentAmount.Text);

                    try
                    {
                        RepeatingPayment repeatingPayment = null;
                        if (_achGatewayAcct.AuthorizeACH(TransactionType.Repeating, tbAccountNumber.Text.Trim(), tbRoutingNumber.Text.Trim(),
                            rblAccountType.Items[0].Selected, pledge.PersonId, pledge.Person.FirstName, pledge.Person.FirstName,
                            pledge.Person.LastName, pledge.Person.Addresses.PrimaryAddress().Address.StreetLine1, pledge.Person.Addresses.PrimaryAddress().Address.City,
                            pledge.Person.Addresses.PrimaryAddress().Address.State, pledge.Person.Addresses.PrimaryAddress().Address.PostalCode,
                            "", "", Convert.ToDecimal(tbPaymentAmount.Text), comment, dtbStartDate.SelectedDate,
                            (PaymentFrequency)Enum.Parse(typeof(PaymentFrequency), ddlFrequency.SelectedValue), Convert.ToInt32(tbNumOfPayments.Text)))
                        {
                            repeatingPayment = _achGatewayAcct.RepeatingPayment;
                        }


                        if (repeatingPayment != null)
                        {
                            //Save Repeating Payment
                            repeatingPayment.Title = "Pledge for " + pledge.Fund.FundName;
                            repeatingPayment.AccountNumber = MaskAccountNumber(tbAccountNumber.Text.Trim());
                            repeatingPayment.OrganizationID = CurrentOrganization.OrganizationID;
                            repeatingPayment.RepeatingPaymentFunds.Clear();

                            RepeatingPaymentFund rpf = new RepeatingPaymentFund();
                            rpf.Amount = Convert.ToDecimal(tbPaymentAmount.Text);
                            rpf.FundID = pledge.FundId;
                            repeatingPayment.RepeatingPaymentFunds.Add(rpf);
                            repeatingPayment.Save(CurrentUser.Identity.Name);

                            try
                            {
                                OnlineGivingContribution ogcEmail = new OnlineGivingContribution();
                                ogcEmail.Send(CurrentOrganization, repeatingPayment);
                            }
                            catch (Exception)
                            {
                            }
                        }
                        else
                        {
                            string error = ParseError("Authorization of your information failed for the following reason(s):", _achGatewayAcct.Messages);
                            throw new ArenaApplicationException(error);
                        }

                    }
                    catch (Exception ex)
                    {
                        throw new ArenaApplicationException("Error occurred during Authorization", ex);
                    }
                    
                }
                RedirectToPledge(pledge.PledgeId);
            }

            ddlCampus.Focus();
        }

        private string ParseError(string header, List<string> errorMsgs)
        {
            StringBuilder message = new StringBuilder();
            message.AppendFormat("{0}\n", header);
            message.Append("<ul>\n");
            for (int i = 0; i < errorMsgs.Count; i++)
                message.AppendFormat("<li>{0}</li>\n", (string)errorMsgs[i]);
            message.Append("</ul>\n");

            return message.ToString();
        }

        private string MaskAccountNumber(string accountNumber)
        {
            string masked = "";
            const int showDigits = 4;

            for (int i = 0; i < accountNumber.Length - showDigits; i++)
                masked += "X";

            if ((accountNumber.Length - showDigits) > 0)
                masked += accountNumber.Substring(accountNumber.Length - showDigits, showDigits);

            return masked;
        }

        protected void btnChoose_Click(object sender, EventArgs e)
        {
            string firstName = "";
            string secondName = "";
            string thirdName = "";
            int envelopeNumber = -1;
            DataTable dt = null;
            if (int.TryParse(tbName.Text, out envelopeNumber))
            {
                dt = new PersonData().GetPersonByEnvelopeNumber_DT(envelopeNumber);
            }
            else
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

            lblPerson.Text = "";
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
                ddlCampus.Focus();
            }
            else if (dgPeople.Items.Count > 0)
            {
                ScriptManager.GetCurrent(Page).SetFocus(dgPeople.Items[0].FindControl("rbChoose"));
            }
            else
            {
                dgPeople.Visible = false;
                tbName.Focus();
                lblPerson.Text = "There are no matches for the name entered.";
            }
        }

        protected void rbChoose_CheckedChanged(object sender, EventArgs e)
        {
            RadioButton rb = (RadioButton)sender;
            DataGridItem dgi = (DataGridItem)rb.Parent.BindingContainer;
            int personID = Int32.Parse(dgPeople.Items[dgi.ItemIndex].Cells[0].Text);
            foreach (DataGridItem i in dgPeople.Items)
            {
                i.Style.Remove("background-color");
                i.Style.Remove("border");
            }
            dgPeople.Items[dgi.ItemIndex].Style.Add("background-color", "#fefebb");
            dgPeople.Items[dgi.ItemIndex].Style.Add("border", "1px solid #000000");
            dgPeople.Items[dgi.ItemIndex].Attributes.Remove("onmouseover");
            dgPeople.Items[dgi.ItemIndex].Attributes.Remove("onmouseout");
            Person p = new Person(personID);
            ihPersonList.Text = p.PersonID.ToString();
            ddlCampus.Focus();
        }

        protected void dgPeople_ItemDataBound(object sender, DataGridItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                HyperLink hl = (HyperLink)e.Item.Cells[2].Controls[0];
                hl.TabIndex = -1;
                hl.Target = "_blank";
            }
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            RedirectToPledge(-1);
        }

        protected void cbCreateRepeatingPayment_CheckedChanged(object sender, EventArgs e)
        {
            DisplayRepeatingPaymentSection();
        }

        private void DisplayRepeatingPaymentSection()
        {
            pnlPaymentSection.Visible = cbCreateRepeatingPayment.Checked;
            if (dtbStartDate.SelectedDate <= DateTime.Today && cbCreateRepeatingPayment.Checked && dtbStartDate.Text != string.Empty)
                lblError.Text = "The Start Date must be in the future to create a repeating payment";
        }
        
        protected void Control_ValueChanged(object sender, EventArgs e)
        {
            if (cbCreateRepeatingPayment.Checked)
                UpdatePaymentTerms((Control)sender);
        }

        private void RedirectToPledge(int pledgeId)
        {
            NameValueCollection query = HttpUtility.ParseQueryString(Request.Url.Query);
            query["pledge"] = pledgeId.ToString();
            Response.Redirect(string.Format("{0}?{1}", Request.Url.AbsolutePath, query.ToString()));
        }

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

        private void UpdatePaymentTerms(Control control)
        {
            string field = control.ID;

            if (ValidControls(field))
            {
                int numberOfPayments = 0;
                decimal amount = 0;

                switch (field)
                {
                    case "tbAmount":
                    case "ddlFrequency":
                    case "dtbStartDate":
                    case "dtbEndDate":
                        //calculate payment amount from total
                        numberOfPayments = GetNumberOfPayments();
                        amount = Convert.ToDecimal(tbAmount.Text) / numberOfPayments;
                        tbPaymentAmount.Text = amount.ToString("N2");
                        break;
                    case "tbPaymentAmount":
                        //calculate total amount from payment
                        numberOfPayments = GetNumberOfPayments();
                        amount = numberOfPayments * Convert.ToDecimal(tbPaymentAmount.Text);
                        tbAmount.Text = amount.ToString("N2");
                        break;
                    case "tbNumOfPayments":
                        numberOfPayments = Convert.ToInt32(tbNumOfPayments.Text);
                        amount = Convert.ToDecimal(tbAmount.Text) / numberOfPayments;
                        DateTime newEndDate = GetEndDate(dtbStartDate.SelectedDate, numberOfPayments, ddlFrequency.SelectedValue);
                        tbPaymentAmount.Text = amount.ToString("N2");
                        dtbEndDate.SelectedDate = newEndDate;
                        break;
                }
                tbNumOfPayments.Text = numberOfPayments.ToString();
                lblError.Text = "";
            }
            else
            {
                lblError.Text = "Unable to calculate due to missing value(s)";
            }
        }

        private DateTime GetEndDate(DateTime startDate, int numberOfPayments, string selectedFrequency)
        {
            DateTime endDate = startDate;

            switch ((PaymentFrequency)Enum.Parse(typeof(PaymentFrequency), selectedFrequency))
            {
                case PaymentFrequency.Every_Week:
                    endDate = startDate.AddDays(7 * numberOfPayments);
                    break;
                case PaymentFrequency.Once_a_Month:
                    endDate = startDate.AddMonths(numberOfPayments).AddDays(-1);
                    break;
                case PaymentFrequency.Every_Three_Months:
                    endDate = startDate.AddMonths(numberOfPayments * 3);
                    break;
                case PaymentFrequency.Every_Year:
                    endDate = startDate.AddYears(numberOfPayments);
                    break;
            }
            return endDate;
        }

        private int GetNumberOfPayments()
        {
            int durationInDays = 0;
            int numOfPayments = 0;

            TimeSpan timeSpan = dtbEndDate.SelectedDate - dtbStartDate.SelectedDate;
            durationInDays = timeSpan.Days;

            switch ((PaymentFrequency)Enum.Parse(typeof(PaymentFrequency), ddlFrequency.SelectedValue))
            {
                case PaymentFrequency.Every_Week:
                    numOfPayments = (durationInDays / 7);
                    numOfPayments = numOfPayments == 0 ? 1 : numOfPayments;
                    break;
                case PaymentFrequency.Once_a_Month:
                    numOfPayments = 12 * (dtbEndDate.SelectedDate.Year - dtbStartDate.SelectedDate.Year) 
                        + dtbEndDate.SelectedDate.Month - dtbStartDate.SelectedDate.Month + 1;
                    break;
                case PaymentFrequency.Every_Three_Months:
                    numOfPayments = (12 * (dtbEndDate.SelectedDate.Year - dtbStartDate.SelectedDate.Year)
                        + dtbEndDate.SelectedDate.Month - dtbStartDate.SelectedDate.Month) / 3 + 1;
                    break;
                case PaymentFrequency.Every_Year:
                    numOfPayments = (durationInDays / 365) + 1;
                    break;
            }

            return numOfPayments;
        }

        private bool ValidControls(string field)
        {
            switch (field)
            {
                case "dtbStartDate":
                case "dtbEndDate":
                case "tbAmount":
                case "ddlFrequency":
                    if (dtbStartDate.SelectedDate <= DateTime.Today ||
                        dtbStartDate.SelectedDate > dtbEndDate.SelectedDate ||
                        dtbStartDate.Text == string.Empty ||
                        dtbEndDate.Text == string.Empty ||
                        tbAmount.Text == string.Empty)
                        return false;
                    break;
                case "tbPaymentAmount":
                    if (dtbStartDate.SelectedDate <= DateTime.Today ||
                        dtbStartDate.SelectedDate > dtbEndDate.SelectedDate ||
                        dtbStartDate.Text == string.Empty ||
                        dtbEndDate.Text == string.Empty ||
                        tbPaymentAmount.Text == string.Empty)
                        return false;
                    break;
                case "tbNumberOfPayments":
                    if (dtbStartDate.SelectedDate <= DateTime.Today ||
                        dtbStartDate.Text == string.Empty ||
                        tbAmount.Text == string.Empty)
                        return false;
                    break;
            }

            return true;
        }
    }
}
