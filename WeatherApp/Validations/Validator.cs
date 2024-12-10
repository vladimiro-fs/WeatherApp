namespace WeatherApp.Validations
{
    using System.Text.RegularExpressions;

    public class Validator : IValidator
    {
        public string NameError { get; set; } = "";

        public string EmailError { get; set; } = "";

        public string PhoneNumberError { get; set; } = "";

        public string PasswordError { get; set; } = "";

        private const string EmptyNameErrorMsg = "Please enter your name";
        private const string InvalidNameErrorMsg = "Please enter a valid name";
        private const string EmptyEmailErrorMsg = "Please enter your email";
        private const string InvalidEmailErrorMsg = "Please enter a valid email";
        private const string EmptyPhoneNumberErrorMsg = "Please enter your phone number";
        private const string InvalidPhoneNumberErrorMsg = "Please enter a valid phone number";
        private const string EmptyPasswordError = "Please enter your password";
        private const string InvalidPasswordError = "The password must contain at least 8 characters, including numbers and letters";


        public Task<bool> Validate(string name, string email, string phoneNumber, string password)
        {
            var isValidName = ValidateName(name);
            var isValidEmail = ValidateEmail(email);
            var isValidPhoneNumber = ValidatePhoneNumber(phoneNumber);
            var isValidPassword = ValidatePassword(password);

            return Task.FromResult(isValidName && isValidEmail && isValidPhoneNumber && isValidPassword);
        }

        private bool ValidateName(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                NameError = EmptyNameErrorMsg;
                return false;
            }

            if (name.Length < 3)
            {
                NameError = InvalidNameErrorMsg;
                return false;
            }

            NameError = "";
            return true;
        }

        private bool ValidateEmail(string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                EmailError = EmptyEmailErrorMsg;
                return false;
            }

            if (!Regex.IsMatch(email, @"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$"))
            {
                EmailError = InvalidEmailErrorMsg;
                return false;
            }

            EmailError = "";
            return true;
        }

        private bool ValidatePhoneNumber(string phoneNumber)
        {
            if (string.IsNullOrEmpty(phoneNumber))
            {
                PhoneNumberError = EmptyPhoneNumberErrorMsg;
                return false;
            }

            if (phoneNumber.Length < 9)
            {
                PhoneNumberError = InvalidPhoneNumberErrorMsg;
                return false;
            }

            PhoneNumberError = "";
            return true;
        }

        private bool ValidatePassword(string password)
        {
            if (string.IsNullOrEmpty(password))
            {
                PasswordError = EmptyPasswordError;
                return false;
            }

            if (password.Length < 8 || !Regex.IsMatch(password, @"[a-zA-Z]") || !Regex.IsMatch(password, @"\d"))
            {
                PasswordError = InvalidPasswordError;
                return false;
            }

            PasswordError = "";
            return true;
        }
    }
}
