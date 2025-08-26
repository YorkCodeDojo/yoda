namespace Davids_Solutions;

public static class OpCode
{
        public const byte Halt = 0b0000_0000;   // 00
        public const byte Wait = 0b0000_0001;   // 01
        
        public const byte SaveToFileMMM = 0b0001_0000;  //16
        public const byte SaveToFileMMI = 0b0001_0001;  //17
        public const byte SaveToFileMIM = 0b0001_0010;  //18
        public const byte SaveToFileMII = 0b0001_0011;  //19
        public const byte SaveToFileIMM = 0b0001_0100;  //20
        public const byte SaveToFileIMI = 0b0001_0101;  //21
        public const byte SaveToFileIIM = 0b0001_0110;  //22
        public const byte SaveToFileIII = 0b0001_0111;  //23
        
        public const byte LoadFromFileMM = 0b0010_0000;  //32
        public const byte LoadFromFileMI = 0b0010_0001;  //33
        public const byte LoadFromFileIM = 0b0010_0010;  //34
        public const byte LoadFromFileII = 0b0010_0011;  //35
        
        public const byte WriteMM = 0b0011_0000;  //48
        public const byte WriteMI = 0b0011_0001;  //49
        public const byte WriteIM = 0b0011_0010;  //50
        public const byte WriteII = 0b0011_0011;  //51
        
        public const byte AddMMD = 0b0100_0000;  //64
        public const byte AddMMI = 0b0100_0001;  //65
        public const byte AddMID = 0b0100_0010;  //66
        public const byte AddMII = 0b0100_0011;  //67
        public const byte AddIMD = 0b0100_0100;  //68
        public const byte AddIMI = 0b0100_0101;  //69
        public const byte AddIID = 0b0100_0110;  //70
        public const byte AddIII = 0b0100_0111;  //71

        public const byte SubMMD = 0b0101_0000;  //80
        public const byte SubMMI = 0b0101_0001;  //81
        public const byte SubMID = 0b0101_0010;  //82
        public const byte SubMII = 0b0101_0011;  //83
        public const byte SubIMD = 0b0101_0100;  //84
        public const byte SubIMI = 0b0101_0101;  //85
        public const byte SubIID = 0b0101_0110;  //86
        public const byte SubIII = 0b0101_0111;  //87
        
        public const byte IncM = 0b0110_0000;  //96
        public const byte IncI = 0b0110_0001;  //97
        
        public const byte DecM = 0b0111_0000;  //112
        public const byte DecI = 0b0111_0001;  //113
        
        public const byte Nop = 0b1000_0000; //128

        public const byte JumpIfZeroMM = 0b1001_0000;  //144
        public const byte JumpIfZeroMI = 0b1001_0001;  //145
        public const byte JumpIfZeroIM = 0b1001_0010;  //146
        public const byte JumpIfZeroII = 0b1001_0011;  //147
        
        public const byte CopyMM = 0b1010_0000;  //160
        public const byte CopyMI = 0b1010_0001;  //161
        public const byte CopyIM = 0b1010_0010;  //162
        public const byte CopyII = 0b1010_0011;  //163
}

public static class Screen
{
        public const byte LCD_0 = 0xF8;
        public const byte LCD_1 = 0xF9;
        public const byte LCD_2 = 0xFA;
        public const byte LCD_3 = 0xFB;
        public const byte LCD_4 = 0xFC;

        public const byte ControlFlags = 0xFD;
}