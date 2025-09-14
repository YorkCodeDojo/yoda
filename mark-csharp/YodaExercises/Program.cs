using System.Reflection;
using SimpleInstructionMachine;

namespace YodaExercises;

internal static class Program
{
	private static string _filePath = null!;

	internal static void Main(string[] args)
	{
		//	Get individual path parts to the assembly and where the assembly is in the list
		var pathParts = (Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? "")
			.Split(Path.DirectorySeparatorChar)
			.ToList();
		var assemblyIndex = pathParts.IndexOf(nameof(YodaExercises));
		//	Reassemble the paths up to the point of the assembly name
		var basePath = string.Join(Path.DirectorySeparatorChar, pathParts[..assemblyIndex]);
		//	Append Files to the path - this is the location of the boot file area
		_filePath = Path.Combine(basePath, "Files");

		//	Make sure the Files folder exists
		if (!Directory.Exists(_filePath))
			Directory.CreateDirectory(_filePath);

		// Exercise1();
		// Exercise2();
		// Exercise3();
		// Exercise4();
		// Exercise5();
		Exercise6();
	}

	private static void WriteFile(string name, byte[]? contents)
	{
		//	programs can only occupy 256 bytes - any more will break the sim!
		var fileLength = 1 + byte.MaxValue;
		ArgumentOutOfRangeException.ThrowIfGreaterThan(contents?.Length ?? 0, fileLength, nameof(contents));

		//	Create the 256 byte buffer for the file
		var buffer = new byte[fileLength];
		//	Copy any contents we have been sent to the buffer
		if (contents?.Length > 0)
			contents.CopyTo(buffer, 0);
		//	Dump the buffer to the specified file, ensuring we only retain the name of the file and place it correctly
		File.WriteAllBytes(Path.Combine(_filePath, Path.GetFileName(name)), buffer);
	}

	private static void MakeFileBoot(string name)
	{
		var fileName = Path.Combine(_filePath, Path.GetFileName(name));
		if (!File.Exists(fileName))
			throw new FileNotFoundException($"File {fileName} not found.");
		File.Copy(fileName, Path.Combine(_filePath, "boot"), true);
	}

	private static void RunVm(string program)
	{
		MakeFileBoot(program);

		var vm = new VirtualMachine(true);
		vm.Run(_filePath).Wait();
	}

	private static void Exercise1()
	{
		WriteFile("ex1", null);
		RunVm("ex1");
	}

	private static void Exercise2()
	{
		WriteFile("ex2", [0xFF]);
		RunVm("ex2");
	}

	private static void Exercise3()
	{
		//	Two parts to this... the program itself and its data
		//	Program writes to file 0, from address 0x20 for 5 bytes
		byte[] program = [OpCode.SaveToFileIII, 0, 0x20, 5];
		//	Data to be stored at byte 0x20
		byte[] data = [5, 4, 3, 2, 1];

		//	Copy program and data to a temp buffer to pass to the VM Runner
		var buffer = new byte[64];
		program.CopyTo(buffer, 0);
		data.CopyTo(buffer, 0x20);
		WriteFile("ex3", buffer);
		RunVm("ex3");
	}

	private static void Exercise4()
	{
		byte[] message = "5 4 3 2 1 LIFTOFF".ToCharArray().Select(x => (byte)x).ToArray();
		byte[] program = [OpCode.SaveToFileIII, 9, 0x20, (byte)message.Length];

		//	Copy program and message to a temp buffer to pass to the VM Runner
		var buffer = new byte[64];
		program.CopyTo(buffer, 0);
		message.CopyTo(buffer, 0x20);
		WriteFile("ex4", buffer);
		RunVm("ex4");
	}

	private static void RefuelFile1(byte refuelAmount)
	{
		byte[] program = [OpCode.SaveToFileIII, 1, 0x20, 1];
		var buffer = new byte[64];
		program.CopyTo(buffer, 0);
		buffer[0x20] = refuelAmount;
		WriteFile("refuelFile1", buffer);
		RunVm("refuelFile1");
	}

	private static void Exercise5()
	{
		RefuelFile1(123);
		byte[] program =
		[
			OpCode.LoadFromFileII, 1, 0x80,
			OpCode.DecI, 0x80,
			OpCode.SaveToFileIII, 1, 0x80, 1
		];
		var buffer = new byte[64];
		program.CopyTo(buffer, 0);
		WriteFile("ex5", buffer);
		RunVm("ex5");
	}

	private static void Exercise6()
	{
		byte[] message = "54321".ToCharArray().Select(x => (byte)x).ToArray();
		byte[] program =
		[
			OpCode.WriteIM, KnownMemory.LCD_0, 0x80,
			OpCode.WriteII, KnownMemory.ControlFlags, 0x01,
			OpCode.WriteII, KnownMemory.ControlFlags, 0x00,
			OpCode.Wait,
			OpCode.WriteIM, KnownMemory.LCD_0, 0x81,
			OpCode.WriteII, KnownMemory.ControlFlags, 0x01,
			OpCode.WriteII, KnownMemory.ControlFlags, 0x00,
			OpCode.Wait,
			OpCode.WriteIM, KnownMemory.LCD_0, 0x82,
			OpCode.WriteII, KnownMemory.ControlFlags, 0x01,
			OpCode.WriteII, KnownMemory.ControlFlags, 0x00,
			OpCode.Wait,
			OpCode.WriteIM, KnownMemory.LCD_0, 0x83,
			OpCode.WriteII, KnownMemory.ControlFlags, 0x01,
			OpCode.WriteII, KnownMemory.ControlFlags, 0x00,
			OpCode.Wait,
			OpCode.WriteIM, KnownMemory.LCD_0, 0x84,
			OpCode.WriteII, KnownMemory.ControlFlags, 0x01,
			OpCode.WriteII, KnownMemory.ControlFlags, 0x00,
			OpCode.Wait,
		];

		//	Copy program and message to a temp buffer to pass to the VM Runner
		var buffer = new byte[1 + byte.MaxValue];
		program.CopyTo(buffer, 0);
		message.CopyTo(buffer, 0x80);

		WriteFile("ex6", buffer);
		RunVm("ex6");
	}
}