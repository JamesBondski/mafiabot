using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using MafiaBotV2;

namespace MafiaRun
{
    public partial class MainForm : Form
    {
        bool allowCheck = false;

        public MainForm() {
            InitializeComponent();
            MainForm_Resize(this, null);

            PopulateTestList();
        }

        private void runButton_Click(object sender, EventArgs e) {
            testList.Items.Clear();
            expectedText.Text = "";
            actualText.Text = "";

            PopulateTestList();
        }

        private void PopulateTestList() {
            allowCheck = true;

            DirectoryInfo dir = new DirectoryInfo("input");
            foreach(FileInfo file in dir.GetFiles("*.txt")) {
                Bot bot = new Bot("config.xml", new MafiaBotV2.Network.File.FileMaster("input/" + file.Name, "actual/" + file.Name));
                bot.Run();
                bot.Shutdown();

                bool same = false;
                try {
                    if (File.ReadAllText("actual/" + file.Name) == File.ReadAllText("expected/" + file.Name)) {
                        same = true;
                    }
                }
                catch (FileNotFoundException ex) { }
                testList.Items.Add(file.Name, same);
            }

            allowCheck = false;
        }

        private void testList_SelectedIndexChanged(object sender, EventArgs e) {
            string selectedFile = testList.SelectedItem as string;
            if(selectedFile != null) {
                actualText.Text = File.ReadAllText("actual/" + selectedFile);
                try {
                    expectedText.Text = File.ReadAllText("expected/" + selectedFile);
                }
                catch(FileNotFoundException ex) {
                    expectedText.Text = "File not found.";
                }
            }
            
        }

        private void useButton_Click(object sender, EventArgs e) {
            string selectedFile = testList.SelectedItem as string;
            File.Copy("actual/" + selectedFile, "expected/" + selectedFile, true);

            allowCheck = true;
            testList.SetItemChecked(testList.SelectedIndex, true);
            allowCheck = false;

            expectedText.Text = actualText.Text;
        }

        private void testList_ItemCheck(object sender, ItemCheckEventArgs e) {
            if (!allowCheck) {
                e.NewValue = e.CurrentValue;
            }
        }

        private void MainForm_Resize(object sender, EventArgs e) {
            outputGroup.Width = this.Width - testsGroup.Width - 48;
            expectedGroup.Width = outputGroup.Width / 2 - 6;
            actualGroup.Left = expectedGroup.Width + 6;
            actualGroup.Width = expectedGroup.Width;

            outputGroup.Height = this.Height - runButton.Height - 64;
            actualGroup.Height = outputGroup.Height - 48 - useButton.Height;
            expectedGroup.Height = actualGroup.Height;
            useButton.Top = actualGroup.Height + 24;
            useButton.Left = actualGroup.Left;
        }
    }
}
