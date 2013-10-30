<%@ Page Async="true" Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="TweetDemo.aspx.cs" Inherits="Linq2TwitterDemos_WebForms.StatusDemos.TweetDemo" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <h1>Tweet Demo</h1>
    <p></p>
	<p>Post an update:</p> 
    <p>
        <asp:TextBox ID="UpdateTextBox" runat="server" Columns="70" Rows="2" />
        <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ControlToValidate="UpdateTextBox"
            ErrorMessage="type in some message first" />
    </p>
    <p>
        <asp:Button ID="PostUpdateButton" runat="server" Text="Post update" OnClick="PostUpdateButton_Click" />
    </p>
    <p>
        <asp:Label runat="server" EnableViewState="false" Text="Update posted." Visible="false" ID="SuccessLabel" />
    </p>
	
</asp:Content>
