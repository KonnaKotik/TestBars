namespace TaskNet
{
	/// <summary>
	/// Модель сервера PostgreSQL
	/// </summary>
	public class ServerConfig
	{
		/// <summary>
		/// Строка подключения к серверу PostgreSQL
		/// </summary>
		public string Connection { get; set; }
		/// <summary>
		/// Размер диска на сервере 
		/// </summary>
		public int Size { get; set; }
	}
}