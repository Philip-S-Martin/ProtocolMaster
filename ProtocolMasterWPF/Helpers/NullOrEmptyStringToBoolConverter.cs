﻿using System;
using System.Globalization;
using System.Windows.Data;

namespace ProtocolMasterWPF.Helpers
{
    public class NullOrEmptyStringToBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return !String.IsNullOrEmpty(value as string);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return !String.IsNullOrEmpty(value as string);
        }
    }
}
