using System;
using System.Collections.Generic;

namespace TaskNet
{
	/// <summary>
	/// Модель строки для заполнения электронной таблицы
	/// </summary>
	public class SheetsModel
	{
		/// <summary>
		/// Название сервера
		/// </summary>
		public string ServerName { get; set;}
		/// <summary>
		/// Назавнаие БД
		/// </summary>
		public string Name { get; set; }
		/// <summary>
		/// Размер БД
		/// </summary>
		public double Size { get; set; }
		/// <summary>
		/// Дата обновления
		/// </summary>
		public string UpdateData { get; set; }

		/// <summary>
		/// Конвертирование в список объктов
		/// </summary>
		/// <returns>список объектов</returns>
		public List<object> ToListObject()
		{
			return new List<object>{this.ServerName, this.Name, this.Size, this.UpdateData};
		}
	}
}