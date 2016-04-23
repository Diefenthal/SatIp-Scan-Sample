using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SatIp
{
    public class IniMapping
    {
        #region Private Fields
        private string _name;
        private string _file;
        #endregion

        #region Constructor
        public IniMapping(string name, string file)
        {
            _name = name;
            _file = file;
        }
        #endregion

        #region Properties
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        public string File
        {
            get { return _file; }
            set { _file = value; }
        }
        #endregion

        public override string ToString()
        {
            return Name;
        }
    }
}
