using System;
using System.IO;
using System.Linq;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Microsoft.Win32;

namespace Fences
{
    public class FileCreator
    {
        public static void ToFile(string id, double length, int pilnum, string path, int flrnum)
            // HACK Временный вариант, нужно улучшить
        {
            int barnum = (int) Math.Ceiling(length / 100 - pilnum);

            length = length * flrnum;
            pilnum = pilnum * flrnum;
            barnum = barnum * flrnum;

            using (StreamWriter sw = File.CreateText(path))
            {
                sw.WriteLine("#\tID\tLength\tNumber of pillars\tNumber of bars");
                sw.WriteLine(1 + "\tid" + id + "\t" + length + "\t" + pilnum + "\t" + barnum);
            }

            string text = File.ReadLines(path).Last();
            string[] bits = text.Split('\t');

            string x = bits[0];

            int num = int.Parse(x);
            if ("id" + id != bits[1])
                num++;
            using (StreamWriter file = new StreamWriter(path, true))
            {
                file.WriteLine(num + "\tid" + id + "\t" + length + "\t" + pilnum + "\t" + barnum);
            }
        }

        public static void GetFromFile(string path)
        {
            string text = File.ReadAllText(path);

            int lines = TotalLines(path);

            double[] lng = new double[lines];
            double[] pls = new double[lines];
            double[] brs = new double[lines];

            string[] bits = text.Split('\n');

            for (int i = 1; i < lines; i++)
            {
                string[] get = bits[i].Split('\t');
                lng[i - 1] = Convert.ToDouble(get[3]);
                pls[i - 1] = Convert.ToDouble(get[4]);
                brs[i - 1] = Convert.ToDouble(get[5]);
            }
            Calculator(lng, pls, brs);
        }

        private static void Calculator(double[] lng, double[] pls, double[] brs)
        {
            double totalLng = lng.Sum();
            double totalPls = pls.Sum();
            double totalBrs = brs.Sum();
        }

        public static void CreateTable() //TODO Сделать метод с таблицей
        {
            PromptPointResult pr = _ed.GetPoint("\nУкажите место расположения таблицы :");
            Table tb = new Table();
            EditTablestyle();
            tb.TableStyle = _db.Tablestyle;
            tb.NumRows = 19;
            tb.NumColumns = 7;
            tb.SetRowHeight(3);
            tb.SetColumnWidth(15);
            tb.Position = pr.Value;
        }

        //TODO Придумать как передавать массу металла
        public static void EditTablestyle() //TODO Сделать метод со стилем таблицы
        {
        }

        public static int TotalLines(string filePath)
        {
            using (StreamReader r = new StreamReader(filePath))
            {
                int i = 0;
                while (r.ReadLine() != null) i++;
                return i;
            }
        }

        public static string OpenFile()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog {Filter = "Текстовые файлы (*.txt) | *.txt"};
            openFileDialog.ShowDialog();
            return openFileDialog.FileName;
        }

        public static string CreateFile()
        {
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            saveFileDialog1.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";
            saveFileDialog1.ShowDialog();
            File.Create(saveFileDialog1.FileName).Dispose();

            return saveFileDialog1.FileName;
        }
    }
}