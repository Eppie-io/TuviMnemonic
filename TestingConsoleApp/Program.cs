///////////////////////////////////////////////////////////////////////////////
//   Copyright 2023 Eppie (https://eppie.io)
//
//   Licensed under the Apache License, Version 2.0(the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at
//
//   http://www.apache.org/licenses/LICENSE-2.0
//
//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.
///////////////////////////////////////////////////////////////////////////////

using NBitcoin;
using System.Numerics;
using MnemonicSharingLib;

namespace TestingConsoleApp
{
    public static class Program
    {
        static void Main(string[] args)
        {
            //DictionaryMnemonicSearch(1_0_0);

            //SavingMnemonicsInFile(10);

            //HowLongToWaitTillValidGroup(20000);

            //HowManyMnemonicsHaveRepeatedWords(100);

            SplitMnemonicAndShowShares(3, 16);
            //HowManyValidSharesInMnemonicShares(2, 16);
            //HowManyMnemonicsHaveRepeatedWords(10000);
        }

        /// <summary>
        /// Calculates how many mnemonic have repeated words in their words representation.
        /// </summary>
        /// <param name="amount">Amount of mnemonics to check.</param>
        private static void HowManyMnemonicsHaveRepeatedWords(int amount)
        {
            ////Experiment 3. How many mnemonics has repeated words (two or more the same words in a seed phrase).

            PrintOutput("Start");

            int AmountOfPhrases = 0;
            for (int i = 0; i < amount; i++)
            {
                Mnemonic mnemonic = new Mnemonic(Wordlist.English, WordCount.Twelve);
                var words = mnemonic.Words;
                for (int k = 0; k < words.Length - 1; k++)
                {
                    int counter = 1;
                    for (int l = k + 1; l < words.Length; l++)
                    {
                        if (words[k] == words[l])
                            counter++;
                    }
                    if (counter > 1)
                    {
                        //Console.WriteLine($"Word {words[k]} meets {counter} times.");
                        AmountOfPhrases++;
                        //PrintWords(mnemonic);
                    }
                }
            }

            Console.WriteLine($"\nThere {AmountOfPhrases} frases.");
        }

        /// <summary>
        /// Calculate and show how many repeats you have to do before you find specific amount
        /// of valid submnemonics.
        /// </summary>
        /// <param name="amount">Amount of random mnemonics to check.</param>
        private static void HowLongToWaitTillValidGroup(int amount)
        {
            //Experiment 5.How many repeats we have to do to get 3-5 valid partial mnemonics (after generation 16 ones).

            PrintOutput("Start");

            List<int> listFor3 = new ();
            List<int> listFor4 = new ();
            List<int> listFor5 = new ();
            List<int> listFor6 = new ();
            List<int> listFor7 = new ();

            int counter3 = 1;
            int counter4 = 1;
            int counter5 = 1;
            int counter6 = 1;
            int counter7 = 1;

            for (int i = 0; i < amount; i++)
            {
                int counter = 0;

                Mnemonic mnemonic = new Mnemonic(Wordlist.English, WordCount.Twelve);

                Mnemonic[] mnemonics = MnemonicSharing.SplitMnemonic(mnemonic, 2, 16);

                foreach (var mn in mnemonics)
                {
                    if (mn.IsValidChecksum == true)
                        counter++;
                }

                CheckListForThreshold(ref counter3, counter, 3, listFor3);
                CheckListForThreshold(ref counter4, counter, 4, listFor4);
                CheckListForThreshold(ref counter5, counter, 5, listFor5);
                CheckListForThreshold(ref counter6, counter, 6, listFor6);
                CheckListForThreshold(ref counter7, counter, 7, listFor7);
            }

            ShowResults(3, listFor3);
            ShowResults(4, listFor4);
            ShowResults(5, listFor5);
            ShowResults(6, listFor6);
            ShowResults(7, listFor7);
        }

        private static void CheckListForThreshold(ref int counterValue, int validSharesAmount, int threshold, List<int> list)
        {
            if (validSharesAmount >= threshold)
            {
                list.Add(counterValue);
                counterValue = 1;
            }
            else
            {
                counterValue++;
            }
        }

        private static void ShowResults(int num, List<int> list)
        {
            Console.WriteLine($"Results for {num} valid parts:");
            Console.WriteLine($"Minimal number of retries to get {num} valid parts: {list.Min()}");
            Console.WriteLine($"Maximal number of retries to get {num} valid parts: {list.Max()}");
            Console.WriteLine($"Average number of retries to get {num} valid parts: {list.Average()}");
            Console.WriteLine($"All amount of retries when we get {num} valid parts: {list.Count}");
            Console.WriteLine("");
            list.Sort();
            int limit = Math.Min (list.Count, 5);
            Console.Write($"First {limit} the shortest tries: ");
            for (int i = 0; i < limit; i++)
            {
                Console.Write($"{list[i]}, ");
            }
            Console.WriteLine("");
            Console.Write($"Last {limit} the longest tries: ");
            for (int i = list.Count - 1; i > list.Count - limit - 1; i--)
            {
                Console.Write($"{list[i]}, ");
            }
            Console.WriteLine("");
            Console.WriteLine("");
        }

        /// <summary>
        /// Calculate how many valid partial mnemonics in (2, 16) splitting of random {amount} mnemonics. 
        /// </summary>
        /// <param name="amount">Amount of mnemonics.</param>
        private static void CalculateStatisticsOfValidShares(int amount)
        {
            PrintOutput("Start");

            int[] result = new int[17];

            //Mnemonic mnemonic = new Mnemonic(Wordlist.English, WordCount.Twelve);
            for (int i = 0; i < amount; i++)
            {
                int counter = 0;
                Mnemonic mnemonic = new Mnemonic(Wordlist.English, WordCount.Twelve);

                Mnemonic[] mnemonics = MnemonicSharing.SplitMnemonic(mnemonic, 2, 16);

                foreach (var mn in mnemonics)
                {
                    if (mn.IsValidChecksum == true)
                        counter++;
                }
                result[counter]++;
            }

            PrintOutput("Result:");
            for (int i = 0; i < 17; i++)
            {
                Console.WriteLine($"With {i} valid shares: {result[i]} initial mnemonics.");
            }

        }

        /// <summary>
        /// Split mnemonic into shares and show them. Reccover initial mnemonic from shares.
        /// </summary>
        /// <param name="threshold">Threshold of sharing.</param>
        /// <param name="numberOfShares">Number of shares.</param>
        private static void SplitMnemonicAndShowShares(byte threshold, byte numberOfShares)
        {
            PrintOutput("Start");
            Mnemonic mnemonic = new Mnemonic(Wordlist.English, WordCount.Twelve);
            //Mnemonic mnemonic = new Mnemonic("three three three three three three three three three three three tiger");
            int counter = 0;
            PrintOutput("Initial mnemonic:");
            PrintWords(mnemonic);

            Mnemonic[] mnemonics = MnemonicSharing.SplitMnemonic(mnemonic, threshold, numberOfShares);
            PrintOutput("Partial mnemonics:");
            foreach (var mn in mnemonics)
            {
                PrintWords(mn);
                Console.WriteLine($"Is valid? {mn.IsValidChecksum}");
                if (mn.IsValidChecksum == true)
                    counter++;
            }

            PrintOutput("---------------------------");
            PrintOutput("Recovered mnemonic from shares:");
            Mnemonic recoveredMnemonic = MnemonicSharing.RecoverMnemonic(mnemonics);
            PrintWords(recoveredMnemonic);
        }

        /// <summary>
        /// Split mnemonic into shares and check how many of them are valid. Show the result.
        /// </summary>
        /// <param name="threshold">Threshold of sharing.</param>
        /// <param name="numberOfShares">Number of shares.</param>
        private static void HowManyValidSharesInMnemonicShares(byte threshold, byte numberOfShares)
        {
            Mnemonic mnemonic = new Mnemonic(Wordlist.English, WordCount.Twelve);
            int counter = 0;
            PrintOutput("Initial mnemonic:");
            PrintWords(mnemonic);

            Mnemonic[] mnemonics = MnemonicSharing.SplitMnemonic(mnemonic, threshold, numberOfShares);
            foreach (var mn in mnemonics)
            {
                if (mn.IsValidChecksum == true)
                    counter++;
            }

            Console.WriteLine($"There are {counter} valid mnemonics");
        }

        /// <summary>
        /// Generate {amount} mnemonics, check are them already exist and save them into files.
        /// Shows in the end how many of them are already exist.
        /// </summary>
        /// <param name="amount">Amount of mnemonics.</param>
        private static void SavingMnemonicsInFile(int amount)
        {
            PrintOutput("Start");
            int counter = 0;

            string dirPath = $"data\\";
            if (!Directory.Exists(dirPath))
            {
                Directory.CreateDirectory(dirPath);
            }

            for (int i = 0; i < amount; i++)
            {
                Mnemonic mnemonic = new Mnemonic(Wordlist.English, WordCount.Twelve);
                //Mnemonic mnemonic = new Mnemonic("three three three three three three three three three three three three");

                var key = GetTwoWords(mnemonic);

                bool NeedToWriteMnemonic = false;
                string filePath = $"data\\{key}.txt";

                if (File.Exists(filePath))
                {
                    using StreamReader stream = new StreamReader(filePath);
                    if (IsFileContainsMnemonic(stream, mnemonic))
                    {
                        PrintOutput("Current mnemonic already exists!!!");
                        PrintWords(mnemonic);
                    }
                    else
                    {
                        NeedToWriteMnemonic = true;
                    }
                }
                else
                {
                    NeedToWriteMnemonic = true;
                }

                if (NeedToWriteMnemonic)
                {
                    using StreamWriter outputFile = new StreamWriter(filePath, true);
                    WriteMnemonicToFile(outputFile, mnemonic);
                }
            }

            Console.WriteLine($"\nThere are {counter} repeated frases.");
        }

        /// <summary>
        /// Creates partial mnemonics from random ones. Check are they valid. Input valid ones into dictionary.
        /// Check are partial mnemonics already exist or no. Shows in the end how many of them are already exist.
        /// </summary>
        /// <param name="amount">Amount of mnemonics to check.</param>
        private static void DictionaryMnemonicSearch(int amount)
        {
            uint amountOfValidMnemonics = 0;
            int counter = 0;
            PrintOutput("Start");
            Dictionary<string, int> storage = new Dictionary<string, int>();
            for (int i = 0; i < amount; i++)
            {
                Mnemonic mnemonic = new Mnemonic(Wordlist.English, WordCount.Twelve);
                //Mnemonic mnemonic = new Mnemonic("three three three three three three three three three three three three");

                Mnemonic[] mnemonics = MnemonicSharing.SplitMnemonic(mnemonic, 2, 16);

                foreach (var mn in mnemonics)
                {
                    if (mn.IsValidChecksum == true)
                    {
                        amountOfValidMnemonics++;
                        var words = mn.Words;
                        string line = string.Join(" ", words).Trim();

                        if (storage.ContainsKey(line))
                        {
                            storage[line]++;
                            PrintOutput("This mnemonic is repeated!!!");
                            counter++;
                        }
                        else
                        {
                            storage.Add(line, 1);
                        }
                    }
                }
            }

            Console.WriteLine($"We checked {amountOfValidMnemonics} valid partial mnemonics, " +
                $"created from {amount} random mnemonics. There are {counter} repeated mnemonics.");
        }

        private static bool IsFileContainsMnemonic(StreamReader stream, Mnemonic mnemonic)
        {
            List<string> words = new List<string>();
            string? line;
            while ((line = stream.ReadLine()) != null)
            {
                words.Add(line.Trim());
            }

            var mnemonicWords = mnemonic.Words;
            string frase = string.Join(" ", mnemonicWords).Trim();
            return words.Contains(frase);
        }

        private static void WriteMnemonicToFile(StreamWriter fileWriter, Mnemonic mnemonic)
        {
            var words = mnemonic.Words;
            string line = string.Join(" ", words).Trim();
            fileWriter.WriteLine(line);
        }

        private static void PrintWords(Mnemonic mnemonic)
        {
            Console.WriteLine(string.Join(" ", mnemonic.Words));
        }

        private static (string, string) GetTwoWords(Mnemonic mnemonic)
        {
            return (mnemonic.Words[0], mnemonic.Words[1]);
        }

        private static void PrintOutput(string output)
        {
            Console.WriteLine(output);
        }
    }
}
