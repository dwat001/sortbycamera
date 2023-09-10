using System.CommandLine;
using System.CommandLine.NamingConventionBinder;
using SortByCamera.Lib;

namespace SortByCamera.Cli;


public class SortByCameraCli
{
    public static void Main(string[] args)
    {
        try
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
            rootCommand.Invoke(args);
        }catch(Exception ex)
        {
            Console.WriteLine("Well something went wrong, here is an unhelpful error message");
            Console.WriteLine(ex.Message);
            Console.WriteLine();
            Console.WriteLine("Sorry that did not work, the above mess would tell a programmer what happened.");
        }

    }

    static void SortByCamera(DirectoryInfo source)
    {
        
        var service = new SortByCameraService(Console.Out);
        service.SortByCamera(source);
    }
}
