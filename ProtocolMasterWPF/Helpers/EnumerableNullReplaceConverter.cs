using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Data;

namespace ProtocolMasterWPF.Helpers
{
    public class EnumerableNullReplaceConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var collection = (IEnumerable)value; 
            var list = collection.Cast<object>().ToList(); list.Insert(0, DeviceInformationEmpty.NullEquivalent); 
            return list;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
