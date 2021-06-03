using Spectre.Console;

namespace GitGudCLI.Utils
{
    public static class SpectreHelper
    {
        public static void WriteError(string message)
            => AnsiConsole.MarkupLine($"[red]{Markup.Escape(message)}[/]");
        
        public static void WriteWarning(string message)
            => AnsiConsole.MarkupLine($"[yellow]{Markup.Escape(message)}[/]");
        
        public static void WriteSuccess(string message)
            => AnsiConsole.MarkupLine($"[green]{Markup.Escape(message)}[/]");

        public static void WriteInfo(string message)
            => AnsiConsole.MarkupLine($"[blue]{Markup.Escape(message)}[/]");

        public static void WriteWrappedHeader(string message)
        {
            var panel = new Panel(message) {Border = BoxBorder.Rounded};
            AnsiConsole.Render(panel);
        }

    }
}