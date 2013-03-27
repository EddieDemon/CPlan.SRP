<%@ Page Title="Home Page" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true"
    CodeBehind="Default.aspx.cs" Inherits="CPlan.SRP.Web._Default" %>

<%@ Import Namespace="CPlan.SRP.Web" %>
<%@ Import Namespace="System.Linq" %>
<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
    <!--
      -- Connection Planet - SRP6a Implementation
      -- Copyright (C) 2013  MusicDemon
      -- 
      -- This program is free software: you can redistribute it and/or modify
      -- it under the terms of the GNU General Public License as published by
      -- the Free Software Foundation, either version 3 of the License, or
      -- (at your option) any later version.
      -- 
      -- This program is distributed in the hope that it will be useful,
      -- but WITHOUT ANY WARRANTY; without even the implied warranty of
      -- MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
      -- GNU General Public License for more details.
      -- 
      -- You should have received a copy of the GNU General Public License
      -- along with this program.  If not, see <http://www.gnu.org/licenses/>.
      --
      --
      -->
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">
    <h2>
        Create user / Log in.
    </h2>
    <div>
        <p>
            <a id="save" href="#">Save</a></p>
    </div>
    <script type="text/javascript">
        $("#save").click(function () {
            $u.crypto.srp.initialize("<%: Global.Hex(Global.bis.N) %>", "<%: Global.Hex(Global.bis.g) %>", "<%: Global.Hex(Global.bis.k) %>");
            $u.crypto.srp.demo();
        });
        
    </script>
</asp:Content>
