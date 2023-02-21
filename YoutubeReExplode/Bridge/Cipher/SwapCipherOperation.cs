using System.Diagnostics.CodeAnalysis;
using YoutubeReExplode.Utils.Extensions;

namespace YoutubeReExplode.Bridge.Cipher;

internal class SwapCipherOperation : ICipherOperation
{
    private readonly int _index;

    public SwapCipherOperation(int index) => _index = index;

    public string Decipher(string input) => input.SwapChars(0, _index);

    [ExcludeFromCodeCoverage]
    public override string ToString() => $"Swap ({_index})";
}