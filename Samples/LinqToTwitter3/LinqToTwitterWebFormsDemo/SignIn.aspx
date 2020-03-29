<%@ Page Language="C#" AutoEventWireup="true" CodeFile="SignIn.aspx.cs" Inherits="SignIn" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
	<title></title>
</head>
<body>
	<form id="form1" runat="server">
	<asp:MultiView ID="AuthMultiView" runat="server" ActiveViewIndex="0">
		<asp:View ID="SetupTwitterConsumer" runat="server">
			<h2>
				Twitter Consumer setup</h2>
			<p>
				A Twitter client app must be endorsed by a Twitter user.
			</p>
			<ol>
				<li><a target="_blank" href="https://twitter.com/oauth_clients">Visit Twitter and create
					a client app</a>. </li>
				<li>Modify your web.config file to include your consumer key and consumer secret.</li>
			</ol>
		</asp:View>
		<asp:View ID="StartSignInView" runat="server">
			<asp:ImageButton ID="signInButton" runat="server" 
				ImageUrl="~/images/Sign-in-with-Twitter-darker.png" 
				onclick="signInButton_Click" />
		</asp:View>
		<asp:View ID="SignedInView" runat="server">
			You've signed in as <asp:Label runat="server" ID="screenNameLabel" />
		</asp:View>
	</asp:MultiView>
	</form>
</body>
</html>
