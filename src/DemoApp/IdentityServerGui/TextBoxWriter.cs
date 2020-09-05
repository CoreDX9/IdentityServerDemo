using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace IdentityServerGui
{
    public class TextBoxWriter : TextWriter
    {
        public override Encoding Encoding => Encoding.Unicode;

        public TextBox TextBox { get; }
        public Window Window { get; }

        public TextBoxWriter(Window window, TextBox textBox)
        {
            Window = window;
            TextBox = textBox;
        }

        public override void Write(char value)
        {
            WriteAsync(value);
        }

        public override Task WriteAsync(char value)
        {
            Window.Dispatcher.InvokeAsync(() =>
            {
                TextBox.AppendText(value.ToString());
                TextBox.ScrollToEnd();
            }, System.Windows.Threading.DispatcherPriority.Normal);
            return Task.CompletedTask;
        }

        public override void WriteLine(string value)
        {
            WriteLineAsync(value);
        }

        public override Task WriteLineAsync(string value)
        {
            Window.Dispatcher.InvokeAsync(() =>
            {
                TextBox.AppendText(value + Environment.NewLine);
                TextBox.ScrollToEnd();
            }, System.Windows.Threading.DispatcherPriority.Normal);
            return Task.CompletedTask;
        }
    }
}
