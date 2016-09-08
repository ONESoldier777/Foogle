<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="Foogle.Default" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <link rel="stylesheet" href="http://www.w3schools.com/lib/w3.css">
    <script src="Scripts/jquery-1.6.4.js"></script>
    <script src="Scripts/jquery.signalR-2.2.1.js"></script>
    <script src='<%: ResolveClientUrl("~/signalr/hubs") %>'></script>
    <script type="text/javascript">
        function openTab(tabName) {
            var i;
            var x = document.getElementsByClassName("tab");
            for (i = 0; i < x.length; i++) {
                x[i].style.display = "none";
            }
            document.getElementById(tabName).style.display = "block";
        }
        function ClearIndex() {
            $('#indexBox').val('');
        }
        $(function () {
            openTab("indexTab")
            var hub = $.connection.foogleHub;
            hub.client.hello = function () {
                $('#lblMessage').html('hello');
            };
            hub.client.clearIndexMessages = function (text) {
                $('#lblIndexMessage').html('');
            };
            hub.client.sendIndexMessage = function (text) {
                $('#lblIndexMessage').html($('#lblIndexMessage').html() + "<br />" + text);
            };
            hub.client.clearSearchMessages = function (text) {
                $('#lblSearchMessage').html('');
            };
            hub.client.sendSearchMessage = function (text) {
                $('#lblSearchMessage').html($('#lblSearchMessage').html() + "<br />" + text);
            };
            $.connection.hub.start().done(function () {
                $('#indexButton').click(function () {
                    hub.server.indexPage($('#indexBox').val());
                });
                $('#searchButton').click(function () {
                    hub.server.searchInterwebs($('#searchBox').val());
                });
            })
        });
    </script>
</head>
<body>
    <form id="form1" runat="server">
        <br />
        <center>
        <img src="Images/Foogle.png" />
            <br />
            <div style="width:157px;">
                <ul class="w3-navbar w3-black">
                    <li><a href="#" onclick="openTab('indexTab')">Index</a></li>
                    <li><a href="#" onclick="openTab('searchTab')">Search</a></li>
                </ul>
            </div>
            <br />
            <br />
        <div id="indexTab" class="tab">
            <h2>Index</h2>
            <p>
                <input type="text" id="indexBox" name="indexBox" value="" />
                <input type="button" id="indexButton" name="indexButton" value="Index" />
                <input type="button" name="clearButton" value="Clear" onclick="ClearIndex();" />
            </p>
            <div runat="server" id="lblIndexMessage"></div>
        </div>
        <div id="searchTab" class="tab">
            <h2>Search</h2>
            <p>
                <input type="text" id="searchBox" name="searchBox" value="" />
                <input type="button" id="searchButton" name="searchButton" value="Search" />
            </p>
            <div runat="server" id="lblSearchMessage"></div>
        </div>
            </center>
    </form>
</body>
</html>
