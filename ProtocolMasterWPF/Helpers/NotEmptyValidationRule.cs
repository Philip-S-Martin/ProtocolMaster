using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Windows.Controls;

namespace ProtocolMasterWPF.Helpers
{
    public class NotEmptyValidationRule : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            return string.IsNullOrWhiteSpace((string)value)
                ? new ValidationResult(false, "Field is required.")
                : ValidationResult.ValidResult;
        }
    }
}
