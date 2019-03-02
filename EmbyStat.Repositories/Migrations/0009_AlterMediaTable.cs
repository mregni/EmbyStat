using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Text;
using Dapper;
using EmbyStat.Common;
using FluentMigrator;

namespace EmbyStat.Repositories.Migrations
{
    [Migration(9, TransactionBehavior.None)]
    public class AlterMediaTableTwo : Migration
    {
        public override void Up()
        {
            var dbFilePath = "./data.db";
            var dbConnection = new SQLiteConnection($"Data Source={dbFilePath};");
            dbConnection.Open();

            dbConnection.Execute("BEGIN TRANSACTION;");
            dbConnection.Execute($"ALTER TABLE {Constants.Tables.Media} RENAME TO {Constants.Tables.Media}_B;");
            dbConnection.Execute($"CREATE TABLE {Constants.Tables.Media} (" +
                                 "Id TEXT NOT NULL PRIMARY KEY," +
                                 "DateCreated DATETIME NULL," +
                                 "ParentId UNIQUEIDENTIFIER NOT NULL," +
                                 "PremiereDate DATETIME NULL," +
                                 "ProductionYear INTEGER NULL," +
                                 "Logo TEXT NULL," +
                                 "Primary TEXT NULL," +
                                 "Thumb TEXT NULL," +
                                 "Name TEXT NULL," +
                                 "Path TEXT NULL," +
                                 "SortName TEXT NULL," +
                                 "Discriminator TEXT NOT NULL," +
                                 "CommunityRating NUMERIC NULL," +
                                 "RunTimeTicks INTEGER NULL," +
                                 "Overview TEXT NULL," +
                                 "IMDB TEXT NULL," +
                                 "TMDB TEXT NULL," +
                                 "TVDB TEXT NULL," +
                                 "Container TEXT NULL," +
                                 "HasSubtitles INTEGER NULL," +
                                 "IdHD INTEGER NULL," +
                                 "MediaType TEXT NULL," +
                                 "IndexNumber INTEGER NULL," +
                                 "DvdEpisodeNumber NUMERIC NULL," +
                                 "DvdSeasonNumber INTEGER NULL," +
                                 "IndexNumberEnd INTEGER NULL," +
                                 "HomePageUrl TEXT NULL," +
                                 "OfficialRating TEXT NULL," +
                                 "OriginalTitle TEXT NULL," +
                                 "CumulativeRunTimeTicks INTEGER NULL," +
                                 "DateLastMediaAdded DATETIME NULL," +
                                 "Status TEXT NULL," +
                                 "TvdbSynced INTEGER NULL," +
                                 "MissingEpisodesCount INTEGER NULL," +
                                 "TvdbFailed TEXT NULL," +
                                 "Season_IndexNumber INTEGER NULL," +
                                 "Season_IndexNumberEnd INTEGER NULL," +
                                 $"FOREIGN KEY (ParentId) REFERENCES {Constants.Tables.Media}(id) " +
                                 "ON DELETE CASCADE ON UPDATE NO ACTION" +
                                 ");");

            dbConnection.Execute($"INSERT INTO {Constants.Tables.Media} SELECT * FROM {Constants.Tables.Media}_B;");
            dbConnection.Execute($"DROP TABLE {Constants.Tables.Media}_B;");
            dbConnection.Execute("COMMIT;");

            dbConnection.Close();
        }

        public override void Down()
        {
            throw new NotImplementedException();
        }
    }
}
