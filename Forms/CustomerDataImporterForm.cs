using Serilog;
using System.IO;
using System.Windows.Forms;
using TCPOS.InsertCustomers.Domain;

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
            //// Configure Serilog
            Log.Logger = new LoggerConfiguration()
                .WriteTo.File($"{this.fileNameTextBox.Text}_log.txt")
                .CreateLogger();


            if (File.Exists(this.fileNameTextBox.Text))
            {
                try
                {
                    var csvFileReader = new CsvFileReader(Log.Logger);
                    csvFileReader.ReadCsvFile(this.fileNameTextBox.Text);
                }
                catch
                {
                    MessageBox.Show("An error occurred during the import process.\r\n" +
                        $"Check {this.fileNameTextBox.Text}_log.txt for detail", "Error while Reading Csv",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                }
                finally
                {
                    Log.CloseAndFlush(); //// Ensure all log events are written
                }
            }
            else
            {
                MessageBox.Show("File does not exist!", "Select Correct File...", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
