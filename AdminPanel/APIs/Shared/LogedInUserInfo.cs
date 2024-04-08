using System;
namespace AdminPanel.APIs.Shared
{
    public record LogedInUserInfo
    {
        public string token { get; set; } = String.Empty;
        public DateTime expiration { get; set; }
        public string Id { get; set; } = String.Empty;
        public string UserName { get; set; } = String.Empty;
        public string Email { get; set; } = String.Empty;
    }
}

