﻿namespace Task
{
    partial class Form1
    {
        /// <summary>
        /// Требуется переменная конструктора.
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
        /// Обязательный метод для поддержки конструктора - не изменяйте
        /// содержимое данного метода при помощи редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.label1 = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.button2 = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.textBox5 = new System.Windows.Forms.TextBox();
            this.button4 = new System.Windows.Forms.Button();
            this.button5 = new System.Windows.Forms.Button();
            this.button6 = new System.Windows.Forms.Button();
            this.textBox6 = new System.Windows.Forms.TextBox();
            this.textBox7 = new System.Windows.Forms.TextBox();
            this.textBox8 = new System.Windows.Forms.TextBox();
            this.textBoxX2 = new System.Windows.Forms.TextBox();
            this.comboBox2 = new System.Windows.Forms.ComboBox();
            this.textBoxZ1 = new System.Windows.Forms.TextBox();
            this.textBoxY2 = new System.Windows.Forms.TextBox();
            this.textBoxY1 = new System.Windows.Forms.TextBox();
            this.textBoxX1 = new System.Windows.Forms.TextBox();
            this.textBoxZ2 = new System.Windows.Forms.TextBox();
            this.textBoxAngle = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.comboBox3 = new System.Windows.Forms.ComboBox();
            this.button7 = new System.Windows.Forms.Button();
            this.saveButton = new System.Windows.Forms.Button();
            this.selectorAxis = new System.Windows.Forms.ComboBox();
            this.counterSplits = new System.Windows.Forms.TextBox();
            this.buttonDrawSolid = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // pictureBox1
            // 
            this.pictureBox1.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.pictureBox1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pictureBox1.Location = new System.Drawing.Point(329, 38);
            this.pictureBox1.Margin = new System.Windows.Forms.Padding(4);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(1074, 604);
            this.pictureBox1.TabIndex = 0;
            this.pictureBox1.TabStop = false;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label1.Location = new System.Drawing.Point(19, 112);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(171, 25);
            this.label1.TabIndex = 1;
            this.label1.Text = "Выбор элемента";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(1217, 661);
            this.button1.Margin = new System.Windows.Forms.Padding(4);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(187, 73);
            this.button1.TabIndex = 2;
            this.button1.Text = "Очистить";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // comboBox1
            // 
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Items.AddRange(new object[] {
            "...",
            "Гексаэдр",
            "Тетраэдр",
            "Октаэдр",
            "Загрузить из файла",
            "Фигура вращения"});
            this.comboBox1.Location = new System.Drawing.Point(20, 159);
            this.comboBox1.Margin = new System.Windows.Forms.Padding(4);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(185, 24);
            this.comboBox1.TabIndex = 3;
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(20, 208);
            this.button2.Margin = new System.Windows.Forms.Padding(4);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(288, 123);
            this.button2.TabIndex = 4;
            this.button2.Text = "Нарисовать";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(20, 361);
            this.button3.Margin = new System.Windows.Forms.Padding(4);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(152, 49);
            this.button3.TabIndex = 13;
            this.button3.Text = "масштабирование";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // textBox5
            // 
            this.textBox5.Location = new System.Drawing.Point(180, 385);
            this.textBox5.Margin = new System.Windows.Forms.Padding(4);
            this.textBox5.Name = "textBox5";
            this.textBox5.Size = new System.Drawing.Size(36, 22);
            this.textBox5.TabIndex = 14;
            this.textBox5.Text = "1";
            // 
            // button4
            // 
            this.button4.Location = new System.Drawing.Point(20, 441);
            this.button4.Margin = new System.Windows.Forms.Padding(4);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(152, 49);
            this.button4.TabIndex = 15;
            this.button4.Text = "смещение";
            this.button4.UseVisualStyleBackColor = true;
            this.button4.Click += new System.EventHandler(this.button4_Click);
            // 
            // button5
            // 
            this.button5.Location = new System.Drawing.Point(20, 519);
            this.button5.Margin = new System.Windows.Forms.Padding(4);
            this.button5.Name = "button5";
            this.button5.Size = new System.Drawing.Size(151, 52);
            this.button5.TabIndex = 16;
            this.button5.Text = "отражение";
            this.button5.UseVisualStyleBackColor = true;
            this.button5.Click += new System.EventHandler(this.button5_Click);
            // 
            // button6
            // 
            this.button6.Location = new System.Drawing.Point(21, 603);
            this.button6.Margin = new System.Windows.Forms.Padding(4);
            this.button6.Name = "button6";
            this.button6.Size = new System.Drawing.Size(151, 52);
            this.button6.TabIndex = 17;
            this.button6.Text = "поворот";
            this.button6.UseVisualStyleBackColor = true;
            this.button6.Click += new System.EventHandler(this.button6_Click);
            // 
            // textBox6
            // 
            this.textBox6.Location = new System.Drawing.Point(180, 465);
            this.textBox6.Margin = new System.Windows.Forms.Padding(4);
            this.textBox6.Name = "textBox6";
            this.textBox6.Size = new System.Drawing.Size(36, 22);
            this.textBox6.TabIndex = 18;
            this.textBox6.Text = "0";
            // 
            // textBox7
            // 
            this.textBox7.Location = new System.Drawing.Point(225, 465);
            this.textBox7.Margin = new System.Windows.Forms.Padding(4);
            this.textBox7.Name = "textBox7";
            this.textBox7.Size = new System.Drawing.Size(37, 22);
            this.textBox7.TabIndex = 19;
            this.textBox7.Text = "0";
            // 
            // textBox8
            // 
            this.textBox8.Location = new System.Drawing.Point(272, 465);
            this.textBox8.Margin = new System.Windows.Forms.Padding(4);
            this.textBox8.Name = "textBox8";
            this.textBox8.Size = new System.Drawing.Size(36, 22);
            this.textBox8.TabIndex = 20;
            this.textBox8.Text = "0";
            // 
            // textBoxX2
            // 
            this.textBoxX2.Location = new System.Drawing.Point(180, 630);
            this.textBoxX2.Margin = new System.Windows.Forms.Padding(4);
            this.textBoxX2.Name = "textBoxX2";
            this.textBoxX2.Size = new System.Drawing.Size(36, 22);
            this.textBoxX2.TabIndex = 21;
            this.textBoxX2.Text = "10";
            // 
            // comboBox2
            // 
            this.comboBox2.FormattingEnabled = true;
            this.comboBox2.Items.AddRange(new object[] {
            "X",
            "Y",
            "Z"});
            this.comboBox2.Location = new System.Drawing.Point(179, 545);
            this.comboBox2.Margin = new System.Windows.Forms.Padding(4);
            this.comboBox2.Name = "comboBox2";
            this.comboBox2.Size = new System.Drawing.Size(129, 24);
            this.comboBox2.TabIndex = 22;
            this.comboBox2.Text = "...";
            // 
            // textBoxZ1
            // 
            this.textBoxZ1.Location = new System.Drawing.Point(271, 598);
            this.textBoxZ1.Margin = new System.Windows.Forms.Padding(4);
            this.textBoxZ1.Name = "textBoxZ1";
            this.textBoxZ1.Size = new System.Drawing.Size(36, 22);
            this.textBoxZ1.TabIndex = 23;
            this.textBoxZ1.Text = "0";
            // 
            // textBoxY2
            // 
            this.textBoxY2.Location = new System.Drawing.Point(225, 630);
            this.textBoxY2.Margin = new System.Windows.Forms.Padding(4);
            this.textBoxY2.Name = "textBoxY2";
            this.textBoxY2.Size = new System.Drawing.Size(36, 22);
            this.textBoxY2.TabIndex = 24;
            this.textBoxY2.Text = "10";
            // 
            // textBoxY1
            // 
            this.textBoxY1.Location = new System.Drawing.Point(225, 598);
            this.textBoxY1.Margin = new System.Windows.Forms.Padding(4);
            this.textBoxY1.Name = "textBoxY1";
            this.textBoxY1.Size = new System.Drawing.Size(36, 22);
            this.textBoxY1.TabIndex = 25;
            this.textBoxY1.Text = "0";
            // 
            // textBoxX1
            // 
            this.textBoxX1.Location = new System.Drawing.Point(180, 598);
            this.textBoxX1.Margin = new System.Windows.Forms.Padding(4);
            this.textBoxX1.Name = "textBoxX1";
            this.textBoxX1.Size = new System.Drawing.Size(36, 22);
            this.textBoxX1.TabIndex = 26;
            this.textBoxX1.Text = "0";
            // 
            // textBoxZ2
            // 
            this.textBoxZ2.Location = new System.Drawing.Point(272, 630);
            this.textBoxZ2.Margin = new System.Windows.Forms.Padding(4);
            this.textBoxZ2.Name = "textBoxZ2";
            this.textBoxZ2.Size = new System.Drawing.Size(36, 22);
            this.textBoxZ2.TabIndex = 27;
            this.textBoxZ2.Text = "10";
            // 
            // textBoxAngle
            // 
            this.textBoxAngle.Location = new System.Drawing.Point(227, 698);
            this.textBoxAngle.Margin = new System.Windows.Forms.Padding(4);
            this.textBoxAngle.Name = "textBoxAngle";
            this.textBoxAngle.Size = new System.Drawing.Size(36, 22);
            this.textBoxAngle.TabIndex = 28;
            this.textBoxAngle.Text = "45";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label2.Location = new System.Drawing.Point(180, 441);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(137, 15);
            this.label2.TabIndex = 29;
            this.label2.Text = "Введите координаты: ";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(199, 666);
            this.label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(99, 17);
            this.label3.TabIndex = 30;
            this.label3.Text = "Введите угол:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(20, 31);
            this.label4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(118, 17);
            this.label4.TabIndex = 31;
            this.label4.Text = "Выбор проекции";
            // 
            // comboBox3
            // 
            this.comboBox3.FormattingEnabled = true;
            this.comboBox3.Items.AddRange(new object[] {
            "Изометрическая",
            "Ортогональная на YoZ",
            "Ортогональная на XoZ",
            "Ортогональная на XoY",
            "Перспективная"});
            this.comboBox3.Location = new System.Drawing.Point(21, 66);
            this.comboBox3.Margin = new System.Windows.Forms.Padding(4);
            this.comboBox3.Name = "comboBox3";
            this.comboBox3.Size = new System.Drawing.Size(184, 24);
            this.comboBox3.TabIndex = 32;
            this.comboBox3.Text = "...";
            // 
            // button7
            // 
            this.button7.Location = new System.Drawing.Point(16, 681);
            this.button7.Margin = new System.Windows.Forms.Padding(4);
            this.button7.Name = "button7";
            this.button7.Size = new System.Drawing.Size(156, 81);
            this.button7.TabIndex = 33;
            this.button7.Text = "поворот вокруг прямой, проходящей через центр";
            this.button7.UseVisualStyleBackColor = true;
            this.button7.Click += new System.EventHandler(this.button7_Click);
            // 
            // saveButton
            // 
            this.saveButton.Location = new System.Drawing.Point(1000, 661);
            this.saveButton.Margin = new System.Windows.Forms.Padding(4);
            this.saveButton.Name = "saveButton";
            this.saveButton.Size = new System.Drawing.Size(187, 73);
            this.saveButton.TabIndex = 34;
            this.saveButton.Text = "Сохранить в файл ";
            this.saveButton.UseVisualStyleBackColor = true;
            this.saveButton.Click += new System.EventHandler(this.SaveButton_Click);
            // 
            // selectorAxis
            // 
            this.selectorAxis.FormattingEnabled = true;
            this.selectorAxis.Items.AddRange(new object[] {
            "X",
            "Y",
            "Z"});
            this.selectorAxis.Location = new System.Drawing.Point(342, 659);
            this.selectorAxis.Margin = new System.Windows.Forms.Padding(4);
            this.selectorAxis.Name = "selectorAxis";
            this.selectorAxis.Size = new System.Drawing.Size(129, 24);
            this.selectorAxis.TabIndex = 35;
            this.selectorAxis.Text = "Выберите ось";
            // 
            // counterSplits
            // 
            this.counterSplits.Location = new System.Drawing.Point(479, 661);
            this.counterSplits.Margin = new System.Windows.Forms.Padding(4);
            this.counterSplits.Name = "counterSplits";
            this.counterSplits.Size = new System.Drawing.Size(36, 22);
            this.counterSplits.TabIndex = 36;
            this.counterSplits.Text = "0";
            // 
            // buttonDrawSolid
            // 
            this.buttonDrawSolid.Location = new System.Drawing.Point(523, 654);
            this.buttonDrawSolid.Margin = new System.Windows.Forms.Padding(4);
            this.buttonDrawSolid.Name = "buttonDrawSolid";
            this.buttonDrawSolid.Size = new System.Drawing.Size(252, 36);
            this.buttonDrawSolid.TabIndex = 37;
            this.buttonDrawSolid.Text = "Нарисовать фигуру вращения";
            this.buttonDrawSolid.UseVisualStyleBackColor = true;
            this.buttonDrawSolid.Click += new System.EventHandler(this.ButtonDrawSolid_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1579, 814);
            this.Controls.Add(this.buttonDrawSolid);
            this.Controls.Add(this.counterSplits);
            this.Controls.Add(this.selectorAxis);
            this.Controls.Add(this.saveButton);
            this.Controls.Add(this.button7);
            this.Controls.Add(this.comboBox3);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.textBoxAngle);
            this.Controls.Add(this.textBoxZ2);
            this.Controls.Add(this.textBoxX1);
            this.Controls.Add(this.textBoxY1);
            this.Controls.Add(this.textBoxY2);
            this.Controls.Add(this.textBoxZ1);
            this.Controls.Add(this.comboBox2);
            this.Controls.Add(this.textBoxX2);
            this.Controls.Add(this.textBox8);
            this.Controls.Add(this.textBox7);
            this.Controls.Add(this.textBox6);
            this.Controls.Add(this.button6);
            this.Controls.Add(this.button5);
            this.Controls.Add(this.button4);
            this.Controls.Add(this.textBox5);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.comboBox1);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.pictureBox1);
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "Form1";
            this.Text = "Form1";
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        public System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.ComboBox comboBox1;
        private System.Windows.Forms.Button button2;
		private System.Windows.Forms.Button button3;
		private System.Windows.Forms.TextBox textBox5;
		private System.Windows.Forms.Button button4;
		private System.Windows.Forms.Button button5;
		private System.Windows.Forms.Button button6;
		private System.Windows.Forms.TextBox textBox6;
		private System.Windows.Forms.TextBox textBox7;
		private System.Windows.Forms.TextBox textBox8;
		private System.Windows.Forms.TextBox textBoxX2;
		private System.Windows.Forms.ComboBox comboBox2;
		private System.Windows.Forms.TextBox textBoxZ1;
		private System.Windows.Forms.TextBox textBoxY2;
		private System.Windows.Forms.TextBox textBoxY1;
		private System.Windows.Forms.TextBox textBoxX1;
		private System.Windows.Forms.TextBox textBoxZ2;
		private System.Windows.Forms.TextBox textBoxAngle;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ComboBox comboBox3;
        private System.Windows.Forms.Button button7;
        private System.Windows.Forms.Button saveButton;
        private System.Windows.Forms.ComboBox selectorAxis;
        private System.Windows.Forms.TextBox counterSplits;
        private System.Windows.Forms.Button buttonDrawSolid;
    }
}

