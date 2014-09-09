<%@ Page Language="C#" AutoEventWireup="true" CodeFile="UserTimeline.aspx.cs" Inherits="UserTimeline" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <input type="submit" />
    <div>
	<asp:ListView ID="UserListView" runat="server">
		<LayoutTemplate>
			<table id="Table1" runat="server">
				<tr id="Tr1" runat="server">
					<th>Picture </th>
					<th>Name </th>
					<th>Last Tweet </th>
                    <th>Mentions </th>
				</tr>
				<tr id="itemPlaceholder">
				</tr>
			</table>
		</LayoutTemplate>
		<ItemTemplate>
			<tr id="Tr2" runat="server">
				<td>
					<asp:Image ID="UserImage" runat="server" ImageUrl='<%#Eval("User.ProfileImageUrl") %>' />
				</td>
				<td><asp:Label ID="NameLabel" runat="server" Text='<%#Eval("User.Name") %>' /> </td>
				<td><asp:Label ID="TweetLabel" runat="server" Text='<%#Eval("Text") %>' /> </td>
                <td>
                    <asp:Repeater ID="Repeater1" runat="server" DataSource='<%#Eval("Entities.UserMentionEntities") %>'>
                        <ItemTemplate><%# Eval("ScreenName")%>,</ItemTemplate>
                    </asp:Repeater>
                    <asp:Repeater ID="Repeater2" runat="server" DataSource='<%#Eval("Entities.UrlEntities") %>'>
                        <ItemTemplate><%# Eval("Url")%>,</ItemTemplate>
                    </asp:Repeater>

                    <asp:Repeater ID="Repeater3" runat="server" DataSource='<%#Eval("Entities.HashTagEntities") %>'>
                        <ItemTemplate><%# Eval("Tag")%>,</ItemTemplate>
                    </asp:Repeater>
                </td>
			</tr>
		</ItemTemplate>
	</asp:ListView>
    </div>
    </form>
</body>
</html>
