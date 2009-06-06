<%@ Page Language="C#" AutoEventWireup="true"  CodeFile="Default.aspx.cs" Inherits="_Default" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
    
        <asp:ListView ID="TwitterListView" runat="server">
            <LayoutTemplate>
                <table runat="server">
                    <tr runat="server">
                        <th>Picture</th>
                        <th>Name</th>
                        <th>Last Tweet</th>
                    </tr>
                    <tr id="itemPlaceholder"></tr>
                </table>
            </LayoutTemplate>
            <ItemTemplate>
                <tr runat="server">
                    <td>
                        <asp:Image ID="UserImage" runat="server" ImageUrl='<%#Eval("User.ProfileImageUrl") %>' />
                    </td>
                    <td>
                        <asp:Label ID="NameLabel" runat="server" Text='<%#Eval("User.Name") %>' />
                    </td>
                    <td>
                        <asp:Label ID="TweetLabel" runat="server" Text='<%#Eval("Text") %>' />
                    </td>
                </tr>
            </ItemTemplate>
        </asp:ListView>
    
    </div>
    </form>
</body>
</html>
