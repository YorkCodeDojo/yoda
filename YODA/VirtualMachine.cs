namespace SimpleInstructionMachine;

public class VirtualMachine
{
    private static class Command
    {
        public const byte Halt = 0;
        public const byte LoadFromFile = 1;
        public const byte SaveToFile = 2;
        public const byte Write = 3;
        public const byte Add = 4;
        public const byte Inc = 5;
        public const byte Dec = 6;
        public const byte Nop = 7;
        public const byte JumpIfZero = 8;
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
            var operand = _memory[_instructionPointer];
            try
            {
                switch (operand)
                {
                    case Command.Halt:
                        halted = true;
                        break;
                    case Command.LoadFromFile:
                        await LoadFromFile();
                        break;
                    case Command.SaveToFile:
                        await SaveToFile();
                        break;
                    case Command.Write:
                        Write();
                        break;
                    case Command.Inc:
                        Inc();
                        break;
                    case Command.Dec:
                        Dec();
                        break;
                    case Command.Nop:
                        Nop();
                        break;
                    case Command.JumpIfZero:
                        JumpIfZero();
                        break;
                    case Command.Add:
                        throw new Exception("Due to lack of time this method has not been implemented");
                    default:
                        throw new Exception("Unknown command " + operand);
                }
            }
            catch (Exception e)
            {
                await Console.Error.WriteLineAsync($"Your program has crashed,  things aren't looking to good for the space craft.\n");
                await Console.Error.WriteLineAsync($"{e.Message}");
                await Console.Error.WriteLineAsync($"Instruction Pointer: {_instructionPointer}");
                await Console.Error.WriteLineAsync($"Operand: {operand}\n");
                await File.WriteAllBytesAsync("crash_dump", _memory);
                await Console.Error.WriteLineAsync($"A crash dump containing all the memory has been written to : ./crash_dump");
                return;
            }
        }
    }

    private async Task Initialise()
    {
        Array.Fill(_memory,(byte)0);
        _instructionPointer = 0;

        var filename = FilenameFromFileNumber(0);
        if (File.Exists(filename))
        {
            var fileContents = await File.ReadAllBytesAsync(filename);
            if (fileContents.Length > _memory.Length)
                throw new Exception($"The initial file is too large. It is {fileContents.Length} bytes long,  which exceeds the maximum allowed of {_memory.Length} bytes");

            fileContents.CopyTo(_memory, 0);
            
            Console.WriteLine($"\nMemory has been initialised using {filename}.");
        }
        else
        {
            Console.WriteLine("\nNo memory initialisation file found.");
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
        var fileNumber = ValueAt(_instructionPointer + 1);
        var targetLocation = _memory[_instructionPointer + 2];
        
        var fileContents = await File.ReadAllBytesAsync(FilenameFromFileNumber(fileNumber));
        if (fileContents.Length + targetLocation > _memory.Length)
            throw new Exception("File too large");
        fileContents.CopyTo(_memory, targetLocation );
        
        _instructionPointer += 3;
    }

    /// <summary>
    /// SaveToFile [FileNumber] SourceLocation [Length]
    /// </summary>
    private async Task SaveToFile()
    {
        var fileNumber = ValueAt(_instructionPointer + 1);
        var sourceLocation = _memory[_instructionPointer + 2];
        var length = ValueAt(_instructionPointer + 3);
        
        await File.WriteAllBytesAsync(FilenameFromFileNumber(fileNumber), _memory[sourceLocation..(sourceLocation+length)]);
        
        _instructionPointer += 4;
    }
    
    
    /// <summary>
    /// Write [Location] Value
    /// </summary>
    private void Write()
    {
        var location = ValueAt(_instructionPointer + 1);
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
        var value = ValueAt(_instructionPointer + 1);
        var address = _memory[_instructionPointer + 2];

        if (value == 0)
            _instructionPointer = address;
        else
            _instructionPointer += 3;
    }

    private byte ValueAt(int location)
    {
        if (location >= _memory.Length)
            throw new Exception("Illegal memory location " + location);

        var reference = _memory[location];
        if (reference >= _memory.Length)
            throw new Exception("Illegal de-referenced memory location " + location);
        
        return  _memory[reference];
    } 
}