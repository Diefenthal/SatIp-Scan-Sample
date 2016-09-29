using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SatIp.Analyzer
{
    public class ProgramAssociation
    {
        public int Number;
        public int Pid;
    }
    /// <summary>
    /// 
    /// </summary>
    public class ProgramAssociationTable
    {
        public int TableId;
        public int SyntaxIndicator;
        public int Length;
        public int TransportStreamID;
        public int VersionNumber;
        public bool CurrentNextIndicator;
        public int SectionNumber;
        public int LastSectionNumber;
        public int ProgramCount;
        public List<ProgramAssociation> Programs;

        public ProgramAssociationTable()
        {
            
        }
        /// <summary>
        /// TransportStreamID An List of ServiceID with PMTPID
        /// </summary>
        /// <param name="start"></param>
        /// <param name="point"></param>
        /// <param name="buffer"></param>
        public static ProgramAssociationTable Decode(bool start, int point, byte[] buffer)
        {
            var pat = new ProgramAssociationTable();
            pat.TableId = buffer[point + 1];
            pat.SyntaxIndicator = (buffer[point + 1] >> 7) & 1;
            pat.Length = ((buffer[point + 2] & 15) * 0x100) + buffer[point + 3];
            pat.TransportStreamID = (buffer[point + 4] * 0x100) + buffer[point + 5];
            pat.VersionNumber = buffer[point + 8];
            pat.CurrentNextIndicator = (buffer[point + 5] & 1) != 0;
            pat.LastSectionNumber = buffer[point + 8];
            pat.SectionNumber = buffer[point + 7];
            pat.ProgramCount = (int)Math.Round((double)(((double)(pat.Length - 9)) / 4.0));
            int index = point + 9;
            int num3 = 0;
            pat.Programs = new List<ProgramAssociation>();
            while ((((num3 * 4) + 9) < pat.Length) & ((index + 3) < 0xbc))
            {   
                ProgramAssociation program=null;
                if ((buffer[index] == 0) & (buffer[index + 1] == 0))
                {
                    //ProgramCount = ((buffer[index + 2] & 0x1f) * 0x100) + buffer[index + 3];
                    
                }
                else
                {
                    program= new ProgramAssociation
                    {
                        Number= (((buffer[index] * 0x100) + buffer[index + 1])),
                        Pid = (((buffer[index + 2] & 0x1f) * 0x100) + buffer[index + 3]) 
                    };
                    pat.Programs.Add(program);
                    //.ToString(); 
                    //" for program_map_PID=" + ((((buffer[index + 2] & 0x1f) * 0x100) + buffer[index + 3])).ToString());
                }
                index += 4;
                num3++;
            }
            return pat;            
        }
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("Program Association Table .\n");
            sb.AppendFormat("Table ID : {0} .\n",TableId);
            sb.AppendFormat("Syntax Indicator : {0} .\n", SyntaxIndicator);
            sb.AppendFormat("Length : {0} .\n", Length);
            sb.AppendFormat("Transport Stream ID : {0} .\n", TransportStreamID);
            sb.AppendFormat("Version Number : {0} .\n", VersionNumber);
            sb.AppendFormat("Current / Next Indicator : {0} .\n", CurrentNextIndicator);
            sb.AppendFormat("Section Number : {0} .\n", SectionNumber);
            sb.AppendFormat("Last Section Number : {0} .\n", LastSectionNumber);

            foreach (var program in Programs)
            {
                sb.AppendFormat("Program Number : {0} - Program ID : {1}.\n", program.Number,program.Pid);
            }
            sb.AppendFormat(".\n");
            return sb.ToString();
        }
    }
}
