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
                    <font color="red">點擊儲存格進行編輯修改</font>  <asp:Button ID="btnQuery" runat="server" Text="學習" CssClass="btn btn-secondary" OnClick="btnLearn_Click" OnClientClick="return confirm('是否要學習 ?');" />
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
                    <table class="table">
                        <tr>
                            <td align="left">原文件
                            </td>
                            <td align="left">
                                <textarea id="orgText" class="form-control" disabled></textarea>
                            </td>
                        </tr>
                        <tr id="rowCompareSupplierArticle">
                            <td align="left">比對
                            </td>
                            <td align="left">
                              <%--  <select id="dlPARTS_TYPE" onchange="dlPARTS_TYPE_change()"></select>
                                <select id="dlPARTS_CODE" onchange="dlPARTS_CODE_change()"></select>
                                <select id="dlPARTS_DESC" onchange="dlPARTS_DESC_change()"></select>
                                <select id="dlMAT_ID"></select>--%>
                                <asp:HiddenField ID="hidPARTS_TYPE" runat="server" />
                                <asp:HiddenField ID="hidPARTS_CODE" runat="server" />
                                <asp:HiddenField ID="hidPARTS_DESC" runat="server" />
                                <asp:HiddenField ID="hidMAT_ID" runat="server" />

                                 關鍵字:<input type="text" id="txtSearchMAT_NO"><input type="button" value="查詢" onclick="onSearchMAT_NO()" /> 
                                <div style="height:200px;width:650px; overflow:scroll">
                                    <table class="table">
                                        <tr>
                                             <th>
                                                	
                                            </th>
                                            <th>
                                                MAT_NO	
                                            </th>
                                             <th>
                                                MAT_NAME
                                            </th>
                                        </tr>
                                        <tbody id="tblMAT_NO"></tbody>
                                    </table>
                                </div>
                            </td>
                        </tr>
                         <tr id="rowCompareColor">
                            <td align="left">比對
                            </td>
                            <td align="left">
                                 關鍵字:<input type="text" id="txtSearchCOLOR_DESC"><input type="button" value="查詢" onclick="onSearchCOLOR_DESC()" /> 
                                <div style="height:200px;width:650px; overflow:scroll">
                                    <table class="table">
                                        <tr>
                                             <th>
                                                	
                                            </th>
                                            <th>
                                                COLOR_DESC	
                                            </th>
                                             <th>
                                                COLOR_DESC_CHN
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
                        <tr>
                            <td align="center" colspan="2">
                                <input type="button" value="取消" class="btn btn-danger" data-dismiss="modal" />
                                <input type="button" value="儲存" class="btn btn-success" onclick="onSave()" />
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

        var PARTS_TYPE = "";
        var PARTS_CODE = "";
        var PARTS_DESC = "";
        var MAT_ID = "";


        function editHeader(td) {
            let text = $(td).find('span:first').text();
            //$('#editText').val(text);

            $('#rowCompareSupplierArticle').hide();
            $('#rowCompareColor').hide();

            objTd = $(td);
            saveUrl = "UA_Header.ashx?fun=saveByCol"
            saveId = $(td).attr('data-luhid');
            saveCol = $(td).attr('data-col');
            isSaveRecord = false;


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

            objTd = $(td);
            saveUrl = "UA_BOM.ashx?fun=saveByCol"
            saveId = $(td).attr('data-lubid');
            saveCol = $(td).attr('data-col');
            saveColorCol = "";
            isSaveRecord = true;

            $('#rowCompareColor').hide();

            if (saveCol == "SupplierArticle") {

                PARTS_TYPE = $(td).attr('data-PARTS_TYPE');
                PARTS_CODE = $(td).attr('data-PARTS_CODE');
                PARTS_DESC = $(td).attr('data-PARTS_DESC');
                MAT_ID = $(td).attr('data-MAT_ID');
                isSaveRecord = false;
                $('#rowCompareSupplierArticle').show();
            } else {

                PARTS_TYPE = "";
                PARTS_CODE = "";
                PARTS_DESC = "";
                MAT_ID = "";

                $('#rowCompareSupplierArticle').hide();
                
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

        function editSizeTable(td) {
            let text = $(td).find('span:first').text();
            //$('#editText').val(text);

            $('#rowCompareSupplierArticle').hide();
            $('#rowCompareColor').hide();

            objTd = $(td);
            saveUrl = "UA_SizeTable.ashx?fun=saveByCol"
            saveId = $(td).attr('data-lustid');
            saveCol = $(td).attr('data-col');
            isSaveRecord = false;

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
                        $('#editText').val(res.newText);

                        if (saveCol == "SupplierArticle") {

                            getPARTS_TYPE(res.orgText);
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

            if (saveCol == 'StandardPlacement' || saveCol == 'Placement' || saveCol == 'SupplierArticle' || saveCol == 'Supplier' || saveCol == 'Name' || saveCol == 'Criticality') {
            //if (isSaveRecord) {

                if (confirm('改的內容是否紀錄至詞庫')) {
                    isRecord = 1;
                }
            }

            let data = {
                orgId: orgId,
                id: saveId,
                col: saveCol,
                colorCol:saveColorCol,
                text: text,
                chNote: note,

                PARTS_TYPE: PARTS_TYPE,
                PARTS_CODE: PARTS_CODE,
                PARTS_DESC: PARTS_DESC,
                MAT_ID: MAT_ID,
                isRecord: isRecord
            };

            console.log(data);

            $.ajax({
                type: "POST",
                url: saveUrl,
                data: data,
                dataType: 'json',
                success: function (res) {

                    if (isRecord==1 && res.learnmgrItem != 1) {
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

                        if (res == 1) {

                            //$('#row' + iSeq).hide();
                            window.location.reload();
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

            let data = {
                MAT_NO: MAT_NO,
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
                        html += "<td><input type='radio' name='rbMAT_NO' data-MAT_NO='" + res[i].MAT_NO + "' onchange='onRbMAT_NO_change(this)' /></td>";
                        html += "<td>" + res[i].MAT_NO + "</td>";
                        html += "<td>" + res[i].MAT_NAME + "</td>";
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
