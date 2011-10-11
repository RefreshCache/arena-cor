<%@ Control Language="C#" AutoEventWireup="true" CodeFile="RezPledgeEntry.ascx.cs" Inherits="ArenaWeb.UserControls.Custom.cotr.RezPledgeEntry" %>
<style type="text/css">
    .style1
    {
        width: 122px;
    }
</style>
<asp:Panel ID="pnlPerson" runat="server" DefaultButton="btnSearchPerson">
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
                                <asp:Button ID="btnSearchPerson" runat="server" Text="Apply Filter" CssClass="smallText"
                                    OnClick="btnSearchPerson_Click" CausesValidation="false" />
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
        </table>
    </div>
</asp:Panel>    
 <asp:UpdatePanel ID="upnlPerson" runat="server" UpdateMode="conditional" >
    <ContentTemplate>
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
                                <asp:RadioButton ID="rbChoose" runat="server" OnCheckedChanged="rbChoose_CheckedChanged"
                                    AutoPostBack="true" />
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
    </ContentTemplate>
</asp:UpdatePanel>
<asp:Panel ID="DataPanel" runat="server">
    <asp:UpdatePanel ID="upnlCampusByFund" runat="server" UpdateMode="Conditional">
       <ContentTemplate>
            <table style="width: 26%;">
                <tr>
                    <td class="formLabel">
                        Fund</td>
                    <td>
                        <asp:DropDownList ID="ddlFund" runat="server" OnSelectedIndexChanged="ddlFund_OnChange" CssClass="formItem" AutoPostBack="true">
                        </asp:DropDownList>
                    </td>
                </tr>
                <tr>
                    <td class="formLabel">
                        <asp:Label ID="lblCampus" runat="server">Campus</asp:Label></td>
                    <td>
                        <asp:DropDownList ID="ddlCampus" runat="server" CssClass="formItem">
                        </asp:DropDownList>
                        <asp:RequiredFieldValidator ID="rfvCampus" runat="server" InitialValue="-1" ControlToValidate="ddlCampus"
                            ErrorMessage="Campus selection is required" SetFocusOnError="true">*</asp:RequiredFieldValidator>
                    </td>
                </tr>
            </table>
        </ContentTemplate>
    </asp:UpdatePanel>    
    <table style="width: 26%;"> 
         <tr>
            <td class="formLabel">
                Pledge Frequency</td>
            <td>
                <asp:RadioButtonList ID="rblPledgeFrequency" CssClass="formItem" OnPreRender="rblPledgeFrequency_OnPreRender"
                    runat="server" >
                </asp:RadioButtonList>
            </td>
        </tr>
        <tr>
            <td class="formLabel">
                Start Date</td>
            <td>
                <Arena:DateTextBox ID="tbStartDate" runat="server" CssClass="formItem" Width="80" />
                <asp:RequiredFieldValidator ID="rfvStartDate" runat="server" ControlToValidate="tbStartDate"
                    ErrorMessage="Start Date is required" SetFocusOnError="true">*</asp:RequiredFieldValidator>
                <asp:CompareValidator ID="cvStartDateEndDate" runat="server" Operator="LessThanEqual" ControlToValidate="tbStartDate" ControlToCompare="tbEndDate" 
                    Type="Date" ErrorMessage="Sorry, but the Start Date must be less than or equal to the End Date">*</asp:CompareValidator>
            </td>
        </tr>
        <tr>
            <td class="formLabel">
                End Date</td>
            <td>
                 <Arena:DateTextBox ID="tbEndDate" runat="server" CssClass="formItem" Width="80" 
                AutoPostBack="false"/>
                <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ControlToValidate="tbEndDate"
                    ErrorMessage="Start Date is required" SetFocusOnError="true">*</asp:RequiredFieldValidator>
            </td>
        </tr>
         <tr>
            <td class="formLabel" nowrap style="width: 70px;">
                Frequency Count
            </td>
            <td class="formItem" nowrap>
           
                <asp:TextBox ID="tbFrequencyCount" runat="server" CssClass="formItem" style="text-align: right;" ></asp:TextBox>
            </td>
        </tr>
         <tr>
            <td class="formLabel" nowrap style="width: 70px;">
                Frequency Amount
            </td>
            <td class="formItem" nowrap>
           
                <asp:TextBox ID="tbFrequencyAmt" runat="server" CssClass="formItem" style="text-align: right;" ></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td class="formLabel" nowrap>
                Total Amount
            </td>
            <td class="formItem" nowrap="nowrap">
                <asp:TextBox ID="tbAmount" runat="server" CssClass="formItem" style="text-align: right;" />
                <asp:RequiredFieldValidator ID="rfvAmount" runat="server" ControlToValidate="tbAmount"
                    ErrorMessage="Amount is required" SetFocusOnError="true">*</asp:RequiredFieldValidator>
            </td>
        </tr>        
        <tr>
            <td>                
                <asp:Button ID="btnUpdate" runat="server" Text="Save" CssClass="smallText" OnClick="btnUpdate_Click" />
                <asp:Button ID="btnCancel" runat="server" Text="Cancel" CssClass="smallText" OnClick="btnCancel_Click" CausesValidation="False" />
            </td>
        </tr>
    </table>
    <asp:Label ID="lblUpdated" runat="server" CssClass="smallText" />
    <asp:Label ID="lblError" runat="server" CssClass="errorText" />  
</asp:Panel>
<span id="result"></span>
<script type="text/javascript" src="/Arena/UserControls/custom/cotr/Contributions/Pledging/date.js"></script>
<script type="text/javascript">
    
    function getSelectedRadio(buttonGroup) {
        // returns the array number of the selected radio button or -1 if no button is selected
        if (buttonGroup[0]) { // if the button group is an array (one button is not an array)
            for (var i = 0; i < buttonGroup.length; i++) {
                if (buttonGroup[i].checked) {
                    return i
                }
            }
        } else {
            if (buttonGroup.checked) { return 0; } // if the one button is checked, return zero
        }
        // if we get to this point, no radio button is selected
        return -1;
    } // Ends the "getSelectedRadio" function

    function getSelectedRadioValue(buttonGroup) {
        // returns the value of the selected radio button or "" if no button is selected
        var i = getSelectedRadio(buttonGroup);
        if (i == -1) {
            return "";
        } else {
            if (buttonGroup[i]) { // Make sure the button group is an array (not just one button)
                return buttonGroup[i].value;
            } else { // The button group is just the one button, and it is checked
                return buttonGroup.value;
            }
        }
    } // Ends the "getSelectedRadioValue" function

    function writeTimeSpan(ts) {
        var result = document.getElementById("result"),
        output = "<p>";

        //console.dir(ts);
        //console.log("---------------------------------------");
        for (var prop in ts) {
            // output += ("<span>" + prop + ": " + ts[prop] + "</span>");
            var week = ts[prop];
            week = Math.floor(parseInt(week));

            var tbCount = document.getElementById("<%= tbFrequencyCount.ClientID %>");
            tbCount.value = week;

            console.log("frequency count="+week);
        }
        output += ("<span style='clear:both;display:block;height:1px;overflow:hidden;'>&nbsp;</span></p>");
        result.innerHTML += output;

    }

    function getTimeSpans() {
        var toDate = new Date(document.getElementById("<%=tbEndDate.ClientID %>").value),
            fromDate = new Date(document.getElementById("<%=tbStartDate.ClientID %>").value);
        
        // Debugging    
        document.getElementById("result").innerHTML = "<p><b>" + fromDate.toString() + "</b> to <b>" + toDate.toString() + "</b></p>";

        var PledgeFrequency = getSelectedRadioValue( $("input:radio[name=<%=rblPledgeFrequency.UniqueID %>]") );
        console.log("Pledge frequency=" + PledgeFrequency);
        if (PledgeFrequency == "Weekly")
        { writeTimeSpan(timeSpan(fromDate, toDate, "weeks")); console.log("weeeeekes"); }
        else if (PledgeFrequency == "Semi_Monthly")
        { writeTimeSpan(timeSpan(fromDate, toDate, "months")); }
        else if (PledgeFrequency == "Monthly_First")
        { writeTimeSpan(timeSpan(fromDate, toDate, "months")); }
        else if (PledgeFrequency == "Monthly_Third")
        { writeTimeSpan(timeSpan(fromDate, toDate, "months")); }
        //Amount x frequency

        //Code for calculation
        var tbCount = document.getElementById("<%= tbFrequencyCount.ClientID %>");

        var tbAmount = document.getElementById("<%= tbFrequencyAmt.ClientID %>");

        var total = tbAmount.value * tbCount.value;
        var tbTotalAmount = document.getElementById("<%= tbAmount.ClientID %>");
        tbTotalAmount.value = total;
    }
    
</script>