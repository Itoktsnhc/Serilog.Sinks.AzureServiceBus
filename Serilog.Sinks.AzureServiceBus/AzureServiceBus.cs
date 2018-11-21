using System;
using System.IO;
using System.Text;
using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.ServiceBus.Core;
using Serilog.Configuration;
using Serilog.Core;
using Serilog.Events;
using Serilog.Formatting;
using Serilog.Formatting.Display;

namespace Serilog.Sinks.AzureServiceBus
{
    public class AzureServiceBus : ILogEventSink
    {
        private readonly IMessageSender _sender;
        private readonly ITextFormatter _formatter;

        public AzureServiceBus(IMessageSender sender, ITextFormatter formatter)
        {
            _sender = sender;
            _formatter = formatter;
        }

        public void Emit(LogEvent logEvent)
        {
            byte[] body;
            using (var render = new StringWriter())
            {
                _formatter.Format(logEvent, render);
                body = Encoding.UTF8.GetBytes(render.ToString());
            }

            var message = new Message(body);
            _sender.SendAsync(message).ConfigureAwait(false).GetAwaiter().GetResult();
        }
    }

    public static class AzureServiceBusSinkExtensions
    {
        private const string DefaultOutputTemplate =
            "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level}] {Message}{NewLine}{Exception}";


        public static LoggerConfiguration AzureServiceBus(
            this LoggerSinkConfiguration loggerConfiguration,
            string connStr,
            string entityPath,
            string outputTemplate = DefaultOutputTemplate,
            IFormatProvider formatProvider = null,
            LogEventLevel restrictedToMinimumLevel = LevelAlias.Minimum)
        {
            if (loggerConfiguration == null) throw new ArgumentNullException(nameof(loggerConfiguration));
            if (connStr == null) throw new ArgumentNullException(nameof(connStr));
            if (entityPath == null) throw new ArgumentNullException(nameof(entityPath));
            if (outputTemplate == null) throw new ArgumentNullException(nameof(outputTemplate));
            if (formatProvider == null) throw new ArgumentNullException(nameof(formatProvider));

            var sender = new MessageSender(connStr, entityPath);
            var formatter = new MessageTemplateTextFormatter(outputTemplate, formatProvider);
            var sink = new AzureServiceBus(sender, formatter);
            return loggerConfiguration.Sink(sink, restrictedToMinimumLevel);
        }

        public static LoggerConfiguration AzureServiceBus(
            this LoggerSinkConfiguration loggerConfiguration,
            string connStr,
            string entityPath,
            string outputTemplate = DefaultOutputTemplate,
            ITextFormatter textFormatter = null,
            LogEventLevel restrictedToMinimumLevel = LevelAlias.Minimum)
        {
            if (loggerConfiguration == null) throw new ArgumentNullException(nameof(loggerConfiguration));
            if (connStr == null) throw new ArgumentNullException(nameof(connStr));
            if (entityPath == null) throw new ArgumentNullException(nameof(entityPath));
            if (outputTemplate == null) throw new ArgumentNullException(nameof(outputTemplate));

            var sender = new MessageSender(connStr, entityPath);
            var sink = new AzureServiceBus(sender, textFormatter);
            return loggerConfiguration.Sink(sink, restrictedToMinimumLevel);
        }

        public static LoggerConfiguration AzureServiceBus(
            this LoggerSinkConfiguration loggerConfiguration,
            string connStr,
            string entityPath,
            ITextFormatter formatter,
            LogEventLevel restrictedToMinimumLevel = LevelAlias.Minimum)
        {
            if (loggerConfiguration == null) throw new ArgumentNullException(nameof(loggerConfiguration));
            if (formatter == null) throw new ArgumentNullException(nameof(formatter));
            if (connStr == null) throw new ArgumentNullException(nameof(connStr));
            if (entityPath == null) throw new ArgumentNullException(nameof(entityPath));

            var sender = new MessageSender(connStr, entityPath);
            var sink = new AzureServiceBus(sender, formatter);
            return loggerConfiguration.Sink(sink, restrictedToMinimumLevel);

        }
    }
}