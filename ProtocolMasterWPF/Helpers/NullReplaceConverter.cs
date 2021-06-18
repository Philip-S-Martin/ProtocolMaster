﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Windows.Data;

namespace ProtocolMasterWPF.Helpers
{
    public class NullReplaceConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value ?? DeviceInformationEmpty.NullEquivalent;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value == DeviceInformationEmpty.NullEquivalent ? null : value;
        }
    }
}
