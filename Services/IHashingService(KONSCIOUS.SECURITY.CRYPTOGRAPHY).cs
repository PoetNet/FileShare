// using System.Security.Cryptography;

// namespace FileShare.Services;

// public interface IHashingService
// {
//     public byte[] CreateSalt();
//     public byte[] HashPassword(string password, byte[] salt);
//     public bool VerifyHash(string password, byte[] salt, byte[] hash);
// }

// public class HashingService
// {
//     public byte[] CreateSalt()
//     {
//         var buffer = new byte[16];
//         var rng = new RNGCryptographyServiceProvider();
//     }
// }