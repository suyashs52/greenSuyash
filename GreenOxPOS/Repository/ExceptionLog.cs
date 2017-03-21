using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GreenOxPOS.Repository
{
    public class ExceptionLog
    {
        private List<ExceptionFields> _ExceptionDetail = new List<ExceptionFields>();
        public List<ExceptionFields> ExceptionDetail { get { return _ExceptionDetail; } set { _ExceptionDetail = value; } }
    }

    public class ExceptionFields
    {
        public string Name { get; set; }
        public string Message { get; set; }
        public string Stack { get; set; }
        public string Source { get; set; }
        public string Target { get; set; }
        public string Form { get; set; }
        public string Function { get; set; }
        public string OccuredOn { get; set; }
    }
}