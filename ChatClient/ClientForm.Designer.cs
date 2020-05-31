namespace ChatClient
{
    partial class ClientForm
    {
        /// <summary>
        /// Обязательная переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором форм Windows

        /// <summary>
        /// Требуемый метод для поддержки конструктора — не изменяйте 
        /// содержимое этого метода с помощью редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            this.lblPassword = new System.Windows.Forms.Label();
            this.tbPassword = new System.Windows.Forms.TextBox();
            this.lblName = new System.Windows.Forms.Label();
            this.lbCurrentDialog = new System.Windows.Forms.Label();
            this.tbChatContent = new System.Windows.Forms.RichTextBox();
            this.lbParticipants = new System.Windows.Forms.ListBox();
            this.label3 = new System.Windows.Forms.Label();
            this.tbName = new System.Windows.Forms.TextBox();
            this.btDisconnect = new System.Windows.Forms.Button();
            this.cbIsConnected = new System.Windows.Forms.CheckBox();
            this.btSendMessage = new System.Windows.Forms.Button();
            this.btConnect = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.tbIPAdress = new System.Windows.Forms.TextBox();
            this.tbMessageContent = new System.Windows.Forms.RichTextBox();
            this.SuspendLayout();
            // 
            // lblPassword
            // 
            this.lblPassword.AutoSize = true;
            this.lblPassword.Location = new System.Drawing.Point(31, 76);
            this.lblPassword.Name = "lblPassword";
            this.lblPassword.Size = new System.Drawing.Size(68, 17);
            this.lblPassword.TabIndex = 66;
            this.lblPassword.Text = "password";
            // 
            // tbPassword
            // 
            this.tbPassword.Location = new System.Drawing.Point(105, 76);
            this.tbPassword.Name = "tbPassword";
            this.tbPassword.Size = new System.Drawing.Size(122, 22);
            this.tbPassword.TabIndex = 65;
            // 
            // lblName
            // 
            this.lblName.AutoSize = true;
            this.lblName.Location = new System.Drawing.Point(31, 48);
            this.lblName.Name = "lblName";
            this.lblName.Size = new System.Drawing.Size(43, 17);
            this.lblName.TabIndex = 64;
            this.lblName.Text = "name";
            // 
            // lbCurrentDialog
            // 
            this.lbCurrentDialog.AutoSize = true;
            this.lbCurrentDialog.Location = new System.Drawing.Point(231, 150);
            this.lbCurrentDialog.Name = "lbCurrentDialog";
            this.lbCurrentDialog.Size = new System.Drawing.Size(100, 17);
            this.lbCurrentDialog.TabIndex = 63;
            this.lbCurrentDialog.Text = "Not connected";
            // 
            // tbChatContent
            // 
            this.tbChatContent.Location = new System.Drawing.Point(33, 179);
            this.tbChatContent.Name = "tbChatContent";
            this.tbChatContent.ReadOnly = true;
            this.tbChatContent.Size = new System.Drawing.Size(491, 174);
            this.tbChatContent.TabIndex = 62;
            this.tbChatContent.Text = "";
            // 
            // lbParticipants
            // 
            this.lbParticipants.FormattingEnabled = true;
            this.lbParticipants.ItemHeight = 16;
            this.lbParticipants.Location = new System.Drawing.Point(540, 179);
            this.lbParticipants.Name = "lbParticipants";
            this.lbParticipants.Size = new System.Drawing.Size(229, 228);
            this.lbParticipants.TabIndex = 61;
            this.lbParticipants.SelectedIndexChanged += new System.EventHandler(this.lbParticipants_SelectedIndexChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(616, 150);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(82, 17);
            this.label3.TabIndex = 60;
            this.label3.Text = "Participants";
            // 
            // tbName
            // 
            this.tbName.Location = new System.Drawing.Point(105, 48);
            this.tbName.Name = "tbName";
            this.tbName.Size = new System.Drawing.Size(122, 22);
            this.tbName.TabIndex = 59;
            // 
            // btDisconnect
            // 
            this.btDisconnect.Enabled = false;
            this.btDisconnect.Location = new System.Drawing.Point(353, 44);
            this.btDisconnect.Name = "btDisconnect";
            this.btDisconnect.Size = new System.Drawing.Size(88, 30);
            this.btDisconnect.TabIndex = 58;
            this.btDisconnect.Text = "Disconnect";
            this.btDisconnect.UseVisualStyleBackColor = true;
            this.btDisconnect.Click += new System.EventHandler(this.btDisconnect_Click);
            // 
            // cbIsConnected
            // 
            this.cbIsConnected.AutoSize = true;
            this.cbIsConnected.Location = new System.Drawing.Point(49, 110);
            this.cbIsConnected.Name = "cbIsConnected";
            this.cbIsConnected.Size = new System.Drawing.Size(110, 21);
            this.cbIsConnected.TabIndex = 57;
            this.cbIsConnected.Text = "Is connected";
            this.cbIsConnected.UseVisualStyleBackColor = true;
            // 
            // btSendMessage
            // 
            this.btSendMessage.Location = new System.Drawing.Point(465, 349);
            this.btSendMessage.Name = "btSendMessage";
            this.btSendMessage.Size = new System.Drawing.Size(59, 55);
            this.btSendMessage.TabIndex = 56;
            this.btSendMessage.Text = "Send";
            this.btSendMessage.UseVisualStyleBackColor = true;
            this.btSendMessage.Click += new System.EventHandler(this.btSendMessage_Click);
            // 
            // btConnect
            // 
            this.btConnect.Location = new System.Drawing.Point(243, 44);
            this.btConnect.Name = "btConnect";
            this.btConnect.Size = new System.Drawing.Size(88, 30);
            this.btConnect.TabIndex = 55;
            this.btConnect.Text = "Connect";
            this.btConnect.UseVisualStyleBackColor = true;
            this.btConnect.Click += new System.EventHandler(this.btConnect_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(485, 48);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(66, 17);
            this.label1.TabIndex = 54;
            this.label1.Text = "Server IP";
            // 
            // tbIPAdress
            // 
            this.tbIPAdress.Location = new System.Drawing.Point(557, 48);
            this.tbIPAdress.Name = "tbIPAdress";
            this.tbIPAdress.ReadOnly = true;
            this.tbIPAdress.Size = new System.Drawing.Size(122, 22);
            this.tbIPAdress.TabIndex = 53;
            // 
            // tbMessageContent
            // 
            this.tbMessageContent.Location = new System.Drawing.Point(33, 349);
            this.tbMessageContent.Name = "tbMessageContent";
            this.tbMessageContent.Size = new System.Drawing.Size(435, 55);
            this.tbMessageContent.TabIndex = 52;
            this.tbMessageContent.Text = "";
            // 
            // ClientForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.lblPassword);
            this.Controls.Add(this.tbPassword);
            this.Controls.Add(this.lblName);
            this.Controls.Add(this.lbCurrentDialog);
            this.Controls.Add(this.tbChatContent);
            this.Controls.Add(this.lbParticipants);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.tbName);
            this.Controls.Add(this.btDisconnect);
            this.Controls.Add(this.cbIsConnected);
            this.Controls.Add(this.btSendMessage);
            this.Controls.Add(this.btConnect);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.tbIPAdress);
            this.Controls.Add(this.tbMessageContent);
            this.Name = "ClientForm";
            this.Text = "LAN chat";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ClientForm_FormClosing);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblPassword;
        private System.Windows.Forms.TextBox tbPassword;
        private System.Windows.Forms.Label lblName;
        private System.Windows.Forms.Label lbCurrentDialog;
        private System.Windows.Forms.RichTextBox tbChatContent;
        private System.Windows.Forms.ListBox lbParticipants;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox tbName;
        private System.Windows.Forms.Button btDisconnect;
        private System.Windows.Forms.CheckBox cbIsConnected;
        private System.Windows.Forms.Button btSendMessage;
        private System.Windows.Forms.Button btConnect;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox tbIPAdress;
        private System.Windows.Forms.RichTextBox tbMessageContent;
    }
}

