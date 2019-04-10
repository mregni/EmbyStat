using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Text;
using Dapper;
using EmbyStat.Common;
using FluentMigrator;

namespace EmbyStat.Repositories.Migrations
{
    [Migration(10, TransactionBehavior.None)]
    public class AlterServerInfoTable : Migration
    {
        public override void Up()
        {
            var dbFilePath = "./data.db";
            var dbConnection = new SQLiteConnection($"Data Source={dbFilePath};");
            dbConnection.Open();

            dbConnection.Execute("BEGIN TRANSACTION;");
            dbConnection.Execute($"ALTER TABLE {Constants.Tables.ServerInfo} RENAME TO {Constants.Tables.ServerInfo}_B;");
            dbConnection.Execute($"CREATE TABLE {Constants.Tables.ServerInfo} (" +
                                 "Id TEXT NOT NULL PRIMARY KEY," +
                                 "SystemUpdateLevel INTEGER NOT NULL," +
                                 "OperatingSystemDisplayName TEXT NOT NULL," +
                                 "HasPendingRestart INTEGER NOT NULL," +
                                 "IsShuttingDown INTEGER NOT NULL," +
                                 "SupportsLibraryMonitor INTEGER NOT NULL," +
                                 "WebSocketPortNumber INTEGER NOT NULL," +
                                 "CanSelfRestart INTEGER NOT NULL," +
                                 "CanSelfUpdate INTEGER NOT NULL," +
                                 "CanLaunchWebBrowser INTEGER NOT NULL," +
                                 "ProgramDataPath TEXT NOT NULL," +
                                 "ItemsByNamePath TEXT NOT NULL," +
                                 "CachePath TEXT NOT NULL," +
                                 "LogPath TEXT NOT NULL," +
                                 "InternalMetadataPath TEXT NOT NULL," +
                                 "TranscodingTempPath TEXT NOT NULL," +
                                 "HttpServerPortNumber INTEGER NOT NULL," +
                                 "SupportsHttps INTEGER NOT NULL," +
                                 "HttpsPortNumber INTEGER NOT NULL," +
                                 "HasUpdateAvailable INTEGER NOT NULL," +
                                 "SupportsAutoRunAtStartup INTEGER NOT NULL," +
                                 "HardwareAccelerationRequiresPremiere INTEGER NOT NULL," +
                                 "LocalAddress TEXT NOT NULL," +
                                 "WanAddress TEXT NULL," +
                                 "ServerName TEXT NOT NULL," +
                                 "Version TEXT NOT NULL," +
                                 "OperatingSystem TEXT NOT NULL" +
                                 ");");

            dbConnection.Execute($"INSERT INTO {Constants.Tables.ServerInfo} " +
                                 $"SELECT * FROM {Constants.Tables.ServerInfo}_B;");

            dbConnection.Execute($"DROP TABLE {Constants.Tables.ServerInfo}_B;");
            dbConnection.Execute("COMMIT;");

            dbConnection.Close();
        }

        public override void Down()
        {
            
        }
    }
}
