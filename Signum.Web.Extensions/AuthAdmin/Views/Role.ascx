<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>
<%@ Import Namespace="Signum.Web" %>
<%@ Import Namespace="Signum.Engine" %>
<%@ Import Namespace="Signum.Entities" %>
<%@ Import Namespace="Signum.Entities.Basics" %>
<%@ Import Namespace="Signum.Utilities" %>
<%@ Import Namespace="Signum.Entities.Authorization" %>
<%@ Import Namespace="Signum.Engine.Authorization" %>
<%
    using (var e = Html.TypeContext<RoleDN>())
    {
        Html.ValueLine(e, f => f.Name);
        Html.EntityList(e, f => f.Roles);


        if (BasicPermissions.AdminRules.IsAuthorized() && !e.Value.IsNew)
        {
%>
<ul>
    <%
        if (Navigator.Manager.EntitySettings.ContainsKey(typeof(TypeRulePack)))
        {%>
    <li>
        <%= Html.ActionLink(typeof(TypeDN).NiceName(), "Types", "AuthAdmin", new { role = e.Value.Id }, null)%></li>
    <%}%>
    <%
        if (Navigator.Manager.EntitySettings.ContainsKey(typeof(PermissionRulePack)))
        {%>
    <li>
        <%= Html.ActionLink(typeof(PermissionDN).NiceName(), "Permissions", "AuthAdmin", new { role = e.Value.Id }, null)%></li>
    <%}%>
    <%
        if (Navigator.Manager.EntitySettings.ContainsKey(typeof(FacadeMethodRulePack)))
        {%>
    <li>
        <%= Html.ActionLink(typeof(FacadeMethodDN).NiceName(), "FacadeMethods", "AuthAdmin", new { role = e.Value.Id }, null)%></li>
    <%}%>
    <%
        if (Navigator.Manager.EntitySettings.ContainsKey(typeof(EntityGroupRulePack)))
        {%>
    <li>
        <%= Html.ActionLink(typeof(EntityGroupDN).NiceName(), "EntityGroups", "AuthAdmin", new { role = e.Value.Id }, null)%></li>
    <%}%>
</ul>
<%
        }
    }
%>
