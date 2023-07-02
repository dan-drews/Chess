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
            chessBoard = new ChessBoard(_game, _whiteEngine, _blackEngine);
            SuspendLayout();
            // 
            // chessBoard
            // 
            chessBoard.AutoSize = true;
            chessBoard.Location = new Point(0, 0);
            chessBoard.Name = "chessBoard";
            chessBoard.Size = new Size(400, 400);
            chessBoard.TabIndex = 0;
            // 
            // MainGame
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(484, 461);
            Controls.Add(chessBoard);
            Name = "MainGame";
            Text = "MainGame";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private ChessBoard chessBoard;
    }
}