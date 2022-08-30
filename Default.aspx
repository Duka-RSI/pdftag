<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="_Default" %>

<%@ Register Src="control/LoginControl.ascx" TagName="LoginControl" TagPrefix="uc1" %>
<%@ Register Src="control/MaterialListControl.ascx" TagName="MaterialListControl"
    TagPrefix="uc2" %>
<%@ Register Src="control/TestListControl.ascx" TagName="TestListControl" TagPrefix="uc3" %>
<%@ Register Src="control/PostListControl.ascx" TagName="PostListControl" TagPrefix="uc4" %>
<%@ Register Src="control/PracticeListControl.ascx" TagName="PracticeListControl"
    TagPrefix="uc5" %>
<%@ Register Src="control/OfficeHourListControl2.ascx" TagName="OfficeHourListControl2"
    TagPrefix="uc6" %>
<%@ Register Src="control/DiscussListControl.ascx" TagName="DiscussListControl" TagPrefix="uc7" %>
<%@ Register Src="control/ViewCounterControl.ascx" TagName="ViewCounterControl" TagPrefix="uc8" %>
<%@ Register Src="control/SysInfoControl.ascx" TagName="SysInfoControl" TagPrefix="uc9" %>
<%@ Register Src="control/clinicalControl.ascx" TagName="clinicalControl" TagPrefix="uc10" %>
<%@ Register Src="control/ViewCounterControl.ascx" TagName="ViewCounterControl" TagPrefix="uc11" %>
<%@ Register Src="control/HospitalListControl.ascx" TagName="HospitalListControl" TagPrefix="uc12" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="X-UA-Compatible" content="IE=edge,chrome=1" />
    <title></title>
    <link rel="stylesheet" href="css/style.css">
    <script src="js/jquery-1.8.2.js"></script>
    <script src="js/javasctipt.js"></script>
    <script language="javascript">
     $(document).ready(function () {
                <%=script %>
            });
        function MenuChange(id) {

            $('div[name=menu]').hide();
            $('#' + id).show();
        }
    
    
    </script>
</head>
<body>
    <form id="form1" runat="server">
    <div id="wrapper">
        <header>
	</header>
        <div id="menu">
            <ul>
               <!-- <li class='front'>八德廠桃園縣八德市和平路704巷25號&nbsp;&nbsp;&nbsp;Tel:(03)377-1605, Fax:(03)377-1624
    <br>鶯歌廠新北市鶯歌區二甲路38-6號&nbsp;&nbsp;&nbsp;Tel:(02)2678-3801, Fax(02)8070-9045</li> -->
               <!-- <li class='classroom'><a href='itemOrder/OrderView1.aspx' target='_blank'></a></li>
                <li class='equi'><a href='itemOrder/OrderView3.aspx' target='_blank'></a></li>
                <li class='osce'><a href='itemOrder/OrderView2.aspx' target='_blank'></a></li> -->
            </ul> 
            <div style='width: 180px; float: right; line-height: 19px;'>
                <uc11:ViewCounterControl ID="ViewCounterControl" runat="server" />
            </div>
        </div>
        <div id="content">
            <nav>
		</nav>
            <div id="primary">
                <div id="main_Login" name="menu" style="display: none">
                    <%--<uc1:LoginControl ID="LoginControl1" runat="server" />--%>
                    <div id="contentLogout">
                        <div id="loginPage">
                            <div class="title">
                            </div>
                            <p>
                                *請利用校務資訊系統設定之帳號/密碼，進行登入作業<br>
                                *若無法登入，請與管理單位聯繫</p>
                            <ul>
                                <li class="acct">
                                    <asp:TextBox ID="txtLogin" runat="server" Width="350px"></asp:TextBox></li>
                                <li class="psw">
                                    <asp:TextBox ID="txtPassword" runat="server" Width="350px" TextMode="Password"></asp:TextBox></li>
                                <li>
                                    <asp:LinkButton ID="LinkButton1" runat="server" OnClick="LinkButton1_Click"><img border="0" src="<%=Request.ApplicationPath %>/images/btn_login.png" class="SwapImg"  img1="<%=Request.ApplicationPath %>/images/btn_login.png" img2="<%=Request.ApplicationPath %>/images/btn_login_.png" /></asp:LinkButton>
                                    &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                                    <img src="<%=Request.ApplicationPath %>/images/btn_cancel.png" class="SwapImg" img1="<%=Request.ApplicationPath %>/images/btn_cancel.png"
                                        img2="<%=Request.ApplicationPath %>/images/btn_cancel_.png" />
                                    <asp:Label ID="lblError" runat="server" ForeColor="Red"></asp:Label>
                                </li>
                            </ul>
                        </div>
                    </div>
                </div>
                <div id="main_conten" name="menu">
                    <uc4:PostListControl ID="PostListControl1" runat="server" />
                </div>
                <div id="main_officehour" name="menu" style="display: none">
                    <uc6:OfficeHourListControl2 ID="OfficeHourListControl1" runat="server" />
                </div>
                <div id="main_clinical" name="menu" style="display: none">
                    <uc10:clinicalControl ID="clinicalControl" runat="server" />
                </div>
                <div id="main_Material" name="menu" style="display: none">
                    <uc2:MaterialListControl ID="MaterialListControl1" runat="server" />
                </div>
                <div id="main_TestList" name="menu" style="display: none">
                    <uc3:TestListControl ID="TestListControl1" runat="server" />
                </div>
                <div id="main_DiscussList" name="menu" style="display: none">
                    <uc7:DiscussListControl ID="DiscussListControl1" runat="server" />
                </div>
                <div id="main_HospitalList" name="menu" style="display: none">
                    <uc12:HospitalListControl ID="HospitalListControl1" runat="server" />
                </div>
            </div>
        </div>
    
    <footer>
	</footer></div>
    </form>
</body>
</html>
