using System.Collections.Generic;

namespace Models.Places.Schedules
{
    public class Schedule
    {
        public long? PlaceId { get; set; }
        public List<ScheduleApp> ScheduleApps { get; set; }
        public long? ScheduleId { get; set; }
    }
}
