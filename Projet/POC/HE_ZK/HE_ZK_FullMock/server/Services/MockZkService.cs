
namespace HEZK.Server.Services;
public class MockZkService : IZkService
{
    public bool Verify(string[] ciphertext, string proof)
    {
        return proof == "VALID_PROOF";
    }
}
