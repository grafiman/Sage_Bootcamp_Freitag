namespace WinFormsSDataClient
{
    partial class FormSDataClient
    {
        /// <summary>
        /// Erforderliche Designervariable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Verwendete Ressourcen bereinigen.
        /// </summary>
        /// <param name="disposing">True, wenn verwaltete Ressourcen gelöscht werden sollen; andernfalls False.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Vom Windows Form-Designer generierter Code

        /// <summary>
        /// Erforderliche Methode für die Designerunterstützung.
        /// Der Inhalt der Methode darf nicht mit dem Code-Editor geändert werden.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormSDataClient));
            this.buttonLoad = new System.Windows.Forms.Button();
            this.buttonCreate = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.textBoxSeminar = new System.Windows.Forms.TextBox();
            this.textBoxKonto = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.textVorname = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.textBoxNachname = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.textBoxEmail = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.textBoxBuchungsID = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.textBoxReferenz = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // buttonLoad
            // 
            this.buttonLoad.Location = new System.Drawing.Point(481, 12);
            this.buttonLoad.Name = "buttonLoad";
            this.buttonLoad.Size = new System.Drawing.Size(140, 38);
            this.buttonLoad.TabIndex = 0;
            this.buttonLoad.Text = "Load Buchung";
            this.buttonLoad.UseVisualStyleBackColor = true;
            this.buttonLoad.Click += new System.EventHandler(this.buttonLoad_Click);
            // 
            // buttonCreate
            // 
            this.buttonCreate.Location = new System.Drawing.Point(481, 54);
            this.buttonCreate.Name = "buttonCreate";
            this.buttonCreate.Size = new System.Drawing.Size(140, 38);
            this.buttonCreate.TabIndex = 1;
            this.buttonCreate.Text = "Create Buchung";
            this.buttonCreate.UseVisualStyleBackColor = true;
            this.buttonCreate.Click += new System.EventHandler(this.buttonCreate_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(9, 15);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(45, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Seminar";
            // 
            // textBoxSeminar
            // 
            this.textBoxSeminar.Location = new System.Drawing.Point(121, 12);
            this.textBoxSeminar.Name = "textBoxSeminar";
            this.textBoxSeminar.Size = new System.Drawing.Size(337, 20);
            this.textBoxSeminar.TabIndex = 3;
            this.textBoxSeminar.Text = "S100001";
            // 
            // textBoxKonto
            // 
            this.textBoxKonto.Location = new System.Drawing.Point(121, 38);
            this.textBoxKonto.Name = "textBoxKonto";
            this.textBoxKonto.Size = new System.Drawing.Size(337, 20);
            this.textBoxKonto.TabIndex = 5;
            this.textBoxKonto.Text = "D100000";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(9, 41);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(38, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "Kunde";
            // 
            // textVorname
            // 
            this.textVorname.Location = new System.Drawing.Point(121, 64);
            this.textVorname.Name = "textVorname";
            this.textVorname.Size = new System.Drawing.Size(337, 20);
            this.textVorname.TabIndex = 10;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 67);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(49, 13);
            this.label3.TabIndex = 11;
            this.label3.Text = "Vorname";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(12, 93);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(59, 13);
            this.label4.TabIndex = 13;
            this.label4.Text = "Nachname";
            // 
            // textBoxNachname
            // 
            this.textBoxNachname.Location = new System.Drawing.Point(121, 90);
            this.textBoxNachname.Name = "textBoxNachname";
            this.textBoxNachname.Size = new System.Drawing.Size(337, 20);
            this.textBoxNachname.TabIndex = 12;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(12, 119);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(32, 13);
            this.label5.TabIndex = 15;
            this.label5.Text = "Email";
            // 
            // textBoxEmail
            // 
            this.textBoxEmail.Location = new System.Drawing.Point(121, 116);
            this.textBoxEmail.Name = "textBoxEmail";
            this.textBoxEmail.Size = new System.Drawing.Size(337, 20);
            this.textBoxEmail.TabIndex = 14;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(12, 145);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(66, 13);
            this.label6.TabIndex = 17;
            this.label6.Text = "BuchungsID";
            // 
            // textBoxBuchungsID
            // 
            this.textBoxBuchungsID.Location = new System.Drawing.Point(121, 142);
            this.textBoxBuchungsID.Name = "textBoxBuchungsID";
            this.textBoxBuchungsID.Size = new System.Drawing.Size(337, 20);
            this.textBoxBuchungsID.TabIndex = 16;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(12, 171);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(93, 13);
            this.label7.TabIndex = 19;
            this.label7.Text = "Buchungsreferenz";
            // 
            // textBoxReferenz
            // 
            this.textBoxReferenz.Location = new System.Drawing.Point(121, 168);
            this.textBoxReferenz.Name = "textBoxReferenz";
            this.textBoxReferenz.Size = new System.Drawing.Size(337, 20);
            this.textBoxReferenz.TabIndex = 18;
            // 
            // FormSDataClient
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(649, 348);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.textBoxReferenz);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.textBoxBuchungsID);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.textBoxEmail);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.textBoxNachname);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.textVorname);
            this.Controls.Add(this.textBoxKonto);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.textBoxSeminar);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.buttonCreate);
            this.Controls.Add(this.buttonLoad);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "FormSDataClient";
            this.Text = "Seminarbuchung SData Client";
            this.Load += new System.EventHandler(this.FormSDataClient_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button buttonLoad;
        private System.Windows.Forms.Button buttonCreate;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBoxSeminar;
        private System.Windows.Forms.TextBox textBoxKonto;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox textVorname;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox textBoxNachname;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox textBoxEmail;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox textBoxBuchungsID;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox textBoxReferenz;
    }
}

