using System;
using StackExchange.Exceptional;
using StackExchange.Exceptional.Stores;
using StackExchange.Exceptional.Notifiers;
using static System.Console;
using System.Threading.Tasks;

namespace Samples.LogLevelTesting
{
    public static class Program
    {
        public static void Main()
        {
            Exceptional.Configure(settings =>
            {
                settings.DefaultStore = new SQLErrorStore(new ErrorStoreSettings
                {
                    ApplicationName = "Samples.LogLevelTest",
                    ConnectionString = "Server=.;Database=Local.Exceptional;Trusted_Connection=True;",
                    TableName = "Exceptions"
                });
            });

            // Optional: for logging all unhandled exceptions
            Exceptional.ObserveAppDomainUnhandledExceptions();

            // Normally we wouldn't want to .GetAwaiter().GetResult(), but async Main is only on a the latest platforms at the moment
            DisplayExceptionStatsAsync().GetAwaiter().GetResult();
            PauseForInput();

            try
            {
                throw new Exception("Just a try/catch test");
            }
            catch (Exception ex)
            {
                ex.AddLogData("Example string", DateTime.UtcNow.ToString())
                  .AddLogData("User Id", "You could fetch a user/account Id here, etc.")
                  .AddLogData("Links get linkified", "https://www.google.com")
                  .Trace();

                // logged, but caught so we don't crash
                ex.LogNoContext();
            }

            DisplayExceptionStatsAsync().GetAwaiter().GetResult();
            PauseForInput();

            WriteLine("This next one will crash the program, but will be logged on the way out...");
            PauseForInput();

            // one not explicitly caught, will be logged by ExceptionHandler
            throw new Exception("I am an exception thrown on exit");
        }

        private static async Task DisplayExceptionStatsAsync()
        {
            var settings = Exceptional.Settings;
            WriteLine(settings.DefaultStore.Name + " for " + settings.DefaultStore.Name);
            var count = await settings.DefaultStore.GetCountAsync().ConfigureAwait(false);
            WriteLine("Exceptions in the log: " + count.ToString());

            var errors = await settings.DefaultStore.GetAllAsync().ConfigureAwait(false);

            if (errors.Count == 0) return;

            var last = errors[0];
            WriteLine($"Latest: {last.Message} on {last.CreationDate.ToString()}");
            foreach (var customData in last.CustomData)
                WriteLine($"    CustomData: '{customData.Key}': '{customData.Value}'");
        }

        private static void PauseForInput()
        {
            WriteLine("Press any key to continue...");
            ReadLine();
        }
    }
}
