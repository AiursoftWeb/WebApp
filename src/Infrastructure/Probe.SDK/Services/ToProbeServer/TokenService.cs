﻿using Aiursoft.Handler.Exceptions;
using Aiursoft.Handler.Models;
using Aiursoft.Probe.SDK.Models.TokenAddressModels;
using Aiursoft.Scanner.Interfaces;
using Aiursoft.XelNaga.Models;
using Aiursoft.XelNaga.Services;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace Aiursoft.Probe.SDK.Services.ToProbeServer
{
    public class TokenService : IScopedDependency
    {
        private readonly HTTPService _http;
        private readonly ProbeLocator _serviceLocation;

        public TokenService(
            HTTPService http,
            ProbeLocator serviceLocation)
        {
            _http = http;
            _serviceLocation = serviceLocation;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="accessToken"></param>
        /// <param name="siteName"></param>
        /// <param name="permissions">Upload, Download</param>
        /// <param name="underPath"></param>
        /// <returns></returns>
        public async Task<string> GetTokenAsync(string accessToken, string siteName, string[] permissions, string underPath)
        {
            var url = new AiurUrl(_serviceLocation.Endpoint, "Token", "GetToken", new { });
            var form = new AiurUrl(string.Empty, new GetUploadTokenAddressModel
            {
                AccessToken = accessToken,
                SiteName = siteName,
                Permissions = string.Join(",", permissions),
                UnderPath = underPath
            });
            var result = await _http.Post(url, form, true);
            var jResult = JsonConvert.DeserializeObject<AiurValue<string>>(result);
            if (jResult.Code != ErrorType.Success)
                throw new AiurUnexceptedResponse(jResult);
            return jResult.Value;
        }
    }
}
