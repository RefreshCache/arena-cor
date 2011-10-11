<%@ control language="C#" autoeventwireup="true" inherits="ArenaWeb.UserControls.Custom.cotr.PledgeQuickEntry"    CodeFile="PledgeQuickEntry.ascx.cs"  %>
<asp:Panel ID="pnlMain" runat="server" DefaultButton="btnUpdate">
    <asp:UpdatePanel ID="upPerson" runat="server">
        <ContentTemplate>
            <asp:Panel ID="pnlPerson" runat="server" DefaultButton="btnChoose">
                <div class="listFilter">
                    <table cellpadding="3" cellspacing="0" border="0">
                        <tr>
                            <td valign="top" align="left" style="padding-left: 10px; padding-top: 10px">
                                <img src="images/filter.gif" border="0">
                            </td>
                            <td valign="top">
                                <table cellpadding="5" cellspacing="0" border="0" style="padding-top: 15px;">
                                    <tr>
                                        <td valign="center" style="padding-left: 10px;" class="formLabel">
                                            Name
                                        </td>
                                        <td valign="center" class="formItem">
                                            <asp:TextBox ID="tbName" runat="server" CssClass="formItem"></asp:TextBox>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                        </td>
                                        <td align="right">
                                            <asp:Button ID="btnChoose" runat="server" Text="Apply Filter" CssClass="smallText"
                                                OnClick="btnChoose_Click" CausesValidation="false" />
                                        </td>
                                    </tr>
                                </table>
                            </td>
                        </tr>
                    </table>
                </div>
                <div>
                    <Arena:DataGrid ID="dgPeople" runat="server" DataKeyField="person_id" Visible="false"
                        EditEnabled="false" DeleteEnabled="false" AddEnabled="false" ShowFooter="false"
                        CellPadding="6" ExportEnabled="false" MergeEnabled="false" AllowSorting="false"
                        OnItemDataBound="dgPeople_ItemDataBound">
                        <Columns>
                            <asp:BoundColumn DataField="person_id" ReadOnly="true" Visible="False"></asp:BoundColumn>
                            <asp:TemplateColumn HeaderStyle-CssClass="reportHeader" HeaderStyle-VerticalAlign="Bottom"
                                HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center" ItemStyle-Wrap="False"
                                HeaderText="">
                                <ItemTemplate>
                                    <asp:UpdatePanel ID="uCheckBox" runat="server" UpdateMode="conditional">
                                        <ContentTemplate>
                                            <asp:RadioButton ID="rbChoose" runat="server" onclick="UnCheckAllRBs(this);" OnCheckedChanged="rbChoose_CheckedChanged"
                                                AutoPostBack="true" />
                                        </ContentTemplate>
                                    </asp:UpdatePanel>
                                </ItemTemplate>
                            </asp:TemplateColumn>
                            <asp:HyperLinkColumn DataTextField="name" DataNavigateUrlField="guid" HeaderText="Name">
                            </asp:HyperLinkColumn>
                            <asp:BoundColumn DataField="address" HeaderText="Address"></asp:BoundColumn>
                            <asp:BoundColumn DataField="home_phone" HeaderText="Home Number"></asp:BoundColumn>
                            <asp:BoundColumn DataField="email" HeaderText="Email Address"></asp:BoundColumn>
                        </Columns>
                    </Arena:DataGrid>
                    <asp:Label ID="lblPerson" runat="server" CssClass="formLabel" />
                    <asp:RequiredFieldValidator ID="rfvPerson" runat="server" ControlToValidate="ihPersonList"
                        ErrorMessage="Person is required">*</asp:RequiredFieldValidator>
                    <asp:TextBox ID="ihPersonList" runat="server" Style="visibility: hidden; display: none;" />
                </div>
            </asp:Panel>
        </ContentTemplate>
    </asp:UpdatePanel>
    <asp:UpdatePanel ID="upRepeatingPaymentSection" runat="server">
        <ContentTemplate>
            <table cellpadding="0" cellspacing="5" border="0">
              
              <!-- remove when done -->
              
               <tr>
                    <td class="formLabel" nowrap style="width: 70px;">
                        Campus
                    </td>
                    <td class="formItem" nowrap>
                        <asp:DropDownList ID="ddlCampus" runat="server" CssClass="formItem" EnableViewState="True" />
                        <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ControlToValidate="ddlFund"
                            ErrorMessage="Fund is required" SetFocusOnError="true" InitialValue="-1">*</asp:RequiredFieldValidator>
                    </td>
                </tr>
                <tr>
                    <td class="formLabel" nowrap style="width: 70px;">
                        Fund
                    </td>
                    <td class="formItem" nowrap>
                        <asp:DropDownList ID="ddlFund" runat="server" CssClass="formItem"  EnableViewState="True"/>
                        <asp:RequiredFieldValidator ID="rfvFund" runat="server" ControlToValidate="ddlFund"
                            ErrorMessage="Fund is required" SetFocusOnError="true" InitialValue="-1">*</asp:RequiredFieldValidator>
                    </td>
                </tr>
                <tr>
                    <td class="formLabel" nowrap>
                        Start Date
                    </td>
                    <td class="formItem" nowrap>
                        <Arena:DateTextBox ID="dtbStartDate" runat="server" CssClass="formItem" Width="80" OnTextChanged="Control_ValueChanged" AutoPostBack="false"/>
                        <asp:RequiredFieldValidator ID="rfvStartDate" runat="server" ControlToValidate="dtbStartDate"
                            ErrorMessage="Start Date is required" SetFocusOnError="true">*</asp:RequiredFieldValidator>
                    </td>
                </tr>
                <tr>
                    <td class="formLabel" nowrap>
                        End Date
                    </td>
                    <td class="formItem" nowrap>
                        <Arena:DateTextBox ID="dtbEndDate" runat="server" CssClass="formItem" Width="80" OnTextChanged="Control_ValueChanged" AutoPostBack="false"/>
                        <asp:RequiredFieldValidator ID="rfvEndDate" runat="server" ControlToValidate="dtbEndDate"
                            ErrorMessage="End Date is required" SetFocusOnError="true">*</asp:RequiredFieldValidator>
                    </td>
                </tr>
                 <tr>
                    <td class="formLabel" nowrap style="width: 70px;">
                        Frequency
                    </td>
                    <td class="formItem" nowrap>
                        <asp:DropDownList ID="ddlPledgeFrequency" runat="server" CssClass="formItem" />
                        <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" ControlToValidate="ddlFund"
                            ErrorMessage="Pledge frequency is required" SetFocusOnError="true" InitialValue="-1">*</asp:RequiredFieldValidator>
                    </td>
                </tr>
                 <tr>
                    <td class="formLabel" nowrap style="width: 70px;">
                        Frequency Amount
                    </td>
                    <td class="formItem" nowrap>
                   
                        <asp:TextBox ID="tbFrequencyAmt" runat="server" style="text-align: right;" EnableViewState="False"></asp:TextBox>
                    </td>
                </tr>
                 <tr>
                    <td class="formLabel" nowrap style="width: 70px;">
                        Frequency Count
                    </td>
                    <td class="formItem" nowrap>
                   
                        <asp:TextBox ID="tbFrequencyCount" runat="server" style="text-align: right;" EnableViewState="False"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td class="formLabel" nowrap>
                        Amount
                    </td>
                    <td class="formItem" nowrap="nowrap">
                        <asp:TextBox ID="tbAmount" runat="server" Columns="10" style="text-align: right;" CssClass="formItem" OnTextChanged="Control_ValueChanged" AutoPostBack="false" EnableViewState="False" />
                        <asp:RequiredFieldValidator ID="rfvAmount" runat="server" ControlToValidate="tbAmount"
                            ErrorMessage="Amount is required" SetFocusOnError="true">*</asp:RequiredFieldValidator>
                        <ajax:FilteredTextBoxExtender ID="ftbeAmount" runat="server" TargetControlID="tbAmount"
                            FilterType="Custom, Numbers" ValidChars="." />
                    </td>
                </tr>
                <tr>
                    <td class="formItem" nowrap>
                    </td>
                </tr>
            </table>
            <asp:CheckBox ID="cbCreateRepeatingPayment" runat="server" CssClass="formItem" Text="Create Repeating Payment" AutoPostBack="true" Visible="false" OnCheckedChanged="cbCreateRepeatingPayment_CheckedChanged"/>
            <asp:Panel ID="pnlPaymentSection" runat="server" Visible="false">
                <table cellpadding="0" cellspacing="5" border="0">
                    <tr>
                        <th valign="middle" align="left" nowrap class="personalInfoHeader" colspan="2">
                            Repeating Payment Information
                        </th>
                    </tr>
                    <tr>
                        <td class="formLabel">Frequency</td>
                        <td class="formItem">
                            <asp:DropDownList ID="ddlFrequency" Runat="server" CssClass="formItem" OnSelectedIndexChanged="Control_ValueChanged" AutoPostBack="true" ></asp:DropDownList>
                        </td>
                    </tr>
                    <tr>
                        <td class="formLabel">Payment Amount</td>
                        <td class="formItem">
                            <asp:TextBox ID="tbPaymentAmount" runat="server" CssClass="formItem" Columns="10" style="text-align: right;" OnTextChanged="Control_ValueChanged" AutoPostBack="true"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="formLabel">Number of Payments</td>
                        <td class="formItem">
                             <asp:TextBox ID="tbNumOfPayments" runat="server" CssClass="formItem"  style="text-align: right;" Width="30" MaxLength="3" OnTextChanged="Control_ValueChanged" AutoPostBack="true"></asp:TextBox>
                             <asp:RangeValidator ID="rvNumOfPayments" runat="server" ErrorMessage="Number of payments must be a valid number." Display="dynamic" MinimumValue="0" MaximumValue="999" ControlToValidate="tbNumOfPayments" CssClass="errorText" SetFocusOnError="true"> *</asp:RangeValidator>
                        </td>
                    </tr>
                    <tr>
                        <td class="formLabel">Account Type</td>
                        <td class="formItem">
                            <asp:RadioButtonList ID="rblAccountType" runat="server" CssClass="formItem" RepeatDirection="Horizontal">
                                <asp:ListItem Selected="True">Checking</asp:ListItem>
                                <asp:ListItem>Savings</asp:ListItem>
                            </asp:RadioButtonList>
                        </td>
                    </tr>
                    <tr>
                        <td class="formLabel">Routing Number</td>
                        <td class="formItem">
                            <asp:TextBox ID="tbRoutingNumber" runat="server" CssClass="formItem" Width="200" MaxLength="9"></asp:TextBox>
				            <asp:RequiredFieldValidator ControlToValidate="tbRoutingNumber" ID="rfvRoutingNumber" Runat="server" ErrorMessage="Routing Number is required" CssClass="errorText" Display="Dynamic" SetFocusOnError="true"> *</asp:RequiredFieldValidator>
                        </td>
                    </tr>
                    <tr>
                        <td class="formLabel">Account Number</td>
                        <td class="formItem">
                            <asp:TextBox ID="tbAccountNumber" runat="server" CssClass="formItem" Width="200" MaxLength="17"></asp:TextBox>
				            <asp:RequiredFieldValidator ControlToValidate="tbAccountNumber" ID="rfvAccountNumber" Runat="server" ErrorMessage="Account Number is required" CssClass="errorText" Display="Dynamic" SetFocusOnError="true"> *</asp:RequiredFieldValidator>
                        </td>
                    </tr>
                </table>
            </asp:Panel>
    <table cellpadding="0" cellspacing="5" border="0">
        <tr>
            <td>
                <asp:Button ID="btnUpdate" runat="server" Text="Save" CssClass="smallText" OnClick="btnUpdate_Click" />
                <asp:Button ID="btnCancel" runat="server" Text="Cancel" CssClass="smallText" CausesValidation="False"
                    OnClick="btnCancel_Click"></asp:Button>
            </td>
        </tr>
    </table>
    <asp:Label ID="lblUpdated" runat="server" CssClass="smallText" />
    <asp:Label ID="lblError" runat="server" CssClass="errorText" />
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Panel>

<script language="javascript" type="text/javascript">
    function UnCheckAllRBs(rbCurrent) {
        var coll = document.getElementsByTagName('INPUT');
        if (coll != null)
            for (i = 0; i < coll.length; i++)
            if (coll[i].type == 'radio')
            coll[i].checked = false;
        rbCurrent.checked = true;
    }

    var lastFocusedControlId = "";

    function focusHandler(e) {
        document.activeElement = e.originalTarget;
    }

    function appInit() {
        if (typeof (window.addEventListener) !== "undefined") {
            window.addEventListener("focus", focusHandler, true);
        }
        Sys.WebForms.PageRequestManager.getInstance().add_pageLoading(pageLoadingHandler);
        Sys.WebForms.PageRequestManager.getInstance().add_pageLoaded(pageLoadedHandler);
    }

    function pageLoadingHandler(sender, args) {
        lastFocusedControlId = typeof (document.activeElement) === "undefined"
        ? "" : document.activeElement.id;
    }

    function focusControl(targetControl) {
        if (Sys.Browser.agent === Sys.Browser.InternetExplorer) {
            var focusTarget = targetControl;
            if (focusTarget && (typeof (focusTarget.contentEditable) !== "undefined")) {
                oldContentEditableSetting = focusTarget.contentEditable;
                focusTarget.contentEditable = false;
            }
            else {
                focusTarget = null;
            }
            targetControl.focus();
            if (focusTarget) {
                focusTarget.contentEditable = oldContentEditableSetting;
            }
        }
        else {
            targetControl.focus();
        }
    }

    function pageLoadedHandler(sender, args) {
        if (typeof (lastFocusedControlId) !== "undefined" && lastFocusedControlId != "") {
            var newFocused = $get(lastFocusedControlId);
            if (newFocused) {
                focusControl(newFocused);
            }
        }
    }
    Sys.Application.add_init(appInit)



/*Added by Anthony*/
    function calculateAmt() {
        var frequency = document.getElementById("<%=ddlPledgeFrequency.ClientID %>");
        var frequencyAmt = document.getElementById("<%= tbFrequencyAmt.ClientID %>");
       
        if (frequencyAmt.value != null) {
            var total = frequency.value * frequencyAmt.value;
            var tbAmount = document.getElementById("<%= tbAmount.ClientID %>");
            tbAmount.value = total;
             }
        }
   
</script>

