using System;
using System.Runtime.ExceptionServices;
using StackExchange.Exceptional.Internal;

namespace StackExchange.Exceptional
{
    public static partial class Extensions
    {
        public const string LogLevelKey = "ExceptionLogging.Level";
        
        public static T Trace<T>(this T source, bool overrideAnyCurrentValue = true) where T : Exception =>
            source.RecordLogLevel(ExceptionLogLevel.Trace, overrideAnyCurrentValue);
        
        public static T Debug<T>(this T source, bool overrideAnyCurrentValue = true) where T : Exception =>
            source.RecordLogLevel(ExceptionLogLevel.Debug, overrideAnyCurrentValue);
        
        public static T Info<T>(this T source, bool overrideAnyCurrentValue = true) where T : Exception =>
            source.RecordLogLevel(ExceptionLogLevel.Info, overrideAnyCurrentValue);
        
        public static T Error<T>(this T source, bool overrideAnyCurrentValue = true) where T : Exception =>
            source.RecordLogLevel(ExceptionLogLevel.Error, overrideAnyCurrentValue);
        
        public static T Warning<T>(this T source, bool overrideAnyCurrentValue = true) where T : Exception =>
            source.RecordLogLevel(ExceptionLogLevel.Warning, overrideAnyCurrentValue);
        
        public static T Critical<T>(this T source, bool overrideAnyCurrentValue = true) where T : Exception =>
            source.RecordLogLevel(ExceptionLogLevel.Critical, overrideAnyCurrentValue);
        
        static T RecordLogLevel<T>(this T source, ExceptionLogLevel logLevel, bool overrideAnyCurrentValue) where T : Exception
        {
            if (overrideAnyCurrentValue || !source.Data.Contains(LogLevelKey))
                source.Data[LogLevelKey] = logLevel;
            return source;
        }
        
        // TODO: Maybe this is only required by unit tests if we're relying on Exceptional to record the log level value in OpServer?
        public static ExceptionLogLevel? TryToGetLogLevel(this Exception ex) =>
            // Note: ex.Data[..] will return null if the key is not found, so no need to call Contains before attempting access
            // TODO: If RecordLogLevel is changed to use Exceptional's AddLogData method then update this as well (Constants.CustomDataKeyPrefix + LogLevelKey)
            Enum.TryParse<ExceptionLogLevel>(ex.Data[Constants.CustomDataKeyPrefix + LogLevelKey] as string, out var result)
                ? result
                : (ExceptionLogLevel?)null;
    }
}
