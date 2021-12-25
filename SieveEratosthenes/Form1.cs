using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace SieveEratosthenes
{
    public partial class Form1 : Form
    {
        private ulong i = 2;
        private ulong j;
        private bool[] nonPrime;
        private ulong maxNumber;
        private ulong minNumber;
        public Form1()
        {
            InitializeComponent();
            richTextBox1.ReadOnly = true;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            richTextBox1.Clear();
            if (textBox2.Text != "")
            {
                minNumber = ulong.Parse(textBox2.Text);
                maxNumber = ulong.Parse(textBox1.Text);
                EratospheneBetweenTwoIntS();
            }
            else
            {
                maxNumber = ulong.Parse(textBox1.Text);
                if (maxNumber <= 300)
                {
                    CreateSieve();
                    Eratosphene(maxNumber);
                    timer1.Start();
                }
                else
                {
                    EratospheneForBigNumber();
                }
            }
        }

        private List<ulong> GetPrimes()
        {
            List<ulong> primes = new List<ulong>();

            for (int i = 0; i < nonPrime.Length; i++)
            {
                if (!nonPrime[i])
                {
                    primes.Add((ulong)i);
                }
            }

            return primes;
        }

        private void EratospheneBetweenTwoIntS()
        {
            Eratosphene((ulong)Math.Sqrt(maxNumber));
            List<ulong> primes = GetPrimes();
            nonPrime = new bool[maxNumber - minNumber + 1];

            foreach (var prime in primes)
            {
                ulong firstMultiple = minNumber / prime;
                if (firstMultiple <= 1)
                {
                    firstMultiple = prime + prime;
                }
                else if (minNumber % prime != 0)
                {
                    firstMultiple = (firstMultiple * prime) + prime;
                }
                else
                {
                    firstMultiple *= prime;
                }

                for (ulong j = firstMultiple; j <= maxNumber; j += prime)
                {
                    nonPrime[j - minNumber] = true;
                }
            }
            WriteToFile();
        }

        private void EratospheneForBigNumber()
        {
            ulong sqrt = (ulong)Math.Sqrt(maxNumber >> 1);
            byte[] nonPrime = new byte[(maxNumber >> 4) + 1];
            ulong len = (ulong)nonPrime.Length << 3;
            ulong countsOfBits = 0;
            ulong maxByte = (ulong)((sqrt + 1) >> 3);

            for (ulong i = 0; i < maxByte; i++, countsOfBits += 8)
            {
                for (int bitNumber = 0; bitNumber < 8;)
                {
                    if ((nonPrime[i] & 1 << bitNumber++) == 0)
                    {
                        ulong step = ((countsOfBits + (ulong)bitNumber) << 1) + 1;

                        for (ulong j = (step * step >> 1) - 1; j < len; j += step)
                        {
                            nonPrime[j >> 3] |= (byte)(1 << (int)(j % 8));
                        }
                    }
                }
            }
            WriteToFile(nonPrime);
        }
        private void WriteToFile()
        {
            using (StreamWriter write = new StreamWriter("Primes.txt"))
            {
                for (ulong i = minNumber; i <= maxNumber; i++)
                {
                    if (!nonPrime[i - minNumber])
                    {
                        write.Write(i + " ");
                    }
                }
            }
        }

        private void WriteToFile(byte[] arr)
        {
            ulong c = 0;
            using (StreamWriter write = new StreamWriter("Primes.txt"))
            {
                write.WriteLine(2);

                for (int i = 0; i < arr.Length; i++, c += 8)
                {
                    for (ulong k = 0; k < 8;)
                    {
                        if ((arr[i] & 1 << (int)k++) == 0)
                        {
                            if ((k + c) * 2 + 1 <= maxNumber)
                            {
                                
                                write.WriteLine(((k + c) * 2 + 1) + " ");
                            }
                        }
                    }
                }
            }
        }

        private void Eratosphene(ulong maxNumber)
        {
            uint sqrt = (uint)Math.Sqrt(maxNumber);
            nonPrime = new bool[maxNumber + 1];
            nonPrime[0] = nonPrime[1] = true;

            for (uint i = 2; i <= sqrt; i++)
            {
                if (!nonPrime[i])
                {
                    for (ulong j = i * i; j <= maxNumber; j += i)
                    {
                        nonPrime[j] = true;
                    }
                }
            }
        }

        private void CreateSieve()
        {
            for (ulong i = 2; i < maxNumber + 1; i++)
            {
                richTextBox1.Text += $"{i} ";
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (i <= maxNumber && !nonPrime[i])
            {
                j = i * i;
                timer1.Stop();
                timer2.Start();
            }
            else if (i > maxNumber)
            {
                timer2.Stop();
                timer1.Stop();
            }
            else
            {
                i++;
            }
        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            if (j <= maxNumber && nonPrime[j])
            {
                richTextBox1.Select(richTextBox1.Text.IndexOf($"{j}"), j.ToString().Length);
                richTextBox1.SelectionColor = Color.White;
                nonPrime[j] = true;
            }
            else
            {
                i++;
                timer1.Start();
                timer2.Stop();
            }
            j += i;
        }
    }
}
