# Blockchain Simulation

I created this Project just for testing Purposes. It is kind of a Simulation how Blockchain works while saving Data encrypted by Hashes.

1. There is a Collection of Data like key, value Format ( Just a random Format )
2. We generate a Hash of every encrypted Data-Block, since the first Hash is not generated because there is no previous Block we have a random starting Hash.
3. This generated Hash is the key of the next encrypted Data-Block and so on.
4. Additionally we have a Nonce Value which will be increased till the user-defined amount of leading Zeros is reached to make sure theres some variety.
