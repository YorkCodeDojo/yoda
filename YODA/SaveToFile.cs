namespace SimpleInstructionMachine;

/// <summary>
/// SaveToFile FileNumber SourceLocation Length
/// </summary>
public partial class VirtualMachine
{
    private async Task SaveToFile(int opCode)
    {
        var fileNumber = Read(_instructionPointer + 1, opCode, 0);
        var sourceLocation = Read(_instructionPointer + 2 , opCode, 1);
        var length = Read(_instructionPointer + 3 , opCode, 2);
        
        await File.WriteAllBytesAsync(FilenameFromFileNumber(fileNumber), _memory[sourceLocation..(sourceLocation+length)]);
        
        _instructionPointer += 4;
    }
    
    private byte Read(int location,int opCode, int mask)
    {
        //isSet means immediate rather than memory
        var isSet = ((opCode >> mask) & 1) == 1;
        
        if (location >= _memory.Length)
            throw new Exception("Illegal memory location " + location);

        if (isSet)
            return  _memory[location];
            
        var reference = _memory[location];
        if (reference >= _memory.Length)
            throw new Exception("Illegal de-referenced memory location " + location);
        
        return  _memory[reference];
    } 
    
}