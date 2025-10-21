namespace SoundMatchAPI.Data.AuthModels
{
    public class AuthResult
    {
        public bool Succeeded;
        public List<string>? Errors;
        public string? Token;
        public string? UserId;
    }
}
