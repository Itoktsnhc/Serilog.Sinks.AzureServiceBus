using System;
using Serilog;
using Serilog.Configuration;
using Serilog.Context;
using Serilog.Events;
using Serilog.Formatting.Json;
using Serilog.Sinks.AzureServiceBus;

namespace SinkTester
{
    class Program
    {
        static void Main(string[] args)
        {
            var connStr = "<FILL ME>";
            var entityPath = "<FILL ME>";
            var config = new LoggerConfiguration();
            var logger = config.WriteTo.AzureServiceBus(connStr, entityPath, new JsonFormatter()).CreateLogger().ForContext<Program>();
            logger.Error(new Exception(), "foobar");
            Console.WriteLine("Hello World!");
        }
    }
}
