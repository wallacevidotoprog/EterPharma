using DocumentFormat.OpenXml.ExtendedProperties;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EterPharma.Services
{
	public class IniFile
	{
		private readonly string path;
		private readonly Dictionary<string, Dictionary<string, string>> data;

		public IniFile(string path)
		{
			this.path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, path);  
			data = new Dictionary<string, Dictionary<string, string>>(StringComparer.OrdinalIgnoreCase);

			if (File.Exists(path))
			{
				var lines = File.ReadAllLines(path);
				Dictionary<string, string> section = null;

				foreach (var line in lines)
				{
					var trimmedLine = line.Trim();
					if (string.IsNullOrWhiteSpace(trimmedLine) || trimmedLine.StartsWith(";"))
						continue;

					if (trimmedLine.StartsWith("[") && trimmedLine.EndsWith("]"))
					{
						var sectionName = trimmedLine.Substring(1, trimmedLine.Length - 2).Trim();
						section = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
						data[sectionName] = section;
					}
					else if (section != null)
					{
						var keyValuePair = trimmedLine.Split(new[] { '=' }, 2);
						if (keyValuePair.Length == 2)
						{
							var key = keyValuePair[0].Trim();
							var value = keyValuePair[1].Trim();
							section[key] = value;
						}
					}
				}
			}
			else
			{
				File.Create(path).Dispose();
			}
		}

		public string Read(string section, string key, string defaultValue = "")
		{
			if (data.TryGetValue(section, out var sectionData) && sectionData.TryGetValue(key, out var value))
			{
				return value;
			}
			return defaultValue;
		}

		public void Write(string section, string key, string value)
		{
			if (!data.TryGetValue(section, out var sectionData))
			{
				sectionData = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
				data[section] = sectionData;
			}

			sectionData[key] = value;

			var lines = new List<string>();
			foreach (var sectionPair in data)
			{
				lines.Add($"[{sectionPair.Key}]");
				lines.AddRange(sectionPair.Value.Select(pair => $"{pair.Key}={pair.Value}"));
			}

			File.WriteAllLines(path, lines);
		}

		//public void Create(string section, string key, string value)
		//{

		//	File.WriteAllLines(path, lines);
		//}
	}
}
