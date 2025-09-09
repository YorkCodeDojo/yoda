using System;
using Avalonia.Controls;
using Avalonia.Input;

namespace Intro;

public partial class MainWindow : Window
{
    private readonly ISlide[] _slides =
    [
        new ImageSlide("Images/logo.jpg", 1200),
        new ListSlide(["Networking and Food", "Explain Problem", "Work In Pairs", "Present", "Pub"]),
        new ImageSlide("Images/ethos.jpg"),
        new TextSlide("Simple Instruction Machine", 30),
        new ListSlide(["01000111000000100000001010001000", "01000111 00000010 00000010 10001010", "0x47 0x02 0x02 0x0A", "Add 2 + 2 ==> [10]"]),
        
        new MemorySlide("Memory", ["0x00: 0x47", "0x01: 0x02", "0x02: 0x02", "0x03: 0x0A", "....", "0xFF: 0x00"], 75, -1),
        
        new MemorySlide("Before", ["0x00: 0x47", "0x01: 0x02", "0x02: 0x02", "0x03: 0x0A", "....", "0x0A: 0x00"], 75, -1),
        new MemorySlide("After", ["0x00: 0x47", "0x01: 0x02", "0x02: 0x02", "0x03: 0x0A", "....", "0x0A: 0x04"], 75, -1),
        
        new MemorySlide("Before", ["0x00: 0x47", "0x01: 0x02", "0x02: 0x02", "0x03: 0x0A", "0x04: 0xAD"], 75, 0x00),
        new MemorySlide("After", ["0x00: 0x47", "0x01: 0x02", "0x02: 0x02", "0x03: 0x0A", "0x04: 0xAD"], 75, 0x04),
        
        // Addressing
        new ListSlide(["Addressing Modes", "0x10 means immediate", "[0x0A] means direct address", "[[0x0A]] means indirect address"]),
        
        // Interrupts
        new MemorySlide("Interrupt Vector Table", ["0xFE: 0x47", "0xFF: 0x02"], 75, -1),
        new ListSlide(["Left Arrow Pressed", "Address in 0xFE looked up", "IP changed to that address", "Code Runs", "RET returns back to previous location"]),
        
        // Screen Memory
        new MemorySlide("Screen Memory", ["0xF8: 32", "0xF9: 32", "0xFA: 32", "0xFB: 32", "0xFC: 32", "0xFD: 1"], 75, -1),
        
        new UrlSlide("https://github.com/YorkCodeDojo/yoda", 50),
    ];
    
    private int _slideNumber = 0;
   
    public MainWindow()
    {
        InitializeComponent();
        this.WindowState = WindowState.Maximized;
        Switcher.Content = _slides[_slideNumber];
        _slides[_slideNumber].Display(true);
    }

    protected override void OnKeyDown(KeyEventArgs e)
    {
        if (e.Key == Key.P)
        {
            // P for previous slide
            _slideNumber = Math.Max(0, _slideNumber - 1);
            var previousPage = _slides[_slideNumber];
            Switcher.Content = previousPage;
            previousPage.Display(true);
        }
        
        else if (e.Key == Key.Space)
        {
            // Space bar to either build this slide, or advance to the following slide
            if (_slides[_slideNumber].Display(false) == DisplayResult.Completed)
            {
                // Page is complete, display the next page
                _slideNumber = (_slideNumber + 1) % _slides.Length;
                var nextSlide = _slides[_slideNumber];
                Switcher.Content = nextSlide;
                nextSlide.Display(true);
            }
        }
    }
}