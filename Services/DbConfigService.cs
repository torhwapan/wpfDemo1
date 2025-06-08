using System;
using System.Collections.Generic;
using System.Linq;

namespace ConfigurationManager.Services
{
    public class DbConfig
    {
        public string Name { get; set; }
        
        public DbConfig(string name)
        {
            Name = name;
        }

        public override string ToString()
        {
            return Name;
        }
    }

    public class DbConfigService
    {
        private List<DbConfig> _cachedConfigs;

        public DbConfigService()
        {
            // 初始化时加载所有配置
            _cachedConfigs = GenerateDbConfigs();
        }

        private List<DbConfig> GenerateDbConfigs()
        {
            // 这里可以从实际的数据源（如数据库、配置文件等）获取数据
            // 现在我们模拟1000个DB配置
            var dbConfigs = new List<DbConfig>();
            for (int i = 1; i <= 1000; i++)
            {
                dbConfigs.Add(new DbConfig($"DB{i}"));
            }
            return dbConfigs;
        }

        public List<DbConfig> GetDbConfigurations()
        {
            return _cachedConfigs;
        }

        public List<DbConfig> SearchDbConfigurations(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                return _cachedConfigs;

            return _cachedConfigs
                .Where(config => config.Name.Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
                .ToList();
        }
    }
} 