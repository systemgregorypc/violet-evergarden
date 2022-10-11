<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="DepartmentDataList.aspx.cs" Inherits="RinnaiPortal.Area.Sign.DepartmentDataList" %>
<%@ Import Namespace="violetevergardenPortal.Extensions" %>
<%@ Import Namespace="violetevergardenPortal.Tools" %>

<asp:Content ID="DepartmentDataListContent" ContentPlaceHolderID="MainContent" runat="server">
<input runat="server" type="text" hidden id="PageTitle" class='page-title' value="" />
    <div id="layout-content-wrapper">
        <div id="layout-content" class="container">
                <div class="list-toolbar">
                    <a class="btn btn-success pull-left" href="DepartmentData.aspx">
                        <span class="glyphicon glyphicon-plus"></span>&nbsp;新增
                    </a>

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

                    <div class="row text-center">
                        <div class="col-lg-3" style="float: none; display: inline-block">
                            <div class="input-group">
                                <asp:TextBox runat="server" ID="queryTextBox" class="form-control" placeholder="部門代碼、名稱、主管姓名" />
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
                <asp:GridView runat="server" CssClass="table table-striped table-bordered table-condensed" ID="DepartmentGridView" AutoGenerateColumns="False">
                    <Columns>
                        <asp:TemplateField>
                            <HeaderTemplate>
                                <asp:HyperLink runat="server" ClientIDMode="Static" ID="DepartmentID" NavigateUrl='<%# WebUtils.GetGridViewHeadUrl("DepartmentID", Request)%>' Text="部門代碼"></asp:HyperLink>
                            </HeaderTemplate>
                            <ItemTemplate>
                                <asp:Label runat="server" Text='<%# Eval("DepartmentID") %>'> </asp:Label>
                            </ItemTemplate>
                            <HeaderStyle CssClass="info" />
                        </asp:TemplateField>
                        <asp:TemplateField>
                            <HeaderTemplate>
                                <asp:HyperLink runat="server" ClientIDMode="Static" ID="DepartmentName" NavigateUrl='<%# WebUtils.GetGridViewHeadUrl("DepartmentName", Request)%>' Text="部門名稱"></asp:HyperLink>
                            </HeaderTemplate>
                            <ItemTemplate>
                                <asp:Label runat="server" Text='<%# Eval("DepartmentName") %>'> </asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField>
                            <HeaderTemplate>
                                <asp:HyperLink runat="server" ClientIDMode="Static" ID="ChiefID_FK" NavigateUrl='<%# WebUtils.GetGridViewHeadUrl("ChiefID_FK", Request)%>' Text="主管員工編號"></asp:HyperLink>
                            </HeaderTemplate>
                            <ItemTemplate>
                                <asp:Label runat="server" Text='<%# Eval("ChiefID_FK") %>'> </asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField>
                            <HeaderTemplate>
                                <asp:HyperLink runat="server" ClientIDMode="Static" ID="ChiefName" NavigateUrl='<%# WebUtils.GetGridViewHeadUrl("ChiefName", Request)%>' Text="主管姓名"></asp:HyperLink>
                            </HeaderTemplate>
                            <ItemTemplate>
                                <asp:Label runat="server" Text='<%# Eval("ChiefName") %>'> </asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField>
                            <HeaderTemplate>
                                <asp:HyperLink runat="server" ClientIDMode="Static" ID="UpperDepartmentID" NavigateUrl='<%# WebUtils.GetGridViewHeadUrl("UpperDepartmentID", Request)%>' Text="上層部門代碼"></asp:HyperLink>
                            </HeaderTemplate>
                            <ItemTemplate>
                                <asp:Label runat="server" Text='<%# Eval("UpperDepartmentID") %>'> </asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField>
                            <HeaderTemplate>
                                <asp:HyperLink runat="server" ClientIDMode="Static" ID="UpperDepartmentName" NavigateUrl='<%# WebUtils.GetGridViewHeadUrl("UpperDepartmentName", Request)%>' Text="上層部門名稱"></asp:HyperLink>
                            </HeaderTemplate>
                            <ItemTemplate>
                                <asp:Label runat="server" Text='<%# Eval("UpperDepartmentName") %>'> </asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField>
                            <HeaderTemplate>
                                <asp:HyperLink runat="server" ClientIDMode="Static" ID="DepartmentLevel" NavigateUrl='<%# WebUtils.GetGridViewHeadUrl("DepartmentLevel", Request)%>' Text="階級"></asp:HyperLink>
                            </HeaderTemplate>
                            <ItemTemplate>
                                <asp:Label runat="server" Text='<%# Eval("DepartmentLevel") %>'> </asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField>
                            <HeaderTemplate>
                                <asp:HyperLink runat="server" ClientIDMode="Static" ID="FilingEmployeeID_FK" NavigateUrl='<%# WebUtils.GetGridViewHeadUrl("FilingEmployeeID_FK", Request)%>' Text="歸檔員工編號"></asp:HyperLink>
                            </HeaderTemplate>
                            <ItemTemplate>
                                <asp:Label runat="server" Text='<%# Eval("FilingEmployeeID_FK") %>'> </asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField>
                            <HeaderTemplate>
                                <asp:HyperLink runat="server" ClientIDMode="Static" ID="FilingEmployeeName" NavigateUrl='<%# WebUtils.GetGridViewHeadUrl("FilingEmployeeName", Request)%>' Text="歸檔員工姓名"></asp:HyperLink>
                            </HeaderTemplate>
                            <ItemTemplate>
                                <asp:Label runat="server" Text='<%# Eval("FilingEmployeeName") %>'> </asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField>
                            <HeaderTemplate>
                                <asp:HyperLink runat="server" ClientIDMode="Static" ID="Disabled" NavigateUrl='<%# WebUtils.GetGridViewHeadUrl("Disabled", Request)%>' Text="停用"></asp:HyperLink>
                            </HeaderTemplate>
                            <ItemTemplate>
                                <asp:Label runat="server" Text='<%# "True".Equals(Eval("Disabled").ToString(), StringComparison.OrdinalIgnoreCase) ? "是" : "否" %>' title='<%# "停用日期:" + Eval("DisabledDate").ToDateTimeFormateString() %>'> </asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
<%--                        <asp:TemplateField>
                            <HeaderTemplate>
                                <asp:HyperLink runat="server" ClientIDMode="Static" ID="DisabledDate" NavigateUrl='<%# WebUtils.GetGridViewHeadUrl("DisabledDate", Request)%>' Text="停用日期"></asp:HyperLink>
                            </HeaderTemplate>
                            <ItemTemplate>
                                <asp:Label runat="server" Text='<%# Eval("DisabledDate").ToDateTimeFormateString() %>'> </asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>--%>
                        <asp:TemplateField>
                            <HeaderTemplate>
                                <asp:HyperLink runat="server" ClientIDMode="Static" ID="Creator" NavigateUrl='<%# WebUtils.GetGridViewHeadUrl("Creator", Request)%>' Text="建立者"></asp:HyperLink>
                            </HeaderTemplate>
                            <ItemTemplate>
                                <asp:Label runat="server" Text='<%# Eval("Creator") %>'> </asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField>
                            <HeaderTemplate>
                                <asp:HyperLink runat="server" ClientIDMode="Static" ID="CreateDate" NavigateUrl='<%# WebUtils.GetGridViewHeadUrl("CreateDate", Request)%>' Text="建立日期"></asp:HyperLink>
                            </HeaderTemplate>
                            <ItemTemplate>
                                <asp:Label runat="server" Text='<%# Eval("CreateDate").ToDateTimeFormateString() %>'> </asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField>
                            <HeaderTemplate>
                                <asp:HyperLink runat="server" ClientIDMode="Static" ID="Modifier" NavigateUrl='<%# WebUtils.GetGridViewHeadUrl("Modifier", Request)%>' Text="編輯者"></asp:HyperLink>
                            </HeaderTemplate>
                            <ItemTemplate>
                                <asp:Label runat="server" Text='<%# Eval("Modifier") %>'> </asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField>
                            <HeaderTemplate>
                                <asp:HyperLink runat="server" ClientIDMode="Static" ID="ModifyDate" NavigateUrl='<%# WebUtils.GetGridViewHeadUrl("ModifyDate", Request)%>' Text="編輯日期"></asp:HyperLink>
                            </HeaderTemplate>
                            <ItemTemplate>
                                <asp:Label runat="server" Text='<%# Eval("ModifyDate").ToDateTimeFormateString() %>'> </asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="編輯" HeaderStyle-CssClass="text-center">
                            <ItemStyle CssClass="vertical-middle" />
                            <ItemTemplate>
                                <asp:HyperLink runat="server" CommandName="Edit" CssClass="btn btn-warning btn-xs" NavigateUrl='<%# "~/Area/Sign/DepartmentData.aspx?DepartmentID=" + Eval("DepartmentID") %>'
                                    Text="編輯" Width="50px"><span class="glyphicon glyphicon-pencil"></span></asp:HyperLink>
                            </ItemTemplate>                            
                        </asp:TemplateField>
                    </Columns>
                    <RowStyle CssClass="text-center" />
                    <AlternatingRowStyle BackColor="#f9f9f9" CssClass="text-center" />
                    <HeaderStyle CssClass="text-nowrap info" Font-Bold="true" BackColor="#dff0d8" />
                </asp:GridView>
            </div>
            <div runat="server" id="paginationBar" class="text-center"></div>

        </div>
</asp:Content>
