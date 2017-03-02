namespace Fences
{
    partial class DialogBox
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DialogBox));
            this.openCreateRadioBox = new System.Windows.Forms.GroupBox();
            this.chooseFileButton = new System.Windows.Forms.Button();
            this.openFileRadioButton = new System.Windows.Forms.RadioButton();
            this.createFileRadioButton = new System.Windows.Forms.RadioButton();
            this.okButton = new System.Windows.Forms.Button();
            this.picture = new System.Windows.Forms.PictureBox();
            this.pilBox = new System.Windows.Forms.TextBox();
            this.btmBox = new System.Windows.Forms.TextBox();
            this.topBox = new System.Windows.Forms.TextBox();
            this.dwnPilBox = new System.Windows.Forms.TextBox();
            this.barBox = new System.Windows.Forms.TextBox();
            this.piLabel = new System.Windows.Forms.Label();
            this.btmLabel = new System.Windows.Forms.Label();
            this.topLabel = new System.Windows.Forms.Label();
            this.dwnPilLabel = new System.Windows.Forms.Label();
            this.barLabel = new System.Windows.Forms.Label();
            this.endBox = new System.Windows.Forms.TextBox();
            this.endLabel = new System.Windows.Forms.Label();
            this.changeMassButton = new System.Windows.Forms.Button();
            this.counter = new System.Windows.Forms.TextBox();
            this.openCreateRadioBox.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picture)).BeginInit();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.openCreateRadioBox.Controls.Add(this.chooseFileButton);
            this.openCreateRadioBox.Controls.Add(this.openFileRadioButton);
            this.openCreateRadioBox.Controls.Add(this.createFileRadioButton);
            this.openCreateRadioBox.Location = new System.Drawing.Point(12, 12);
            this.openCreateRadioBox.Name = "openCreateRadioBox";
            this.openCreateRadioBox.Size = new System.Drawing.Size(179, 68);
            this.openCreateRadioBox.TabIndex = 0;
            this.openCreateRadioBox.TabStop = false;
            this.openCreateRadioBox.Text = "Файл расчетов";
            // 
            // button2
            // 
            this.chooseFileButton.Location = new System.Drawing.Point(98, 36);
            this.chooseFileButton.Name = "chooseFileButton";
            this.chooseFileButton.Size = new System.Drawing.Size(75, 23);
            this.chooseFileButton.TabIndex = 15;
            this.chooseFileButton.Text = "ОК";
            this.chooseFileButton.UseVisualStyleBackColor = true;
            this.chooseFileButton.Click += new System.EventHandler(this.chooseFileButton_click);
            // 
            // radioButton2
            // 
            this.openFileRadioButton.AutoSize = true;
            this.openFileRadioButton.Location = new System.Drawing.Point(6, 42);
            this.openFileRadioButton.Name = "openFileRadioButton";
            this.openFileRadioButton.Size = new System.Drawing.Size(69, 17);
            this.openFileRadioButton.TabIndex = 1;
            this.openFileRadioButton.TabStop = true;
            this.openFileRadioButton.Text = "Открыть";
            this.openFileRadioButton.UseVisualStyleBackColor = true;
            // 
            // radioButton1
            // 
            this.createFileRadioButton.AutoSize = true;
            this.createFileRadioButton.Location = new System.Drawing.Point(6, 19);
            this.createFileRadioButton.Name = "createFileRadioButton";
            this.createFileRadioButton.Size = new System.Drawing.Size(67, 17);
            this.createFileRadioButton.TabIndex = 0;
            this.createFileRadioButton.TabStop = true;
            this.createFileRadioButton.Text = "Создать";
            this.createFileRadioButton.UseVisualStyleBackColor = true;
            // 
            // button1
            // 
            this.okButton.Location = new System.Drawing.Point(356, 242);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(75, 23);
            this.okButton.TabIndex = 1;
            this.okButton.Text = "ОК";
            this.okButton.UseVisualStyleBackColor = true;
            this.okButton.Click += new System.EventHandler(this.okButton_Click);
            // 
            // pictureBox1
            // 
            this.picture.Image = global::Fences.Properties.Resources.Scheme;
            this.picture.Location = new System.Drawing.Point(197, 17);
            this.picture.Name = "picture";
            this.picture.Size = new System.Drawing.Size(234, 193);
            this.picture.TabIndex = 2;
            this.picture.TabStop = false;
            // 
            // textBox1
            // 
            this.pilBox.Location = new System.Drawing.Point(91, 86);
            this.pilBox.Name = "pilBox";
            this.pilBox.Size = new System.Drawing.Size(100, 20);
            this.pilBox.TabIndex = 3;
            // 
            // textBox2
            // 
            this.btmBox.Location = new System.Drawing.Point(91, 112);
            this.btmBox.Name = "btmBox";
            this.btmBox.Size = new System.Drawing.Size(100, 20);
            this.btmBox.TabIndex = 4;
            // 
            // textBox3
            // 
            this.topBox.Location = new System.Drawing.Point(91, 138);
            this.topBox.Name = "topBox";
            this.topBox.Size = new System.Drawing.Size(100, 20);
            this.topBox.TabIndex = 5;
            // 
            // textBox4
            // 
            this.dwnPilBox.Location = new System.Drawing.Point(91, 164);
            this.dwnPilBox.Name = "dwnPilBox";
            this.dwnPilBox.Size = new System.Drawing.Size(100, 20);
            this.dwnPilBox.TabIndex = 6;
            // 
            // textBox5
            // 
            this.barBox.Location = new System.Drawing.Point(91, 190);
            this.barBox.Name = "barBox";
            this.barBox.Size = new System.Drawing.Size(100, 20);
            this.barBox.TabIndex = 7;
            // 
            // label1
            // 
            this.piLabel.AutoSize = true;
            this.piLabel.Location = new System.Drawing.Point(26, 86);
            this.piLabel.Name = "label1";
            this.piLabel.Size = new System.Drawing.Size(39, 13);
            this.piLabel.TabIndex = 8;
            this.piLabel.Text = "Поз. 1";
            // 
            // label2
            // 
            this.btmLabel.AutoSize = true;
            this.btmLabel.Location = new System.Drawing.Point(26, 112);
            this.btmLabel.Name = "btmLabel";
            this.btmLabel.Size = new System.Drawing.Size(39, 13);
            this.btmLabel.TabIndex = 9;
            this.btmLabel.Text = "Поз. 2";
            // 
            // label3
            // 
            this.topLabel.AutoSize = true;
            this.topLabel.Location = new System.Drawing.Point(26, 138);
            this.topLabel.Name = "topLabel";
            this.topLabel.Size = new System.Drawing.Size(39, 13);
            this.topLabel.TabIndex = 10;
            this.topLabel.Text = "Поз. 3";
            // 
            // label4
            // 
            this.dwnPilLabel.AutoSize = true;
            this.dwnPilLabel.Location = new System.Drawing.Point(26, 164);
            this.dwnPilLabel.Name = "dwnPilLabel";
            this.dwnPilLabel.Size = new System.Drawing.Size(39, 13);
            this.dwnPilLabel.TabIndex = 11;
            this.dwnPilLabel.Text = "Поз. 4";
            // 
            // label5
            // 
            this.barLabel.AutoSize = true;
            this.barLabel.Location = new System.Drawing.Point(26, 190);
            this.barLabel.Name = "barLabel";
            this.barLabel.Size = new System.Drawing.Size(39, 13);
            this.barLabel.TabIndex = 12;
            this.barLabel.Text = "Поз. 5";
            // 
            // textBox6
            // 
            this.endBox.Location = new System.Drawing.Point(91, 216);
            this.endBox.Name = "endBox";
            this.endBox.Size = new System.Drawing.Size(100, 20);
            this.endBox.TabIndex = 13;
            // 
            // label6
            // 
            this.endLabel.AutoSize = true;
            this.endLabel.Location = new System.Drawing.Point(26, 216);
            this.endLabel.Name = "endLabel";
            this.endLabel.Size = new System.Drawing.Size(56, 13);
            this.endLabel.TabIndex = 14;
            this.endLabel.Text = "Заглушки";
            // 
            // button3
            // 
            this.changeMassButton.Location = new System.Drawing.Point(101, 242);
            this.changeMassButton.Name = "changeMassButton";
            this.changeMassButton.Size = new System.Drawing.Size(75, 23);
            this.changeMassButton.TabIndex = 16;
            this.changeMassButton.Text = "Сохранить";
            this.changeMassButton.UseVisualStyleBackColor = true;
            this.changeMassButton.Click += new System.EventHandler(this.changeMassButton_Click);
            // 
            // textBox7
            // 
            this.counter.Location = new System.Drawing.Point(197, 216);
            this.counter.Name = "counter";
            this.counter.ReadOnly = true;
            this.counter.Size = new System.Drawing.Size(234, 20);
            this.counter.TabIndex = 17;
            // 
            // DialogBox
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(447, 280);
            this.Controls.Add(this.counter);
            this.Controls.Add(this.changeMassButton);
            this.Controls.Add(this.endLabel);
            this.Controls.Add(this.endBox);
            this.Controls.Add(this.barLabel);
            this.Controls.Add(this.dwnPilLabel);
            this.Controls.Add(this.topLabel);
            this.Controls.Add(this.btmLabel);
            this.Controls.Add(this.piLabel);
            this.Controls.Add(this.barBox);
            this.Controls.Add(this.dwnPilBox);
            this.Controls.Add(this.topBox);
            this.Controls.Add(this.btmBox);
            this.Controls.Add(this.pilBox);
            this.Controls.Add(this.picture);
            this.Controls.Add(this.okButton);
            this.Controls.Add(this.openCreateRadioBox);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "DialogBox";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "SimpleFences";
            this.Load += new System.EventHandler(this.DialogBox_Load);
            this.openCreateRadioBox.ResumeLayout(false);
            this.openCreateRadioBox.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picture)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox openCreateRadioBox;
        private System.Windows.Forms.RadioButton openFileRadioButton;
        private System.Windows.Forms.RadioButton createFileRadioButton;
        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.PictureBox picture;
        private System.Windows.Forms.TextBox pilBox;
        private System.Windows.Forms.TextBox btmBox;
        private System.Windows.Forms.TextBox topBox;
        private System.Windows.Forms.TextBox dwnPilBox;
        private System.Windows.Forms.TextBox barBox;
        private System.Windows.Forms.Label piLabel;
        private System.Windows.Forms.Label btmLabel;
        private System.Windows.Forms.Label topLabel;
        private System.Windows.Forms.Label dwnPilLabel;
        private System.Windows.Forms.Label barLabel;
        private System.Windows.Forms.TextBox endBox;
        private System.Windows.Forms.Label endLabel;
        private System.Windows.Forms.Button chooseFileButton;
        private System.Windows.Forms.Button changeMassButton;
        private System.Windows.Forms.TextBox counter;
    }
}