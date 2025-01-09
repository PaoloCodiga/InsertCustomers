using System;
using System.Windows.Forms;
using TCPOS.InsertCustomers.Forms;

namespace InsertCustomers
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new customerDataImporterForm());
        }
    }
}
