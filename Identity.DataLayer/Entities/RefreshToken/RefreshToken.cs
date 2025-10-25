﻿namespace Identity.DataLayer.Entities.RefreshToken
{
    public class RefreshToken
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Token { get; set; }
        public DateTime ExpiresAt { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsRevoked { get; set; }
        public DateTime? RevokedAt { get; set; }
        public string CreatedByIp { get; set; }
        public string? RevokedByIp { get; set; }
    }
}
