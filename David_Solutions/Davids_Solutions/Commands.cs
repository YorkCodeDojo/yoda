namespace Davids_Solutions;

public static class OpCode
{
    public const byte Halt = 0b0000_0000;   // 00
        
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
        
    public const byte Write = 0xA3;
    public const byte Add = 0xA4;
    public const byte Inc = 0xA5;
    public const byte Dec = 0xA6;
    public const byte Nop = 0xA7;
    public const byte JumpIfZero = 0xA8;
    public const byte Copy = 0xA9;
}
