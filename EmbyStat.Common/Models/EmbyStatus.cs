using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace EmbyStat.Common.Models
{
    public class EmbyStatusKeyValue
    {
        [Key]
        public string Id { get; set; }
        public string Value { get; set; }
    }

    public class EmbyStatus
    {
        private readonly Dictionary<string, string> _status;

        public int MissedPings
        {
            get => Convert.ToInt32(_status[Constants.EmbyStatus.MissedPings]);
            set => _status[Constants.EmbyStatus.MissedPings] = value.ToString();
        }

        public EmbyStatus(IEnumerable<EmbyStatusKeyValue> list)
        {
            _status = list.ToDictionary(x => x.Id, y => y.Value);
        }

        [Obsolete("Don't use this contructor! AutMapper needs it")]
        public EmbyStatus()
        {
            _status = new Dictionary<string, string>();
        }

        public IEnumerable<EmbyStatusKeyValue> GetKeyValuePairs()
        {
            return _status.Select(x => new EmbyStatusKeyValue { Id = x.Key, Value = x.Value });
        }
    }
}
