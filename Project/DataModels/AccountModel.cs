public class AccountModel
{
    public Int64 Id { get; set; }
    public string EmailAddress { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;

    public AccountModel()
    {
    }

    public AccountModel(Int64 id, string email, string password, string fullname)
    {
        Id = id;
        EmailAddress = email;
        Password = password;
        FullName = fullname;
    }
}



