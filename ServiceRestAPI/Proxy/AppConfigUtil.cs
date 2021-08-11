using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Text;

namespace Allprimetech.ServiceRestAPI.Proxy
{
    public class AppConfigUtil
    {
        public static string _CurrentToken = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy5taWNyb3NvZnQuY29tL3dzLzIwMDgvMDYvaWRlbnRpdHkvY2xhaW1zL3JvbGUiOlsiMSIsIjEuMSIsIjEuMiIsIjEuMyIsIjEuNCIsIjIwIiwiMjAuMSIsIjIwLjIiLCIyMC4zIiwiMjAuNCIsIjQwIiwiNDAuMSIsIjQwLjIiLCI0MC4zIiwiNDAuNCIsIjYwIiwiNjAuMSIsIjYwLjIiLCI2MC4zIiwiNjAuNCIsIjgwIiwiODAuMSIsIjgwLjIiLCI4MC4zIiwiODAuNCIsIjEwMCIsIjEwMC4xIiwiMTAwLjIiLCIxMDAuMyIsIjEwMC40IiwiMTIwIiwiMTIwLjEiLCIxMjAuMiIsIjEyMC4zIiwiMTIwLjQiLCIxNTAiLCIxNTAuMSIsIjE1MC4yIiwiMTUwLjMiLCIxNTAuNCIsIjE3MCIsIjE3MC4xIiwiMTcwLjIiLCIxNzAuMyIsIjE3MC40IiwiMjAwIiwiMjAwLjEiLCIyMDAuMiIsIjIwMC4zIiwiMjAwLjQiXSwiVXNlcklkIjoiNTNlYjQyMDctYTM3ZC00ZTY2LTk0YjktNDk1ZjI3YzUxMWY5IiwiVXNlcm5hbWUiOiJhZG1pbiIsIkVtYWlsVmVyaWZpZWQiOiJ2ZXJpZmllZCIsIm5iZiI6MTYxNDcwMTg0OCwiZXhwIjoxNjQ2MjM3ODQ4LCJpc3MiOiJBdWRpYW5jZSJ9.m_EAPFiSmFxMCV49nBEB2mGKieTA4zW6GaCTtjcXuHo";
        public static string _IntegraAdminToken = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy5taWNyb3NvZnQuY29tL3dzLzIwMDgvMDYvaWRlbnRpdHkvY2xhaW1zL3JvbGUiOlsiMSIsIjEuMiIsIjEuMSIsIjEuMyIsIjIwIiwiMjAuMSIsIjIwLjIiLCIyMC4zIiwiNDAiLCI0MC4xIiwiNDAuMiIsIjQwLjMiLCI2MCIsIjYwLjEiLCI2MC4yIiwiNjAuMyIsIjgwIiwiODAuMSIsIjgwLjIiLCI4MC4zIiwiMTAwIiwiMTAwLjEiLCIxMDAuMiIsIjEwMC4zIiwiMTIwIiwiMTQwIiwiMzAwIiwiMzEwIiwiMzIwIiwiMzMwIiwiMzMwLjEiLCIzMzAuMiIsIjQwMCIsIjQwMC4xIiwiNDAwLjIiLCI0MDAuMyIsIjQwMC40IiwiNTAwLjEiLCI1MDAuMiIsIjUwMC4zIiwiNTAwLjQiLCI2MDAuMSIsIjYwMC4yIiwiNjAwLjMiLCI2MDAuNCIsIjcwMC4xIiwiNzAwLjIiLCI3MDAuMyIsIjcwMC40IiwiODAwLjEiLCI4MDAuMiIsIjgwMC4zIiwiODAwLjQiLCI5MDAuMSIsIjkwMC4yIiwiOTAwLjMiLCI5MDAuNCIsIjEwMDAuMSIsIjEwMDAuMiIsIjEwMDAuMyIsIjEwMDAuNCIsIjExMDAuMSIsIjExMDAuMiIsIjExMDAuMyIsIjExMDAuNCIsIjUwMCIsIjYwMCIsIjcwMCIsIjgwMCIsIjkwMCIsIjEwMDAiLCIxMTAwIiwiMS40IiwiMjAuNCIsIjQwLjQiLCI2MC40IiwiODAuNCIsIjEwMC40IiwiMTIwMCIsIjEyMDAuMSIsIjEyMDAuMiIsIjEyMDAuMyIsIjEyMDAuNCIsIjEzMDAiLCIxMzAwLjEiLCIxMzAwLjIiLCIxMzAwLjMiLCIxMzAwLjQiXSwiVXNlcklkIjoiMmExYmEyMDAtMTI0MS00OTBlLWJmMWYtYzQ2NTNmNzY1YWEwIiwiVXNlcm5hbWUiOiJhZG1pbiIsIkFwaVVzZXIiOiIiLCJQYXJ0bmVySWQiOiIxIiwiUGhvdG9QYXRoIjoiMjZjM2RiODAtNDQxNi00ZDgwLWI1NzAtZWQ3Y2IxYTA0OWQzX2F2YXRhci0xMzItNC5qcGciLCJFbWFpbFZlcmlmaWVkIjoidmVyaWZpZWQiLCJuYmYiOjE2MjU5ODQyNTMsImV4cCI6MTgxNTI4NjY1MywiaXNzIjoiQXVkaWFuY2UiLCJhdWQiOiJodHRwOi8vbG9jYWxob3N0OjQyMDAvIn0.-tH69cigEJ4l2eOYOrQLglcUn-jEwTH6LEAinRKZWHk";
        public static int _RequestTimeOut = 0;
        public static string HttpProtocol = "https";
        //public static string BaseResource = $"{HttpProtocol}://localhost:44356/api/";
        public static JsonSerializerSettings Jsonsettings = new JsonSerializerSettings() { DateFormatHandling = DateFormatHandling.MicrosoftDateFormat };

        public static RestRequest AddCustomHeader(RestRequest Req)
        {

            if (!string.IsNullOrEmpty(_CurrentToken))
                Req.AddHeader("authorization", $"Bearer {_CurrentToken}");

            Req.Timeout = _RequestTimeOut;
            return Req;
        }
        public static RestRequest AddIntegraAdminHeader(RestRequest Req)
        {

            if (!string.IsNullOrEmpty(_CurrentToken))
                Req.AddHeader("authorization", $"Bearer {_IntegraAdminToken}");

            Req.Timeout = _RequestTimeOut;
            return Req;
        }
        public static string BaseResource(string url)
        {
            return $"{url}";
            //return $"{url}/api/";
        }
    }
}
