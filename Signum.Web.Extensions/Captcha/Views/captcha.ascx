﻿<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>
<%@ Import Namespace="Signum.Web" %>
<%@ Import Namespace="Signum.Web.Captcha" %>

<script type="text/javascript">
    function solicitarCaptcha() {
        $('#ajax-loader').show();
        $.ajax({
            type: "POST",
            url: "Captcha.ashx/Refresh",
            data: "",
            async: false,
            dataType: "html",
            success:
                   function(msg) {
                       $("#divCaptchaImage").html(msg);
                       $('#ajax-loader').hide();
                   },
            error:
                   function(XMLHttpRequest, textStatus, errorThrown) {
                       $('#ajax-loader').hide();
                       ShowError(XMLHttpRequest, textStatus, errorThrown);
                   }
        });
    }
</script>

<div id="divCaptcha">
    <label class="labelLine">Escriba estos caracteres:</label>
    <div id="divCaptchaImage">
        <% Html.RenderPartial(CaptchaClient.CaptchaImageUrl); %>
    </div>
    <%= Html.Href("solicitarNuevoCaptcha", "Solicite un nuevo código", "javascript:solicitarCaptcha();", "Solicite un nuevo código", null, new Dictionary<string, object> { {"style", "float:left" } })%>
    <div class="clearall"></div>
    <label class="labelLine" for="sfCaptcha">Aquí:</label>
    <%= Html.TextBox("sfCaptcha", null, new {autocomplete="off"})%>
</div>