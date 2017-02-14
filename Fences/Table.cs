using System;
using System.IO;
using System.Linq;
using Autodesk.AutoCAD.EditorInput;

namespace Fences
{
    public class Table
    {
        public const string Path = @"C:\ToFile\table.xls";

        public static void ToFile(string id, double length, int pilnum) // HACK Временный вариант, нужно улучшить
        {
            int barnum = (int)Math.Ceiling(length / 100 - pilnum);

            if (!File.Exists(Path))
            {
                using (StreamWriter sw = File.CreateText(Path))
                {
                    sw.WriteLine("#\tID\tLength\tNumber of pillars\tNumber of bars");
                    sw.WriteLine(1 + "\tid" + id + "\t" + length + "\t" + pilnum + "\t" + barnum);
                }
            }
            else
            {
                string text = File.ReadLines(Path).Last();
                string[] bits = text.Split('\t');

                string x = bits[0];

                int num = int.Parse(x);
                if ("id" + id != bits[1])
                    num++;
                using (StreamWriter file = new StreamWriter(Path, true))
                {
                    file.WriteLine(num + "\tid" + id + "\t" + length + "\t" + pilnum + "\t" + barnum);
                }
            } //TODO Добавить проверку на все айдишники, а не только в последней строке
        }

        public static void Calculator() //TODO Добавить поддержку рандомной локации
        {
            Editor ed = Autodesk.AutoCAD.ApplicationServices.Core.Application.DocumentManager.MdiActiveDocument.Editor;
            ed.WriteMessage("It works\n");

            if (!File.Exists(Path))
            {
                throw new ArgumentException("Файла не существует");
            }
            else
            {
                string text = File.ReadAllText(Path);
                
                int lines = TotalLines(Path);
                
                double[] lng = new double[lines];
                double[] brs = new double[lines];

                string[] bits = text.Split('\n');

                for (int i = 1; i < lines; i++)
                {
                    string[] get = bits[i].Split('\t');
                    lng[i - 1] = Convert.ToDouble(get[3]);
                    brs[i - 1] = Convert.ToDouble(get[4]);
                }
                //TODO Создаем новый файл для записи расчетов
            }
        }

         public static int TotalLines(string filePath)
        {
            using (StreamReader r = new StreamReader(filePath))
            {
                int i = 0;
                while (r.ReadLine() != null) { i++; }
                return i;
            }
        }
    }
}