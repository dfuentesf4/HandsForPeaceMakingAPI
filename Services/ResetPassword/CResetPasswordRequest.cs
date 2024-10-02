namespace HandsForPeaceMakingAPI.Services.ResetPassword
{
    public class ResetPasswordRequest
    {
        public string UserName { get; set; }
        public string NewPassword { get; set; }
        public string Token { get; set; }
    }
}
