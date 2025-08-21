// ReSharper disable InconsistentNaming
namespace SimpleInstructionMachine;

public partial class VirtualMachine
{
    private static class Mask
    {
        public const byte SaveToFile = 0b0001;
        public const byte LoadFromFile = 0b0010;
    }

    private static class OpCode
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
                    case Mask.SaveToFile:
                        await SaveToFile(opCode);
                        continue;
                    case Mask.LoadFromFile:
                        await LoadFromFile(opCode);
                        continue;
                    default:
                        switch (opCode)
                        {
                            case OpCode.Halt:
                                halted = true;
                                break;
                            case OpCode.Write:
                                Write();
                                break;
                            case OpCode.Inc:
                                Inc();
                                break;
                            case OpCode.Dec:
                                Dec();
                                break;
                            case OpCode.Nop:
                                Nop();
                                break;
                            case OpCode.JumpIfZero:
                                JumpIfZero();
                                break;
                            case OpCode.Copy:
                                Copy();
                                break;
                            case OpCode.Add:
                                throw new Exception("Due to lack of time this method has not been implemented");
                            default:
                                throw new Exception("Unknown command " + opCode);
                        }

                        break;
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
    /// LoadFromFile [FileNumber] TargetLocation
    /// </summary>
    private async Task LoadFromFile()
    {
        var fileNumber = Memory(_instructionPointer + 1);
        var targetLocation = _memory[_instructionPointer + 2];
        
        var fileContents = await File.ReadAllBytesAsync(FilenameFromFileNumber(fileNumber));
        if (fileContents.Length + targetLocation > _memory.Length)
            throw new Exception("File too large");
        fileContents.CopyTo(_memory, targetLocation );
        
        _instructionPointer += 3;
    }
    
    /// <summary>
    /// Write [Location] Value
    /// </summary>
    private void Write()
    {
        var location = Memory(_instructionPointer + 1);
        var value = _memory[_instructionPointer + 2];

        _memory[location] = value;
        _instructionPointer += 3;
    }
    
    /// <summary>
    /// Inc [Location]
    /// </summary>
    private void Inc()
    {
        var location = _memory[_instructionPointer + 1];
        _memory[location]++;
        _instructionPointer += 2;
    }
    
    /// <summary>
    /// Dec [Location]
    /// </summary>
    private void Dec()
    {
        var location = _memory[_instructionPointer + 1];
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

    /// <summary>
    /// JumoIfZero [Location] Address
    /// </summary>
    private void JumpIfZero()
    {
        var value = Memory(_instructionPointer + 1);
        var address = _memory[_instructionPointer + 2];

        if (value == 0)
            _instructionPointer = address;
        else
            _instructionPointer += 3;
    }

    /// <summary>
    /// Copy [SourceLocation] [TargetLocation]
    /// </summary>
    private void Copy()
    {
        var value = Memory(_instructionPointer + 1);
        var targetAddress = _memory[_instructionPointer + 2];

        _memory[targetAddress] = value;
        
        _instructionPointer += 3;
    }

    private byte Memory(int location)
    {
        if (location >= _memory.Length)
            throw new Exception("Illegal memory location " + location);

        var reference = _memory[location];
        if (reference >= _memory.Length)
            throw new Exception("Illegal de-referenced memory location " + location);
        
        return  _memory[reference];
    } 
    
    private byte Immediate(int location)
    {
        if (location >= _memory.Length)
            throw new Exception("Illegal memory location " + location);
        
        return  _memory[location];
    } 
}