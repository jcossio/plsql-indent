using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using System.Linq;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;


namespace PLSQLIndent
{
    public partial class frmIndenter : Form
    {
        // Statements stack
        Stack<Statement> stckStatements = new Stack<Statement>();
        int CurrentIndentation = 0;

        string OriginalCode = string.Empty;
        string IndentedCode = string.Empty;

        // Expresiones regulare evaluadas aqui
        Regex rgPACKAGE       = new Regex(@"PACKAGE( )+(BODY)? ([""a-zA-Z0-9_]+)\b", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        Regex rgIS            = new Regex(@"\b(IS|AS)\b", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        Regex rgENDPACKAGE    = new Regex("END(\r\n)?( PACKAGE)?( )*;", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        Regex rgENDPACKAGE2   = new Regex(@"END(\r\n)?( PACKAGE)?( )*([""a-zA-Z0-9_]?)\b;", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        Regex rgPROCFUN       = new Regex(@"(PROCEDURE|FUNCTION)([ a-z0-9_]+) ?", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        Regex rgDECLARE       = new Regex(@"^ ?DECLARE", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        Regex rgENDPROCFUN    = new Regex("END( )? ", RegexOptions.IgnoreCase);
        Regex rgBEGIN         = new Regex(@"( )*BEGIN[ a-z0-9-]?", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        Regex rgIF            = new Regex(@"^((?!END\b)(?!ELS) )*IF\b+", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        Regex rgTHEN          = new Regex(@"( )*THEN( )*", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        Regex rgENDIF         = new Regex(@"END( )+IF( )?;", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        Regex rgEND           = new Regex(@"( )*?END( )?;", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        Regex rgFOR           = new Regex(@"^( )*(FOR|WHILE)( )+[^UPDATE]", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        Regex rgLOOP          = new Regex(@"[ ]?LOOP( )?", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        Regex rgENDLOOP       = new Regex(@"^( )*END LOOP( )?;", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        Regex rgSELECT        = new Regex(@"^[^']*(?<KWD>SELECT)( )+", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        Regex rgUPDATE        = new Regex(@"^[^']*(?<KWD>UPDATE)( )+", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        Regex rgDELETE        = new Regex(@"^[^']*(?<KWD>DELETE)( )+", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        Regex rgENDSELECT     = new Regex(@";", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        Regex rgENDINSERT     = new Regex(@";", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        Regex rgENDUPDATE     = new Regex(@";", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        Regex rgENDDELETE     = new Regex(@";", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        Regex rgELSE          = new Regex(@"\b?ELSE\b?", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        Regex rgEXCEPTION     = new Regex(@"^( )*EXCEPTION( )?", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        Regex rgELSIF         = new Regex(@"\b?ELSIF\b+", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        Regex rgLINECOMMENT   = new Regex(@"^( )*--", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        Regex rgBLKCOMMENTSTA = new Regex(@"^( )*\/\*", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        Regex rgBLKCOMMENTSTA2 = new Regex(@"\/\*", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        Regex rgBLKCOMMENTEND = new Regex(@"\*\/", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        Regex rgUNION         = new Regex(@"( )+UNION (ALL)?", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        Regex rgASSIGN        = new Regex(@"(.)*:=(.)*;", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        Regex rgSELECTKWS     = new Regex(@"^( )*(?<KWD>WHERE|INTO|FROM|AND|OR|ORDER|GROUP|HAVING) ", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        Regex rgINSERTKWS     = new Regex(@"^( )*(?<KWD>VALUES|SELECT)\(?", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        Regex rgUPDATEKWS     = new Regex(@"^( )*(?<KWD>WHERE|SET|AND|OR) ", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        Regex rgDELETEKWS     = new Regex(@"^( )*(?<KWD>WHERE|AND|OR) ", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        Regex rgSELECTKWSEND  = new Regex(@"[a-z0-9_]+( )+(WHERE|INTO|FROM|AND|OR|ORDER|GROUP|HAVING)", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        Regex rgCURSOR        = new Regex(@"^( )*CURSOR ", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        Regex rgTYPE          = new Regex(@"^( )*TYPE ", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        Regex rgWHEN          = new Regex(@"^( )*WHEN\b", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        Regex rgINTOTABLE     = new Regex(@"^( )*INTO( )+(?<TABLENAME>[a-z0-9_]+)", RegexOptions.IgnoreCase | RegexOptions.Compiled);

        public frmIndenter()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Open a file for indentation and place on original tab
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tsbOpen_Click(object sender, EventArgs e)
        {
            // Get the file from file
            dlgOpen.CheckFileExists =true;
            DialogResult result = dlgOpen.ShowDialog(this);
            if (result != System.Windows.Forms.DialogResult.OK)
            {
                return;
            }

            Cursor.Current = Cursors.WaitCursor;

            // Open the file specified by the user and dump on first tab
            //StreamReader reader = File.Open(dlgOpen.FileName,FileMode.Open,FileAccess.Read,FileShare.ReadWrite);
            StreamReader reader = new StreamReader(dlgOpen.FileName, Encoding.Default, true);

            txtOriginal.Text =  reader.ReadToEnd();
            reader.Close();
            pgbProgreso.Maximum = txtOriginal.Text.Count(f => f == '\n') + 10;
            pgbProgreso.Value = 0;
            OriginalCode = txtOriginal.Text;

            // Focus on tab
            txtOriginal.Focus();
            Cursor.Current = Cursors.Default;
        }

        /// <summary>
        /// Indent the file on the original tab
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tsbIndent_Click(object sender, EventArgs e)
        {

            // Checks
            if (txtOriginal.Text.Trim() == string.Empty)
            {
                return;
            }

            tsbIndent.Enabled = false;
            tsbUpper.Enabled = false;
            txtOriginal.Enabled = false;
            txtIndented.Enabled = false;
            OriginalCode = txtOriginal.Text;
            pgbProgreso.Maximum = txtOriginal.Text.Count(f => f == '\n') + 10;
            pgbProgreso.Value = 0;
            CurrentIndentation = 0;

            // Correr proceso asincronamente
            bgwProceso.RunWorkerAsync();
        }

        private void tsbUpper_Click(object sender, EventArgs e)
        {

            // Checks
            if (txtOriginal.Text.Trim() == string.Empty)
            {
                return;
            }
            OriginalCode = txtOriginal.Text;
            UpperCaseCode();
            txtIndented.Text = IndentedCode;
            tpgOriginal.Focus();
            System.Windows.Forms.SendKeys.Send("+{TAB}");
            System.Windows.Forms.SendKeys.Send("{RIGHT}");

        }

        private void tsbAbout_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Brute force PL/SQL indentation and upper casing by\nJorge Cossio 2015");
        }

        /// <summary>
        /// Pasar todas las palabras claves conocidas a upper case
        /// </summary>
        void UpperCaseCode()
        {

            string UpperCode = OriginalCode;

            Cursor.Current = Cursors.WaitCursor;
            /*
            UpperCode = Regex.Replace(UpperCode, @"<Calendars>.*</Calendars>", "", RegexOptions.Multiline | RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
            UpperCode = Regex.Replace(UpperCode, @"<ManualStart>.+?</IgnoreResourceCalendar>", "", RegexOptions.Multiline | RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
            */
            UpperCode = Regex.Replace(UpperCode, @"\t",            @"    ",      RegexOptions.IgnoreCase | RegexOptions.CultureInvariant); // TABS a espacios
            UpperCode = Regex.Replace(UpperCode, @"\ball\b",       @"ALL",       RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
            UpperCode = Regex.Replace(UpperCode, @"\band\b",       @"AND",       RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
            UpperCode = Regex.Replace(UpperCode, @"\bbegin\b",     @"BEGIN",     RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
            UpperCode = Regex.Replace(UpperCode, @"\bbetween\b",   @"BETWEEN",   RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
            UpperCode = Regex.Replace(UpperCode, @"\bboolean\b",   @"BOOLEAN",   RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
            UpperCode = Regex.Replace(UpperCode, @"\bclose\b",     @"CLOSE",     RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
            UpperCode = Regex.Replace(UpperCode, @"\bcommit\b",    @"COMMIT",    RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
            UpperCode = Regex.Replace(UpperCode, @"\bcreate\b",    @"CREATE",    RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
            UpperCode = Regex.Replace(UpperCode, @"\bcursor\b",    @"CURSOR",    RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
            UpperCode = Regex.Replace(UpperCode, @"\bdate\b",      @"DATE",      RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
            UpperCode = Regex.Replace(UpperCode, @"\bdeclare\b",   @"DECLARE",   RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
            UpperCode = Regex.Replace(UpperCode, @"\bdelete\b",    @"DELETE",    RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
            UpperCode = Regex.Replace(UpperCode, @"\bdistinct\b",  @"DISTINCT",  RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
            UpperCode = Regex.Replace(UpperCode, @"\belse\b",      @"ELSE",      RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
            UpperCode = Regex.Replace(UpperCode, @"\belsif\b",     @"ELSIF",     RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
            UpperCode = Regex.Replace(UpperCode, @"\bend if\b",    @"END IF",    RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
            UpperCode = Regex.Replace(UpperCode, @"\bend\b",       @"END",       RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
            UpperCode = Regex.Replace(UpperCode, @"\bend loop\b",  @"END LOOP",  RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
            UpperCode = Regex.Replace(UpperCode, @"\bexception\b", @"EXCEPTION", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
            UpperCode = Regex.Replace(UpperCode, @"\bexit\b",      @"EXIT",      RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
            UpperCode = Regex.Replace(UpperCode, @"\bfetch\b",     @"FETCH",     RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
            UpperCode = Regex.Replace(UpperCode, @"\bfor\b",       @"FOR",       RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
            UpperCode = Regex.Replace(UpperCode, @"\bfrom\b",      @"FROM",      RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
            UpperCode = Regex.Replace(UpperCode, @"\bfunction\b",  @"FUNCTION",  RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
            UpperCode = Regex.Replace(UpperCode, @"\bgroup by\b",  @"GROUP BY",  RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
            UpperCode = Regex.Replace(UpperCode, @"\bif\b",        @"IF",        RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
            UpperCode = Regex.Replace(UpperCode, @"\bin\b",        @"IN",        RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
            UpperCode = Regex.Replace(UpperCode, @"\binsert\b",    @"INSERT",    RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
            UpperCode = Regex.Replace(UpperCode, @"\binto\b",      @"INTO",      RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
            UpperCode = Regex.Replace(UpperCode, @"\bis\b",        @"IS",        RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
            UpperCode = Regex.Replace(UpperCode, @"\bloop\b",      @"LOOP",      RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
            UpperCode = Regex.Replace(UpperCode, @"\bmax\(",       @"MAX(",      RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
            UpperCode = Regex.Replace(UpperCode, @"\bminus\b",     @"MINUS",     RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
            UpperCode = Regex.Replace(UpperCode, @"\bnot\b",       @"NOT",       RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
            UpperCode = Regex.Replace(UpperCode, @"\bnull\b",      @"NULL",      RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
            UpperCode = Regex.Replace(UpperCode, @"\bnumber\b",    @"NUMBER",    RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
            UpperCode = Regex.Replace(UpperCode, @"\bnvl\b",       @"NVL",       RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
            UpperCode = Regex.Replace(UpperCode, @"\bopen\b",      @"OPEN",      RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
            UpperCode = Regex.Replace(UpperCode, @"\bor\b",        @"OR",        RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
            UpperCode = Regex.Replace(UpperCode, @"\border by\b",  @"ORDER BY",  RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
            UpperCode = Regex.Replace(UpperCode, @"\bothers\b",    @"OTHERS",    RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
            UpperCode = Regex.Replace(UpperCode, @"\bout\b",       @"OUT",       RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
            UpperCode = Regex.Replace(UpperCode, @"\bpower\b",     @"POWER",     RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
            UpperCode = Regex.Replace(UpperCode, @"\bprocedure\b", @"PROCEDURE", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
            UpperCode = Regex.Replace(UpperCode, @"\braise\b",     @"RAISE",     RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
            UpperCode = Regex.Replace(UpperCode, @"\breplace\b",   @"REPLACE",   RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
            UpperCode = Regex.Replace(UpperCode, @"\breturn\b",    @"RETURN",    RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
            UpperCode = Regex.Replace(UpperCode, @"\bround\b",     @"ROUND",     RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
            UpperCode = Regex.Replace(UpperCode, @"\bselect\b",    @"SELECT",    RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
            UpperCode = Regex.Replace(UpperCode, @"\bset\b",       @"SET",       RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
            UpperCode = Regex.Replace(UpperCode, @"\bsysdate\b",   @"SYSDATE",   RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
            UpperCode = Regex.Replace(UpperCode, @"\bthen\b",      @"THEN",      RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
            UpperCode = Regex.Replace(UpperCode, @"\bto_char\b",   @"TO_CHAR",   RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
            UpperCode = Regex.Replace(UpperCode, @"\btrunc\b",     @"TRUNC",     RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
            UpperCode = Regex.Replace(UpperCode, @"\bunion",       @"UNION",     RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
            UpperCode = Regex.Replace(UpperCode, @"\bupdate\b",    @"UPDATE",    RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
            UpperCode = Regex.Replace(UpperCode, @"\buser\b",      @"USER",      RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
            UpperCode = Regex.Replace(UpperCode, @"\bvalues\b",    @"VALUES",    RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
            UpperCode = Regex.Replace(UpperCode, @"\bvarchar\b",   @"VARCHAR",   RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
            UpperCode = Regex.Replace(UpperCode, @"\bvarchar2\b",  @"VARCHAR2",  RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
            UpperCode = Regex.Replace(UpperCode, @"\bwhen\b",      @"WHEN",      RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
            UpperCode = Regex.Replace(UpperCode, @"\bwhere\b",     @"WHERE",     RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
            UpperCode = Regex.Replace(UpperCode, @"\bwhile\b",     @"WHILE",     RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
            UpperCode = Regex.Replace(UpperCode, @"\bdefault\b",   @"DEFAULT",   RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
            UpperCode = Regex.Replace(UpperCode, @"\blong\b",      @"LONG",      RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);

            // SEPARATE COMMIT
            UpperCode = Regex.Replace(UpperCode, "\n(?<COMMITWSPACES>\\s+COMMIT;)"
                , "\r\n${COMMITWSPACES}", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);

            // Spaces between assignment
            // Change 
            // X    :=10;
            // 
            // X    := 10;
            UpperCode = Regex.Replace(UpperCode, @"(?<VARIABLE>[a-z0-9_\-]{1})(?<SPACES>[\s]+):=(?<VALUE>[;\(\)\'\|a-z0-9_\-]{1})"
                , "${VARIABLE}${SPACES}:= ${VALUE}", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);

            // Spaces between assignment
            // Change 
            // X:=    10;
            // 
            // X :=     10;
            UpperCode = Regex.Replace(UpperCode, @"(?<VARIABLE>[a-z0-9_\-]{1}):=(?<VALUE>[;\(\)\'\|a-z0-9_\- ]{1})"
                , "${VARIABLE} := ${VALUE}", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);

            // SPLIT ELSE xxx
            // INTO
            //       ELSE
            //            xxx
            UpperCode = Regex.Replace(UpperCode, @"\belse( )+(?<THEREST>.+)"
                , "ELSE\r\n${THEREST}", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);

            // Split
            // INTO TABLEX (xx_xx,yy,zz) VALUES (AA,BB
            // INTO TABLEX
            //      (xx_xx,yy,zz) 
            // VALUES (AA,BB
            UpperCode = Regex.Replace(UpperCode, @"\sINTO\s(?<TABLENAME>[a-z0-9_]+)(?<FIELDS>[\(a-z0-9_\-, \)]+)( )?VALUES\s?(?<VALUES>[\(;\'a-z0-9_,\|\- \)]+)"
                , " INTO ${TABLENAME}\r\n${FIELDS}\r\nVALUES\r\n${VALUES}", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);

            // Split
            // VALUES (AA,BB);
            // VALUES
            //      (AA,BB);
            UpperCode = Regex.Replace(UpperCode, @"VALUES\s?(?<VALUES>[\(;\'a-z0-9_,\|\- \)]+)"
                , "VALUES\r\n${VALUES}", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);

            // SPLIT INSERT INTO
            UpperCode = Regex.Replace(UpperCode, @"\binsert into\b", "INSERT\r\nINTO", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);

            // Split EXCEPTION WHEN
            UpperCode = Regex.Replace(UpperCode, @"\bexception when\b", "EXCEPTION\r\nWHEN", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
            
            // Split 
            //        WHEN XX THEN YYY 
            // INTO
            //        WHEN XX THEN
            //             YYY
            UpperCode = Regex.Replace(UpperCode, @"\bwhen( )+(?<EXCEP>[a-z0-9_]+)( )+then( )+(?<REST>.+)"
                , "WHEN ${EXCEP} THEN\r\n${REST}", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);

            // Split 
            //      INSERT INTO TABLE (
            // INTO
            //      INSERT
            //        INTO TABLE 
            //            (
            UpperCode = Regex.Replace(UpperCode, @"\bINTO( )+(?<TABLENAME>[a-z0-9_]+)( )?(?<PARENTHESIS>\()"
                , "INTO ${TABLENAME}\r\n${PARENTHESIS}", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);

            // Split 
            // SELECT * FROM
            //  
            //  SELECT *
            //   FROM
            UpperCode = Regex.Replace(UpperCode, @"\bSELECT( )+(?<FIELDS>[a-z0-9_\*]+)( )?FROM( )+"
                , "SELECT ${FIELDS}\r\nFROM ", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);

            // Split 
            // FROM TABLE ORDER BY XXX, YY;
            //  
            //  FROM TABLE
            //   ORDER BY XXX, YY;
            UpperCode = Regex.Replace(UpperCode, @"\bFROM\s(?<TABLENAME>[a-z0-9_]+)\sORDER BY\s(?<ORDERFIELDS>[\(;a-z0-9_\-, \)]+)"
                                                , "FROM ${TABLENAME}\r\nORDER BY ${ORDERFIELDS}", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);

            //UpperCode = Regex.Replace(UpperCode, "\n", "\r\n", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);

            IndentedCode = UpperCode;
            Cursor.Current = Cursors.Default;
        }

        /// <summary>
        /// Aqui se hace todo el proceso de indentacion
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void bgwProceso_DoWork(object sender, DoWorkEventArgs e)
        {
            // Read the whole file and process using the stack for indentation

            // DO WHOLE UPPER CASE
            UpperCaseCode();
            //txtOriginal.Text = txtIndented.Text;
            OriginalCode = IndentedCode;

            StringBuilder sbIndented = new StringBuilder();
            string WorkLine = string.Empty;
            string WorkLineWOComments = string.Empty;
            string PreviousWorkLine;
            string DelimiterPending = string.Empty;
            bool ControlStructureFound = false;
            Match MoreInfo;
            int ParenthesisCount = 0;
            int ParenthesisTemp = 0;
            int LineCount = 0;
            bool InsideCommentBlock = false;
            int LinePointerStart = 0;
            int LinePointerEnd = -1;
            int LinesProcessed = 0;
            bool OneParenthesisIsStillOpen = false;
            int OneParenthesisPosition = 0;
            bool OneParenthesisFlag = false; // Since parenthesis indentation works on NEXT line then we use this flag to skip current line
            bool StartVariableDeclareFound = false;


            LineCount = 0;
            
            LinePointerEnd = OriginalCode.IndexOf("\r", LinePointerStart);
            while (LinePointerEnd != -1)
            {
                LineCount++;
                bgwProceso.ReportProgress(LineCount,CurrentIndentation);

                try
                {
                    // Grab line
                    PreviousWorkLine = WorkLine;
                    WorkLine = OriginalCode.Substring(LinePointerStart, (LinePointerEnd - LinePointerStart));
                    LinesProcessed++;
                    // Move pointer
                    LinePointerStart = LinePointerEnd;
                    ControlStructureFound = false;
                    if (StartVariableDeclareFound)
                    {
                        StartVariableDeclareFound = false;
                        sbIndented.Append("\r\n" + "".PadRight(CurrentIndentation) + "-- START VARIABLE DECLARATION 2 ------------\r\n");
                    }

                    // Esto aqui es para hacer pruebas
                    if (WorkLine.Contains("FOR UPDATE"))
                    {
                        WorkLine = WorkLine;
                    }

                    if (WorkLine.Trim() == string.Empty) // Is comment so skip all the rest
                    {
                        goto FinalCode;
                    }
                    if (rgLINECOMMENT.IsMatch(WorkLine)) // Is comment so skip all the rest
                    {
                        goto FinalCode;
                    }
                    if (rgBLKCOMMENTEND.IsMatch(WorkLine))
                    {
                        InsideCommentBlock = false;
                        goto FinalCode;
                    }
                    if (InsideCommentBlock)
                    {
                        goto FinalCode;
                    }
                    if (rgBLKCOMMENTSTA.IsMatch(WorkLine))
                    {
                        InsideCommentBlock = true;
                        goto FinalCode;
                    }

                    // Process Parenthesis counter (add and substract parenthesis. used for Select statements)
                    if (rgBLKCOMMENTSTA2.IsMatch(WorkLine) && !rgBLKCOMMENTSTA.IsMatch(WorkLine))
                    {
                        Match MatchComment = rgBLKCOMMENTSTA2.Match(WorkLine);
                        WorkLineWOComments = WorkLine.Substring(0, MatchComment.Groups[0].Index);
                    }
                    else
                    {
                        WorkLineWOComments = WorkLine;
                    }
                    
                    ParenthesisTemp = WorkLineWOComments.Count(f => f == ')');
                    ParenthesisCount += WorkLineWOComments.Count(f => f == '(') - ParenthesisTemp;

                    // Finish indentation of parameters of called procs or functions
                    // Only do this is parenthesis count = 0 and we are not on a inner proc or function
                    if (!rgPROCFUN.IsMatch(WorkLine) && ParenthesisCount == 0 && OneParenthesisIsStillOpen) // Procedure or function header
                    {
                        // Say we are not waiting on a parenthesis closure
                        OneParenthesisIsStillOpen = false;
                        // Turn flag on for current line skip
                        OneParenthesisFlag = true;
                    }

                    // Check if last on stack is a select that ends with a parenthesis and we just closed all
                    if (stckStatements.Count != 0)
                    {
                        if (stckStatements.Peek().Type == "SL")
                        {
                            if (stckStatements.Peek().SelectEndsWithParenthesis && ParenthesisCount == 0)
                            {
                                // Pop SELECT
                                WorkLine = "".PadRight(CurrentIndentation) + WorkLine.TrimStart();
                                CurrentIndentation -= stckStatements.Peek().Indentation;
                                stckStatements.Pop();
                                ControlStructureFound = true;
                                // Cant go to final code since we might have a ;
                            }
                        }
                    }

                    // Process
                    
                    // Check for this type of assignment
                    // EXCEPTION WHEN E_NOGENERAR THEN  P_NEXITO := 0;
                    // or this one
                    //           WHEN E_NOGENERAR THEN  P_NEXITO := 0;
                    // which qualifies as EXCEPTION not as assigment
                    if (stckStatements.Count != 0)
                    {
                        if (rgASSIGN.IsMatch(WorkLine) && 
                            !rgEXCEPTION.IsMatch(WorkLine) && 
                            !rgWHEN.IsMatch(WorkLine) && 
                            stckStatements.Peek().Type != "PKB" &&
                            stckStatements.Peek().Type != "PF" &&
                            stckStatements.Peek().Type != "BG") // Assignment X := Y;
                        {
                            goto FinalCode;
                        }
                    }

                    if (rgPACKAGE.IsMatch(WorkLine))
                    {
                        // Check if user wants an empty line before
                        if (chkSpaceBeforeProc.Checked)
                        {
                            //Check if prev line was empty
                            if (PreviousWorkLine.Trim() != string.Empty && !PreviousWorkLine.TrimStart().StartsWith("--") && !PreviousWorkLine.TrimStart().StartsWith("CREATE"))
                            {
                                // Add empty line
                                sbIndented.Append("\r\n");
                            }
                        }

                        // Get more information
                        MoreInfo = rgPACKAGE.Match(WorkLine);

                        // Push
                        Statement stmt = new Statement();
                        stmt.Text = WorkLine;
                        //stmt.ProcName = string.Empty; // Procedure name
                        stmt.ProcName = MoreInfo.Groups[3].Value.Trim().Replace(@"""", ""); // Procedure name
                        stmt.HasAName = false;
                        stmt.Type = "PKB";
                        stmt.HasBegin = false;
                        stmt.BeginFound = false;
                        stmt.Indentation = 2;
                        stmt.DelimiterFound = rgIS.IsMatch(WorkLine) ? true : false;
                        stmt.SelectEndsWithParenthesis = false;
                        stckStatements.Push(stmt);
                        WorkLine = "".PadRight(CurrentIndentation) + WorkLine.TrimStart();
                        if (stmt.DelimiterFound)
                        {
                            CurrentIndentation += stmt.Indentation;
                            DelimiterPending = "";
                        }
                        else
                        {
                            DelimiterPending = "IS";
                        }
                        
                        if (DelimiterPending == "" && chkFixVarDeclares.Checked)
                            sbIndented.Append("\r\n" + "".PadRight(CurrentIndentation) + "-- START VARIABLE DECLARATION 1------------\r\n");

                        ControlStructureFound = true;
                        goto FinalCode;
                    }

                    // End Package
                    if (rgENDPACKAGE2.IsMatch(WorkLine))
                    {
                        if (stckStatements.Count == 1)
                        {
                            if (stckStatements.Peek().Type == "PKB" || stckStatements.Peek().Type == "PF") 
                            {
                                // Check if user wants to name this end statement
                                if (chkNameProcEnds.Checked)
                                {
                                    // See if we dont have a name
                                    if (!WorkLine.Contains(stckStatements.Peek().ProcName))
                                    {
                                        // Set the package name as we found it
                                        WorkLine = WorkLine.Replace("END", "END " + stckStatements.Peek().ProcName);
                                    }
                                }
                                // Pop package
                                CurrentIndentation -= stckStatements.Peek().Indentation;
                                //if (!this.DesignMode) if (CurrentIndentation <= 0) CurrentIndentation = 0;
                                WorkLine = "".PadRight(CurrentIndentation) + WorkLine.TrimStart();
                                stckStatements.Pop();
                                ControlStructureFound = true;
                                goto FinalCode;
                            }
                        }
                    }

                    if (rgIS.IsMatch(WorkLine))
                    {
                        if (DelimiterPending != string.Empty)
                        {
                            if (DelimiterPending == "IS" || DelimiterPending == "AS")
                            {
                                DelimiterPending = string.Empty;
                                stckStatements.Peek().DelimiterFound = true;
                                WorkLine = "".PadRight(CurrentIndentation) + WorkLine.TrimStart();

                                CurrentIndentation += stckStatements.Peek().Indentation;

                                if (chkFixVarDeclares.Checked)
                                    StartVariableDeclareFound = true;

                                //ControlStructureFound = true;
                                goto FinalCode;
                            }
                        }
                    }

                    if (rgPROCFUN.IsMatch(WorkLine)) // Procedure or function header
                    {
                        // Check if user wants an empty line before
                        if (chkSpaceBeforeProc.Checked)
                        {
                            //Check if prev line was empty
                            if (PreviousWorkLine.Trim() != string.Empty && !PreviousWorkLine.TrimStart().StartsWith("--"))
                            {
                                // Add empty line
                                sbIndented.Append("\r\n");
                            }
                        }

                        // Get more information
                        MoreInfo = rgPROCFUN.Match(WorkLine);

                        // Push
                        Statement stmt = new Statement();
                        stmt.Text = WorkLine;
                        stmt.ProcName = MoreInfo.Groups[2].Value.Trim(); // Procedure name
                        stmt.HasAName = true;
                        
                        // Indent procedure parameters
                        if (WorkLine.IndexOf("(") != -1)
                        {
                            stmt.ProcedureWordPos = ("".PadRight(CurrentIndentation) + WorkLine.TrimStart()).IndexOf("(") + 1;
                        }
                        else
                        {
                            stmt.ProcedureWordPos = ("".PadRight(CurrentIndentation) + WorkLine.TrimStart()).IndexOf(stmt.ProcName) + stmt.ProcName.Length + 1;
                        }
                        
                        stmt.Type = "PF";
                        stmt.HasBegin = true;
                        stmt.BeginFound = false;
                        stmt.Indentation = 2;
                        stmt.DelimiterFound = rgIS.IsMatch(WorkLine) ? true : false;
                        stmt.SelectEndsWithParenthesis = false;
                        stmt.ExceptionFound = false;
                        stckStatements.Push(stmt);
                        WorkLine = "".PadRight(CurrentIndentation) + WorkLine.TrimStart();

                        if (stmt.DelimiterFound)
                        {
                            CurrentIndentation += stmt.Indentation;
                            DelimiterPending = "";
                        }
                        else
                        {
                            DelimiterPending = "IS";
                        }

                        if (DelimiterPending == "" && chkFixVarDeclares.Checked)
                            sbIndented.Append("\r\n" + "".PadRight(CurrentIndentation) + "-- START VARRIABLE DECLARATION 3 ------------\r\n");

                        ControlStructureFound = true;
                        goto FinalCode;

                    }

                    if (rgDECLARE.IsMatch(WorkLine)) // Declare header
                    {

                        // Push
                        Statement stmt = new Statement();
                        stmt.Text = WorkLine;
                        stmt.ProcName = string.Empty;
                        stmt.HasAName = false;
                        stmt.ProcedureWordPos = 0;
                        stmt.Type = "PF";
                        stmt.HasBegin = true;
                        stmt.BeginFound = false;
                        stmt.Indentation = 2;
                        stmt.DelimiterFound = true;
                        stmt.SelectEndsWithParenthesis = false;
                        stmt.ExceptionFound = false;
                        stckStatements.Push(stmt);
                        WorkLine = "".PadRight(CurrentIndentation) + WorkLine.TrimStart();
                        CurrentIndentation += stmt.Indentation;
                        DelimiterPending = "";
                        ControlStructureFound = true;
                        goto FinalCode;

                    }

                    // A Begin block
                    if (rgBEGIN.IsMatch(WorkLine))
                    {
                        if (stckStatements.Count != 0)
                        {
                            // Check if pushed statement requires a begin and one hasnt been foud yet
                            if (stckStatements.Peek().HasBegin)
                            {
                                if (!stckStatements.Peek().BeginFound)
                                {
                                    if (chkFixVarDeclares.Checked)
                                        sbIndented.Append("".PadRight(CurrentIndentation) + "-- END VARIABLE DECLARATION ------------\r\n");

                                    // Check if user wants to name this begin statement
                                    if (chkNameProcBegins.Checked)
                                    {
                                        // See if we dont have a name on this line
                                        if (!WorkLine.Contains(stckStatements.Peek().ProcName) && !WorkLine.Contains("--"))
                                        {
                                            // Set the procedure name as we found it
                                            WorkLine = WorkLine.Replace("BEGIN", "BEGIN --" + stckStatements.Peek().ProcName);
                                        }
                                    }
                                    // We have here a procedure begin clause so paint without indentation and move on
                                    CurrentIndentation -= stckStatements.Peek().Indentation;
                                    WorkLine = "".PadRight(CurrentIndentation) + WorkLine.TrimStart();
                                    CurrentIndentation += stckStatements.Peek().Indentation;
                                    stckStatements.Peek().BeginFound = true; // Say we found our begin clause
                                    ControlStructureFound = true;
                                    goto FinalCode;
                                }
                                else
                                {
                                    // This is a brand new begin structure
                                    // Push
                                    Statement stmt = new Statement();
                                    stmt.Text = WorkLine;
                                    stmt.ProcName = string.Empty; // Procedure name
                                    stmt.HasAName = false;
                                    stmt.Type = "BG";
                                    stmt.HasBegin = false;
                                    stmt.BeginFound = false;
                                    stmt.Indentation = 2;
                                    stmt.DelimiterFound = true;
                                    stmt.SelectEndsWithParenthesis = false;
                                    stmt.ExceptionFound = false;
                                    stckStatements.Push(stmt);
                                    WorkLine = "".PadRight(CurrentIndentation) + WorkLine.TrimStart();
                                    CurrentIndentation += stmt.Indentation;
                                    ControlStructureFound = true;
                                    goto FinalCode;
                                }
                            }
                            else
                            {
                                // This is a brand new begin structure
                                // Push
                                Statement stmt = new Statement();
                                stmt.Text = WorkLine;
                                stmt.ProcName = string.Empty; // Procedure name
                                stmt.HasAName = false;
                                stmt.Type = "BG";
                                stmt.HasBegin = false;
                                stmt.BeginFound = false;
                                stmt.Indentation = 2;
                                stmt.DelimiterFound = true;
                                stmt.SelectEndsWithParenthesis = false;
                                stmt.ExceptionFound = false;
                                stckStatements.Push(stmt);
                                WorkLine = "".PadRight(CurrentIndentation) + WorkLine.TrimStart();
                                CurrentIndentation += stmt.Indentation;
                                ControlStructureFound = true;
                                goto FinalCode;
                            }
                        }
                    }

                    if (stckStatements.Count != 0)
                    {
                        //Are we inside a begin or program or function block
                        if (stckStatements.Peek().Type == "BG" || stckStatements.Peek().Type == "PF")
                        {
                            //IF exception keyword found
                            if (rgEXCEPTION.IsMatch(WorkLine))
                            {
                                //Substract this item's indentation (BG or PF) since exception needs to be not indented
                                CurrentIndentation -= stckStatements.Peek().Indentation;
                                // Say that our exception section has been found
                                stckStatements.Peek().ExceptionFound = true;
                                // Change 2 space indentation into 7
                                // since exception has 7 spaces like this
                                // EXCEPTION
                                //   WHEN OTHERS THEN
                                // 1234567
                                //        NULL;
                                stckStatements.Peek().Indentation = 7;
                                //if (!this.DesignMode) if (CurrentIndentation <= 0) CurrentIndentation = 0;
                                WorkLine = "".PadRight(CurrentIndentation) + WorkLine.TrimStart();
                                //Add indentation again
                                CurrentIndentation += stckStatements.Peek().Indentation;
                                ControlStructureFound = true;
                                goto FinalCode;
                            }
                        }
                    }

                    if (stckStatements.Count != 0)
                    {
                        //Are we inside a begin or program or function block and we have reached the exception section
                        if ((stckStatements.Peek().Type == "BG" || stckStatements.Peek().Type == "PF") && stckStatements.Peek().ExceptionFound)
                        {
                            // WHEN FOUND 
                            if (rgWHEN.IsMatch(WorkLine))
                            {
                                // Remove partial indentation since its currently 7
                                // EXCEPTION
                                // 1234567
                                //   WHEN
                                CurrentIndentation -= 5;
                                WorkLine = "".PadRight(CurrentIndentation) + WorkLine.TrimStart();
                                CurrentIndentation += 5;
                                ControlStructureFound = true;
                                goto FinalCode;
                            }
                            else
                            {
                                /* Cannot be used since inside a when clause you can continue having control structures
                                //if (PreviousWorkLine.Contains("WHEN ") && !WorkLine.Contains("END"))
                                if (!WorkLine.Contains("END"))
                                {
                                    // its a when subclause so indent as normal since block's indentation should be at 7
                                    // i.e WHEN X THEN
                                    //          DO SOMETHING
                                    WorkLine = "".PadRight(CurrentIndentation) + WorkLine.TrimStart();
                                    ControlStructureFound = true;
                                    goto FinalCode;
                                }
                                */
                            }
                        }
                    }

                    if (rgEND.IsMatch(WorkLine))
                    {
                        if (stckStatements.Count != 0)
                        {
                            // Check if we are ending last pusheded statement as a begin
                            if (stckStatements.Peek().Type == "BG")
                            {
                                // Pop Begin
                                CurrentIndentation -= stckStatements.Peek().Indentation;
                                //if (!this.DesignMode) if (CurrentIndentation <= 0) CurrentIndentation = 0;
                                WorkLine = "".PadRight(CurrentIndentation) + WorkLine.TrimStart();
                                stckStatements.Pop();
                                ControlStructureFound = true;
                                goto FinalCode;
                            }
                        }
                    }

                    if (rgFOR.IsMatch(WorkLine))
                    {
                        // Push
                        Statement stmt = new Statement();
                        stmt.Text = WorkLine;
                        stmt.ProcName = string.Empty; // Procedure name
                        stmt.HasAName = false;
                        stmt.Type = "FR";
                        stmt.HasBegin = false;
                        stmt.BeginFound = false;
                        stmt.Indentation = 4;
                        stmt.DelimiterFound = rgLOOP.IsMatch(WorkLine) ? true : false;
                        stmt.SelectEndsWithParenthesis = false;
                        stckStatements.Push(stmt);
                        WorkLine = "".PadRight(CurrentIndentation) + WorkLine.TrimStart();
                        if (stmt.DelimiterFound)
                        {
                            CurrentIndentation += stmt.Indentation;
                            DelimiterPending = "";
                        }
                        else
                        {
                            DelimiterPending = "LOOP";
                        }
                        ControlStructureFound = true;
                        goto FinalCode;
                    }

                    // Begin loop and not ending loop
                    if (rgLOOP.IsMatch(WorkLine) && !rgENDLOOP.IsMatch(WorkLine)) // Can be a delimiter pending or a starting loop
                    {
                        if (DelimiterPending != string.Empty)
                        {
                            if (DelimiterPending == "LOOP")
                            {
                                DelimiterPending = string.Empty;
                                stckStatements.Peek().DelimiterFound = true;
                                WorkLine = "".PadRight(CurrentIndentation) + WorkLine.TrimStart();

                                CurrentIndentation += stckStatements.Peek().Indentation;
                                ControlStructureFound = true;
                                goto FinalCode;
                            }
                        }
                        else // new loop section. behaves as a for or while
                        {
                            // Push
                            Statement stmt = new Statement();
                            stmt.Text = WorkLine;
                            stmt.ProcName = string.Empty; // Procedure name
                            stmt.HasAName = false;
                            stmt.Type = "FR";
                            stmt.HasBegin = false;
                            stmt.BeginFound = false;
                            stmt.Indentation = 4;
                            stmt.DelimiterFound = true;
                            stmt.SelectEndsWithParenthesis = false;
                            stckStatements.Push(stmt);
                            WorkLine = "".PadRight(CurrentIndentation) + WorkLine.TrimStart();
                            CurrentIndentation += stmt.Indentation;
                            DelimiterPending = "";
                            ControlStructureFound = true;
                            goto FinalCode;
                        }
                    }

                    // Check for procedure closure
                    if (stckStatements.Count != 0)
                    {
                        if (stckStatements.Peek().BeginFound && (stckStatements.Peek().ProcName != string.Empty || !stckStatements.Peek().HasAName))
                        {
                            rgENDPROCFUN = new Regex("END( )?" + stckStatements.Peek().ProcName + ";", RegexOptions.IgnoreCase);
                            if (rgENDPROCFUN.IsMatch(WorkLine))
                            {
                                // Check if user wants to name this end statement
                                if (chkNameProcEnds.Checked)
                                {
                                    // See if we dont have a name
                                    if (!WorkLine.Contains(stckStatements.Peek().ProcName))
                                    {
                                        // Set the package name as we found it
                                        WorkLine = WorkLine.Replace("END", "END " + stckStatements.Peek().ProcName);
                                    }
                                }
                                // Pop Procedure
                                CurrentIndentation -= stckStatements.Peek().Indentation;
                                //if (!this.DesignMode) if (CurrentIndentation <= 0) CurrentIndentation = 0;
                                stckStatements.Pop();
                                ControlStructureFound = true;
                                WorkLine = "".PadRight(CurrentIndentation) + WorkLine.TrimStart();

                                if (chkSpaceBeforeProc.Checked)
                                {
                                    WorkLine += "\r\n";
                                }
                                goto FinalCode;
                            }
                            else
                            {
                                rgENDPROCFUN = new Regex("END( )?;", RegexOptions.IgnoreCase);
                                if (rgENDPROCFUN.IsMatch(WorkLine))
                                {
                                    // Check if user wants to name this end statement
                                    if (chkNameProcEnds.Checked)
                                    {
                                        // See if we dont have a name
                                        if (!WorkLine.Contains(stckStatements.Peek().ProcName))
                                        {
                                            // Set the package name as we found it
                                            WorkLine = WorkLine.Replace("END", "END " + stckStatements.Peek().ProcName);
                                        }
                                    }
                                    // Pop Procedure
                                    CurrentIndentation -= stckStatements.Peek().Indentation;
                                    //if (!this.DesignMode) if (CurrentIndentation <= 0) CurrentIndentation = 0;
                                    stckStatements.Pop();
                                    ControlStructureFound = true;
                                    WorkLine = "".PadRight(CurrentIndentation) + WorkLine.TrimStart();
                                    if (chkSpaceBeforeProc.Checked)
                                    {
                                        WorkLine += "\r\n";
                                    }
                                    goto FinalCode;
                                }
                            }
                        }
                    }

                    if (rgIF.IsMatch(WorkLine))
                    {
                        // Get more information
                        MoreInfo = rgIF.Match(WorkLine);

                        // Push
                        Statement stmt = new Statement();
                        stmt.Text = WorkLine;
                        stmt.ProcName = string.Empty; // Procedure name
                        stmt.HasAName = false;
                        stmt.Type = "IF";
                        stmt.HasBegin = false;
                        stmt.BeginFound = false;
                        stmt.Indentation = 4;
                        stmt.DelimiterFound = rgTHEN.IsMatch(WorkLine) ? true : false;
                        stmt.SelectEndsWithParenthesis = false;
                        stckStatements.Push(stmt);
                        WorkLine = "".PadRight(CurrentIndentation) + WorkLine.TrimStart();
                        // FIX 2 spaces after the IF
                        int IfPos = WorkLine.IndexOf("IF") + 2;
                        while (WorkLine.Substring(IfPos, 1) == " ")
                        {
                            IfPos++;
                        }
                        WorkLine = WorkLine.Substring(0, WorkLine.IndexOf("IF") + 2) + "  " + WorkLine.Substring(IfPos);

                        if (stmt.DelimiterFound)
                        {
                            CurrentIndentation += stmt.Indentation;
                            DelimiterPending = "";
                        }
                        else
                        {
                            DelimiterPending = "THEN";
                        }
                        ControlStructureFound = true;
                        goto FinalCode;
                    }

                    if (rgTHEN.IsMatch(WorkLine))
                    {
                        if (DelimiterPending != string.Empty)
                        {
                            if (DelimiterPending == "THEN")
                            {
                                DelimiterPending = string.Empty;
                                stckStatements.Peek().DelimiterFound = true;
                                WorkLine = "".PadRight(CurrentIndentation) + WorkLine.TrimStart();

                                CurrentIndentation += stckStatements.Peek().Indentation;
                                ControlStructureFound = true;
                                goto FinalCode;
                            }
                        }
                    }

                    if (stckStatements.Count != 0)
                    {
                        if (stckStatements.Peek().Type == "IF")
                        {
                            if (rgELSE.IsMatch(WorkLine))
                            {
                                // We have here a else clause so paint without indentation and move on
                                CurrentIndentation -= stckStatements.Peek().Indentation;
                                WorkLine = "".PadRight(CurrentIndentation) + WorkLine.TrimStart();
                                CurrentIndentation += stckStatements.Peek().Indentation;
                                ControlStructureFound = true;
                                goto FinalCode;
                            }
                        }
                    }

                    if (stckStatements.Count != 0)
                    {
                        if (stckStatements.Peek().Type == "IF")
                        {
                            if (rgELSIF.IsMatch(WorkLine))
                            {
                                // We have here a else clause so paint without indentation and move on
                                CurrentIndentation -= stckStatements.Peek().Indentation;
                                WorkLine = "".PadRight(CurrentIndentation) + WorkLine.TrimStart();
                                CurrentIndentation += stckStatements.Peek().Indentation;
                                ControlStructureFound = true;
                                goto FinalCode;
                            }
                        }
                    }

                    if (rgENDIF.IsMatch(WorkLine))
                    {
                        if (stckStatements.Count != 0)
                        {
                            // Pop package
                            CurrentIndentation -= stckStatements.Peek().Indentation;
                            //if (!this.DesignMode) if (CurrentIndentation <= 0) CurrentIndentation = 0;
                            WorkLine = "".PadRight(CurrentIndentation) + WorkLine.TrimStart();
                            stckStatements.Pop();
                            ControlStructureFound = true;
                            goto FinalCode;
                        }
                    }

                    if (rgENDLOOP.IsMatch(WorkLine))
                    {
                        if (stckStatements.Count != 0)
                        {
                            // Pop package
                            CurrentIndentation -= stckStatements.Peek().Indentation;
                            //if (!this.DesignMode) if (CurrentIndentation <= 0) CurrentIndentation = 0;
                            WorkLine = "".PadRight(CurrentIndentation) + WorkLine.TrimStart();
                            stckStatements.Pop();
                            ControlStructureFound = true;
                            goto FinalCode;
                        }
                    }

                    if (rgUNION.IsMatch(WorkLine))
                    {
                        // Check if prev stack is select then pop
                        if (stckStatements.Count != 0)
                        {
                            if (stckStatements.Peek().Type == "SL")
                            {
                                // Pop Select
                                CurrentIndentation -= stckStatements.Peek().Indentation;
                                //if (!this.DesignMode) if (CurrentIndentation <= 0) CurrentIndentation = 0;
                                WorkLine = "".PadRight(CurrentIndentation) + WorkLine.TrimStart();
                                stckStatements.Pop();
                                ControlStructureFound = true;
                            }
                        }
                    }

                    // Check this situation of an inner select
                    //             AND nplazofinal = (SELECT min(nplazofinal) FROM toconceptostasas WHERE oconcept_vkpcodigo = p_vconcepto);
                    if (rgSELECT.IsMatch(WorkLine))
                    {
                        
                        // Chech if we are an inner select that ends here. So no push needed

                        // If we are a subselect inside a major one and all parenthesis have been closed
                        if (stckStatements.Peek().Type == "SL" && ParenthesisCount == 0)
                        {
                            // Discard list push pop management. Treat as a normal select sub statement (such as where, order by etc)
                        }
                        else
                        {
                            // Check if we are part of a INSERT statement such as
                            // INSERT 
                            //   INTO TABLE
                            //        (x, y, z)
                            // SELECT A, B C FROM D...
                            if (stckStatements.Peek().Type == "IN")
                            {
                                // Nothing to do just continue 
                            }
                            else
                            {
                                // FIX spaces between keyword and the rest if no previous keyword found
                                if (!rgSELECTKWS.IsMatch(WorkLine)  && ParenthesisCount == 0)
                                {
                                    Match match = rgSELECT.Match(WorkLine);
                                    WorkLine = "".PadRight(CurrentIndentation) + match.Groups["KWD"].ToString() + " " + WorkLine.TrimStart().Remove(0, match.Groups["KWD"].ToString().Length).TrimStart();
                                }
                                
                                // Push
                                Statement stmt = new Statement();
                                stmt.Text = WorkLine;
                                stmt.ProcName = string.Empty; // Procedure name
                                stmt.HasAName = false;
                                stmt.Type = "SL";
                                stmt.HasBegin = false;
                                stmt.BeginFound = false;
                                stmt.Indentation = 2;
                                stmt.DelimiterFound = true;
                                // See if we have an open parenthesis and we are in a select
                                if (stckStatements.Count != 0)
                                {
                                    if (stckStatements.Peek().Type == "SL" && ParenthesisCount > 0)
                                    {
                                        // We are a select inside a select
                                        // So say we end with a parenthesis count = 0
                                        stmt.SelectEndsWithParenthesis = true;
                                        // Locate the select word position
                                        stmt.MainKWordPos = (WorkLine.TrimStart()).IndexOf("SELECT");
                                    }
                                    else
                                    {
                                        stmt.SelectEndsWithParenthesis = false;
                                        stmt.MainKWordPos = 0;
                                    }
                                }
                                stckStatements.Push(stmt);

                                WorkLine = "".PadRight(CurrentIndentation) + WorkLine.TrimStart();
                                CurrentIndentation += stmt.Indentation;
                                ControlStructureFound = true;
                                goto FinalCode;
                            }
                        }
                    }

                    // new UPDATE statement
                    if (rgUPDATE.IsMatch(WorkLine)) // THIS IF NEEDS TO BE CHECKED AND WAS COPIED FROM rgSELECT
                    {
                        // Chech if we are an inner select that ends here. So no push needed

                        // If we are a subselect inside a major one and all parenthesis have been closed
                        if (stckStatements.Peek().Type == "UP" && ParenthesisCount == 0)
                        {
                            // Discard list push pop management. Treat as a normal select sub statement (such as where, order by etc)
                        }
                        else
                        {
                            // FIX spaces between keyword and the rest
                            Match match = rgUPDATE.Match(WorkLine);
                            WorkLine = "".PadRight(CurrentIndentation) + match.Groups["KWD"].ToString() + " " + WorkLine.Replace(match.Groups["KWD"].ToString(), " ").TrimStart();

                            // Push
                            Statement stmt = new Statement();
                            stmt.Text = WorkLine;
                            stmt.ProcName = string.Empty; // Procedure name
                            stmt.HasAName = false;
                            stmt.Type = "UP";
                            stmt.HasBegin = false;
                            stmt.BeginFound = false;
                            stmt.Indentation = 2;
                            stmt.DelimiterFound = true;
                            // See if we have an open parenthesis and we are in a select
                            if (stckStatements.Count != 0)
                            {
                                if (stckStatements.Peek().Type == "UP" && ParenthesisCount > 0)
                                {
                                    // We are a select inside a select
                                    // So say we end with a parenthesis count = 0
                                    stmt.SelectEndsWithParenthesis = true;
                                    // Locate the select word position
                                    stmt.MainKWordPos = (WorkLine.TrimStart()).IndexOf("UPDATE");
                                }
                                else
                                {
                                    stmt.SelectEndsWithParenthesis = false;
                                    stmt.MainKWordPos = 0;
                                }
                            }
                            stckStatements.Push(stmt);

                            WorkLine = "".PadRight(CurrentIndentation) + WorkLine.TrimStart();
                            CurrentIndentation += stmt.Indentation;
                            ControlStructureFound = true;
                            goto FinalCode;
                        }
                    }

                    // new DELETE statement
                    if (rgDELETE.IsMatch(WorkLine)) // THIS IF NEEDS TO BE CHECKED AND WAS COPIED FROM rgSELECT
                    {
                        // Chech if we are an inner select that ends here. So no push needed

                        // If we are a subselect inside a major one and all parenthesis have been closed
                        if (stckStatements.Peek().Type == "DL" && ParenthesisCount == 0)
                        {
                            // Discard list push pop management. Treat as a normal select sub statement (such as where, order by etc)
                        }
                        else
                        {
                            // FIX spaces between keyword and the rest
                            Match match = rgDELETE.Match(WorkLine);
                            WorkLine = "".PadRight(CurrentIndentation) + match.Groups["KWD"].ToString() + " " + WorkLine.Replace(match.Groups["KWD"].ToString(), " ").TrimStart();

                            // Push
                            Statement stmt = new Statement();
                            stmt.Text = WorkLine;
                            stmt.ProcName = string.Empty; // Procedure name
                            stmt.HasAName = false;
                            stmt.Type = "DL";
                            stmt.HasBegin = false;
                            stmt.BeginFound = false;
                            stmt.Indentation = 2;
                            stmt.DelimiterFound = true;
                            // See if we have an open parenthesis and we are in a select
                            if (stckStatements.Count != 0)
                            {
                                if (stckStatements.Peek().Type == "DL" && ParenthesisCount > 0)
                                {
                                    // We are a select inside a select
                                    // So say we end with a parenthesis count = 0
                                    stmt.SelectEndsWithParenthesis = true;
                                    // Locate the select word position
                                    stmt.MainKWordPos = (WorkLine.TrimStart()).IndexOf("DELETE");
                                }
                                else
                                {
                                    stmt.SelectEndsWithParenthesis = false;
                                    stmt.MainKWordPos = 0;
                                }
                            }
                            stckStatements.Push(stmt);

                            WorkLine = "".PadRight(CurrentIndentation) + WorkLine.TrimStart();
                            CurrentIndentation += stmt.Indentation;
                            ControlStructureFound = true;
                            goto FinalCode;
                        }
                    }

                    // Look for ; on the SELECT statement
                    if (rgENDSELECT.IsMatch(WorkLine))
                    {
                        if (stckStatements.Count != 0)
                        {
                            // Check if we are ending a select statement
                            if (stckStatements.Peek().Type == "SL")
                            {
                                // if select kw then indent less else indent 7 = "SELECT "
                                if (rgSELECTKWS.IsMatch(WorkLine))
                                {
                                    Match match = rgSELECTKWS.Match(WorkLine);

                                    // FIX spaces between keyword and the rest
                                    WorkLine = "".PadRight(CurrentIndentation) + match.Groups["KWD"].ToString() + " " + WorkLine.TrimStart().Remove(0, match.Groups["KWD"].ToString().Length).TrimStart();

                                    // ie. SELECT 1 
                                    //       FROM DUAL;
                                    int FirstWordLength = WorkLine.TrimStart().Substring(0, WorkLine.TrimStart().IndexOf(" ")).Length;
                                    WorkLine = "".PadRight(CurrentIndentation - stckStatements.Peek().Indentation + "SELECT".Length - FirstWordLength) + WorkLine.TrimStart().Replace(match.Groups["KWD"].ToString() + "  ", match.Groups["KWD"].ToString() + " ");
                                    // Also remove double spaces after keyword 
                                    // i.e AND  XX
                                    // to  AND XX
                                }
                                else
                                {
                                    // ie. SELECT 1 FROM
                                    //            DUAL;
                                    WorkLine = "".PadRight(CurrentIndentation - stckStatements.Peek().Indentation + "SELECT ".Length) + WorkLine.TrimStart();
                                }
                                CurrentIndentation -= stckStatements.Peek().Indentation;
                                //if (!this.DesignMode) if (CurrentIndentation <= 0) CurrentIndentation = 0;
                                // Pop Select
                                stckStatements.Pop();
                                ControlStructureFound = true;
                                goto FinalCode;
                            }
                        }
                    }

                    // Look for ; on the INSERT statement
                    if (rgENDINSERT.IsMatch(WorkLine))
                    {
                        if (stckStatements.Count != 0)
                        {
                            // Check if we are ending a INSERT statement
                            if (stckStatements.Peek().Type == "IN")
                            {
                                // if VALUES kw then indent less else indent 7 = "INSERT "
                                if (rgINSERTKWS.IsMatch(WorkLine))
                                {
                                    // Fix spaces in between keyword to just 1 space
                                    Match match = rgINSERTKWS.Match(WorkLine);
                                    WorkLine = "".PadRight(CurrentIndentation) + match.Groups["KWD"].ToString() + " " + WorkLine.TrimStart().Remove(0, match.Groups["KWD"].ToString().Length).TrimStart();

                                    WorkLine = "".PadRight(CurrentIndentation - "INSERT".Length) + WorkLine.TrimStart();
                                }
                                else
                                {
                                    WorkLine = "".PadRight(CurrentIndentation) + WorkLine.TrimStart();
                                }

                                CurrentIndentation -= stckStatements.Peek().Indentation;
                                //if (!this.DesignMode) if (CurrentIndentation <= 0) CurrentIndentation = 0;
                                // Pop Select
                                stckStatements.Pop();
                                ControlStructureFound = true;
                                goto FinalCode;
                            }
                        }
                    }

                    // Look for VALUES KWd on the INSERT statement
                    if (stckStatements.Count != 0)
                    {
                        // Check if we are ending a INSERT statement
                        if (stckStatements.Peek().Type == "IN")
                        {
                            // if VALUES kw then indent less else indent 7 = "INSERT "
                            if (rgINSERTKWS.IsMatch(WorkLine))
                            {
                                // Fix spaces in between keyword to just 1 space
                                Match match = rgINSERTKWS.Match(WorkLine);
                                WorkLine = "".PadRight(CurrentIndentation) + match.Groups["KWD"].ToString() + " " + WorkLine.TrimStart().Remove(0, match.Groups["KWD"].ToString().Length).TrimStart();

                                WorkLine = "".PadRight(CurrentIndentation - "INSERT".Length) + WorkLine.TrimStart();
                                ControlStructureFound = true;
                                goto FinalCode;
                            }
                        }
                    }

                    // Look for ; on the UPDATE statement
                    if (rgENDUPDATE.IsMatch(WorkLine)) //this needs to be tested
                    {
                        if (stckStatements.Count != 0)
                        {
                            // Check if we are ending a UPDATE statement
                            if (stckStatements.Peek().Type == "UP")
                            {
                                // if UPDATE kw then indent less else indent 7 = "UPDATE "
                                if (rgUPDATEKWS.IsMatch(WorkLine))
                                {
                                    Match match = rgUPDATEKWS.Match(WorkLine);
                                    // Fix spaces in between keyword to just 1 space
                                    WorkLine = "".PadRight(CurrentIndentation) + match.Groups["KWD"].ToString() + " " + WorkLine.TrimStart().Remove(0, match.Groups["KWD"].ToString().Length).TrimStart();

                                    // ie. UPDATE X 
                                    //        SET Y;
                                    int FirstWordLength = WorkLine.TrimStart().Substring(0, WorkLine.TrimStart().IndexOf(" ")).Length;
                                    WorkLine = "".PadRight(CurrentIndentation - stckStatements.Peek().Indentation + "UPDATE".Length - FirstWordLength) + WorkLine.TrimStart().Replace(match.Groups["KWD"].ToString() + "  ", match.Groups["KWD"].ToString() + " ");
                                    // Also remove double spaces after keyword 
                                    // i.e AND  XX
                                    // to  AND XX
                                }
                                else
                                {
                                    // ie. UPDATE X SET Y = 
                                    //            1;
                                    WorkLine = "".PadRight(CurrentIndentation - stckStatements.Peek().Indentation + "UPDATE ".Length) + WorkLine.TrimStart();
                                }
                                CurrentIndentation -= stckStatements.Peek().Indentation;
                                //if (!this.DesignMode) if (CurrentIndentation <= 0) CurrentIndentation = 0;
                                // Pop Select
                                stckStatements.Pop();
                                ControlStructureFound = true;
                                goto FinalCode;
                            }
                        }
                    }

                    // Look for ; on the DELETE statement
                    if (rgENDDELETE.IsMatch(WorkLine)) //this needs to be tested
                    {
                        if (stckStatements.Count != 0)
                        {
                            // Check if we are ending a UPDATE statement
                            if (stckStatements.Peek().Type == "DL")
                            {
                                // if DELETE kw then indent less else indent 7 = "DELETE "
                                if (rgUPDATEKWS.IsMatch(WorkLine))
                                {
                                    Match match = rgDELETEKWS.Match(WorkLine);
                                    // ie. UPDATE X 
                                    //        SET Y;
                                    int FirstWordLength = WorkLine.TrimStart().Substring(0, WorkLine.TrimStart().IndexOf(" ")).Length;
                                    WorkLine = "".PadRight(CurrentIndentation - stckStatements.Peek().Indentation + "DELETE".Length - FirstWordLength) + WorkLine.TrimStart().Replace(match.Groups["KWD"].ToString() + "  ", match.Groups["KWD"].ToString() + " ");
                                    // Also remove double spaces after keyword 
                                    // i.e AND  XX
                                    // to  AND XX
                                }
                                else
                                {
                                    // ie. DELETE X WHERE Y = 
                                    //            1;
                                    WorkLine = "".PadRight(CurrentIndentation - stckStatements.Peek().Indentation + "DELETE ".Length) + WorkLine.TrimStart();
                                }
                                CurrentIndentation -= stckStatements.Peek().Indentation;
                                //if (!this.DesignMode) if (CurrentIndentation <= 0) CurrentIndentation = 0;
                                // Pop Select
                                stckStatements.Pop();
                                ControlStructureFound = true;
                                goto FinalCode;
                            }
                        }
                    }

                    // Indent parameters of called procs or functions
                    // Only do this is parenthesis count = 1 and we are not on a inner proc or function
                    // This is done at the end so CurrentIndentation is already set
                    if (!rgPROCFUN.IsMatch(WorkLine) &&
                        !rgSELECT.IsMatch(WorkLine) && 
                        ParenthesisCount == 1 && 
                        !OneParenthesisIsStillOpen && 
                        //stckStatements.Peek().Type != "PF" &&
                        stckStatements.Peek().Type != "SL") // Procedure or function header
                    {
                        // Determine parenthesis position and save it. Use it later for indentation whilst parenthesis count = 0
                        OneParenthesisPosition = WorkLine.TrimStart().LastIndexOf("(") + CurrentIndentation + 1;
                        // Say we are waiting on a parenthesis closure
                        OneParenthesisIsStillOpen = true;
                        // Turn flag on to skip current line
                        OneParenthesisFlag = true;
                    }

                    
                    // If we have an INTO TABLE
                    if (rgINTOTABLE.IsMatch(WorkLine))
                    { 
                        // If previous line is an insert
                        if (PreviousWorkLine.Trim() == "INSERT")
                        {
                            // Add 2 spaces
                            WorkLine = "".PadRight(CurrentIndentation) + "  " + WorkLine.TrimStart();
                            // Add to stack
                            // Push
                            Statement stmt = new Statement();
                            stmt.Text = WorkLine;
                            stmt.ProcName = string.Empty; // Procedure name
                            stmt.HasAName = false;
                            stmt.Type = "IN";
                            stmt.HasBegin = false;
                            stmt.BeginFound = false;
                            stmt.Indentation = 6;
                            stmt.DelimiterFound = true;
                            stckStatements.Push(stmt);

                            CurrentIndentation += stmt.Indentation;
                            ControlStructureFound = true;
                            goto FinalCode;
                        }
                    }

                FinalCode:

                    // Comentario al final de una linea pero no iniciando ya que eso se evaluo anteriormente
                    // ej x:= 10; /* wever 
                    if (rgBLKCOMMENTSTA2.IsMatch(WorkLine) && !rgBLKCOMMENTSTA.IsMatch(WorkLine))
                    {
                        InsideCommentBlock = true;
                    }

                    if (!ControlStructureFound)
                    {
                        if (chkKeepGreaterIndent.Checked)
                        {
                            // Check if indentation is < then
                            if (WorkLine.Length - WorkLine.TrimStart().Length < CurrentIndentation)
                            {
                                // If one parenthesis open then only do that padding
                                if (OneParenthesisIsStillOpen)
                                {
                                    // This flag is used to skip current line
                                    if (OneParenthesisFlag)
                                    {
                                        WorkLine = "".PadRight(CurrentIndentation) + WorkLine.TrimStart();
                                        OneParenthesisFlag = false;
                                    }
                                    else
                                    {
                                        WorkLine = "".PadRight(OneParenthesisPosition) + WorkLine.TrimStart();
                                    }
                                    
                                }
                                else
                                {
                                    // This flag is used here to indent one last time
                                    if (OneParenthesisFlag)
                                    {
                                        WorkLine = "".PadRight(OneParenthesisPosition) + WorkLine.TrimStart();
                                        // Clear parenthesis position 
                                        OneParenthesisPosition = 0;
                                        OneParenthesisFlag = false;
                                    }
                                    else
                                    {
                                        WorkLine = "".PadRight(CurrentIndentation) + WorkLine.TrimStart();
                                    }
                                }
                            }
                        }
                        else
                        {
                            // If one parenthesis open then only do that padding
                            if (OneParenthesisIsStillOpen)
                            {
                                // This flag is used to skip current line
                                if (OneParenthesisFlag)
                                {
                                    WorkLine = "".PadRight(CurrentIndentation) + WorkLine.TrimStart();
                                    OneParenthesisFlag = false;
                                }
                                else
                                {
                                    WorkLine = "".PadRight(OneParenthesisPosition) + WorkLine.TrimStart();
                                }
                                
                            }
                            else
                            {
                                // This flag is used here to indent one last time
                                if (OneParenthesisFlag)
                                {
                                    WorkLine = "".PadRight(OneParenthesisPosition) + WorkLine.TrimStart();
                                    // Clear parenthesis position 
                                    OneParenthesisPosition = 0;
                                    OneParenthesisFlag = false;
                                }
                                else
                                {
                                    WorkLine = "".PadRight(CurrentIndentation) + WorkLine.TrimStart();
                                }
                            }
                            
                        }

                        // Check for SELECT indentation that behaves differently
                        if (stckStatements.Count != 0 && WorkLine.Trim().Length != 0)
                        {
                            if (stckStatements.Peek().Type == "SL")
                            {
                                if (!rgSELECT.IsMatch(WorkLine))
                                {
                                    int AdditionalIndent = stckStatements.Peek().MainKWordPos;

                                    if (rgSELECTKWS.IsMatch(WorkLine))
                                    {
                                        Match match = rgSELECTKWS.Match(WorkLine);

                                        // FIX spaces between keyword and the rest
                                        // was something like "   AND  x = Y AND Z=W
                                        // To
                                        //                    "AND x = Y AND Z=W
                                        WorkLine = "".PadRight(CurrentIndentation) + match.Groups["KWD"].ToString() + " " + WorkLine.TrimStart().Remove(0, match.Groups["KWD"].ToString().Length).TrimStart();

                                        // indent line under select based on first word length
                                        // get first word length
                                        string TrimmedLine = WorkLine.TrimStart();
                                        int FirstWordLength = TrimmedLine.Substring(0, TrimmedLine.IndexOf(" ")).Length;
                                        WorkLine = "".PadRight(CurrentIndentation - stckStatements.Peek().Indentation) + 
                                                   "".PadRight("SELECT".Length - FirstWordLength) +
                                                   "".PadRight(AdditionalIndent) + WorkLine.TrimStart().Replace(match.Groups["KWD"].ToString() + "  ", match.Groups["KWD"].ToString() + " "); 
                                                   // Also remove double spaces after keyword 
                                                   // i.e AND  XX
                                                   // to  AND XX
                                    }
                                    else
                                    {
                                        // Check if we have a comma here
                                        if (WorkLine.TrimStart().Substring(0, 2) == ", ")
                                        {
                                            // SELECT X
                                            //      , Y
                                            WorkLine = "".PadRight(CurrentIndentation - stckStatements.Peek().Indentation + "SELECT ".Length - 2) +
                                                   WorkLine.TrimStart();
                                        }
                                        else
                                        {
                                            if (WorkLine.TrimStart().Substring(0, 1) == ",")
                                            {
                                                // SELECT X
                                                //       ,Y
                                                WorkLine = "".PadRight(CurrentIndentation - stckStatements.Peek().Indentation + "SELECT ".Length - 1) +
                                                       WorkLine.TrimStart();
                                            }
                                            else
                                            {
                                                // SELECT X,
                                                //        Y
                                                WorkLine = "".PadRight(CurrentIndentation - stckStatements.Peek().Indentation + "SELECT ".Length) +
                                                   WorkLine.TrimStart();
                                            }
                                        }
                                        
                                    }
                                }
                            }

                            // UPDATE
                            if (stckStatements.Peek().Type == "UP") // This needs to be checked
                            {
                                if (!rgUPDATE.IsMatch(WorkLine))
                                {
                                    int AdditionalIndent = stckStatements.Peek().MainKWordPos;

                                    if (rgUPDATEKWS.IsMatch(WorkLine))
                                    {
                                        Match match = rgUPDATEKWS.Match(WorkLine);
                                        
                                        // FIX spaces between keyword and the rest
                                        // was something like "   AND  x = Y AND Z=W
                                        // To
                                        //                    "AND x = Y AND Z=W
                                        WorkLine = "".PadRight(CurrentIndentation) + match.Groups["KWD"].ToString() + " " + WorkLine.TrimStart().Remove(0, match.Groups["KWD"].ToString().Length).TrimStart();

                                        // indent line under select based on first word length
                                        // get first word length
                                        string TrimmedLine = WorkLine.TrimStart();
                                        int FirstWordLength = TrimmedLine.Substring(0, TrimmedLine.IndexOf(" ")).Length;
                                        WorkLine = "".PadRight(CurrentIndentation - stckStatements.Peek().Indentation) +
                                                   "".PadRight("UPDATE".Length - FirstWordLength) +
                                                   "".PadRight(AdditionalIndent) + WorkLine.TrimStart().Replace(match.Groups["KWD"].ToString() + "  ", match.Groups["KWD"].ToString() + " ");
                                        // Also remove double spaces after keyword 
                                        // i.e AND  XX
                                        // to  AND XX
                                    }
                                    else
                                    {
                                        // Check if we have a comma here
                                        if (WorkLine.TrimStart().Substring(0, 2) == ", ")
                                        {
                                            // UPDATE X
                                            //    SET A = B
                                            //      , Y = C
                                            WorkLine = "".PadRight(CurrentIndentation - stckStatements.Peek().Indentation + "UPDATE ".Length - 2) +
                                                   WorkLine.TrimStart();
                                        }
                                        else
                                        {
                                            if (WorkLine.TrimStart().Substring(0, 1) == ",")
                                            {
                                                // UPDATE X
                                                //    SET A = B
                                                //       ,Y = C
                                                WorkLine = "".PadRight(CurrentIndentation - stckStatements.Peek().Indentation + "UPDATE ".Length - 1) +
                                                       WorkLine.TrimStart();
                                            }
                                            else
                                            {
                                                // UPDATE X
                                                //    SET A = B,
                                                //        Y = C
                                                WorkLine = "".PadRight(CurrentIndentation - stckStatements.Peek().Indentation + "UPDATE ".Length) +
                                                   WorkLine.TrimStart();
                                            }
                                        }

                                    }
                                }
                            }

                            // DELETE
                            if (stckStatements.Peek().Type == "DL") // This needs to be checked
                            {
                                if (!rgDELETE.IsMatch(WorkLine))
                                {
                                    int AdditionalIndent = stckStatements.Peek().MainKWordPos;

                                    if (rgDELETEKWS.IsMatch(WorkLine))
                                    {
                                        Match match = rgDELETEKWS.Match(WorkLine);
                                        
                                        // FIX spaces between keyword and the rest
                                        // was something like "   AND  x = Y AND Z=W
                                        // To
                                        //                    "AND x = Y AND Z=W
                                        WorkLine = "".PadRight(CurrentIndentation) + match.Groups["KWD"].ToString() + " " + WorkLine.TrimStart().Remove(0, match.Groups["KWD"].ToString().Length).TrimStart();

                                        // indent line under select based on first word length
                                        // get first word length
                                        string TrimmedLine = WorkLine.TrimStart();
                                        int FirstWordLength = TrimmedLine.Substring(0, TrimmedLine.IndexOf(" ")).Length;
                                        WorkLine = "".PadRight(CurrentIndentation - stckStatements.Peek().Indentation) +
                                                   "".PadRight("DELETE".Length - FirstWordLength) +
                                                   "".PadRight(AdditionalIndent) + WorkLine.TrimStart().Replace(match.Groups["KWD"].ToString() + "  ", match.Groups["KWD"].ToString() + " ");
                                        // Also remove double spaces after keyword 
                                        // i.e AND  XX
                                        // to  AND XX
                                    }
                                    else
                                    {
                                        // Check if we have a comma here
                                        if (WorkLine.TrimStart().Substring(0, 2) == ", ")
                                        {
                                            // DELETE X
                                            //    SET A = B
                                            //      , Y = C
                                            WorkLine = "".PadRight(CurrentIndentation - stckStatements.Peek().Indentation + "DELETE ".Length - 2) +
                                                   WorkLine.TrimStart();
                                        }
                                        else
                                        {
                                            if (WorkLine.TrimStart().Substring(0, 1) == ",")
                                            {
                                                // DELETE X
                                                //    SET A = B
                                                //       ,Y = C
                                                WorkLine = "".PadRight(CurrentIndentation - stckStatements.Peek().Indentation + "DELETE ".Length - 1) +
                                                       WorkLine.TrimStart();
                                            }
                                            else
                                            {
                                                // DELETE X
                                                //    SET A = B,
                                                //        Y = C
                                                WorkLine = "".PadRight(CurrentIndentation - stckStatements.Peek().Indentation + "DELETE ".Length) +
                                                   WorkLine.TrimStart();
                                            }
                                        }

                                    }
                                }
                            }

                            // Check for Procedure parameter indentation
                            if (stckStatements.Peek().Type == "PF")
                            {
                                if ((!stckStatements.Peek().DelimiterFound) ||
                                    (stckStatements.Peek().DelimiterFound && rgIS.IsMatch(WorkLine) && !rgCURSOR.IsMatch(WorkLine) && !rgTYPE.IsMatch(WorkLine)))
                                {
                                    int StripComma = 0;
                                    if (WorkLine.TrimStart().StartsWith(","))
                                    {
                                        StripComma = -1;
                                    }
                                    if (!rgPROCFUN.IsMatch(WorkLine))
                                    {
                                        WorkLine = "".PadRight(stckStatements.Peek().ProcedureWordPos + StripComma) + WorkLine.TrimStart();
                                    }
                                }
                            }

                        }
                    }

                    // Add to final output buffer
                    sbIndented.Append(WorkLine.TrimEnd() + "\n");

                    // Seach next line
                    if (LinePointerStart != OriginalCode.Length)
                    {
                        LinePointerEnd = OriginalCode.IndexOf("\r", LinePointerStart + 1);
                    }
                    else
                    {
                        break;
                    }
                    if (LinePointerEnd == -1)
                    {
                        LinePointerEnd = OriginalCode.Length;
                    }
                    if (LinePointerEnd - 1 == LinePointerStart)
                    {
                        LinePointerStart += 1;
                    }
                    else
                    {
                        LinePointerStart += 2;
                    }
                }
                catch (Exception ex)
                {
                    // Something happened so cancel process and output upto here
                    LinePointerEnd = -1;
                    sbIndented.Append("\r\n------------ ERROR AQUI ---------\r\n");
                    sbIndented.Append("Linea con problemas: " + WorkLine + "\r\n");
                    sbIndented.Append(ex.ToString());
                    MessageBox.Show("Ocurrio un error procesando la linea " + LineCount);
                }
            }
            
            bgwProceso.ReportProgress(LineCount, CurrentIndentation);

            IndentedCode = sbIndented.ToString();
            IndentedCode = Regex.Replace(IndentedCode, "\n", "\r\n", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);

            sbIndented.Clear();

            // Check if additional processing is needed
            if (chkFixVarDeclares.Checked)
            {
                // User IndentedCode 
                FixVariableTypeAlignment();
            }
        }

        private void bgwProceso_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            tsbIndent.Enabled = true;
            tsbUpper.Enabled = true;

            txtOriginal.Enabled = true;
            txtIndented.Text = IndentedCode;
            txtIndented.Enabled = true;
            tpgOriginal.Focus();
            System.Windows.Forms.SendKeys.Send("+{TAB}");
            System.Windows.Forms.SendKeys.Send("{RIGHT}");
            pgbProgreso.Value = pgbProgreso.Maximum;
        }

        private void bgwProceso_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            if (e.ProgressPercentage <= pgbProgreso.Maximum)
            {
                pgbProgreso.Value = e.ProgressPercentage;
            }
            if (int.Parse(e.UserState.ToString()) <= pgbIndent.Maximum)
            {
                pgbIndent.Value = int.Parse(e.UserState.ToString());
            }
        }


        /// <summary>
        /// Cambio la seleccion para poder saber en que linea esta
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtOriginal_SelectionChanged(object sender, EventArgs e)
        {
            int intChar;

            //intChar = txtOriginal..GetCharIndexFromPosition(new System.Drawing.Point(0, 0));
            //lblLineNumber.Text = txtOriginal..GetLineFromCharIndex(intChar).ToString();
        }

        private void txtOriginal_SelectionChanged_1(object sender, EventArgs e)
        {
            int intChar;

            //intChar = txtOriginal..GetCharIndexFromPosition(new System.Drawing.Point(0, 0));
            //lblLineNumber.Text = txtOriginal..GetLineFromCharIndex(intChar).ToString();
        }

        private void txtIndented_KeyDown(object sender, KeyEventArgs e)
        {
            int SelectionStart = 0;
            int SelectionLength = 0;

            if (e.KeyCode.ToString() == "A"  && e.Control)
            {
                txtIndentedOld.SelectAll();
            }


            if (e.KeyCode.ToString() == "U" && e.Control)
            {
                if (txtIndentedOld.SelectionLength != 0)
                {
                    // Coger parametros de seleccion
                    SelectionStart = txtIndentedOld.SelectionStart;
                    SelectionLength = txtIndentedOld.SelectionLength;

                    // Ver si estamos upper para servir como switch
                    if (txtIndentedOld.SelectedText == txtIndentedOld.SelectedText.ToUpper())
                    {
                        txtIndentedOld.Text = txtIndentedOld.Text.Substring(0, txtIndentedOld.SelectionStart) + txtIndentedOld.SelectedText.ToLower() + txtIndentedOld.Text.Substring(txtIndentedOld.SelectionStart + txtIndentedOld.SelectionLength);
                    }
                    else
                    {
                        txtIndentedOld.Text = txtIndentedOld.Text.Substring(0, txtIndentedOld.SelectionStart) + txtIndentedOld.SelectedText.ToUpper() + txtIndentedOld.Text.Substring(txtIndentedOld.SelectionStart + txtIndentedOld.SelectionLength);
                    }

                    // Restablecer la seleccion
                    txtIndentedOld.SelectionStart = SelectionStart;
                    txtIndentedOld.SelectionLength = SelectionLength;
                    txtIndentedOld.ScrollToCaret();

                }
            }

        }

        private void txtOriginal_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.ToString() == "A" && e.Control)
            {
                //txtOriginal.SelectAll();
            }
        }

        private void tsbLineTerminator_Click(object sender, EventArgs e)
        {
            txtIndented.EndOfLine.IsVisible = tsbLineTerminator.Checked;
            txtOriginal.EndOfLine.IsVisible = tsbLineTerminator.Checked;
        }

        private void tsbLineNumber_Click(object sender, EventArgs e)
        {
            txtIndented.Margins[0].Width = tsbLineNumber.Checked ? 40 : 0;
            txtOriginal.Margins[0].Width = tsbLineNumber.Checked ? 40 : 0;
        }

        private void tsbSpaces_Click(object sender, EventArgs e)
        {
            txtIndented.Whitespace.Mode = tsbSpaces.Checked ? ScintillaNET.WhitespaceMode.VisibleAlways : ScintillaNET.WhitespaceMode.Invisible;
            txtOriginal.Whitespace.Mode = tsbSpaces.Checked ? ScintillaNET.WhitespaceMode.VisibleAlways : ScintillaNET.WhitespaceMode.Invisible;
        }

        private void tabFiles_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (tabFiles.SelectedTab == tpgIndented)
            {
                txtIndented.Select();
                txtIndented.Focus();
            }
            else
            {
                txtOriginal.Select();
                txtOriginal.Focus();
            }
        }

        /// <summary>
        /// Fix variable type alignment such as
        ///  vCODIGO  VARCHAR2(10);
        ///  vNOMBRE    VARCHAR2(20);
        ///  INTO
        ///  vCODIGO  VARCHAR2(10);
        ///  vNOMBRE  VARCHAR2(20);
        /// </summary>
        private void FixVariableTypeAlignment()
        {
            // Read the whole file and process using the stack for indentation

            //txtOriginal.Text = txtIndented.Text;
            OriginalCode = IndentedCode;

            StringBuilder sbIndented = new StringBuilder();
            string WorkLineFix = string.Empty;
            string PreviousWorkLine;
            string DelimiterPending = string.Empty;
            bool ControlStructureFound = false;
            Match MoreInfo;
            int ParenthesisCount = 0;
            int ParenthesisTemp = 0;
            int LineCount = 0;
            bool InsideCommentBlock = false;
            int LinePointerStart = 0;
            int LinePointerEnd = -1;
            int LinesProcessed = 0;
            bool OneParenthesisIsStillOpen = false;
            int OneParenthesisPosition = 0;
            bool OneParenthesisFlag = false; // Since parenthesis indentation works on NEXT line then we use this flag to skip current line

            LineCount = 0;

            LinePointerEnd = OriginalCode.IndexOf("\r", LinePointerStart);
            while (LinePointerEnd != -1)
            {
                LineCount++;
                bgwProceso.ReportProgress(LineCount, CurrentIndentation);

                try
                {
                    // Grab line
                    PreviousWorkLine = WorkLineFix;
                    WorkLineFix = OriginalCode.Substring(LinePointerStart, (LinePointerEnd - LinePointerStart));
                    LinesProcessed++;
                    // Move pointer
                    LinePointerStart = LinePointerEnd;
                    ControlStructureFound = false;


                    // Esto aqui es para hacer pruebas
                    if (WorkLineFix.Contains("p_nexito := 0;"))
                    {
                        WorkLineFix = WorkLineFix;
                    }

                    if (WorkLineFix.Trim() == string.Empty) // Is comment so skip all the rest
                    {
                        goto FinalCode;
                    }
                    if (rgLINECOMMENT.IsMatch(WorkLineFix)) // Is comment so skip all the rest
                    {
                        goto FinalCode;
                    }
                    if (rgBLKCOMMENTEND.IsMatch(WorkLineFix))
                    {
                        InsideCommentBlock = false;
                        goto FinalCode;
                    }
                    if (InsideCommentBlock)
                    {
                        goto FinalCode;
                    }
                    if (rgBLKCOMMENTSTA.IsMatch(WorkLineFix))
                    {
                        InsideCommentBlock = true;
                        goto FinalCode;
                    }

                    // Process Parenthesis counter (add and substract parenthesis. used for Select statements)
                    ParenthesisTemp = WorkLineFix.Count(f => f == ')');
                    ParenthesisCount += WorkLineFix.Count(f => f == '(') - ParenthesisTemp;

                    // Finish indentation of parameters of called procs or functions
                    // Only do this is parenthesis count = 0 and we are not on a inner proc or function
                    if (!rgPROCFUN.IsMatch(WorkLineFix) && ParenthesisCount == 0 && OneParenthesisIsStillOpen) // Procedure or function header
                    {
                        // Say we are not waiting on a parenthesis closure
                        OneParenthesisIsStillOpen = false;
                        // Turn flag on for current line skip
                        OneParenthesisFlag = true;
                    }

                    // Check if last on stack is a select that ends with a parenthesis and we just closed all
                    if (stckStatements.Count != 0)
                    {
                        if (stckStatements.Peek().Type == "SL")
                        {
                            if (stckStatements.Peek().SelectEndsWithParenthesis && ParenthesisCount == 0)
                            {
                                // Pop SELECT
                                //WorkLine = "".PadRight(CurrentIndentation) + WorkLine.TrimStart();
                                CurrentIndentation -= stckStatements.Peek().Indentation;
                                stckStatements.Pop();
                                ControlStructureFound = true;
                                // Cant go to final code since we might have a ;
                            }
                        }
                    }

                    // Process

                    // Check for this type of assignment
                    // EXCEPTION WHEN E_NOGENERAR THEN  P_NEXITO := 0;
                    // or this one
                    //           WHEN E_NOGENERAR THEN  P_NEXITO := 0;
                    // which qualifies as EXCEPTION not as assigment
                    if (stckStatements.Count != 0)
                    {
                        if (rgASSIGN.IsMatch(WorkLineFix) &&
                            !rgEXCEPTION.IsMatch(WorkLineFix) &&
                            !rgWHEN.IsMatch(WorkLineFix) &&
                            stckStatements.Peek().Type != "PKB" &&
                            stckStatements.Peek().Type != "PF" &&
                            stckStatements.Peek().Type != "BG") // Assignment X := Y;
                        {
                            goto FinalCode;
                        }
                    }

                    if (rgPACKAGE.IsMatch(WorkLineFix))
                    {
                        // Check if user wants an empty line before
                        if (chkSpaceBeforeProc.Checked)
                        {
                            //Check if prev line was empty
                            if (PreviousWorkLine.Trim() != string.Empty && !PreviousWorkLine.TrimStart().StartsWith("--") && !PreviousWorkLine.TrimStart().StartsWith("CREATE"))
                            {
                                // Add empty line
                                sbIndented.Append("\r\n");
                            }
                        }

                        // Get more information
                        MoreInfo = rgPACKAGE.Match(WorkLineFix);

                        // Push
                        Statement stmt = new Statement();
                        stmt.Text = WorkLineFix;
                        //stmt.ProcName = string.Empty; // Procedure name
                        stmt.ProcName = MoreInfo.Groups[3].Value.Trim().Replace(@"""", ""); // Procedure name
                        stmt.HasAName = false;
                        stmt.Type = "PKB";
                        stmt.HasBegin = false;
                        stmt.BeginFound = false;
                        stmt.Indentation = 2;
                        stmt.DelimiterFound = rgIS.IsMatch(WorkLineFix) ? true : false;
                        stmt.SelectEndsWithParenthesis = false;
                        stckStatements.Push(stmt);
                        //WorkLine = "".PadRight(CurrentIndentation) + WorkLine.TrimStart();
                        if (stmt.DelimiterFound)
                        {
                            CurrentIndentation += stmt.Indentation;
                            DelimiterPending = "";
                        }
                        else
                        {
                            DelimiterPending = "IS";
                        }
                        ControlStructureFound = true;
                        goto FinalCode;
                    }

                    // End Package
                    if (rgENDPACKAGE2.IsMatch(WorkLineFix))
                    {
                        if (stckStatements.Count == 1)
                        {
                            if (stckStatements.Peek().Type == "PKB" || stckStatements.Peek().Type == "PF")
                            {
                                // Check if user wants to name this end statement
                                if (chkNameProcEnds.Checked)
                                {
                                    // See if we dont have a name
                                    if (!WorkLineFix.Contains(stckStatements.Peek().ProcName))
                                    {
                                        // Set the package name as we found it
                                        //WorkLine = WorkLine.Replace("END", "END " + stckStatements.Peek().ProcName);
                                    }
                                }
                                // Pop package
                                CurrentIndentation -= stckStatements.Peek().Indentation;
                                //if (!this.DesignMode) if (CurrentIndentation <= 0) CurrentIndentation = 0;
                                //WorkLine = "".PadRight(CurrentIndentation) + WorkLine.TrimStart();
                                stckStatements.Pop();
                                ControlStructureFound = true;
                                goto FinalCode;
                            }
                        }
                    }

                    if (rgIS.IsMatch(WorkLineFix))
                    {
                        if (DelimiterPending != string.Empty)
                        {
                            if (DelimiterPending == "IS" || DelimiterPending == "AS")
                            {
                                DelimiterPending = string.Empty;
                                stckStatements.Peek().DelimiterFound = true;
                                //WorkLine = "".PadRight(CurrentIndentation) + WorkLine.TrimStart();

                                CurrentIndentation += stckStatements.Peek().Indentation;
                                //ControlStructureFound = true;
                                goto FinalCode;
                            }
                        }
                    }

                    if (rgPROCFUN.IsMatch(WorkLineFix)) // Procedure or function header
                    {
                        // Check if user wants an empty line before
                        if (chkSpaceBeforeProc.Checked)
                        {
                            //Check if prev line was empty
                            if (PreviousWorkLine.Trim() != string.Empty && !PreviousWorkLine.TrimStart().StartsWith("--"))
                            {
                                // Add empty line
                                sbIndented.Append("\r\n");
                            }
                        }

                        // Get more information
                        MoreInfo = rgPROCFUN.Match(WorkLineFix);

                        // Push
                        Statement stmt = new Statement();
                        stmt.Text = WorkLineFix;
                        stmt.ProcName = MoreInfo.Groups[2].Value.Trim(); // Procedure name
                        stmt.HasAName = true;

                        // Indent procedure parameters
                        if (WorkLineFix.IndexOf("(") != -1)
                        {
                            stmt.ProcedureWordPos = ("".PadRight(CurrentIndentation) + WorkLineFix.TrimStart(' ')).IndexOf("(") + 1;
                        }
                        else
                        {
                            stmt.ProcedureWordPos = ("".PadRight(CurrentIndentation) + WorkLineFix.TrimStart(' ')).IndexOf(stmt.ProcName) + stmt.ProcName.Length + 1;
                        }

                        stmt.Type = "PF";
                        stmt.HasBegin = true;
                        stmt.BeginFound = false;
                        stmt.Indentation = 2;
                        stmt.DelimiterFound = rgIS.IsMatch(WorkLineFix) ? true : false;
                        stmt.SelectEndsWithParenthesis = false;
                        stmt.ExceptionFound = false;
                        stckStatements.Push(stmt);
                        //WorkLine = "".PadRight(CurrentIndentation) + WorkLine.TrimStart();

                        if (stmt.DelimiterFound)
                        {
                            CurrentIndentation += stmt.Indentation;
                            DelimiterPending = "";
                        }
                        else
                        {
                            DelimiterPending = "IS";
                        }
                        ControlStructureFound = true;
                        goto FinalCode;

                    }

                    if (rgDECLARE.IsMatch(WorkLineFix)) // Declare header
                    {

                        // Push
                        Statement stmt = new Statement();
                        stmt.Text = WorkLineFix;
                        stmt.ProcName = string.Empty;
                        stmt.HasAName = false;
                        stmt.ProcedureWordPos = 0;
                        stmt.Type = "PF";
                        stmt.HasBegin = true;
                        stmt.BeginFound = false;
                        stmt.Indentation = 2;
                        stmt.DelimiterFound = true;
                        stmt.SelectEndsWithParenthesis = false;
                        stmt.ExceptionFound = false;
                        stckStatements.Push(stmt);
                        //WorkLine = "".PadRight(CurrentIndentation) + WorkLine.TrimStart();
                        CurrentIndentation += stmt.Indentation;
                        DelimiterPending = "";
                        ControlStructureFound = true;
                        goto FinalCode;

                    }

                    // A Begin block
                    if (rgBEGIN.IsMatch(WorkLineFix))
                    {
                        if (stckStatements.Count != 0)
                        {
                            // Check if pushed statement requires a begin and one hasnt been foud yet
                            if (stckStatements.Peek().HasBegin)
                            {
                                if (!stckStatements.Peek().BeginFound)
                                {
                                    // Check if user wants to name this begin statement
                                    if (chkNameProcBegins.Checked)
                                    {
                                        // See if we dont have a name on this line
                                        if (!WorkLineFix.Contains(stckStatements.Peek().ProcName) && !WorkLineFix.Contains("--"))
                                        {
                                            // Set the procedure name as we found it
                                            //WorkLine = WorkLine.Replace("BEGIN", "BEGIN --" + stckStatements.Peek().ProcName);
                                        }
                                    }
                                    // We have here a procedure begin clause so paint without indentation and move on
                                    CurrentIndentation -= stckStatements.Peek().Indentation;
                                    //WorkLine = "".PadRight(CurrentIndentation) + WorkLine.TrimStart();
                                    CurrentIndentation += stckStatements.Peek().Indentation;
                                    stckStatements.Peek().BeginFound = true; // Say we found our begin clause
                                    ControlStructureFound = true;
                                    goto FinalCode;
                                }
                                else
                                {
                                    // This is a brand new begin structure
                                    // Push
                                    Statement stmt = new Statement();
                                    stmt.Text = WorkLineFix;
                                    stmt.ProcName = string.Empty; // Procedure name
                                    stmt.HasAName = false;
                                    stmt.Type = "BG";
                                    stmt.HasBegin = false;
                                    stmt.BeginFound = false;
                                    stmt.Indentation = 2;
                                    stmt.DelimiterFound = true;
                                    stmt.SelectEndsWithParenthesis = false;
                                    stmt.ExceptionFound = false;
                                    stckStatements.Push(stmt);
                                    //WorkLine = "".PadRight(CurrentIndentation) + WorkLine.TrimStart();
                                    CurrentIndentation += stmt.Indentation;
                                    ControlStructureFound = true;
                                    goto FinalCode;
                                }
                            }
                            else
                            {
                                // This is a brand new begin structure
                                // Push
                                Statement stmt = new Statement();
                                stmt.Text = WorkLineFix;
                                stmt.ProcName = string.Empty; // Procedure name
                                stmt.HasAName = false;
                                stmt.Type = "BG";
                                stmt.HasBegin = false;
                                stmt.BeginFound = false;
                                stmt.Indentation = 2;
                                stmt.DelimiterFound = true;
                                stmt.SelectEndsWithParenthesis = false;
                                stmt.ExceptionFound = false;
                                stckStatements.Push(stmt);
                                //WorkLine = "".PadRight(CurrentIndentation) + WorkLine.TrimStart();
                                CurrentIndentation += stmt.Indentation;
                                ControlStructureFound = true;
                                goto FinalCode;
                            }
                        }
                    }

                    if (stckStatements.Count != 0)
                    {
                        //Are we inside a begin or program or function block
                        if (stckStatements.Peek().Type == "BG" || stckStatements.Peek().Type == "PF")
                        {
                            //IF exception keyword found
                            if (rgEXCEPTION.IsMatch(WorkLineFix))
                            {
                                //Substract this item's indentation (BG or PF) since exception needs to be not indented
                                CurrentIndentation -= stckStatements.Peek().Indentation;
                                // Say that our exception section has been found
                                stckStatements.Peek().ExceptionFound = true;
                                // Change 2 space indentation into 7
                                // since exception has 7 spaces like this
                                // EXCEPTION
                                //   WHEN OTHERS THEN
                                // 1234567
                                //        NULL;
                                stckStatements.Peek().Indentation = 7;
                                //if (!this.DesignMode) if (CurrentIndentation <= 0) CurrentIndentation = 0;
                                //WorkLine = "".PadRight(CurrentIndentation) + WorkLine.TrimStart();
                                //Add indentation again
                                CurrentIndentation += stckStatements.Peek().Indentation;
                                ControlStructureFound = true;
                                goto FinalCode;
                            }
                        }
                    }

                    if (stckStatements.Count != 0)
                    {
                        //Are we inside a begin or program or function block and we have reached the exception section
                        if ((stckStatements.Peek().Type == "BG" || stckStatements.Peek().Type == "PF") && stckStatements.Peek().ExceptionFound)
                        {
                            // WHEN FOUND 
                            if (rgWHEN.IsMatch(WorkLineFix))
                            {
                                // Remove partial indentation since its currently 7
                                // EXCEPTION
                                // 1234567
                                //   WHEN
                                CurrentIndentation -= 5;
                                //WorkLine = "".PadRight(CurrentIndentation) + WorkLine.TrimStart();
                                CurrentIndentation += 5;
                                ControlStructureFound = true;
                                goto FinalCode;
                            }
                            else
                            {
                                //if (PreviousWorkLine.Contains("WHEN ") && !WorkLine.Contains("END"))
                                if (!WorkLineFix.Contains("END"))
                                {
                                    // its a when subclause so indent as normal since block's indentation should be at 7
                                    // i.e WHEN X THEN
                                    //          DO SOMETHING
                                    //WorkLine = "".PadRight(CurrentIndentation) + WorkLine.TrimStart();
                                    ControlStructureFound = true;
                                    goto FinalCode;
                                }
                            }
                        }
                    }

                    if (rgEND.IsMatch(WorkLineFix))
                    {
                        if (stckStatements.Count != 0)
                        {
                            // Check if we are ending last pusheded statement as a begin
                            if (stckStatements.Peek().Type == "BG")
                            {
                                // Pop Begin
                                CurrentIndentation -= stckStatements.Peek().Indentation;
                                //if (!this.DesignMode) if (CurrentIndentation <= 0) CurrentIndentation = 0;
                                //WorkLine = "".PadRight(CurrentIndentation) + WorkLine.TrimStart();
                                stckStatements.Pop();
                                ControlStructureFound = true;
                                goto FinalCode;
                            }
                        }
                    }

                    if (rgFOR.IsMatch(WorkLineFix))
                    {
                        // Push
                        Statement stmt = new Statement();
                        stmt.Text = WorkLineFix;
                        stmt.ProcName = string.Empty; // Procedure name
                        stmt.HasAName = false;
                        stmt.Type = "FR";
                        stmt.HasBegin = false;
                        stmt.BeginFound = false;
                        stmt.Indentation = 4;
                        stmt.DelimiterFound = rgLOOP.IsMatch(WorkLineFix) ? true : false;
                        stmt.SelectEndsWithParenthesis = false;
                        stckStatements.Push(stmt);
                        //WorkLine = "".PadRight(CurrentIndentation) + WorkLine.TrimStart();
                        if (stmt.DelimiterFound)
                        {
                            CurrentIndentation += stmt.Indentation;
                            DelimiterPending = "";
                        }
                        else
                        {
                            DelimiterPending = "LOOP";
                        }
                        ControlStructureFound = true;
                        goto FinalCode;
                    }

                    // Begin loop and not ending loop
                    if (rgLOOP.IsMatch(WorkLineFix) && !rgENDLOOP.IsMatch(WorkLineFix)) // Can be a delimiter pending or a starting loop
                    {
                        if (DelimiterPending != string.Empty)
                        {
                            if (DelimiterPending == "LOOP")
                            {
                                DelimiterPending = string.Empty;
                                stckStatements.Peek().DelimiterFound = true;
                                //WorkLine = "".PadRight(CurrentIndentation) + WorkLine.TrimStart();

                                CurrentIndentation += stckStatements.Peek().Indentation;
                                ControlStructureFound = true;
                                goto FinalCode;
                            }
                        }
                        else // new loop section. behaves as a for or while
                        {
                            // Push
                            Statement stmt = new Statement();
                            stmt.Text = WorkLineFix;
                            stmt.ProcName = string.Empty; // Procedure name
                            stmt.HasAName = false;
                            stmt.Type = "FR";
                            stmt.HasBegin = false;
                            stmt.BeginFound = false;
                            stmt.Indentation = 4;
                            stmt.DelimiterFound = true;
                            stmt.SelectEndsWithParenthesis = false;
                            stckStatements.Push(stmt);
                            //WorkLine = "".PadRight(CurrentIndentation) + WorkLine.TrimStart();
                            CurrentIndentation += stmt.Indentation;
                            DelimiterPending = "";
                            ControlStructureFound = true;
                            goto FinalCode;
                        }
                    }

                    // Check for procedure closure
                    if (stckStatements.Count != 0)
                    {
                        if (stckStatements.Peek().BeginFound && (stckStatements.Peek().ProcName != string.Empty || !stckStatements.Peek().HasAName))
                        {
                            rgENDPROCFUN = new Regex("END( )?" + stckStatements.Peek().ProcName + ";", RegexOptions.IgnoreCase);
                            if (rgENDPROCFUN.IsMatch(WorkLineFix))
                            {
                                // Check if user wants to name this end statement
                                if (chkNameProcEnds.Checked)
                                {
                                    // See if we dont have a name
                                    if (!WorkLineFix.Contains(stckStatements.Peek().ProcName))
                                    {
                                        // Set the package name as we found it
                                        //WorkLine = WorkLine.Replace("END", "END " + stckStatements.Peek().ProcName);
                                    }
                                }
                                // Pop Procedure
                                CurrentIndentation -= stckStatements.Peek().Indentation;
                                //if (!this.DesignMode) if (CurrentIndentation <= 0) CurrentIndentation = 0;
                                stckStatements.Pop();
                                ControlStructureFound = true;
                                //WorkLine = "".PadRight(CurrentIndentation) + WorkLine.TrimStart();

                                if (chkSpaceBeforeProc.Checked)
                                {
                                    WorkLineFix += "\r\n";
                                }
                                goto FinalCode;
                            }
                            else
                            {
                                rgENDPROCFUN = new Regex("END( )?;", RegexOptions.IgnoreCase);
                                if (rgENDPROCFUN.IsMatch(WorkLineFix))
                                {
                                    // Check if user wants to name this end statement
                                    if (chkNameProcEnds.Checked)
                                    {
                                        // See if we dont have a name
                                        if (!WorkLineFix.Contains(stckStatements.Peek().ProcName))
                                        {
                                            // Set the package name as we found it
                                            //WorkLine = WorkLine.Replace("END", "END " + stckStatements.Peek().ProcName);
                                        }
                                    }
                                    // Pop Procedure
                                    CurrentIndentation -= stckStatements.Peek().Indentation;
                                    //if (!this.DesignMode) if (CurrentIndentation <= 0) CurrentIndentation = 0;
                                    stckStatements.Pop();
                                    ControlStructureFound = true;
                                    //WorkLine = "".PadRight(CurrentIndentation) + WorkLine.TrimStart();
                                    if (chkSpaceBeforeProc.Checked)
                                    {
                                        WorkLineFix += "\r\n";
                                    }
                                    goto FinalCode;
                                }
                            }
                        }
                    }

                    if (rgIF.IsMatch(WorkLineFix))
                    {
                        // Get more information
                        MoreInfo = rgIF.Match(WorkLineFix);

                        // Push
                        Statement stmt = new Statement();
                        stmt.Text = WorkLineFix;
                        stmt.ProcName = string.Empty; // Procedure name
                        stmt.HasAName = false;
                        stmt.Type = "IF";
                        stmt.HasBegin = false;
                        stmt.BeginFound = false;
                        stmt.Indentation = 4;
                        stmt.DelimiterFound = rgTHEN.IsMatch(WorkLineFix) ? true : false;
                        stmt.SelectEndsWithParenthesis = false;
                        stckStatements.Push(stmt);
                        //WorkLine = "".PadRight(CurrentIndentation) + WorkLine.TrimStart();
                        // FIX 2 spaces after the IF
                        int IfPos = WorkLineFix.IndexOf("IF") + 2;
                        while (WorkLineFix.Substring(IfPos, 1) == " ")
                        {
                            IfPos++;
                        }
                        //WorkLine = WorkLine.Substring(0, WorkLine.IndexOf("IF") + 2) + "  " + WorkLine.Substring(IfPos);

                        if (stmt.DelimiterFound)
                        {
                            CurrentIndentation += stmt.Indentation;
                            DelimiterPending = "";
                        }
                        else
                        {
                            DelimiterPending = "THEN";
                        }
                        ControlStructureFound = true;
                        goto FinalCode;
                    }

                    if (rgTHEN.IsMatch(WorkLineFix))
                    {
                        if (DelimiterPending != string.Empty)
                        {
                            if (DelimiterPending == "THEN")
                            {
                                DelimiterPending = string.Empty;
                                stckStatements.Peek().DelimiterFound = true;
                                //WorkLine = "".PadRight(CurrentIndentation) + WorkLine.TrimStart();

                                CurrentIndentation += stckStatements.Peek().Indentation;
                                ControlStructureFound = true;
                                goto FinalCode;
                            }
                        }
                    }

                    if (stckStatements.Count != 0)
                    {
                        if (stckStatements.Peek().Type == "IF")
                        {
                            if (rgELSE.IsMatch(WorkLineFix))
                            {
                                // We have here a else clause so paint without indentation and move on
                                CurrentIndentation -= stckStatements.Peek().Indentation;
                                //WorkLine = "".PadRight(CurrentIndentation) + WorkLine.TrimStart();
                                CurrentIndentation += stckStatements.Peek().Indentation;
                                ControlStructureFound = true;
                                goto FinalCode;
                            }
                        }
                    }

                    if (stckStatements.Count != 0)
                    {
                        if (stckStatements.Peek().Type == "IF")
                        {
                            if (rgELSIF.IsMatch(WorkLineFix))
                            {
                                // We have here a else clause so paint without indentation and move on
                                CurrentIndentation -= stckStatements.Peek().Indentation;
                                //WorkLine = "".PadRight(CurrentIndentation) + WorkLine.TrimStart();
                                CurrentIndentation += stckStatements.Peek().Indentation;
                                ControlStructureFound = true;
                                goto FinalCode;
                            }
                        }
                    }

                    if (rgENDIF.IsMatch(WorkLineFix))
                    {
                        if (stckStatements.Count != 0)
                        {
                            // Pop package
                            CurrentIndentation -= stckStatements.Peek().Indentation;
                            //if (!this.DesignMode) if (CurrentIndentation <= 0) CurrentIndentation = 0;
                            //WorkLine = "".PadRight(CurrentIndentation) + WorkLine.TrimStart();
                            stckStatements.Pop();
                            ControlStructureFound = true;
                            goto FinalCode;
                        }
                    }

                    if (rgENDLOOP.IsMatch(WorkLineFix))
                    {
                        if (stckStatements.Count != 0)
                        {
                            // Pop package
                            CurrentIndentation -= stckStatements.Peek().Indentation;
                            //if (!this.DesignMode) if (CurrentIndentation <= 0) CurrentIndentation = 0;
                            //WorkLine = "".PadRight(CurrentIndentation) + WorkLine.TrimStart();
                            stckStatements.Pop();
                            ControlStructureFound = true;
                            goto FinalCode;
                        }
                    }

                    if (rgUNION.IsMatch(WorkLineFix))
                    {
                        // Check if prev stack is select then pop
                        if (stckStatements.Count != 0)
                        {
                            if (stckStatements.Peek().Type == "SL")
                            {
                                // Pop Select
                                CurrentIndentation -= stckStatements.Peek().Indentation;
                                //if (!this.DesignMode) if (CurrentIndentation <= 0) CurrentIndentation = 0;
                                //WorkLine = "".PadRight(CurrentIndentation) + WorkLine.TrimStart();
                                stckStatements.Pop();
                                ControlStructureFound = true;
                            }
                        }
                    }

                    // Check this situation of an inner select
                    //             AND nplazofinal = (SELECT min(nplazofinal) FROM toconceptostasas WHERE oconcept_vkpcodigo = p_vconcepto);
                    if (rgSELECT.IsMatch(WorkLineFix))
                    {

                        // Chech if we are an inner select that ends here. So no push needed

                        // If we are a subselect inside a major one and all parenthesis have been closed
                        if (stckStatements.Peek().Type == "SL" && ParenthesisCount == 0)
                        {
                            // Discard list push pop management. Treat as a normal select sub statement (such as where, order by etc)
                        }
                        else
                        {
                            // Check if we are part of a INSERT statement such as
                            // INSERT 
                            //   INTO TABLE
                            //        (x, y, z)
                            // SELECT A, B C FROM D...
                            if (stckStatements.Peek().Type == "IN")
                            {
                                // Nothing to do just continue 
                            }
                            else
                            {
                                // FIX spaces between keyword and the rest
                                Match match = rgSELECT.Match(WorkLineFix);
                                //WorkLine = "".PadRight(CurrentIndentation) + match.Groups["KWD"].ToString() + " " + WorkLine.Replace(match.Groups["KWD"].ToString(), " ").TrimStart();

                                // Push
                                Statement stmt = new Statement();
                                stmt.Text = WorkLineFix;
                                stmt.ProcName = string.Empty; // Procedure name
                                stmt.HasAName = false;
                                stmt.Type = "SL";
                                stmt.HasBegin = false;
                                stmt.BeginFound = false;
                                stmt.Indentation = 2;
                                stmt.DelimiterFound = true;
                                // See if we have an open parenthesis and we are in a select
                                if (stckStatements.Count != 0)
                                {
                                    if (stckStatements.Peek().Type == "SL" && ParenthesisCount > 0)
                                    {
                                        // We are a select inside a select
                                        // So say we end with a parenthesis count = 0
                                        stmt.SelectEndsWithParenthesis = true;
                                        // Locate the select word position
                                        stmt.MainKWordPos = (WorkLineFix.TrimStart(' ')).IndexOf("SELECT");
                                    }
                                    else
                                    {
                                        stmt.SelectEndsWithParenthesis = false;
                                        stmt.MainKWordPos = 0;
                                    }
                                }
                                stckStatements.Push(stmt);

                                //WorkLine = "".PadRight(CurrentIndentation) + WorkLine.TrimStart();
                                CurrentIndentation += stmt.Indentation;
                                ControlStructureFound = true;
                                goto FinalCode;
                            }
                        }
                    }

                    // new UPDATE statement
                    if (rgUPDATE.IsMatch(WorkLineFix)) // THIS IF NEEDS TO BE CHECKED AND WAS COPIED FROM rgSELECT
                    {
                        // Chech if we are an inner select that ends here. So no push needed

                        // If we are a subselect inside a major one and all parenthesis have been closed
                        if (stckStatements.Peek().Type == "UP" && ParenthesisCount == 0)
                        {
                            // Discard list push pop management. Treat as a normal select sub statement (such as where, order by etc)
                        }
                        else
                        {
                            // FIX spaces between keyword and the rest
                            Match match = rgUPDATE.Match(WorkLineFix);
                            //WorkLine = "".PadRight(CurrentIndentation) + match.Groups["KWD"].ToString() + " " + WorkLine.Replace(match.Groups["KWD"].ToString(), " ").TrimStart();

                            // Push
                            Statement stmt = new Statement();
                            stmt.Text = WorkLineFix;
                            stmt.ProcName = string.Empty; // Procedure name
                            stmt.HasAName = false;
                            stmt.Type = "UP";
                            stmt.HasBegin = false;
                            stmt.BeginFound = false;
                            stmt.Indentation = 2;
                            stmt.DelimiterFound = true;
                            // See if we have an open parenthesis and we are in a select
                            if (stckStatements.Count != 0)
                            {
                                if (stckStatements.Peek().Type == "UP" && ParenthesisCount > 0)
                                {
                                    // We are a select inside a select
                                    // So say we end with a parenthesis count = 0
                                    stmt.SelectEndsWithParenthesis = true;
                                    // Locate the select word position
                                    stmt.MainKWordPos = (WorkLineFix.TrimStart(' ')).IndexOf("UPDATE");
                                }
                                else
                                {
                                    stmt.SelectEndsWithParenthesis = false;
                                    stmt.MainKWordPos = 0;
                                }
                            }
                            stckStatements.Push(stmt);

                            //WorkLine = "".PadRight(CurrentIndentation) + WorkLine.TrimStart();
                            CurrentIndentation += stmt.Indentation;
                            ControlStructureFound = true;
                            goto FinalCode;
                        }
                    }

                    // new DELETE statement
                    if (rgDELETE.IsMatch(WorkLineFix)) // THIS IF NEEDS TO BE CHECKED AND WAS COPIED FROM rgSELECT
                    {
                        // Chech if we are an inner select that ends here. So no push needed

                        // If we are a subselect inside a major one and all parenthesis have been closed
                        if (stckStatements.Peek().Type == "DL" && ParenthesisCount == 0)
                        {
                            // Discard list push pop management. Treat as a normal select sub statement (such as where, order by etc)
                        }
                        else
                        {
                            // FIX spaces between keyword and the rest
                            Match match = rgDELETE.Match(WorkLineFix);
                            //WorkLine = "".PadRight(CurrentIndentation) + match.Groups["KWD"].ToString() + " " + WorkLine.Replace(match.Groups["KWD"].ToString(), " ").TrimStart();

                            // Push
                            Statement stmt = new Statement();
                            stmt.Text = WorkLineFix;
                            stmt.ProcName = string.Empty; // Procedure name
                            stmt.HasAName = false;
                            stmt.Type = "DL";
                            stmt.HasBegin = false;
                            stmt.BeginFound = false;
                            stmt.Indentation = 2;
                            stmt.DelimiterFound = true;
                            // See if we have an open parenthesis and we are in a select
                            if (stckStatements.Count != 0)
                            {
                                if (stckStatements.Peek().Type == "DL" && ParenthesisCount > 0)
                                {
                                    // We are a select inside a select
                                    // So say we end with a parenthesis count = 0
                                    stmt.SelectEndsWithParenthesis = true;
                                    // Locate the select word position
                                    stmt.MainKWordPos = (WorkLineFix.TrimStart(' ')).IndexOf("DELETE");
                                }
                                else
                                {
                                    stmt.SelectEndsWithParenthesis = false;
                                    stmt.MainKWordPos = 0;
                                }
                            }
                            stckStatements.Push(stmt);

                            //WorkLine = "".PadRight(CurrentIndentation) + WorkLine.TrimStart();
                            CurrentIndentation += stmt.Indentation;
                            ControlStructureFound = true;
                            goto FinalCode;
                        }
                    }

                    // Look for ; on the SELECT statement
                    if (rgENDSELECT.IsMatch(WorkLineFix))
                    {
                        if (stckStatements.Count != 0)
                        {
                            // Check if we are ending a select statement
                            if (stckStatements.Peek().Type == "SL")
                            {
                                // if select kw then indent less else indent 7 = "SELECT "
                                if (rgSELECTKWS.IsMatch(WorkLineFix))
                                {
                                    Match match = rgSELECTKWS.Match(WorkLineFix);

                                    // FIX spaces between keyword and the rest
                                    //WorkLine = "".PadRight(CurrentIndentation) + match.Groups["KWD"].ToString() + " " + WorkLine.Replace(match.Groups["KWD"].ToString(), " ").TrimStart();

                                    // ie. SELECT 1 
                                    //       FROM DUAL;
                                    int FirstWordLength = WorkLineFix.TrimStart().Substring(0, WorkLineFix.TrimStart().IndexOf(" ")).Length;
                                    //WorkLine = "".PadRight(CurrentIndentation - stckStatements.Peek().Indentation + "SELECT".Length - FirstWordLength) + WorkLine.TrimStart().Replace(match.Groups["KWD"].ToString() + "  ", match.Groups["KWD"].ToString() + " ");
                                    // Also remove double spaces after keyword 
                                    // i.e AND  XX
                                    // to  AND XX
                                }
                                else
                                {
                                    // ie. SELECT 1 FROM
                                    //            DUAL;
                                    //WorkLine = "".PadRight(CurrentIndentation - stckStatements.Peek().Indentation + "SELECT ".Length) + WorkLine.TrimStart();
                                }
                                CurrentIndentation -= stckStatements.Peek().Indentation;
                                //if (!this.DesignMode) if (CurrentIndentation <= 0) CurrentIndentation = 0;
                                // Pop Select
                                stckStatements.Pop();
                                ControlStructureFound = true;
                                goto FinalCode;
                            }
                        }
                    }

                    // Look for ; on the INSERT statement
                    if (rgENDINSERT.IsMatch(WorkLineFix))
                    {
                        if (stckStatements.Count != 0)
                        {
                            // Check if we are ending a INSERT statement
                            if (stckStatements.Peek().Type == "IN")
                            {
                                // if VALUES kw then indent less else indent 7 = "INSERT "
                                if (rgINSERTKWS.IsMatch(WorkLineFix))
                                {
                                    // Fix spaces in between keyword to just 1 space
                                    Match match = rgINSERTKWS.Match(WorkLineFix);
                                    //WorkLine = "".PadRight(CurrentIndentation) + match.Groups["KWD"].ToString() + " " + WorkLine.Replace(match.Groups["KWD"].ToString(), " ").TrimStart();

                                    //WorkLine = "".PadRight(CurrentIndentation - "INSERT".Length) + WorkLine.TrimStart();
                                }
                                else
                                {
                                    //WorkLine = "".PadRight(CurrentIndentation) + WorkLine.TrimStart();
                                }

                                CurrentIndentation -= stckStatements.Peek().Indentation;
                                //if (!this.DesignMode) if (CurrentIndentation <= 0) CurrentIndentation = 0;
                                // Pop Select
                                stckStatements.Pop();
                                ControlStructureFound = true;
                                goto FinalCode;
                            }
                        }
                    }

                    // Look for VALUES KWd on the INSERT statement
                    if (stckStatements.Count != 0)
                    {
                        // Check if we are ending a INSERT statement
                        if (stckStatements.Peek().Type == "IN")
                        {
                            // if VALUES kw then indent less else indent 7 = "INSERT "
                            if (rgINSERTKWS.IsMatch(WorkLineFix))
                            {
                                // Fix spaces in between keyword to just 1 space
                                Match match = rgINSERTKWS.Match(WorkLineFix);
                                //WorkLine = "".PadRight(CurrentIndentation) + match.Groups["KWD"].ToString() + " " + WorkLine.Replace(match.Groups["KWD"].ToString(), " ").TrimStart();

                                //WorkLine = "".PadRight(CurrentIndentation - "INSERT".Length) + WorkLine.TrimStart();
                                ControlStructureFound = true;
                                goto FinalCode;
                            }
                        }
                    }

                    // Look for ; on the UPDATE statement
                    if (rgENDUPDATE.IsMatch(WorkLineFix)) //this needs to be tested
                    {
                        if (stckStatements.Count != 0)
                        {
                            // Check if we are ending a UPDATE statement
                            if (stckStatements.Peek().Type == "UP")
                            {
                                // if UPDATE kw then indent less else indent 7 = "UPDATE "
                                if (rgUPDATEKWS.IsMatch(WorkLineFix))
                                {
                                    Match match = rgUPDATEKWS.Match(WorkLineFix);
                                    // Fix spaces in between keyword to just 1 space
                                    //WorkLine = "".PadRight(CurrentIndentation) + match.Groups["KWD"].ToString() + " " + WorkLine.Replace(match.Groups["KWD"].ToString(), " ").TrimStart();

                                    // ie. UPDATE X 
                                    //        SET Y;
                                    int FirstWordLength = WorkLineFix.TrimStart().Substring(0, WorkLineFix.TrimStart().IndexOf(" ")).Length;
                                    //WorkLine = "".PadRight(CurrentIndentation - stckStatements.Peek().Indentation + "UPDATE".Length - FirstWordLength) + WorkLine.TrimStart().Replace(match.Groups["KWD"].ToString() + "  ", match.Groups["KWD"].ToString() + " ");
                                    // Also remove double spaces after keyword 
                                    // i.e AND  XX
                                    // to  AND XX
                                }
                                else
                                {
                                    // ie. UPDATE X SET Y = 
                                    //            1;
                                    //WorkLine = "".PadRight(CurrentIndentation - stckStatements.Peek().Indentation + "UPDATE ".Length) + WorkLine.TrimStart();
                                }
                                CurrentIndentation -= stckStatements.Peek().Indentation;
                                //if (!this.DesignMode) if (CurrentIndentation <= 0) CurrentIndentation = 0;
                                // Pop Select
                                stckStatements.Pop();
                                ControlStructureFound = true;
                                goto FinalCode;
                            }
                        }
                    }

                    // Look for ; on the DELETE statement
                    if (rgENDDELETE.IsMatch(WorkLineFix)) //this needs to be tested
                    {
                        if (stckStatements.Count != 0)
                        {
                            // Check if we are ending a UPDATE statement
                            if (stckStatements.Peek().Type == "DL")
                            {
                                // if DELETE kw then indent less else indent 7 = "DELETE "
                                if (rgUPDATEKWS.IsMatch(WorkLineFix))
                                {
                                    Match match = rgDELETEKWS.Match(WorkLineFix);
                                    // ie. UPDATE X 
                                    //        SET Y;
                                    int FirstWordLength = WorkLineFix.TrimStart().Substring(0, WorkLineFix.TrimStart().IndexOf(" ")).Length;
                                    //WorkLine = "".PadRight(CurrentIndentation - stckStatements.Peek().Indentation + "DELETE".Length - FirstWordLength) + WorkLine.TrimStart().Replace(match.Groups["KWD"].ToString() + "  ", match.Groups["KWD"].ToString() + " ");
                                    // Also remove double spaces after keyword 
                                    // i.e AND  XX
                                    // to  AND XX
                                }
                                else
                                {
                                    // ie. DELETE X WHERE Y = 
                                    //            1;
                                    //WorkLine = "".PadRight(CurrentIndentation - stckStatements.Peek().Indentation + "DELETE ".Length) + WorkLine.TrimStart();
                                }
                                CurrentIndentation -= stckStatements.Peek().Indentation;
                                //if (!this.DesignMode) if (CurrentIndentation <= 0) CurrentIndentation = 0;
                                // Pop Select
                                stckStatements.Pop();
                                ControlStructureFound = true;
                                goto FinalCode;
                            }
                        }
                    }

                    // Indent parameters of called procs or functions
                    // Only do this is parenthesis count = 1 and we are not on a inner proc or function
                    // This is done at the end so CurrentIndentation is already set
                    if (!rgPROCFUN.IsMatch(WorkLineFix) &&
                        !rgSELECT.IsMatch(WorkLineFix) &&
                        ParenthesisCount == 1 &&
                        !OneParenthesisIsStillOpen &&
                        stckStatements.Peek().Type != "PF" &&
                        stckStatements.Peek().Type != "SL") // Procedure or function header
                    {
                        // Determine parenthesis position and save it. Use it later for indentation whilst parenthesis count = 0
                        OneParenthesisPosition = WorkLineFix.TrimStart(' ').LastIndexOf("(") + CurrentIndentation + 1;
                        // Say we are waiting on a parenthesis closure
                        OneParenthesisIsStillOpen = true;
                        // Turn flag on to skip current line
                        OneParenthesisFlag = true;
                    }


                    // If we have an INTO TABLE
                    if (rgINTOTABLE.IsMatch(WorkLineFix))
                    {
                        // If previous line is an insert
                        if (PreviousWorkLine.Trim() == "INSERT")
                        {
                            // Add 2 spaces
                            //WorkLine = "".PadRight(CurrentIndentation) + "  " + WorkLine.TrimStart();
                            // Add to stack
                            // Push
                            Statement stmt = new Statement();
                            stmt.Text = WorkLineFix;
                            stmt.ProcName = string.Empty; // Procedure name
                            stmt.HasAName = false;
                            stmt.Type = "IN";
                            stmt.HasBegin = false;
                            stmt.BeginFound = false;
                            stmt.Indentation = 6;
                            stmt.DelimiterFound = true;
                            stckStatements.Push(stmt);

                            CurrentIndentation += stmt.Indentation;
                            ControlStructureFound = true;
                            goto FinalCode;
                        }
                    }


                FinalCode:

                    if (!ControlStructureFound)
                    {
                        if (chkKeepGreaterIndent.Checked)
                        {
                            // Check if indentation is < then
                            if (WorkLineFix.Length - WorkLineFix.TrimStart(' ').Length < CurrentIndentation)
                            {
                                // If one parenthesis open then only do that padding
                                if (OneParenthesisIsStillOpen)
                                {
                                    // This flag is used to skip current line
                                    if (OneParenthesisFlag)
                                    {
                                        //WorkLine = "".PadRight(CurrentIndentation) + WorkLine.TrimStart();
                                        OneParenthesisFlag = false;
                                    }
                                    else
                                    {
                                        //WorkLine = "".PadRight(OneParenthesisPosition) + WorkLine.TrimStart();
                                    }

                                }
                                else
                                {
                                    // This flag is used here to indent one last time
                                    if (OneParenthesisFlag)
                                    {
                                        //WorkLine = "".PadRight(OneParenthesisPosition) + WorkLine.TrimStart();
                                        // Clear parenthesis position 
                                        OneParenthesisPosition = 0;
                                        OneParenthesisFlag = false;
                                    }
                                    else
                                    {
                                        //WorkLine = "".PadRight(CurrentIndentation) + WorkLine.TrimStart();
                                    }
                                }
                            }
                        }
                        else
                        {
                            // If one parenthesis open then only do that padding
                            if (OneParenthesisIsStillOpen)
                            {
                                // This flag is used to skip current line
                                if (OneParenthesisFlag)
                                {
                                    //WorkLine = "".PadRight(CurrentIndentation) + WorkLine.TrimStart();
                                    OneParenthesisFlag = false;
                                }
                                else
                                {
                                    //WorkLine = "".PadRight(OneParenthesisPosition) + WorkLine.TrimStart();
                                }

                            }
                            else
                            {
                                // This flag is used here to indent one last time
                                if (OneParenthesisFlag)
                                {
                                    //WorkLine = "".PadRight(OneParenthesisPosition) + WorkLine.TrimStart();
                                    // Clear parenthesis position 
                                    OneParenthesisPosition = 0;
                                    OneParenthesisFlag = false;
                                }
                                else
                                {
                                    //WorkLine = "".PadRight(CurrentIndentation) + WorkLine.TrimStart();
                                }
                            }

                        }

                        // Check for SELECT indentation that behaves differently
                        if (stckStatements.Count != 0 && WorkLineFix.Trim().Length != 0)
                        {
                            if (stckStatements.Peek().Type == "SL")
                            {
                                if (!rgSELECT.IsMatch(WorkLineFix))
                                {
                                    int AdditionalIndent = stckStatements.Peek().MainKWordPos;

                                    if (rgSELECTKWS.IsMatch(WorkLineFix))
                                    {
                                        Match match = rgSELECTKWS.Match(WorkLineFix);

                                        // FIX spaces between keyword and the rest
                                        //WorkLine = "".PadRight(CurrentIndentation) + match.Groups["KWD"].ToString() + " " + WorkLine.Replace(match.Groups["KWD"].ToString(), " ").TrimStart();

                                        // indent line under select based on first word length
                                        // get first word length
                                        string TrimmedLine = WorkLineFix.TrimStart();
                                        int FirstWordLength = TrimmedLine.Substring(0, TrimmedLine.IndexOf(" ")).Length;
                                        //WorkLine = "".PadRight(CurrentIndentation - stckStatements.Peek().Indentation) +
                                        //           "".PadRight("SELECT".Length - FirstWordLength) +
                                        //           "".PadRight(AdditionalIndent) + WorkLine.TrimStart().Replace(match.Groups["KWD"].ToString() + "  ", match.Groups["KWD"].ToString() + " ");

                                        // Also remove double spaces after keyword 
                                        // i.e AND  XX
                                        // to  AND XX
                                    }
                                    else
                                    {
                                        // Check if we have a comma here
                                        if (WorkLineFix.TrimStart().Substring(0, 2) == ", ")
                                        {
                                            // SELECT X
                                            //      , Y
                                            //WorkLine = "".PadRight(CurrentIndentation - stckStatements.Peek().Indentation + "SELECT ".Length - 2) +
                                                   WorkLineFix.TrimStart();
                                        }
                                        else
                                        {
                                            if (WorkLineFix.TrimStart().Substring(0, 1) == ",")
                                            {
                                                // SELECT X
                                                //       ,Y
                                                //WorkLine = "".PadRight(CurrentIndentation - stckStatements.Peek().Indentation + "SELECT ".Length - 1) +
                                                       WorkLineFix.TrimStart();
                                            }
                                            else
                                            {
                                                // SELECT X,
                                                //        Y
                                                //WorkLine = "".PadRight(CurrentIndentation - stckStatements.Peek().Indentation + "SELECT ".Length) +
                                                   WorkLineFix.TrimStart();
                                            }
                                        }

                                    }
                                }
                            }

                            // UPDATE
                            if (stckStatements.Peek().Type == "UP") // This needs to be checked
                            {
                                if (!rgUPDATE.IsMatch(WorkLineFix))
                                {
                                    int AdditionalIndent = stckStatements.Peek().MainKWordPos;

                                    if (rgUPDATEKWS.IsMatch(WorkLineFix))
                                    {
                                        Match match = rgUPDATEKWS.Match(WorkLineFix);

                                        // FIX spaces between keyword and the rest
                                        //WorkLine = "".PadRight(CurrentIndentation) + match.Groups["KWD"].ToString() + " " + WorkLine.Replace(match.Groups["KWD"].ToString(), " ").TrimStart();

                                        // indent line under select based on first word length
                                        // get first word length
                                        string TrimmedLine = WorkLineFix.TrimStart();
                                        int FirstWordLength = TrimmedLine.Substring(0, TrimmedLine.IndexOf(" ")).Length;
                                        //WorkLine = "".PadRight(CurrentIndentation - stckStatements.Peek().Indentation) +
                                        //           "".PadRight("UPDATE".Length - FirstWordLength) +
                                        //           "".PadRight(AdditionalIndent) + WorkLine.TrimStart().Replace(match.Groups["KWD"].ToString() + "  ", match.Groups["KWD"].ToString() + " ");

                                        // Also remove double spaces after keyword 
                                        // i.e AND  XX
                                        // to  AND XX
                                    }
                                    else
                                    {
                                        // Check if we have a comma here
                                        if (WorkLineFix.TrimStart().Substring(0, 2) == ", ")
                                        {
                                            // UPDATE X
                                            //    SET A = B
                                            //      , Y = C
                                            //WorkLine = "".PadRight(CurrentIndentation - stckStatements.Peek().Indentation + "UPDATE ".Length - 2) +
                                                   WorkLineFix.TrimStart();
                                        }
                                        else
                                        {
                                            if (WorkLineFix.TrimStart().Substring(0, 1) == ",")
                                            {
                                                // UPDATE X
                                                //    SET A = B
                                                //       ,Y = C
                                                //WorkLine = "".PadRight(CurrentIndentation - stckStatements.Peek().Indentation + "UPDATE ".Length - 1) +
                                                       WorkLineFix.TrimStart();
                                            }
                                            else
                                            {
                                                // UPDATE X
                                                //    SET A = B,
                                                //        Y = C
                                                //WorkLine = "".PadRight(CurrentIndentation - stckStatements.Peek().Indentation + "UPDATE ".Length) +
                                                   WorkLineFix.TrimStart();
                                            }
                                        }

                                    }
                                }
                            }

                            // DELETE
                            if (stckStatements.Peek().Type == "DL") // This needs to be checked
                            {
                                if (!rgDELETE.IsMatch(WorkLineFix))
                                {
                                    int AdditionalIndent = stckStatements.Peek().MainKWordPos;

                                    if (rgDELETEKWS.IsMatch(WorkLineFix))
                                    {
                                        Match match = rgDELETEKWS.Match(WorkLineFix);

                                        // FIX spaces between keyword and the rest
                                        //WorkLine = "".PadRight(CurrentIndentation) + match.Groups["KWD"].ToString() + " " + WorkLine.Replace(match.Groups["KWD"].ToString(), " ").TrimStart();

                                        // indent line under select based on first word length
                                        // get first word length
                                        string TrimmedLine = WorkLineFix.TrimStart();
                                        int FirstWordLength = TrimmedLine.Substring(0, TrimmedLine.IndexOf(" ")).Length;
                                        //WorkLine = "".PadRight(CurrentIndentation - stckStatements.Peek().Indentation) +
                                        //           "".PadRight("DELETE".Length - FirstWordLength) +
                                        //           "".PadRight(AdditionalIndent) + WorkLine.TrimStart().Replace(match.Groups["KWD"].ToString() + "  ", match.Groups["KWD"].ToString() + " ");

                                        // Also remove double spaces after keyword 
                                        // i.e AND  XX
                                        // to  AND XX
                                    }
                                    else
                                    {
                                        // Check if we have a comma here
                                        if (WorkLineFix.TrimStart().Substring(0, 2) == ", ")
                                        {
                                            // DELETE X
                                            //    SET A = B
                                            //      , Y = C
                                            //WorkLine = "".PadRight(CurrentIndentation - stckStatements.Peek().Indentation + "DELETE ".Length - 2) +
                                                   WorkLineFix.TrimStart();
                                        }
                                        else
                                        {
                                            if (WorkLineFix.TrimStart().Substring(0, 1) == ",")
                                            {
                                                // DELETE X
                                                //    SET A = B
                                                //       ,Y = C
                                                //WorkLine = "".PadRight(CurrentIndentation - stckStatements.Peek().Indentation + "DELETE ".Length - 1) +
                                                       WorkLineFix.TrimStart();
                                            }
                                            else
                                            {
                                                // DELETE X
                                                //    SET A = B,
                                                //        Y = C
                                                //WorkLine = "".PadRight(CurrentIndentation - stckStatements.Peek().Indentation + "DELETE ".Length) +
                                                   WorkLineFix.TrimStart();
                                            }
                                        }

                                    }
                                }
                            }

                            // Check for Procedure parameter indentation
                            if (stckStatements.Peek().Type == "PF")
                            {
                                if ((!stckStatements.Peek().DelimiterFound) ||
                                    (stckStatements.Peek().DelimiterFound && rgIS.IsMatch(WorkLineFix) && !rgCURSOR.IsMatch(WorkLineFix) && !rgTYPE.IsMatch(WorkLineFix)))
                                {
                                    int StripComma = 0;
                                    if (WorkLineFix.TrimStart().StartsWith(","))
                                    {
                                        StripComma = -1;
                                    }
                                    if (!rgPROCFUN.IsMatch(WorkLineFix))
                                    {
                                        //WorkLine = "".PadRight(stckStatements.Peek().ProcedureWordPos + StripComma) + WorkLine.TrimStart();
                                    }
                                }
                            }

                        }
                    }

                    // Add to final output buffer
                    sbIndented.Append(WorkLineFix.TrimEnd() + "\n");

                    // Seach next line
                    if (LinePointerStart != OriginalCode.Length)
                    {
                        LinePointerEnd = OriginalCode.IndexOf("\r", LinePointerStart + 1);
                    }
                    else
                    {
                        break;
                    }
                    if (LinePointerEnd == -1)
                    {
                        LinePointerEnd = OriginalCode.Length;
                    }
                    if (LinePointerEnd - 1 == LinePointerStart)
                    {
                        LinePointerStart += 1;
                    }
                    else
                    {
                        LinePointerStart += 2;
                    }
                }
                catch (Exception ex)
                {
                    // Something happened so cancel process and output upto here
                    LinePointerEnd = -1;
                    sbIndented.Append("\r\n------------ ERROR AQUI ---------\r\n");
                    sbIndented.Append("Linea con problemas: " + WorkLineFix + "\r\n");
                    sbIndented.Append(ex.ToString());
                    MessageBox.Show("Ocurrio un error procesando la linea " + LineCount);
                }
            }

            //bgwProceso.ReportProgress(LineCount, CurrentIndentation);

            IndentedCode = sbIndented.ToString();
            IndentedCode = Regex.Replace(IndentedCode, "\n", "\r\n", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);

            sbIndented.Clear();

        }

        /// <summary>
        /// Alinear asignaciones o definicion de variables
        /// Sirve asi:
        /// 
        ///   nContador NUMBER;
        ///   vNombre VARCHAR2(100):='xxx';
        ///   vDireccion VARCHAR2(200) := 'direccion';
        ///    
        ///   nContador  NUMBER;
        ///   vNombre    VARCHAR2(100) := 'xxx';
        ///   vDireccion VARCHAR2(200) := 'direccion';
        ///   
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tsbAlinear_Click(object sender, EventArgs e)
        {
            // Determinar tamano maximo de variable antes de asignacion o definicion de tipo
            //
        }
    }
}
