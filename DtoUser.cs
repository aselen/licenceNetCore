using Newtonsoft.Json;

namespace licenseDemoNetCore
{
    public class DtoUser
    {
        public string name { get; set; }
        public string surname { get; set; }
        public string authToken { get; set; }
        public string refreshToken { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string username { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string password { get; set; }
    }
}