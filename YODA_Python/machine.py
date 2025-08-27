import asyncio
import os
import sys
import termios
import tty
import select
from opcodes import OpCode
import keyboard


class KnownMemory:
    APP_DATA_BOTTOM = 0x00   # User space grows upwards
    STACK_BOTTOM    = 0xF7   # Stack grows downwards
    LCD_0           = 0xF8
    LCD_1           = 0xF9
    LCD_2           = 0xFA
    LCD_3           = 0xFB
    LCD_4           = 0xFC
    ControlFlags    = 0xFD
    IVT_RIGHT_ARROW = 0xFE
    IVT_LEFT_ARROW  = 0xFF

class Mask:
    """Bitmap masks for opcode groups (top 4 bits)."""
    Misc           = 0b0000
    SaveToFile     = 0b0001
    LoadFromFile   = 0b0010
    Write          = 0b0011
    Add            = 0b0100
    Sub            = 0b0101
    Inc            = 0b0110
    Dec            = 0b0111
    JumpIfZero     = 0b1000
    JumpWithReturn = 0b1001

# import sys,tty,os,termios
# def getkey():
#     old_settings = termios.tcgetattr(sys.stdin)
#     tty.setcbreak(sys.stdin.fileno())
#     try:
#         while True:
#             b = os.read(sys.stdin.fileno(), 3).decode()
#             if len(b) == 3:
#                 k = ord(b[2])
#             else:
#                 k = ord(b)
#             key_mapping = {
#                 127: 'backspace',
#                 10: 'return',
#                 32: 'space',
#                 9: 'tab',
#                 27: 'esc',
#                 65: 'up',
#                 66: 'down',
#                 67: 'right',
#                 68: 'left'
#             }
#             return key_mapping.get(k, chr(k))
#     finally:
#         termios.tcsetattr(sys.stdin, termios.TCSADRAIN, old_settings)

from pynput.keyboard import Key, Listener

# Flag to track if the left arrow key is pressed
is_left_arrow_pressed = False

def on_press(key):
    global is_left_arrow_pressed
    try:
        if key == Key.left:
            is_left_arrow_pressed = True
            print("Left arrow key pressed")
    except AttributeError:
        pass  # Handle special keys (like shift, ctrl)

def on_release(key):
    global is_left_arrow_pressed
    if key == Key.left:
        is_left_arrow_pressed = False
        print("Left arrow key released")

    # Stop listener when escape key is pressed
    if key == Key.esc:
        return False

# Create the listener for keyboard events
with Listener(on_press=on_press, on_release=on_release) as listener:
    listener.join()

class VirtualMachine:
    def __init__(self, debug=False):
        global is_left_arrow_pressed
        self.debug = debug
        self._memory = [0] * (1 + 255)
        self._instruction_pointer = KnownMemory.APP_DATA_BOTTOM
        self._stack_head_pointer = KnownMemory.STACK_BOTTOM
        self._interrupts_enabled = False
        self._folder = "."

    async def run(self, folder_path):
        self._folder = folder_path
        await self.boot()

        halted = False
        while not halted:
            # === Handle Interrupts ===
            if self._interrupts_enabled:
                if is_left_arrow_pressed: # Left arrow
                    self.push_to_stack(self._instruction_pointer & 0xFF)
                    self._instruction_pointer = self._memory[KnownMemory.IVT_LEFT_ARROW]
                    if self.debug:
                        print(f"Interrupt: LEFT ARROW triggered -> Jump {self._instruction_pointer:02X}")
                # elif keyboard.is_pressed("right"): # Right arrow
                #     self.push_to_stack(self._instruction_pointer & 0xFF)
                #     self._instruction_pointer = self._memory[KnownMemory.IVT_RIGHT_ARROW]
                #     if self.debug:
                #         print(f"Interrupt: RIGHT ARROW triggered -> Jump {self._instruction_pointer:02X}")

            op_code = self._memory[self._instruction_pointer]
            try:
                mask = op_code >> 4
                if mask == Mask.Misc:
                    if op_code == OpCode.Halt:
                        halted = True
                        continue
                    elif op_code == OpCode.Wait:
                        await self.wait()
                        continue
                    elif op_code == OpCode.Nop:
                        self.nop()
                        continue
                    elif op_code == OpCode.Sif:
                        self.sif()
                        continue
                    elif op_code == OpCode.Cif:
                        self.cif()
                        continue
                    elif op_code == OpCode.Ret:
                        self.ret()
                        continue
                    else:
                        raise Exception(f"Unknown command {op_code}")

                elif mask == Mask.SaveToFile:
                    await self.save_to_file(op_code)
                elif mask == Mask.LoadFromFile:
                    await self.load_from_file(op_code)
                elif mask == Mask.Write:
                    self.write(op_code)
                elif mask == Mask.Add:
                    self.add(op_code)
                elif mask == Mask.Sub:
                    raise Exception("Sub not implemented")
                elif mask == Mask.Inc:
                    self.inc(op_code)
                elif mask == Mask.Dec:
                    self.dec(op_code)
                elif mask == Mask.JumpIfZero:
                    self.jump_if_zero(op_code)
                elif mask == Mask.JumpWithReturn:
                    self.jump_with_return(op_code)
                else:
                    raise Exception(f"Unknown command {op_code}")

            except Exception as e:
                sys.stderr.write(
                    "Your program has crashed, things aren't looking good for the space craft.\n"
                )
                sys.stderr.write(str(e) + "\n")
                sys.stderr.write(
                    f"Instruction Pointer: {self._instruction_pointer}\n"
                )
                sys.stderr.write(f"Opcode: {op_code}\n")

                with open("crash_dump", "wb") as f:
                    f.write(bytes(self._memory))

                with open("crash_dump.txt", "w") as f:
                    for i, val in enumerate(self._memory):
                        if i == self._instruction_pointer:
                            f.write(f"{i:02X}   {val}    <---- INSTRUCTION POINTER\n")
                        else:
                            f.write(f"{i:02X}   {val}\n")

                sys.stderr.write(
                    "A crash dump containing all the memory has been written to crash_dump and crash_dump.txt\n"
                )
                return

        print("\n\nProgram completed successfully")

    # === Stack ===
    def push_to_stack(self, value: int):
        self._memory[self._stack_head_pointer] = value
        self._stack_head_pointer -= 1

    def pop_from_stack(self) -> int:
        self._stack_head_pointer += 1
        if self._stack_head_pointer > KnownMemory.STACK_BOTTOM:
            raise Exception("Stack underflow")
        return self._memory[self._stack_head_pointer]

    # === Boot ===
    async def boot(self):
        self._memory = [0] * len(self._memory)
        self._instruction_pointer = KnownMemory.APP_DATA_BOTTOM
        self._interrupts_enabled = False
        self._stack_head_pointer = KnownMemory.STACK_BOTTOM

        filename = os.path.join(self._folder, "boot")
        if os.path.exists(filename):
            with open(filename, "rb") as f:
                file_contents = f.read()
            if len(file_contents) > len(self._memory):
                raise Exception("Boot file too large")
            self._memory[: len(file_contents)] = file_contents
            print(f"\nMemory has been initialised using the boot file ({filename}).")
        else:
            print("\nNo boot file found.")

    # === Helpers ===
    def filename_from_file_number(self, file_number: int) -> str:
        if file_number < 8:
            return os.path.join(self._folder, str(file_number))
        elif file_number < 16:
            return os.path.join(self._folder, f"{file_number}.txt")
        else:
            raise Exception("Unknown file number")

    # === Instructions (with debug logging) ===
    async def save_to_file(self, op_code):
        file_number = self.read(self._instruction_pointer + 1, op_code, 2)
        source_location = self.read(self._instruction_pointer + 2, op_code, 1)
        length = self.read(self._instruction_pointer + 3, op_code, 0)
        if self.debug:
            print(f"{self._instruction_pointer:08x} SaveToFile:: Writing {length} bytes from {source_location:02X}")
        with open(self.filename_from_file_number(file_number), "wb") as f:
            f.write(bytes(self._memory[source_location : source_location + length]))
        self._instruction_pointer += 4

    async def load_from_file(self, op_code):
        file_number = self.read(self._instruction_pointer + 1, op_code, 1)
        target_location = self.read(self._instruction_pointer + 2, op_code, 0)
        with open(self.filename_from_file_number(file_number), "rb") as f:
            file_contents = f.read()
        if len(file_contents) + target_location > len(self._memory):
            raise Exception("File too large")
        self._memory[target_location : target_location + len(file_contents)] = file_contents
        if self.debug:
            print(f"{self._instruction_pointer:08x} LoadFromFile:: Reading into {target_location:02X}")
        self._instruction_pointer += 3

    def write(self, op_code):
        location = self.read(self._instruction_pointer + 1, op_code, 1)
        value = self.read(self._instruction_pointer + 2, op_code, 0)
        if self.debug:
            print(f"{self._instruction_pointer:08x} Write:: {value} into {location:02X}")
        self.update_screen_if_required(location, value)
        self._memory[location] = value
        self._instruction_pointer += 3

    def add(self, op_code):
        lhs = self.read(self._instruction_pointer + 1, op_code, 2)
        rhs = self.read(self._instruction_pointer + 2, op_code, 1)
        location = self.read(self._instruction_pointer + 3, op_code, 0)
        total = (lhs + rhs) & 0xFF
        if self.debug:
            print(f"{self._instruction_pointer:08x} Add:: {lhs} + {rhs} = {total} -> {location:02X}")
        self._memory[location] = total
        self._instruction_pointer += 4

    def inc(self, op_code):
        location = self.read(self._instruction_pointer + 1, op_code, 0)
        if self.debug:
            print(f"{self._instruction_pointer:08x} Inc:: {self._memory[location]} -> {self._memory[location]+1}")
        self._memory[location] = (self._memory[location] + 1) & 0xFF
        self._instruction_pointer += 2

    def dec(self, op_code):
        location = self.read(self._instruction_pointer + 1, op_code, 0)
        if self.debug:
            print(f"{self._instruction_pointer:08x} Dec:: {self._memory[location]} -> {self._memory[location]-1}")
        self._memory[location] = (self._memory[location] - 1) & 0xFF
        self._instruction_pointer += 2

    def nop(self):
        if self.debug:
            print(f"{self._instruction_pointer:08x} Nop")
        self._instruction_pointer += 1

    def sif(self):
        if self.debug:
            print(f"{self._instruction_pointer:08x} Sif:: enabling interrupts")
        self._interrupts_enabled = True
        self._instruction_pointer += 1

    def cif(self):
        if self.debug:
            print(f"{self._instruction_pointer:08x} Cif:: disabling interrupts")
        self._interrupts_enabled = False
        self._instruction_pointer += 1

    async def wait(self):
        if self.debug:
            print(f"{self._instruction_pointer:08x} Wait")
        await asyncio.sleep(0.1)
        self._instruction_pointer += 1

    def ret(self):
        goto_address = self.pop_from_stack()
        if self.debug:
            print(f"{self._instruction_pointer:08x} Ret:: {goto_address:02X}")
        self._instruction_pointer = goto_address

    def jump_if_zero(self, op_code):
        value_to_check = self.read(self._instruction_pointer + 1, op_code, 1)
        location_to_jump_to = self.read(self._instruction_pointer + 2, op_code, 0)
        if self.debug:
            print(f"{self._instruction_pointer:08x} JumpIfZero:: jump {location_to_jump_to:02X} if {value_to_check}==0")
        if value_to_check == 0:
            self._instruction_pointer = location_to_jump_to
        else:
            self._instruction_pointer += 3

    def jump_with_return(self, op_code):
        location_to_jump_to = self.read(self._instruction_pointer + 1, op_code, 0)
        if self.debug:
            print(f"{self._instruction_pointer:08x} JumpWithReturn:: {location_to_jump_to:02X}")
        self.push_to_stack(self._instruction_pointer + 2)
        self._instruction_pointer = location_to_jump_to

    def read(self, location, op_code, mask):
        is_set = (((op_code & 0x0F) >> mask) & 1) == 1
        if location >= len(self._memory):
            raise Exception("Illegal memory location " + str(location))
        if is_set:
            return self._memory[location]
        reference = self._memory[location]
        if reference >= len(self._memory):
            raise Exception("Illegal de-referenced memory location " + str(reference))
        return self._memory[reference]

    def update_screen_if_required(self, location: int, value: int):
        if location == KnownMemory.ControlFlags:
            if (self._memory[location] & 1) == 0 and (value & 1) == 1:
                # bit 0 has been set, refresh the LCD display
                print("---------------------")
                print("| ", end="")
                print(self._to_char(self._memory[KnownMemory.LCD_0]), end=" | ")
                print(self._to_char(self._memory[KnownMemory.LCD_1]), end=" | ")
                print(self._to_char(self._memory[KnownMemory.LCD_2]), end=" | ")
                print(self._to_char(self._memory[KnownMemory.LCD_3]), end=" | ")
                print(self._to_char(self._memory[KnownMemory.LCD_4]), end=" |")
                print()
                print("---------------------")

    def _to_char(self, b: int) -> str:
        if b == 0x00:
            return ' '
        else:
            return chr(b)