using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace backend.Models;

public class FileData
{
    public string Name { get; set; }
    public byte[] File { get; set; }

    public Image<Rgb24> AsImage()
    {
        if (File == null || File.Length == 0)
            throw new InvalidDataException("Invalid file.");

        using var ms = new MemoryStream(File);
        return Image.Load<Rgb24>(ms);
    }
}