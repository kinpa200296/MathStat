using System;
using System.Globalization;
using System.IO;
using System.Windows.Controls;

namespace MathStat.ViewModel
{
    public class FileNameValidationRule : ValidationRule
    {
        public bool ShouldExist { get; set; }

        public FileNameValidationRule()
        {
        }

        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            if (value == null)
            {
                return new ValidationResult(false, "Value is null");
            }
            if (value.GetType() != typeof(string))
            {
                return new ValidationResult(false, "Value is not a string");
            }
            if (ShouldExist && !File.Exists((string)value))
            {
                return new ValidationResult(false, "File doesn't exists");
            }
            if (!ShouldExist)
            {
                try
                {
                    using (File.OpenRead((string)value))
                    {

                    }
                }
                catch (FileNotFoundException) { }
                catch (IOException) { }
                catch (Exception)
                {
                    return new ValidationResult(false, "Not a valid path");
                }
            }
            return new ValidationResult(true, null);
        }
    }
}
