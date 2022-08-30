$(function () {
    //$("#menu").append("<ul> <li class='front'>物品借閱表123</li> <li class='classroom'><a href='itemOrder/OrderView1.aspx' target='_blank'></a></li> <li class='equi'><a href='itemOrder/OrderView3.aspx' target='_blank'></a></li> <li class='osce'><a href='itemOrder/OrderView2.aspx' target='_blank'></a></li> </ul> <div style='width:180px;float:right;line-height:19px;'> 今日瀏覽人次：00065<br> 累計人次：35731<br> <div class='button1' style='display:none'><a href='material/MaterialList.aspx' ></a></div> </div>");

    var LoginSession = '';
    var role = '0';
    $.ajax({
        type: "POST",
        async: false,
        url: "./session.ashx",
        data: { 'file': 'dave' },
        dataType: "json",
        success: function (result) {
            LoginSession = result.id;
            role = result.role;
        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            //alert("Status: " + textStatus); alert("Error: " + errorThrown); 
        },
        complete: function () {
            console.log('session:' + LoginSession);
            console.log('role:' + role);
        }
    });

    if (LoginSession == '') {
        $("#content nav").append("<ul> <li><a href='Login.aspx'>使用者登入</a></li>    </ul>");
    // $("#content nav").append("<ul> <li><a href='Login.aspx'>使用者登入</a></li> <li><a href='UF011.aspx' >借用登記</a></li><li><a href='UF012.aspx' >物品查詢</a></li>    </ul>");
    
	}
    else {
        //$("#content nav").append("<ul> <li><a href='Login/LoginOut.aspx'>登出</a></li><li><a href='UF001.aspx'>主檔設定</a></li> <li class='qq'><a href='UF002.aspx'>寫入資料</a>  </li><li><a href='UF003.aspx' > 匯出資料</a></li><li><a href='UF004.aspx' >統計圖表</a></li>  </ul>");
        //if (role=='1')
        //    $("#content nav").append("<ul> <li><a href='Login/LoginOut.aspx'>登出</a></li><li><a href='UF005.aspx'>人員管理</a></li> <li class='qq'><a href='UF006.aspx'>帳號管理</a>  </li><li><a href='UF007.aspx' > 倉儲位置</a></li><li><a href='UF013.aspx' > 條碼清冊</a></li><li><a href='UF008.aspx' >進貨管理</a></li><li><a href='UF009.aspx' >報表管理</a></li><li><a href='UF011.aspx' >借用登記</a></li><li><a href='UF012.aspx' >物品查詢</a></li>  </ul>");
        //else if (role == '2')
        //    $("#content nav").append("<ul> <li><a href='Login/LoginOut.aspx'>登出</a></li><li><a href='UF005.aspx'>人員管理</a></li> <li class='qq'><a href='UF006.aspx'>帳號管理</a>  </li><li><a href='UF007.aspx' > 倉儲位置</a></li><li><a href='UF008.aspx' >進貨管理</a></li><li><a  href='UF009.aspx' >報表管理</a></li><li><a href='UF011.aspx' >借用登記</a></li><li><a href='UF012.aspx' >物品查詢</a></li>  </ul>");
        //else if (role == '3')
        //    $("#content nav").append("<ul> <li><a href='Login/LoginOut.aspx'>登出</a></li><li><a href='UF007.aspx' > 倉儲位置</a></li><li><a href='UF008.aspx' >進貨管理</a></li><li><a href='UF009.aspx' >報表管理</a></li><li><a href='UF011.aspx' >借用登記</a></li><li><a href='UF012.aspx' >物品查詢</a></li>  </ul>");
        //else if (role == '4')
        //    $("#content nav").append("<ul> <li><a href='Login/LoginOut.aspx'>登出</a></li><li><a href='UF011.aspx' >借用登記</a></li><li><a href='UF012.aspx' >物品查詢</a></li>  </ul>");
        //else
        //    $("#content nav").append("<ul> <li><a href='Login/LoginOut.aspx'>登出</a></li><li><a href='UF011.aspx' >借用登記</a></li><li><a href='UF012.aspx' >物品查詢</a></li></ul>");

        if (LoginSession.substring(0, 1) == "P")
            $("#content nav").append("<ul> <li><a href='Login/LoginOut.aspx'>登出</a></li><li><a href='UF031.aspx'>基本資料維護</a></li><li><a href='javascript:void(0)'  onclick='menuChange(&#039;main5a&#039;)'>專案管理</a></li><li><a href='javascript:void(0)'  onclick='menuChange(&#039;main5b&#039;)'>會議管理</a></li><li><a href='javascript:void(0)'  onclick='menuChange(&#039;main7b&#039;)'>行政支援管理</a></li></ul>");
        else
            $("#content nav").append("<ul> <li><a href='Login/LoginOut.aspx'>登出</a></li><li><a href='javascript:void(0)' onclick='menuChange(&#039;main1&#039;)' >基本資料管理</a></li><li><a href='javascript:void(0)'  onclick='menuChange(&#039;main2&#039;)'>工具管理(待確認)</a></li><li><a href='javascript:void(0)'  onclick='menuChange(&#039;main3&#039;)'>同步歷程管理</a></li><li><a href='javascript:void(0)'  onclick='menuChange(&#039;main5a&#039;)'>專案管理</a></li><li><a href='javascript:void(0)'  onclick='menuChange(&#039;main6&#039;)'>會議管理</a></li><li><a href='javascript:void(0)'  onclick='menuChange(&#039;main7&#039;)'>行政支援管理</a></li></ul>");

		//    $("#content nav").append("<ul> <li><a href='Login/LoginOut.aspx'>登出</a></li><li><a href='javascript:void(0)' onclick='menuChange(&#039;main1&#039;)' >基本資料管理</a></li><li><a href='javascript:void(0)'  onclick='menuChange(&#039;main2&#039;)'>工具管理(待確認)</a></li><li><a href='javascript:void(0)'  onclick='menuChange(&#039;main3&#039;)'>同步歷程管理</a></li><li><a href='javascript:void(0)'  onclick='menuChange(&#039;main4&#039;)'>匯入資料檢視</a></li><li><a href='javascript:void(0)'  onclick='menuChange(&#039;main5&#039;)'>專案管理</a></li></ul>");
	

    }



    $("footer").append("Copyright© 2019 台北智慧材料股份有限公司，所刊載內容均受著作權法保護.<br> 32544桃園市龍潭區中豐路高平段10巷67號&nbsp;&nbsp;&nbsp;Tel:03-411-6545, Fax:03-411-6546");
    $("nav li.qq").hover(
	   function () {
	       $(this).find("ul.subNav").show();
	   },
	   function () {
	       $("ul.subNav").hide();
	   }
	);

    $("#switch").click(
	   function () {
	       alert('dd');
	       $("#navSwitch").slideToggle();
	   }
	);
});