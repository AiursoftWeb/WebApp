﻿using Aiursoft.XelNaga.Services.Authentication;
using Newtonsoft.Json;

namespace Aiursoft.Pylon.Services.Authentication.ToFaceBookServer
{
    public class FaceBookUserDetail : IUserDetail
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("avatar_url")]
        public string AvatarUrl { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("email")]
        public string Email { get; set; }

        [JsonProperty("bio")]
        public string Bio { get; set; } = "";
    }
}
