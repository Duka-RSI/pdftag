<%@ Page Title="" Language="C#" MasterPageFile="~/master/MasterPage_Login.master"
    AutoEventWireup="true" CodeFile="PDF_Manage_001.aspx.cs" Inherits="Passport_Passport_A000" %>

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
                    <th scope="col" colspan="5">PDF檔管理</th>
                </tr>
            </thead>
            <tr>
                <th scope="col" colspan="5">
                    <div class="row m-0">
                        <div class="col-0">
                            <label>
                                版本：
                            </label>
                        </div>
                        <div class="col-4 col-md-3">
                            <asp:DropDownList ID="dlVersion" runat="server" CssClass="form-control" OnSelectedIndexChanged="dlVersion_SelectedIndexChanged" AutoPostBack="true">
                                <%-- <asp:ListItem Value="" Text="全部"></asp:ListItem>--%>
                                <%-- <asp:ListItem Value="1" Text="Lulu"></asp:ListItem>
                                <asp:ListItem Value="2" Text="UA"></asp:ListItem>
                                <asp:ListItem Value="3" Text="GAP"></asp:ListItem>--%>
                            </asp:DropDownList>
                        </div>
                        <div class="col-0">
                            <label>
                                群組名稱：
                            </label>
                        </div>
                        <div class="col-4 col-md-3">
                            <asp:DropDownList ID="ddlGroup" runat="server" CssClass="form-control">
                            </asp:DropDownList>
                        </div>
                    </div>
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
                            <a href="PDF_Manage_001_1.aspx">
                                <input type="button" value="群組名稱" class="btn btn-warning"></a>
                        </div>
                    </div>
                </th>
            </tr>
            <tr>
                <td style="height: 450px; vertical-align: top;">

                    <table class="table">
                        <tr>
                            <th scope="col">編號</th>
                            <th scope="col">款號</th>
                            <th scope="col">群組名稱</th>
                            <th scope="col">客戶款號</th>
                            <th scope="col">季節</th>
                            <th scope="col">BOM Date</th>
                            <th scope="col" width='20%'>上傳PDF檔名</th>
                            <th scope="col">版本</th>
                            <th scope="col">建立者</th>
                            <th scope="col">執行時間</th>
                            <th scope="col"></th>
                        </tr>
                        <asp:Repeater ID="Repeater1" runat="server" OnItemCommand="Repeater1_ItemCommand">
                            <ItemTemplate>
                                <tr>
                                    <td>
                                        <%# Eval("pipid")%>
                                    </td>
                                    <td>
                                        <%# Eval("ptitle")%>
                                    </td>
                                    <td>
                                        <%# Eval("gmname")%>
                                    </td>
                                    <td>
                                        <%# Eval("style")%>
                                    </td>
                                    <td>
                                        <%# Eval("season")%>
                                    </td>
                                    <td>
                                        <%-- <%# Eval("pidate")%>--%>
                                        <%# Eval("generateddate")%>
                                    </td>
                                    <td style="word-break: break-all">
                                        <a href="#" onclick="Open('<%# Eval("piuploadfile")%>')"><%# System.IO.Path.GetFileName((string)Eval("piuploadfile"))%></a>
                                    </td>
                                    <td>
                                        <%# (int)Eval("pver")==1?"Lulu": (int)Eval("pver")==2?"UA": (int)Eval("pver")==3?"GAP":""%>
                                    </td>
                                    <td>
                                        <%# Eval("creator")%>
                                    </td>
                                    <td>
                                        <%# Eval("mdate")%>
                                    </td>
                                    <td>
                                        <a href="#" onclick="showEdit('<%# Eval("pipid")%>','<%# Eval("gmid")%>','<%# Eval("ptitle")%>','<%# Eval("pidate")%>','<%# Eval("piuploadfile")%>'
                                            ,'<%# Eval("pver")%>','<%# Eval("Gaptype")%>','<%# Eval("creator")%>','<%# Eval("createordate")%>','<%# Eval("mdate")%>','<%# Eval("isShow")%>','<%# Eval("titleType")%>','<%# Eval("unit")%>' );return false;">
                                            <input type="button" value="編輯" class="btn btn-secondary"></a>
                                        <asp:LinkButton ID="LinkButton2" CommandName="del" CommandArgument='<%# Eval("pipid")%>'
                                            runat="server" OnClientClick="return confirm('是否要刪除 ?');"><input type="button" value="刪除" class="btn btn-danger"></asp:LinkButton>

                                        <span <%# ((int)Eval("pver")==1 && string.IsNullOrEmpty(Eval("mdate").ToString()))?"":"style='display:none'"%>>
                                            <asp:LinkButton ID="LinkButton1" CommandName="parsePDF" CommandArgument='<%# Eval("pipid")%>'
                                                runat="server" OnClientClick="return onParsePDF();"><input type="button" value="執行" class="btn btn-warning"></asp:LinkButton>
                                        </span>

                                        <span <%# ((int)Eval("pver")==2 && string.IsNullOrEmpty(Eval("mdate").ToString()))?"":"style='display:none'"%>>
                                            <%--<a href="#" onclick="showExecuteGAP('<%# Eval("pipid")%>');return false;"><input type="button" value="執行" class="btn btn-secondary"></a>--%>
                                            <asp:LinkButton ID="LinkButton4" CommandName="parsePDF_UA" CommandArgument='<%# Eval("pipid")%>'
                                                runat="server" OnClientClick="return onParsePDF();"><input type="button" value="執行" class="btn btn-warning"></asp:LinkButton>
                                        </span>

                                        <span <%# ((int)Eval("pver")==3 && string.IsNullOrEmpty(Eval("mdate").ToString()))?"":"style='display:none'"%>>
                                            <%--<a href="#" onclick="showExecuteGAP('<%# Eval("pipid")%>');return false;"><input type="button" value="執行" class="btn btn-secondary"></a>--%>
                                            <asp:LinkButton ID="LinkButton3" CommandName="parsePDF_GAP" CommandArgument='<%# Eval("pipid")%>'
                                                runat="server" OnClientClick="return onParsePDF();"><input type="button" value="執行" class="btn btn-warning"></asp:LinkButton>
                                        </span>
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
                            <th>標題:
                            </th>
                            <td align="left">
                                <asp:DropDownList ID="dlTitleType" runat="server" onchange="dlTitleType_change()">
                                    <asp:ListItem Value="1" Text="系統於執行後帶入"></asp:ListItem>
                                    <asp:ListItem Value="2" Text="自行輸入"></asp:ListItem>
                                </asp:DropDownList>
                                <asp:TextBox ID="ptitle" runat="server" CssClass="form-control"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <th>檔案:
                            </th>
                            <td align="left">
                                <asp:FileUpload ID="FileUpload1" runat="server" CssClass="form-control" />
                                <div id="divFile"></div>
                            </td>
                        </tr>
                        <tr>
                            <th>版本:
                            </th>
                            <td align="left">
                                <asp:DropDownList ID="ddlpver" runat="server" CssClass="form-control" onchange="ddlpver_change()">
                                    <%--  <asp:ListItem Value="1" Text="Lulu"></asp:ListItem>
                                    <asp:ListItem Value="2" Text="UA"></asp:ListItem>
                                    <asp:ListItem Value="3" Text="GAP"></asp:ListItem>--%>
                                </asp:DropDownList>
                            </td>
                        </tr>
                        <tr id="rowGap">
                            <th>格式選項:
                            </th>
                            <td align="left">
                                <asp:DropDownList ID="dlGaptype" runat="server" CssClass="form-control">
                                    <asp:ListItem Value="2" Text="有格線"></asp:ListItem>
                                    <asp:ListItem Value="1" Text="無格線"></asp:ListItem>

                                </asp:DropDownList>
                            </td>
                        </tr>
                        <tr>
                            <th>size table單位:
                            </th>
                            <td align="left">
                                <asp:DropDownList ID="dlunit" runat="server" CssClass="form-control">
                                    <asp:ListItem Value="inch" Text="inch"></asp:ListItem>
                                    <asp:ListItem Value="cm" Text="cm"></asp:ListItem>
                                </asp:DropDownList>
                            </td>
                        </tr>
                        <tr>
                            <td align="left" colspan="2">群組類別:
                                <asp:DropDownList ID="dlgmcate" runat="server">
                                    <%-- <asp:ListItem Value="1" Text="Lulu"></asp:ListItem>
                                    <asp:ListItem Value="2" Text="UA"></asp:ListItem>
                                    <asp:ListItem Value="3" Text="GAP"></asp:ListItem>--%>
                                </asp:DropDownList>
                                <asp:HiddenField ID="hidgmid" runat="server" />
                                群組名稱:<input type="text" id="txtSearch2" /><input type="button" value="查詢" onclick="onQuery()" />
                                <div style="height: 300px; overflow-y: scroll">
                                    <table>
                                        <tr>
                                            <th></th>
                                            <th>群組名稱	
                                            </th>
                                        </tr>
                                        <tbody id="tbl"></tbody>
                                    </table>
                                </div>
                            </td>
                        </tr>
                        <tr>
                            <td colspan="2" align="center">
                                <asp:HiddenField ID="hidpipid" runat="server" />
                                <asp:Button ID="Button1" runat="server" Text="取消" CssClass="btn btn-danger" data-dismiss="modal" />
                                <asp:Button ID="btnAdd" runat="server" Text="確定" CssClass="btn btn-success" OnClick="btnAdd_Click" OnClientClick="return onSave()" />
                                <asp:Button ID="btnEdit" runat="server" Text="儲存" CssClass="btn btn-warning" OnClick="btnEdit_Click" OnClientClick="return onSave()" />
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

            $("#<%=FileUpload1.ClientID%>").attr('accept', 'application/pdf');
            hideLoading();
            $('#<%=dlgmcate.ClientID %>').prop('disabled', true);
        });

        function ddlpver_change() {
            let pver = $('#<%= ddlpver.ClientID %>').val();
            if (pver == "3") {
                
                $('#rowGap').show();
            } else {
                $('#rowGap').val('1');
                $('#rowGap').hide();

            }
              $('#tbl').html('');
            $('#<%=dlgmcate.ClientID %>').val(pver);

        }

        function onParsePDF() {
            if (confirm('是否要執行 ?')) {
                showLoading();
                return true;
            }
            return false;
        }

        function dlTitleType_change() {
            let titleType = $('#<%= dlTitleType.ClientID %>').val();
            if (titleType == "2") {
                $('#<%=ptitle.ClientID %>').show();
            } else {
                $('#<%=ptitle.ClientID %>').val('');
                $('#<%=ptitle.ClientID %>').hide();

            }
        }

        function showAdd() {
            $('#<%=btnAdd.ClientID %>').show();
            $('#<%=btnEdit.ClientID %>').hide();


            $('#<%=hidpipid.ClientID %>').val('');
            $('#<%= dlTitleType.ClientID %>').val("1");
            dlTitleType_change();
            $('#<%=ptitle.ClientID %>').val('');
            $('#<%=ddlpver.ClientID %>').val('1');
            ddlpver_change();

            $('#<%=FileUpload1.ClientID %>').val('');
            $("#divFile").html('');

            $('#<%=hidgmid.ClientID %>').val('');
            //onQuery();

            $('#addModalTitle').text('新增');
            $('#addModal').modal('show')
        }

        function showEdit(pipid, gmid, ptitle, pidate, piuploadfile, pver, Gaptype, creator, createordate, mdate, isShow, titleType, unit) {
            $('#<%=btnAdd.ClientID %>').hide();
            $('#<%=btnEdit.ClientID %>').show();


            $('#<%=hidpipid.ClientID %>').val(pipid);
            $('#<%= dlTitleType.ClientID %>').val(titleType);
            dlTitleType_change();
            $('#<%=ptitle.ClientID %>').val(ptitle);
            $('#<%=ddlpver.ClientID %>').val(pver);
            $('#<%=dlGaptype.ClientID %>').val(Gaptype);
            ddlpver_change()

            $('#<%=dlunit.ClientID %>').val(unit);

            $("#divFile").html('<a href="#" onclick="Open(&#039;' + piuploadfile + '&#039;)">檢視</a> ');


            $('#<%=hidgmid.ClientID %>').val(gmid);

            onQuery();

            $('#addModalTitle').text('編輯');
            $('#addModal').modal('show')


        }

        function onSave() {

            let gmid = '';


            $('.ckgmid').each(function () {

                if ($(this).prop("checked")) {
                    gmid = $(this).attr('data-gmid');
                    return;
                }
            });

            $('#<%=hidgmid.ClientID %>').val(gmid);

            if (!gmid) {
                alert('請選擇群組名稱');
                return false;
            }

            showLoading();
        }


        function deleteFile(pipid) {

            if (confirm('您確定要刪除?')) {


                $.ajax({
                    type: "post",
                    url: 'NewsManageFile.ashx?fun=delNewsManageFile',
                    dataType: 'json',
                    data: { nmfid: nmfid },
                    success: function (result) {

                        getUploadFile(tpaid);
                    },
                    complete: function () {

                    },
                    error: function (error) {

                    }
                });
            }
        }


        function onQuery() {
            let gmcate = $('#<%=dlgmcate.ClientID %>').val();
            let txt = $('#txtSearch2').val();

            $('#tbl').html('');

            $.ajax({
                type: "post",
                url: 'GroupManage.ashx?fun=getSearch&gmcate=' + gmcate + '&txt=' + txt,
                dataType: 'json',
                data: {},
                success: function (res) {
                    let gmid = $('#<%=hidgmid.ClientID %>').val();
                    let html = "";
                    for (let i in res) {
                        html += "<tr>";
                        html += "<td><input type='checkbox' class='ckgmid' data-gmid='" + res[i].gmid + "' " + (res[i].gmid == gmid ? "checked" : "") + "></td>";
                        html += "<td>" + res[i].gmname + "</td>";
                        html += "</tr>";
                    }
                    $('#tbl').html(html);

                },
                complete: function () {

                },
                error: function (error) {

                }
            });
        }


        function showExecuteGAP(pipid) {

            $.ajax({
                type: "post",
                url: 'GAP.ashx?fun=parse&pipid=' + pipid,
                dataType: 'json',
                data: {},
                success: function (res) {


                },
                complete: function () {

                },
                error: function (error) {

                }
            });

        }

        function Open(filePath) {
            window.open(filePath);

        }





    </script>
</asp:Content>
