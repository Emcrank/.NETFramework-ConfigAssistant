using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;

namespace ConfigAssistant
{
    /// <summary>
    /// Static utility class to wrap getting settings from the
    /// AppSettings section with more meaningful exception messages
    /// and as the correct type.
    /// </summary>
    public static class AppSettings
    {
        /// <summary>
        /// Gets and converts a setting from the appSettings section.
        /// </summary>
        /// <typeparam name="T">The type to convert to.</typeparam>
        /// <param name="key">The key for the setting.</param>
        /// <returns>The setting value</returns>
        public static T Get<T>(string key)
        {
            string actualValue = ConfigurationManager.AppSettings[key];

            return string.IsNullOrEmpty(actualValue)
                ? default(T)
                : TryChangeType<T>(actualValue, key);
        }

        /// <summary>
        /// Gets and converts a setting from the appSettings section.
        /// Throws a <see cref="ConfigurationErrorsException" /> if the setting does not exist.
        /// </summary>
        /// <typeparam name="T">The type to convert to.</typeparam>
        /// <param name="key">The key for the setting.</param>
        /// <returns>The setting value</returns>
        /// <exception cref="ConfigurationErrorsException">Thrown if the setting does not exist.</exception>
        public static T GetRequired<T>(string key)
        {
            string actualValue = ConfigurationManager.AppSettings[key];

            if (string.IsNullOrEmpty(actualValue))
                throw new ConfigurationErrorsException(
                    $"The required setting '{key}' was not found in appSettings in the configuration file.");

            return TryChangeType<T>(actualValue, key);
        }

        /// <summary>
        /// Gets a string value from appSettings section.
        /// </summary>
        /// <param name="key">The key for the setting.</param>
        /// <returns>The setting value.</returns>
        public static string Get(string key)
        {
            return Get<string>(key);
        }

        /// <summary>
        /// Gets a required string value from appSettings section.
        /// </summary>
        /// <param name="key">The key for the setting.</param>
        /// <returns>The setting value.</returns>
        /// <exception cref="ConfigurationErrorsException">Thrown if setting not found.</exception>
        public static string GetRequired(string key)
        {
            return GetRequired<string>(key);
        }

        /// <summary>
        /// Splits an appSetting value and projects it into an IEnumerable of T.
        /// </summary>
        /// <typeparam name="T">The type of the members</typeparam>
        /// <param name="key">The setting key</param>
        /// <param name="delimiter">The delimiter value</param>
        /// <param name="options">Options for splitting</param>
        /// <returns>An <see cref="IEnumerable{T}" /> for the specified setting.</returns>
        public static IEnumerable<T> SplitAndGet<T>(
            string key,
            string delimiter = ";",
            StringSplitOptions options = StringSplitOptions.RemoveEmptyEntries)
        {
            string actualValue = ConfigurationManager.AppSettings[key];

            if (string.IsNullOrWhiteSpace(actualValue))
                return Enumerable.Empty<T>();

            var splitValues = actualValue.Split(new[] { delimiter }, options);

            return splitValues.Select(x => TryChangeType<T>(x, key));
        }

        private static T TryChangeType<T>(object value, string key)
        {
            if (typeof(T) == typeof(string))
                return (T)value;

            try
            {
                return (T)Convert.ChangeType(value, typeof(T), CultureInfo.InvariantCulture);
            }
            catch (InvalidCastException ex)
            {
                throw new ConfigurationErrorsException(
                    $"The setting value with the key '{key}' could not be parsed as an {typeof(T).Name}.",
                    ex);
            }
            catch (FormatException ex)
            {
                throw new ConfigurationErrorsException(
                    $"The setting value with the key '{key}' is in the wrong format for a {typeof(T).Name}",
                    ex);
            }
            catch (OverflowException ex)
            {
                throw new ConfigurationErrorsException(
                    $"An overflow occured trying to convert setting with the key '{key}' to type {typeof(T).Name}",
                    ex);
            }
        }
    }
}