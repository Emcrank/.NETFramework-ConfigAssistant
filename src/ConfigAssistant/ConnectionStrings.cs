using System.Configuration;

namespace ConfigAssistant
{
    public static class ConnectionStrings
    {
        /// <summary>
        /// Gets a connection string from the configuration file. If it does not exist or is empty then an exception is thrown.
        /// </summary>
        /// <param name="name">The name of the connection string in the configuration.</param>
        /// <returns></returns>
        /// <exception cref="ConfigurationErrorsException">Thrown if empty or not found.</exception>
        public static string GetRequired(string name)
        {
            string connectionString = Get(name);

            if (string.IsNullOrWhiteSpace(connectionString))
                throw new ConfigurationErrorsException(
                    $"A connection string does not exist in the config with the name '{name}'.");

            return connectionString;
        }

        public static string Get(string name)
        {
            return ConfigurationManager.ConnectionStrings[name]?.ConnectionString;
        }
    }
}