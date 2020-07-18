namespace Cashback.Models.ViewModel
{
    public class UserViewModel 
    {   
        public UserViewModel(User user)
        {
            Id = user.Id;
            Name = user.Name;
            Cpf = user.Cpf;
            Email = user.Email;
            Role = user.Role;
        } 

        public string Id { get; set; }
        public string Name { get; set; }
        public string Cpf { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }
    }

    public class ChangePasswordViewmodel
    {
        public string CurrentPassword { get; set; }
        public string NewPassword { get; set; }
        public string ConfirmNewPassword { get; set; }
    }    
}