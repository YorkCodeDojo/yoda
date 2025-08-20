namespace Davids_Solutions;

static class Command
{
    public const byte Halt = 0;
    public const byte LoadFromFile = 1;
    public const byte SaveToFile = 2;
    public const byte Write = 3;
    public const byte Add = 4;
    public const byte Inc = 5;
    public const byte Dec = 6;
    public const byte Nop = 7;
    public const byte JumpIfZero = 8;
}
