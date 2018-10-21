// Ben Martin
// CS4200
// 10/21/18
// This program reads in a file, takes the opcode of each line of data, and prints out the appropriate Assembly instruction

using System;

namespace CS4200Program1
{
    class Program
    {
        public static String commands = "";
       
        static void Main(string[] args)
        {
            Console.WriteLine("Press any key to run program");
            Console.ReadKey();
            string[] lines = System.IO.File.ReadAllLines("MachineCode.txt");    // Puts text from text file into string array
            long[] mLanguage = new long[lines.Length];

            for (int i = 0; i < lines.Length; i++)      // Converts hex to integer values, stores them in long array
            {
                mLanguage[i] = Convert.ToInt64(lines[i], 16);
            }

            toAssembly(mLanguage);      // Calls function to convert the machine language to assembly commands
            System.IO.File.WriteAllText("output.txt", commands);
            Console.ReadKey();
        }

        // Function responsible for getting the opcode and passing it onto the appropriate function for each command
        static void toAssembly(long[] mLanguage)
        {
            long[] opcode = new long[mLanguage.Length];

            for (int i = 0; i < mLanguage.Length; i++) // Gets opcode from each command 
            {
                opcode[i] = mLanguage[i] >> 26;        // First 6 bits of mLanguage
            }

            for (int i = 0; i < mLanguage.Length; i++)
            {
                switch (opcode[i])                      // Checks opcode for each command, passes it on to the appropriate function
                {
                    case 0: add(mLanguage[i]); break; // Add
                    case 2: j(mLanguage[i]); break; // J
                    case 4: beq(mLanguage[i]); break; // Beq
                    case 8: addi(mLanguage[i]); break; // Addi
                    case 13: ori(mLanguage[i]); break; // Ori
                    case 15: lui(mLanguage[i]); break; // Lui
                    case 35: lw(mLanguage[i]); break; // Lw
                    case 43: sw(mLanguage[i]); break; // Sw
                    default: Console.WriteLine("Error, invalid input."); break;
                }
            }

        }

        static void add(long mLanguage)           // Add Command Function           DONE!
        {
            if (mLanguage == 12)
            {
                Console.WriteLine("syscall");       // Since add and syscall have the same opcode, checks to see if it's a syscall instead of an add command
                commands += "syscall";
            }
            else
            {
                long rs = (mLanguage >> 21) & 0x1f;
                long rt = (mLanguage >> 16) & 0x1f;
                long rd = (mLanguage >> 11) & 0x1f;
                Console.WriteLine("add\t{0}, {1}, {2}", getreg(rd), getreg(rs), getreg(rt));
                commands += "add\t" + getreg(rd) + ", " + getreg(rs) + ", " + getreg(rt) + "\n";
            }
        }

        static void j(long mLanguage)             // J Command Function             DONE!
        {
            long address = mLanguage & 0x3fffff;
            address = (address & 0xf0000000) | (address << 2);      // Calculates jump address from last 16 bits of mLanguage
            string hex = address.ToString("X").PadLeft(8, '0');
            Console.WriteLine("j\t0x{0}", hex);
            commands += "j\t0x" + hex + "\n";
        }

        static void beq(long mLanguage)           // Beq Command Function           DONE!
        {
            long rs = (mLanguage >> 21) & 0x1f;
            long rt = (mLanguage >> 16) & 0x1f;
            long address = mLanguage & 0xffff;                      // Last 16 bits of mLanguage
            string hex = address.ToString("X").PadLeft(4, '0');     // Converts address back to a hex string, padded with 0's to fit formatting
            Console.WriteLine("beq\t{0}, {1}, 0x{2}", getreg(rs), getreg(rt), hex);
            commands += "beq\t" + getreg(rs) + ", " + getreg(rt) + ", 0x" + hex + "\n";
        }

        static void addi(long mLanguage)          // Addi Command Function          DONE!
        {
            long rs = (mLanguage >> 21) & 0x1f;
            long rt = (mLanguage >> 16) & 0x1f;
            long address = mLanguage & 0xffff;
            int signbit = (int)address >> 15;   // Gets sign bit of address
            if (signbit == 1)                   // Checks sign bit for negative, accounts for two's complement
                address = -1 * ((address ^ 0xffff) + 1);

            Console.WriteLine("addi\t{0}, {1}, {2}", getreg(rt), getreg(rs), address);
            commands += "addi\t" + getreg(rt) + ", " + getreg(rs) + ", " + address + "\n";
        }

        static void ori(long mLanguage)           // Ori Command Function           DONE!
        {
            long rs = (mLanguage >> 21) & 0x1f;
            long rt = (mLanguage >> 16) & 0x1f;
            long address = mLanguage & 0xffff;
            string hex = address.ToString("X").PadLeft(4, '0');     // Converts address back to a hex string, padded with 0's to fit formatting
            Console.WriteLine("ori\t{0}, {1}, 0x{2}", getreg(rt), getreg(rs), hex);
            commands += "ori\t" + getreg(rt) + ", " + getreg(rs) + ", 0x" + hex + "\n";
        }

        static void lui(long mLanguage)           // Lui Command Function           DONE!
        {
            long rs = (mLanguage >> 21) & 0x1f;
            long rt = (mLanguage >> 16) & 0x1f;
            long address = mLanguage & 0xffff;
            string hex = address.ToString("X").PadLeft(4, '0');     // Converts address back to a hex string, padded with 0's to fit formatting
            Console.WriteLine("lui\t{0}, 0x{1}", getreg(rt), hex);
            commands += "lui\t" + getreg(rt) + ", 0x" + hex + "\n";
        }

        static void lw(long mLanguage)            // Lw Command Function            DONE!
        {
            long rs = (mLanguage >> 21) & 0x1f;
            long rt = (mLanguage >> 16) & 0x1f;
            long address = mLanguage & 0xffff;
            Console.WriteLine("lw\t{0} {1}({2})", getreg(rt), address, getreg(rs));
            commands += "lw\t" + getreg(rt) + " " + address + "(" + getreg(rs) + ")\n";
        }

        static void sw(long mLanguage)            // Sw Command Function            DONE!
        {
            long rs = (mLanguage >> 21) & 0x1f;
            long rt = (mLanguage >> 16) & 0x1f;
            long address = mLanguage & 0xffff;
            Console.WriteLine("sw\t{0} {1}({2})", getreg(rt), address, getreg(rs));
            commands += "sw\t" + getreg(rt) + " " + address + "(" + getreg(rs) + ")\n";
        }

        // Function that gets the value of the register and returns the name of the actual register
        static string getreg(long rs)
        {
            switch (rs)
            {
                case 0: return "$zero";              // $zero
                case 1: return "$at";               // $at
                case 2:
                case 3: return "$v" + (rs - 2);       // $v0-v1
                case var n when (n < 8 && n >= 4):          // $a0-a3
                    return "$a" + (rs - 4);
                case var n when (n < 16 && n >= 8):         // $t0-t7
                    return "$t" + (rs - 8);
                case var n when (n < 24 && n >= 16):        // $s0-s7
                    return "$s" + (rs - 16);
                case 24:
                case 25: return "$t" + (rs - 16);     // $t8-t9
                default: Console.WriteLine("Error, invalid input."); return null;       // Breaks if an invalid register is received
            }
        }
    }
}
