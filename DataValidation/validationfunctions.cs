using System.Text.RegularExpressions;

namespace DataValidation
{
    public class MyValidationPlugin : IValidationPlugin
    {
        public Dictionary<string, Func<string, Task<bool>>> GetValidationFunctions()
        {
            return new Dictionary<string, Func<string, Task<bool>>>(StringComparer.OrdinalIgnoreCase)
        {
            { "ValidateEmail", async value => await Task.Run(() => ValidateEmail(value)) },
            { "ValidateRepositoryName", async value => await Task.Run(() => ValidateRepositoryName(value)) },
            { "ValidateContainsSubstring", async value => await Task.Run(() => ValidateContainsSubstring(value, "important")) }
        };
        }

        private static bool ValidateEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email)) return false;

            string emailPattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
            return Regex.IsMatch(email, emailPattern, RegexOptions.IgnoreCase);
        }

        private static bool ValidateRepositoryName(string repoName) =>
            Regex.IsMatch(repoName, "^[a-zA-Z0-9._-]+$");

        private static bool ValidateContainsSubstring(string input, string substring)
        {
            if (string.IsNullOrEmpty(input) || string.IsNullOrEmpty(substring))
                return false;

            return input.Contains(substring, StringComparison.OrdinalIgnoreCase);
        }
    }
}
