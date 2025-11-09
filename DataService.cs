using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Linq;
using System.Diagnostics;


namespace Timer
{
    public class UsageRecord
    {
        public DateTime Date { get; set; }
        public TimeSpan TotalUsage { get; set; }
        public int WorkSessions { get; set; }
        public int BreakSessions { get; set; }
    }
    public class DataService
    {
        private readonly string _dataPath;
        private List<UsageRecord> _usageRecords;

        public DataService(string dataPath)
        {
            Debug.Write(dataPath);
            _dataPath = dataPath;
            _usageRecords = LoadData();
        }

        public void SaveUsageRecord(TimeSpan totalUsage, int workSessions, int breakSessions)
        {
            var today = DateTime.Today;
            var existingRecord = _usageRecords.FirstOrDefault(r => r.Date == today);

            if (existingRecord != null)
            {
                existingRecord.TotalUsage = totalUsage;
                existingRecord.WorkSessions = workSessions;
                existingRecord.BreakSessions = breakSessions;
            }
            else
            {
                _usageRecords.Add(new UsageRecord
                {
                    Date = today,
                    TotalUsage = totalUsage,
                    WorkSessions = workSessions,
                    BreakSessions = breakSessions
                });
            }

            SaveData();
        }

        public UsageRecord GetTodayUsage()
        {
            return _usageRecords.FirstOrDefault(r => r.Date == DateTime.Today);
        }

        public List<UsageRecord> GetWeeklyUsage()
        {
            var weekStart = DateTime.Today.AddDays(-(int)DateTime.Today.DayOfWeek);
            return _usageRecords.Where(r => r.Date >= weekStart).ToList();
        }

        private List<UsageRecord> LoadData()
        {
            try
            {
                if (File.Exists(_dataPath))
                {
                    var json = File.ReadAllText(_dataPath);
                    return JsonSerializer.Deserialize<List<UsageRecord>>(json) ?? new List<UsageRecord>();
                }
            }
            catch (Exception)
            {
                // 如果读取失败，返回空列表
            }
            return new List<UsageRecord>();
        }

        private void SaveData()
        {
            try
            {
                var json = JsonSerializer.Serialize(_usageRecords, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(_dataPath, json);
            }
            catch (Exception ex)
            {
                // 处理保存错误
                System.Diagnostics.Debug.WriteLine($"保存数据失败: {ex.Message}");
            }
        }
    }
}