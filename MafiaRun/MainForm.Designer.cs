namespace MafiaRun
{
    partial class MainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.testsGroup = new System.Windows.Forms.GroupBox();
            this.testList = new System.Windows.Forms.CheckedListBox();
            this.outputGroup = new System.Windows.Forms.GroupBox();
            this.useButton = new System.Windows.Forms.Button();
            this.actualGroup = new System.Windows.Forms.GroupBox();
            this.actualText = new System.Windows.Forms.RichTextBox();
            this.expectedGroup = new System.Windows.Forms.GroupBox();
            this.expectedText = new System.Windows.Forms.RichTextBox();
            this.runButton = new System.Windows.Forms.Button();
            this.testsGroup.SuspendLayout();
            this.outputGroup.SuspendLayout();
            this.actualGroup.SuspendLayout();
            this.expectedGroup.SuspendLayout();
            this.SuspendLayout();
            // 
            // testsGroup
            // 
            this.testsGroup.Controls.Add(this.testList);
            this.testsGroup.Location = new System.Drawing.Point(12, 12);
            this.testsGroup.Name = "testsGroup";
            this.testsGroup.Size = new System.Drawing.Size(159, 387);
            this.testsGroup.TabIndex = 0;
            this.testsGroup.TabStop = false;
            this.testsGroup.Text = "Tests";
            // 
            // testList
            // 
            this.testList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.testList.FormattingEnabled = true;
            this.testList.Location = new System.Drawing.Point(3, 16);
            this.testList.Name = "testList";
            this.testList.Size = new System.Drawing.Size(153, 364);
            this.testList.TabIndex = 0;
            this.testList.SelectedIndexChanged += new System.EventHandler(this.testList_SelectedIndexChanged);
            this.testList.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.testList_ItemCheck);
            // 
            // outputGroup
            // 
            this.outputGroup.Controls.Add(this.useButton);
            this.outputGroup.Controls.Add(this.actualGroup);
            this.outputGroup.Controls.Add(this.expectedGroup);
            this.outputGroup.Location = new System.Drawing.Point(174, 12);
            this.outputGroup.Name = "outputGroup";
            this.outputGroup.Size = new System.Drawing.Size(497, 387);
            this.outputGroup.TabIndex = 2;
            this.outputGroup.TabStop = false;
            this.outputGroup.Text = "Output";
            // 
            // useButton
            // 
            this.useButton.Location = new System.Drawing.Point(252, 360);
            this.useButton.Name = "useButton";
            this.useButton.Size = new System.Drawing.Size(75, 23);
            this.useButton.TabIndex = 2;
            this.useButton.Text = "Use";
            this.useButton.UseVisualStyleBackColor = true;
            this.useButton.Click += new System.EventHandler(this.useButton_Click);
            // 
            // actualGroup
            // 
            this.actualGroup.Controls.Add(this.actualText);
            this.actualGroup.Location = new System.Drawing.Point(249, 19);
            this.actualGroup.Name = "actualGroup";
            this.actualGroup.Size = new System.Drawing.Size(237, 333);
            this.actualGroup.TabIndex = 1;
            this.actualGroup.TabStop = false;
            this.actualGroup.Text = "Actual";
            // 
            // actualText
            // 
            this.actualText.Dock = System.Windows.Forms.DockStyle.Fill;
            this.actualText.Location = new System.Drawing.Point(3, 16);
            this.actualText.Name = "actualText";
            this.actualText.Size = new System.Drawing.Size(231, 314);
            this.actualText.TabIndex = 0;
            this.actualText.Text = "";
            this.actualText.WordWrap = false;
            // 
            // expectedGroup
            // 
            this.expectedGroup.Controls.Add(this.expectedText);
            this.expectedGroup.Location = new System.Drawing.Point(6, 19);
            this.expectedGroup.Name = "expectedGroup";
            this.expectedGroup.Size = new System.Drawing.Size(237, 333);
            this.expectedGroup.TabIndex = 0;
            this.expectedGroup.TabStop = false;
            this.expectedGroup.Text = "Expected";
            // 
            // expectedText
            // 
            this.expectedText.Dock = System.Windows.Forms.DockStyle.Fill;
            this.expectedText.Location = new System.Drawing.Point(3, 16);
            this.expectedText.Name = "expectedText";
            this.expectedText.Size = new System.Drawing.Size(231, 314);
            this.expectedText.TabIndex = 0;
            this.expectedText.Text = "";
            this.expectedText.WordWrap = false;
            // 
            // runButton
            // 
            this.runButton.Location = new System.Drawing.Point(12, 405);
            this.runButton.Name = "runButton";
            this.runButton.Size = new System.Drawing.Size(75, 23);
            this.runButton.TabIndex = 1;
            this.runButton.Text = "Run";
            this.runButton.UseVisualStyleBackColor = true;
            this.runButton.Click += new System.EventHandler(this.runButton_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(901, 595);
            this.Controls.Add(this.outputGroup);
            this.Controls.Add(this.runButton);
            this.Controls.Add(this.testsGroup);
            this.Name = "MainForm";
            this.Text = "MainForm";
            this.Resize += new System.EventHandler(this.MainForm_Resize);
            this.testsGroup.ResumeLayout(false);
            this.outputGroup.ResumeLayout(false);
            this.actualGroup.ResumeLayout(false);
            this.expectedGroup.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.CheckedListBox testList;
        private System.Windows.Forms.Button runButton;
        private System.Windows.Forms.Button useButton;
        private System.Windows.Forms.RichTextBox actualText;
        private System.Windows.Forms.RichTextBox expectedText;
        private System.Windows.Forms.GroupBox outputGroup;
        private System.Windows.Forms.GroupBox actualGroup;
        private System.Windows.Forms.GroupBox expectedGroup;
        private System.Windows.Forms.GroupBox testsGroup;
    }
}