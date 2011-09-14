<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Assembly Name="Microsoft.SharePoint.ApplicationPages, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %> 
<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ManageServiceApplication.aspx.cs" Inherits="ParagoServices.AdminUI.ManageServiceApplicationPage" MasterPageFile="/_admin/admin.master" %>

<%@ Register Tagprefix="SP" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>

<asp:Content ContentPlaceHolderId="PlaceHolderPageTitle" runat="server">
    Parago Service Application Settings
</asp:Content>

<asp:Content ContentPlaceHolderID="PlaceHolderPageTitleInTitleArea" runat="server">
    Parago Service Application Settings
</asp:Content>

<asp:Content ContentPlaceHolderId="PlaceHolderAdditionalPageHead" runat="server">
</asp:Content>

<asp:content ContentPlaceHolderID="PlaceHolderMain" runat="server">         
	<SP:FormDigest runat="server" />
	<p>...</p>
</asp:content>
