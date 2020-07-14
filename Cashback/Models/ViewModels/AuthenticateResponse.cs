namespace Cashback.Models.ViewModel
{
    public class AuthenticateResponse
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Cpf { get; set; }
        public string Email { get; set; }
        public string Token { get; set; }


        public AuthenticateResponse(User user, string token)
        {
            Id = user.Id;
            Name = user.Name;
            Cpf = user.Cpf;
            Email = user.Email;
            Token = token;
        }
    }
}