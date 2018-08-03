using System;
using System.Collections.Generic;
using System.Text;

namespace PLSQLIndent
{
    /// <summary>
    /// Will hold the statement info on the stack
    /// </summary>
    class Statement
    {
        /// <summary>
        /// Original text of this piece of code
        /// </summary>
        public string Text {get;set;}
        /// <summary>
        /// Indentation specified for this structure
        /// </summary>
        public int Indentation {get;set;}
        /// <summary>
        /// Have we found already the structure delimiter?
        /// </summary>
        public bool DelimiterFound {get;set;}
        /// <summary>
        /// Structure type
        /// </summary>
        public string Type {get;set;}
        /// <summary>
        /// Name of the procedure or function (used for closing end)
        /// </summary>
        public string ProcName { get; set; }
        /// <summary>
        /// Does this structure has a BEGIN statement?
        /// </summary>
        public bool HasBegin { get; set; }
        /// <summary>
        /// Have we found the begin statement for this stucture ?
        /// </summary>
        public bool BeginFound { get; set; }
        /// <summary>
        /// Does the SELECT statement ends with a parenthesis (Subselect)
        /// </summary>
        public bool SelectEndsWithParenthesis { get; set; }
        /// <summary>
        /// Position where the SELECT keyword starts
        /// </summary>
        public int MainKWordPos { get; set; }
        /// <summary>
        /// Position where the procedure name starts
        /// </summary>
        public int ProcedureWordPos { get; set; }
        /// <summary>
        /// Does this segment has a name?
        /// </summary>
        public bool HasAName { get; set; }
        /// <summary>
        /// For Begin blocks, has the exception section been reached
        /// </summary>
        public bool ExceptionFound { get; set; }
    }
}
