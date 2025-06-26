<%@ Page  Async="true" Language="C#" AutoEventWireup="true" CodeBehind="ReportViewer.aspx.cs" Inherits="CGHSBilling.Reports.ReportViewer" %>
<%@ Register Assembly="Microsoft.ReportViewer.WebForms, Version=11.0.0.0, Culture=neutral, PublicKeyToken=89845dcd8080cc91" Namespace="Microsoft.Reporting.WebForms" TagPrefix="rsweb" %>
<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <div id="divPartialLoading"></div>
        <div>
            <asp:ScriptManager ID="ScriptManager1" runat="server"></asp:ScriptManager>
            <rsweb:reportviewer id="ReportViewer1" runat="server" asyncrendering="true" sizetoreportcontent="True" width="99%" BorderStyle="None" 
                backcolor="White" font-names="Verdana" font-size="8pt" waitmessagefont-names="Verdana" waitmessagefont-size="14pt">
            </rsweb:reportviewer>   
        </div>
    </form>
</body>

</html>
