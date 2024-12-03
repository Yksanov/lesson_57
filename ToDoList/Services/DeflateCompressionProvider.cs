using System.IO.Compression;
using Microsoft.AspNetCore.ResponseCompression;

namespace ToDoList.Services;

public class DeflateCompressionProvider :  ICompressionProvider
{
    public Stream CreateStream(Stream outputStream)
    {
        return new DeflateStream(outputStream, CompressionLevel.Optimal);
    }

    public string EncodingName => "deflate";
    public bool SupportsFlush => true;
}