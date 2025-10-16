public interface IAuthService
{
    Task<bool> IsAuthenticated();
}
public class AuthService : IAuthService
{
    private readonly ISession _session;

    public AuthService(ISession session)
    {
        _session = session;
    }

    public Task<bool> IsAuthenticated()
    {
        // Implement your authentication logic here
        return Task.FromResult(false);
    }
}
