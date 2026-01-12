public class UsersLogic
{
    private UsersAccess _usersAccess = new UsersAccess();

    public bool CreateUser(UserModel newUser)
    {
        // Check if user already exists
        var existingUser = _usersAccess.GetByEmail(newUser.EmailAddress);
        if (existingUser != null)
        {
            return false; // User already exists
        }

        _usersAccess.Write(newUser);
        return true;
    }

    public bool IsValidEmail(string email)
    {
        return email.Contains("@") && email.Contains(".");
    }

    public bool IsValidPhoneNumber(string phoneNumber)
    {
        return phoneNumber.Length >= 10 && phoneNumber.All(char.IsDigit);
    }

    public UserModel? CheckLogin(string email, string password)
    {
        var user = _usersAccess.GetByEmail(email);
        if (user != null && user.Password == password)
        {
            return user;
        }
        return null;
    }
    public int GetIdByEmail(string email)
    {
        int id = _usersAccess.GetIdByEmail(email);
        if (id != 0 )
        {
            return id;
        }
        return 0;
    }
}
