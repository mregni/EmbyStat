using System.Data.SQLite;
using Dapper;
using EmbyStat.Common;
using FluentMigrator;

namespace EmbyStat.Repositories.Migrations
{
    [Migration(8, TransactionBehavior.None)]
    public class AlterAudioStreams : Migration
    {
        public override void Up()
        {
            var dbFilePath = "./data.db";
            var dbConnection = new SQLiteConnection($"Data Source={dbFilePath};");
            dbConnection.Open();

            dbConnection.Execute("BEGIN TRANSACTION;");
            dbConnection.Execute($"ALTER TABLE {Constants.Tables.AudioStreams} RENAME TO {Constants.Tables.AudioStreams}_B;");
            dbConnection.Execute($"CREATE TABLE {Constants.Tables.AudioStreams} (" +
                                 "Id TEXT NOT NULL PRIMARY KEY," +
                                 "BitRate INTEGER," + 
                                 "ChannelLayout TEXT," + 
                                 "Channels INTEGER NULL," + 
                                 "Codec TEXT NULL," + 
                                 "Language TEXT," + 
                                 "SampleRate INTEGER NULL," +
                                 "VideoId TEXT NOT NULL," +
                                 $"FOREIGN KEY (VideoId) REFERENCES {Constants.Tables.Media}(id) " +
                                 "ON DELETE CASCADE ON UPDATE NO ACTION" +
                                 ");");

            dbConnection.Execute($"INSERT INTO {Constants.Tables.AudioStreams} " +
                                 "(Id, BitRate, ChannelLayout, Channels, Codec, Language, SampleRate, VideoId) " +
                                 "SELECT Id, BitRate, ChannelLayout, Channels, Codec, Language, SampleRate, VideoId " +
                                 $"FROM {Constants.Tables.AudioStreams}_B;");

            dbConnection.Execute($"DROP TABLE {Constants.Tables.AudioStreams}_B;");
            dbConnection.Execute("COMMIT;");

            dbConnection.Close();
        }

        public override void Down()
        {
            
        }
    }
}
