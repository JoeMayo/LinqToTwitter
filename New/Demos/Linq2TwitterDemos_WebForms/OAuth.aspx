<%@ Page Async="true" Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="OAuth.aspx.cs" Inherits="Linq2TwitterDemos_WebForms.OAuth" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <p></p>
    <p>
        Please press this button to start the Twitter OAuth authorization process:
    </p>
    <p>
        <asp:Button ID="AuthorizeButton" runat="server" OnClick="AuthorizeButton_Click" Text="Authorize" />
    </p>
</asp:Content>
