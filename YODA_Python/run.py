import machine
import asyncio

vm = machine.VirtualMachine(True)
asyncio.run(vm.run("/Users/davidbetteridge/SimpleInstructionMachine/Files"))

