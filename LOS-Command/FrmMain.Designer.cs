namespace LOS_Command
{
    partial class FrmMain
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmMain));
            this.ConIO = new FastColoredTextBoxNS.FastColoredTextBox();
            ((System.ComponentModel.ISupportInitialize)(this.ConIO)).BeginInit();
            this.SuspendLayout();
            // 
            // ConIO
            // 
            this.ConIO.AllowDrop = false;
            this.ConIO.AllowMacroRecording = false;
            this.ConIO.AutoCompleteBracketsList = new char[] {
        '(',
        ')',
        '{',
        '}',
        '[',
        ']',
        '\"',
        '\"',
        '\'',
        '\''};
            this.ConIO.AutoIndentChars = false;
            this.ConIO.AutoScrollMinSize = new System.Drawing.Size(0, 14);
            this.ConIO.BackBrush = null;
            this.ConIO.BackColor = System.Drawing.Color.MidnightBlue;
            this.ConIO.CaretColor = System.Drawing.Color.Yellow;
            this.ConIO.CharHeight = 14;
            this.ConIO.CharWidth = 8;
            this.ConIO.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.ConIO.DisabledColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(180)))), ((int)(((byte)(180)))), ((int)(((byte)(180)))));
            this.ConIO.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ConIO.ForeColor = System.Drawing.Color.Yellow;
            this.ConIO.HighlightFoldingIndicator = false;
            this.ConIO.IndentBackColor = System.Drawing.Color.Transparent;
            this.ConIO.IsReplaceMode = false;
            this.ConIO.Location = new System.Drawing.Point(0, 0);
            this.ConIO.Name = "ConIO";
            this.ConIO.Paddings = new System.Windows.Forms.Padding(0);
            this.ConIO.SelectionColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(255)))));
            this.ConIO.ServiceColors = ((FastColoredTextBoxNS.ServiceColors)(resources.GetObject("ConIO.ServiceColors")));
            this.ConIO.ServiceLinesColor = System.Drawing.Color.Transparent;
            this.ConIO.ShowLineNumbers = false;
            this.ConIO.Size = new System.Drawing.Size(725, 492);
            this.ConIO.TabIndex = 0;
            this.ConIO.TabStop = false;
            this.ConIO.WideCaret = true;
            this.ConIO.WordWrap = true;
            this.ConIO.WordWrapIndent = 4;
            this.ConIO.Zoom = 100;
            this.ConIO.LineInserted += new System.EventHandler<FastColoredTextBoxNS.LineInsertedEventArgs>(this.ConIO_LineInserted);
            this.ConIO.KeyUp += new System.Windows.Forms.KeyEventHandler(this.ConIO_KeyUp);
            // 
            // FrmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(725, 492);
            this.Controls.Add(this.ConIO);
            this.Name = "FrmMain";
            this.Text = "Commander";
            ((System.ComponentModel.ISupportInitialize)(this.ConIO)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private FastColoredTextBoxNS.FastColoredTextBox ConIO;
    }
}

