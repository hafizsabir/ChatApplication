using System.ComponentModel;

namespace ChatApplication.Models.Responses.Common
{
    public enum ResponseMessages
    {
        // ✅ General 
        [Description("File is not uploaded")]
        NoFileUploaded,
        [Description("User Updated SuccessFully")]
        UserUpdatedSuccessFully,
        [Description("User Failed To Update")]
        UserFailedToUpdate,
        [Description("User Deleted SuccessFully")]
        UserDeletedSuccessfully,
        [Description("User Failed to Delete")]
        UserFailedToDelete,
        [Description("Role Assigned Successfully")]
        RoleAssignedSuccessFully,
        [Description("Role Assignment failed")]
        RoleAsignmentFailed,
        [Description("All Messages Retrieved SuccessFully")]
        MessagesRetrievedSuccessFully,
        [Description("Messages Failed to Retrieved")]
        MessagesFailedToRetrieved,
        [Description("Sender  or Reciever Id Are Missing")]
        IdsAreMissing,
        [Description("Token Is Missing")]
        TokenIsMissing,
        [Description("Message is SuccessFully Sent")]
        MessageSentSuccessFully,
        [Description("Message is Failed To Send")]
        MessageFailedToSend,
        [Description("Error While Sending the Message ")]
        MesgSendingFailed,
        [Description("Chat Data Is Saved Successfully")]
        ChatDataSaveSuccessfully,
        [Description("User Verified Sycesfully")]
        UserVerifiedSuccessFully,
        [Description("User Verification Failed")]
        UserVerificationFailed,
        [Description("Token is failed to Validate")]
        TokenFailedToValidate,
        [Description("Token failed to Validate")]
        TokenValidateSuccessfully,
        [Description("Email not Found in Token")]
        EmailNotFoundInToken,
        [Description("Token is Not Refreshed")]
        TokenFailedToRefreshed,
        [Description("Token is Refreshed Successfully")]
        TokenRefreshedSuccessfully,
        [Description("Profile is Updated Successfully")]
        ProfileUpdated,
        [Description("Profile is not Updated")]
        ProfileUpdatedFailed,
        [Description("Profile is not Updated")]
        ProfileNotUpdated,
        [Description("Token is InValid")]
        TokenInvalid,
        [Description("Profile Picture Not Found.")]
        ProfilePictureNotFound,
        [Description("User Profile Picture Retrieved Successfully.")]
        ProfilePictureFound,
        [Description("All users Retrieved Successfully.")]
        AllUsersRetrieved,
        [Description("Not Any User Found in database")]
        NotAnyUserFound,
        [Description(" Unauthorized or invalid token.")]
        UnAuthorizedToken,
        [Description("Token Has Expired.")]
        TokenExpired,
        [Description("Authorization Header Is Missing.")]
        AuthorizationHeaderMissing,
        [Description("UserInfo Retrived from Toke Successfully.")]
        UserInforRetrieved,
        [Description("Token Expiration Claim is missing")]
        TokenExpirationCalimMissing,
        [Description("User Created Successfully")]
        UserCreatedSuccessfully,
        [Description("JWt Failed to Decrypted")]
        JWTFailedToDecrypted,
        [Description("Token is Not Found")]
        PayloadNotFound,
       
        [Description("Server Error JWT")]
        InternalServerErrorJWT,
        [Description("Failed to Retreived Expiry")]
        FailedtoRetrievedExpiry,
        [Description("Token decrypted successfully ")]
        jwtDataretrievedSuccessfully,


        [Description("Token Generated Successfully")]
        TokenGeneratedSuccessFully,

        [Description("Token Failed to Generate")]
        TokenFailedToGenerate,

        [Description("Internal Server Error")]
        InternalServerError,

        [Description("Server Error")]
        ServerError,
        [Description("Password is InCorrect")]
        PasswordIncorrect,

        [Description("Operation Failed")]
        OperationFailed,

        [Description("Unknown Error Occurred")]
        UnknownError,

        // 🔐 Authentication
        [Description("Login Successful")]
        LoginSuccessfull,

        [Description("Login Failed")]
        LoginFailed,

        [Description("Account Locked")]
        AccountLocked,

        [Description("Account is Locked Due to Multiple Attempts")]
        AccountLockedMultipleAttempts,

        [Description("Account is Locked. Try Again after 5 Minutes")]
        AccountIsLocked,

        [Description("Account is Active")]
        AccountIsActive,

        [Description("Account Unlocked Successfully")]
        AccountUnlocked,

        [Description("Account Already Locked")]
        AccountAlreadyLocked,

        [Description("Account Already Unlocked")]
        AccountAlreadyUnlocked,

        // 🔑 Registration
        [Description("Successfully Registered")]
        SuccessfullyRegistered,

        [Description("Registration Failed")]
        RegistrationFailed2,

        [Description("User Already Exists")]
        UserAlreadyExists,

        [Description("Email Already Taken")]
        EmailAlreadyTaken,

        // 📩 OTP
        [Description("OTP Sent")]
        OTPSent,

        [Description("OTP Resent")]
        OTPResent,

        [Description("OTP Verified Successfully and Token Generated")]
        OTPVerifiedAndTokenGenerated,

        [Description("OTP Verification Failed")]
        OTPVerificationFailed,

        [Description("OTP Invalid")]
        InValidOTP,

        [Description("OTP Expired")]
        OTPExpired,

        [Description("OTP Failed to Send")]
        OTPFailedToSend,

        // 🔁 Password Reset
        [Description("Password Reset Requested")]
        PasswordResetRequested,

        [Description("Reset Link Sent Successfully")]
        ResetLinkSentSuccessfully,

        [Description("Reset Link Failed to Send")]
        ResetLinkFailedTo_Sent,

        [Description("Password Reset Successfully")]
        PAssword_Reset_Successfully,

        [Description("Password Failed to Reset")]
        PasswordFailed_To_Reset,

        [Description("Password Confirmation Does Not Match")]
        PasswordMismatch,

        [Description("Reset Token Invalid")]
        ResetTokenInvalid,

        [Description("Reset Token Expired")]
        ResetTokenExpired,

        // 👤 User Status
        [Description("User Not Found")]
        UserNotFound,

        [Description("User Found Successfully")]
        UserFoundSuccessfully,

        [Description("Successfully Updated Profile")]
        SuccessfullyUpdatedProfile,

        [Description("Successfully Deleted User")]
        SuccessfullyDeletedUser,

        // ❌ Credentials
        [Description("Invalid Credentials")]
        InvalidCredentials
    }
   

    // Add implicit conversion to string
  

    //public static class EnumExtensions
    //{
    //    public static string GetDescription(this Enum value)
    //    {
    //        var field = value.GetType().GetField(value.ToString());
    //        var attribute = (DescriptionAttribute)Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute));
    //        return attribute == null ? value.ToString() : attribute.Description;
    //    }

    //}
}
