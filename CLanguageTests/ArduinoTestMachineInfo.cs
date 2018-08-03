using System;
using CLanguage.Interpreter;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using System.Diagnostics;
using System.IO;

namespace CLanguage.Tests
{
    public class ArduinoTestMachineInfo : MachineInfo
	{
        public TestArduino Arduino = new TestArduino ();

		public ArduinoTestMachineInfo ()
		{
            CharSize = 1;
            ShortIntSize = 2;
            IntSize = 2;
            LongIntSize = 4;
            LongLongIntSize = 8;
            FloatSize = 4;
            DoubleSize = 8;
            LongDoubleSize = 8;
            PointerSize = 2;
            HeaderCode = @"
#define HIGH 1
#define LOW 0
#define INPUT 0
#define INPUT_PULLUP 2
#define OUTPUT 1
#define A0 0
#define A1 1
#define A2 2
#define A3 3
#define A4 4
#define A5 5
#define DEC 10
#define HEX 16
#define OCT 8
#define BIN 2
struct SerialClass {
    void begin(int baud);
    void print(const char *value);
    void println(int value, int bas);
    void println(int value);
    void println(const char *value);
};
struct SerialClass Serial;
";

            AddInternalFunction ("void pinMode (int pin, int mode)", Arduino.PinMode);
            AddInternalFunction ("void digitalWrite (int pin, int value)", Arduino.DigitalWrite);
            AddInternalFunction ("int digitalRead (int pin)", Arduino.DigitalRead);
            AddInternalFunction ("void analogWrite (int pin, int value)");
            AddInternalFunction ("int analogRead (int pin)", Arduino.AnalogRead);
            AddInternalFunction ("int map (int value, int fromLow, int fromHigh, int toLow, int toHigh)", Arduino.Map);
            AddInternalFunction ("int constrain (int x, int a, int b)", Arduino.Constrain);
            AddInternalFunction ("void delay (unsigned long ms)");
            AddInternalFunction ("void tone (int pin, int note, int duration)");
            AddInternalFunction ("void assertAreEqual (int expected, int actual)", AssertAreEqual);
            AddInternalFunction ("void assertBoolsAreEqual (bool expected, bool actual)", AssertBoolsAreEqual);
            AddInternalFunction ("void assertFloatsAreEqual (float expected, float actual)", AssertFloatsAreEqual);
            AddInternalFunction ("void assertDoublesAreEqual (double expected, double actual)", AssertDoublesAreEqual);
            AddInternalFunction ("long millis ()", Arduino.Millis);
            AddInternalFunction ("void SerialClass::begin (int baud)", Arduino.SerialBegin);
            AddInternalFunction ("void SerialClass::print (const char *value)", Arduino.SerialPrintlnI);
            AddInternalFunction ("void SerialClass::println (int value, int base)", Arduino.SerialPrintlnII);
            AddInternalFunction ("void SerialClass::println (int value)", Arduino.SerialPrintlnI);
            AddInternalFunction ("void SerialClass::println (const char *value)", Arduino.SerialPrintlnI);
		}

		static void AssertAreEqual (CInterpreter state)
		{
			var expected = state.ReadArg(0);
			var actual = state.ReadArg(1);
            Assert.AreEqual ((int)expected, (int)actual);
		}

        static void AssertFloatsAreEqual (CInterpreter state)
        {
            var expected = state.ReadArg(0);
            var actual = state.ReadArg(1);
            Assert.AreEqual ((float)expected, (float)actual, 1.0e-6);
        }

        static void AssertDoublesAreEqual (CInterpreter state)
        {
            var expected = state.ReadArg(0);
            var actual = state.ReadArg(1);
            Assert.AreEqual ((double)expected, (double)actual, 1.0e-12);
        }

        static void AssertBoolsAreEqual (CInterpreter state)
        {
            var expected = state.ReadArg(0);
            var actual = state.ReadArg(1);
            Assert.AreEqual ((int)expected, (int)actual);
        }

        public class TestArduino
        {
            readonly Stopwatch stopwatch = new Stopwatch ();

            public Pin[] Pins = Enumerable.Range (0, 32).Select (x => new Pin { Index = x }).ToArray ();

            public StringWriter SerialOut = new StringWriter ();

            public TestArduino ()
            {
                stopwatch.Start ();
            }

            public void Millis (CInterpreter state)
            {
                state.Push ((int)stopwatch.ElapsedMilliseconds);
            }

            public void Map (CInterpreter state)
            {
                state.Push (0);
            }

            public void Constrain (CInterpreter state)
            {
                state.Push (0);
            }

            public void PinMode (CInterpreter state)
            {
                var pin = state.ReadArg(0);
                var mode = state.ReadArg(1);
                Pins[pin].Mode = mode;
            }

            public void AnalogRead (CInterpreter state)
            {
                var pin = state.ReadArg(0);
                var value = Pins[pin].AnalogValue;
                state.Push (value);
            }

            public void DigitalRead (CInterpreter state)
            {
                var pin = state.ReadArg(0);
                var value = Pins[pin].DigitalValue;
                state.Push (value);
            }

            public void DigitalWrite (CInterpreter state)
            {
                var pin = state.ReadArg(0);
                var value = state.ReadArg(1);
                Pins[pin].DigitalValue = value;
            }

            public void SerialBegin (CInterpreter state)
            {
            }

            public void SerialPrintlnII (CInterpreter state)
            {
                var v = state.ReadArg(0);
                SerialOut.WriteLine (v);
            }

            public void SerialPrintlnI (CInterpreter state)
            {
                var v = state.ReadArg(0);
                SerialOut.WriteLine (v);
            }

            public class Pin
            {
                public int Index;
                public int Mode;
                public int DigitalValue;
                public int AnalogValue = 42;
            }
        }
	}
}

