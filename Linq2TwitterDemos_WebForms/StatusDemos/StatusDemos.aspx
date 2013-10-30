<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="StatusDemos.aspx.cs" Inherits="Linq2TwitterDemos_WebForms.StatusDemos.StatusDemos" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <h1>Status Demos</h1>
    <p></p>
    <p>
        <asp:HyperLink ID="TweetDemoHyperLink" runat="server" NavigateUrl="~/StatusDemos/TweetDemo.aspx">Tweet Demo</asp:HyperLink>
    </p>
    <p>
        <asp:HyperLink ID="HomeTimelineDemoHyperLink" runat="server" NavigateUrl="~/StatusDemos/HomeTimelineDemo.aspx">Home Timeline Demo</asp:HyperLink>
    </p>
</asp:Content>
