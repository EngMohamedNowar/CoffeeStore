namespace ECommerce.Application.Common
{
    public record Error(string code,string description,ErrorType ErrorType = ErrorType.Failure)
    {
        public static Error Failure(string code = "General.Failure", string description = "General Failure error has occurred") => new(code, description, ErrorType.Failure);
        public static Error Validation(string code = "General.Validation", string description = "General Validation error has occurred") => new(code, description, ErrorType.Validation);
        public static Error NotFound(string code = "General.NotFound", string description = "General NotFound error has occurred") => new(code, description, ErrorType.NotFound);
        public static Error Conflict(string code = "General.Conflict", string description = "General Conflict error has occurred") => new(code, description, ErrorType.Conflict);
        public static Error Unauthorized(string code = "General.Unauthorized", string description = "General Unauthorized error has occurred") => new(code, description, ErrorType.Unauthorized);
        public static Error Forbiden(string code = "General.Forbiden", string description = "General Forbiden error has occurred") => new(code, description, ErrorType.Forbiden);
        public static Error InvalidCredentials(string code = "General. InvalidCredentials", string description = "General  InvalidCredentials error has occurred") => new(code, description, ErrorType.InvalidCredentials);



    }


    public enum ErrorType
    {
        Failure = 0,
        Validation = 1,
        NotFound = 2,
        Conflict = 3,
        Unauthorized = 4,
        Forbiden = 5,
        InvalidCredentials = 6
    }
}