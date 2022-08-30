<%@ Page Title="" Language="C#" MasterPageFile="~/master/MasterPage_Login.master"
    AutoEventWireup="true" CodeFile="PDF_Manage_003_1.aspx.cs" Inherits="Passport_Passport_A000" %>

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
                        <asp:Label ID="lblInfo" runat="server"></asp:Label>:Header
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
                    <%-- <font color="red">點擊儲存格進行編輯修改</font>--%>
                    <a href="PDF_Manage_003_1_1.aspx?hdid=<%=hdid %>">
                        <input type="button" value="類別1管理" class="btn btn-secondary"></a>
                    <a href="PDF_Manage_003_1_2.aspx?hdid=<%=hdid %>">
                        <input type="button" value="類別2管理" class="btn btn-secondary"></a>
                    <a href="PDF_Manage_003_1_3.aspx?hdid=<%=hdid %>">
                        <input type="button" value="類別3管理" class="btn btn-secondary"></a>
                    <a href="PDF_Manage_003_1_4.aspx?hdid=<%=hdid %>">
                        <input type="button" value="類別4管理" class="btn btn-secondary"></a>
                    <div id="divHeader" runat="server"></div>
                    <asp:Button ID="btnEdit" runat="server" Text="儲存" CssClass="btn btn-warning" OnClick="btnEdit_Click" OnClientClick="return onSave()" />
                    <asp:HiddenField ID="hidluhid" runat="server" />
                    <asp:HiddenField ID="hid_STYLE_DESC_LEVEL1" runat="server" />
                    <asp:HiddenField ID="hid_STYLE_DESC_LEVEL2" runat="server" />
                    <asp:HiddenField ID="hid_STYLE_DESC_LEVEL3" runat="server" />
                    <asp:HiddenField ID="hid_STYLE_DESC_LEVEL4" runat="server" />
                </td>
            </tr>
        </table>
    </div>
    <script type="text/javascript">
        $(document).ready(function () {
                <%=script %>


        });
        function onSave() {

            let style1 = $("#dlStyleLevel1").val();
            let style2 = $("#dlStyleLevel2").val();
            let style3 = $("#dlStyleLevel3").val();
            let style4 = $("#dlStyleLevel4").val();

            $('#<%=hid_STYLE_DESC_LEVEL1.ClientID %>').val(style1);
            $('#<%=hid_STYLE_DESC_LEVEL2.ClientID %>').val(style2);
            $('#<%=hid_STYLE_DESC_LEVEL3.ClientID %>').val(style3);
            $('#<%=hid_STYLE_DESC_LEVEL4.ClientID %>').val(style4);

        }

        function uploadFile(file) {

            let luhid = $('#<%=hidluhid.ClientID %>').val();

            var formData = new FormData();
            formData.append('file', $(file)[0].files[0]);
            formData.append('luhid', luhid);


            $.ajax({
                url: 'Lu_Header.ashx?fun=upload_stylesketch',
                type: 'POST',
                data: formData,
                async: false,
                processData: false,
                contentType: false,
                success: function (res) {
                    let obj = JSON.parse(res);

                    if (!obj.success) {
                        alert('上傳失敗!');
                    } else {

                        let html = "";
                        html += "<a href='#' onclick='Open(&#039;" + obj.file + "&#039;)'>" + obj.fileName + "</a>";
                        html += "<input type='button' value='刪除' onclick='deleteResultFile(" + obj.luhid + ")'/>";

                        $('#divReultFile').html(html);
                    }

                }, error: function (error) {
                    alert('上傳失敗!...');
                }
            });
        }

        function deleteResultFile(qrlcruid) {


            if (confirm('確定要刪除?')) {

                let luhid = $('#<%=hidluhid.ClientID %>').val();

                $.ajax({
                    type: "POST",
                    url: 'Lu_Header.ashx?fun=delete_stylesketch&luhid=' + luhid,
                    dataType: 'json',
                    success: function (result) {
                        $('#divReultFile').html('');
                    },
                    complete: function () {

                    },
                    error: function (error) {
                        alert(error);
                    }
                });

            }

        }


        function Open(filePath) {
            window.open(filePath);

        }





    </script>
</asp:Content>
