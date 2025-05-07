namespace ChatApplication.Models.Responses.Common
{
    public enum ErrorCodes
    {
        // ✅ Success Codes
        OK = 200,                     // Successful request
        Created = 201,                // Resource created

        // ❌ Client Error Codes
        BadRequest = 400,            // Bad request, invalid input
        Unauthorized = 401,          // Unauthorized access
        Forbidden = 403,             // Forbidden access
        NotFound = 404,              // Resource not found
        Conflict = 409,              // Conflict in the request

        // ⚠️ Server Error Codes
        InternalServerError = 500,   // Generic internal server error
        NotImplemented = 501,        // Method not implemented
        BadGateway = 502,            // Bad Gateway error
        ServiceUnavailable = 503,    // Service unavailable

        // 🔐 Authentication & Authorization
        Login_Success = 1000,
        Login_Failed = 401,
        Login_Requires2FA = 1002,
        Login_AccountLocked = 1003,
        Login_AccountNotConfirmed = 1004,
        Login_PasswordIncorrect = 1005,

        // 🔑 Registration
        Registration_Success = 1100,
        Registration_UserAlreadyExists = 1101,
        Registration_EmailAlreadyTaken = 1102,

        // 📩 OTP Handling
        OTP_Sent = 1200,
        OTP_Failed_ToSend=1206,
        OTP_Resent = 1201,
        OTP_Verified = 1202,
        OTP_Invalid = 1203,
        OTP_Expired = 1204,
        OTP_VerificationFailed = 1205,

        // 🔁 Password Reset
        PasswordReset_TokenGenerated = 1300,
        PasswordReset_TokenInvalid = 1301,
        PasswordReset_TokenExpired = 1302,
        PasswordReset_Success = 1303,
        PasswordReset_Failed = 1304,
        PasswordReset_Mismatch = 401,

        // 🔐 Token Handling
        Token_Generated = 1600,
        Token_GenerationFailed = 1601,
        Token_Invalid = 1602,
        Token_Expired = 1603,
        Token_Revoked = 1604,
        Token_Missing = 1605,
        // 🔒 Account Lock/Unlock
        Account_Locked = 1400,
        Account_Unlocked = 1401,
        Account_AlreadyLocked = 1402,
        Account_AlreadyUnlocked = 1403,
        Account_Active = 1404,
        ResetLinkSent = 3000,   // New error code for reset link sent
        ResetLinkExpired = 3001, // Error code when the reset link expires
        ResetLinkInvalid = 3002, // Error code for invalid reset link
        // 👤 User Management
        User_Found = 1700,
        User_NotFound = 1701,
        User_Deleted = 1702,
        User_UpdateSuccess = 1703,
        // ❓ Misc
        Operation_Failed = 1500,
        Operation_Timeout = 1501,
        Unknown_Error = 1502
    }
}
