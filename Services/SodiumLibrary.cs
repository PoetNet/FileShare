using System.Security.Cryptography;
using System.Runtime.InteropServices;
using System.Text;

namespace FileShare.Services;

public class SodiumLibrary
{
    private const string Name = "libsodium";
    public const int crypto_pwhash_argon2id_ALG_ARGON2ID13 = 2;
    public const long crypto_pwhash_argon2id_OPSLIMIT_SENSETIVE = 4;
    public const int crypto_pwhash_argon2id_MEMLIMIT_SENSETIVE = 1073741824;

    static SodiumLibrary()
    {
        sodium_init();
    }

    [DllImport(Name, CallingConvention = CallingConvention.Cdecl)]
    internal static extern void sodium_init();

    [DllImport(Name, CallingConvention = CallingConvention.Cdecl)]
    internal static extern void randombytes_buf(byte[] buffer, int size);

    [DllImport(Name, CallingConvention = CallingConvention.Cdecl)]
    internal static extern int crypto_pwhash(byte[] buffer, 
                                             long bufferLen, 
                                             byte[] password, 
                                             long passwordLen,
                                             byte[] salt,
                                             long OPSLIMIT_SENSETIVE,
                                             int MEMLIMIT_SENSETIVE,
                                             int ALG_ARGON2ID13);

    public static byte[] CreateSalt()
    {
        var buffer = new byte[16];
        randombytes_buf(buffer, buffer.Length);
        return buffer;
    }

    public static byte[] HashPassword(string password, byte[] salt)
    {
        var hash = new byte[16];

        var result = crypto_pwhash(
            hash,
            hash.Length,
            Encoding.UTF8.GetBytes(password),
            password.Length,
            salt,
            crypto_pwhash_argon2id_OPSLIMIT_SENSETIVE,
            crypto_pwhash_argon2id_MEMLIMIT_SENSETIVE,
            crypto_pwhash_argon2id_ALG_ARGON2ID13
        );

        if (result != 0) throw new Exception("An unexpected error has occured.");

        return hash;
    }    

    public static bool VerifyPassword(string password, byte[] salt, byte[] hashedPassword)
    {
        var generatedHash = HashPassword(password, salt);
        return CompareByteArrays(generatedHash, hashedPassword);
    }

    private static bool CompareByteArrays(byte[] array1, byte[] array2)
    {
        if (array1.Length != array2.Length) return false;
        for (int i = 0; i < array1.Length; i++)
            if (array1[i] != array2[i]) return false;

        return true;
    }
}
