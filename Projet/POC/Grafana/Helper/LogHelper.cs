using Enums;
using Microsoft.Extensions.Logging;
using System;

namespace Project.Helpers
{
    public static class LogHelper
    {
        /// <summary>
        /// Log a structured activity event for Grafana/Loki querying.
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="context"></param>
        /// <param name="userId"></param>
        /// <param name="activityType"></param>
        /// <param name="platformId"></param>
        /// <param name="severityLevel"></param>
        /// <param name="details"></param>
        public static void LogActivity(
            ILogger logger,
            string context,
            long userId,
            UserActivityType? activityType,
            byte platformId,
            byte severityLevel,
            object details = null)
        {
            logger.LogInformation(
                "ActivityLog Context={Context} UserId={UserId} ActivityType={ActivityType} PlatformId={PlatformId} SeverityLevel={SeverityLevel} {@Details}",
                context,
                userId,
                activityType,
                platformId,
                severityLevel,
                details ?? new { }
            );
        }


        public static void LogError(
        ILogger logger,
        Exception exception,
        string context,
        object details = null)
        {
            logger.LogError(
                exception,
                "Error Context={Context} {@Details}",
                context,
                details ?? new { }
            );
        }
    }


}
