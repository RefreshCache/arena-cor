<%@ control language="c#" CodeFile="PersonPledgeList.ascx.cs" inherits="ArenaWeb.UserControls.Custom.cotr.PersonPledgeList" %>
<%@ Register TagPrefix="Arena" Namespace="Arena.Portal.UI" Assembly="Arena.Portal.UI" %>

<asp:UpdatePanel ID="upPledges" runat="server">
    <ContentTemplate>
        <table width="100%" cellpadding="0" cellspacing="0" border="0">
            <tr>
	            <td width="11" valign="top"><img src="images/bar_left.jpg" alt="" border="0" /></td>
	            <td width="25" valign="top" style="background-image: url(images/bar_middle.jpg);"><img src="images/pledge.jpg" alt="" border="0" />
	            <td width="*" valign="middle" nowrap="nowrap" style="background-image: url(images/bar_middle.jpg);" class="heading2">
		            <div>Pledges</div>
	            </td>
	            <td width="11" valign="top"><img src="images/bar_right.jpg" alt="" border="0" /></td>
            </tr>
        </table>
        <table cellpadding="0" cellspacing="10" border="0" width="100%" bgcolor="#FFFFFF">
            <tr>
                <td>
                    <Arena:DataGrid id="dgPledges" Runat="server" AllowSorting="true">
	                    <Columns>
		                    <asp:boundcolumn HeaderText="ID" datafield="pledge_id" ReadOnly="True" Visible="False" />
		                    <asp:boundcolumn HeaderText="Fund" SortExpression="fund_name" DataField="fund_name" ReadOnly="true" />
		                    <asp:TemplateColumn HeaderText="Responsible<br />Person">
			                    <ItemTemplate><asp:Label id="lblPersonName" runat="server" /></ItemTemplate>
			                </asp:TemplateColumn>
			                <asp:TemplateColumn>
			                    <EditItemTemplate>
			                        <Arena:LookupDropDown runat="server" LookupTypeID="102" />
			                    </EditItemTemplate>
			                </asp:TemplateColumn>
		                    <asp:TemplateColumn HeaderText="Begin Date" SortExpression="pledge_begin_date">
			                    <ItemTemplate><%# DataBinder.Eval(Container.DataItem, "pledge_begin_date", "{0:d}") %></ItemTemplate>
			                    <EditItemTemplate>
			                        <Arena:DateTextBox ID="tbStartDate" runat="server" CssClass="smallText" Text='<%# DataBinder.Eval(Container.DataItem, "pledge_begin_date", "{0:d}") %>' InvalidValueMessage="Start Date must be a valid date." SetFocusOnError="false" />
                                    <asp:CompareValidator ID="cvStartDate" runat="server" ErrorMessage="The start date cannot be after the end date." ControlToValidate="tbStartDate" ControlToCompare="tbEndDate" Type="Date" Operator="LessThanEqual" Text=" *" />
			                    </EditItemTemplate>
			                </asp:TemplateColumn>
		                    <asp:TemplateColumn HeaderText="End Date" SortExpression="pledge_end_date">
			                    <ItemTemplate><%# DataBinder.Eval(Container.DataItem, "pledge_end_date", "{0:d}")%></ItemTemplate>
			                    <EditItemTemplate>
			                        <Arena:DateTextBox ID="tbEndDate" runat="server" CssClass="smallText" text='<%# DataBinder.Eval(Container.DataItem, "pledge_end_date", "{0:d}") %>' InvalidValueMessage="End Date must be a valid date." SetFocusOnError="false" />
                                    <asp:CompareValidator ID="cvEndDate" runat="server" ErrorMessage="The end date cannot be before the start date." ControlToValidate="tbEndDate" ControlToCompare="tbStartDate" Type="Date" Operator="GreaterThanEqual" Text=" *" />
			                    </EditItemTemplate>
			                </asp:TemplateColumn>
		                    <asp:TemplateColumn HeaderText="Amount" HeaderStyle-HorizontalAlign="Right" ItemStyle-HorizontalAlign="Right" SortExpression="amount">
			                    <ItemTemplate><%# DataBinder.Eval(Container.DataItem, "amount", "{0:C2}") %></ItemTemplate>
			                    <EditItemTemplate>
			                        <asp:TextBox id="tbAmount" runat="server" cssClass="smallText" text='<%# DataBinder.Eval(Container.DataItem, "amount", "{0:#########0.00}") %>' />
                                    <asp:RangeValidator ID="rvAmount" runat="server" Type="Currency" ErrorMessage="Amount must be a valid currency amount." Display="dynamic" MinimumValue="0" MaximumValue="9999999999" ControlToValidate="tbAmount" CssClass="errorText" SetFocusOnError="true" Text=" *" />
			                    </EditItemTemplate>
			                </asp:TemplateColumn>
		                    <asp:boundcolumn HeaderText="Contributions" SortExpression="contribution_amount" DataField="contribution_amount"
 			                    HeaderStyle-HorizontalAlign="Right" ItemStyle-HorizontalAlign="Right" DataFormatString="{0:C2}" ReadOnly="true" />
		                    <asp:boundcolumn HeaderText="Balance" SortExpression="pledge_balance" DataField="pledge_balance"
 			                    HeaderStyle-HorizontalAlign="Right" ItemStyle-HorizontalAlign="Right" DataFormatString="{0:C2}" ReadOnly="true" />
	                    </Columns>
                    </Arena:DataGrid>
                    <asp:Panel ID="pnlAdd" runat="server" style="padding-top:3px;text-align:right" CssClass="formLabel" DefaultButton="btnAddPledge">
                        Add Pledge:&nbsp;
                        <asp:DropDownList ID="ddlFunds" runat="server" CssClass="smallText" />
                        &nbsp;
                        <asp:Button ID="btnAddPledge" runat="server" CssClass="smallText" Text="Add" />
                        <asp:Label ID="lblError" runat="server" CssClass="errorText" />
                    </asp:Panel>
                </td>
            </tr>
        </table>
        <br />
    </ContentTemplate>
</asp:UpdatePanel>