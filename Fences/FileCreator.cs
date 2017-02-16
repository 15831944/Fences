using System;
using System.IO;
using System.Linq;
using System.Windows;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Microsoft.Win32;
using Application = Autodesk.AutoCAD.ApplicationServices.Application;

namespace Fences
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
        
        public static void GetFromFile(string path) //TODO FIX
        {
            Editor ed = Application.DocumentManager.MdiActiveDocument.Editor;
            string text = File.ReadAllText(path);

            int lines = TotalLines(path);
            lines--;
            string[] bits = text.Split('\n');
            MessageBox.Show(lines.ToString());
            double[] lng = new double[lines];
            double[] pls = new double[lines];
            double[] brs = new double[lines];

           //MessageBox.Show(string.Join(",", bits));
            
            for (int i = 1; i <= lines; i++)
            {
                string[] get = bits[i].Split('\t');
                
                lng[i] = Convert.ToDouble(get[2]);
                pls[i] = Convert.ToDouble(get[3]);
                brs[i] = Convert.ToDouble(get[4]);
                MessageBox.Show(string.Join(",", lng));
                MessageBox.Show(string.Join(",", pls));
                MessageBox.Show(string.Join(",", brs));
            }

            Calculator(lng, pls, brs);
        }

        private static void Calculator(double[] lng, double[] pls, double[] brs)
        {
            double total60X30X4 = lng.Sum()/1000*Properties.Settings.Default.top;
            double total40X30X4 = pls.Sum()*Properties.Settings.Default.pil*Properties.Settings.Default.pilLength + (lng.Sum()/1000 - 0.04*pls.Sum()) * Properties.Settings.Default.pil;
            double totalT10 = pls.Sum()*Properties.Settings.Default.btm;
            double totalT4 = ((lng.Length - 1) * 2)* Properties.Settings.Default.ending;
            double totalt14 = brs.Sum() * Properties.Settings.Default.barLength * Properties.Settings.Default.bar;

            CreateTable(total60X30X4/1000, total40X30X4 / 1000, totalT10 / 1000, totalT4 / 1000, totalt14 / 1000); //TODO Переделать для первых этажей
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
            str[2, 4] = t60.ToString();
            str[2, 5] = t60.ToString();
            str[4, 4] = t40.ToString();
            str[6, 4] = t10.ToString();
            str[7, 4] = t4.ToString();
            str[9, 4] = t14.ToString();

            //CellRange mcells1 = CellRange.Create(tb, 1, 1, 1, 7);
            //CellRange mcells2 = CellRange.Create(tb, 4, 1, 4, 3);
            //tb.MergeCells(mcells1);
            //tb.MergeCells(mcells2);

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
            //File.Create(saveFileDialog1.FileName).Dispose();

            return saveFileDialog1.FileName;
        }
    }
}