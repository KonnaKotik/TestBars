using Microsoft.Extensions.Configuration;

namespace TaskNet
{
	/// <summary>
	/// Класс для сичтывания данных с файла конфигурации 
	/// </summary>
	public class ConfigurationProvider
	{
		/// <summary>
		/// Название файла конфигурации
		/// </summary>
		private const string File ="credentials.json" ;
		
		public static IConfigurationRoot Configuration { get; private set; }
		
		public ConfigurationProvider()
		{
			Configuration = new ConfigurationBuilder().AddJsonFile(File).Build();
		}
	}
}