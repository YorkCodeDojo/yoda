import * as fs from "fs/promises";
import * as path from "path";
import { KnownMemory } from "./KnownMemory";
import { Mask } from "./Mask";
import { OpCode } from "./OpCodes";


// === VIRTUAL MACHINE ===
export class VirtualMachine {
  private readonly memory: Uint8Array = new Uint8Array(1 + 0xff);
  private instructionPointer: number = KnownMemory.APP_DATA_BOTTOM;
  private stackHeadPointer: number = KnownMemory.STACK_BOTTOM;
  private interruptsEnabled: boolean = false;
  private folder: string = ".";
  private debug: boolean;

  // Interrupt state
  private lastKey: string | null = null;

  constructor(debug: boolean) {
    this.debug = debug;

    // Setup stdin for keypress interrupts
    if (process.stdin.isTTY) {
      process.stdin.setRawMode(true);
      process.stdin.resume();
      process.stdin.on("data", (chunk: Buffer) => {
        this.lastKey = chunk.toString("utf8");
        if (this.lastKey === "\u0003") { // ^C
          console.log("\nCTRL+C detected — shutting down VM.");
          process.exit(0);
        }
      });
    }
  }

  async run(folderPath: string): Promise<void> {
    this.folder = folderPath;
    await this.boot();

    let halted = false;
    while (!halted) {
      // 🔹 Interrupt handling
      if (this.interruptsEnabled && this.lastKey) {
        const key = this.lastKey;
        this.lastKey = null; // consume key

        if (key === "\u001b[D") { // Left arrow = ESC [ D
          this.pushToStack(this.instructionPointer);
          this.instructionPointer = this.memory[KnownMemory.IVT_LEFT_ARROW]!;
        } else if (key === "\u001b[C") { // Right arrow = ESC [ C
          this.pushToStack(this.instructionPointer);
          this.instructionPointer = this.memory[KnownMemory.IVT_RIGHT_ARROW]!;
        }
      }


      const opCode = this.memory[this.instructionPointer]!;
      try {
        switch (opCode >> 4) {
          case Mask.Misc:
            switch (opCode) {
              case OpCode.Halt: halted = true; continue;
              case OpCode.Wait: await this.wait(); continue;
              case OpCode.Nop: this.nop(); continue;
              case OpCode.Sif: this.sif(); continue;
              case OpCode.Cif: this.cif(); continue;
              case OpCode.Ret: this.ret(); continue;
              default: throw new Error("Unknown command " + opCode);
            }
          case Mask.SaveToFile: await this.saveToFile(opCode); continue;
          case Mask.LoadFromFile: await this.loadFromFile(opCode); continue;
          case Mask.Write: this.write(opCode); continue;
          case Mask.Add: this.add(opCode); continue;
          case Mask.Sub: throw new Error("Sub not implemented");
          case Mask.Inc: this.inc(opCode); continue;
          case Mask.Dec: this.dec(opCode); continue;
          case Mask.JumpIfZero: this.jumpIfZero(opCode); continue;
          case Mask.JumpWithReturn: this.jumpWithReturn(opCode); continue;
          default: throw new Error("Unknown command " + opCode);
        }
      } catch (e: any) {
        console.error("Your program has crashed!");
        console.error(e.message);
        console.error(`Instruction Pointer: ${this.instructionPointer}`);
        console.error(`Opcode: ${opCode}`);

        await fs.writeFile("crash_dump", this.memory);

        const lines: string[] = [];
        for (let i = 0; i < this.memory.length; i++) {
          if (i === this.instructionPointer)
            lines.push(`${i.toString(16).padStart(2, "0")}   ${this.memory[i]}    <---- INSTRUCTION POINTER`);
          else
            lines.push(`${i.toString(16).padStart(2, "0")}   ${this.memory[i]}`);
        }
        await fs.writeFile("crash_dump.txt", lines.join("\n"));
        return;
      }
    }

    console.log("\n\nProgram completed successfully");
    process.exit(0);
  }

  // === STACK ===
  private pushToStack(value: number) {
    this.memory[this.stackHeadPointer--] = value;
  }
  private popFromStack(): number {
    this.stackHeadPointer++;
    if (this.stackHeadPointer > KnownMemory.STACK_BOTTOM) throw new Error("Stack underflow");
    return this.memory[this.stackHeadPointer]!;
  }

  // === BOOT ===
  private async boot() {
    this.memory.fill(0);
    this.instructionPointer = KnownMemory.APP_DATA_BOTTOM;
    this.interruptsEnabled = false;
    this.stackHeadPointer = KnownMemory.STACK_BOTTOM;

    const filename = path.join(this.folder, "boot");
    try {
      const fileContents = await fs.readFile(filename);
      if (fileContents.length > this.memory.length)
        throw new Error(`Boot file too large: ${fileContents.length} > ${this.memory.length}`);
      fileContents.copy(this.memory, 0);
      console.log(`\nMemory initialised using boot file (${filename}).`);
    } catch {
      console.log("\nNo boot file found.");
    }
  }

  // === FILE IO ===
  private filenameFromFileNumber(fileNumber: number): string {
    if (fileNumber < 8) return path.join(this.folder, `${fileNumber}`);
    if (fileNumber < 16) return path.join(this.folder, `${fileNumber}.txt`);
    throw new Error(`Unknown file ${fileNumber}`);
  }

  private async saveToFile(opCode: number) {
    const fileNumber = this.read(this.instructionPointer + 1, opCode, 2);
    const sourceLocation = this.read(this.instructionPointer + 2, opCode, 1);
    const length = this.read(this.instructionPointer + 3, opCode, 0);

    if (this.debug)
      console.log(`${this.instructionPointer.toString(16)} SaveToFile:: Writing ${length} bytes from ${sourceLocation.toString(16)}.`);

    await fs.writeFile(
      this.filenameFromFileNumber(fileNumber),
      this.memory.slice(sourceLocation, sourceLocation + length)
    );

    this.instructionPointer += 4;
  }

  private async loadFromFile(opCode: number) {
    const fileNumber = this.read(this.instructionPointer + 1, opCode, 1);
    const targetLocation = this.read(this.instructionPointer + 2, opCode, 0);

    const fileContents = await fs.readFile(this.filenameFromFileNumber(fileNumber));
    if (fileContents.length + targetLocation > this.memory.length)
      throw new Error("File too large");
    fileContents.copy(this.memory, targetLocation);

    if (this.debug)
      console.log(`${this.instructionPointer.toString(16)} LoadFromFile:: Reading into ${targetLocation.toString(16)}.`);

    this.instructionPointer += 3;
  }

  // === MEMORY OPS ===
  private write(opCode: number) {
    const location = this.read(this.instructionPointer + 1, opCode, 1);
    const value = this.read(this.instructionPointer + 2, opCode, 0);

    if (this.debug)
      console.log(`${this.instructionPointer.toString(16)} Write:: ${value} into ${location.toString(16)}`);

    this.updateScreenIfRequired(location, value);
    this.memory[location] = value;
    this.instructionPointer += 3;
  }

  private updateScreenIfRequired(location: number, value: number) {
    if (location === KnownMemory.ControlFlags) {
      if ((this.memory[location]! & 1) === 0 && (value & 1) === 1) {
        console.log("---------------------");
        console.log(
          `| ${this.toChar(this.memory[KnownMemory.LCD_0]!)} | ${this.toChar(this.memory[KnownMemory.LCD_1]!)} | ${this.toChar(this.memory[KnownMemory.LCD_2]!)} | ${this.toChar(this.memory[KnownMemory.LCD_3]!)} | ${this.toChar(this.memory[KnownMemory.LCD_4]!)} |`
        );
        console.log("---------------------");
      }
    }
  }

  private toChar(b: number): string {
    return b === 0x00 ? " " : String.fromCharCode(b);
  }

  private add(opCode: number) {
    const lhs = this.read(this.instructionPointer + 1, opCode, 2);
    const rhs = this.read(this.instructionPointer + 2, opCode, 1);
    const location = this.read(this.instructionPointer + 3, opCode, 0);

    if (this.debug)
      console.log(`${this.instructionPointer.toString(16)} Add:: ${lhs}+${rhs} => ${location.toString(16)}`);
    this.memory[location] = (lhs + rhs) & 0xff;
    this.instructionPointer += 4;
  }

  private inc(opCode: number) {
    const location = this.read(this.instructionPointer + 1, opCode, 0);
    if (this.debug)
      console.log(`${this.instructionPointer.toString(16)} Inc:: ${this.memory[location]} -> ${this.memory[location]! + 1}`);
    this.memory[location]!++;
    this.instructionPointer += 2;
  }

  private dec(opCode: number) {
    const location = this.read(this.instructionPointer + 1, opCode, 0);
    if (this.debug)
      console.log(`${this.instructionPointer.toString(16)} Dec:: ${this.memory[location]} -> ${this.memory[location]! - 1}`);
    this.memory[location]!--;
    this.instructionPointer += 2;
  }

  // === FLOW CONTROL ===
  private nop() {
    if (this.debug) console.log(`${this.instructionPointer.toString(16)} Nop::`);
    this.instructionPointer++;
  }

  private sif() {
    if (this.debug) console.log(`${this.instructionPointer.toString(16)} Sif:: Was ${this.interruptsEnabled}`);
    this.interruptsEnabled = true;
    this.instructionPointer++;
  }

  private cif() {
    if (this.debug) console.log(`${this.instructionPointer.toString(16)} Cif:: Was ${this.interruptsEnabled}`);
    this.interruptsEnabled = false;
    this.instructionPointer++;
  }

  private async wait() {
    if (this.debug) console.log(`${this.instructionPointer.toString(16)} Wait::`);
    await new Promise((r) => setTimeout(r, 100));
    this.instructionPointer++;
  }

  private ret() {
    const gotoAddress = this.popFromStack();
    if (this.debug)
      console.log(`${this.instructionPointer.toString(16)} Ret:: to ${gotoAddress.toString(16)}`);
    this.instructionPointer = gotoAddress;
  }

  private jumpIfZero(opCode: number) {
    const valueToCheck = this.read(this.instructionPointer + 1, opCode, 1);
    const locationToJumpTo = this.read(this.instructionPointer + 2, opCode, 0);
    if (this.debug)
      console.log(`${this.instructionPointer.toString(16)} JumpIfZero:: to ${locationToJumpTo.toString(16)} if ${valueToCheck} == 0`);
    if (valueToCheck === 0) this.instructionPointer = locationToJumpTo;
    else this.instructionPointer += 3;
  }

  private jumpWithReturn(opCode: number) {
    const locationToJumpTo = this.read(this.instructionPointer + 1, opCode, 0);
    if (this.debug)
      console.log(`${this.instructionPointer.toString(16)} JumpWithReturn:: to ${locationToJumpTo.toString(16)}`);
    this.pushToStack(this.instructionPointer + 2);
    this.instructionPointer = locationToJumpTo;
  }

  // === MEMORY READ ===
  private read(location: number, opCode: number, mask: number): number {
    const isSet = (((opCode & 0x0f) >> mask) & 1) === 1;
    if (location >= this.memory.length) throw new Error("Illegal memory location " + location);

    if (isSet) return this.memory[location]!;

    const reference = this.memory[location]!;
    if (reference >= this.memory.length) throw new Error("Illegal deref location " + location);

    return this.memory[reference]!;
  }
}
