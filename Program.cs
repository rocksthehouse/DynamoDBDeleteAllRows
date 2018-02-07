using System;

namespace DynamoDeleteAllRows
{
    /// <summary>
    /// Class for launching the application.
    /// </summary>
    public class Program
    {
        /// <summary>
        /// The entry point to the application.
        /// </summary>
        /// <param name="args">The command line arguments</param>
        public static void Main(string[] args)
        {
            try
            {
                if (args.Length == 2 || args.Length == 3)
                {
                    var tableName = args[0];
                    var hashKey = args[1];

                    string rangeKey = null;

                    if (args.Length == 3)
                    {
                        rangeKey = args[2];
                    }

                    if (string.IsNullOrWhiteSpace(tableName) || string.IsNullOrWhiteSpace(hashKey) || args.Length == 3 && string.IsNullOrWhiteSpace(rangeKey))
                    {
                        DisplaySyntaxError();
                    }
                    else
                    {
                        new DeleteAllTableRows().DeleteAllRows(tableName, hashKey, rangeKey);
                    }
                }
                else
                {
                    DisplaySyntaxError();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed due to exception: {ex}");
            }
        }

        private static void DisplaySyntaxError()
        {
            Console.WriteLine("Expected syntax: DynamoDeleteAllRows.exe table_name hash_key_name [range_key_name]");
            Console.WriteLine("AWS profile and region name can be specified by modifying the application configuration file.");
        }
    }
}
