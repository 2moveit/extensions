<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>
<%@ Import Namespace="Signum.Web" %>
<%@ Import Namespace="Signum.Engine" %>
<%@ Import Namespace="Signum.Entities" %>
<%@ Import Namespace="Signum.Utilities" %>
<%@ Import Namespace="Signum.Test" %>

<%
using (var e = Html.TypeContext<GrammyAwardDN>()) 
{
	Html.ValueLine(e, f => f.Year);
	Html.ValueLine(e, f => f.Category);
	Html.ValueLine(e, f => f.Result);
}
%>