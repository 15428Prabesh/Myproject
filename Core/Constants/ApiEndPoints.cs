using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Constants
{
    public class ApiEndPoints
    {
        public static string LoginUrl = "/Accounts/authenticate";
        public static string Register = "/Accounts/register";
        public static string GetUsers = "/Accounts";
        public static string ValidateToken = "/Accounts/validate-token";

        public static string EmailValidation="/Accounts/verify-email";
        public static string ForgotPassword= "/Accounts/forgot-password";
        public static string ResetPassword= "/Accounts/reset-password";
        public static string RefreshToken= "/Accounts/refresh-token";
    }
}
