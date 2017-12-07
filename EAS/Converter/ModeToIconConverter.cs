using EAS.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace EAS.Converter
{
    public class ModeToIconConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            Modes mode = (Modes)value;
            switch (mode)
            {
                default:
                case Modes.Multimedia:
                    return new Uri($"..\\Resources\\multimedia-icon.png", UriKind.RelativeOrAbsolute);
                case Modes.Communication:
                    return new Uri($"..\\Resources\\voice-support-headset-icon.png", UriKind.RelativeOrAbsolute);
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
