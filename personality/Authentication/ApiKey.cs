public class ApiKey
{
    public ApiKey(string key, IList<Role> roles) {
        this.key = key;
        this.roles = roles;
    }
    public string key { get; }
    public IList<Role> roles { get; }
}