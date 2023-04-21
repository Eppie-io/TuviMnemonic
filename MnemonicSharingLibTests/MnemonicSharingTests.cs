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

using MnemonicSharingLib;
using NBitcoin;

namespace MnemonicSharingLibTests
{
    public class MnemonicSharingTests
    {
        [TestCase(WordCount.Twelve)]
        [TestCase(WordCount.Fifteen)]
        [TestCase(WordCount.Eighteen)]
        [TestCase(WordCount.TwentyOne)]
        [TestCase(WordCount.TwentyFour)]

        public void RandomMnemonicRecoveryAllPossibilitiesTests(WordCount wordCount)
        {
            Mnemonic mnemonic = new Mnemonic(Wordlist.English, wordCount);

            Mnemonic[] mnemonics = MnemonicSharing.SplitMnemonic(mnemonic, 3, 5);
            Assert.Multiple(() =>
            {
                Assert.That(mnemonic.EqualsTo(MnemonicSharing.RecoverMnemonic(new Mnemonic[] { mnemonics[0], mnemonics[1], mnemonics[2] })), Is.True);
                Assert.That(mnemonic.EqualsTo(MnemonicSharing.RecoverMnemonic(new Mnemonic[] { mnemonics[0], mnemonics[1], mnemonics[3] })), Is.True);
                Assert.That(mnemonic.EqualsTo(MnemonicSharing.RecoverMnemonic(new Mnemonic[] { mnemonics[0], mnemonics[1], mnemonics[4] })), Is.True);
                Assert.That(mnemonic.EqualsTo(MnemonicSharing.RecoverMnemonic(new Mnemonic[] { mnemonics[0], mnemonics[2], mnemonics[3] })), Is.True);
                Assert.That(mnemonic.EqualsTo(MnemonicSharing.RecoverMnemonic(new Mnemonic[] { mnemonics[0], mnemonics[2], mnemonics[4] })), Is.True);
                Assert.That(mnemonic.EqualsTo(MnemonicSharing.RecoverMnemonic(new Mnemonic[] { mnemonics[0], mnemonics[3], mnemonics[4] })), Is.True);
                Assert.That(mnemonic.EqualsTo(MnemonicSharing.RecoverMnemonic(new Mnemonic[] { mnemonics[1], mnemonics[2], mnemonics[3] })), Is.True);
                Assert.That(mnemonic.EqualsTo(MnemonicSharing.RecoverMnemonic(new Mnemonic[] { mnemonics[1], mnemonics[2], mnemonics[4] })), Is.True);
                Assert.That(mnemonic.EqualsTo(MnemonicSharing.RecoverMnemonic(new Mnemonic[] { mnemonics[1], mnemonics[3], mnemonics[4] })), Is.True);
                Assert.That(mnemonic.EqualsTo(MnemonicSharing.RecoverMnemonic(new Mnemonic[] { mnemonics[2], mnemonics[3], mnemonics[4] })), Is.True);
            });
        }

        [TestCase("adjust only visit burger course talent home visit knock desk struggle throw")]
        [TestCase("peanut intact wedding box grit remain quality drift subject door stamp emotion")]
        [TestCase("thing gaze dose wonder pave seat saddle moral cream move beauty space dignity sail typical")]
        [TestCase("raw conduct stay hair push lunch shoe bicycle shuffle victory salt hidden explain bench urge")]
        [TestCase("minor border hurt heart eye embrace doll symptom mutual angle gadget whisper toward early photo pass infant scout")]
        [TestCase("hill fish denial occur title canyon entry clown grocery student sorry candy pulse frame sort crime mango sorry")]
        [TestCase("smart clock trouble alpha author economy venue arrange meadow paper liar vote mercy talent month wolf fitness grass boring mercy bridge")]
        [TestCase("sword hollow series occur mass holiday fresh hazard already man law priority clinic other stand urban obscure latin moon exact such")]
        [TestCase("hood little cute spot exit suffer damage old chuckle chuckle unaware hospital column wage profit slow material waste utility enact position mother trash battle")]
        [TestCase("sentence buzz route mandate lumber velvet indicate addict bunker park universe grow tomorrow radar brief prize addict enact above exit minute slow sponsor share")]
        public void MnemonicRecoveryAllPossibilitiesTests(string words)
        {
            Mnemonic mnemonic = new Mnemonic(words);
            Mnemonic[] mnemonics = MnemonicSharing.SplitMnemonic(mnemonic, 3, 5);
            Assert.That(MnemonicSharing.RecoverMnemonic(new Mnemonic[] { mnemonics[0], mnemonics[1], mnemonics[2] }).Words, Is.EqualTo(mnemonic.Words));
            Assert.That(MnemonicSharing.RecoverMnemonic(new Mnemonic[] { mnemonics[0], mnemonics[1], mnemonics[3] }).Words, Is.EqualTo(mnemonic.Words));
            Assert.That(MnemonicSharing.RecoverMnemonic(new Mnemonic[] { mnemonics[0], mnemonics[1], mnemonics[4] }).Words, Is.EqualTo(mnemonic.Words));
            Assert.That(MnemonicSharing.RecoverMnemonic(new Mnemonic[] { mnemonics[0], mnemonics[2], mnemonics[3] }).Words, Is.EqualTo(mnemonic.Words));
            Assert.That(MnemonicSharing.RecoverMnemonic(new Mnemonic[] { mnemonics[0], mnemonics[2], mnemonics[4] }).Words, Is.EqualTo(mnemonic.Words));
            Assert.That(MnemonicSharing.RecoverMnemonic(new Mnemonic[] { mnemonics[0], mnemonics[3], mnemonics[4] }).Words, Is.EqualTo(mnemonic.Words));
            Assert.That(MnemonicSharing.RecoverMnemonic(new Mnemonic[] { mnemonics[1], mnemonics[2], mnemonics[3] }).Words, Is.EqualTo(mnemonic.Words));
            Assert.That(MnemonicSharing.RecoverMnemonic(new Mnemonic[] { mnemonics[1], mnemonics[2], mnemonics[4] }).Words, Is.EqualTo(mnemonic.Words));
            Assert.That(MnemonicSharing.RecoverMnemonic(new Mnemonic[] { mnemonics[1], mnemonics[3], mnemonics[4] }).Words, Is.EqualTo(mnemonic.Words));
            Assert.That(MnemonicSharing.RecoverMnemonic(new Mnemonic[] { mnemonics[2], mnemonics[3], mnemonics[4] }).Words, Is.EqualTo(mnemonic.Words));
        }

        [TestCase("three three three three three three three three three three three three")]
        [TestCase("raw conduct stay hair push lunch shoe bicycle shuffle victory salt hidden explain bench useful")]
        [TestCase("minor border hurt heart eye embrace doll symptom mutual angle gadget whisper toward early photo pass infant sea")]
        [TestCase("sword hollow series occur mass holiday fresh hazard already man law priority clinic other stand urban obscure latin moon exact sugar")]
        [TestCase("sentence buzz route mandate lumber velvet indicate addict bunker park universe grow tomorrow radar brief prize addict enact above exit minute slow sponsor seven")]
        public void MnemonicRecoveryWrongCheckSumLastWordChangedTests(string words)
        {
            Mnemonic mnemonic = new Mnemonic(words);
            Mnemonic[] mnemonics = MnemonicSharing.SplitMnemonic(mnemonic, 3, 5);
            Mnemonic actealResult = MnemonicSharing.RecoverMnemonic(new Mnemonic[] { mnemonics[0], mnemonics[1], mnemonics[2] });
            for (int i = 0; i < mnemonic.Words.Length - 1; i++)
            {
                Assert.That(actealResult.Words[i], Is.EqualTo(mnemonic.Words[i]));
            }

            Assert.That(actealResult.Words[^1], Is.Not.EqualTo(mnemonic.Words[^1]));
        }

        [TestCase(WordCount.Twelve)]
        [TestCase(WordCount.Fifteen)]
        [TestCase(WordCount.Eighteen)]
        [TestCase(WordCount.TwentyOne)]
        [TestCase(WordCount.TwentyFour)]
        public void MnemonicRecoveryNotEnoughSharesFailure(WordCount wordCount)
        {
            Mnemonic mnemonic = new Mnemonic(Wordlist.English, wordCount);

            Mnemonic[] mnemonics = MnemonicSharing.SplitMnemonic(mnemonic, 3, 5);
            Assert.Multiple(() =>
            {
                Assert.That(mnemonic.EqualsTo(MnemonicSharing.RecoverMnemonic(new Mnemonic[] { mnemonics[0], mnemonics[1] })), Is.False);
                Assert.That(mnemonic.EqualsTo(MnemonicSharing.RecoverMnemonic(new Mnemonic[] { mnemonics[0], mnemonics[2] })), Is.False);
                Assert.That(mnemonic.EqualsTo(MnemonicSharing.RecoverMnemonic(new Mnemonic[] { mnemonics[0], mnemonics[3] })), Is.False);
                Assert.That(mnemonic.EqualsTo(MnemonicSharing.RecoverMnemonic(new Mnemonic[] { mnemonics[0], mnemonics[4] })), Is.False);
                Assert.That(mnemonic.EqualsTo(MnemonicSharing.RecoverMnemonic(new Mnemonic[] { mnemonics[1], mnemonics[2] })), Is.False);
                Assert.That(mnemonic.EqualsTo(MnemonicSharing.RecoverMnemonic(new Mnemonic[] { mnemonics[1], mnemonics[3] })), Is.False);
                Assert.That(mnemonic.EqualsTo(MnemonicSharing.RecoverMnemonic(new Mnemonic[] { mnemonics[1], mnemonics[4] })), Is.False);
                Assert.That(mnemonic.EqualsTo(MnemonicSharing.RecoverMnemonic(new Mnemonic[] { mnemonics[2], mnemonics[3] })), Is.False);
                Assert.That(mnemonic.EqualsTo(MnemonicSharing.RecoverMnemonic(new Mnemonic[] { mnemonics[2], mnemonics[4] })), Is.False);
                Assert.That(mnemonic.EqualsTo(MnemonicSharing.RecoverMnemonic(new Mnemonic[] { mnemonics[3], mnemonics[4] })), Is.False);
            });
        }

        [Test]
        public void RecoverMnemonicShareArrayIsNullThrowArgumentNullException()
        {
            Mnemonic[]? shares = null;
            Assert.Throws<ArgumentNullException>(() => MnemonicSharing.RecoverMnemonic(shares),
                message: "Share array can not be a null.");
        }

        [Test]
        public void RecoverMnemonicShareArrayIsEmptyThrowArgumentException()
        {
            Mnemonic[] shares = Array.Empty<Mnemonic>();
            Assert.Throws<ArgumentException>(() => MnemonicSharing.RecoverMnemonic(shares),
                message: "There are should be at least 1 partial mnemonic.");
        }

        [Test]
        public void SplitMnemonicSecretIsNullThrowArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => MnemonicSharing.SplitMnemonic(null, 3, 5),
                message: "Secret mnemonic can not be a null.");
        }

        [Test]
        public void SplitMnemonicThresholdIsZeroThrowArgumentOutOfRangeException()
        {
            Mnemonic mnemonic = new Mnemonic("adjust only visit burger course talent home visit knock desk struggle throw");
            Assert.Throws<ArgumentOutOfRangeException>(() => MnemonicSharing.SplitMnemonic(mnemonic, 0, 5),
                message: "Threshold can not be 0.");
        }

        [Test]
        public void SplitMnemonicThresholdIsBiggerThanSharesThrowArgumentException()
        {
            Mnemonic mnemonic = new Mnemonic("adjust only visit burger course talent home visit knock desk struggle throw");
            Assert.Throws<ArgumentException>(() => MnemonicSharing.SplitMnemonic(mnemonic, 6, 5),
                message: "Threshold can not be bigger than number of shares.");
        }

        [Test]
        public void SplitMnemonicNumberOfSharesIsTooBigThrowAArgumentOutOfRangeException()
        {
            Mnemonic mnemonic = new Mnemonic("adjust only visit burger course talent home visit knock desk struggle throw");
            Assert.Throws<ArgumentOutOfRangeException>(() => MnemonicSharing.SplitMnemonic(mnemonic, 5, 17),
                message: "Too many shares, max amount - 16.");
        }
    }
}