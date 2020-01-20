using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Npgsql;

namespace TaskNet
{
	/// <summary>
	/// Класс для работы с БД PostgreSQL
	/// </summary>
	public class DataBaseHelper
	{
		/// <summary>
		/// Запрос для получения названия всех баз данных на сервере
		/// </summary>
		private static readonly string AllDatabaseNameCommand = "SELECT datname FROM pg_database;";

		private IConfiguration _configuration;

		/// <summary>
		/// Cловарь с данными серверов
		/// </summary>
		private IDictionary<string, ServerConfig> _serverDict;

		public DataBaseHelper()
		{
			_configuration = new ConfigurationProvider().Configuration;
			_serverDict = GetServerConnectionString();
		}

		/// <summary>
		/// Получение 
		/// </summary>
		/// <returns>Словарь </returns>
		private IDictionary<string, ServerConfig> GetServerConnectionString()
		{
			return new ConfigurationProvider().Configuration.GetSection("Servers")
				.Get<IDictionary<string, ServerConfig>>();
		}

		/// <summary>
		/// Получение списка имен серверов
		/// </summary>
		/// <returns>Список имен серверов</returns>
		public List<string> GetServerNames()
		{
			var names = new List<string>();
			var serverNames = _serverDict.Select(x => x.Key);
			foreach (var keyValuePair in serverNames)
			{
				names.Add(keyValuePair);
			}

			return names;
		}

		/// <summary>
		/// Получение строки подключения к БД
		/// </summary>
		/// <param name="serverName">Название сервера</param>
		/// <returns>Строку подключения</returns>
		private string GetConnectionString(string serverName)
		{
			var url = _serverDict[serverName].Connection;
			return url;
		}

		/// <summary>
		/// Получение размера диска на сервере
		/// </summary>
		/// <param name="serverName">Название сервера</param>
		/// <returns>Размер диска на сервере</returns>
		public double GetServerSize(string serverName)
		{
			var size = _serverDict[serverName].Size;
			return Convert.ToDouble(size);
		}

		/// <summary>
		/// Соединение с сервером PostgreSQL
		/// </summary>
		/// <param name="serverName">Название сервера</param>
		/// <returns>Соединение с сервером PostgreSQL</returns>
		public NpgsqlConnection OnConnection(string serverName)
		{
			var url = GetConnectionString(serverName);
			NpgsqlConnection connection = new NpgsqlConnection(url);
			return connection;
		}

		/// <summary>
		/// Инструкция для выполнения запроса
		/// </summary>
		/// <param name="commandName">Текс запроса</param>
		/// <param name="connection">Соединение с сервером PostgreSQL</param>
		/// <returns></returns>
		private NpgsqlCommand GetCommand(string commandName, NpgsqlConnection connection)
		{
			return new NpgsqlCommand(commandName, connection);
		}

		/// <summary>
		/// Получения резмера базы данных на сервере  в ГБ
		/// </summary>
		/// <param name="databaseName"> Название БД</param>
		/// <param name="connection">Соединение с сервером PostgreSQL</param>
		/// <returns> Зармер БД в ГБ</returns>
		public double GetSizeDatabase(string databaseName, NpgsqlConnection connection)
		{
			var command = GetCommand($"SELECT pg_size_pretty( pg_database_size( '{databaseName}' ) );", connection);
			string size;

			using (var textReader = command.ExecuteReader())
			{
				textReader.Read();
				size = textReader.GetString(0);
			}

			var _size = Convert.ToDouble(size.Split(' ')[0]) / (1024 * 1024);
			return _size;
		}

		/// <summary>
		/// Получение списка названия всех баз данных на сервере
		/// </summary>
		/// <param name="connection">Соединение с сервером PostgreSQL</param>
		/// <returns>Список названия всех БД на сервере</returns>
		public IList<string> GetAllDatabase(NpgsqlConnection connection)
		{
			var dataBasesNameList = new List<string>();

			var command = GetCommand(AllDatabaseNameCommand, connection);
			using (var textReader = command.ExecuteReader())
			{
				while (textReader.Read())
				{
					var databaseName = textReader.GetString(0);
					dataBasesNameList.Add(databaseName);
				}
			}

			return dataBasesNameList;
		}
	}
}