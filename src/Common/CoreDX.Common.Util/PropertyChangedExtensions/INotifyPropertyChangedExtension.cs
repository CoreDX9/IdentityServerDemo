using System.ComponentModel;

namespace CoreDX.Common.Util.PropertyChangedExtensions
{
    public class PropertyChangedExtensionEventArgs : PropertyChangedEventArgs
    {
        public virtual object OldValue { get; }
        public virtual object NewValue { get; }

        public PropertyChangedExtensionEventArgs(string propertyName, object oldValue, object newValue) : base(propertyName)
        {

        }
    }

    public delegate void PropertyChangedExtensionEventHandler(object sender, PropertyChangedExtensionEventArgs e);

    public interface INotifyPropertyChangedExtension : INotifyPropertyChanged
    {
        event PropertyChangedExtensionEventHandler PropertyChangedExtension;
    }
}
