namespace PLSQLIndent
{
    partial class frmIndenter
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmIndenter));
            this.tabFiles = new System.Windows.Forms.TabControl();
            this.tpgOriginal = new System.Windows.Forms.TabPage();
            this.txtOriginal = new ScintillaNET.Scintilla();
            this.txtOriginalOld = new System.Windows.Forms.RichTextBox();
            this.lblLineNumber = new System.Windows.Forms.Label();
            this.lblLine = new System.Windows.Forms.Label();
            this.tpgIndented = new System.Windows.Forms.TabPage();
            this.txtIndented = new ScintillaNET.Scintilla();
            this.txtIndentedOld = new System.Windows.Forms.TextBox();
            this.tstOptions = new System.Windows.Forms.ToolStrip();
            this.tsbOpen = new System.Windows.Forms.ToolStripButton();
            this.tsbIndent = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.tsbSettings = new System.Windows.Forms.ToolStripButton();
            this.tsbUpper = new System.Windows.Forms.ToolStripButton();
            this.tsbLineTerminator = new System.Windows.Forms.ToolStripButton();
            this.tsbLineNumber = new System.Windows.Forms.ToolStripButton();
            this.tsbSpaces = new System.Windows.Forms.ToolStripButton();
            this.tsbAbout = new System.Windows.Forms.ToolStripButton();
            this.dlgOpen = new System.Windows.Forms.OpenFileDialog();
            this.pgbProgreso = new System.Windows.Forms.ProgressBar();
            this.chkKeepGreaterIndent = new System.Windows.Forms.CheckBox();
            this.bgwProceso = new System.ComponentModel.BackgroundWorker();
            this.ttips = new System.Windows.Forms.ToolTip(this.components);
            this.chkNameProcEnds = new System.Windows.Forms.CheckBox();
            this.chkNameProcBegins = new System.Windows.Forms.CheckBox();
            this.chkSpaceBeforeProc = new System.Windows.Forms.CheckBox();
            this.chkFixVarDeclares = new System.Windows.Forms.CheckBox();
            this.pgbIndent = new System.Windows.Forms.ProgressBar();
            this.lblProgreso = new System.Windows.Forms.Label();
            this.lblIndentacion = new System.Windows.Forms.Label();
            this.tsbAlinear = new System.Windows.Forms.ToolStripButton();
            this.tabFiles.SuspendLayout();
            this.tpgOriginal.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtOriginal)).BeginInit();
            this.tpgIndented.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtIndented)).BeginInit();
            this.tstOptions.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabFiles
            // 
            this.tabFiles.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabFiles.Controls.Add(this.tpgOriginal);
            this.tabFiles.Controls.Add(this.tpgIndented);
            this.tabFiles.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tabFiles.Location = new System.Drawing.Point(12, 103);
            this.tabFiles.Name = "tabFiles";
            this.tabFiles.SelectedIndex = 0;
            this.tabFiles.Size = new System.Drawing.Size(707, 469);
            this.tabFiles.TabIndex = 0;
            this.tabFiles.SelectedIndexChanged += new System.EventHandler(this.tabFiles_SelectedIndexChanged);
            // 
            // tpgOriginal
            // 
            this.tpgOriginal.Controls.Add(this.txtOriginal);
            this.tpgOriginal.Controls.Add(this.txtOriginalOld);
            this.tpgOriginal.Controls.Add(this.lblLineNumber);
            this.tpgOriginal.Controls.Add(this.lblLine);
            this.tpgOriginal.Location = new System.Drawing.Point(4, 22);
            this.tpgOriginal.Name = "tpgOriginal";
            this.tpgOriginal.Padding = new System.Windows.Forms.Padding(3);
            this.tpgOriginal.Size = new System.Drawing.Size(699, 443);
            this.tpgOriginal.TabIndex = 0;
            this.tpgOriginal.Text = "   Original    ";
            this.tpgOriginal.UseVisualStyleBackColor = true;
            // 
            // txtOriginal
            // 
            this.txtOriginal.BackColor = System.Drawing.Color.WhiteSmoke;
            this.txtOriginal.ConfigurationManager.Language = "sql";
            this.txtOriginal.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtOriginal.Font = new System.Drawing.Font("Lucida Sans Typewriter", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtOriginal.IsBraceMatching = true;
            this.txtOriginal.Lexing.Lexer = ScintillaNET.Lexer.MsSql;
            this.txtOriginal.Lexing.LexerName = "mssql";
            this.txtOriginal.Lexing.LineCommentPrefix = "";
            this.txtOriginal.Lexing.StreamCommentPrefix = "";
            this.txtOriginal.Lexing.StreamCommentSufix = "";
            this.txtOriginal.Location = new System.Drawing.Point(3, 3);
            this.txtOriginal.Margins.Margin0.Width = 40;
            this.txtOriginal.Name = "txtOriginal";
            this.txtOriginal.Size = new System.Drawing.Size(693, 437);
            this.txtOriginal.Styles.LineNumber.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.txtOriginal.Styles.LineNumber.IsChangeable = false;
            this.txtOriginal.TabIndex = 9;
            // 
            // txtOriginalOld
            // 
            this.txtOriginalOld.AcceptsTab = true;
            this.txtOriginalOld.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtOriginalOld.DetectUrls = false;
            this.txtOriginalOld.Font = new System.Drawing.Font("Lucida Sans Typewriter", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtOriginalOld.Location = new System.Drawing.Point(9, 19);
            this.txtOriginalOld.Name = "txtOriginalOld";
            this.txtOriginalOld.Size = new System.Drawing.Size(40, 421);
            this.txtOriginalOld.TabIndex = 8;
            this.txtOriginalOld.Text = "";
            this.txtOriginalOld.WordWrap = false;
            this.txtOriginalOld.SelectionChanged += new System.EventHandler(this.txtOriginal_SelectionChanged_1);
            this.txtOriginalOld.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtOriginal_KeyDown);
            // 
            // lblLineNumber
            // 
            this.lblLineNumber.AutoSize = true;
            this.lblLineNumber.Location = new System.Drawing.Point(45, 3);
            this.lblLineNumber.Name = "lblLineNumber";
            this.lblLineNumber.Size = new System.Drawing.Size(13, 13);
            this.lblLineNumber.TabIndex = 2;
            this.lblLineNumber.Text = "0";
            this.lblLineNumber.Visible = false;
            // 
            // lblLine
            // 
            this.lblLine.AutoSize = true;
            this.lblLine.Location = new System.Drawing.Point(6, 3);
            this.lblLine.Name = "lblLine";
            this.lblLine.Size = new System.Drawing.Size(33, 13);
            this.lblLine.TabIndex = 1;
            this.lblLine.Text = "Line :";
            this.lblLine.Visible = false;
            // 
            // tpgIndented
            // 
            this.tpgIndented.BackColor = System.Drawing.Color.LightBlue;
            this.tpgIndented.Controls.Add(this.txtIndented);
            this.tpgIndented.Controls.Add(this.txtIndentedOld);
            this.tpgIndented.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tpgIndented.ForeColor = System.Drawing.SystemColors.HotTrack;
            this.tpgIndented.Location = new System.Drawing.Point(4, 22);
            this.tpgIndented.Name = "tpgIndented";
            this.tpgIndented.Padding = new System.Windows.Forms.Padding(3);
            this.tpgIndented.Size = new System.Drawing.Size(699, 443);
            this.tpgIndented.TabIndex = 1;
            this.tpgIndented.Text = "  Indented  ";
            // 
            // txtIndented
            // 
            this.txtIndented.BackColor = System.Drawing.Color.White;
            this.txtIndented.ConfigurationManager.Language = "sql";
            this.txtIndented.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtIndented.Font = new System.Drawing.Font("Lucida Sans Typewriter", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtIndented.IsBraceMatching = true;
            this.txtIndented.Lexing.Lexer = ScintillaNET.Lexer.MsSql;
            this.txtIndented.Lexing.LexerName = "mssql";
            this.txtIndented.Lexing.LineCommentPrefix = "";
            this.txtIndented.Lexing.StreamCommentPrefix = "";
            this.txtIndented.Lexing.StreamCommentSufix = "";
            this.txtIndented.Location = new System.Drawing.Point(3, 3);
            this.txtIndented.Margins.Margin0.Width = 40;
            this.txtIndented.Name = "txtIndented";
            this.txtIndented.Size = new System.Drawing.Size(693, 437);
            this.txtIndented.Styles.LineNumber.BackColor = System.Drawing.Color.YellowGreen;
            this.txtIndented.Styles.LineNumber.IsChangeable = false;
            this.txtIndented.TabIndex = 10;
            // 
            // txtIndentedOld
            // 
            this.txtIndentedOld.Font = new System.Drawing.Font("Lucida Sans Typewriter", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtIndentedOld.ForeColor = System.Drawing.Color.Navy;
            this.txtIndentedOld.Location = new System.Drawing.Point(3, 3);
            this.txtIndentedOld.Multiline = true;
            this.txtIndentedOld.Name = "txtIndentedOld";
            this.txtIndentedOld.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtIndentedOld.Size = new System.Drawing.Size(101, 113);
            this.txtIndentedOld.TabIndex = 0;
            this.txtIndentedOld.Visible = false;
            this.txtIndentedOld.WordWrap = false;
            this.txtIndentedOld.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtIndented_KeyDown);
            // 
            // tstOptions
            // 
            this.tstOptions.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.tstOptions.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsbOpen,
            this.tsbIndent,
            this.toolStripSeparator1,
            this.tsbAlinear,
            this.tsbSettings,
            this.tsbUpper,
            this.tsbLineTerminator,
            this.tsbLineNumber,
            this.tsbSpaces,
            this.tsbAbout});
            this.tstOptions.Location = new System.Drawing.Point(0, 0);
            this.tstOptions.Name = "tstOptions";
            this.tstOptions.Size = new System.Drawing.Size(731, 25);
            this.tstOptions.TabIndex = 1;
            this.tstOptions.Text = "toolStrip1";
            // 
            // tsbOpen
            // 
            this.tsbOpen.Image = ((System.Drawing.Image)(resources.GetObject("tsbOpen.Image")));
            this.tsbOpen.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbOpen.Name = "tsbOpen";
            this.tsbOpen.Size = new System.Drawing.Size(56, 22);
            this.tsbOpen.Text = "Open";
            this.tsbOpen.ToolTipText = "Open File for Indentation";
            this.tsbOpen.Click += new System.EventHandler(this.tsbOpen_Click);
            // 
            // tsbIndent
            // 
            this.tsbIndent.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tsbIndent.Image = ((System.Drawing.Image)(resources.GetObject("tsbIndent.Image")));
            this.tsbIndent.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbIndent.Name = "tsbIndent";
            this.tsbIndent.Size = new System.Drawing.Size(64, 22);
            this.tsbIndent.Text = "Indent";
            this.tsbIndent.ToolTipText = "Indent Original Code";
            this.tsbIndent.Click += new System.EventHandler(this.tsbIndent_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
            // 
            // tsbSettings
            // 
            this.tsbSettings.Image = ((System.Drawing.Image)(resources.GetObject("tsbSettings.Image")));
            this.tsbSettings.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbSettings.Name = "tsbSettings";
            this.tsbSettings.Size = new System.Drawing.Size(81, 22);
            this.tsbSettings.Text = "Settings ...";
            this.tsbSettings.ToolTipText = "Set Indentation Options";
            this.tsbSettings.Visible = false;
            // 
            // tsbUpper
            // 
            this.tsbUpper.Image = ((System.Drawing.Image)(resources.GetObject("tsbUpper.Image")));
            this.tsbUpper.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbUpper.Name = "tsbUpper";
            this.tsbUpper.Size = new System.Drawing.Size(80, 22);
            this.tsbUpper.Text = "Upper KW";
            this.tsbUpper.ToolTipText = "Upper Case Keywords, No indentation";
            this.tsbUpper.Click += new System.EventHandler(this.tsbUpper_Click);
            // 
            // tsbLineTerminator
            // 
            this.tsbLineTerminator.CheckOnClick = true;
            this.tsbLineTerminator.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbLineTerminator.Image = ((System.Drawing.Image)(resources.GetObject("tsbLineTerminator.Image")));
            this.tsbLineTerminator.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbLineTerminator.Name = "tsbLineTerminator";
            this.tsbLineTerminator.Size = new System.Drawing.Size(23, 22);
            this.tsbLineTerminator.Text = "toolStripButton1";
            this.tsbLineTerminator.ToolTipText = "Show Line termination";
            this.tsbLineTerminator.Click += new System.EventHandler(this.tsbLineTerminator_Click);
            // 
            // tsbLineNumber
            // 
            this.tsbLineNumber.Checked = true;
            this.tsbLineNumber.CheckOnClick = true;
            this.tsbLineNumber.CheckState = System.Windows.Forms.CheckState.Checked;
            this.tsbLineNumber.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbLineNumber.Image = ((System.Drawing.Image)(resources.GetObject("tsbLineNumber.Image")));
            this.tsbLineNumber.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbLineNumber.Name = "tsbLineNumber";
            this.tsbLineNumber.Size = new System.Drawing.Size(23, 22);
            this.tsbLineNumber.Text = "toolStripButton2";
            this.tsbLineNumber.ToolTipText = "Show Line numbers";
            this.tsbLineNumber.Click += new System.EventHandler(this.tsbLineNumber_Click);
            // 
            // tsbSpaces
            // 
            this.tsbSpaces.CheckOnClick = true;
            this.tsbSpaces.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbSpaces.Image = ((System.Drawing.Image)(resources.GetObject("tsbSpaces.Image")));
            this.tsbSpaces.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbSpaces.Name = "tsbSpaces";
            this.tsbSpaces.Size = new System.Drawing.Size(23, 22);
            this.tsbSpaces.Text = "toolStripButton3";
            this.tsbSpaces.ToolTipText = "Show spaces";
            this.tsbSpaces.Click += new System.EventHandler(this.tsbSpaces_Click);
            // 
            // tsbAbout
            // 
            this.tsbAbout.Image = ((System.Drawing.Image)(resources.GetObject("tsbAbout.Image")));
            this.tsbAbout.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbAbout.Name = "tsbAbout";
            this.tsbAbout.Size = new System.Drawing.Size(72, 22);
            this.tsbAbout.Text = "About ...";
            this.tsbAbout.ToolTipText = "About Indenter";
            this.tsbAbout.Click += new System.EventHandler(this.tsbAbout_Click);
            // 
            // dlgOpen
            // 
            this.dlgOpen.DefaultExt = "*.*";
            this.dlgOpen.Filter = "Script Files|*.sql|Text Files|*.txt|All Files|*.*";
            this.dlgOpen.Title = "Select the source code file to indent";
            // 
            // pgbProgreso
            // 
            this.pgbProgreso.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pgbProgreso.Location = new System.Drawing.Point(75, 28);
            this.pgbProgreso.Name = "pgbProgreso";
            this.pgbProgreso.Size = new System.Drawing.Size(644, 23);
            this.pgbProgreso.TabIndex = 1;
            // 
            // chkKeepGreaterIndent
            // 
            this.chkKeepGreaterIndent.AutoSize = true;
            this.chkKeepGreaterIndent.Location = new System.Drawing.Point(12, 80);
            this.chkKeepGreaterIndent.Name = "chkKeepGreaterIndent";
            this.chkKeepGreaterIndent.Size = new System.Drawing.Size(119, 17);
            this.chkKeepGreaterIndent.TabIndex = 1;
            this.chkKeepGreaterIndent.Text = "Keep greater indent";
            this.ttips.SetToolTip(this.chkKeepGreaterIndent, "If line indentation is greater then keep it");
            this.chkKeepGreaterIndent.UseVisualStyleBackColor = true;
            // 
            // bgwProceso
            // 
            this.bgwProceso.WorkerReportsProgress = true;
            this.bgwProceso.DoWork += new System.ComponentModel.DoWorkEventHandler(this.bgwProceso_DoWork);
            this.bgwProceso.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.bgwProceso_ProgressChanged);
            this.bgwProceso.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.bgwProceso_RunWorkerCompleted);
            // 
            // chkNameProcEnds
            // 
            this.chkNameProcEnds.AutoSize = true;
            this.chkNameProcEnds.Location = new System.Drawing.Point(272, 80);
            this.chkNameProcEnds.Name = "chkNameProcEnds";
            this.chkNameProcEnds.Size = new System.Drawing.Size(126, 17);
            this.chkNameProcEnds.TabIndex = 3;
            this.chkNameProcEnds.Text = "Name Proc End keys";
            this.ttips.SetToolTip(this.chkNameProcEnds, "Set a name to end keywords if not named.");
            this.chkNameProcEnds.UseVisualStyleBackColor = true;
            // 
            // chkNameProcBegins
            // 
            this.chkNameProcBegins.AutoSize = true;
            this.chkNameProcBegins.Checked = true;
            this.chkNameProcBegins.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkNameProcBegins.Location = new System.Drawing.Point(137, 80);
            this.chkNameProcBegins.Name = "chkNameProcBegins";
            this.chkNameProcBegins.Size = new System.Drawing.Size(134, 17);
            this.chkNameProcBegins.TabIndex = 4;
            this.chkNameProcBegins.Text = "Name Proc Begin keys";
            this.ttips.SetToolTip(this.chkNameProcBegins, "Set a name to the begin keywords if not named.\r\n");
            this.chkNameProcBegins.UseVisualStyleBackColor = true;
            // 
            // chkSpaceBeforeProc
            // 
            this.chkSpaceBeforeProc.AutoSize = true;
            this.chkSpaceBeforeProc.Checked = true;
            this.chkSpaceBeforeProc.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkSpaceBeforeProc.Location = new System.Drawing.Point(404, 80);
            this.chkSpaceBeforeProc.Name = "chkSpaceBeforeProc";
            this.chkSpaceBeforeProc.Size = new System.Drawing.Size(181, 17);
            this.chkSpaceBeforeProc.TabIndex = 5;
            this.chkSpaceBeforeProc.Text = "Leave a space before procedure";
            this.ttips.SetToolTip(this.chkSpaceBeforeProc, "Leave a space before a procedure is declared if a space is missing");
            this.chkSpaceBeforeProc.UseVisualStyleBackColor = true;
            // 
            // chkFixVarDeclares
            // 
            this.chkFixVarDeclares.AutoSize = true;
            this.chkFixVarDeclares.Location = new System.Drawing.Point(585, 80);
            this.chkFixVarDeclares.Name = "chkFixVarDeclares";
            this.chkFixVarDeclares.Size = new System.Drawing.Size(141, 17);
            this.chkFixVarDeclares.TabIndex = 8;
            this.chkFixVarDeclares.Text = "Fix variable Declarations";
            this.ttips.SetToolTip(this.chkFixVarDeclares, "Align variable declaration types.");
            this.chkFixVarDeclares.UseVisualStyleBackColor = true;
            // 
            // pgbIndent
            // 
            this.pgbIndent.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pgbIndent.Location = new System.Drawing.Point(75, 51);
            this.pgbIndent.Maximum = 50;
            this.pgbIndent.Name = "pgbIndent";
            this.pgbIndent.Size = new System.Drawing.Size(644, 23);
            this.pgbIndent.TabIndex = 2;
            // 
            // lblProgreso
            // 
            this.lblProgreso.AutoSize = true;
            this.lblProgreso.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblProgreso.Location = new System.Drawing.Point(9, 32);
            this.lblProgreso.Name = "lblProgreso";
            this.lblProgreso.Size = new System.Drawing.Size(56, 13);
            this.lblProgreso.TabIndex = 6;
            this.lblProgreso.Text = "Progress";
            // 
            // lblIndentacion
            // 
            this.lblIndentacion.AutoSize = true;
            this.lblIndentacion.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblIndentacion.Location = new System.Drawing.Point(9, 54);
            this.lblIndentacion.Name = "lblIndentacion";
            this.lblIndentacion.Size = new System.Drawing.Size(43, 13);
            this.lblIndentacion.TabIndex = 7;
            this.lblIndentacion.Text = "Indent";
            // 
            // tsbAlinear
            // 
            this.tsbAlinear.Image = ((System.Drawing.Image)(resources.GetObject("tsbAlinear.Image")));
            this.tsbAlinear.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbAlinear.Name = "tsbAlinear";
            this.tsbAlinear.Size = new System.Drawing.Size(64, 22);
            this.tsbAlinear.Text = "Align";
            this.tsbAlinear.ToolTipText = "Align variable definition and assignments";
            this.tsbAlinear.Click += new System.EventHandler(this.tsbAlinear_Click);
            // 
            // frmIndenter
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(731, 584);
            this.Controls.Add(this.chkFixVarDeclares);
            this.Controls.Add(this.lblIndentacion);
            this.Controls.Add(this.lblProgreso);
            this.Controls.Add(this.chkSpaceBeforeProc);
            this.Controls.Add(this.chkNameProcBegins);
            this.Controls.Add(this.chkNameProcEnds);
            this.Controls.Add(this.pgbIndent);
            this.Controls.Add(this.chkKeepGreaterIndent);
            this.Controls.Add(this.pgbProgreso);
            this.Controls.Add(this.tstOptions);
            this.Controls.Add(this.tabFiles);
            this.Name = "frmIndenter";
            this.Text = "Indenter";
            this.tabFiles.ResumeLayout(false);
            this.tpgOriginal.ResumeLayout(false);
            this.tpgOriginal.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtOriginal)).EndInit();
            this.tpgIndented.ResumeLayout(false);
            this.tpgIndented.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtIndented)).EndInit();
            this.tstOptions.ResumeLayout(false);
            this.tstOptions.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TabPage tpgOriginal;
        private System.Windows.Forms.TabPage tpgIndented;
        private System.Windows.Forms.ToolStrip tstOptions;
        private System.Windows.Forms.ToolStripButton tsbOpen;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripButton tsbSettings;
        private System.Windows.Forms.ToolStripButton tsbAbout;
        private System.Windows.Forms.OpenFileDialog dlgOpen;
        private System.Windows.Forms.ToolStripButton tsbIndent;
        private System.Windows.Forms.ToolStripButton tsbUpper;
        public System.Windows.Forms.TextBox txtIndentedOld;
        public System.Windows.Forms.TabControl tabFiles;
        private System.Windows.Forms.ProgressBar pgbProgreso;
        private System.Windows.Forms.CheckBox chkKeepGreaterIndent;
        private System.ComponentModel.BackgroundWorker bgwProceso;
        private System.Windows.Forms.ToolTip ttips;
        private System.Windows.Forms.ProgressBar pgbIndent;
        private System.Windows.Forms.CheckBox chkNameProcEnds;
        private System.Windows.Forms.CheckBox chkNameProcBegins;
        private System.Windows.Forms.CheckBox chkSpaceBeforeProc;
        private System.Windows.Forms.Label lblProgreso;
        private System.Windows.Forms.Label lblIndentacion;
        private System.Windows.Forms.Label lblLineNumber;
        private System.Windows.Forms.Label lblLine;
        private System.Windows.Forms.RichTextBox txtOriginalOld;
        private ScintillaNET.Scintilla txtOriginal;
        private ScintillaNET.Scintilla txtIndented;
        private System.Windows.Forms.ToolStripButton tsbLineTerminator;
        private System.Windows.Forms.ToolStripButton tsbLineNumber;
        private System.Windows.Forms.ToolStripButton tsbSpaces;
        private System.Windows.Forms.CheckBox chkFixVarDeclares;
        private System.Windows.Forms.ToolStripButton tsbAlinear;
    }
}

