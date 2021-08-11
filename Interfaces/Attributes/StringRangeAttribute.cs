using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace Allprimetech.Interfaces.Attributes
{
    public class StringRangeAttribute : ValidationAttribute
    {
        public string[] AllowableValues { get; set; }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (AllowableValues?.Contains(value?.ToString()) == true)
            {
                return ValidationResult.Success;
            }

            var msg = $"Please enter one of the allowable values: {string.Join(", ", AllowableValues ?? new string[] { "No allowable values found" })}.";
            return new ValidationResult(msg);
        }
    }

    public class IntRangeAttribute : ValidationAttribute
    {
        public int[] AllowableValues { get; set; }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (IsInt(value))
            {
                if (AllowableValues.Contains(Convert.ToInt32(value)))
                {
                    return ValidationResult.Success;
                }
            }

            var msg = $"Invalid value";
            return new ValidationResult(msg);
        }

        public bool IsInt(object value)
        {
            if(value is int)
            {
                return true;
            }
            return false;
        }
    }
}
