namespace ExaminationSystem.Application.Interfaces
{
    public interface ITokenServies
    {
        public Task<string> CreateToken(AppUser appUser);
        public string GenerateRefreshToken();
        public string HashRefreshToken(string rawToken);
    }
}
