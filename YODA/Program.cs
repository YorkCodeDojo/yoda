namespace SimpleInstructionMachine;

public static class Program
{
    public static async Task Main(string[] args)
    {
        var folder = ".";
        if (args.Length > 0)
            folder = args[0];
        
        Console.WriteLine("Starting YODA (+ Anti-gravity module) - Release Build 12x.11g-34");
        Console.WriteLine($"Folder Path: {folder}\n" );
        Console.WriteLine($"Connecting to Engine Control System.... SUCCESS!" );
        Console.WriteLine($"Connecting to Landing Control System.... SUCCESS!" );
        Console.WriteLine($"Connecting to Interplanetary Communication System.... SUCCESS!" );
        Console.WriteLine($"All systems are GO!" );
        
        var machine = new VirtualMachine();
        await machine.Run(folder);    
    }
    
}