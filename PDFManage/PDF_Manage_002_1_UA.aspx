<%@ Page Title="" Language="C#" MasterPageFile="~/master/MasterPage_Login.master"
	AutoEventWireup="true" CodeFile="PDF_Manage_002_1_UA.aspx.cs" Inherits="Passport_Passport_A000" %>

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

		.span_w {
			overflow-wrap: break-word;
			-webkit-hyphens: auto;
			-ms-hyphens: auto;
			hyphens: auto;
		}
	</style>
	<div class="">
		<table class="table table-sm">
			<thead>
				<tr>
					<th scope="col" colspan="5">
						<a href="PDF_Manage_002.aspx">歷程管理</a> >>
                        <asp:Label ID="lblInfo" runat="server"></asp:Label>:進入編輯
                        <asp:HiddenField ID="hidpipid" runat="server" />
					</th>
				</tr>
			</thead>
			<%-- <tr>
                <th scope="col" colspan="5">
                    <div class="row m-0">
                        <div class="col-0">
                            <label>
                                項目：
                            </label>
                        </div>
                        <div class="col-4 col-md-3">
                            <asp:DropDownList ID="dlVersion" runat="server" CssClass="form-control">
                                <asp:ListItem Value="1" Text="Header"></asp:ListItem>
                                <asp:ListItem Value="2" Text="UA"></asp:ListItem>
                                <asp:ListItem Value="3" Text="GAP"></asp:ListItem>
                            </asp:DropDownList>
                        </div>
                        <div class="col">
                            <asp:Button ID="btnQuery" runat="server" Text="查詢" CssClass="btn btn-secondary" OnClick="btnQuery_Click" />
                            <button type="button" class="btn btn-secondary" onclick="showAdd();">新增</button>
                        </div>
                    </div>

                </th>
            </tr>--%>
			<tr>
				<td style="height: 450px; vertical-align: top;">
					<font color="red">點擊儲存格進行編輯修改</font>
					<asp:Button ID="btnQuery" runat="server" Text="學習" CssClass="btn btn-secondary" OnClick="btnLearn_Click" OnClientClick="return confirm('是否要學習 ?');" />
					<div id="divHeader" runat="server"></div>

				</td>
			</tr>
		</table>
	</div>
	<div class="modal fade" id="addModal" tabindex="-1" role="dialog" aria-labelledby="exampleModalLabel" aria-hidden="true">
		<div class="modal-dialog modal-lg" role="document">
			<div class="modal-content">
				<div class="modal-header">
					<h5 class="modal-title" id="addModalTitle"></h5>
					<button type="button" class="close" data-dismiss="modal" aria-label="Close">
						<span aria-hidden="true">&times;</span>
					</button>
				</div>
				<div class="modal-body">
					<div style="overflow-y: scroll; height: 600px">
						<table class="table">
							<tr>
								<td align="left">原文件
								</td>
								<td align="left">
									<textarea id="orgText" class="form-control" disabled></textarea>
								</td>
							</tr>
							<tr class="rowCompareSupplierArticleHide" style="display: none">
								<td align="left">比對
								</td>
								<td align="left">
									<%--  <select id="dlPARTS_TYPE" onchange="dlPARTS_TYPE_change()"></select>
                                <select id="dlPARTS_CODE" onchange="dlPARTS_CODE_change()"></select>
                                <select id="dlPARTS_DESC" onchange="dlPARTS_DESC_change()"></select>
                                <select id="dlMAT_ID"></select>--%>
									<%--<asp:HiddenField ID="hidPARTS_TYPE" runat="server" />
								<asp:HiddenField ID="hidPARTS_CODE" runat="server" />
								<asp:HiddenField ID="hidPARTS_DESC" runat="server" />
								<asp:HiddenField ID="hidMAT_ID" runat="server" />

								關鍵字:<input type="text" id="txtSearchMAT_NO"><input type="button" value="查詢" onclick="onSearchMAT_NO()" />
								只撈ERP料號:<input type='checkbox' name='rbERP_MAT' id="SearchOnlyERPMat" value="Y" />
								<div style="height: 200px; width: 650px; overflow: scroll">
									<table class="table">
										<tr>
											<th></th>
											<th>MAT_NO	
											</th>
											<th>MAT_NAME
											</th>
											<th>ERP_EXIST
											</th>
										</tr>
										<tbody id="tblMAT_NO"></tbody>
									</table>
								</div>--%>
								</td>
							</tr>
							<tr id="rowCompareColor">
								<td align="left">比對
								</td>
								<td align="left">關鍵字:<input type="text" id="txtSearchCOLOR_DESC"><input type="button" value="查詢" onclick="onSearchCOLOR_DESC()" />
									<div style="height: 200px; width: 650px; overflow: scroll">
										<table class="table">
											<tr>
												<th></th>
												<th>COLOR_DESC	
												</th>
												<th>COLOR_DESC_CHN
												</th>
											</tr>
											<tbody id="tblCOLOR_DESC"></tbody>
										</table>
									</div>
								</td>
							</tr>
							<tr>
								<td align="left">更改
								</td>
								<td align="left">
									<textarea id="editText" class="form-control"></textarea>
								</td>
							</tr>
							<tr>
								<td align="left">中文備註
								</td>
								<td align="left">
									<textarea id="noteText" class="form-control"></textarea>
								</td>
							</tr>
							<tr class="rowCompareSupplierArticle">
								<td align="left" colspan="2">
									<table class="table">
										<tr>
											<th></th>
											<th>原文件</th>
											<th>更改</th>
										</tr>
										<tbody id="tblUATagData">
											<%--<tr>
											<td>描述</td>
											<td><span id="span_W1"></span></td>
											<td>
												<input type="text" id="EW1" value="" style="width: 300px"></td>
										</tr>
										<tr>
											<td>物料描述</td>
											<td><span id="span_W2"></span></td>
											<td>
												<input type="text" id="EW2" value="" style="width: 300px"></td>
										</tr>
										<tr>
											<td>廠商</td>
											<td><span id="span_W3"></span></td>
											<td>
												<input type="text" id="EW3" value="" style="width: 300px"></td>
										</tr>
										<tr>
											<td>廠商料號</td>
											<td><span id="span_W4"></span></td>
											<td>
												<input type="text" id="EW4" value="" style="width: 300px">
												<input type="button" value="挑選" onclick="showSelectMatNo();return false;" />
											</td>
										</tr>
										<tr>
											<td>物料狀態</td>
											<td><span id="span_W5"></span></td>
											<td>
												<input type="text" id="EW5" value="" style="width: 300px"></td>
										</tr>
										<tr>
											<td>成份</td>
											<td><span id="span_W6"></span></td>
											<td>
												<input type="text" id="EW6" value="" style="width: 300px"></td>
										</tr>
										<tr class="rowNotFabric">
											<td>規格</td>
											<td><span id="span_W7"></span></td>
											<td>
												<input type="text" id="EW7" value="" style="width: 300px"></td>
										</tr>
										<tr class="rowNotFabric">
											<td>單位</td>
											<td><span id="span_W8"></span></td>
											<td>
												<input type="text" id="EW8" value="" style="width: 300px"></td>
										</tr>--%>
										</tbody>
									</table>
								</td>
							</tr>
						</table>
					</div>
				</div>
				<div class="modal-footer">
					<table class="table">
						<tr>
							<td align="center">
								<input type="button" value="取消" class="btn btn-danger" data-dismiss="modal" />
								<input type="button" value="儲存" class="btn btn-success" onclick="onSave()" />
							</td>
						</tr>
					</table>

				</div>
			</div>
		</div>
	</div>
	<div class="modal fade" id="selectMatNoModal" tabindex="-1" role="dialog" aria-labelledby="exampleModalLabel" aria-hidden="true">
		<div class="modal-dialog modal-lg" role="document">
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
							<td align="left">比對
							</td>
							<td align="left">
								<%--  <select id="dlPARTS_TYPE" onchange="dlPARTS_TYPE_change()"></select>
                                <select id="dlPARTS_CODE" onchange="dlPARTS_CODE_change()"></select>
                                <select id="dlPARTS_DESC" onchange="dlPARTS_DESC_change()"></select>
                                <select id="dlMAT_ID"></select>--%>
								<asp:HiddenField ID="HiddenField1" runat="server" />
								<asp:HiddenField ID="HiddenField2" runat="server" />
								<asp:HiddenField ID="HiddenField3" runat="server" />
								<asp:HiddenField ID="HiddenField4" runat="server" />

								關鍵字:<input type="text" id="txtSearchMAT_NO"><input type="button" value="查詢" onclick="onSearchMAT_NO()" />
								只撈ERP料號:<input type='checkbox' name='rbERP_MAT' id="SearchOnlyERPMat" value="Y" />
								<div style="height: 200px; width: 650px; overflow: scroll">
									<table class="table">
										<tr>
											<th></th>
											<th>MAT_NO	
											</th>
											<th>MAT_NAME
											</th>
											<th>ERP_EXIST
											</th>
										</tr>
										<tbody id="tblMAT_NO"></tbody>
									</table>
								</div>
							</td>
						</tr>
						<tr>
							<td align="center" colspan="2">
								<input type="hidden" id="hidUaTagDataItem" />
								<input type="button" value="取消" class="btn btn-danger" data-dismiss="modal" />
								<input type="button" value="確定" class="btn btn-success" onclick="onSelectMatNo();return false;" />
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
		var objTd;
		var saveUrl = "";
		var orgUrl = "";
		var saveId = "";
		var saveCol = "";
		var saveColorCol = "";
		var isSaveRecord = false;
		var isSaveUATagData = false;
		var bomType = "";

		var PARTS_TYPE = "";
		var PARTS_CODE = "";
		var PARTS_DESC = "";
		var MAT_ID = "";


		function editHeader(td) {
			let text = $(td).find('span:first').text();
			//$('#editText').val(text);

			$('.rowCompareSupplierArticle').hide();
			$('#rowCompareColor').hide();
			$('#editText').prop('disabled', false);

			objTd = $(td);
			saveUrl = "UA_Header.ashx?fun=saveByCol"
			saveId = $(td).attr('data-luhid');
			saveCol = $(td).attr('data-col');
			isSaveRecord = false;
			isSaveUATagData = false;

			orgUrl = "UA_Header.ashx?fun=get_org"
			orgId = $(td).attr('data-org_luhid');

			getOrgTest();
			getChNote("luhid");
			$('#addModalTitle').text('編輯');
			$('#addModal').modal('show')
		}

		function editBom(td) {
			let text = $(td).find('span:first').text();
			//$('#editText').val(text);

			bomType = $(td).attr('data-type');

			objTd = $(td);
			saveUrl = "UA_BOM.ashx?fun=saveByCol"
			saveId = $(td).attr('data-lubid');
			saveCol = $(td).attr('data-col');
			saveColorCol = "";
			isSaveRecord = true;
			isSaveUATagData = false;
			isFabric = false;

			$('#rowCompareColor').hide();

			if (saveCol == "SupplierArticle") {

				PARTS_TYPE = $(td).attr('data-PARTS_TYPE');
				PARTS_CODE = $(td).attr('data-PARTS_CODE');
				PARTS_DESC = $(td).attr('data-PARTS_DESC');
				MAT_ID = $(td).attr('data-MAT_ID');
				isSaveRecord = false;
				$('.rowCompareSupplierArticle').show();
				$('#editText').prop('disabled', true);

				isSaveUATagData = true;
				getUADataTagTable(saveId);



			} else {

				PARTS_TYPE = "";
				PARTS_CODE = "";
				PARTS_DESC = "";
				MAT_ID = "";

				$('.rowCompareSupplierArticle').hide();
				$('#editText').prop('disabled', false);
			}

			orgUrl = "UA_BOM.ashx?fun=get_org"
			orgId = $(td).attr('data-org_lubid');
			getOrgTest();
			getChNote("lubid");

			if (saveCol == 'B1' || saveCol == 'B2' || saveCol == 'B3' || saveCol == 'B4' || saveCol == 'B5' || saveCol == 'B6' || saveCol == 'B7' || saveCol == 'B8' || saveCol == 'B9' || saveCol == 'B10') {
				saveColorCol = $(td).attr('data-colheader');
				$('#rowCompareColor').show();
			}


			$('#addModalTitle').text('編輯');
			$('#addModal').modal('show')
		}

		function showSelectMatNo(item) {

			$('#hidUaTagDataItem').val(item);
			$('#selectMatNoModal').modal('show')
		}
		function onSelectMatNo() {
			let MAT_NO = "";
			$('.rbMAT_NO').each(function () {

				if ($(this).prop("checked")) {
					MAT_NO = $(this).attr('data-MAT_NO');

				}
			});
			if (MAT_NO) {
				let item = $('#hidUaTagDataItem').val();
				$('#EW' + item).val(MAT_NO);
			}

			$('#selectMatNoModal').modal('hide');
			return false;
		}

		function getUADataTagTable(lubid) {
			$("#tblUATagData").html('');
			//$('#span_W1').text('');
			//$('#span_W2').text('');
			//$('#span_W3').text('');
			//$('#span_W4').text('');
			//$('#span_W5').text('');
			//$('#span_W6').text('');
			//$('#span_W7').text('');
			//$('#span_W8').text('');

			//$('#EW1').val('');
			//$('#EW2').val('');
			//$('#EW3').val('');
			//$('#EW4').val('');
			//$('#EW5').val('');
			//$('#EW6').val('');
			//$('#EW7').val('');
			//$('#EW8').val('');

			$.ajax({
				type: "POST",
				url: "UA_TagData.ashx?fun=get_by_lubid&lubid=" + lubid,
				dataType: 'json',
				success: function (res) {

					let html = "";

					if (res) {

						let arrFabric = ["描述", "物料描述", "廠商", "廠商料號", "物料狀態", "成份"];
						let arrTrim = ["客戶料號", "物料描述", "補充描述", "(描述)", "廠商", "物料狀態", "規格", "單位"];
						let arrThread = ["客戶料號", "物料描述", "補充描述", "廠商", "", "物料狀態", "規格", "單位"];
						let arrThread2 = ["客戶料號", "物料描述", "補充描述", "(描述)", "廠商","", "物料狀態", "規格", ""];

						let arrHangtag = ["客戶料號", "物料描述", "廠商", "(描述)", "物料狀態", "單位"];
						let arrEmbellishment = ["描述", "物料描述", "廠商", "(描述)", "物料狀態", "規格", "單位"];

						let arrName = [];
						let isFabric = false;
						if (bomType == "Fabric") {
							arrName = arrFabric;
							isFabric = true;
						}
						else if (bomType == "Trim") arrName = arrTrim;
						else if (bomType == "Thread") {
							if (!res["W6"])
								arrName = arrThread2;
							else
								arrName = arrThread;
						}
						else if (bomType.indexOf('Hangtag') > -1 || bomType == "Label") arrName = arrHangtag;
						else if (bomType == "Embellishment") arrName = arrEmbellishment;

						
						for (let i = 1; i <= arrName.length; i++) {
							let isHideRow = false;
							if (bomType == "Thread") {
								//isHideRow = true;
								//res["W6"] = res["W5"];
								//res["W5"] = res["W4"];
								if (!arrName[i - 1])
									isHideRow = true;
							}

							html += "<tr " + (isHideRow?"style='display:none'":"")+">";
							html += "<td>" + arrName[i - 1] + "</td>";
							html += "<td style='width:50%'><div><span id='span_W" + i + "' class='span_w'>" + res["W" + i] + "</span></div></td>";
							html += "<td style='width:50%'><input type='text' id='EW" + i + "' value='' style='width:100%'>";

							if ((!isFabric && i == 1) || (isFabric && i == 4)) {
								html += "<input type='button' value='挑選' onclick='showSelectMatNo(" + i + "); return false;' />";
							}
							html += "</td>";
							html += "</tr>";

							
						}

						$("#tblUATagData").html(html);

						let arrVal = [];
						for (let i = 1; i <= arrName.length; i++) {

							$('#EW' + i).val((res["EW" + i] ?? res["W" + i]));

							arrVal.push((res["EW" + i] ?? res["W" + i]));
						}

						$('#editText').val(arrVal.join(' / '));

						//$('#EW1').val((res.EW1 ?? res.W1));
						//$('#EW2').val((res.EW2 ?? res.W2));
						//$('#EW3').val((res.EW3 ?? res.W3));
						//$('#EW4').val((res.EW4 ?? res.W4));
						//$('#EW5').val((res.EW5 ?? res.W5));
						//$('#EW6').val((res.EW6 ?? res.W6));
						//$('#EW7').val((res.EW7 ?? res.W7));
						//$('#EW8').val((res.EW8 ?? res.W8));



					}



				},
				complete: function () {

				},
				error: function (error) {
					alert("getUADataTagTable 失敗");
				}
			});
		}

		function onSaveUATagData(lubid) {
			let EW1 = $('#EW1').val();
			let EW2 = $('#EW2').val();
			let EW3 = $('#EW3').val();
			let EW4 = $('#EW4').val();
			let EW5 = $('#EW5').val();
			let EW6 = $('#EW6').val();
			let EW7 = $('#EW7').val();
			let EW8 = $('#EW8').val();

			let data = {
				lubid: lubid,
				bomType: bomType,
				EW1: EW1,
				EW2: EW2,
				EW3: EW3,
				EW4: EW4,
				EW5: EW5,
				EW6: EW6,
				EW7: EW7,
				EW8: EW8
			};
			$.ajax({
				type: "POST",
				url: "UA_TagData.ashx?fun=edit",
				data: data,
				async: false,
				dataType: 'json',
				success: function (res) {

					console.log("[onSaveUATagData]");
				},
				complete: function () {

				},
				error: function (error) {
					alert("儲存失敗-UATagData");
				}
			});

		}

		function editSizeTable(td) {
			let text = $(td).find('span:first').text();
			//$('#editText').val(text);

			$('.rowCompareSupplierArticle').hide();
			$('#rowCompareColor').hide();
			$('#editText').prop('disabled', false);

			objTd = $(td);
			saveUrl = "UA_SizeTable.ashx?fun=saveByCol"
			saveId = $(td).attr('data-lustid');
			saveCol = $(td).attr('data-col');
			isSaveRecord = false;
			isSaveUATagData = false;

			orgUrl = "UA_SizeTable.ashx?fun=get_org"
			orgId = $(td).attr('data-org_lustid');

			getOrgTest();
			getChNote("lustid");
			$('#addModalTitle').text('編輯');
			$('#addModal').modal('show')
		}

		function getOrgTest() {

			$('#orgText').val('');

			let data = {
				id: orgId,
				newid: saveId,
				col: saveCol,
			};

			console.log(data);

			$.ajax({
				type: "POST",
				url: orgUrl,
				data: data,
				dataType: 'json',
				success: function (res) {

					if (res) {

						$('#orgText').val(res.orgText);
						

						if (saveCol == "SupplierArticle") {

							getPARTS_TYPE(res.orgText);
						} else {
							$('#editText').val(res.newText);
						}
					}

				},
				complete: function () {

				},
				error: function (error) {
					alert("失敗");
				}
			});
		}

		function getChNote(idName) {

			$('#noteText').val('');

			let data = {
				idName: idName,
				id: saveId,
				ColName: saveCol,
			};

			console.log(data);

			$.ajax({
				type: "POST",
				url: "UA_Ch_Note.ashx?fun=get",
				data: data,
				dataType: 'json',
				success: function (res) {

					if (res) {

						$('#noteText').val(res.Note);
					}

				},
				complete: function () {

				},
				error: function (error) {
					alert("失敗");
				}
			});
		}



		function onSave() {
			let orgtext = $('#orgText').val();
			let text = $('#editText').val();
			let note = $('#noteText').val();

			//UA_BOM
			let PARTS_TYPE = $('#dlPARTS_TYPE').val();
			let PARTS_CODE = $('#dlPARTS_CODE').val();
			let PARTS_DESC = $('#dlPARTS_DESC').val();
			let MAT_ID = $('#dlMAT_ID').val();

			let isRecord = 0;

			if (saveCol == 'Usage' || saveCol == 'SupplierArticle' || saveCol == 'B1' || saveCol == 'B2' || saveCol == 'B3' || saveCol == 'B4' || saveCol == 'B5' || saveCol == 'B6' || saveCol == 'B7' || saveCol == 'B8' || saveCol == 'B9' || saveCol == 'B10') {
				//if (isSaveRecord) {

				if (confirm('改的內容是否紀錄至詞庫')) {
					isRecord = 1;
				}
			}

			let data = {
				orgId: orgId,
				id: saveId,
				col: saveCol,
				colorCol: saveColorCol,
				text: text,
				chNote: note,

				PARTS_TYPE: PARTS_TYPE,
				PARTS_CODE: PARTS_CODE,
				PARTS_DESC: PARTS_DESC,
				MAT_ID: MAT_ID,
				isRecord: isRecord

			};



			if (isSaveUATagData) {
				onSaveUATagData(saveId);

				let W1 = $('#span_W1').text();
				let W2 = $('#span_W2').text();
				let W3 = $('#span_W3').text();
				let W4 = $('#span_W4').text();
				let W5 = $('#span_W5').text();
				let W6 = $('#span_W6').text();
				let W7 = $('#span_W7').text();
				let W8 = $('#span_W8').text();

				let EW1 = $('#EW1').val();
				let EW2 = $('#EW2').val();
				let EW3 = $('#EW3').val();
				let EW4 = $('#EW4').val();
				let EW5 = $('#EW5').val();
				let EW6 = $('#EW6').val();
				let EW7 = $('#EW7').val();
				let EW8 = $('#EW8').val();


				if (W1 != EW1 || W2 != EW2 || W3 != EW3 || W4 != EW4 || W5 != EW5 || W6 != EW6 || W7 != EW7 || W8 != EW8) {
					//有修改
					var arrTagData = [];
					if (W1)
						arrTagData.push(EW1);
					if (W2)
						arrTagData.push(EW2);
					if (W3)
						arrTagData.push(EW3);
					if (W4)
						arrTagData.push(EW4);
					if (W5)
						arrTagData.push(EW5);
					if (W6)
						arrTagData.push(EW6);
					if (W7)
						arrTagData.push(EW7);
					if (W8)
						arrTagData.push(EW8);


					text = arrTagData.join(' / ');
				}

				data["EW1"] = EW1;
				data["EW2"] = EW2;
				data["EW3"] = EW3;
				data["EW4"] = EW4;
				data["EW5"] = EW5;
				data["EW6"] = EW6;
				if (W7)
					data["EW7"] = EW7;
				if (W8)
					data["EW8"] = EW8;
			}



			console.log(data);

			$.ajax({
				type: "POST",
				url: saveUrl,
				data: data,
				dataType: 'json',
				success: function (res) {

					if (isRecord == 1 && res.learnmgrItem != 1) {
						alert('內容已存在於詞庫');
					}

					if (res.data == 1) {

						if (saveCol == "SupplierArticle") {

							window.location.reload();
						}

						//objTd.text(text);

						let html = "<font>原:" + orgtext + "</font><br><font color='red'>修:" + text + "</font><br><font color='blue'>中:" + note + "</font>"

						objTd.html(html);
						$('#addModal').modal('hide');
					}

				},
				complete: function () {

				},
				error: function (error) {
					alert("儲存失敗");
				}
			});
		}

		function deleteHeader(luhid, iSeq) {

			if (confirm('確定刪除?')) {
				let data = {
					luhid: luhid,

				};

				console.log(data);

				$.ajax({
					type: "POST",
					url: "UA_Header.ashx?fun=delete",
					data: data,
					dataType: 'json',
					success: function (res) {

						if (res == 1) {

							$('#row' + iSeq).hide();
						}

					},
					complete: function () {

					},
					error: function (error) {
						alert("刪除失敗");
					}
				});
			}

		}

		function deleteBom(lubid, iSeq) {

			if (confirm('確定刪除?')) {
				let data = {
					lubid: lubid,

				};

				console.log(data);

				$.ajax({
					type: "POST",
					url: "UA_BOM.ashx?fun=delete",
					data: data,
					dataType: 'json',
					success: function (res) {

						//if (res == 1) {

							//$('#row' + iSeq).hide();
							window.location.reload();
						//}

					},
					complete: function () {

					},
					error: function (error) {
						alert("刪除失敗");
					}
				});
			}

		}

		function deleteSizeTable(lustid, iSeq) {

			if (confirm('確定刪除?')) {
				let data = {
					lustid: lustid,

				};

				console.log(data);

				$.ajax({
					type: "POST",
					url: "UA_SizeTable.ashx?fun=delete",
					data: data,
					dataType: 'json',
					success: function (res) {

						if (res == 1) {

							$('#row' + iSeq).hide();
						}

					},
					complete: function () {

					},
					error: function (error) {
						alert("刪除失敗");
					}
				});
			}



		}

		function copyHeader(luhid) {

			let data = {
				luhid: luhid,

			};

			console.log(data);

			$.ajax({
				type: "POST",
				url: "UA_Header.ashx?fun=copy",
				data: data,
				dataType: 'json',
				success: function (res) {

					if (res == 1) {

						//$('#row' + iSeq).hide();
						window.location.reload();
					}

				},
				complete: function () {

				},
				error: function (error) {
					alert("新增失敗");
				}
			});
		}

		function copyBom(lubid) {

			let data = {
				lubid: lubid,

			};

			console.log(data);

			$.ajax({
				type: "POST",
				url: "UA_BOM.ashx?fun=copy",
				data: data,
				dataType: 'json',
				success: function (res) {

					//if (res == 1) {

						//$('#row' + iSeq).hide();
						window.location.reload();
					//}

				},
				complete: function () {

				},
				error: function (error) {
					alert("新增失敗");
				}
			});
		}

		function copySizeTable(lustid) {

			let data = {
				lustid: lustid,

			};

			console.log(data);

			$.ajax({
				type: "POST",
				url: "UA_SizeTable.ashx?fun=copy",
				data: data,
				dataType: 'json',
				success: function (res) {

					if (res == 1) {

						//$('#row' + iSeq).hide();
						window.location.reload();
					}

				},
				complete: function () {

				},
				error: function (error) {
					alert("新增失敗");
				}
			});
		}

		function dlPARTS_TYPE_change() {

			$("#dlPARTS_CODE").html('');
			$("#dlPARTS_DESC").html('');
			$("#dlMAT_ID").html('');


			let PARTS_TYPE = $('#dlPARTS_TYPE').val();

			getPARTS_CODE(PARTS_TYPE);
		}

		function dlPARTS_CODE_change() {

			$("#dlPARTS_DESC").html('');
			$("#dlMAT_ID").html('');


			let PARTS_TYPE = $('#dlPARTS_TYPE').val();
			let PARTS_CODE = $('#dlPARTS_CODE').val();

			getPARTS_DESC(PARTS_TYPE, PARTS_CODE);
		}

		function dlPARTS_DESC_change() {



			let PARTS_TYPE = $('#dlPARTS_TYPE').val();
			let PARTS_CODE = $('#dlPARTS_CODE').val();
			let PARTS_DESC = $('#dlPARTS_DESC').val();

			getMAT_ID(MAT_NO, PARTS_TYPE, PARTS_CODE, PARTS_DESC);
		}

		function getPARTS_TYPE(MAT_NO) {



			let data = {
				MAT_NO: MAT_NO,
			};

			console.log(data);

			$("#dlPARTS_TYPE").html('');

			$.ajax({
				type: "POST",
				url: "SAM_MO_D.ashx?fun=getPARTS_TYPE",
				data: data,
				dataType: 'json',
				success: function (res) {

					let html = "<option value=''>請選擇</option>";
					for (let i in res) {
						html += "<option value='" + res[i].PARTS_TYPE + "'>" + res[i].PARTS_TYPE + "</option>";
					}

					$("#dlPARTS_TYPE").html(html);

					if (PARTS_TYPE) {
						$("#dlPARTS_TYPE").val(PARTS_TYPE);
						getPARTS_CODE(PARTS_TYPE);
					}
				},
				complete: function () {

				},
				error: function (error) {
					alert("失敗");
				}
			});
		}

		function getPARTS_CODE(PARTS_TYPE) {


			let MAT_NO = $('#orgText').val();

			let data = {
				MAT_NO: MAT_NO,
				PARTS_TYPE: PARTS_TYPE,
			};

			console.log(data);

			$("#dlPARTS_CODE").html('');

			$.ajax({
				type: "POST",
				url: "SAM_MO_D.ashx?fun=getPARTS_CODE",
				data: data,
				dataType: 'json',
				success: function (res) {

					let html = "<option value=''>請選擇</option>";
					for (let i in res) {
						html += "<option value='" + res[i].PARTS_CODE + "'>" + res[i].PARTS_CODE + "</option>";
					}

					$("#dlPARTS_CODE").html(html);

					if (PARTS_CODE) {
						$("#dlPARTS_CODE").val(PARTS_CODE);
						getPARTS_DESC(PARTS_TYPE, PARTS_CODE);
					}

				},
				complete: function () {

				},
				error: function (error) {
					alert("失敗");
				}
			});
		}

		function getPARTS_DESC(PARTS_TYPE, PARTS_CODE) {

			let MAT_NO = $('#orgText').val();

			let data = {
				MAT_NO: MAT_NO,
				PARTS_TYPE: PARTS_TYPE,
				PARTS_CODE: PARTS_CODE
			};

			console.log(data);

			$("#dlPARTS_DESC").html('');

			$.ajax({
				type: "POST",
				url: "SAM_MO_D.ashx?fun=getPARTS_DESC",
				data: data,
				dataType: 'json',
				success: function (res) {

					let html = "<option value=''>請選擇</option>";
					for (let i in res) {
						html += "<option value='" + res[i].PARTS_DESC + "'>" + res[i].PARTS_DESC + "</option>";
					}

					$("#dlPARTS_DESC").html(html);

					if (PARTS_DESC) {
						$("#dlPARTS_DESC").val(PARTS_DESC);
						getMAT_ID(PARTS_TYPE, PARTS_CODE, PARTS_DESC);
					}
				},
				complete: function () {

				},
				error: function (error) {
					alert("失敗");
				}
			});
		}

		function getMAT_ID(PARTS_TYPE, PARTS_CODE, PARTS_DESC) {
			let MAT_NO = $('#orgText').val();

			let data = {
				MAT_NO: MAT_NO,
				PARTS_TYPE: PARTS_TYPE,
				PARTS_CODE: PARTS_CODE,
				PARTS_DESC: PARTS_DESC
			};

			console.log(data);

			$("#dlMAT_ID").html('');

			$.ajax({
				type: "POST",
				url: "SAM_MO_D.ashx?fun=getMAT_ID",
				data: data,
				dataType: 'json',
				success: function (res) {

					let html = "<option value=''>請選擇</option>";
					for (let i in res) {
						html += "<option value='" + res[i].MAT_ID + "'>" + res[i].MAT_ID + "</option>";
					}

					$("#dlMAT_ID").html(html);

					if (MAT_ID) {
						$("#dlMAT_ID").val(MAT_ID);
					}
				},
				complete: function () {

				},
				error: function (error) {
					alert("失敗");
				}
			});
		}


		function onSearchMAT_NO() {

			let MAT_NO = $('#txtSearchMAT_NO').val();
			let searchonlyerp = "N";
			if ($("input[name=rbERP_MAT]:checked").length > 0) searchonlyerp = "Y";

			let data = {
				MAT_NO: MAT_NO,
				searchonlyerp: searchonlyerp,
			};


			$("#tblMAT_NO").html('');

			$.ajax({
				type: "POST",
				url: "SAM_MAT_DEF.ashx?fun=SearchMAT_NO",
				data: data,
				dataType: 'json',
				success: function (res) {

					let html = "";
					for (let i in res) {
						html += "<tr>";
						//html += "<td><input type='radio' class='rbMAT_NO' name='rbMAT_NO' data-MAT_NO='" + res[i].MAT_NO + "' onchange='onRbMAT_NO_change(this)' /></td>";
						html += "<td><input type='radio' class='rbMAT_NO' name='rbMAT_NO' data-MAT_NO='" + res[i].MAT_NO + "'  /></td>";
						html += "<td>" + res[i].MAT_NO + "</td>";
						html += "<td>" + res[i].MAT_NAME + "</td>";
						html += "<td>" + res[i].ERP_EXIST + "</td>";
						html += "</tr>";
					}
					$('#tblMAT_NO').html(html);

					if (res.length == 0) {
						alert('無對應資料。');
						$('#editText').val('');
					}
				},
				complete: function () {

				},
				error: function (error) {
					alert("失敗");
				}
			});
		}

		function onRbMAT_NO_change(obj) {
			let MAT_NO = $(obj).attr('data-MAT_NO');

			$('#editText').val(MAT_NO);
		}


		function onSearchCOLOR_DESC() {

			let COLOR_DESC = $('#txtSearchCOLOR_DESC').val();

			let data = {
				COLOR_DESC: COLOR_DESC,
			};


			$("#tblCOLOR_DESC").html('');

			$.ajax({
				type: "POST",
				url: "SAM_MAT_COLOR_DEF.ashx?fun=SearchCOLOR_DESC",
				data: data,
				dataType: 'json',
				success: function (res) {

					let html = "";
					for (let i in res) {
						html += "<tr>";
						html += "<td><input type='radio' name='rbCOLOR_DESC' data-COLOR_DESC='" + res[i].COLOR_DESC + "' onchange='onRbCOLOR_DESC_change(this)' /></td>";
						html += "<td>" + res[i].COLOR_DESC + "</td>";
						html += "<td>" + res[i].COLOR_DESC_CHN + "</td>";
						html += "</tr>";
					}
					$('#tblCOLOR_DESC').html(html);
				},
				complete: function () {

				},
				error: function (error) {
					alert("失敗");
				}
			});
		}

		function onRbCOLOR_DESC_change(obj) {
			let COLOR_DESC = $(obj).attr('data-COLOR_DESC');

			$('#editText').val(COLOR_DESC);
		}

		function Open(filePath) {
			window.open(filePath);

		}





	</script>
</asp:Content>
