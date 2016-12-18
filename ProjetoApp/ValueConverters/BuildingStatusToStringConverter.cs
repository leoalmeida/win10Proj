using System;
using System.Globalization;
using Windows.UI.Xaml.Data;
using ProjetoApp.Data;

namespace ProjetoApp.ValueConverters
{
    /// <summary>
    /// Converts EnterpriseModels.BuildingStatus values to strings. 
    /// </summary>
    public class BuildingStatusToStringConverter : IValueConverter
    {


        /// <summary>
        /// Converts the BuildingStatus object to the string to display. 
        /// </summary>
        /// <param name="value">The EnterpriseModels.BuildingStatus object.</param>
        /// <param name="targetType">The type to convert to. This should an object.</param>
        /// <param name="parameter">The format string.</param>
        /// <param name="language">Language and culture info. If this is null, we use the current culture.</param>
        /// <returns>The converted object.</returns>
        public object Convert(object value, Type targetType,
            object parameter, string language)
        {

            // Verify that the value is an BuildingStatus
            var buildingStatus = value;
            if (targetType.Equals(typeof(System.Object)) && buildingStatus != null)
            {

                // Retrieve the format string and use it to format the value.
                string formatString = parameter as string;
                if (!string.IsNullOrEmpty(formatString))
                {

                    CultureInfo culture = (!string.IsNullOrEmpty(language)) ? new CultureInfo(language) : CultureInfo.CurrentCulture;
                    return string.Format(culture, formatString, value);
                }


                // If no formatting information is provided, do a simple string conversion.
                return buildingStatus.ToString();
            }
            else
            {
                throw new ArgumentException($"Unsuported type: {targetType.FullName}");
            }
        }

        /// <summary>
        /// Converts a string to an BuildingStatus. 
        /// </summary>
        /// <param name="value">The string value to convert.</param>
        /// <param name="targetType">The target type to convert to. This should be BuildingStatus.</param>
        /// <param name="parameter">Formatting info (not used).</param>
        /// <param name="language">Language info (not used).</param>
        /// <returns>The converted enum value.</returns>
        public object ConvertBack(object value, Type targetType,
            object parameter, string language)
        {
            return Enum.Parse(typeof(ProjetoApp.Data.BuildingStatus), (string)value);
        }

    }
}
