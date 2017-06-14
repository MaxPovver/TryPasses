Welcome to TryPasses. 
=====================
This software has been developed for unlocking ethereum accounts, whose passwords you can not remember. It is Windows only.
You pass here all passwords you could use either by file or directly to console, and program combines them in pairs.
It also uses CAPSLOCKED versions of passwords and for russian users also uses incorrect(Russian) layout version.
It is working with geth used by Mist, so you should have Mist installed.

Command options
--------------

* -a --account               Id of account you are trying to bruteforce
* -d --deeper                Add this option if you want to test combinations of 3 passwords
* -f --pfile         Path to file where you have all possible password you could use in some form.
* -g --gethdir               Pass here your geth location if it different from default. By default it is searched in C:\Users\*your user folder*\AppData\Roaming\Mist\binaries\Geth\unpacked
* -p --passes                You can pass here your passwords directly, but remember that it is less secure!
* -s --skiprussian           Pass this if you don't have russian keyboard layout.

Example of usage
================

Passing passwords as parameter
------------------------------

```
trypasses -a 0xDB6B649E82637E19e920BF6D005337D10d83b983 -s -p Password1 Password2 Password3

Imported 3 passwords.
Trying "Password1"...(1 of 42, 2,38095238095238%)
Password "Password1" doesn't match.
Trying "Password2"...(2 of 42, 4,76190476190476%)
Password "Password2" doesn't match.
Trying "Password3"...(3 of 42, 7,14285714285714%)
Password "Password3" doesn't match.
Trying "pASSWORD1"...(4 of 42, 9,52380952380952%)
Password "pASSWORD1" doesn't match.
Trying "pASSWORD2"...(5 of 42, 11,9047619047619%)
Password "pASSWORD2" doesn't match.
Trying "pASSWORD3"...(6 of 42, 14,2857142857143%)
Password "pASSWORD3" doesn't match.
Trying "Password1Password1"...(7 of 42, 16,6666666666667%)
Password "Password1Password1" doesn't match.
Trying "Password1Password2"...(8 of 42, 19,047619047619%)
Password "Password1Password2" doesn't match.
Trying "Password1Password3"...(9 of 42, 21,4285714285714%)
Password "Password1Password3" doesn't match.
Trying "Password1pASSWORD1"...(10 of 42, 23,8095238095238%)
....
```

Passing passwords as file
-------------------------
 First you create passes.txt in directory where you have the app with given contents:
 ```
 Password4
 Password5
 Password6
 ```
 Then you call app:
 ```
 trypasses -a 0xDB6B649E82637E19e920BF6D005337D10d83b983 -s -f passes.txt
Imported 3 passwords.
Trying "Password4"...(1 of 42, 2,38095238095238%)
Password "Password4" doesn't match.
Trying "Password5"...(2 of 42, 4,76190476190476%)
Password "Password5" doesn't match.
Trying "Password6"...(3 of 42, 7,14285714285714%)
Password "Password6" doesn't match.
Trying "pASSWORD4"...(4 of 42, 9,52380952380952%)
Password "pASSWORD4" doesn't match.
Trying "pASSWORD5"...(5 of 42, 11,9047619047619%)
Password "pASSWORD5" doesn't match.
Trying "pASSWORD6"...(6 of 42, 14,2857142857143%)
Password "pASSWORD6" doesn't match.
Trying "Password4Password4"...(7 of 42, 16,6666666666667%)
Password "Password4Password4" doesn't match.
Trying "Password4Password5"...(8 of 42, 19,047619047619%)
Password "Password4Password5" doesn't match.
Trying "Password4Password6"...(9 of 42, 21,4285714285714%)
...
```
Usage
=====

This app is free to use, but donations are welcome!

0xBfD86078A4581c92302E87Cd6687B09CB1663A85 (ETH)

1EVRYSRL9CARRBv6NZhSPiLv29eua33iSN (BTC)

Good luck at restoring your wallet!