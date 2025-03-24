using Newtonsoft.Json;

namespace ReportDataBuilder.JsonSettings
{
    public static class GlobalSettings
    {
        private static Settings _settings;
      
        public static Settings settings
        {
            get
            {
                if (JsonConvert.DeserializeObject<DatabaseSettings>(File.ReadAllText("./settings.json")).MySql)
                {

                    if (_settings == null)
                        _settings = JsonConvert.DeserializeObject<Settings>(File.ReadAllText("./mysql.settings.json"));
                    return _settings;
                }
                if (_settings == null)
                    _settings = JsonConvert.DeserializeObject<Settings>(File.ReadAllText("./mssql.settings.json"));
                return _settings;
            }
        }
    }
}
