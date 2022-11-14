using System.Security.Cryptography;
using DirTrackerBL.Interfaces;

namespace DirTrackerBL.Services;

public class HashService : IHashService
{
    private readonly HashAlgorithm _hasher;

    public HashService()
    {
        _hasher = SHA512.Create();
    }

    public async Task<byte[]> HashStream(FileStream fileContent)
    {
        var hashedBytes = await _hasher.ComputeHashAsync(fileContent);
        return hashedBytes;
    }
}