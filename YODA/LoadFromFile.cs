namespace SimpleInstructionMachine;

/// <summary>
/// LoadFromFile FileNumber SourceLocation Length
/// </summary>
public partial class VirtualMachine
{
    private async Task LoadFromFile(int opCode)
    {
        var fileNumber = Read(_instructionPointer + 1, opCode, 0);
        var targetLocation = Read(_instructionPointer + 2 , opCode, 1);
        
        var fileContents = await File.ReadAllBytesAsync(FilenameFromFileNumber(fileNumber));
        if (fileContents.Length + targetLocation > _memory.Length)
            throw new Exception("File too large");
        fileContents.CopyTo(_memory, targetLocation );
        
        _instructionPointer += 3;
    }
    
}