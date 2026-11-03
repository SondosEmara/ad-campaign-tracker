using System.Security.Cryptography;
using ClickToCart.Marketing.Application;

namespace ClickToCart.Marketing.Infrastructure;

public sealed class Base62CodeGenerator : ICodeGenerator
{
    private const string Alphabet = "abcdefghijkmnpqrstuvwxyzABCDEFGHJKLMNPQRSTUVWXYZ23456789";
    private const int Length = 5;

    public string Next() =>
        string.Create(Length, Alphabet, (span, chars) =>
        {
            for (var i = 0; i < span.Length; i++) span[i] = chars[RandomNumberGenerator.GetInt32(chars.Length)];
        });
}
