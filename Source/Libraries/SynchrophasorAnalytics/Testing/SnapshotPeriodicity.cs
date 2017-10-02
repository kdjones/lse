using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SynchrophasorAnalytics.Testing
{
    /// <summary>
    /// An enumeration which gives options for automatically taking network snapshots.
    /// </summary>
    public enum SnapshotPeriodicity
    {
        /// <summary>
        /// Indicates a snapshot should be taken every other hour.
        /// </summary>
        EveryOtherHour,

        /// <summary>
        /// Indicates a snapshot should be taken every hour.
        /// </summary>
        EveryHour,

        /// <summary>
        /// Indicates a snapshot should be taken every thirty minutes.
        /// </summary>
        EveryThirtyMinutes,

        /// <summary>
        /// Indicates a snapshot should be taken every fifteen minutes.
        /// </summary>
        EveryFifteenMinutes,

        /// <summary>
        /// Indicates a snapshot should be taken every minute.
        /// </summary>
        EveryMinute,

        /// <summary>
        /// Indicates a snapshot should be taken every second.
        /// </summary>
        EverySecond
    }
}
