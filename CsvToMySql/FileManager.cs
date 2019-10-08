using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace CsvToMySql
{
    public class FileManager
    {
        public string FilePath { get; set; }

        public bool IsValidFilePath(string filePath)
        {
            if (File.Exists(filePath))
                return true;

            return false;
        }

        public List<string> RowReader(int pos)
        {
            string row;

            using (FileStream stream = new FileStream(FilePath, FileMode.Open, FileAccess.Read))
            {
                using (StreamReader reader = new StreamReader(stream))
                {
                    for (int i = 0; i < pos; i++)
                    {
                        reader.ReadLine();
                    }
                    row = reader.ReadLine();
                    reader.Close();
                }
                stream.Close();
            }
            row = row.Replace("\"", "");
            List<string> splittedRow = SplitRow(row);
            return splittedRow;
        }

        public int RowCounter()
        {
            int counter = 0;
            using (FileStream stream = new FileStream(FilePath, FileMode.Open, FileAccess.Read))
            {
                using (StreamReader reader = new StreamReader(stream))
                {
                    while (!reader.EndOfStream)
                    {
                        reader.ReadLine();
                        counter++;
                    }
                    reader.Close();
                }
                stream.Close();
            }
            return counter;
        }

        private List<string> SplitRow(string row)
        {
            string[] rowSplittedTemp = row.Split(';');
            List<string> rowSplitted = rowSplittedTemp.OfType<string>().ToList();
            rowSplitted.RemoveAt(rowSplitted.Count - 1);
            return rowSplitted;
        }
    }
}
