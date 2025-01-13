using Serilog;
using System;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using TCPOS.InsertCustomers.Domain;
using TCPOS.InsertCustomers.Utils;

namespace TCPOS.InsertCustomers.Forms
{
    public partial class customerDataImporterForm : Form
    {
        OpenFileDialog openFileDialog = new OpenFileDialog();

        public customerDataImporterForm()
        {
            InitializeComponent();

            this.searchFileButton.Click += (sender, e) => SearchFileButton_Click();
            this.importButton.Click += (sender, e) => ImportButton_Click();
        }

        private void SearchFileButton_Click()
        {
            this.openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "*.csv|*.CSV";
            openFileDialog.RestoreDirectory = true;

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                this.fileNameTextBox.Text = openFileDialog.FileName;
            }
        }

        private void ImportButton_Click()
        {
            LogUtil.InitLogger();
            Log.Logger.Information($"**************************************************");
            Log.Logger.Information($"*****    Program InsertCustomers started     *****");
            Log.Logger.Information($"**************************************************");
            Log.Logger.Information($"Importing process starts....");

            if (File.Exists(this.fileNameTextBox.Text))
            {
                try
                {
                    var csvFileReader = new CsvFileReader();
                    csvFileReader.ReadCsvFile(this.fileNameTextBox.Text);

                    Log.Logger.Information($"Importing process completes....");

                    MessageBox.Show("Importing has completed!", "Importing Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch
                {
                    MessageBox.Show("An error occurred during the import process.\r\n" +
                        $"Check {Environment.CurrentDirectory + ConfigurationManager.AppSettings["logFile"]}DebugLogs-{DateTime.Today:yyyyMMdd}.Log for detail", "Error",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                }
                finally
                {
                    //// Ensure all log events are written
                    Log.CloseAndFlush();
                }
            }
            else
            {
                Log.Logger.Error($"Importing process fails because file not exists....");

                //// Ensure all log events are written
                Log.CloseAndFlush();

                MessageBox.Show("File does not exist!", "Select Correct File...", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}