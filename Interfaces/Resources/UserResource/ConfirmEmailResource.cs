using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Allprimetech.Interfaces.Resources
{
    public class VerifyEmailResource
    {
        public string _Code { get; set; }
        public string _UserId { get; set; }
    }

    public class ConfirmEmailResource
    {
        public bool _Succeeded { get; set; }
        public string _Message { get; set; }
    }

    public class ResendVerifyCodeResource
    {
        [Required]
        public string _UserId { get; set; }
    }

    public class ResendVerificationResponse
    {
        public double _MinutesCount { get; set; }
        public bool _Succeeded { get; set; }
        public bool _EmailVerified { get; set; }
        public string _Message { get; set; }
    }
}
