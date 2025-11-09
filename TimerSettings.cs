using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text.Json;
using Timer;

public class TimerSettings : INotifyPropertyChanged
{
    // 设置文件路径（存储在系统本地应用数据目录）
    private readonly string _settingsPath;

    // 默认值
    private int _workTime = 40;
    private int _breakTime = 10;
    private int _notificationThreshold = 5; // 提前5分钟提醒

    /// <summary>
    /// 工作时间（分钟）
    /// </summary>
    public int WorkTime
    {
        get => _workTime;
        set
        {
            if (_workTime != value) // 避免重复更新
            {
                _workTime = value;
                OnPropertyChanged();
                SaveToFile(); // 修改后自动保存
            }
        }
    }

    /// <summary>
    /// 休息时间（分钟）
    /// </summary>
    public int BreakTime
    {
        get => _breakTime;
        set
        {
            if (_breakTime != value)
            {
                _breakTime = value;
                OnPropertyChanged();
                SaveToFile();
            }
        }
    }

    /// <summary>
    /// 提前提醒阈值（分钟）
    /// </summary>
    public int NotificationThreshold
    {
        get => _notificationThreshold;
        set
        {
            if (_notificationThreshold != value)
            {
                _notificationThreshold = value;
                OnPropertyChanged();
                SaveToFile();
            }
        }
    }

    // 属性变更事件
    public event PropertyChangedEventHandler? PropertyChanged;

    /// <summary>
    /// 构造函数：初始化设置文件路径并加载配置
    /// </summary>
    public TimerSettings()
    {
        // 1. 构建设置文件路径（替换为你的应用名称）
        
        _settingsPath = Path.Combine(Config.AppFolder, "TimerSettings.json");

        // 3. 加载设置（无文件则用默认值）
        LoadFromFile();
    }

    /// <summary>
    /// 从JSON文件加载设置
    /// </summary>
    private void LoadFromFile()
    {
        try
        {
            if (File.Exists(_settingsPath))
            {
                //var json = File.ReadAllText(_settingsPath);
                //var deserialized = JsonSerializer.Deserialize<TimerSettings>(json);

                //if (deserialized != null)
                //{
                //    // 赋值给属性（触发PropertyChanged通知UI更新）
                //    WorkTime = deserialized.WorkTime;
                //    BreakTime = deserialized.BreakTime;
                //    NotificationThreshold = deserialized.NotificationThreshold;
                //}
                var json = File.ReadAllText(_settingsPath);
                // 使用 JsonDocument 解析（避免反序列化创建新实例）
                using (var doc = JsonDocument.Parse(json))
                {
                    var root = doc.RootElement;
                    // 手动读取每个属性，赋值给当前实例
                    if (root.TryGetProperty(nameof(WorkTime), out var workTimeElem))
                        _workTime = workTimeElem.GetInt32();
                    if (root.TryGetProperty(nameof(BreakTime), out var breakTimeElem))
                        _breakTime = breakTimeElem.GetInt32();
                    if (root.TryGetProperty(nameof(NotificationThreshold), out var thresholdElem))
                        _notificationThreshold = thresholdElem.GetInt32();
                }
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"加载设置失败：{ex.Message}");
            // 加载失败时保持默认值
        }
    }

    /// <summary>
    /// 将当前设置保存到JSON文件
    /// </summary>
    private void SaveToFile()
    {
        try
        {
            // 序列化当前对象（格式化输出，方便查看）
            var json = JsonSerializer.Serialize(this, new JsonSerializerOptions { WriteIndented = true });

            // 写入文件
            File.WriteAllText(_settingsPath, json);
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"保存设置失败：{ex.Message}");
        }
    }

    /// <summary>
    /// 触发属性变更事件
    /// </summary>
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}