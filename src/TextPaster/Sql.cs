using System.Collections.Generic;
using System.Data.SQLite;

namespace TextPaster
{
    class KeyboardMap
    {
        public int Id { get; set; }

        public string Key { get; set; }

        public string Text { get; set; }
    }

    class Sql
    {
        private string connectionString = "Data Source=Keybaord.db";

        public Sql()
        {
            string database = "Keyboard.db";

            if (!System.IO.File.Exists(database))
            {
                SQLiteConnection.CreateFile(database);
            }
        }

        public bool IsTableExist(string tableName, out bool isExist)
        {
            isExist = false;

            try
            {
                using (var connection = new SQLiteConnection(connectionString))
                {
                    connection.Open();
                    var command = connection.CreateCommand();

                    command.CommandText = $"SELECT name FROM sqlite_master WHERE type='table' AND name='{tableName}';";

                    using (var reader = command.ExecuteReader())
                    {
                        isExist = reader.HasRows;
                    }
                }

                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool CreateTable(string tableName)
        {
            int ret = -1;
            using (var connection = new SQLiteConnection(connectionString))
            {
                connection.Open();

                var command = connection.CreateCommand();

                command.CommandText =
                    @"CREATE TABLE keyboard (
                      `id` INTEGER PRIMARY KEY AUTOINCREMENT,
                      `key` TEXT NOT NULL UNIQUE,
                      `text` TEXT
                     );";

                ret = command.ExecuteNonQuery();
            }

            return ret == 0;
        }

        public bool Insert(KeyboardMap model)
        {
            int result = -1;
            using (var connection = new SQLiteConnection(connectionString))
            {
                connection.Open();

                var command = connection.CreateCommand();

                command.CommandText =
                    $@"INSERT INTO keyboard (`key`, `text`) VALUES ('{model.Key}', '{model.Text}');";

                result = command.ExecuteNonQuery();
            }

            return result == 0;
        }

        public bool Select(out List<KeyboardMap> models)
        {
            models = new List<KeyboardMap>();

            try
            {
                using (var connection = new SQLiteConnection(connectionString))
                {
                    connection.Open();

                    var command = connection.CreateCommand();

                    command.CommandText = @"SELECT * FROM keyboard";

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var id = reader.GetInt32(0);
                            var key = reader.GetString(1);
                            var text = reader.GetString(2);
                            models.Add(new KeyboardMap
                            {
                                Id = id,
                                Key = key,
                                Text = text
                            });
                        }
                    }
                }
            }
            catch
            {
                return false;
            }

            return true;
        }

        public bool Update(KeyboardMap model)
        {
            int result = -1;
            using (var connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = $@"
                     UPDATE keyboard
                     SET `key` = '{model.Key}', `text` = '{model.Text}'
                     WHERE `id` = '{model.Id}';";

                result = command.ExecuteNonQuery();
            }

            return result == 0;
        }

        public bool Delete(KeyboardMap model)
        {
            int result = -1;
            using (var connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText =
                    $@"DELETE FROM keyboard WHERE `id`='{model.Id}';";

                result = command.ExecuteNonQuery();
            }

            return result == 0;
        }
    }
}
