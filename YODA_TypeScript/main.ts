// main.ts
import { VirtualMachine } from "./VirtualMachine";

// Grab command line args
const args = process.argv.slice(2);

let folder = "/Users/davidbetteridge/SimpleInstructionMachine/Files";
if (args.length > 0) {
    folder = args[0];
}

const debug = args.length > 1 && args[1] === "--debug";

console.log(
    "Starting landing computer running York's Obscenely Dumb Architecture (YODA) - Release Build 12x.11g-34 + Anti-gravity module"
);
console.log(`Folder Path: ${folder}\n`);
console.log("Connecting to Engine Control System.... SUCCESS!");
console.log("Connecting to Landing Control System.... SUCCESS!");
console.log("Connecting to Interplanetary Communication System.... SUCCESS!");
console.log("All systems are GO!");

(async () => {
    const machine = new VirtualMachine(debug);
    await machine.run(folder);
})();
