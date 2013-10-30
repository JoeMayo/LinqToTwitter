<%@ Page Async="true" Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="HomeTimelineDemo.aspx.cs" Inherits="Linq2TwitterDemos_WebForms.StatusDemos.HomeTimelineDemo" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <h1>Home Timeline Demo</h1>
    <p>
        <asp:Button ID="RefreshButton" runat="server" Text="Refresh" OnClick="RefreshButton_Click" />
    </p>
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
                <td>
                    <asp:Label ID="NameLabel" runat="server" Text='<%#Eval("User.ScreenNameResponse") %>' />
                </td>
                <td>
                    <asp:Label ID="TweetLabel" runat="server" Text='<%#Eval("Text") %>' />
                </td>
                <td>
                    <asp:Repeater runat="server" DataSource='<%#Eval("Entities.UserMentionEntities") %>'>
                        <ItemTemplate><%# Eval("ScreenName")%>,</ItemTemplate>
                    </asp:Repeater>
                    <asp:Repeater ID="Repeater1" runat="server" DataSource='<%#Eval("Entities.UrlEntities") %>'>
                        <ItemTemplate><%# Eval("Url")%>,</ItemTemplate>
                    </asp:Repeater>

                    <asp:Repeater ID="Repeater2" runat="server" DataSource='<%#Eval("Entities.HashTagEntities") %>'>
                        <ItemTemplate><%# Eval("Tag")%>,</ItemTemplate>
                    </asp:Repeater>
                </td>
            </tr>
        </ItemTemplate>
    </asp:ListView>
</asp:Content>
