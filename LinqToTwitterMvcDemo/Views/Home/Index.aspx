<%@ Page Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<List<LinqToTwitterMvcDemo.Models.TweetViewModel>>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    Friend Tweets
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

<ul>
<%
    foreach (var tweet in Model)
    {
    %>
        <li><%: tweet.ScreenName %>: <%: tweet.Tweet %></li>
    <%    
    }
%>
</ul>

</asp:Content>
