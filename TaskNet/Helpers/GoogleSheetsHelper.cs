using System;
using System.Collections.Generic;
using System.IO;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using static Google.Apis.Sheets.v4.SpreadsheetsResource;
using ValueInputOptionEnum =
	Google.Apis.Sheets.v4.SpreadsheetsResource.ValuesResource.UpdateRequest.ValueInputOptionEnum;

namespace TaskNet
{
	/// <summary>
	/// Класс для работы с Gppgle Sheets
	/// </summary>
	public class GoogleSheetsHelper
	{
		private static string ClientSecret = "credentials.json";

		private static readonly string[] ScopesSheets = {SheetsService.Scope.Spreadsheets};
		/// <summary>
		/// Id электронной таблицы
		/// </summary>
		private static string _spreadSheetsId;
		/// <summary>
		/// Сервис для работы с Google Sheets API
		/// </summary>
		public SheetsService Service { get; private set; }

		/// <summary>
		/// Диапозон значения для заполнения строки наименования столбцов 
		/// </summary>
		private const string WriteBaseRange = "A1:D1";

		/// <summary>
		/// Значение начала заполнения строк данными на новом листе
		/// </summary>
		private const int InitialRowIndex = 2;

		/// <summary>
		/// Счетчик заполненных строк в таблице на одном листе
		/// </summary>
		private int _count;

		/// <summary>
		/// Список значения строк наименования столбцов
		/// </summary>
		private readonly List<object> _baseData = new List<object>()
		{
			"Сервер", "База данных", "Размер ГБ", "Дата обновления"
		};
		

		public GoogleSheetsHelper(List<string> sheets)
		{
			Service = GetSheetsService();
			GetSpreadSheetsId(sheets);
		}


		/// <summary>
		/// Метод получения адреса электронной таблицы
		/// </summary>
		/// <param name="sheets">Список названия листов в Google Tables</param>
		private void GetSpreadSheetsId(List<string> sheets)
		{
			_spreadSheetsId = ConfigurationProvider.Configuration.GetSection("Sheets")["spreadSheetsId"];
			if (string.IsNullOrEmpty(_spreadSheetsId))
			{
				CreateNewGoogleSheet(sheets);
			}

			Console.WriteLine($"Spread Sheets ID: {_spreadSheetsId}");
			Console.WriteLine($"Path: https://docs.google.com/spreadsheets/d/{_spreadSheetsId}/edit?usp=sharing");
		}

		/// <summary>
		/// Метод для создания новой документа Google Tables с листами 
		/// </summary>
		/// <param name="sheets">Список названия листов в Google Tables</param>
		private void CreateNewGoogleSheet(List<string> sheets)
		{
			var newSpreadsheet = new Spreadsheet {Properties = new SpreadsheetProperties(), Sheets = GetSheets(sheets)};
			newSpreadsheet.Properties.Title = "Test Spread Sheet";
			var newSheet = Service.Spreadsheets.Create(newSpreadsheet).Execute();
			_spreadSheetsId = newSheet.SpreadsheetId;
		}

		/// <summary>
		/// Метод для получения списка листов Google Tables
		/// </summary>
		/// <param name="sheets">Список названия листов в Google Tables</param>
		/// <returns>Список листов в Google Tables</returns>
		private List<Sheet> GetSheets(List<string> sheets)
		{
			var sheetList = new List<Sheet>();
			foreach (var sheetName in sheets)
			{
				var sheet = new Sheet {Properties = new SheetProperties()};
				sheet.Properties.Title = sheetName;
				sheetList.Add(sheet);
			}

			return sheetList;
		}


		/// <summary>
		/// Создание сервиса Google Sheets API
		/// </summary>
		/// <returns>Сервис Google Sheets API </returns>
		// Create Google Sheets API service.
		private SheetsService GetSheetsService()
		{
			using (var stream = new FileStream(ClientSecret, FileMode.Open, FileAccess.Read))
			{
				var serviceInitializer = new BaseClientService.Initializer
				{
					HttpClientInitializer = GoogleCredential.FromStream(stream).CreateScoped(ScopesSheets)
				};
				return new SheetsService(serviceInitializer);
			}
		}


		/// <summary>
		/// Заполнение строки наименования столбцов 
		/// </summary>
		/// <param name="valuesResource">Значение ресурса электронной таблицы</param>
		/// <param name="sheet"> Значение листа для заполнения</param>
		public void FillBaseRows(ValuesResource valuesResource, string sheet)
		{
			_count = InitialRowIndex;
			var valueRange = new ValueRange {Values = new List<IList<object>> {_baseData}};
			var baseRangeAndSheet = sheet + "!" + WriteBaseRange;
			var update = valuesResource.Update(valueRange, _spreadSheetsId, baseRangeAndSheet);

			update.ValueInputOption = ValueInputOptionEnum.RAW;
			var response = update.Execute(); }

		/// <summary>
		/// Заполнение строк новыми данными
		/// </summary>
		/// <param name="valuesResource">Значение ресурса электронной таблицы</param>
		/// <param name="data">Список объектов для заполнения в таблицу</param>
		/// <param name="sheet">Значение листа для заполнения</param>
		public void FillRows(ValuesResource valuesResource, List<object> data, string sheet)
		{
			var value = new ValueRange
				{Values = new List<IList<object>> {data}};
			var coordinate = sheet + "!A" + _count + ":D" + _count;
			var update = valuesResource.Update(value, _spreadSheetsId, coordinate);

			update.ValueInputOption = ValueInputOptionEnum.RAW;
			var response = update.Execute(); 
			_count++;
		}
	}
}