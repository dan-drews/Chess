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
            button1.Text = "button1";
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
            // MainGame
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(784, 961);
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
    }
}