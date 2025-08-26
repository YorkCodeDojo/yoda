// ReSharper disable InconsistentNaming
namespace SimpleInstructionMachine;

public class VirtualMachine(bool Debug)
{
    private static class Mask
    {
        public const byte Halt = 0b0000;
        public const byte SaveToFile = 0b0001;
        public const byte LoadFromFile = 0b0010;
        public const byte Write = 0b0011;
        public const byte Add = 0b0100;
        public const byte Sub = 0b0101;
        public const byte Inc = 0b0110;
        public const byte Dec = 0b0111;
        public const byte Nop = 0b1000;
        public const byte JumpIfZero = 0b1001;
        public const byte Copy = 0b1010;
    }

    private static class OpCode
    {
        //1 means immediate rather than memory (0)
        
        //Values can either be
        //      M - Memory (0) or Immediate (1)
        //      D - Indirect (0) or Direct (1) 
        
        public const byte Halt = 0b0000_0000;   // 00
        public const byte Wait = 0b0000_0001;   // 00

        
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
    
    private const byte LCD_0 = 0xF8;
    private const byte LCD_1 = 0xF9;
    private const byte LCD_2 = 0xFA;
    private const byte LCD_3 = 0xFB;
    private const byte LCD_4 = 0xFC;

    private const byte ControlFlags = 0xFD;
        
    
    private readonly byte[] _memory = new byte[1 + byte.MaxValue];

    private int _instructionPointer = 0;

    private string _folder = ".";
    
    public async Task Run(string folderPath)
    {
        
        _folder = folderPath;
        
        await Initialise();

        var halted = false;
        while (!halted)
        {
            var opCode = _memory[_instructionPointer];
            try
            {
                switch (opCode >> 4)
                {
                    case 0:
                        switch (opCode)
                        {
                            case OpCode.Halt:
                            {
                                halted = true;
                                continue;
                            }
                            case OpCode.Wait:
                            {
                                await Wait();
                                continue;
                            }
                            default:
                                throw new Exception("Unknown command " + opCode);
                        }

                    case Mask.SaveToFile:
                        await SaveToFile(opCode);
                        continue;
                    case Mask.LoadFromFile:
                        await LoadFromFile(opCode);
                        continue;
                    case Mask.Write:
                        Write(opCode);
                        continue;
                    case Mask.Add:
                        Add(opCode);
                        continue;
                    case Mask.Sub:
                        throw new Exception("Due to lack of time this method has not been implemented");
                    case Mask.Inc:
                        Inc(opCode);
                        continue;
                    case Mask.Dec:
                        Dec(opCode);
                        continue;
                    case Mask.Nop:
                        Nop();
                        break;
                    case Mask.JumpIfZero:
                        JumpIfZero(opCode);
                        break;
                    case Mask.Copy:
                        Copy(opCode);
                        break;
                    default:
                        throw new Exception("Unknown command " + opCode);
                }
                
                Console.WriteLine("\n\nProgram completed successfully");
                
            }
            catch (Exception e)
            {
                await Console.Error.WriteLineAsync($"Your program has crashed,  things aren't looking to good for the space craft.\n");
                await Console.Error.WriteLineAsync($"{e.Message}");
                await Console.Error.WriteLineAsync($"Instruction Pointer: {_instructionPointer}");
                await Console.Error.WriteLineAsync($"Opcode: {opCode}\n");
                
                // Dump as bytes
                await File.WriteAllBytesAsync("crash_dump", _memory);

                // Dump as text
                await using var textFile = File.CreateText("crash_dump.txt");
                for (var i = 0; i < _memory.Length; i++)
                {
                    if (i == _instructionPointer)
                        await textFile.WriteLineAsync($"{i:X2}   {_memory[i]}    <---- INSTRUCTION POINTER");
                    else
                        await textFile.WriteLineAsync($"{i:X2}   {_memory[i]}");
                }

                await textFile.FlushAsync();                    
                
                await Console.Error.WriteLineAsync($"A crash dump containing all the memory has been written to : crash_dump and crash_dump.txt");
                return;
            }
        }
    }

    private async Task Initialise()
    {
        Array.Fill(_memory,(byte)0);
        _instructionPointer = 0;

        var filename = Path.Combine(_folder, "boot");
        if (File.Exists(filename))
        {
            var fileContents = await File.ReadAllBytesAsync(filename);
            if (fileContents.Length > _memory.Length)
                throw new Exception($"The boot file is too large. It is {fileContents.Length} bytes long,  which exceeds the maximum allowed of {_memory.Length} bytes");

            fileContents.CopyTo(_memory, 0);
            
            Console.WriteLine($"\nMemory has been initialised using the boot file ({filename}).");
        }
        else
        {
            Console.WriteLine("\nNo boot file found.");
        }
    }

    private string FilenameFromFileNumber(byte fileNumber)
    {
        return fileNumber switch
        {
            < 8 => Path.Combine(_folder, $"{fileNumber}"),
            < 16 => Path.Combine(_folder, $"{fileNumber}.txt"),
            _ => throw new Exception($"Unknown file {fileNumber}.  Binary files are between 0 and 7.   Text files are between 8 and 15")
        };
    }
    
    /// <summary>
    /// SaveToFile FileNumber SourceLocation Length
    /// </summary>
    private async Task SaveToFile(int opCode)
    {
        var fileNumber = Read(_instructionPointer + 1, opCode, 2);
        var sourceLocation = Read(_instructionPointer + 2 , opCode, 1);
        var length = Read(_instructionPointer + 3 , opCode, 0);
        
        await File.WriteAllBytesAsync(FilenameFromFileNumber(fileNumber), _memory[sourceLocation..(sourceLocation+length)]);
        
        _instructionPointer += 4;
    }
    
    
    /// <summary>
    /// LoadFromFile FileNumber SourceLocation Length
    /// </summary>
    private async Task LoadFromFile(int opCode)
    {
        var fileNumber = Read(_instructionPointer + 1, opCode, 1);
        var targetLocation = Read(_instructionPointer + 2 , opCode, 0);
        
        var fileContents = await File.ReadAllBytesAsync(FilenameFromFileNumber(fileNumber));
        if (fileContents.Length + targetLocation > _memory.Length)
            throw new Exception("File too large");
        fileContents.CopyTo(_memory, targetLocation );
        
        _instructionPointer += 3;
    }
    
    
    
    /// <summary>
    /// Write [Location] Value
    /// </summary>
    private void Write(int opCode)
    {
        var location = Read(_instructionPointer + 1, opCode, 1);
        var value = Read(_instructionPointer + 2 , opCode, 0);

        if (Debug) Console.WriteLine($"Write {value} into {location:X2}");
        
        if (location == ControlFlags)
        {
           var diffs = _memory[location] ^ value;
           if ((diffs & 1) == 1)
           {
               //bit 0 has changed, refresh the LCD display
               Console.Write(ToChar(_memory[LCD_0]));
               Console.Write(ToChar(_memory[LCD_1]));
               Console.Write(ToChar(_memory[LCD_2]));
               Console.Write(ToChar(_memory[LCD_3]));
               Console.WriteLine(ToChar(_memory[LCD_4]));

               char ToChar(byte b)
               {
                   if (b == 0x00)
                       return ' ';
                   else
                       return (char)b;
               }

           }
        }
        
        _memory[location] = value;
        _instructionPointer += 3;
    }
    
    /// <summary>
    /// Add LHS RHS Total
    /// </summary>
    private void Add(int opCode)
    {
        var lhs = Read(_instructionPointer + 1, opCode, 2);
        var rhs = Read(_instructionPointer + 2 , opCode, 1);
        var location = Read(_instructionPointer + 3 , opCode, 0);
        
        _memory[location] = (byte)(lhs+rhs); // Can overflow
        _instructionPointer += 4;
    }
    
    
    /// <summary>
    /// Inc [Location]
    /// </summary>
    private void Inc(int opCode)
    {
        var location = Read(_instructionPointer + 1 , opCode, 0);
        _memory[location]++;
        _instructionPointer += 2;
    }
    
    /// <summary>
    /// Dec [Location]
    /// </summary>
    private void Dec(int opCode)
    {
        var location = Read(_instructionPointer + 1 , opCode, 0);
        
        if (Debug) Console.WriteLine($"DEC: Decreasing value in {location:x8} from {_memory[location]} to {(_memory[location])-1}");
        
        _memory[location]--;
        _instructionPointer += 2;
    }
    
    /// <summary>
    /// Nop
    /// </summary>
    private void Nop()
    {
        _instructionPointer++;
    }

    private async Task Wait()
    {
        if (Debug) Console.WriteLine("Wait");
        await Task.Delay(100);
        _instructionPointer++;
    }

    /// <summary>
    /// JumoIfZero ValueToCheck Address
    /// </summary>
    private void JumpIfZero(int opCode)
    {
        var valueToCheck = Read(_instructionPointer + 1 , opCode, 1);
        var locationToJumpTo = Read(_instructionPointer + 2 , opCode, 0);

        if (Debug) Console.WriteLine($"JumpIfZero ({opCode:b8}) - jump to {locationToJumpTo:X2} if {valueToCheck} is 0");
         
        if (valueToCheck == 0)
            _instructionPointer = locationToJumpTo;
        else
            _instructionPointer += 3;
    }

    /// <summary>
    /// Copy [SourceLocation] [TargetLocation]
    /// </summary>
    private void Copy(int opCode)
    {
        var value = Read(_instructionPointer + 1 , opCode, 1);
        var targetAddress = Read(_instructionPointer + 2 , opCode, 0);

        _memory[targetAddress] = value;
        
        _instructionPointer += 3;
    }

    private byte Read(int location,int opCode, int mask)
    {
        //isSet means immediate rather than memory
        var isSet = (((opCode & 0b0000_1111) >> mask) & 1) == 1;
        // Console.WriteLine($"{opCode:b8} {(opCode & 0b0000_1111):b8} mask {mask} {(((opCode & 0b0000_1111) >> mask) & 1 ):b8} {isSet} ");
        
        if (location >= _memory.Length)
            throw new Exception("Illegal memory location " + location);

        if (isSet)
        {
            // Console.WriteLine($"{opCode:b8}, {mask}, {_memory[location]:x8}");  
            return _memory[location];
        }

        var reference = _memory[location];
        if (reference >= _memory.Length)
            throw new Exception("Illegal de-referenced memory location " + location);
        
        // Console.WriteLine($"{opCode:b8}, {mask}, {reference:x8}, {_memory[reference]}");

        
        return  _memory[reference];
    } 
}