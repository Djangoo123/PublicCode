
namespace HEZK.Server.Services;
public class MockHomomorphicService : IHomomorphicService
{
    public string[] Sum(string[] ciphertexts)
    {
        // mock: just return a placeholder encrypted result
        return new[]{ "ENC_SUM_RESULT" };
    }
}
