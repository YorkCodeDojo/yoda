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
    
    public async Task Run()
    {
        Array.Fill(_memory,(byte)0);
        var fileContents = await File.ReadAllBytesAsync(FilenameFromFileNumber(0));
        if (fileContents.Length > _memory.Length)
            throw new Exception("File too large");
        fileContents.CopyTo(_memory, 0);
        _instructionPointer = 0;

        var halted = false;
        while (!halted)
        {
            var operand = _memory[_instructionPointer];
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
    }

    private static string FilenameFromFileNumber(byte fileNumber)
    {
        if (fileNumber < 8)
            return $"/Users/davidbetteridge/SimpleInstructionMachine/Files/{fileNumber}";
        if (fileNumber < 16)
            return $"/Users/davidbetteridge/SimpleInstructionMachine/Files/{fileNumber}.txt";
        throw new Exception("Unknown file " + fileNumber);
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