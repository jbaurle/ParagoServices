<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Assembly Name="Microsoft.SharePoint.ApplicationPages, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %> 
<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="CreateServiceApplication.aspx.cs" Inherits="ParagoServices.AdminUI.CreateServiceApplicationPage" MasterPageFile="~/_layouts/dialog.master" %>
<%@ Register Tagprefix="SP" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %> 
<%@ Register TagPrefix="SP" TagName="InputFormSection" src="/_controltemplates/InputFormSection.ascx" %> 
<%@ Register TagPrefix="SP" TagName="InputFormControl" src="/_controltemplates/InputFormControl.ascx" %> 
<%@ Register TagPrefix="SP" TagName="IisWebServiceApplicationPoolSection" src="~/_admin/IisWebServiceApplicationPoolSection.ascx" %>
<%@ Import Namespace="Microsoft.SharePoint" %> 
<%@ Import Namespace="Microsoft.SharePoint.ApplicationPages" %> 

<asp:Content ContentPlaceHolderID="PlaceHolderDialogHeaderPageTitle" runat="server">
	Create New Parago Service Application
</asp:Content>

<asp:Content ContentPlaceHolderID="PlaceHolderDialogDescription" runat="server">
	Please specify the settings for the new Parago Service Application. The settings you specify here can be changed later at the Manage Service Applications page.
</asp:Content>

<asp:Content ContentPlaceHolderId="PlaceHolderAdditionalPageHead" runat="server">
</asp:Content>

<asp:content ContentPlaceHolderID="PlaceHolderDialogBodyMainSection" runat="server">
	<SP:FormDigest runat="server" />
	<table width="100%" class="ms-propertysheet" cellspacing="5" cellpadding="0" border="0"> 
		<tr> 
			<td class="ms-descriptionText"> 
				<asp:ValidationSummary ID="ValidationSummary" HeaderText="This page contains one or more errors. Fix the following before continuing:" DisplayMode="BulletList" ShowSummary="True" runat="server" /> 
			</td> 
		</tr> 
	</table>
	<table border="0" cellspacing="0" cellpadding="0" width="100%">
    	<SP:InputFormSection Title="Name" Description="Enter the name of the Parago Service Application. The name entered here will be used in the list of Service Applications displayed in the Manage Service Applications page." runat="server">
			<Template_InputFormControls>
				<SP:InputFormControl LabelText="Service Application Name" runat="server">
					<Template_Control>
						<SP:InputFormTextBox ID="ServiceApplicationNameTextBox" Title="Service Application Name" class="ms-input" Columns="35" MaxLength="256" runat="server" />
						<SP:InputFormRequiredFieldValidator ID="ServiceApplicationNameValidator" ControlToValidate="ServiceApplicationNameTextBox" ErrorMessage="Missing service application name value" runat="server" />
					</Template_Control>
				</SP:InputFormControl>
			</Template_InputFormControls>
		</SP:InputFormSection>
		<SP:IisWebServiceApplicationPoolSection ID="ApplicationPoolSection" runat="server">
			<Template_Description>
				<SP:EncodedLiteral Text="<%$Resources:spadmin, multipages_iiswebserviceapppool_desc1%>" EncodeMethod="HtmlEncodeAllowSimpleTextFormatting" runat="server" />
				<br /><br />
				<SP:EncodedLiteral Text="<%$Resources:spadmin, multipages_apppool_desc2%>" EncodeMethod="HtmlEncodeAllowSimpleTextFormatting" runat="server" />
				<asp:CustomValidator ID="ApplicationPoolValidator" Text="*" OnServerValidate="OnApplicationPoolValidate" runat="server" />
			</Template_Description>
		</SP:IisWebServiceApplicationPoolSection>
	</table>
</asp:Content>
