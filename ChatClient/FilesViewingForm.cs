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
using System.Windows.Forms.VisualStyles;

namespace ChatClient
{
    public partial class FilesViewingForm : Form
    {
        HTTPClientService HttpClientService;
        MatchingFiles MatchingFiles;
        public FilesViewingForm(HTTPClientService httpService, List<int> filesList)
        {
            InitializeComponent();
            HttpClientService = httpService;
            MatchingFiles = new MatchingFiles();
            FillListBox(filesList);
        }

        private async void FillListBox(List<int> filesList)
        {
            foreach (var fileID in filesList)
            {
                var fileInformation = await HttpClientService.HeadRequest("http://" + ClientForm.HttpServerDomain + ":" +
                        ClientForm.HttpServerPort.ToString() + "/" + fileID.ToString());
                MatchingFiles.AddFile(lbFiles.Items.Add(fileInformation.Name + "  " + fileInformation.Size), fileID);
            }
        }

        private async void btDownload_Click(object sender, EventArgs e)
        {
            if (lbFiles.SelectedIndex != -1)
            {
                if (saveFileDialog.ShowDialog() == DialogResult.Cancel)
                    return;

                try
                {
                    byte[] content = await HttpClientService.GetRequest("http://" + ClientForm.HttpServerDomain + ":" +
                        ClientForm.HttpServerPort.ToString() + "/" + MatchingFiles.IdMatchingDictionary[lbFiles.SelectedIndex].ToString());
                    FilesService.WriteFile(content, saveFileDialog.FileName);
                }
                catch (FileNotFoundException exception)
                {
                    MessageBox.Show(exception.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }
        }
    }
}
