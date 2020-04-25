﻿using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Net;
using Newtonsoft.Json;

namespace SubPixel.Instagram.SDK
{
    public class Client
    {
        public string APIEndpoint { get { return "https://graph.instagram.com/"; } }
        public bool IsProduction { get; internal set; }
        public string AccessToken { get; internal set; }

        public Client(
            string accessToken)
        {
            if(!accessToken.StartsWith("&access_token="))
			    accessToken = "&access_token=" + accessToken;
            AccessToken = accessToken;
        }

        public Models.IGraphResponse DoRequest(string url, Type returnObj)
        {
            if (returnObj.GetInterface("IGraphResponse") == null)
                return null;

            WebClient webClient = new WebClient();

            try
            {
                var json = webClient.DownloadString(url);
                return (Models.IGraphResponse)JsonConvert.DeserializeObject(json, returnObj);
            }
            catch (WebException ex)
            {
                string responseText;
                using (var reader = new StreamReader(ex.Response.GetResponseStream()))
                {
                    responseText = reader.ReadToEnd();
                }

                return JsonConvert.DeserializeObject<Models.ErrorRootObj>(responseText).Error;
            }
            catch (Exception ex)
            {
                dynamic error = new ExpandoObject();
                error.Message = ex.Message;
                return new Models.Error()
                {
                    Code = -1,
                    Message = ex.Message
                };
            }
        }
    }
}
