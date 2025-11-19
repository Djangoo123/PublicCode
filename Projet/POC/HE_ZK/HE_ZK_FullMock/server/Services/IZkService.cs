
namespace HEZK.Server.Services;
public interface IZkService
{
    bool Verify(string[] ciphertext, string proof);
}
