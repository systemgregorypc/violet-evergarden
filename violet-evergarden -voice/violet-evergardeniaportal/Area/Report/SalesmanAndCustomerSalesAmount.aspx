<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="SalesmanAndCustomerSalesAmount.aspx.cs" Inherits="RinnaiPortal.Area.Report.SalesmanAndCustomerSalesAmount" %>
<asp:Content ID="SalesmanAndCustomerSalesAmountContent" ContentPlaceHolderID="MainContent" runat="server">
    <input runat="server" type="text" hidden id="PageTitle" class='page-title' value="" />
    <div class="embed-responsive embed-responsive-16by9">
        <iframe runat="server" class="embed-responsive-item" id="SalesmanAndCustomerSalesAmountFrame"  scrolling="no"  ></iframe>
    </div>
</asp:Content>
