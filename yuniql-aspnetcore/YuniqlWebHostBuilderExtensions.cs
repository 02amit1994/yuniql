﻿using Microsoft.AspNetCore.Hosting;
using Yuniql.Core;
using Yuniql.Extensibility;

namespace Yuniql.AspNetCore
{
    public static class YuniqlWebHostBuilderExtensions
    {
        /// <summary>
        /// Runs database migrations with Yuniql. This uses default trace service FileTraceService.
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="configuration">Desired configuration when yuniql runs. Set your workspace location, connection string, target version and other parameters.</param>
        /// <returns></returns>
        public static IWebHostBuilder UseYuniql(
            this IWebHostBuilder builder,
            YuniqlConfiguration configuration
        )
        {
            var traceService = new FileTraceService { IsDebugEnabled = configuration.DebugTraceMode };
            return builder.UseYuniql(traceService, configuration);
        }

        /// <summary>
        /// Runs database migrations with Yuniql. This uses your specific implementation of ITraceService interface.
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="traceService">Your custom implementation of ITraceService interface</param>
        /// <param name="configuration">Desired configuration when yuniql runs. Set your workspace location, connection string, target version and other parameters.</param>
        /// <returns></returns>
        public static IWebHostBuilder UseYuniql(
            this IWebHostBuilder builder,
            ITraceService traceService,
            YuniqlConfiguration configuration
        )
        {
            var migrationServiceFactory = new MigrationServiceFactory(traceService);
            var migrationService = migrationServiceFactory.Create(configuration.Platform);
            migrationService.Initialize(configuration.ConnectionString);
            migrationService.Run(
                configuration.WorkspacePath,
                configuration.TargetVersion,
                configuration.AutoCreateDatabase,
                configuration.Tokens,
                configuration.VerifyOnly,
                configuration.Delimiter,
                configuration.BatchSize,
                configuration.CommandTimeout);

            return builder;
        }
    }
}
