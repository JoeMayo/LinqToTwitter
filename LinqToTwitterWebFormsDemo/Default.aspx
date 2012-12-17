<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="_Default" %>
<%@ Import Namespace="LinqToTwitter" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
	<title></title>
</head>
<body>
	<form id="form1" runat="server">
	<h1>Twitter updates </h1>
	<asp:MultiView ID="PrivateDataMultiView" runat="server">
		<asp:View ID="SetupTwitterConsumer" runat="server">
			<h2>Twitter Consumer setup</h2>
			<p>A Twitter client app must be endorsed by a Twitter user. </p>
			<ol>
				<li><a target="_blank" href="https://twitter.com/oauth_clients">Visit Twitter and
					create a client app</a>. </li>
				<li>Modify your web.config file to include your consumer key and consumer secret.</li>
			</ol>
			<p>Until you authorize this web app, we can only show you the <b>public</b> feed.</p>
		</asp:View>
		<asp:View ID="AuthorizeTwitter" runat="server">
			<h2>Authorize Twitter Client </h2>
			<p>Now that you have this sample configured with a Twitter consumer key and secret,
				you can authorize this web application to download your private Twitter messages.
			</p>
			<asp:Button runat="server" Text="Cool!  Authorize now." ID="authorizeTwitterButton"
				OnClick="authorizeTwitterButton_Click" />
			<p>Once you authorize this app, you'll see the results of a search on &quot;LINQ to Twitter&quot;.</p>
		</asp:View>
		<asp:View ID="ViewPrivateUpdates" runat="server">
			<p>Twitter has authorized us to download your feeds and now we're displaying your
				<b>personal</b> feed. Notice, <asp:Label runat="server" ID="screenNameLabel" Font-Bold="true" />,
				how we never asked you for your Twitter username or password. </p>
			<asp:Panel DefaultButton="postUpdateButton" runat="server">
				Post an update: <asp:TextBox ID="updateBox" runat="server" Columns="60" Text="Trying out LinqToTwitter's web update post sample." />
				<asp:Button ID="postUpdateButton" runat="server" Text="Post update" OnClick="postUpdateButton_Click" />
				&nbsp;<asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ControlToValidate="updateBox"
					ErrorMessage="type in some message first" />
			</asp:Panel>
		</asp:View>
	</asp:MultiView>
	<asp:ListView ID="TwitterListView" runat="server">
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
                    <asp:Repeater runat="server" DataSource='<%#Eval("Entities.UserMentions") %>'>
                        <ItemTemplate><%# Eval("ScreenName")%>,</ItemTemplate>
                    </asp:Repeater>
                    <asp:Repeater ID="Repeater1" runat="server" DataSource='<%#Eval("Entities.UrlMentions") %>'>
                        <ItemTemplate><%# Eval("Url")%>,</ItemTemplate>
                    </asp:Repeater>

                    <asp:Repeater ID="Repeater2" runat="server" DataSource='<%#Eval("Entities.HashTagMentions") %>'>
                        <ItemTemplate><%# Eval("Tag")%>,</ItemTemplate>
                    </asp:Repeater>
                </td>
			</tr>
		</ItemTemplate>
	</asp:ListView>
	</form>
</body>
</html>
