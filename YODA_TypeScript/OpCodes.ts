// OpCode.ts
export const OpCode = {
    // Misc (no operands)
    Halt: 0b0000_0000, // 0
    Wait: 0b0000_0001, // 1
    Ret:  0b0000_0011, // 3
    Nop:  0b0000_0100, // 4
    Sif:  0b0000_0101, // 5
    Cif:  0b0000_0110, // 6

    // SaveToFile
    SaveToFileMMM: 0b0001_0000, //16
    SaveToFileMMI: 0b0001_0001, //17
    SaveToFileMIM: 0b0001_0010, //18
    SaveToFileMII: 0b0001_0011, //19
    SaveToFileIMM: 0b0001_0100, //20
    SaveToFileIMI: 0b0001_0101, //21
    SaveToFileIIM: 0b0001_0110, //22
    SaveToFileIII: 0b0001_0111, //23

    // LoadFromFile
    LoadFromFileMM: 0b0010_0000, //32
    LoadFromFileMI: 0b0010_0001, //33
    LoadFromFileIM: 0b0010_0010, //34
    LoadFromFileII: 0b0010_0011, //35

    // Write
    WriteMM: 0b0011_0000, //48
    WriteMI: 0b0011_0001, //49
    WriteIM: 0b0011_0010, //50
    WriteII: 0b0011_0011, //51

    // Add
    AddMMD: 0b0100_0000, //64
    AddMMI: 0b0100_0001, //65
    AddMID: 0b0100_0010, //66
    AddMII: 0b0100_0011, //67
    AddIMD: 0b0100_0100, //68
    AddIMI: 0b0100_0101, //69
    AddIID: 0b0100_0110, //70
    AddIII: 0b0100_0111, //71

    // Sub
    SubMMD: 0b0101_0000, //80
    SubMMI: 0b0101_0001, //81
    SubMID: 0b0101_0010, //82
    SubMII: 0b0101_0011, //83
    SubIMD: 0b0101_0100, //84
    SubIMI: 0b0101_0101, //85
    SubIID: 0b0101_0110, //86
    SubIII: 0b0101_0111, //87

    // Inc / Dec
    IncM: 0b0110_0000, //96
    IncI: 0b0110_0001, //97
    DecM: 0b0111_0000, //112
    DecI: 0b0111_0001, //113

    // Jumps
    JumpIfZeroMM: 0b1000_0000, //128
    JumpIfZeroMI: 0b1000_0001, //129
    JumpIfZeroIM: 0b1000_0010, //130
    JumpIfZeroII: 0b1000_0011, //131

    JumpWithReturnM: 0b1001_0000, //144
    JumpWithReturnI: 0b1001_0001  //145
} as const;
