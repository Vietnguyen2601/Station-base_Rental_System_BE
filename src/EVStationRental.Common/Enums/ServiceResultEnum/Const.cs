using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EVStationRental.Common.Enums.ServiceResultEnum
{
    public class Const
    {
        #region Error Codes
        public const int ERROR_EXCEPTION = 500;        // HTTP 500 Internal Server Error
        public const int ERROR_VALIDATION_CODE = 400;  // HTTP 400 Bad Request
        public const string ERROR_INVALID_DATA_MSG = "Invalid data";
        #endregion

        #region Success Codes

        public const int SUCCESS_CREATE_CODE = 200;    // HTTP 200 OK
        public const string SUCCESS_CREATE_MSG = "Save data success";
        public const int SUCCESS_READ_CODE = 200;      // HTTP 200 OK
        public const string SUCCESS_READ_MSG = "Get data success";
        public const int SUCCESS_UPDATE_CODE = 200;    // HTTP 200 OK
        public const string SUCCESS_UPDATE_MSG = "Update data success";
        public const int SUCCESS_DELETE_CODE = 200;    // HTTP 200 OK
        public const string SUCCESS_DELETE_MSG = "Delete data success";
        public const int SUCCESS_PAYMENT_CODE = 8386;
        public const string SUCCESS_PAYMENT_MSG = "COMPLETED";

        #endregion

        #region Fail Codes

        public const int FAIL_CREATE_CODE = 400;       // HTTP 400 Bad Request
        public const string FAIL_CREATE_MSG = "Save data fail";
        public const int FAIL_READ_CODE = 400;         // HTTP 400 Bad Request
        public const string FAIL_READ_MSG = "Get data fail";
        public const int FAIL_UPDATE_CODE = 400;       // HTTP 400 Bad Request
        public const string FAIL_UPDATE_MSG = "Update data fail";
        public const int FAIL_DELETE_CODE = 400;       // HTTP 400 Bad Request
        public const string FAIL_DELETE_MSG = "Delete data fail";

        #endregion

        #region Warning Code

        public const int WARNING_NO_DATA_CODE = 404;   // HTTP 404 Not Found
        public const string WARNING_NO_DATA_MSG = "No data";
        public const int WARNING_DATA_EXISTED_CODE = 409; // HTTP 409 Conflict
        public const string WARNING_DATA_EXISTED_MSG = "Data already exists";

        #endregion

        #region OTP & Password Reset Codes

        public const int SUCCESS_SEND_OTP_CODE = 200;  // HTTP 200 OK
        public const string SUCCESS_SEND_OTP_MSG = "OTP sent successfully";
        public const int SUCCESS_VERIFY_OTP_CODE = 200;// HTTP 200 OK
        public const string SUCCESS_VERIFY_OTP_MSG = "OTP verified successfully";
        public const int FAIL_VERIFY_OTP_CODE = 400;   // HTTP 400 Bad Request
        public const string FAIL_VERIFY_OTP_MSG = "OTP verification failed";
        public const int SUCCESS_RESET_PASSWORD_CODE = 200; // HTTP 200 OK
        public const string SUCCESS_RESET_PASSWORD_MSG = "Password reset successfully";
        public const int FAIL_RESET_PASSWORD_CODE = 400;// HTTP 400 Bad Request
        public const string FAIL_RESET_PASSWORD_MSG = "Password reset failed";
        public const int FAIL_SEND_OTP_CODE = 400;     // HTTP 400 Bad Request
        public const string FAIL_SEND_OTP_MSG = "Failed to send OTP";
        public const int FAIL_OTP_EXPIRED_CODE = 400;       // HTTP 400 Bad Request
        public const string FAIL_OTP_EXPIRED_MSG = "OTP has expired";
        public const int FAIL_OTP_NOT_FOUND_CODE = 404;      // HTTP 404 Not Found
        public const string FAIL_OTP_NOT_FOUND_MSG = "OTP not found";
        public const string ERROR_OTP = "sent OTP failed.";

        #endregion

        #region Access Codes

        public const int FORBIDDEN_ACCESS_CODE = 403;    // HTTP 403 Forbidden
        public const string FORBIDDEN_ACCESS_MSG = "Access denied";
        public const int UNAUTHORIZED_ACCESS_CODE = 401; // HTTP 401 Unauthorized
        public const string UNAUTHORIZED_ACCESS_MSG = "Unauthorized access";

        #endregion

        #region Token Code

        public const int SUCCESS_GENERATE_TOKEN_CODE = 200; // HTTP 200 OK
        public const string SUCCESS_GENERATE_TOKEN_MSG = "Token generated successfully";
        public const int SUCCESS_VALIDATE_TOKEN_CODE = 200;  // HTTP 200 OK
        public const string SUCCESS_VALIDATE_TOKEN_MSG = "Token is valid";
        public const int FAIL_GENERATE_TOKEN_CODE = 400;    // HTTP 400 Bad Request
        public const string FAIL_GENERATE_TOKEN_MSG = "Token generation failed";
        public const int EXPIRED_TOKEN_CODE = 401;           // HTTP 401 Unauthorized
        public const string EXPIRED_TOKEN_MSG = "Token has expired";
        public const int INVALID_TOKEN_CODE = 400;           // HTTP 400 Bad Request
        public const string INVALID_TOKEN_MSG = "Invalid token algorithm";
        public const int INVALID_TOKEN_SIGNATURE_CODE = 401; // HTTP 401 Unauthorized
        public const string INVALID_TOKEN_SIGNATURE_MSG = "Invalid token signature";
        public const int FAIL_VALIDATE_TOKEN_CODE = 400;     // HTTP 400 Bad Request
        public const string FAIL_VALIDATE_TOKEN_MSG = "Token validation failed";

        #endregion

        #region Authentication Code

        public const int SUCCESS_LOGIN_CODE = 200;     // HTTP 200 OK
        public const string SUCCESS_LOGIN_MSG = "Login success";
        public const int SUCCESS_LOGIN_GOOGLE_CODE = 200;
        public const string SUCCESS_LOGIN_GOOGLE_MSG = "Login Google success";
        public const string SUCCESS_LOGOUT_MSG = "Logout success";
        public const int FAIL_VALIDATE_CODE = 400;
        public const string FAIL_VALIDATE_MSG = "Validation failed";
        public const int SUCCESS_REGISTER_CODE = 201; // HTTP 201 Created
        public const string SUCCESS_REGISTER_MSG = "Register success";

        public const string SUCCESS_CHANGE_PASSWORD_MSG = "Change password successfully";
        public const string ERROR_USERNAME_EXISTS_MSG = "Username already exists";
        public const string ERROR_EMAIL_EXISTS_MSG = "Email already exists";
        public const string ERROR_PHONE_EXISTS_MSG = "Phone number already exists";
        public const string FAIL_LOGOUT_MSG = "Logout fail";

        #endregion

        #region Payment Status
        public const string PAYMENT_STATUS_PENDING = "PENDING";
        public const string PAYMENT_STATUS_COMPLETED = "COMPLETED";
        public const string PAYMENT_STATUS_FAILED = "FAILED";
        public const string PAYMENT_STATUS_REFUNDED = "REFUNDED";

        #endregion

        #region Order Status
        public const string ORDER_STATUS_PENDING = "PENDING";
        public const string ORDER_STATUS_CONFIRMED = "CONFIRMED";
        public const string ORDER_STATUS_IN_PROGRESS = "IN_PROGRESS";
        public const string ORDER_STATUS_COMPLETED = "COMPLETED";
        public const string ORDER_STATUS_CANCELLED = "CANCELLED";

        #endregion

        #region Station Status
        public const string STATION_STATUS_AVAILABLE = "AVAILABLE";
        public const string STATION_STATUS_IN_USE = "IN_USE";
        public const string STATION_STATUS_MAINTENANCE = "MAINTENANCE";
        public const string STATION_STATUS_UNAVAILABLE = "UNAVAILABLE";

        #endregion

        #region Vehicle Status
        public const string VEHICLE_STATUS_AVAILABLE = "AVAILABLE";
        public const string VEHICLE_STATUS_RENTED = "RENTED";
        public const string VEHICLE_STATUS_MAINTENANCE = "MAINTENANCE";
        public const string VEHICLE_STATUS_UNAVAILABLE = "UNAVAILABLE";

        #endregion

        #region License Status
        public const string LICENSE_STATUS_ACTIVE = "ACTIVE";
        public const string LICENSE_STATUS_EXPIRED = "EXPIRED";
        public const string LICENSE_STATUS_SUSPENDED = "SUSPENDED";

        #endregion

        #region Account Status
        public const string ACCOUNT_STATUS_ACTIVE = "ACTIVE";
        public const string ACCOUNT_STATUS_INACTIVE = "INACTIVE";
        public const string ACCOUNT_STATUS_SUSPENDED = "SUSPENDED";

        #endregion
    }
}