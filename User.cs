using System;

namespace backend
{
    public class User
    {
        public int id { get; set; }
        public string username { get; set; }
        public string password { get; set; }
        public string name { get; set; }
        public string surname { get; set; }
        public string authToken { get; set; }
        public DateTime authTokenExpireTime { get; set; }
        public string refreshToken { get; set; }
        public DateTime? RefreshTokenEndDate { get; set; }
        public bool isActive { get; set; }
    }
}