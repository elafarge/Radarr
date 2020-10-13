using System.Collections.Generic;
using System.Data;
using System.Text.Json;
using Dapper;
using FluentMigrator;
using NzbDrone.Core.Datastore.Migration.Framework;

namespace NzbDrone.Core.Datastore.Migration
{
    [Migration(198)]
    public class multiple_ratings_support : NzbDroneMigrationBase
    {
        private readonly JsonSerializerOptions _serializerSettings;

        public multiple_ratings_support()
        {
            _serializerSettings = new JsonSerializerOptions
            {
                AllowTrailingCommas = true,
                IgnoreNullValues = false,
                PropertyNameCaseInsensitive = true,
                DictionaryKeyPolicy = JsonNamingPolicy.CamelCase,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = true
            };
        }

        protected override void MainDbUpgrade()
        {
            Execute.WithConnection((conn, tran) => FixRatings(conn, tran, "Movies"));
            Execute.WithConnection((conn, tran) => FixRatings(conn, tran, "ImportListMovies"));
        }

        private void FixRatings(IDbConnection conn, IDbTransaction tran, string table)
        {
            var rows = conn.Query<Movie197>($"SELECT Id, Ratings FROM {table}");

            var corrected = new List<Movie198>();

            foreach (var row in rows)
            {
                var oldRatings = JsonSerializer.Deserialize<Ratings197>(row.Ratings, _serializerSettings);

                var newRatings = new Ratings198
                {
                    Tmdb = new RatingChild198
                    {
                        Votes = oldRatings.Votes,
                        Value = oldRatings.Value,
                        Type = RatingType186.User
                    }
                };

                corrected.Add(new Movie198
                {
                    Id = row.Id,
                    Ratings = JsonSerializer.Serialize(newRatings, _serializerSettings)
                });
            }

            var updateSql = $"UPDATE {table} SET Ratings = @Ratings WHERE Id = @Id";
            conn.Execute(updateSql, corrected, transaction: tran);
        }

        private class Movie197
        {
            public int Id { get; set; }
            public string Ratings { get; set; }
        }

        private class Ratings197
        {
            public int Votes { get; set; }
            public decimal Value { get; set; }
        }

        private class Movie198
        {
            public int Id { get; set; }
            public string Ratings { get; set; }
        }

        private class Ratings198
        {
            public RatingChild198 Tmdb { get; set; }
            public RatingChild198 Imdb { get; set; }
            public RatingChild198 MetaCritic { get; set; }
            public RatingChild198 RottenTomatoes { get; set; }
        }

        private class RatingChild198
        {
            public int Votes { get; set; }
            public decimal Value { get; set; }
            public RatingType186 Type { get; set; }
        }

        private enum RatingType186
        {
            User
        }
    }
}
