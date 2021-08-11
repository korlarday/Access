using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Allprimetech.Interfaces.Models
{
    public class VerificationCode
    {
        [Key]
        public int VerificationCodeID { get; set; }

        [Required]
        public string ApplicationUserId { get; set; }

        [Required]
        public string Token { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; }

        public int RetryCount { get; set; }

        public bool Expired { get; set; }
        public VerificationCode()
        {

        }
        public VerificationCode(string userId, string token)
        {
            ApplicationUserId = userId;
            Token = token;
            CreatedAt = DateTime.UtcNow;
            RetryCount = 0;
            Expired = false;
        }
    }
}
