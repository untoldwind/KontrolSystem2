using System.Collections.Generic;
using System.IO;
using System.Text;
using BenchmarkDotNet.Attributes;
using KontrolSystem.Parsing;
using KontrolSystem.TO2.AST;
using KontrolSystem.TO2.Parser;

namespace KontrolSystem.Benchmarks;

public class ParserBench {
    private List<string> contents;
    
    public ParserBench() {
        contents = new List<string>();
        
        foreach (var baseDir in (string[])["../../../../../../KSP2Runtime-Test/to2KSP", "../../../../../../TO2-Test/to2Core", "../../../../../../TO2-Test/to2SelfTest", "to2Bench", "../../../../../../KSP2Runtime/to2", "../../../../../../KSP2Runtime/to2Local"]) {
            foreach (var fileName in Directory.GetFiles(baseDir, "*.to2", SearchOption.AllDirectories)) {
                if (!fileName.EndsWith(".to2")) continue;

                contents.Add(File.ReadAllText( fileName, Encoding.UTF8));
            }
        }
    }
    
    [Benchmark]
    public bool ParseFiles() {
        bool success = true;
        foreach (var content in contents) {
            success = TO2ParserModule.Module(TO2Module.BuildName("bench")).TryParse(content, "<bench>").WasSuccessful && success;
        }

        return success;
    }
}
