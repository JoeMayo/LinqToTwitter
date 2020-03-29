<%@ Page Title="Home Page" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="Linq2TwitterDemos_WebForms._Default" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">

    <div class="jumbotron">
        <h1>LINQ to Twitter</h1>
        <p class="lead">LINQ to Twitter is an open-source 3rd party library for working with the Twitter API. Please select a menu item to view demos for how LINQ to Twitter supports each category of the Twitter API.</p>
        <p><a href="http://linqtotwitter.codeplex.com" class="btn btn-primary btn-large">Learn more &raquo;</a></p>
    </div>

    <div class="row">
        <div class="col-md-4">
            <h2>Getting started</h2>
            <p>
                LINQ to Twitter has extensive documentation. You can get basic guidance, security info, and specifications for each API. Be sure to check out the FAQ too.
            </p>
            <p>
                <a class="btn btn-default" href="https://linqtotwitter.codeplex.com/documentation">Learn more &raquo;</a>
            </p>
        </div>
        <div class="col-md-4">
            <h2>Get help</h2>
            <p>
                When these demos don't have the info you need, you can still get help through the discussion forums. Tip: the Search box is often helpful in finding answers. 
            </p>
            <p>
                <a class="btn btn-default" href="https://linqtotwitter.codeplex.com/discussions">Learn more &raquo;</a>
            </p>
        </div>
        <div class="col-md-4">
            <h2>Get the code</h2>
            <p>
                You can download the code from the LINQ to Twitter site on CodePlex.com, but the most convenient way to use LINQ to Twitter is via NuGet.
            </p>
            <p>
                <a class="btn btn-default" href="http://www.nuget.org/packages/linqtotwitter">Learn more &raquo;</a>
            </p>
        </div>
    </div>

</asp:Content>
