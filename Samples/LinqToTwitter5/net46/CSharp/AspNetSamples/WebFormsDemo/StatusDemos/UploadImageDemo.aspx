<%@ Page Async="true" Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="UploadImageDemo.aspx.cs" Inherits="WebFormsDemo.StatusDemos.UploadImageDemo" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <h1>Upload Image Demo</h1>
    <p>
        <asp:Button ID="RefreshButton" runat="server" Text="Refresh" OnClick="UploadButton_Click" />
    </p>
    <table>
        <tr>
            <td>
                <asp:Image ID="UserImage" runat="server" />
            </td>
            <td>
                <asp:Label ID="NameLabel" runat="server" />
            </td>
            <td>
                <asp:Label ID="TweetLabel" runat="server" />
            </td>
        </tr>
    </table>
</asp:Content>
