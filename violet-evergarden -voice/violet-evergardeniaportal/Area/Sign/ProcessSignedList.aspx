<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="ProcessSignedList.aspx.cs" Inherits="RinnaiPortal.Area.Sign.ProcessSignedList" %>
<%@ Import Namespace="RinnaiPortal.Extensions" %>
<%@ Import Namespace="RinnaiPortal.Tools" %>

<asp:Content ID="ProcessSignedListContent" ContentPlaceHolderID="MainContent" runat="server">
    <input runat="server" type="text" hidden id="PageTitle" class='page-title' value="" />
    <div id="layout-content-wrapper">
        <div id="layout-content" class="container">
            <div class="list-toolbar">
                <div class="col-lg-1 input-group pull-right">
                    <span class="input-group-addon">每頁</span>
                    <asp:DropDownList runat="server" ID="pageSizeSelect"  class="form-control" data-val="true" data-val-number="The field PageSize must be a number." data-val-required="The PageSize field is required." Style="width: 70px" OnSelectedIndexChanged="PageSize_SelectedIndexChanged" AutoPostBack="true">
                        <asp:ListItem Text="10" Value="10"></asp:ListItem>
                        <asp:ListItem Text="20" Value="20"></asp:ListItem>                            
                        <asp:ListItem Text="30" Value="30"></asp:ListItem>
                    </asp:DropDownList>
                    <span class="input-group-addon">筆</span>
                    <span class="input-group-addon">總共&nbsp;
                        <asp:Label runat="server" ID="totalRowsCount"></asp:Label>
                        &nbsp;筆</span>
                </div>
                <div class="row">
                    <div class="col-lg-3 pull-left" style="float: none; display: inline-block">
                        <div class="input-group">
                            <asp:TextBox runat="server" ID="queryTextBox" class="form-control" placeholder="送簽人員、簽核編號、姓名" />
                            <span class="input-group-btn">
                                <asp:LinkButton runat="server" class="btn btn-default" ID="queryBtn" OnClick="QueryBtn_Click"> <span class="glyphicon glyphicon-search"></span>&nbsp;查詢 </asp:LinkButton>
                            </span>
                        </div>
                    </div>
                </div>
            </div>
            <div class="text-center">
                <asp:Label runat="server" id="noDataTip" ></asp:Label>
            </div>
            <asp:GridView runat="server" CssClass="table table-striped table-bordered table-condensed" ID="ProcessSignedGridView" AutoGenerateColumns="False">
                <Columns>
                <asp:TemplateField>
                    <HeaderTemplate>
                        <asp:HyperLink runat="server" ClientIDMode="Static" ID="SignDocID" NavigateUrl='<%# WebUtils.GetGridViewHeadUrl("SignDocID_FK", Request)%>' Text="文件編號"></asp:HyperLink>
                    </HeaderTemplate>
                    <ItemTemplate>
                        <asp:Label runat="server" Text='<%# Eval("SignDocID_FK") %>'> </asp:Label>
                    </ItemTemplate>
                    <HeaderStyle CssClass="info" />
                </asp:TemplateField>
                <asp:TemplateField>
					<HeaderTemplate>
						<asp:HyperLink runat="server" ClientIDMode="Static" ID="FormType" NavigateUrl='<%# WebUtils.GetGridViewHeadUrl("FormType", Request)%>' Text="表單類型"></asp:HyperLink>
					</HeaderTemplate>
					<ItemTemplate>
						<a runat="server" cssclass="btn btn-link" target="dialog" href='<%# GetFormUrl(Eval("SignDocID_FK"), Eval("EmployeeID_FK"), Eval("FormID_FK"))%>' text=''><%# Eval("FormType") %><i class="glyphicon glyphicon-info-sign" style="top: 3px"></i></a>
						<input runat="server" ID="FormID" visible="false" Value='<%# Eval("FormID_FK") %>' />
					</ItemTemplate>
				</asp:TemplateField>
                <%--<asp:TemplateField>
                    <HeaderTemplate>
                        <asp:HyperLink runat="server" ClientIDMode="Static" ID="FormType" NavigateUrl='<%# WebUtils.GetGridViewHeadUrl("FormType", Request)%>' Text="表單類型"></asp:HyperLink>
                    </HeaderTemplate>
                    <ItemTemplate>
                        <asp:Label runat="server" Text='<%# Eval("FormType") %>'> </asp:Label>
                    </ItemTemplate>
                </asp:TemplateField>   --%>
                <asp:TemplateField>
                    <HeaderTemplate>
                        <asp:HyperLink runat="server" ClientIDMode="Static" ID="EmployeeID_FK" NavigateUrl='<%# WebUtils.GetGridViewHeadUrl("EmployeeID_FK", Request)%>' Text="送簽人員編號"></asp:HyperLink>
                    </HeaderTemplate>
                    <ItemTemplate>
                        <asp:Label runat="server" Text='<%# Eval("EmployeeID_FK") %>'> </asp:Label>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField>
                    <HeaderTemplate>
                        <asp:HyperLink runat="server" ClientIDMode="Static" ID="EmployeeName" NavigateUrl='<%# WebUtils.GetGridViewHeadUrl("EmployeeName", Request)%>' Text="送簽人員姓名"></asp:HyperLink>
                    </HeaderTemplate>
                    <ItemTemplate>
                        <asp:Label runat="server" Text='<%# Eval("EmployeeName") %>'> </asp:Label>
                    </ItemTemplate>
                </asp:TemplateField>   
                <asp:TemplateField>
                    <HeaderTemplate>
                        <asp:HyperLink runat="server" ClientIDMode="Static" ID="SendDate" NavigateUrl='<%# WebUtils.GetGridViewHeadUrl("departmentname", Request)%>' Text="送簽部門"></asp:HyperLink>
                    </HeaderTemplate>
                    <ItemTemplate>
                        <asp:Label runat="server" Text='<%# Eval("departmentname") %>'> </asp:Label>
                    </ItemTemplate>
                </asp:TemplateField>                        
                <asp:TemplateField>
                    <HeaderTemplate>
                        <asp:HyperLink runat="server" ClientIDMode="Static" ID="FinalStatus" NavigateUrl='<%# WebUtils.GetGridViewHeadUrl("FinalStatus", Request)%>' Text="最終簽核狀態"></asp:HyperLink>
                    </HeaderTemplate>
                    <ItemTemplate>
                        <asp:Label runat="server" Text='<%# ViewUtils.ParseStatus(Eval("FinalStatus").ToString())%>'> </asp:Label>
                    </ItemTemplate>
                </asp:TemplateField>
                <%--<asp:TemplateField HeaderText="明細" HeaderStyle-CssClass="text-center">
					<ItemStyle CssClass="vertical-middle" />
					<ItemTemplate>
						<asp:HyperLink runat="server" CommandName="Detail" CssClass="btn btn-info btn-xs" Target="dialog" NavigateUrl='<%# "~/Area/Sign/WorkflowDetail.aspx?signDocID=" + Eval("SignDocID_FK") %>'
							Text="明細" Width="50px"><span class="glyphicon glyphicon-list-alt"></span></asp:HyperLink>
					</ItemTemplate>
				</asp:TemplateField>--%>                               
            </Columns>
            <RowStyle CssClass="text-center" />
            <AlternatingRowStyle BackColor="#f9f9f9" CssClass="text-center" />
            <HeaderStyle CssClass="text-nowrap info" Font-Bold="true" BackColor="#dff0d8" />
            </asp:GridView>
        </div>
        <div runat="server" id="paginationBar" class="text-center"></div>
    </div>


</asp:Content>
