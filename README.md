# TuviMnemonic

This library implements Shamir's Secret Sharing Scheme for mnemonics according to BIP-39 (https://github.com/bitcoin/bips/blob/master/bip-0039.mediawiki).

**It is based on the following libraries:**
* NuGet packet NBitcoin (https://github.com/MetacoSA/NBitcoin) provides class Mnemonic.
* TuviBytesShamirSecretSharingLib (https://github.com/Eppie-io/TuviBytesShamirSecretSharingLib) implements Shamir's Secret Sharing Scheme for bytes and byte arrays.

TuviMnemonic allows you to "split" your main secret mnemonic M into N partial mnemonics. Any T (0 < T <= N <= 16) of them will recover your main secret mnemonic. You can choose T and N.

Your secret must be a mnemonic from NBitcoin library.

**Notable properties:**
1. Mnemonics for sharing must be valid mnemonics. I.e. mnemonic's ending should be a checksum of it's entropy. Or last word can be changed.
2. This software shares only mnemonic's entropy, its checksum is calculated after splitting. After "splitting", partial mnemonic's index number is written in place of checksum. So partial mnemonics could be not valid.
3. Mainly prepared for twelwe-words mnemonics but works for all mnemonics.

**How to use:**
```
Mnemonic mnemonic = new Mnemonic(Wordlist.English, WordCount.Twelve); // mnemonic creation
Mnemonic[] mnemonics = MnemonicSharing.SplitMnemonic(mnemonic, threshold, numberOfShares); // splitting into partial mnemonics
Mnemonic recoveredMnemonic = MnemonicSharing.RecoverMnemonic(mnemonics); // recovering from partial mnemonics
```
More specific example
```
Mnemonic mnemonic = new Mnemonic("adjust only visit burger course talent home visit knock desk struggle throw");
Mnemonic[] mnemonics = MnemonicSharing.SplitMnemonic(mnemonic, 2, 3);
Mnemonic recoveredMnemonic = MnemonicSharing.RecoverMnemonic(new Mnemonic[] { mnemonics[0], mnemonics[2] }); // any two partial mnemonics
```         
