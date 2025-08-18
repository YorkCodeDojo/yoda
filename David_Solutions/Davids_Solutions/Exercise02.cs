namespace Davids_Solutions;

public class Exercise02
{
    public static async Task Do()
    {
        // Locations of variables / constants
        const byte pFileNumber = 100;
        const byte pLengthToWrite = 101;
        const byte pNumberToIncrement = 102;

        var commands = new byte[]
        {
            // Load file. LoadFromFile [FileNumber] TargetLocation
            Command.LoadFromFile, pFileNumber, pNumberToIncrement,

            // Increment
            Command.Inc, pNumberToIncrement,

            // Save. [FileNumber] SourceLocation [Length]
            Command.SaveToFile, pFileNumber, pNumberToIncrement, pLengthToWrite
        };
        var fixedSize = new byte[1024];
        commands.CopyTo(fixedSize, 0);


        // Initial values

        // Write the file number into memory location 100
        fixedSize[pFileNumber] = 1;

        // Number of bytes to write
        fixedSize[pLengthToWrite] = 1;

        await File.WriteAllBytesAsync("/Users/davidbetteridge/SimpleInstructionMachine/Files/0", fixedSize);


        // Input file
        await File.WriteAllBytesAsync("/Users/davidbetteridge/SimpleInstructionMachine/Files/1", [42]);
    }
}