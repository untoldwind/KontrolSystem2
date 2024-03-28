using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KontrolSystem.GenDocs;

public class TableHelper(params string[] columns) {
    private readonly List<string[]> rows = [];

    public void AddRow(params string[] row) => rows.Add(row);

    public string Markdown {
        get {
            var sizes = new int[columns.Length];

            for (int i = 0; i < sizes.Length; i++) {
                sizes[i] = rows.Select(row => row[i].Length).Append(columns[i].Length).Append(3).Max();
            }

            var builder = new StringBuilder();

            for (int i = 0; i < sizes.Length; i++) {
                builder.Append("| ").Append(columns[i].PadRight(sizes[i])).Append(" ");
            }

            builder.Append("|\n");
            for (int i = 0; i < sizes.Length; i++) {
                builder.Append("| ").Append("---".PadRight(sizes[i], '-')).Append(" ");
            }
            builder.Append("|\n");

            foreach (var row in rows) {
                for (int i = 0; i < sizes.Length; i++) {
                    builder.Append("| ").Append(row[i].PadRight(sizes[i])).Append(" ");
                }
                builder.Append("|\n");
            }

            return builder.ToString();
        }
    }
}
