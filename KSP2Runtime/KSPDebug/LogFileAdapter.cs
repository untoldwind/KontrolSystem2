using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using KontrolSystem.TO2.Runtime;

namespace KontrolSystem.KSP.Runtime.KSPDebug;

public class DirectLogFile(string logDirectory, string fileName) : KSPDebugModule.ILogFile {
    private StreamWriter? streamWriter;

    public Future<object?> Log(string message) {
        return new CoreBackground.WaitComplete<object?>(EnsureWriter(true).WriteLineAsync(message).ContinueWith(_ => (object?)null));
    }

    public Future<string[]> ReadLines() {
        return new CoreBackground.WaitComplete<string[]>(DoReadLines());
    }
    
    public void Truncate() {
        streamWriter?.Close();
        EnsureWriter(false);
    }

    public async void Close() {
        if (streamWriter != null) {
            await streamWriter.FlushAsync();
            streamWriter.Close();
        }
    }

    private Task<string[]> DoReadLines() {
        var logFile = Path.Combine(logDirectory, fileName);
        if (File.Exists(logFile)) {
            return File.ReadAllLinesAsync(logFile);
        }

        return Task.FromResult(Array.Empty<string>());
    }
    
    private StreamWriter EnsureWriter(bool append) {
        if (streamWriter == null) {
            if (!Directory.Exists(logDirectory))
                Directory.CreateDirectory(logDirectory);
            streamWriter = new StreamWriter(Path.Combine(logDirectory, fileName), append, Encoding.UTF8) {
                AutoFlush = true
            };
        }
        return streamWriter;
    }
}

public class DelegateLogFile : KSPDebugModule.ILogFile {
    public Future<object?> Log(string message) {
        return DelegatedLogFile()?.Log(message) ?? new Future.Success<object?>(null);
    }

    public Future<string[]> ReadLines() {
        return DelegatedLogFile()?.ReadLines() ?? new Future.Success<string[]>([]);
    }
    
    public void Truncate() {
        DelegatedLogFile()?.Truncate();
    }
    
    private KSPDebugModule.ILogFile? DelegatedLogFile() {
        var context = KSPContext.CurrentContext;

        return context.AddLogFile(context.ProcessName);
    }
}
