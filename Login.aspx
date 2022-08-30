<%@ Page Title="" Language="C#" MasterPageFile="~/master/MasterPage.master" AutoEventWireup="true" CodeFile="Login.aspx.cs" Inherits="Login" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <div id="banner">
        <!-- <img class="bg" src="img/fake.png" alt=""> -->
        <div id="banner-title-box">
            <article>
                  <div id="caption">
                      <h2>
                        會員登入
                      </h2>
                  </div>
                </article>
        </div>
    </div>
    <div id="content" class="container mt-5">
        <div id="login-frame">
            <form>
                <div class="form-group row">
                    <label for="inputAcct" class="col-sm-2 col-form-label">
                        <h5 class="secondary-color">帳號
                        </h5>
                    </label>
                    <div class="col-sm-10">
                        <asp:TextBox ID="txtAccount" runat="server" placeholder="Account" CssClass="form-control"></asp:TextBox>
                    </div>
                </div>
                <div class="form-group row">
                    <label for="inputPsw" class="col-sm-2 col-form-label">
                        <h5 class="secondary-color">密碼
                        </h5>
                    </label>
                    <div class="col-sm-10">
                        <asp:TextBox ID="txtPw" runat="server" TextMode="Password" placeholder="Password" CssClass="form-control"></asp:TextBox>
                    </div>
                </div>
                <div class="form-group row">
                    <div class="col-sm-12 text-center">
                        <asp:Button ID="btnLogin" runat="server" Text="登入" CssClass="btn bg-main-color text-white" OnClick="btnLogin_Click" />

                        <button type="button" class="btn btn-secondary">忘記密碼</button>
                    
                    </div>
                </div>
            </form>
        </div>
    </div>
    <script type="text/javascript">
        $(document).ready(function () {
                <%=script %>

         });



    </script>
</asp:Content>

