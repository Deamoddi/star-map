namespace star_map;

internal class Program
{
    static async Task Main(string[] args)
    {
        // Set console encoding to UTF-8 for Ukrainian Cyrillic and emoji support
        Console.OutputEncoding = System.Text.Encoding.UTF8;
        Console.InputEncoding = System.Text.Encoding.UTF8;

        var application = new Application();
        await application.RunAsync();
    }
}


