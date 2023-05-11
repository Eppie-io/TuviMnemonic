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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using TuviBytesShamirSecretSharingLib;

namespace MnemonicSharingLib
{
    /// <summary>
    /// Realizes Shamir's Secret Sharing Scheme for mnemonics in a sense of BIP-39 
    /// (https://github.com/bitcoin/bips/blob/master/bip-0039.mediawiki).
    /// Mnemonic is a byte array. It contains entropy (first 128, 160, 192, 224, 256 bits) and checksum (last 4, 5, 6, 7, 8 bits respectively).
    /// Each 11 bits are changed to a word from special WorldList.
    /// Shamir Sharing correctly works ONLY with CORRECT CHECKSUM mnemonics. 
    /// Or last word can be changed to the word that contains valid checksum.
    /// </summary>
    public static class MnemonicSharing
    {
        private const int MnemonicWordBitSize = 11;
        private const int MaxAmountOfShares = 16;

        /// <summary>
        /// Get entropy of this mnemonic.
        /// </summary>
        /// <param name="mnemonic">Mnemonic to get entropy.</param>
        /// <returns>Entropy as byte array.</returns>
        public static byte[] GetEntropy(this Mnemonic mnemonic)
        {
            if (mnemonic is null)
            {
                throw new ArgumentNullException(nameof(mnemonic));
            }

            int[] indices = mnemonic.Indices;
            BigInteger number = new BigInteger(0);
            foreach (var index in indices)
            {
                number <<= MnemonicWordBitSize;
                number |= index;
            }

            number >>= CheckSumLength(mnemonic);
            return number.ToBigEndianByteArrayWithSpecificLength(mnemonic.GetEntropyByteSize());
        }

        /// <summary>
        /// Splits mnemonic into partial mnemonics.
        /// </summary>
        /// <param name="mnemonic">Mnemonic.</param>
        /// <param name="threshold">Threshold of scheme.</param>
        /// <param name="numberOfShares">Amount of shares.</param>
        /// <returns>Partial mnemonics.</returns>
        public static Mnemonic[] SplitMnemonic(Mnemonic mnemonic, byte threshold, byte numberOfShares)
        {
            if (mnemonic is null)
            {
                throw new ArgumentNullException(nameof(mnemonic));
            }

            if (threshold == 0)
            {
                throw new ArgumentOutOfRangeException(nameof(threshold), "Threshold can not be 0.");
            }

            if (threshold > numberOfShares)
            {
                throw new ArgumentException("Threshold can not be bigger than number of shares.");
            }

            if (numberOfShares > MaxAmountOfShares)
            {
                throw new ArgumentOutOfRangeException(nameof(numberOfShares), $"Too many shares, max amount - {MaxAmountOfShares}.");
            }

            byte[] entropy = mnemonic.GetEntropy();
            Share[] shares = SecretSharing.SplitSecret(threshold, numberOfShares, entropy);
            Mnemonic[] mnemonicShares = shares.Select(item => MnemonicFromShare(item)).ToArray();
           
            return mnemonicShares;
        }

        /// <summary>
        /// Split mnemonic into only valid partial mnemonics.
        /// Will work slower the more words mnemonic contains (extremely slow for 21 or 24 words).
        /// this is because probability that partial mnemonic is valid is reduced significantly.
        /// </summary>
        /// <param name="mnemonic">Mnemonic.</param>
        /// <param name="threshold">Threshold of scheme.</param>
        /// <param name="numberOfShares">Amount of shares.</param>
        /// <returns>Partial mnemonics.</returns>
        public static Mnemonic[] SplitMnemonicOnlyValidPartialOnes(Mnemonic mnemonic, byte threshold, byte numberOfShares)
        {
            if (mnemonic is null)
            {
                throw new ArgumentNullException(nameof(mnemonic));
            }

            if (threshold == 0)
            {
                throw new ArgumentOutOfRangeException(nameof(threshold), "Threshold can not be 0.");
            }

            if (threshold > numberOfShares)
            {
                throw new ArgumentException("Threshold can not be bigger than number of shares.");
            }

            if (numberOfShares > MaxAmountOfShares)
            {
                throw new ArgumentOutOfRangeException(nameof(numberOfShares), $"Too many shares, max amount - {MaxAmountOfShares}.");
            }

            byte[] entropy = mnemonic.GetEntropy();
            List<Mnemonic> mnemonicShares = new List<Mnemonic>();
            bool isReady = false;

            while (!isReady)
            {
                mnemonicShares = new List<Mnemonic>();
                Share[] shares = SecretSharing.SplitSecret(threshold, MaxAmountOfShares, entropy);
                foreach (var share in shares)
                {
                    Mnemonic mn = MnemonicFromShare(share);
                    if (mn.IsValidChecksum == true)
                    {
                        mnemonicShares.Add(mn);
                    }

                    if (mnemonicShares.Count == numberOfShares)
                    {
                        isReady = true;
                        break;
                    }
                }
            }

            return mnemonicShares.ToArray();
        }

        /// <summary>
        /// Split mnemonic into only valid partial mnemonics asyncroniously.
        /// Will work slower the more words mnemonic contains (extremely slow for 21 or 24 words).
        /// this is because probability that partial mnemonic is valid is reduced significantly.
        /// </summary>
        /// <param name="mnemonic">Mnemonic.</param>
        /// <param name="threshold">Threshold of scheme.</param>
        /// <param name="numberOfShares">Amount of shares.</param>
        /// <returns>Partial mnemonics.</returns>
        public static async Task<Mnemonic[]> SplitMnemonicOnlyValidPartialOnesAsync(Mnemonic mnemonic, byte threshold, byte numberOfShares)
        {
            return await Task.Run(() => SplitMnemonicOnlyValidPartialOnes(mnemonic, threshold, numberOfShares)).ConfigureAwait(false);
        }

        /// <summary>
        /// Creates mnemonic from the secret share.
        /// </summary>
        private static Mnemonic MnemonicFromShare(Share share)
        {
            Mnemonic mnemonic = new Mnemonic(Wordlist.English, share.GetShareValue());

            // We have to write share's index number into mnemonic's checksum place.
            // To do that we will nullify checksum at first.
            int checkSumLength = CheckSumLength(mnemonic);
            int lastIndex = mnemonic.Indices[mnemonic.Indices.Length - 1];
            int tempMask = ~((1 << checkSumLength) - 1);
            int newLastIndex = lastIndex & tempMask; // nullify checksum
            newLastIndex |= share.IndexNumber; // write index number
            string newLastWord = Wordlist.English.GetWordAtIndex(newLastIndex); // get new word from WordList
            mnemonic.Words[mnemonic.Words.Length - 1] = newLastWord;
            mnemonic.Indices[mnemonic.Indices.Length - 1] = newLastIndex;
            return mnemonic;
        }

        /// <summary>
        /// Calculates length of check sum of this mnemonic.
        /// </summary>
        private static int CheckSumLength(Mnemonic mnemonic)
        {
            //We can see a direct dependency between number of words and checksum's length (12-4,15-5, 18-6, 21-7, 24-8)
            //so we will use next calculation.
            return mnemonic.Words.Length / 3;
        }

        /// <summary>
        /// Recovers mnemonic from partial ones.
        /// </summary>
        /// <param name="mnemonics">Partial mnemonics.</param>
        /// <returns>Main mnemonic.</returns>
        public static Mnemonic RecoverMnemonic(Mnemonic[] mnemonics)
        {
            if (mnemonics is null)
            {
                throw new ArgumentNullException(nameof(mnemonics));
            }

            if (mnemonics.Length < 1)
            {
                throw new ArgumentException("You should send at least 1 mnemonic to recover a secret.", nameof(mnemonics));
            }

            Share[] shares = mnemonics.Select(item => ShareFromMnemonic(item)).ToArray();

            byte[] entropy = SecretSharing.RecoverSecret(shares);
            return new Mnemonic(Wordlist.English, entropy);
        }

        /// <summary>
        /// Creates secret share from mnemonic.
        /// </summary>
        private static Share ShareFromMnemonic(Mnemonic mnemonic)
        {
            byte index = (byte)(mnemonic.Indices[mnemonic.Indices.Length - 1] & (1 << CheckSumLength(mnemonic)) - 1);
            return new Share(index, mnemonic.GetEntropy());
        }
    }
}
