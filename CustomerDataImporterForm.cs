using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace InsertCustomers
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
            if (File.Exists(this.fileNameTextBox.Text))
            {
                CsvFileReader.ReadCsvFile(this.fileNameTextBox.Text);
            }
            else
            {
                MessageBox.Show("File does not exist!", "Select Correct File...", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
