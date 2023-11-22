<%@ Page Title="" Language="C#" MasterPageFile="~/master/MasterPage_Login.master"
	AutoEventWireup="true" CodeFile="PDF_Manage_001_1.aspx.cs" Inherits="Passport_Passport_A000" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
	<style type="text/css">
		.NotShow {
			display: none;
		}

		.btnNotActive {
			background-color: #e7e7e7;
			color: black;
		}
		/* Gray */
		.btnActive {
			background-color: #555555;
		}
		/* Black */
	</style>
	<div class="">
		<table class="table table-sm">
			<thead>
				<tr>
					<th scope="col" colspan="5">
						<a href="PDF_Manage_001.aspx">PDF檔管理</a> >>
                       群組名稱
					</th>
				</tr>
			</thead>
			<tr>
				<th scope="col" colspan="5">
					<div class="row m-0">
						<div class="col-0">
							<label>
								關鍵字：
							</label>
						</div>
						<div class="col-4 col-md-3">
							<asp:TextBox ID="txtSearch" runat="server" CssClass="form-control"></asp:TextBox>
						</div>
						<div class="col">
							<asp:Button ID="btnQuery" runat="server" Text="查詢" CssClass="btn btn-secondary" OnClick="btnQuery_Click" />
							<button type="button" class="btn btn-secondary" onclick="showAdd();">新增</button>
						</div>
					</div>

				</th>
			</tr>
			<tr>
				<td style="height: 450px; vertical-align: top;">

					<table class="table">
						<tr>
							<th scope="col">群組名稱</th>
							<th>群組類別
							</th>
							<th scope="col">建立者</th>
							<th scope="col"></th>
						</tr>
						<asp:Repeater ID="Repeater1" runat="server" OnItemCommand="Repeater1_ItemCommand">
							<ItemTemplate>
								<tr>
									<td>
										<%# Eval("gmname")%>
									</td>
									<td>
										<%--<%# (string)Eval("gmcate")=="1"?"Lulu": (string)Eval("gmcate")=="2"?"UA": (string)Eval("gmcate")=="3"?"GAP": (string)Eval("gmcate")=="4"?"ON RUNNING": (string)Eval("gmcate")=="5"?"GYMSHARK":""%>--%>
                                        <%# Eval("CUSTOMER")%>
									</td>
									<td>
										<%# Eval("creator")%>
									</td>
									<td>
										<% if (LoginUser.role == LoginUser.ROLE_ADMIN)
											{%>
										<a href="#" onclick="showEdit('<%# Eval("gmid")%>','<%# Eval("gmname")%>','<%# Eval("gmcate")%>','<%# Eval("creator")%>','<%# Eval("createordate")%>');return false;">
											<input type="button" value="編輯" class="btn btn-secondary"></a>
										<asp:LinkButton ID="LinkButton2" CommandName="del" CommandArgument='<%# Eval("gmid")%>'
											runat="server" OnClientClick="return confirm('是否要刪除 ?');"><input type="button" value="刪除" class="btn btn-danger"></asp:LinkButton>
										<%} %>
									</td>
								</tr>
							</ItemTemplate>
						</asp:Repeater>
					</table>

				</td>
			</tr>
			<tr>
				<td style="height: 40px;">
					<asp:Panel ID="PageData1" runat="server" Visible="false">
						<asp:LinkButton ID="LinkPageFirst1" runat="server" OnClick="LinkPageFirst1_Click">第一頁</asp:LinkButton>&nbsp;
                    <asp:LinkButton ID="LinkPageUp1" runat="server" OnClick="LinkPageUp1_Click">上一頁</asp:LinkButton>&nbsp;
                    <asp:LinkButton ID="LinkPageDown1" runat="server" OnClick="LinkPageDown1_Click">下一頁</asp:LinkButton>&nbsp;
                    <asp:LinkButton ID="LinkPageLast1" runat="server" OnClick="LinkPageLast1_Click">最未頁</asp:LinkButton>&nbsp;
                    目前<asp:Label runat="server" ID="LabelNowPage1" Text="1"></asp:Label>頁,共<asp:Label
						ID="LabelPageCount1" runat="server" Text="0"></asp:Label>頁<asp:Label ID="LabelAllCount1"
							runat="server" Text="0"></asp:Label>筆
					</asp:Panel>
					<asp:Panel ID="panQuery" runat="server" Visible="false">
						<table border="0" cellpadding="0" cellspacing="0" width="90%">
							<tr>
								<td>
									<asp:Label ID="labErrMsg" runat="server" Text=""></asp:Label>
								</td>
							</tr>
						</table>
					</asp:Panel>
				</td>
			</tr>
		</table>
	</div>
	<div class="modal fade" id="addModal" tabindex="-1" role="dialog" aria-labelledby="exampleModalLabel" aria-hidden="true">
		<div class="modal-dialog" role="document">
			<div class="modal-content">
				<div class="modal-header">
					<h5 class="modal-title" id="addModalTitle"></h5>
					<button type="button" class="close" data-dismiss="modal" aria-label="Close">
						<span aria-hidden="true">&times;</span>
					</button>
				</div>
				<div class="modal-body">
					<table class="table">
						<tr>
							<th>群組名稱:<font color="red">*</font>
							</th>
							<td align="left">
								<asp:TextBox ID="gmname" runat="server" CssClass="form-control"></asp:TextBox>
							</td>
						</tr>
						<tr>
							<th>群組類別:<font color="red">*</font>
							</th>
							<td align="left">
								<asp:DropDownList ID="dlgmcate" runat="server">
								</asp:DropDownList>
							</td>
						</tr>
						<tr>
							<th>建立者:
							</th>
							<td align="left">
								<span id="span_creator"></span>
							</td>
						</tr>
						<tr>
							<th>建立者:
							</th>
							<td align="left">
								<span id="span_createordate"></span>
							</td>
						</tr>
						<tr>
							<td colspan="2" align="center">
								<asp:HiddenField ID="hidgmid" runat="server" />
								<asp:Button ID="Button1" runat="server" Text="取消" CssClass="btn btn-danger" data-dismiss="modal" />
								<asp:Button ID="btnAdd" runat="server" Text="確定" CssClass="btn btn-success" OnClick="btnAdd_Click" OnClientClick="return check()" />
								<asp:Button ID="btnEdit" runat="server" Text="儲存" CssClass="btn btn-warning" OnClick="btnEdit_Click" OnClientClick="return check()" />
							</td>
						</tr>
					</table>
				</div>
				<div class="modal-footer">
				</div>
			</div>
		</div>
	</div>
	<script type="text/javascript">
		$(document).ready(function () {
                <%=script %>


		});
		function showAdd() {
			$('#<%=btnAdd.ClientID %>').show();
			$('#<%=btnEdit.ClientID %>').hide();


			$('#<%=hidgmid.ClientID %>').val('');
			$('#<%=gmname.ClientID %>').val('');
			$("#span_creator").text('');
			$("#span_createordate").text('');

			$('#addModalTitle').text('新增');
			$('#addModal').modal('show')
		}

		function showEdit(gmid, gmname, gmcate, creator, createordate) {
			$('#<%=btnAdd.ClientID %>').hide();
			$('#<%=btnEdit.ClientID %>').show();


			$('#<%=hidgmid.ClientID %>').val(gmid);
			$('#<%=gmname.ClientID %>').val(gmname);
			$('#<%=dlgmcate.ClientID %>').val(gmcate);
			$("#span_creator").text(creator);
			$("#span_createordate").text(createordate);

			$('#addModalTitle').text('編輯');
			$('#addModal').modal('show')


		}
		function check() {
			let gmname = $('#<%=gmname.ClientID %>').val();
			let gmcate = $('#<%=dlgmcate.ClientID %>').val();

			if (!gmname) {
				alert('群組名稱 不可空白');
				return false;
			}

			gmname = gmname.replace(/ /g, "");

			if (!gmname) {
				alert('群組名稱不能只有空白');
				return false;
			}

			if (!gmcate) {
				alert('群組類別 必選');
				return false;
			}
		}


		function Open(filePath) {
			window.open(filePath);

		}





	</script>
</asp:Content>
