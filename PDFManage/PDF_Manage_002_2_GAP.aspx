<%@ Page Title="" Language="C#" MasterPageFile="~/master/MasterPage_Login.master"
    AutoEventWireup="true" CodeFile="PDF_Manage_002_2_GAP.aspx.cs" Inherits="Passport_Passport_A000" %>

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
            background-color: #b7d2ff;
        }
        /* Black */

        .rowCompare {
            background-color: #CCEEFF;
        }
    </style>
    <div class="">
        <table class="table table-sm">
            <thead>
                <tr>
                    <th scope="col" colspan="5">
                        <a href="PDF_Manage_002.aspx">歷程管理</a> >>
                        <asp:Label ID="lblInfo" runat="server"></asp:Label>:比對版本
                        <asp:HiddenField ID="hidpipid" runat="server" />
                        <asp:HiddenField ID="hidorg_pipid" runat="server" />
                    </th>
                </tr>
            </thead>
            <tr>
                <th scope="col" colspan="5">
                    <div class="row m-0">
                        <div class="col-0">
                            <label>
                                文件來源：
                            </label>
                        </div>
                        <div class="col">
                            <%--<div id="divOrgFile" runat="server"></div>--%>
                            <asp:DropDownList ID="dlSourceFile" runat="server" CssClass="form-control" onchange="dlVerFile_change()">
                            </asp:DropDownList>
                        </div>
                    </div>
                    <div class="row m-0">
                        <div class="col-0">
                            <label>
                                比對目的：
                            </label>
                        </div>
                        <div class="col">
                            <asp:DropDownList ID="dlVerFile" runat="server" CssClass="form-control" onchange="dlVerFile_change()">
                                <asp:ListItem Value="" Text="原文件"></asp:ListItem>
                            </asp:DropDownList>
                        </div>
                    </div>
                    <div class="row m-0">
                        <%--        <div class="col-0">
                            <label>
                                比對欄位：
                            </label>
                        </div>
                        <div class="col-4 col-md-3">
                            <asp:DropDownList ID="dlType" runat="server" CssClass="form-control">
                                <asp:ListItem Value="" Text="請選擇"></asp:ListItem>
                                <asp:ListItem Value="1" Text="更改"></asp:ListItem>
                                <asp:ListItem Value="2" Text="中文備註"></asp:ListItem>
                            </asp:DropDownList>
                        </div>--%>
                        <div class="col">
                            <%-- <asp:Button ID="Button1" runat="server" Text="執行" CssClass="btn btn-warning" OnClick="btnQuery_Click" OnClientClick="return onQuery()" />--%>
                            <input type="button" value="執行" class="btn btn-warning" onclick="onQuery()" />
                        </div>
                    </div>
                </th>
            </tr>
            <tr>
                <td style="height: 450px; vertical-align: top;">
                    <font color="red">紅色部分為差異(比對來源)</font><br>
                     1.來源文件 與 比對目的 都有的欄位 ，欄位底色顯示 <font color="#00FFFF">水藍色</font><br>
                     2.來源文件 有的欄位，但 比對目的 沒有的欄位，欄位底色顯示 <font color="#FF9224">橘色</font><br>
                    <br>
                    <input type="button" value="比對結果" onclick="onShow(1)" id="btnSwitch1" class="btnActive"/>
                    <input type="button" value="目的文件沒被比對到的資料" id="btnSwitch2" onclick="onShow(2)"/>
                    
                    <div id="divHeader" runat="server"></div>
                    <div id="divHeaderNotCompare" runat="server"></div>
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
                            <td align="center">
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
                            <td align="center">
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
    <div class="modal fade" id="addQueryModal" tabindex="-1" role="dialog" aria-labelledby="exampleModalLabel" aria-hidden="true">
        <div class="modal-dialog modal-xl" role="document">
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
                            <td align="left">來源:<span id="span_Source"></span>
                            </td>
                            <td align="left">目的:<span id="span_Compare"></span>
                            </td>
                        </tr>
                        <tr>
                            <td align="left">
                                <div id="div_Source" style="overflow: scroll; width: 530px; height: 600px"></div>
                            </td>
                            <td align="left">
                                <div id="div_Compare" style="overflow: scroll; width: 530px; height: 600px"></div>
                            </td>
                        </tr>
                        <tr>
                            <td align="center" colspan="2">
                                <asp:HiddenField ID="hid_Source_lusthid" runat="server" />
                                <asp:HiddenField ID="hid_Compare_lusthid" runat="server" />
                                <input type="button" value="取消" class="btn btn-danger" data-dismiss="modal" />
                                <asp:Button ID="Button2" runat="server" Text="比對" CssClass="btn btn-warning" OnClick="btnQuery_Click" OnClientClick="return onQuery2()" />
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

            dlVerFile_change();
        });


        function dlVerFile_change() {
            <%--let VerFile = $('#<%= dlVerFile.ClientID %>').val();
            if (!VerFile) {
                $('#<%= dlType.ClientID %>').val('');
                $('#<%= dlType.ClientID %>').prop('disabled', true)
            } else {
                $('#<%= dlType.ClientID %>').prop('disabled', false)
            }--%>
        }

        function onShow(type) {
            if (type == "1") {
                $('#<%= divHeader.ClientID %>').show();
                $('#<%= divHeaderNotCompare.ClientID %>').hide();

                $('#btnSwitch1').addClass("btnActive");
                $('#btnSwitch2').removeClass("btnActive");
            } else {
                $('#<%= divHeader.ClientID %>').hide();
                $('#<%= divHeaderNotCompare.ClientID %>').show();

                $('#btnSwitch1').removeClass("btnActive");
                $('#btnSwitch2').addClass("btnActive");
            }
        }

        function onQuery() {
            let pipid_source = $('#<%= dlSourceFile.ClientID %>').val();
            let pipid_compare = $('#<%= dlVerFile.ClientID %>').val();

            $('#span_Source').text($("#<%= dlSourceFile.ClientID %> option:selected").text());
            $('#span_Compare').text($("#<%= dlVerFile.ClientID %> option:selected").text());

            $('#div_Source').html('');


            $.ajax({
                type: "POST",
                url: "GAP_SizeTable.ashx?fun=getContent",
                data: {
                    pipid: pipid_source
                },
                dataType: 'json',
                success: function (res) {

                    $('#div_Source').html(res.html);

                },
                complete: function () {

                },
                error: function (error) {
                    alert("失敗");
                }
            });

            $.ajax({
                type: "POST",
                url: "GAP_SizeTable.ashx?fun=getContent",
                data: {
                    pipid: pipid_compare
                },
                dataType: 'json',
                success: function (res) {

                    $('#div_Compare').html(res.html);

                },
                complete: function () {

                },
                error: function (error) {
                    alert("失敗");
                }
            });


           

            $('#addQueryModal').modal('show')
        }

        function onQuery2() {

            let pipid_source = $('#<%= dlSourceFile.ClientID %>').val();
            let pipid_compare = $('#<%= dlVerFile.ClientID %>').val();

            let arrSource = [];
            let arrCompare = [];

            $('.ck_' + pipid_source).each(function () {
                if ($(this).prop('checked')) {
                    arrSource.push($(this).attr('data-lusthid'));
                }
            });
            $('.ck_' + pipid_compare).each(function () {
                if ($(this).prop('checked')) {
                    arrCompare.push($(this).attr('data-lusthid'));
                }
            });

            //if (arrSource.length == 0) {
            //    alert('請勾選來源對應區塊');
            //    return false;
            //}
            //if (arrCompare.length == 0) {
            //    alert('請勾選目的對應區塊');
            //    return false;
            //}

            $('#<%= hid_Source_lusthid.ClientID %>').val(arrSource.join(','));
            $('#<%= hid_Compare_lusthid.ClientID %>').val(arrCompare.join(','));

            onShow(1);
            $('#<%= divHeader.ClientID %>').html('');
            $('#<%= divHeaderNotCompare.ClientID %>').html('');

        }

        function checkAll(ck,pipid) {
            let isCheck = $(ck).prop('checked')

            $('.ck_' + pipid).each(function () {
                $(this).prop('checked', isCheck);
            });
        }

        var objTd;
        var saveUrl = "";
        var orgUrl = "";
        var saveId = "";
        var saveCol = "";
        var isCompare = 0;;

        function editHeader(td, compare) {
            let text = $(td).find('span:first').text();
            //$('#editText').val(text);

            objTd = $(td);
            saveUrl = "GAP_Header.ashx?fun=saveByCol"
            saveId = $(td).attr('data-luhid');
            saveCol = $(td).attr('data-col');
            isCompare = compare;


            orgUrl = "GAP_Header.ashx?fun=get_org"
            orgId = $(td).attr('data-org_luhid');

            getOrgTest();
            getChNote("luhid");
            $('#addModalTitle').text('編輯');
            $('#addModal').modal('show')
        }

        function editBom(td, compare) {
            let text = $(td).find('span:first').text();
            //$('#editText').val(text);

            objTd = $(td);
            saveUrl = "GAP_BOM.ashx?fun=saveByCol"
            saveId = $(td).attr('data-lubid');
            saveCol = $(td).attr('data-col');
            isCompare = compare;

            orgUrl = "GAP_BOM.ashx?fun=get_org"
            orgId = $(td).attr('data-org_lubid');
            getOrgTest();
            getChNote("lubid");
            $('#addModalTitle').text('編輯');
            $('#addModal').modal('show')
        }

        function editSizeTable(td, compare) {
            let text = $(td).find('span:first').text();
            //$('#editText').val(text);

            objTd = $(td);
            saveUrl = "GAP_SizeTable.ashx?fun=saveByCol"
            saveId = $(td).attr('data-lustid');
            saveCol = $(td).attr('data-col');
            isCompare = compare;

            orgUrl = "GAP_SizeTable.ashx?fun=get_org"
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
                url: "GAP_Ch_Note.ashx?fun=get",
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


            let data = {
                id: saveId,
                col: saveCol,
                text: text,
                chNote: note
            };

            console.log(data);

            $.ajax({
                type: "POST",
                url: saveUrl,
                data: data,
                dataType: 'json',
                success: function (res) {

                    if (res.data == 1) {

                        if (isCompare == 1) {

                            let html = "<font color='red'>修:" + text + "</font><br><font color='red'>中:" + note + "</font>"
                            objTd.html(html);

                        } else {
                            objTd.text(text);
                        }



                        $('#addModal').modal('hide')
                    }

                },
                complete: function () {

                },
                error: function (error) {
                    alert("儲存失敗");
                }
            });
        }



        function Open(filePath) {
            window.open(filePath);

        }





    </script>
</asp:Content>
