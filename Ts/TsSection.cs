
namespace SatIp
{
    public class TsSection
    {
        public int BufferPos;
        public byte[] Data = new byte[MAX_SECTION_LENGTH * 5];
        public int last_section_number;
        public static int MAX_SECTION_LENGTH = 0x10cc;
        public int section_length;
        public int section_number;
        public int section_syntax_indicator;
        public int table_id;
        public int table_id_extension;
        public int version_number;

        public TsSection()
        {
            this.Reset();
        }

        public int CalcSectionLength(byte[] tsPacket, int start)
        {
            if (this.BufferPos < 3)
            {
                byte num = 0;
                byte num2 = 0;
                if (this.BufferPos == 1)
                {
                    num = tsPacket[start];
                    num2 = tsPacket[start + 1];
                }
                else if (this.BufferPos == 2)
                {
                    num = this.Data[1];
                    num2 = tsPacket[start];
                }
                this.section_length = ((num & 15) << 8) + num2;
            }
            else
            {
                this.section_length = ((this.Data[1] & 15) << 8) + this.Data[2];
            }
            return this.section_length;
        }

        public bool DecodeHeader()
        {
            if (this.BufferPos < 8)
            {
                return false;
            }
            this.table_id = this.Data[0];
            this.section_syntax_indicator = (this.Data[1] >> 7) & 1;
            if (this.section_length == -1)
            {
                this.section_length = ((this.Data[1] & 15) << 8) + this.Data[2];
            }
            this.table_id_extension = (this.Data[3] << 8) + this.Data[4];
            this.version_number = (this.Data[5] >> 1) & 0x1f;
            this.section_number = this.Data[6];
            this.section_syntax_indicator = (this.Data[1] >> 7) & 1;
            return true;
        }

        public void Reset()
        {
            this.table_id = -1;
            this.table_id_extension = -1;
            this.section_length = -1;
            this.section_number = -1;
            this.version_number = -1;
            this.section_syntax_indicator = -1;
            this.BufferPos = 0;
            for (int i = 0; i < this.Data.Length; i++)
            {
                this.Data[i] = 0xff;
            }
        }

        public bool SectionComplete()
        {
            if ((!this.DecodeHeader() && (this.BufferPos > this.section_length)) && (this.section_length > 0))
            {
                return true;
            }
            if (!this.DecodeHeader())
            {
                return false;
            }
            return (this.BufferPos >= this.section_length);
        }
    }
}
