<%@ Page Title="" Language="C#" MasterPageFile="~/master/MasterPage_Login.master"
    AutoEventWireup="true" CodeFile="PDF_Manage_003_2.aspx.cs" Inherits="Passport_Passport_A000" %>

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
                        <a href="PDF_Manage_003.aspx">RD系統欄位</a> >>
                        <asp:Label ID="lblInfo" runat="server"></asp:Label>:BOM Comments
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
                            <td align="left">
                                原文件
                            </td>
                            <td align="left">
                                <textarea id="orgText" class="form-control" disabled></textarea>
                            </td>
                        </tr>
                        <tr>
                            <td align="left">
                                更改
                            </td>
                            <td align="left">
                                <textarea id="editText" class="form-control"></textarea>
                            </td>
                        </tr>
                         <tr>
                            <td align="left">
                                中文備註
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
    <script type="text/javascript">
    $(document).ready(function () {
                <%=script %>


    });
        var objTd;
        var saveUrl = "";
        var orgUrl = "";
        var saveId = "";
        var saveCol = "";

        function editHeader(td) {
            let text = $(td).find('span:first').text();
            //$('#editText').val(text);

            objTd = $(td);
            saveUrl = "Lu_Header.ashx?fun=saveByCol"
            saveId = $(td).attr('data-luhid');
            saveCol = $(td).attr('data-col');


          
            orgUrl = "Lu_Header.ashx?fun=get_org"
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
            saveUrl = "Lu_BOM.ashx?fun=saveByCol"
            saveId = $(td).attr('data-lubid');
            saveCol = $(td).attr('data-col');

            orgUrl = "Lu_BOM.ashx?fun=get_org"
            orgId = $(td).attr('data-org_lubid');
            getOrgTest();
            getChNote("lubid");
            $('#addModalTitle').text('編輯');
            $('#addModal').modal('show')
        }

        function editSizeTable(td) {
            let text = $(td).find('span:first').text();
            //$('#editText').val(text);

            objTd = $(td);
            saveUrl = "Lu_SizeTable.ashx?fun=saveByCol"
            saveId = $(td).attr('data-lustid');
            saveCol = $(td).attr('data-col');

            orgUrl = "Lu_SizeTable.ashx?fun=get_org"
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
                url: "Lu_Ch_Note.ashx?fun=get",
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

                    if (res == 1) {

                        //objTd.text(text);

                        let html = "<font>原:" + orgtext + "</font><br><font color='red'>修:" + text + "</font><br><font color='blue'>中:" + note + "</font>"

                        objTd.html(html);
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
