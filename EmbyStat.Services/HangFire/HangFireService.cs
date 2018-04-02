using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EmbyStat.Repositories.HangFire;
using Hangfire;

namespace EmbyStat.Services.HangFire
{
	public class HangFireService : IHangFireService
	{
		private readonly IHangFireRepository _hangFireRepository;

		public HangFireService(IHangFireRepository hangFireRepository)
		{
			_hangFireRepository = hangFireRepository;
		}

		public List<BackgroundTask> GetTasks()
		{
			var sets = _hangFireRepository.GetSets();

			var backgroundList = new List<BackgroundTask>();
			foreach (var set in sets)
			{
				if (set.Key.EndsWith('s'))
					set.Key = set.Key.Remove(set.Key.Length-1, 1);
				var hashes = _hangFireRepository.GetHashesFromSet(set.Key, set.Value);
				var task = new BackgroundTask(set.Value, hashes);

				backgroundList.Add(task);
			}

			return backgroundList;
		}

		public void FireTask(string id)
		{
			RecurringJob.Trigger(id);
		}
	}
}
