using System;
using System.IO;
using System.Linq;
using System.Windows;
using Autodesk.AutoCAD.EditorInput;
using Excel = Microsoft.Office.Interop.Excel;



namespace Fences
{
    public class FileCreator
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

        public static void GetFromFile() //TODO Добавить поддержку рандомной локации
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
        }

        private static void Calculator(double[] lng, double[] pls, double[] brs)
        {

            //Cчитаем из файла
            //TODO Проблема в количестве секций + 
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
        //TODO Или переписать под Excel api, или вывести результаты в файл
        /*
        public static void ToExcel()
        {
            Excel.Application xlApp = new Excel.Application();
            Excel.Workbook xlWorkBook;
            Excel.Worksheet xlWorkSheet;

        }
        */
    }
}