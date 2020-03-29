<%@ Page Language="C#" AutoEventWireup="true" CodeFile="PostOnly.aspx.cs" Inherits="PostOnly" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
	<title></title>
</head>
<body>
	<form id="form1" runat="server">
	<asp:Panel runat="server" DefaultButton="postUpdateButton">
		Post an update: <asp:TextBox ID="updateBox" runat="server" Columns="60" Text="Trying out LinqToTwitter's web update post sample." />
		<asp:Button ID="postUpdateButton" runat="server" Text="Post update" OnClick="postUpdateButton_Click" />
		&nbsp;<asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ControlToValidate="updateBox"
			ErrorMessage="type in some message first" />
	</asp:Panel>
	<asp:Label runat="server" EnableViewState="false" Text="Update posted." Visible="false"
		ID="successLabel" /> <asp:HyperLink runat="server" Text="Go back to view updates"
			NavigateUrl="~/Default.aspx" />
	</form>
</body>
</html>
