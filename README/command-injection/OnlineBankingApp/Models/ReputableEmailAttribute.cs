using System;
using System.Linq;
using System.ComponentModel.DataAnnotations;

namespace OnlineBankingApp.Models
{
    public class ReputableEmailAttribute : ValidationAttribute
    {
        private static readonly string[] ShadyTlds =
        {
            "country", "kim", "science", "gq", "work", "ninja", "xyz", "date", "faith", "zip",
            "racing", "cricket", "win", "space", "accountant", "realtor", "top", "stream",
            "christmas", "gdn", "mom", "pro", "men"
        };

        public string GetErrorMessage() =>
            "Email address is from a shady domain";

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value is not string email || string.IsNullOrWhiteSpace(email))
            {
                return ValidationResult.Success;
            }

            var lastDot = email.LastIndexOf('.');
            if (lastDot < 0 || lastDot == email.Length - 1)
            {
                return ValidationResult.Success;
            }

            var tld = email[(lastDot + 1)..].ToLowerInvariant();

            if (ShadyTlds.Any(shady => string.Equals(shady, tld, StringComparison.OrdinalIgnoreCase)))
            {
                return new ValidationResult(GetErrorMessage());
            }

            return ValidationResult.Success;
        }
    }

}