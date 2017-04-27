using System.Diagnostics;
using System.Threading;
using System.Windows.Forms;
using Autodesk.AutoCAD.Runtime;
using Fences;
using Fences.Properties;
using Application = System.Windows.Forms.Application;
using Exception = System.Exception;

[assembly: CommandClass(typeof(SimpleFences))]

namespace Fences
{
    public class SimpleFences
    {
        private readonly UserSelection _userSelection = new UserSelection();
        private readonly Cleaner _cleaner = new Cleaner();


        [CommandMethod("SimpleFencesCreate", CommandFlags.Modal)]
        public void SimpleFencesCreate()
        {
            _cleaner.CleanSettings();

            try
            {
                Application.ThreadException +=
                    delegate(object o, ThreadExceptionEventArgs args)
                    {
                        Debug.WriteLine(args.Exception.ToString());
                        MessageBox.Show(@"Exception: " + args.Exception.Message);
                    };
                _userSelection.SelectPolyline();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
                MessageBox.Show(@"Exception: " + ex.Message);
            }
        }

        [CommandMethod("SimpleFencesGetTable", CommandFlags.Modal)]
        public void SimpleFencesGetTable()
        {
            _userSelection.GetDataFromSelection();
            Settings.Default.CounterLength = 0;
            Settings.Default.CounterPils = 0;
            Settings.Default.NumEnd = 0;
        }

        [CommandMethod("SimpleFencesClear", CommandFlags.Modal)]
        public void SimpleFencesClear()
        {
            _cleaner.CleanAllPoly();
        }
    }
}