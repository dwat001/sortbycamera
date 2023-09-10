using MetadataExtractor;
using MetadataExtractor.Formats.Exif;

namespace SortByCamera.Lib;
public class SortByCameraService
{
    private readonly TextWriter _log;

    static readonly string[] ValidExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".bmp", ".tiff" };

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
            if (ValidExtensions.Contains(imageFile.Extension.ToLower()) == false)
            {
                _log.WriteLine($"Skipping {imageFile.FullName} as it is not a valid image file");
                continue;
            }

            string? cameraDescription = GetCameraDescription(imageFile);

            if (cameraDescription == null)
            {
                _log.WriteLine($"Skipping {imageFile.FullName} as it does not have camera metadata");
                continue;
            }

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

    static string? GetCameraDescription(FileInfo imageFile)
    {
        using var inputStream = imageFile.OpenRead();
        var exifDirectories = ImageMetadataReader.ReadMetadata(inputStream)
            .OfType<ExifIfd0Directory>();
        foreach (var exif in exifDirectories)
        {
            var model = exif.GetDescription(ExifDirectoryBase.TagModel) ?? "Unknown";
            return model;
        }

        return null;
    }
}
