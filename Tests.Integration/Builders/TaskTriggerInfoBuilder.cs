using System;
using System.Collections.Generic;
using System.Text;
using EmbyStat.Controllers.ViewModels.Task;

namespace Tests.Integration.Builders
{
    public class TaskTriggerInfoBuilder
    {
        private readonly TaskTriggerInfoViewModel _model;

        public TaskTriggerInfoBuilder()
        {
            _model = new TaskTriggerInfoViewModel();
        }

        public TaskTriggerInfoBuilder AddId(string id)
        {
            _model.Id = id;
            return this;
        }

        public TaskTriggerInfoBuilder AddIntervalTicks(long? intervalTicks)
        {
            _model.IntervalTicks = intervalTicks;
            return this;
        }

        public TaskTriggerInfoBuilder AddDayOfWeek(int? dayOfWeek)
        {
            _model.DayOfWeek = dayOfWeek;
            return this;
        }

        public TaskTriggerInfoBuilder AddMaxRuntimeTicks(long? maxRuntimeTicks)
        {
            _model.MaxRuntimeTicks = maxRuntimeTicks;
            return this;
        }

        public TaskTriggerInfoBuilder AddTimeOfDayTicks(int? timeOfDayTicks)
        {
            _model.TimeOfDayTicks = timeOfDayTicks;
            return this;
        }

        public TaskTriggerInfoBuilder AddType(string type)
        {
            _model.Type = type;
            return this;
        }

        public TaskTriggerInfoViewModel Build()
        {
            return _model;
        }
    }
}
