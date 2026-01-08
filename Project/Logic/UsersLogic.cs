public class UsersLogic
{
    private UsersAccess _usersAccess = new UsersAccess();

    public bool CreateUser(string firstName, string lastName, string phoneNumber, string emailAddress, string password, string roles)
    {
        // Check if user already exists
        var existingUser = _usersAccess.GetByEmail(emailAddress);
        if (existingUser != null)
        {
            return false; // User already exists
        }

        var newUser = new UserModel
        {
            FirstName = firstName,
            LastName = lastName,
            PhoneNumber = phoneNumber,
            EmailAddress = emailAddress,
            Password = password,
            Roles = roles
        };

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
