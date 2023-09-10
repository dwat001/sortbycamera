using System.CommandLine;
using System.CommandLine.NamingConventionBinder;
using SortByCamera.Lib;

namespace SortByCamera.Cli;


public class SortByCameraCli
{
    public static void Main(string[] args)
    {
        var rootCommand = new RootCommand
        {
            new Option<DirectoryInfo>(name: "--source",
                description: "The source directory to process images",
                getDefaultValue: () => new DirectoryInfo(Directory.GetCurrentDirectory())
            ).ExistingOnly()
        };
        
        rootCommand.Description = "Sorts images into folders based on the camera they were taken with";
        
        rootCommand.Handler = CommandHandler.Create<DirectoryInfo>(SortByCamera);
        
    }

    static void SortByCamera(DirectoryInfo source)
    {
        
        var service = new SortByCameraService(Console.Out);
        service.SortByCamera(source);
    }
}
