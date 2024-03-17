using System.IO;
using System.Text;
using KontrolSystem.TO2.Runtime;

namespace KontrolSystem.KSP.Runtime.KSPDebug;

public class DirectLogFile : KSPDebugModule.ILogFile {
    private readonly string logDirectory;
    private readonly string fileName;
    private StreamWriter? streamWriter;

    public DirectLogFile(string logDirectory, string fileName) {
        this.logDirectory = logDirectory;
        this.fileName = fileName;
    }

    public Future<object?> Log(string message) {
        return new CoreBackground.WaitComplete<object?>(EnsureWriter(true).WriteLineAsync(message).ContinueWith(_ => (object?)null));
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

    private StreamWriter EnsureWriter(bool append) {
        if (streamWriter == null) {
            if (!Directory.Exists(logDirectory))
                Directory.CreateDirectory(logDirectory);
            streamWriter = new StreamWriter(Path.Combine(logDirectory, fileName), append, Encoding.UTF8);
            streamWriter.AutoFlush = true;
        }
        return streamWriter;
    }
}

public class DelegateLogFile : KSPDebugModule.ILogFile {
    public Future<object?> Log(string message) {
        return DelegatedLogFile()?.Log(message) ?? new Future.Success<object?>(null);
    }

    public void Truncate() {
        DelegatedLogFile()?.Truncate();
    }

    private KSPDebugModule.ILogFile? DelegatedLogFile() {
        var context = KSPContext.CurrentContext;

        return context.AddLogFile(context.ProcessName + ".log");
    }
}
