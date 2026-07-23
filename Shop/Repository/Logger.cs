using System.Reflection;

namespace UpdatePractice.Services
{
    public class AdvancedLogger : IDisposable
    {
        public enum LogLevel
        {
            Debug,
            Info,
            Warning,
            Error
        }

        private LogLevel _minimumLevel = LogLevel.Info;
        private string _logFilePath;
        private readonly bool _logToFile;
        private readonly bool _logToConsole;
        private readonly object _lockObject = new object();
        private readonly string _sessionStartTime;
        private readonly long _maxFileSizeInBytes;
        private readonly int _logsRetentionDays;

        public AdvancedLogger(bool logToConsole = true, bool logToFile = true,
            long maxFileSizeMB = 500, int logsRetentionDays = 7)
        { 
            _logToConsole = logToConsole;
            _logToFile = logToFile;
            _maxFileSizeInBytes = maxFileSizeMB * 1024 * 1024;
            _logsRetentionDays = logsRetentionDays;
            _sessionStartTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

            if (_logToFile)
            {
                CleanOldLogs();

                _logFilePath = GetOrCreateLogFilePath();
                var directory = Path.GetDirectoryName(_logFilePath);
                if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                    Info($"Создана директория для логов: {directory}");
                }
                // Проверяем размер файла при запуске
                CheckAndRotateFileIfNeeded();

                // Записываем информацию о начале сессии
                Info($"=== Лог-сессия начата: {_sessionStartTime} ===");
                Info($"=== Файл лога: {Path.GetFileName(_logFilePath)} ===");
            }
        }

        private void CheckAndRotateFileIfNeeded()
        {
            if (!File.Exists(_logFilePath))
                return;

            try
            {
                var fileInfo = new FileInfo(_logFilePath);
                if (fileInfo.Length > _maxFileSizeInBytes)
                {
                    Info($"Файл лога достиг максимального размера ({fileInfo.Length / 1024 / 1024} МБ). Создается новый файл.");
                    RotateLogFile();
                }
            }
            catch (Exception ex)
            {
                if (_logToConsole)
                {
                    Console.WriteLine($"Ошибка при проверке размера файла: {ex.Message}");
                }
            }
        }
        private string GetOrCreateLogFilePath()
        {
            // Получаем путь к папке с приложением
            string appDirectory = AppDomain.CurrentDomain.BaseDirectory;

            // Формируем путь к папке logs
            string logsDirectory = Path.Combine(appDirectory, "logs");

            // Создаем папку если её нет
            if (!Directory.Exists(logsDirectory))
            {
                Directory.CreateDirectory(logsDirectory);
            }

            // Ищем файл лога за сегодня
            string todayDate = DateTime.Now.ToString("yyyy-MM-dd");
            string todayLogPattern = $"log_{todayDate}_*.txt";

            try
            {
                var todayLogs = Directory.GetFiles(logsDirectory, todayLogPattern)
                    .OrderByDescending(f => f) // Берем самый свежий файл за сегодня
                    .ToList();

                if (todayLogs.Any())
                {
                    // Если есть файл за сегодня, используем его
                    string existingLogFile = todayLogs.First();
                    Info($"Найден существующий лог-файл за сегодня: {Path.GetFileName(existingLogFile)}");
                    return existingLogFile;
                }
            }
            catch (Exception ex)
            {
                // Если произошла ошибка при поиске, просто создадим новый файл
                if (_logToConsole)
                {
                    Console.WriteLine($"Ошибка при поиске лог-файла: {ex.Message}");
                }
            }

            // Если файла за сегодня нет, создаем новый с временем создания
            string currentTime = DateTime.Now.ToString("HH-mm-ss");
            string fileName = $"log_{todayDate}_{currentTime}.txt";

            return Path.Combine(logsDirectory, fileName);
        }
        private void RotateLogFile()
        {
            try
            {
                // Создаем новый файл с текущим временем
                string todayDate = DateTime.Now.ToString("yyyy-MM-dd");
                string currentTime = DateTime.Now.ToString("HH-mm-ss");
                string newFileName = $"log_{todayDate}_{currentTime}.txt";

                string appDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                string logsDirectory = Path.Combine(appDirectory, "logs");
                string newFilePath = Path.Combine(logsDirectory, newFileName);

                // Записываем информацию о ротации в старый файл
                File.AppendAllText(_logFilePath,
                    $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}] [INFO] === Файл достиг максимального размера, создается новый файл: {newFileName} ===\n");

                // Обновляем путь к файлу
                _logFilePath = newFilePath;

                Info($"Создан новый лог-файл: {newFileName}");
            }
            catch (Exception ex)
            {
                if (_logToConsole)
                {
                    Console.WriteLine($"Ошибка при ротации файла: {ex.Message}");
                }
            }
        }
        public void SetMinimumLevel(LogLevel level) => _minimumLevel = level;

        public void Debug(string message) => Log(LogLevel.Debug, message);
        public void Info(string message) => Log(LogLevel.Info, message);
        public void Warning(string message) => Log(LogLevel.Warning, message);
        public void Error(string message) => Log(LogLevel.Error, message);
        public void Error(Exception ex, string message = null)
        {
            string errorMessage = message == null
                ? $"{ex.GetType().Name}: {ex.Message}"
                : $"{message}\n{ex.GetType().Name}: {ex.Message}";

            Log(LogLevel.Error, $"{errorMessage}\n{ex.StackTrace}");
        }

        private void CleanOldLogs()
        {
            try
            {
                string appDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                string logsDirectory = Path.Combine(appDirectory, "logs");

                if (!Directory.Exists(logsDirectory))
                    return;

                var cutoffDate = DateTime.Now.AddDays(-_logsRetentionDays);
                var oldLogs = Directory.GetFiles(logsDirectory, "log_*.txt")
                    .Where(file =>
                    {
                        try
                        {
                            var fileInfo = new FileInfo(file);
                            return fileInfo.LastWriteTime < cutoffDate;
                        }
                        catch
                        {
                            return false;
                        }
                    })
                    .ToList();

                if (oldLogs.Any())
                {
                    Info($"Найдено {oldLogs.Count} старых лог-файлов старше {_logsRetentionDays} дней");

                    foreach (var oldLog in oldLogs)
                    {
                        try
                        {
                            var fileName = Path.GetFileName(oldLog);
                            File.Delete(oldLog);
                            Info($"Удален старый лог-файл: {fileName}");
                        }
                        catch (Exception ex)
                        {
                            if (_logToConsole)
                            {
                                Console.WriteLine($"Ошибка при удалении файла {oldLog}: {ex.Message}");
                            }
                        }
                    }
                }
                else
                {
                    Info($"Старых лог-файлов старше {_logsRetentionDays} дней не найдено");
                }
            }
            catch (Exception ex)
            {
                if (_logToConsole)
                {
                    Console.WriteLine($"Ошибка при очистке старых логов: {ex.Message}");
                }
            }
        }
        private void Log(LogLevel level, string message)
        {
            if (level < _minimumLevel) return;

            var timestamp = DateTime.Now;
            var logEntry = $"[{timestamp:yyyy-MM-dd HH:mm:ss.fff}] [{level.ToString().ToUpper()}] {message}";

            if (_logToConsole)
            {
                var originalColor = Console.ForegroundColor;
                Console.ForegroundColor = GetColor(level);
                Console.WriteLine(logEntry);
                Console.ForegroundColor = originalColor;
            }

            if (_logToFile)
            {
                lock (_lockObject)
                {
                    try
                    {
                        File.AppendAllText(_logFilePath, logEntry + Environment.NewLine);
                    }
                    catch (Exception ex)
                    {
                        // Если не удалось записать в файл, выводим ошибку в консоль
                        if (_logToConsole)
                        {
                            var originalColor = Console.ForegroundColor;
                            Console.ForegroundColor = ConsoleColor.Red;
                            //Console.WriteLine($"[ERROR] Не удалось записать в лог-файл: {ex.Message}");
                            Console.ForegroundColor = originalColor;
                        }
                    }
                }
            }
        }

        public async Task LogAsync(LogLevel level, string message)
        {
            if (level < _minimumLevel) return;

            var timestamp = DateTime.Now;
            var logEntry = $"[{timestamp:yyyy-MM-dd HH:mm:ss.fff}] [{level.ToString().ToUpper()}] {message}";

            if (_logToConsole)
            {
                await Console.Out.WriteLineAsync(logEntry);
            }

            if (_logToFile)
            {
                try
                {
                    await File.AppendAllTextAsync(_logFilePath, logEntry + Environment.NewLine);
                }
                catch (Exception ex)
                {
                    if (_logToConsole)
                    {
                        await Console.Error.WriteLineAsync($"[ERROR] Не удалось записать в лог-файл: {ex.Message}");
                    }
                }
            }
        }

        private ConsoleColor GetColor(LogLevel level)
        {
            return level switch
            {
                LogLevel.Debug => ConsoleColor.DarkGray,
                LogLevel.Info => ConsoleColor.White,
                LogLevel.Warning => ConsoleColor.Yellow,
                LogLevel.Error => ConsoleColor.Red,
                _ => ConsoleColor.White
            };
        }

        public string GetCurrentLogFilePath()
        {
            return _logFilePath;
        }

        public string GetCurrentLogFileName()
        {
            return Path.GetFileName(_logFilePath);
        }

        public void Dispose()
        {
            if (_logToFile)
            {
                Info($"=== Лог-сессия завершена: {DateTime.Now:yyyy-MM-dd HH:mm:ss} ===");
                Info($"Лог-файл: {GetCurrentLogFileName()}");
            }
        }
    }

    public static class Logger
    {
        private static AdvancedLogger _instance;
        private static readonly object _lockObject = new object();

        public static AdvancedLogger Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lockObject)
                    {
                        if (_instance == null)
                        {
                            _instance = new AdvancedLogger(logToConsole: true, logToFile: true);
                        }
                    }
                }
                return _instance;
            }
        }

        public static void Initialize(bool logToConsole = true, bool logToFile = true,
            AdvancedLogger.LogLevel minLevel = AdvancedLogger.LogLevel.Info)
        {
            if (_instance != null)
            {
                _instance.Dispose();
            }

            _instance = new AdvancedLogger(logToConsole, logToFile);
            _instance.SetMinimumLevel(minLevel);
        }

        public static void Shutdown()
        {
            _instance?.Dispose();
            _instance = null;
        }

        // Удобные методы для прямого доступа
        public static void Debug(string message) => Instance.Debug(message);
        public static void Info(string message) => Instance.Info(message);
        public static void Warning(string message) => Instance.Warning(message);
        public static void Error(string message) => Instance.Error(message);
        public static void Error(Exception ex, string message = null) => Instance.Error(ex, message);
    }
}