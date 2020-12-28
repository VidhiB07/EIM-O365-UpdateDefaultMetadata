using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Newtonsoft.Json;

namespace EIM_O365_MetaDataTermSet.Data {
    public class ConfigFile
    {
        private const string DefaultFileName = "MetadataConfig.json";
        //private string _configFileName;
        
        private string CacheKey;
        private MemoryCacheManager _cacheManager;
        /// <summary>
        /// Collection of fields
        /// </summary>
        public FieldConfigCollection Fields { get; private set; }
        /// <summary>
        /// How many minutes to keep the config in cache
        /// </summary>
        public int KeepConfigCache { get; set; }
        /// <summary>
        /// Disable cache of config
        /// </summary>
        public bool DisableCache { get; set; }
        /// <summary>
        /// Not Applicable ID
        /// </summary>
        public string NotApplicableID { get; set; }
        /// <summary>
        /// Not Applicable text
        /// </summary>
        public string NotApplicableText { get; set; }
        /// <summary>
        /// Enable to add Not Applicable into process tree
        /// </summary>
        public bool AddNotApplicableInProcess { get; set; }
        /// <summary>
        /// Manager for memory cache
        /// </summary>
        [JsonIgnore]
        public MemoryCacheManager CacheManager {
            get {
                if (_cacheManager != null) return _cacheManager;
                _cacheManager = new MemoryCacheManager(CacheKey);
                return _cacheManager;
            }
            private set => _cacheManager = value;
        }

        /// <summary>
        /// Config file object using default config file name
        /// </summary>
        public ConfigFile() : this(DefaultFileName) {}
        /// <summary>
        /// Config file object
        /// </summary>
        /// <param name="configFile"></param>
        public ConfigFile(string configFilePath) {
            if (String.IsNullOrEmpty(configFilePath)) throw new NullReferenceException("configFilePath cannot be null");
            string fileName = GetFileNameFromPath(configFilePath);
            CacheKey = GetCacheKey(fileName);

            _cacheManager = new MemoryCacheManager(CacheKey);
            Fields = new FieldConfigCollection();
            KeepConfigCache = 120;
        }

        /// <summary>
        /// Load config from file
        /// </summary>
        /// <param name="configFile">Config file as string</param>
        /// <param name="saveToCache">Save data to cache as bool</param>
        /// <returns>config as ConfigFile</returns>
        public ConfigFile LoadFromFile(string configFilePath, bool saveToCache = true) {
            string fileName = GetFileNameFromPath(configFilePath);
            ConfigFile config;
            using (StreamReader file = File.OpenText(configFilePath)) {
                JsonSerializer serializer = new JsonSerializer();
                config = (ConfigFile)serializer.Deserialize(file, typeof(ConfigFile));

                if (saveToCache && !config.DisableCache) {
                    CacheManager.SetCache(config, 0, KeepConfigCache);
                }
                config.SetCacheKey(fileName);
                return config;
            }
        }
        /// <summary>
        /// Get cached config or config from default file
        /// </summary>
        /// <returns>config as ConfigFile</returns>
        public static ConfigFile GetConfig(Microsoft.Azure.WebJobs.ExecutionContext context) {
            string fullPath = context.FunctionAppDirectory + "/" + DefaultFileName;
            return GetConfig(fullPath);
        }
        /// <summary>
        /// Get cached config or config from file
        /// </summary>
        /// <param name="configFile">config file as string</param>
        /// <returns>config as ConfigFile</returns>
        public static ConfigFile GetConfig(string configFilePath) {
            ConfigFile config = new ConfigFile(configFilePath);
            string cacheKey = config.CacheKey;

            if (config.CacheManager.IsObjectInCache()) {
                var cachedData = config.CacheManager.GetCacheObject();
                if (cachedData != null) {
                    var cachedConfig = (ConfigFile)cachedData;
                    cachedConfig.CacheKey = cacheKey;
                    return cachedConfig;
                }
            }

            return config.LoadFromFile(configFilePath);
        }

        private string GetFileNameFromPath(string fullPath) {
            return Path.GetFileName(fullPath);
        }

        internal void SetCacheKey(string filename) {
            CacheKey = GetCacheKey(filename);
        }
        private string GetCacheKey(string filename) {
            return $"CKS{filename}";
        }
    }
}
