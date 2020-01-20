using System;
using System.Collections.Generic;
using System.Threading;

namespace TaskNet
{
	internal class Program
	{
		/// <summary>
		/// Список названия серверов
		/// </summary>
		private static List<string> _serverNames;

		/// <summary>
		/// Промежуток времени (в миллисекундах), после которого обновляются данные
		/// </summary>
		private const int MillisecondsTimeout = 60000;

		public static void Main(string[] args)
		{
			var bdConnection = new DataBaseHelper();
			_serverNames = bdConnection.GetServerNames();
			var googleSheetsHelper = new GoogleSheetsHelper(_serverNames);
			var serviceValue = googleSheetsHelper.Service.Spreadsheets.Values;

			while (true)
			{
				foreach (var server in _serverNames)
				{
					using (var connection = bdConnection.OnConnection(server))
					{
						connection.Open();
						var diskSize = 0.0;
						googleSheetsHelper.FillBaseRows(serviceValue, server);
						var name = bdConnection.GetAllDatabase(connection);
						foreach (var database in name)
						{
							var size = bdConnection.GetSizeDatabase(database, connection);
							SheetsModel sheetsModel = new SheetsModel
							{
								ServerName = server,
								Name = database,
								Size = Math.Round(size, 4),
								UpdateData = DateTime.Now.ToShortDateString()
							};
							diskSize += sheetsModel.Size;
							googleSheetsHelper.FillRows(serviceValue, sheetsModel.ToListObject(), server);
						}

						SheetsModel endField = new SheetsModel
						{
							ServerName = server,
							Name = "Свободно",
							Size = bdConnection.GetServerSize(server) - diskSize,
							UpdateData = DateTime.Now.ToShortDateString()
						};
						googleSheetsHelper.FillRows(serviceValue, endField.ToListObject(), server);
						
					}
				}
				Thread.Sleep(MillisecondsTimeout);
			}
		}
	}
}