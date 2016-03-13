namespace Minecraft_Server_Control_Panel
{
    partial class ListWindow
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
            this.UserList = new System.Windows.Forms.ListBox();
            this.GroupUserAdd = new System.Windows.Forms.GroupBox();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.button4 = new System.Windows.Forms.Button();
            this.GroupUserAdd.SuspendLayout();
            this.SuspendLayout();
            // 
            // UserList
            // 
            this.UserList.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.UserList.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.UserList.Font = new System.Drawing.Font("Yu Gothic UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.UserList.FormattingEnabled = true;
            this.UserList.ItemHeight = 32;
            this.UserList.Location = new System.Drawing.Point(12, 12);
            this.UserList.Name = "UserList";
            this.UserList.Size = new System.Drawing.Size(189, 322);
            this.UserList.TabIndex = 0;
            // 
            // GroupUserAdd
            // 
            this.GroupUserAdd.Controls.Add(this.button1);
            this.GroupUserAdd.Controls.Add(this.label1);
            this.GroupUserAdd.Controls.Add(this.textBox1);
            this.GroupUserAdd.Font = new System.Drawing.Font("Yu Gothic UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.GroupUserAdd.Location = new System.Drawing.Point(216, 24);
            this.GroupUserAdd.Name = "GroupUserAdd";
            this.GroupUserAdd.Size = new System.Drawing.Size(244, 103);
            this.GroupUserAdd.TabIndex = 1;
            this.GroupUserAdd.TabStop = false;
            this.GroupUserAdd.Text = "ユーザーの追加";
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(81, 33);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(142, 25);
            this.textBox1.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(16, 36);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(62, 17);
            this.label1.TabIndex = 1;
            this.label1.Text = "ユーザー名:";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(163, 64);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 27);
            this.button1.TabIndex = 2;
            this.button1.Text = "追加";
            this.button1.UseVisualStyleBackColor = true;
            // 
            // button2
            // 
            this.button2.Font = new System.Drawing.Font("Yu Gothic UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.button2.Location = new System.Drawing.Point(216, 187);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(244, 34);
            this.button2.TabIndex = 2;
            this.button2.Text = "選択したユーザーをリストから削除";
            this.button2.UseVisualStyleBackColor = true;
            // 
            // button3
            // 
            this.button3.Font = new System.Drawing.Font("Yu Gothic UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.button3.Location = new System.Drawing.Point(216, 307);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(157, 27);
            this.button3.TabIndex = 3;
            this.button3.Text = "完了";
            this.button3.UseVisualStyleBackColor = true;
            // 
            // button4
            // 
            this.button4.Font = new System.Drawing.Font("Yu Gothic UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.button4.Location = new System.Drawing.Point(379, 306);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(81, 27);
            this.button4.TabIndex = 4;
            this.button4.Text = "キャンセル";
            this.button4.UseVisualStyleBackColor = true;
            // 
            // ListWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(472, 345);
            this.Controls.Add(this.button4);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.GroupUserAdd);
            this.Controls.Add(this.UserList);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ListWindow";
            this.ShowIcon = false;
            this.Text = "読み込み中";
            this.GroupUserAdd.ResumeLayout(false);
            this.GroupUserAdd.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListBox UserList;
        private System.Windows.Forms.GroupBox GroupUserAdd;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.Button button4;
    }
}