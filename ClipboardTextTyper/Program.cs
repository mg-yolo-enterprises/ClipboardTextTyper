using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ClipboardTextTyper
{
    internal class Program
    {
        [STAThread]
        private static void Main()
        {
            Typer();

            Thread.Sleep(TimeSpan.FromSeconds(5));
            Environment.Exit(0);
        }

        private static void Typer()
        {
            if (!Clipboard.ContainsText(TextDataFormat.Text))
            {
                Error("Clipboard doesn't contain text!");
                return;
            }

            var clipboardText = Clipboard.GetText(TextDataFormat.Text);
            if (string.IsNullOrWhiteSpace(clipboardText))
            {
                Error("Clipboard text is blank!");
                return;
            }
            if (clipboardText.Length > 1000)
            {
                Error("Clipboard text has too many characters!");
                return;
            }

            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write("Waiting 5 seconds");
            for (var i = 0; i < 5; i++)
            {
                Thread.Sleep(TimeSpan.FromSeconds(1));
                Console.Write(".");
            }
            Console.WriteLine();
            TypeText(clipboardText);
            Console.WriteLine("Done");

            void Error(string error)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(error);
            }
        }

        private static void TypeText(string clipboardText)
        {
            foreach (var character in clipboardText)
            {
                switch (character)
                {
                    case '\r':
                        continue;
                    case '\n':
                        SendKeys.SendWait($"{{ENTER}}");
                        continue;
                    default:
                        SendKeys.SendWait($"{{{character}}}");
                        break;
                }
            }
        }
    }
}
