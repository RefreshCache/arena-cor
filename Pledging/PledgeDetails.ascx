<%@ control language="c#" inherits="ArenaWeb.UserControls.Contributions.PledgeDetails, Arena" %>
<%@ Register TagPrefix="Arena" Namespace="Arena.Portal.UI" Assembly="Arena.Portal.UI" %>
   
<asp:Panel ID="pnlFilter" runat="server" DefaultButton="btnApply">
    <div style="background-color:#F4F2F2;border-top: solid 1px #B1B1B1;border-right: solid 1px #B1B1B1;border-left: solid 1px #B1B1B1;">
        <table cellpadding="3" cellspacing="0" border="0">
            <tr>
                <td valign="top" align="left" style="padding-left:10px;padding-top:10px"><img src="images/filter.gif" border="0"></td>
                <td valign="top">
                    <table cellpadding="0" cellspacing="3" border="0">
                        <tr>
                            <td class="formLabel" nowrap="nowrap">Fund</td>
                            <td><asp:DropDownList ID="ddFundList" runat="server" CssClass="formItem" /></td>
                        </tr>
                    </table>
                </td>
                <td valign="top" class="formItem">
                     <table>
                        <tr><td colspan="2" class="formLabel">Pledge Amount</td></tr>
                        <tr>
                            <td class="formItem">Minimum </td>
                            <td class="formItem">$<asp:TextBox ID="tbFilterMinPledge" runat="server" CssClass="formItem" /></td>
                        </tr>
                        <tr>
                            <td class="formItem">Maximum </td>
                            <td class="formItem">$<asp:TextBox ID="tbFilterMaxPledge" runat="server" CssClass="formItem" /></td>
                        </tr>
                        <tr><td colspan="2">&nbsp;</td></tr>
                        <tr><td colspan="2" class="formLabel">Percent Complete</td></tr>
                        <tr>
                            <td class="formItem">Minimum</td>
                            <td class="formItem"><asp:TextBox ID="tbFilterMinPercent" runat="server" CssClass="normalText" />%</td>
                        </tr>
                        <tr>
                            <td class="formItem">Maximum</td>
                            <td class="formItem"><asp:TextBox ID="tbFilterMaxPercent" runat="server" CssClass="normalText" />%</td>
                        </tr>
                     </table>
                </td>
            </tr>
            <tr>
                <td>&nbsp;</td>
                <td class="formItem">
                    <asp:CheckBox ID="cbIncludeNoPledge" Runat="server" CssClass="smallText" />Include those with gifts but no pledge
                    <p/>
                    <asp:Button ID="btnApply" Runat="server" CssClass="smallText" Text="Apply Filter" />
                </td>
            </tr>
        </table>
    </div>
</asp:Panel>

<Arena:DataGrid id="dgPledges" Runat="server" AllowSorting="true" Width="100%">
    <Columns>
		<asp:boundcolumn HeaderText="ID" datafield="person_id" Visible="False" />
		<asp:TemplateColumn HeaderText="Name" SortExpression="name" HeaderStyle-HorizontalAlign="Left" ItemStyle-HorizontalAlign="Left" ItemStyle-Wrap="False">
			<ItemTemplate>
				<Arena:PersonLabel ID="plPerson" runat="server" 
					PersonGUID='<%# Eval("person_guid") %>' 
					PersonName='<%# Eval("person_name") %>' 
					PersonPageID='<%# Convert.ToInt32(PersonDetailPageIDSetting) %>'
					HasPhoto='<%# Eval("photo_guid") != DBNull.Value %>'
					Restricted='<%# Eval("restricted") != DBNull.Value && Convert.ToBoolean(Eval("restricted")) %>'
					PersonUrlTarget="_blank" />
			</ItemTemplate>
		</asp:TemplateColumn>
		<asp:boundcolumn HeaderText="Begin<br />Date" datafield="pledge_begin_date" SortExpression="pledge_begin_date" HeaderStyle-HorizontalAlign="Left"
		    ItemStyle-HorizontalAlign="Left" DataFormatString="{0:d}" ItemStyle-Wrap="False" />
		<asp:boundcolumn HeaderText="End<br />Date" datafield="pledge_end_date" SortExpression="pledge_end_date" HeaderStyle-HorizontalAlign="Left"
		    ItemStyle-HorizontalAlign="Left" DataFormatString="{0:d}" ItemStyle-Wrap="False" />
		<asp:boundcolumn HeaderText="Pledge<br />Amount" datafield="pledge_amount" SortExpression="pledge_amount" HeaderStyle-HorizontalAlign="Right"
		    ItemStyle-HorizontalAlign="Right" DataFormatString="{0:C}" ItemStyle-Wrap="False" />
		<asp:boundcolumn HeaderText="Contributed<br />Amount" datafield="contribution_amount" SortExpression="contribution_amount" HeaderStyle-HorizontalAlign="Right"
		    ItemStyle-HorizontalAlign="Right" DataFormatString="{0:C}" ItemStyle-Wrap="False" />
		<asp:boundcolumn HeaderText="Percent<br />Complete" datafield="completion_percentage" SortExpression="completion_percentage" HeaderStyle-HorizontalAlign="Right" 
			ItemStyle-HorizontalAlign="Right" DataFormatString="{0:P2}" ItemStyle-Wrap="False" />
	</Columns>
</Arena:DataGrid>