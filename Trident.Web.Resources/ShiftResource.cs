using System;

namespace Trident.Web.Resources
{
    public class ShiftResource : ResourceBase<long>
    {
        public DateTimeOffset StartTime { get; set; }

        public DateTimeOffset EndTime { get; set; }

        public DateTimeOffset? MealbreakStartTime { get; set; }

        public DateTimeOffset? MealbreakEndTime { get; set; }

        public float? TotalTime { get; set; }

        public float? Cost { get; set; }

        public Guid? FleetId { get; set; }

        public Guid? AccountId { get; set; }

        public string Comment { get; set; }

        public string Warning { get; set; }

        public string WarningOverrideComment { get; set; }

        public bool? Published { get; set; }

        public int? MatchedByTimesheet { get; set; }

        public bool? Open { get; set; }

        public int? ConfirmStatus { get; set; }

        public string ConfirmComment { get; set; }

        public int? ConfirmBy { get; set; }

        public int? ConfirmTime { get; set; }

        public int? SwapStatus { get; set; }

        public int? SwapManageBy { get; set; }

        public int? Creator { get; set; }

        public DateTimeOffset? Created { get; set; }

        public DateTimeOffset? Modified { get; set; }

        // derrived properties, set only via LoadChildren

        public string FleetName { get; set; }
    }
}