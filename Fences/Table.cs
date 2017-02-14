using System;
using System.IO;
using System.Linq;

namespace Fences
{
    public class Table
    {
        public static void ToFile(string id, double length, int pilnum) // HACK Временный говнокод, нужно улучшить
        {
            int barnum = (int)Math.Ceiling(length / 100 - pilnum);

            string path = @"C:\ToFile\table.xls";
            if (!File.Exists(path))
            {
                using (StreamWriter sw = File.CreateText(path))
                {
                    sw.WriteLine("#\tID\tLength\tNumber of pillars\tNumber of bars");
                    sw.WriteLine(1 + "\tid" + id + "\t" + length + "\t" + pilnum + "\t" + barnum);
                }
            }
            else
            {
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
            } //TODO Добавить проверку на все айдишники, а не только в последней строке

            //TODO Запилить класс, делающий расчеты
            //TODO Перенести всю работу с файлами в другой класс
        }

        public static void Calculator() //TODO Добавить поддержку рандомной локации
        {
            
        }
    }
}