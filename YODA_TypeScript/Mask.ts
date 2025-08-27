// Mask.ts
/**
 * Bitmap mask to apply to opcodes.
 * For example all 8 opcodes for Add begin with 0b0100
 */
export const Mask = {
    Misc: 0b0000,
    SaveToFile: 0b0001,
    LoadFromFile: 0b0010,
    Write: 0b0011,
    Add: 0b0100,
    Sub: 0b0101,
    Inc: 0b0110,
    Dec: 0b0111,
    JumpIfZero: 0b1000,
    JumpWithReturn: 0b1001
} as const;
