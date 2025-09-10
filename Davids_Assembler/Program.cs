// Info about each command

using System.Globalization;

// Address modes,  V means Value or A means Address
var commandInfo = new Dictionary<string, CommandInfo>
{
   ["HALT"] = new () { OperandCount = 0, BaseOpcode = 0b0000_0000},
   ["WAIT"] = new () { OperandCount = 0, BaseOpcode = 0b0000_0001},
   ["RET"] = new () { OperandCount = 0, BaseOpcode = 0b0000_0011},
   ["NOP"] = new () { OperandCount = 0, BaseOpcode = 0b0000_0100},
   ["SIF"] = new () { OperandCount = 0, BaseOpcode = 0b0000_0101},
   ["CIF"] = new () { OperandCount = 0, BaseOpcode = 0b0000_0110},
   
   ["WRITE"] = new () { OperandCount = 2, BaseOpcode = 0b0011_0000},
   
   ["DEC"] = new () { OperandCount = 1, BaseOpcode = 0b0111_0000},
   ["INC"] = new () { OperandCount = 1, BaseOpcode = 0b0110_0000},

   ["JUMP"] = new () { OperandCount = 1, BaseOpcode = 0b1001_0000},
   ["JUMP_IF_ZERO"] = new () { OperandCount = 2, BaseOpcode = 0b1000_0000},
   
   ["ADD"] = new () { OperandCount = 3, BaseOpcode = 0b0100_0000},
   ["SUB"] = new () { OperandCount = 3, BaseOpcode = 0b0101_0000},
   
   ["SAVE"] = new () { OperandCount = 3, BaseOpcode = 0b0001_0000},
   ["LOAD"] = new () { OperandCount = 2, BaseOpcode = 0b0010_0000 }
};


var lines = File.ReadAllLines("/Users/davidbetteridge/SimpleInstructionMachine/Davids_Assembler/exercise_04.txt")
                .Select((t,i) => new SourceLine { LineNumber = i, Text = RemoveInLineComment(t)})
                .ToArray();

// Pass 1,  strip out blank lines and comments
lines = lines
         .Where(l => !string.IsNullOrWhiteSpace(l.Text))
         .Where(l => !l.Text.StartsWith(';') )
         .ToArray();

var lineNumber = 0;

var initialValues = new Dictionary<byte, byte>();
if (lines[lineNumber].Text == "[INITIAL]")
{
   lineNumber++;
   
   while (lineNumber < lines.Length && !lines[lineNumber].Text.StartsWith('['))
   {
      var parts = lines[lineNumber].Text.Split("=");
      if (parts.Length != 2)
      {
         Console.Error.WriteLine($"Line {lines[lineNumber].LineNumber}: Initial value {lines[lineNumber]} could not be parsed");
         return;
      }

      var key = parts[0].Trim();
      var value = parts[1].Trim();

      if (!initialValues.TryAdd(StringToByte(key), StringToByte(value)))
      {
         Console.Error.WriteLine($"Line {lines[lineNumber].LineNumber}: The initial value {key} has been defined multiple times");
         return;
      }

      lineNumber++;
   }
}

// Parse out the constants
var constants = new Dictionary<string, string>();
if (lines[lineNumber].Text == "[CONSTANTS]")
{
   lineNumber++;
   
   while (lineNumber < lines.Length && !lines[lineNumber].Text.StartsWith('['))
   {
      var parts = lines[lineNumber].Text.Split("=");
      if (parts.Length != 2)
      {
         Console.Error.WriteLine(
            $"Line {lines[lineNumber].LineNumber}: Constant {lines[lineNumber]} could not be parsed");
         return;
      }

      var key = parts[0].Trim();
      var value = parts[1].Trim();

      if (!constants.TryAdd(key, value))
      {
         Console.Error.WriteLine(
            $"Line {lines[lineNumber].LineNumber}: The constant {key} has been defined multiple times");
         return;
      }

      lineNumber++;
   }
}

// Pass 2,  work out the locations of the labels
var labels = new Dictionary<string, int>();
var commands = new List<CommandLine>();
var address = 0x00;

if (lines[lineNumber].Text != "[PROGRAM]")
{
   Console.Error.WriteLine($"Line {lines[lineNumber].LineNumber}: [PROGRAM not found]");
   return;
}

lineNumber++;

while (lineNumber < lines.Length)
{
   if (lines[lineNumber].Text.StartsWith(':'))
   {
      var label = lines[lineNumber].Text[1..];
      if (!labels.TryAdd(label, address))
      {
         Console.Error.WriteLine($"Line {lines[lineNumber].LineNumber}: The label {label} has been defined multiple times");
         return;
      }
   }
   else
   {
      // It's a command, so we need to increase the address by 1 + the number of operands
      var parts = lines[lineNumber].Text.Split(' ');
      var mnemonic = parts[0];
      if (!commandInfo.TryGetValue(mnemonic, out var info))
      {
         Console.Error.WriteLine($"Line {lines[lineNumber].LineNumber}: The mnemonic {mnemonic} does not exist");
         return;
      }

      if (parts.Length != info.OperandCount + 1)
      {
         Console.Error.WriteLine($"Line {lines[lineNumber].LineNumber}: The operand {mnemonic} needs {info.OperandCount} opcodes not {parts.Length-1}");
         return;
      }

      address += parts.Length;

      commands.Add(new CommandLine
      {
         LineNumber = lineNumber,
         Mnemonic = parts[0],
         Operands = parts[1..]
      });
   }
   
   lineNumber++;
}

// Pass 3,  replace the variables and labels
foreach (var command in commands)
{
   for (var i = 0; i < command.Operands.Length; i++)
   {
      foreach (var label in labels)
         command.Operands[i] = command.Operands[i].Replace(label.Key, "0x" + label.Value.ToString("X2"));
      
      foreach (var constant in constants)
         command.Operands[i] = command.Operands[i].Replace(constant.Key, constant.Value);
   }
}

// Pass 4, write the boot file
var contents = new List<byte>();
foreach (var command in commands)
{
   var info = commandInfo[command.Mnemonic];
   var opcode = info.BaseOpcode;

   for (var i = 0; i < command.Operands.Length; i++)
   {
      if (command.Operands[i].StartsWith("[["))
      {
         Console.Error.WriteLine($"Line {lines[command.LineNumber].LineNumber}: The mnemonic {command.Mnemonic} does not support indirect addressing");
         return;
      }
      // Choice between Immediate and Direct
      if (command.Operands[i].StartsWith('['))
      {
         // Direct addressing - 0 - no change
         command.Operands[i] = command.Operands[i].TrimStart('[').TrimEnd(']');
      }
      else
      {
         // Immediate - 1
         opcode += 1 << (command.Operands.Length - 1 - i);
      }         

      if (!command.Operands[i].StartsWith("0x"))
      {
         command.Operands[i] = "0x" + int.Parse(command.Operands[i], NumberStyles.None);
      }
   }

   Console.WriteLine(command.Mnemonic);
   Console.WriteLine("  0x" + opcode.ToString("X2"));
   foreach (var o in command.Operands)
   {
      Console.WriteLine("  " + o);   
   }
   
   contents.Add((byte)opcode);
   foreach (var o in command.Operands) 
      contents.Add(byte.Parse(o[2..], NumberStyles.HexNumber));
}

var fileContents = new byte[255];
contents.CopyTo(fileContents, 0);

// Initial values
foreach (var iv in initialValues)
   fileContents[iv.Key] = iv.Value;

await File.WriteAllBytesAsync("/Users/davidbetteridge/SimpleInstructionMachine/Files/boot", fileContents);

return;

string RemoveInLineComment(string str) => str.Split(';')[0].Trim();

byte StringToByte(string str)
{
   if (str.StartsWith("0x"))
      return byte.Parse(str[2..], NumberStyles.HexNumber);
   else if (str.StartsWith('\''))
      return (byte)str[1]; //ASCII
   else
      return byte.Parse(str);
}

public record SourceLine
{
   public required int LineNumber { get; init; }
   public required string Text { get; init; }
}

public record CommandLine
{
   public required int LineNumber { get; init; }
   public required string Mnemonic { get; init; }

   public required string[] Operands { get; init; }
}

public record CommandInfo
{
   public required int OperandCount { get; set; }
   public required int BaseOpcode { get; set; }
}