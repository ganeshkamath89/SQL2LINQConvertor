namespace SQL2LINQ {
    partial class SQL2LINQ {
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
            this.txtSQL = new System.Windows.Forms.TextBox();
            this.txtLINQ = new System.Windows.Forms.TextBox();
            this.btnSql2Linq = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.helpProvider1 = new System.Windows.Forms.HelpProvider();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.txtUsedObjectName = new System.Windows.Forms.TextBox();
            this.lblUsedObjectName = new System.Windows.Forms.Label();
            this.lblSearchFormName = new System.Windows.Forms.Label();
            this.txtSearchFormName = new System.Windows.Forms.TextBox();
            this.btnUpdate = new System.Windows.Forms.Button();
            this.btnConfigure = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // txtSQL
            // 
            this.txtSQL.Location = new System.Drawing.Point(16, 77);
            this.txtSQL.Multiline = true;
            this.txtSQL.Name = "txtSQL";
            this.txtSQL.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtSQL.Size = new System.Drawing.Size(388, 401);
            this.txtSQL.TabIndex = 0;
            this.txtSQL.Text = "Select CountryName From Country";
            // 
            // txtLINQ
            // 
            this.txtLINQ.Location = new System.Drawing.Point(463, 77);
            this.txtLINQ.Multiline = true;
            this.txtLINQ.Name = "txtLINQ";
            this.txtLINQ.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtLINQ.Size = new System.Drawing.Size(388, 401);
            this.txtLINQ.TabIndex = 1;
            // 
            // btnSql2Linq
            // 
            this.btnSql2Linq.Location = new System.Drawing.Point(411, 152);
            this.btnSql2Linq.Name = "btnSql2Linq";
            this.btnSql2Linq.Size = new System.Drawing.Size(46, 43);
            this.btnSql2Linq.TabIndex = 2;
            this.btnSql2Linq.Text = "=>";
            this.btnSql2Linq.UseVisualStyleBackColor = true;
            this.btnSql2Linq.Click += new System.EventHandler(this.btnSql2Linq_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(17, 56);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(58, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "SQL String";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(460, 56);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(62, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "LINQ String";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.btnConfigure);
            this.groupBox1.Controls.Add(this.btnUpdate);
            this.groupBox1.Controls.Add(this.txtUsedObjectName);
            this.groupBox1.Controls.Add(this.lblUsedObjectName);
            this.groupBox1.Controls.Add(this.lblSearchFormName);
            this.groupBox1.Controls.Add(this.txtSearchFormName);
            this.groupBox1.Location = new System.Drawing.Point(14, 4);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(836, 49);
            this.groupBox1.TabIndex = 9;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Users Preferences";
            // 
            // txtUsedObjectName
            // 
            this.txtUsedObjectName.Location = new System.Drawing.Point(422, 18);
            this.txtUsedObjectName.Name = "txtUsedObjectName";
            this.txtUsedObjectName.Size = new System.Drawing.Size(245, 20);
            this.txtUsedObjectName.TabIndex = 12;
            this.txtUsedObjectName.Text = "_appdb";
            // 
            // lblUsedObjectName
            // 
            this.lblUsedObjectName.AutoSize = true;
            this.lblUsedObjectName.Location = new System.Drawing.Point(319, 23);
            this.lblUsedObjectName.Name = "lblUsedObjectName";
            this.lblUsedObjectName.Size = new System.Drawing.Size(97, 13);
            this.lblUsedObjectName.TabIndex = 11;
            this.lblUsedObjectName.Text = "Used Object Name";
            // 
            // lblSearchFormName
            // 
            this.lblSearchFormName.AutoSize = true;
            this.lblSearchFormName.Location = new System.Drawing.Point(16, 22);
            this.lblSearchFormName.Name = "lblSearchFormName";
            this.lblSearchFormName.Size = new System.Drawing.Size(98, 13);
            this.lblSearchFormName.TabIndex = 10;
            this.lblSearchFormName.Text = "Search Form Name";
            // 
            // txtSearchFormName
            // 
            this.txtSearchFormName.Location = new System.Drawing.Point(120, 18);
            this.txtSearchFormName.Name = "txtSearchFormName";
            this.txtSearchFormName.Size = new System.Drawing.Size(176, 20);
            this.txtSearchFormName.TabIndex = 9;
            this.txtSearchFormName.Text = "SearchCountryMaster";
            // 
            // btnUpdate
            // 
            this.btnUpdate.Location = new System.Drawing.Point(673, 16);
            this.btnUpdate.Name = "btnUpdate";
            this.btnUpdate.Size = new System.Drawing.Size(60, 23);
            this.btnUpdate.TabIndex = 13;
            this.btnUpdate.Text = "Update";
            this.btnUpdate.UseVisualStyleBackColor = true;
            this.btnUpdate.Click += new System.EventHandler(this.btnUpdate_Click);
            // 
            // btnConfigure
            // 
            this.btnConfigure.Location = new System.Drawing.Point(755, 15);
            this.btnConfigure.Name = "btnConfigure";
            this.btnConfigure.Size = new System.Drawing.Size(63, 23);
            this.btnConfigure.TabIndex = 14;
            this.btnConfigure.Text = "Configure";
            this.btnConfigure.UseVisualStyleBackColor = true;
            this.btnConfigure.Click += new System.EventHandler(this.btnConfigure_Click);
            // 
            // SQL2LINQ
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(864, 505);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnSql2Linq);
            this.Controls.Add(this.txtLINQ);
            this.Controls.Add(this.txtSQL);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Name = "SQL2LINQ";
            this.ShowIcon = false;
            this.Text = "SQL2LINQ";
            this.Shown += new System.EventHandler(this.SQL2LINQ_Shown);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtSQL;
        private System.Windows.Forms.TextBox txtLINQ;
        private System.Windows.Forms.Button btnSql2Linq;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.HelpProvider helpProvider1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button btnUpdate;
        private System.Windows.Forms.TextBox txtUsedObjectName;
        private System.Windows.Forms.Label lblUsedObjectName;
        private System.Windows.Forms.Label lblSearchFormName;
        private System.Windows.Forms.TextBox txtSearchFormName;
        private System.Windows.Forms.Button btnConfigure;
    }
}

