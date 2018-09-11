using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Elmah.Io.Client;
using Elmah.Io.Client.Models;
using log4net.Appender;
using log4net.helpers;
using log4net.spi;
using Sitecore.Configuration;

namespace Sitecore.Foundation.ElmahIo
{
    public class ElmahIoAppender : AppenderSkeleton
    {
        static readonly IElmahioAPI client = ElmahioAPI.Create(Settings.GetSetting("ElmahIo.ApiKey"));

        static List<Item> PropertiesToData(PropertiesCollection properties)
        {
            return properties
                .GetKeys()
                .Select(key => new Item { Key = key, Value = properties[key].ToString() })
                .ToList();
        }

        static Severity? LevelToSeverity(Level level)
        {
            if (level == Level.EMERGENCY)
                return Severity.Fatal;
            if (level == Level.FATAL)
                return Severity.Fatal;
            if (level == Level.ALERT)
                return Severity.Fatal;
            if (level == Level.CRITICAL)
                return Severity.Fatal;
            if (level == Level.SEVERE)
                return Severity.Fatal;
            if (level == Level.ERROR)
                return Severity.Error;
            if (level == Level.WARN)
                return Severity.Warning;
            if (level == Level.NOTICE)
                return Severity.Information;
            if (level == Level.INFO)
                return Severity.Information;
            if (level == Level.DEBUG)
                return Severity.Debug;
            if (level == Level.FINE)
                return Severity.Verbose;
            if (level == Level.TRACE)
                return Severity.Verbose;
            if (level == Level.FINER)
                return Severity.Verbose;
            if (level == Level.VERBOSE)
                return Severity.Verbose;
            if (level == Level.FINEST)
                return Severity.Verbose;

            return Severity.Information;
        }

        protected override void Append(LoggingEvent loggingEvent)
        {

            var data = PropertiesToData(loggingEvent.Properties);

            var message = new CreateMessage
            {
                Title = loggingEvent.RenderedMessage,
                Severity = LevelToSeverity(loggingEvent.Level).ToString(),
                DateTime = loggingEvent.TimeStamp.ToUniversalTime(),
                Detail = loggingEvent.GetExceptionStrRep(),
                Data = data,
                Application = loggingEvent.Domain,
                Source = loggingEvent.LoggerName,
                User = loggingEvent.UserName,
                Hostname = Environment.MachineName,
                Url = HttpContext.Current?.Request?.RawUrl,
                Method = HttpContext.Current?.Request?.HttpMethod
            };

            client.Messages.Create(Settings.GetSetting("ElmahIo.LogId"), message);
        }
    }
}