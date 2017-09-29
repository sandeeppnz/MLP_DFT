namespace MLP_DFT
{
    partial class frmMain
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
            this.cmdRun = new System.Windows.Forms.Button();
            this.cmdSelectFile = new System.Windows.Forms.Button();
            this.txtNumInputNodes = new System.Windows.Forms.TextBox();
            this.txtNumHiddenNodes = new System.Windows.Forms.TextBox();
            this.txtNumOutputNodes = new System.Windows.Forms.TextBox();
            this.txtNumDataRows = new System.Windows.Forms.TextBox();
            this.txtMaxEpochs = new System.Windows.Forms.TextBox();
            this.txtLearnRate = new System.Windows.Forms.TextBox();
            this.txtMomentum = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // cmdRun
            // 
            this.cmdRun.Location = new System.Drawing.Point(419, 139);
            this.cmdRun.Name = "cmdRun";
            this.cmdRun.Size = new System.Drawing.Size(94, 40);
            this.cmdRun.TabIndex = 0;
            this.cmdRun.Text = "Run";
            this.cmdRun.UseVisualStyleBackColor = true;
            // 
            // cmdSelectFile
            // 
            this.cmdSelectFile.Location = new System.Drawing.Point(315, 139);
            this.cmdSelectFile.Name = "cmdSelectFile";
            this.cmdSelectFile.Size = new System.Drawing.Size(94, 40);
            this.cmdSelectFile.TabIndex = 1;
            this.cmdSelectFile.Text = "Open";
            this.cmdSelectFile.UseVisualStyleBackColor = true;
            // 
            // txtNumInputNodes
            // 
            this.txtNumInputNodes.Location = new System.Drawing.Point(146, 16);
            this.txtNumInputNodes.Name = "txtNumInputNodes";
            this.txtNumInputNodes.Size = new System.Drawing.Size(100, 26);
            this.txtNumInputNodes.TabIndex = 2;
            // 
            // txtNumHiddenNodes
            // 
            this.txtNumHiddenNodes.Location = new System.Drawing.Point(146, 48);
            this.txtNumHiddenNodes.Name = "txtNumHiddenNodes";
            this.txtNumHiddenNodes.Size = new System.Drawing.Size(100, 26);
            this.txtNumHiddenNodes.TabIndex = 3;
            // 
            // txtNumOutputNodes
            // 
            this.txtNumOutputNodes.Location = new System.Drawing.Point(146, 83);
            this.txtNumOutputNodes.Name = "txtNumOutputNodes";
            this.txtNumOutputNodes.Size = new System.Drawing.Size(100, 26);
            this.txtNumOutputNodes.TabIndex = 4;
            // 
            // txtNumDataRows
            // 
            this.txtNumDataRows.Location = new System.Drawing.Point(146, 149);
            this.txtNumDataRows.Name = "txtNumDataRows";
            this.txtNumDataRows.Size = new System.Drawing.Size(100, 26);
            this.txtNumDataRows.TabIndex = 5;
            // 
            // txtMaxEpochs
            // 
            this.txtMaxEpochs.Location = new System.Drawing.Point(413, 13);
            this.txtMaxEpochs.Name = "txtMaxEpochs";
            this.txtMaxEpochs.Size = new System.Drawing.Size(100, 26);
            this.txtMaxEpochs.TabIndex = 6;
            // 
            // txtLearnRate
            // 
            this.txtLearnRate.Location = new System.Drawing.Point(413, 51);
            this.txtLearnRate.Name = "txtLearnRate";
            this.txtLearnRate.Size = new System.Drawing.Size(100, 26);
            this.txtLearnRate.TabIndex = 7;
            // 
            // txtMomentum
            // 
            this.txtMomentum.Location = new System.Drawing.Point(413, 83);
            this.txtMomentum.Name = "txtMomentum";
            this.txtMomentum.Size = new System.Drawing.Size(100, 26);
            this.txtMomentum.TabIndex = 8;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(311, 19);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(96, 20);
            this.label1.TabIndex = 9;
            this.label1.Text = "Max Epochs";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(311, 54);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(89, 20);
            this.label2.TabIndex = 10;
            this.label2.Text = "Learn Rate";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(311, 89);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(89, 20);
            this.label3.TabIndex = 11;
            this.label3.Text = "Momentum";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(10, 19);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(109, 20);
            this.label4.TabIndex = 12;
            this.label4.Text = "# Input Nodes";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(10, 54);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(123, 20);
            this.label5.TabIndex = 13;
            this.label5.Text = "# Hidden Nodes";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(10, 89);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(121, 20);
            this.label6.TabIndex = 14;
            this.label6.Text = "# Output Nodes";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(12, 149);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(105, 20);
            this.label7.TabIndex = 15;
            this.label7.Text = "No. Datarows";
            // 
            // checkBox1
            // 
            this.checkBox1.AutoSize = true;
            this.checkBox1.Location = new System.Drawing.Point(16, 200);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(197, 24);
            this.checkBox1.TabIndex = 16;
            this.checkBox1.Text = "MLP Feature Selection";
            this.checkBox1.UseVisualStyleBackColor = true;
            // 
            // frmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(563, 393);
            this.Controls.Add(this.checkBox1);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtMomentum);
            this.Controls.Add(this.txtLearnRate);
            this.Controls.Add(this.txtMaxEpochs);
            this.Controls.Add(this.txtNumDataRows);
            this.Controls.Add(this.txtNumOutputNodes);
            this.Controls.Add(this.txtNumHiddenNodes);
            this.Controls.Add(this.txtNumInputNodes);
            this.Controls.Add(this.cmdSelectFile);
            this.Controls.Add(this.cmdRun);
            this.Name = "frmMain";
            this.Text = "MLP_DFT_Application";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button cmdRun;
        private System.Windows.Forms.Button cmdSelectFile;
        private System.Windows.Forms.TextBox txtNumInputNodes;
        private System.Windows.Forms.TextBox txtNumHiddenNodes;
        private System.Windows.Forms.TextBox txtNumOutputNodes;
        private System.Windows.Forms.TextBox txtNumDataRows;
        private System.Windows.Forms.TextBox txtMaxEpochs;
        private System.Windows.Forms.TextBox txtLearnRate;
        private System.Windows.Forms.TextBox txtMomentum;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.CheckBox checkBox1;
    }
}

