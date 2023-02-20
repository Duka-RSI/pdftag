<%@ Page Title="" Language="C#" MasterPageFile="~/master/MasterPage_Login.master"
    AutoEventWireup="true" CodeFile="PDF_Manage_002.aspx.cs" Inherits="Passport_Passport_A000" %>

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
                    <th scope="col" colspan="5">歷程管理</th>
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
                                <asp:ListItem Value="" Text="全部"></asp:ListItem>
                               <%-- <asp:ListItem Value="1" Text="Lulu"></asp:ListItem>
                                <asp:ListItem Value="2" Text="UA"></asp:ListItem>
                                <asp:ListItem Value="3" Text="GAP"></asp:ListItem>--%>
                            </asp:DropDownList>
                        </div>
                        <div class="col-0">
                            <label>
                                標題：
                            </label>
                        </div>
                        <div class="col-4 col-md-3">
                            <asp:DropDownList ID="dlTitle" runat="server" CssClass="form-control">
                            </asp:DropDownList>
                        </div>
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
                            <%-- <button type="button" class="btn btn-secondary" onclick="showAdd();">新增</button>--%>
                        </div>
                    </div>

                </th>
            </tr>
            <tr>
                <td style="height: 450px; vertical-align: top;">

                    <table class="table table-hover">
                       <%-- <tr>
                            <th scope="col" colspan="5">文件來源</th>
                            <th scope="col" colspan="3">編輯來源</th>
                        </tr>--%>
                        <tr>
                            <th scope="col">編號</th>
                            <th scope="col">標題</th>
                            <th scope="col">版本</th>
                            <th scope="col">上傳日期</th>
                            <th scope="col">上傳PDF檔名</th>
                            <th scope="col">版本</th>
                            <th scope="col">建立者</th>
                            <th scope="col">編輯時間</th>
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
                                        <%# Eval("hisversion")%>
                                    </td>
                                    <td>
                                        <%# Eval("pidate")%>
                                    </td>
                                    <td style="font-size: 14px; word-break: break-all">
                                        <font size='2'><a href="#" onclick="Open('<%# Eval("piuploadfile")%>')"><%# System.IO.Path.GetFileName((string)Eval("piuploadfile"))%></a></font>
                                    </td>
                                    <td>
                                        <%# (int)Eval("pver")==1?"Lulu": (int)Eval("pver")==2?"UA": (int)Eval("pver")==3?"GAP":""%>
                                    </td>
                                    <td>
                                        <%# Eval("creator")%>
                                    </td>
                                    <td>
                                        <%# Eval("editdate")%>
                                    </td>
                                    <td>
                                        <%--  <a href="#" onclick="showEdit('<%# Eval("pipid")%>','<%# Eval("ptitle")%>','<%# Eval("pidate")%>','<%# Eval("piuploadfile")%>','<%# Eval("pver")%>','<%# Eval("creator")%>','<%# Eval("createordate")%>','<%# Eval("mdate")%>','<%# Eval("isShow")%>' );return false;">
                                            <input type="button" value="編輯" class="btn btn-secondary"></a>
                                        --%>
                                        <asp:LinkButton ID="LinkButton2" CommandName="del" CommandArgument='<%# Eval("hdid")%>'
                                            runat="server" OnClientClick="return confirm('是否要刪除 ?');"><input type="button" value="刪除" class="btn btn-danger"></asp:LinkButton>

                                        <a href="PDF_Manage_002_1<%# (int)Eval("pver")==1?"":(int)Eval("pver")==2?"_UA": (int)Eval("pver")==3?"_GAP":""%>.aspx?hdid=<%# Eval("hdid")%>">
                                            <input type="button" value="進入編輯" class="btn btn-secondary"></a>
                                        <a href="PDF_Manage_002_2<%# (int)Eval("pver")==1?"":(int)Eval("pver")==2?"_UA": (int)Eval("pver")==3?"_GAP":""%>.aspx?hdid=<%# Eval("hdid")%>">
                                            <input type="button" value="比對版本" class="btn btn-warning"></a>
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
                            <th>版本:
                            </th>
                            <td align="left">
                                <asp:DropDownList ID="ddlpver" runat="server" CssClass="form-control" onchange="ddlpver_change()">
                                   <%-- <asp:ListItem Value="1" Text="Lulu"></asp:ListItem>
                                    <asp:ListItem Value="2" Text="UA"></asp:ListItem>
                                    <asp:ListItem Value="3" Text="GAP"></asp:ListItem>--%>
                                </asp:DropDownList>
                            </td>
                        </tr>
                        <tr>
                            <th>文件來源:
                            </th>
                            <td align="left">
                                <select id="ddlfile" class="form-control" onchange="ddlfile_change()"></select>
                            </td>
                        </tr>
                        <tr>
                            <th>編號:
                            </th>
                            <td align="left">
                                <span id="span_pipid"></span>
                            </td>
                        </tr>
                        <tr>
                            <th>標題:
                            </th>
                            <td align="left">
                                <span id="span_ptitle"></span>
                            </td>
                        </tr>
                        <tr>
                            <th>檔案:
                            </th>
                            <td align="left">
                                <div id="divFile"></div>
                            </td>
                        </tr>
                        <tr>
                            <td colspan="2" align="center">
                                <asp:HiddenField ID="hidpipid" runat="server" />
                                <asp:Button ID="Button1" runat="server" Text="取消" CssClass="btn btn-danger" data-dismiss="modal" />
                                <asp:Button ID="btnAdd" runat="server" Text="確定" CssClass="btn btn-success" OnClick="btnAdd_Click" OnClientClick="return check()" />
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

            $('#<%=hidpipid.ClientID %>').val('');
            $('#<%=ddlpver.ClientID %>').val('');
            $('#ddlfile').html('');
            $('#span_pipid').text("");
            $('#span_ptitle').text("");
            $("#divFile").html('');

            $('#addModalTitle').text('新增');
            $('#addModal').modal('show')
        }


        function ddlpver_change() {
            let pver = $('#<%=ddlpver.ClientID %>').val();
            $('#ddlfile').html('');

            $.ajax({
                type: "POST",
                url: 'P_inProcess.ashx?fun=getFiles&pver=' + pver,
                dataType: 'json',
                success: function (res) {

                    let html = "<option value=''>請選擇</option>";

                    for (let i in res) {

                        let filepath = res[i].piuploadfile;
                        if (filepath) {

                            var fvals = filepath.split('/');
                            html += "<option value='" + res[i].pipid + "'>" + fvals[fvals.length - 1] + "</option>"
                        }


                    }

                    $('#ddlfile').html(html);


                },
                complete: function () {

                },
                error: function (error) {
                    //alert(error);
                }
            });
        }

        function ddlfile_change() {
            let pipid = $('#ddlfile').val();
            $('#span_pipid').text("");
            $('#span_ptitle').text("");
            $("#divFile").html('');

            $.ajax({
                type: "POST",
                url: 'P_inProcess.ashx?fun=get&pipid=' + pipid,
                dataType: 'json',
                success: function (res) {

                    $('#span_pipid').text(res.pipid);
                    $('#span_ptitle').text(res.ptitle);


                    let filepath = res.piuploadfile;
                    if (filepath) {

                        var fvals = filepath.split('/');
                        $('#divFile').html("<a href='#' onclick='Open(&#039;" + filepath + "&#039;)'>" + fvals[fvals.length - 1] + "</a>")
                    }


                },
                complete: function () {

                },
                error: function (error) {
                    //alert(error);
                }
            });
        }

        function check() {
            let pipid = $('#ddlfile').val();
            $('#<%=hidpipid.ClientID %>').val(pipid);

            if (!pipid) {
                alert('請選擇文件');
                return false;
            }
        }


        function Open(filePath) {
            window.open(filePath);

        }





    </script>
</asp:Content>
