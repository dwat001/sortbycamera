using System.IO;
using MetadataExtractor;
using MetadataExtractor.Formats.Exif;

namespace SortByCamera.Lib;
public class SortByCameraService
{
    private readonly TextWriter _log;

    public SortByCameraService(TextWriter log)
    {
        _log = log;
    }

    public void SortByCamera(DirectoryInfo source)
    {
        _log.WriteLine($"Sorting images in {source.FullName}");

        var subDirectories = new Dictionary<string, DirectoryInfo>();
        foreach (FileInfo imageFile in source.GetFiles())
        {
            using var inputStream = imageFile.OpenRead();
            string cameraDescription = GetCameraDescription(inputStream);
            if (subDirectories.ContainsKey(cameraDescription) == false)
            {
                var existingDirectory = source.GetDirectories(cameraDescription).FirstOrDefault();
                if(existingDirectory == null)
                {
                    _log.WriteLine($"Creating directory {cameraDescription}");
                    existingDirectory = source.CreateSubdirectory(cameraDescription);
                }
                subDirectories.Add(cameraDescription, existingDirectory);
            }
            var destination = subDirectories[cameraDescription];
            var destinationPath = Path.Combine(destination.FullName, imageFile.Name);

            _log.WriteLine($"Moving {imageFile.FullName} to {destinationPath}");
            imageFile.MoveTo(destinationPath);
        }
    }

    static string GetCameraDescription(FileStream inputStream)
    {
        var exifDirectories = ImageMetadataReader.ReadMetadata(inputStream)
            .OfType<ExifSubIfdDirectory>();
        foreach (var exif in exifDirectories)
        {
            var serial = exif.GetDescription(ExifDirectoryBase.TagBodySerialNumber) ?? "Unknown";
            var model = exif.GetDescription(ExifDirectoryBase.TagModel) ?? "Unknown";
            return $"{model} - {serial}";
        }

        return $"UNKNOWN";
    }
}
