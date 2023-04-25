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
using System.Linq;

namespace MnemonicSharingLib
{
    public static class MnemonicExtension
    {
        /// <summary>
        /// Method compares words of two mnemonics.
        /// </summary>
        /// <param name="mnemonic">One mnemonic.</param>
        /// <param name="anotherMnemonic">Another mnemonic.</param>
        /// <returns>True, if all words are the same. False otherwise.</returns>
        public static bool EqualsTo(this Mnemonic mnemonic, Mnemonic anotherMnemonic)
        {
            if (mnemonic is null || anotherMnemonic is null)
            {
                return false;
            }
            
            string[] words = mnemonic.Words;
            string[] words2 = anotherMnemonic.Words;

            return words.SequenceEqual(words2);
        }
    }
}
