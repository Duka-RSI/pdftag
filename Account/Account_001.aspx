<%@ Page Title="" Language="C#" MasterPageFile="~/master/MasterPage_Login.master"
	AutoEventWireup="true" CodeFile="Account_001.aspx.cs" Inherits="Passport_Passport_A000" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
	<div class="">
		<table class="table">
			<thead>
				<tr>
					<th scope="col" colspan="5">人員管理</th>
				</tr>
			</thead>
			<thead>
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
			</thead>
			<tr>
				<td style="height: 450px; vertical-align: top;">

					<table class="table">
						<tr>
							<th scope="col">英文姓名</th>
							<th scope="col">中文姓名</th>
							<th scope="col">帳號</th>
							<th scope="col">部門</th>
							<th scope="col">版本</th>
							<th scope="col">權限</th>
							<th scope="col"></th>
						</tr>
						<asp:Repeater ID="Repeater1" runat="server" OnItemCommand="Repeater1_ItemCommand">
							<ItemTemplate>
								<tr>
									<td>
										<%# Eval("USER_NAME")%>
									</td>
									<td>
										<%# Eval("USER_FULLNAME")%>
									</td>
									<td>
										<%# Eval("USER_AD")%>
									</td>
									<td>
										<%# Eval("DEPARTMENT")%>
									</td>
									<td>
										<%# Eval("CUST")%>
									</td>
									<td>
										<%#(int)Eval("Role")==1?"管理者":(int)Eval("Role")==2?"使用者":""%>
									</td>
									<td>
										<a href="#" onclick="showEdit('<%# Eval("aid")%>','<%# Eval("USER_NAME")%>','<%# Eval("USER_FULLNAME")%>','<%# Eval("USER_AD")%>','<%# Eval("DEPARTMENT")%>','<%# Eval("CUST_NO")%>','<%# Eval("Role")%>');return false;">
											<input type="button" value="編輯" class="btn btn-secondary"></a>
										<asp:LinkButton ID="LinkButton1" CommandName="del" CommandArgument='<%# Eval("aid")%>'
											runat="server" OnClientClick="return confirm('是否要刪除 ?');"><input type="button" value="刪除" class="btn btn-danger"></asp:LinkButton>

										<asp:LinkButton ID="LinkButton2" CommandName="resetPw" CommandArgument='<%# Eval("aid")%>'
											runat="server" OnClientClick="return confirm('確定設定預設密碼 ?');"><input type="button" value="預設密碼" class="btn btn-warning"></asp:LinkButton>
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
								<th>英文姓名:
								</th>
								<td align="left">
									<asp:TextBox ID="USER_NAME" runat="server" CssClass="form-control"></asp:TextBox>
								</td>
								<th>中文姓名:
								</th>
								<td align="left">
									<asp:TextBox ID="USER_FULLNAME" runat="server" CssClass="form-control"></asp:TextBox>
								</td>
							</tr>
							<tr>
								<th>帳號:<font color="red">*</font>
								</th>
								<td align="left">
									<asp:TextBox ID="USER_AD" runat="server" CssClass="form-control"></asp:TextBox>
								</td>
								<th>部門:
								</th>
								<td align="left">
                                    <asp:DropDownList ID="DEPARTMENT" runat="server" CssClass="form-control" CheckBoxes="True">
                                    </asp:DropDownList>
								</td>
							</tr>
							<tr>
								<th>權限:
								</th>
								<td align="left">
									<asp:DropDownList ID="dlRole" runat="server">
										<asp:ListItem Value="1" Text="管理者"></asp:ListItem>
										<asp:ListItem Value="2" Text="使用者"></asp:ListItem>
									</asp:DropDownList>
								</td>
							</tr>
							<tr>
								<td colspan="2" align="center">
									<asp:HiddenField ID="hidaid" runat="server" />
									<asp:Button ID="Button6" runat="server" Text="取消" CssClass="btn btn-danger" data-dismiss="modal" />
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

			function check() {
				let USER_AD = $('#<%=USER_AD.ClientID %>').val();
				if (!USER_AD) {
					alert('帳號不可空白');
					return false;
				}

				USER_AD = USER_AD.replace(/ /g, "");

				if (!USER_AD) {
					alert('帳號不能只有空白');
					return false;
				}
			}

			function clearAddOrEditPanel() {
				$('#<%=USER_NAME.ClientID %>').val('');
				$('#<%=USER_FULLNAME.ClientID %>').val('');
				$('#<%=USER_AD.ClientID %>').val('');
				$('#<%=DEPARTMENT.ClientID %>').val('');

				$('#<%=dlRole.ClientID %>').val('1');

				$('#<%=hidaid.ClientID %>').val('');
			}

			function showAdd() {
				$('#<%=btnAdd.ClientID %>').show();
				$('#<%=btnEdit.ClientID %>').hide();

				clearAddOrEditPanel();

				$('#addModalTitle').text('新增');
				$('#addModal').modal('show')
			}
			function showEdit(aid, USER_NAME, USER_FULLNAME, USER_AD, DEPARTMENT, CUST_NO, Role) {
				$('#<%=btnAdd.ClientID %>').hide();
				$('#<%=btnEdit.ClientID %>').show();



				clearAddOrEditPanel();


				$('#<%=hidaid.ClientID %>').val(aid);


				$('#<%=USER_NAME.ClientID %>').val(USER_NAME);
				$('#<%=USER_FULLNAME.ClientID %>').val(USER_FULLNAME);
				$('#<%=USER_AD.ClientID %>').val(USER_AD);
				$('#<%=DEPARTMENT.ClientID %>').val(DEPARTMENT);

				$('#<%=dlRole.ClientID %>').val(Role);

				$('#addModalTitle').text('編輯');
				$('#addModal').modal('show')
			}



		</script>
	</div>
</asp:Content>
