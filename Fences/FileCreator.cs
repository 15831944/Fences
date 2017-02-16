using System;
using System.IO;
using System.Linq;
using System.Windows;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Microsoft.Win32;
using Application = Autodesk.AutoCAD.ApplicationServices.Application;

namespace Fences //TODO Реализовать нестандартный этаж
{
    public class FileCreator
    {
        //private static Database _db;
        //private static Document _doc;
        //private static Editor _ed;

        public static void ToFile(string id, double length, int pilnum, string path, int flrnum)
        {
            int barnum = (int) Math.Ceiling(length / 100 - pilnum);

            length = length * flrnum;
            pilnum = pilnum * flrnum;
            barnum = barnum * flrnum;

                Editor ed = Application.DocumentManager.MdiActiveDocument.Editor;
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
                    string x;

                    string text = File.ReadLines(path).Last();
                    string[] bits = text.Split('\t');


                    x = bits[0];
                    ed.WriteMessage(x);

                    int num = int.Parse(x);
                    if ("id" + id != bits[1])
                        num++;
                    using (StreamWriter file = new StreamWriter(path, true))
                    {
                        file.WriteLine(num + "\tid" + id + "\t" + length + "\t" + pilnum + "\t" + barnum);
                    }
                }
           }
        
        public static void GetFromFile(string path) //TODO FI
        {
            string text = File.ReadAllText(path);

            int lines = TotalLines(path);
            lines--;
            string[] bits = text.Split('\n');
            double[] lng = new double[lines];
            double[] pls = new double[lines];
            double[] brs = new double[lines];

           //MessageBox.Show(string.Join(",", bits));
            
            for (int i = 1; i <= lines; i++)
            {
                string[] get = bits[i].Split('\t');
                lng[i-1] = Convert.ToDouble(get[2]);
                pls[i-1] = Convert.ToDouble(get[3]);
                brs[i-1] = Convert.ToDouble(get[4]);
            }

            Calculator(lng, pls, brs);
        }

        private static void Calculator(double[] lng, double[] pls, double[] brs)
        {
            double total60X30X4 = lng.Sum()*Properties.Settings.Default.top * 0.000001;
            double total40X30X4 = pls.Sum()*Properties.Settings.Default.pil*Properties.Settings.Default.pilLength * 0.000001 + (lng.Sum() - 0.04*pls.Sum()) * Properties.Settings.Default.pil * 0.000001;
            double totalT10 = pls.Sum()*Properties.Settings.Default.btm * 0.000001;
            double totalT4 = ((lng.Length) * 2)* Properties.Settings.Default.ending * 0.000001;
            double totalt14 = brs.Sum() * Properties.Settings.Default.barLength * Properties.Settings.Default.bar * 0.000001;

            CreateTable(total60X30X4, total40X30X4, totalT10, totalT4, totalt14); //TODO Переделать для первых этажей
        }

        public static void CreateTable(double t60, double t40, double t10, double t4, double t14)
        {
            Document doc = Application.DocumentManager.MdiActiveDocument;
            Database db = doc.Database;
            Editor ed = doc.Editor;

            PromptPointResult pr = ed.GetPoint("\nУкажите место расположения таблицы :");
            Table tb = new Table();
            EditTablestyle();
            tb.TableStyle = db.Tablestyle;
            tb.NumRows = 14;
            tb.NumColumns = 6;
            tb.SetRowHeight(3);
            tb.SetColumnWidth(14);
            tb.Position = pr.Value;
            
            string[,] str = new string[14, 7];       
            for (int i = 0; i < tb.NumRows; i++)
            {
                for (int j = 0; j < tb.NumColumns; j++)
                {
                    str[i, j] = "-";
                }
            }
            
            str[0, 0] = "Техническая спецификация стали";
            str[1, 0] = "Вид профиля ГОСТ";
            str[1, 1] = "Марка металла ГОСТ";
            str[1, 2] = "Обозначение профиля, мм";
            str[1, 3] = "№ п/п";
            str[1, 4] = "Ограждение";
            str[1, 5] = "Общая масса, т";
            str[2, 0] = "Трубы стальные прямоугольные по ГОСТ 8645-68";
            str[2, 1] = "C255 ГОСТ 27772-2015";
            str[2, 2] = "Гн □ 60х30х4";
            str[2, 4] = t60.ToString();
            str[2, 5] = t60.ToString();
            str[3, 4] = t60.ToString();
            str[3, 5] = t60.ToString();
            str[3, 0] = "Всего профиля";
            str[4, 0] = "Стальные гнутые замкнутые сварные квадратные профили по ГОСТ 30245 - 2003";
            str[4, 1] = "C255 ГОСТ 27772-2015";
            str[4, 2] = "Гн □ 40х4";
            str[4, 4] = t40.ToString();
            str[4, 5] = t40.ToString();
            str[5, 4] = t40.ToString();
            str[5, 5] = t40.ToString();
            str[5, 0] = "Всего профиля";
            str[6, 0] = "Прокат листовой Горячекатаный ГОСТ 19903-74*";
            str[6, 1] = "C245 ГОСТ 27772-2015";
            str[6, 2] = "t 10";
            str[6, 4] = t10.ToString();
            str[6, 5] = t10.ToString();
            str[7, 2] = "t 4";
            str[7, 4] = t4.ToString();
            str[7, 5] = t4.ToString();
            str[8, 4] = (t4+t10).ToString();
            str[8, 5] = (t4 + t10).ToString();
            str[8, 0] = "Всего профиля";
            str[9, 0] = "Прокат стальной горячекатаный квадратный ГОСТ 2591-88";
            str[9, 1] = "C245 ГОСТ 27772-2015";
            str[9, 2] = "■ 14";
            str[9, 4] = t14.ToString();
            str[9, 5] = t14.ToString();
            str[10, 4] = t14.ToString();
            str[10, 5] = t14.ToString();
            str[10, 0] = "Всего профиля";
            str[11, 4] = (t14+t4+t10+t40+t60).ToString();
            str[11, 5] = (t14 + t4 + t10 + t40 + t60).ToString();
            str[11, 0] = "Всего масса материала по обьекту";
            str[12, 0] = "В том числе по маркам стали";
            str[12, 2] = "С255";
            str[12, 4] = (t40 + t60).ToString();
            str[12, 5] = (t40 + t60).ToString();
            str[13, 2] = "С245";
            str[13, 4] = (t14 + t4 + t10).ToString();
            str[13, 5] = (t14 + t4 + t10).ToString();

            for (int i = 0; i < 12; i++)
            {
                str[i + 2, 3] = (i + 1).ToString();
            }

            CellRange mcells1 = CellRange.Create(tb, 3, 0, 3, 2); //TODO Наверняка можно сделать проще
            tb.MergeCells(mcells1);
            CellRange mcells2 = CellRange.Create(tb, 5, 0, 5, 2);
            tb.MergeCells(mcells2);
            CellRange mcells3 = CellRange.Create(tb, 8, 0, 8, 2);
            tb.MergeCells(mcells3);
            CellRange mcells4 = CellRange.Create(tb, 10, 0, 10, 2);
            tb.MergeCells(mcells4);
            CellRange mcells5 = CellRange.Create(tb, 11, 0, 11, 2);
            tb.MergeCells(mcells5);
            CellRange mcells6 = CellRange.Create(tb, 11, 0, 11, 2);
            tb.MergeCells(mcells6);
            CellRange mcells7 = CellRange.Create(tb, 12, 0, 13, 1);
            tb.MergeCells(mcells7);
            CellRange mcells8 = CellRange.Create(tb, 6, 0, 7, 0);
            tb.MergeCells(mcells8);
            CellRange mcells9 = CellRange.Create(tb, 6, 1, 7, 1);
            tb.MergeCells(mcells9);

            for (int i = 0; i < 14; i++)
            {
                for (int j = 0; j < 6; j++)
                {
                    tb.SetTextHeight(i, j, 1);
                    tb.SetTextString(i, j, str[i, j]);
                    tb.SetAlignment(i, j, CellAlignment.MiddleCenter);
                }
            }
            tb.GenerateLayout();

            Transaction tr =
                doc.TransactionManager.StartTransaction();
            using (tr)
            {
                BlockTable bt =
                    (BlockTable) tr.GetObject(
                        doc.Database.BlockTableId,
                        OpenMode.ForRead
                    );
                BlockTableRecord btr =
                    (BlockTableRecord) tr.GetObject(
                        bt[BlockTableRecord.ModelSpace],
                        OpenMode.ForWrite
                    );
                btr.AppendEntity(tb);
                tr.AddNewlyCreatedDBObject(tb, true);
                tr.Commit();
            }
        }

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

            return saveFileDialog1.FileName;
        }
    }
}