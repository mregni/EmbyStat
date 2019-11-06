using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using EmbyStat.Common.Models.Settings;
using EmbyStat.Repositories;
using Microsoft.Extensions.Options;
using Moq;

namespace Tests.Unit.Repository
{
    public abstract class BaseRepositoryTester
    {
        public string DbFileName;
        protected abstract void SetupRepository();
        
        protected BaseRepositoryTester(string dbFileName)
        {
            DbFileName = dbFileName;
        }

        protected DbContext CreateDbContext()
        {
            var optionsMock = new Mock<IOptions<AppSettings>>();
            optionsMock.Setup(x => x.Value)
                .Returns(new AppSettings() { DatabaseFile = DbFileName, Dirs = new Dirs { Config = "" } });

            return new DbContext(optionsMock.Object);
        }

        public void CleanUpDatabaseFile()
        {
            var dbPath = Path.Combine(Directory.GetCurrentDirectory(), DbFileName);
            if (File.Exists(dbPath))
            {
                File.Delete(dbPath);
            }
        }

        public void RunTest(Action test)
        {
            CleanUpDatabaseFile();
            SetupRepository();
            test();
            CleanUpDatabaseFile();
        }
    }
}
