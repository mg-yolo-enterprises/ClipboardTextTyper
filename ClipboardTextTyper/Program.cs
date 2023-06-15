using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Tesseract;

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
            if (Clipboard.ContainsText(TextDataFormat.Text))
            {
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
            }
            else if (Clipboard.ContainsImage())
            {
                var clipboardImage = Clipboard.GetImage();
                if (clipboardImage == null)
                {
                    Error("Clipboard text is null!");
                    return;
                }

                using (var engine = new TesseractEngine(@"./tessdata", "eng", EngineMode.Default))
                {
                    using (var page = engine.Process((Bitmap)clipboardImage))
                    {
                        var foundText = page.GetText();
                        Console.WriteLine("Found text (copying to clipboard):");
                        Console.WriteLine(foundText);
                        Clipboard.SetText(foundText);
                    }
                }
            }
            else
            {
                Error("Clipboard doesn't contain text or image!");
                return;
            }

            
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
