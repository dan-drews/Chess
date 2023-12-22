namespace Chess.WinForms
{
    partial class MainGame
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
            components = new System.ComponentModel.Container();
            label1 = new Label();
            chessBoard = new ChessBoard();
            txtFen = new TextBox();
            btnLoadFen = new Button();
            button1 = new Button();
            chkWhiteCpu = new CheckBox();
            chkBlackCpu = new CheckBox();
            timer1 = new System.Windows.Forms.Timer(components);
            lblTimer = new Label();
            txtBlackCpuSeconds = new NumericUpDown();
            lblBlackCpuSeconds = new Label();
            lblWhiteCpuSeconds = new Label();
            txtWhiteCpuSeconds = new NumericUpDown();
            btnCopyPgn = new Button();
            ((System.ComponentModel.ISupportInitialize)txtBlackCpuSeconds).BeginInit();
            ((System.ComponentModel.ISupportInitialize)txtWhiteCpuSeconds).BeginInit();
            SuspendLayout();
            // 
            // label1
            // 
            label1.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            label1.AutoSize = true;
            label1.Location = new Point(12, 937);
            label1.Name = "label1";
            label1.Size = new Size(38, 15);
            label1.TabIndex = 1;
            label1.Text = "label1";
            // 
            // chessBoard
            // 
            chessBoard.IsBlackAi = false;
            chessBoard.IsCalculationg = false;
            chessBoard.IsWhiteAi = false;
            chessBoard.Location = new Point(0, 0);
            chessBoard.MainGame = null;
            chessBoard.Name = "chessBoard";
            chessBoard.Size = new Size(800, 800);
            chessBoard.TabIndex = 2;
            // 
            // txtFen
            // 
            txtFen.Location = new Point(12, 848);
            txtFen.Name = "txtFen";
            txtFen.Size = new Size(585, 23);
            txtFen.TabIndex = 3;
            // 
            // btnLoadFen
            // 
            btnLoadFen.Location = new Point(603, 848);
            btnLoadFen.Name = "btnLoadFen";
            btnLoadFen.Size = new Size(75, 23);
            btnLoadFen.TabIndex = 4;
            btnLoadFen.Text = "Load FEN";
            btnLoadFen.UseVisualStyleBackColor = true;
            btnLoadFen.Click += btnLoadFen_Click;
            // 
            // button1
            // 
            button1.Location = new Point(684, 848);
            button1.Name = "button1";
            button1.Size = new Size(75, 23);
            button1.TabIndex = 5;
            button1.Text = "Compute";
            button1.UseVisualStyleBackColor = true;
            button1.Click += button1_Click;
            // 
            // chkWhiteCpu
            // 
            chkWhiteCpu.AutoSize = true;
            chkWhiteCpu.Location = new Point(12, 877);
            chkWhiteCpu.Name = "chkWhiteCpu";
            chkWhiteCpu.Size = new Size(83, 19);
            chkWhiteCpu.TabIndex = 6;
            chkWhiteCpu.Text = "White CPU";
            chkWhiteCpu.UseVisualStyleBackColor = true;
            chkWhiteCpu.CheckedChanged += chkWhiteCpu_CheckedChanged;
            // 
            // chkBlackCpu
            // 
            chkBlackCpu.AutoSize = true;
            chkBlackCpu.Location = new Point(101, 877);
            chkBlackCpu.Name = "chkBlackCpu";
            chkBlackCpu.Size = new Size(80, 19);
            chkBlackCpu.TabIndex = 7;
            chkBlackCpu.Text = "Black CPU";
            chkBlackCpu.UseVisualStyleBackColor = true;
            chkBlackCpu.CheckedChanged += chkBlackCpu_CheckedChanged;
            // 
            // timer1
            // 
            timer1.Interval = 1000;
            timer1.Tick += timer1_Tick;
            // 
            // lblTimer
            // 
            lblTimer.AutoSize = true;
            lblTimer.Location = new Point(12, 899);
            lblTimer.Name = "lblTimer";
            lblTimer.Size = new Size(38, 15);
            lblTimer.TabIndex = 8;
            lblTimer.Text = "label2";
            // 
            // txtBlackCpuSeconds
            // 
            txtBlackCpuSeconds.DecimalPlaces = 5;
            txtBlackCpuSeconds.Location = new Point(187, 873);
            txtBlackCpuSeconds.Maximum = new decimal(new int[] { 1215752192, 23, 0, 0 });
            txtBlackCpuSeconds.Name = "txtBlackCpuSeconds";
            txtBlackCpuSeconds.Size = new Size(120, 23);
            txtBlackCpuSeconds.TabIndex = 9;
            txtBlackCpuSeconds.ValueChanged += txtBlackCpuSeconds_ValueChanged;
            // 
            // lblBlackCpuSeconds
            // 
            lblBlackCpuSeconds.AutoSize = true;
            lblBlackCpuSeconds.Location = new Point(313, 875);
            lblBlackCpuSeconds.Name = "lblBlackCpuSeconds";
            lblBlackCpuSeconds.Size = new Size(108, 15);
            lblBlackCpuSeconds.TabIndex = 10;
            lblBlackCpuSeconds.Text = "Black CPU Seconds";
            // 
            // lblWhiteCpuSeconds
            // 
            lblWhiteCpuSeconds.AutoSize = true;
            lblWhiteCpuSeconds.Location = new Point(553, 875);
            lblWhiteCpuSeconds.Name = "lblWhiteCpuSeconds";
            lblWhiteCpuSeconds.Size = new Size(111, 15);
            lblWhiteCpuSeconds.TabIndex = 12;
            lblWhiteCpuSeconds.Text = "White CPU Seconds";
            // 
            // txtWhiteCpuSeconds
            // 
            txtWhiteCpuSeconds.DecimalPlaces = 5;
            txtWhiteCpuSeconds.Location = new Point(427, 873);
            txtWhiteCpuSeconds.Maximum = new decimal(new int[] { 1215752192, 23, 0, 0 });
            txtWhiteCpuSeconds.Name = "txtWhiteCpuSeconds";
            txtWhiteCpuSeconds.Size = new Size(120, 23);
            txtWhiteCpuSeconds.TabIndex = 11;
            txtWhiteCpuSeconds.ValueChanged += txtWhiteCpuSeconds_ValueChanged;
            // 
            // btnCopyPgn
            // 
            btnCopyPgn.Location = new Point(684, 874);
            btnCopyPgn.Name = "btnCopyPgn";
            btnCopyPgn.Size = new Size(75, 23);
            btnCopyPgn.TabIndex = 13;
            btnCopyPgn.Text = "Copy PGN";
            btnCopyPgn.UseVisualStyleBackColor = true;
            btnCopyPgn.Click += btnCopyPgn_Click;
            // 
            // MainGame
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(784, 961);
            Controls.Add(btnCopyPgn);
            Controls.Add(lblWhiteCpuSeconds);
            Controls.Add(txtWhiteCpuSeconds);
            Controls.Add(lblBlackCpuSeconds);
            Controls.Add(txtBlackCpuSeconds);
            Controls.Add(lblTimer);
            Controls.Add(chkBlackCpu);
            Controls.Add(chkWhiteCpu);
            Controls.Add(button1);
            Controls.Add(btnLoadFen);
            Controls.Add(txtFen);
            Controls.Add(chessBoard);
            Controls.Add(label1);
            Name = "MainGame";
            Text = "MainGame";
            ResizeEnd += MainGame_ResizeEnd;
            ((System.ComponentModel.ISupportInitialize)txtBlackCpuSeconds).EndInit();
            ((System.ComponentModel.ISupportInitialize)txtWhiteCpuSeconds).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label label1;
        private ChessBoard chessBoard;
        private TextBox txtFen;
        private Button btnLoadFen;
        private Button button1;
        private CheckBox chkWhiteCpu;
        private CheckBox chkBlackCpu;
        private System.Windows.Forms.Timer timer1;
        private Label lblTimer;
        private NumericUpDown txtBlackCpuSeconds;
        private Label lblBlackCpuSeconds;
        private Label lblWhiteCpuSeconds;
        private NumericUpDown txtWhiteCpuSeconds;
        private Button btnCopyPgn;
    }
}