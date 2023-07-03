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
            label1 = new Label();
            chessBoard = new ChessBoard();
            txtFen = new TextBox();
            btnLoadFen = new Button();
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
            chessBoard.Location = new Point(0, 0);
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
            // MainGame
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(984, 961);
            Controls.Add(btnLoadFen);
            Controls.Add(txtFen);
            Controls.Add(chessBoard);
            Controls.Add(label1);
            Name = "MainGame";
            Text = "MainGame";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label label1;
        private ChessBoard chessBoard;
        private TextBox txtFen;
        private Button btnLoadFen;
    }
}