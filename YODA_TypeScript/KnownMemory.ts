// KnownMemory.ts
export const KnownMemory = {
    APP_DATA_BOTTOM: 0x00,   // User space grows upwards
    STACK_BOTTOM: 0xF7,      // Stack grows downwards
    LCD_0: 0xF8,
    LCD_1: 0xF9,
    LCD_2: 0xFA,
    LCD_3: 0xFB,
    LCD_4: 0xFC,
    ControlFlags: 0xFD,
    IVT_RIGHT_ARROW: 0xFE,
    IVT_LEFT_ARROW: 0xFF
} as const;
