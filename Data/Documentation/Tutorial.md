_Nothing new, just copied from my old post at 8bc_

Vortex Tracker II seems to have a reputation for being hard to use. Here's a really basic guide for complete newbies, and anyone else who wants help with understanding the very basics of Vortex Tracker II. Hopefully even if you've never used a tracker before, you'll be able to make a simple song in Vortex Tracker by the end of this tutorial.

Please note I'm not a respected ZX musician and I'm not even very good at music, so the way I do things might not be the best way. But it's better than not being able to make noises at all.

Ingredients

Grab Vortex Tracker II from [here](http://bulba.untergrund.net/vortex_e.htm).

Unpack the archive into a folder. If you're wondering what a .7z file is and how on earth can you unpack it, use [this](http://www.7-zip.org/).

Getting Started

Run VT.exe which is now in the folder you've just made. Welcome to Vortex Tracker II!

Go to File >> New, hit CTRL-N, or click the button with the sheet of paper in the top left corner. Oooh, a new module!

If you have a big screen, and this window looks a bit small, you can fix this. Go to File >> Options, and increase the "number of track lines". Close this window and make a new one. Hopefully it is longer and nicer looking.

But how can I make a noise?!

Maybe now you are wondering how to make a noise. This is quite complicated in Vortex tracker, especially if you are used to FL Studio etc, but you'll get the hang of it in no time.

You are now in the pattern editor. It is where you will spend most of your time composing. However, you should ignore it for now, and click the "Samples" tab.

You should now be looking at some small boxes and a big list of strange looking codes and numbers. This is the sample editor where you create your instruments.

At the moment, the first line of that big list you can see says:

```
00 | tne+000_ +00(00)_ 0_
```

Change it to read:

```
00 | Tne+000_ +00(00)_ F_
```

That's right, all you need to do is capitalise one letter, and change a 0 to an F. Simple!

Now just up and to the left of this list is a box called the "Test field". It should say something like:

```
....|..|C-4 1F.F ....
```

Click on where it says C-4. Hit some buttons on your keyboard. Hurrah!!

Yeah, so I made a beep. I want to make a song!

Of course you do! Go back to the pattern editor by clicking the "Pattern" tab.

The pattern editor works like any tracker, with columns representing different channels. In this case, there are 3 columns, for 3 channels. The main part of the pattern editor contains only 3 different characters (besides the row numbers down the left hand side): **|** and **\---** and **.**

The **|** cannot be changed, it is essentially there for decoration. You'll notice you can't select it. The full stops (periods) are for effects and commands. We'll look at these later. For now, let's focus on the **\---**. This is the most important part of the pattern editor, because it is where you add notes!

Select the first set of three hyphens in the first column. Press "E" on your keyboard. An E note was added. Now use the down key on your keyboard to move the cursor to the row labelled 04. Press "W" on your keyboard. Move to the row labelled 08, and press "Q", then to the row labelled 0C, and press "W" again.

Press F8 to hear what you've done. Press Esc to stop it. Or use the transport controls at the top of the screen.

Sounds okay? Not really, but it's alright. But wouldn't it be nice if the last note didn't carry on forever? Go to line 0E and press "A". "R--" was added. This means note cut. It allows you to use rests in your song.

Press F8 again. It's the first 4 notes of "Mary Had A Little Lamb". Try and finish the song if you want, if you can't be bothered, don't.

Using more than one pattern

Four bars isn't enough for a real song. You need to use more than one pattern to make a song. This is really simple.

Above where you have been entering your notes, there's a long line of little boxes with "**...**" in them. This is the order list. Select the first box, and type a "0" in it. Now select the second box and type a "1" in it. Now you have 2 patterns. Put something in pattern 1, and hit F6. Pattern zero will play first and then pattern one will play. Use delete to get rid of patterns, and remember you can use a pattern more than once in your song (don't reuse them too much though).

I want arps!

To create an arpeggio, use the ornaments tab. It's very simple and easy to get the hang of, even though it looks scary.

Adjust the length of the ornament to 3. Now, just enter "0" in the first row, "3" in the second row, and "7" in the third row. Go to the test field and hit some notes. Minor arp!

```
00|+00
00|+03
00|+07
```

To use this in your song, go back to the pattern editor. Change the third period after the note you want to arp to "1". Play your song, and you'll hear some arps. To turn the arping off (so you can have it applying only to specific notes, not to a whole column), just enter the number of an ornament you haven't used yet. Such as F.

How do I do a kick drum?

length 1, loop 0

```
00 | Tne +0D0^ +00(00)_ F-
```

Of course you can edit this to make better, nicer, and more complicated kick drums, this is just a simple one to get you started.

How do I do a hi-hat?

length 1, loop 0

```
00 | tNe +000_ +00(00)_ F-
```

Of course you can edit this to make better, nicer, and more complicated hi-hats, this is just a simple one to get you started.

It's all very well you telling me how to make instruments, but if I just understood what all these numbers meant, I could make my own.

```
1F|tne +000_ +00(00)_ F_ ***************

11 234 56667 899 AA B CD EEEEEEEEEEEEEEE
```

1 - Line number. Cannot be edited.  
2 - If set to "T", a tone plays. If set to "t", it doesn't.  
3 - If set to "N", white noise plays. If set to "n", it doesn't.  
4 - If set to "E", envelope sound plays. If set to "e", it doesn't.  
5 - Direction of pitch change.  
6 - Amount of pitch change.  
7 - If set to ^, pitch change adds up over time. If set to \_, pitch change is absolute.  
8 - Direction of pitch change for noise and envelope sound.  
9 - Amount of pitch change for noise and envelope sound.  
A - Displays the absolute value of pitch change for noise and envelope sound. Cannot be edited.  
B - Like 7, but for noise and envelope sound.  
C - Volume of sound for this line.  
D - If set to \_, nothing happens. If set to +, volume increases. If set to -, volume decreases.  
E - Visual representation of the volume for that line. Cannot be edited.

All of this is explained in more depth in the manual. Look at some instruments from other people's songs to help you understand how they work.

I wanna be Yerzmyey!

Practise makes perfect. Spend time messing around and figuring stuff out. Load up some .pt3 files by your favourite speccy musicians and have a look at how they work. Read the manual that came with Vortex Tracker II. Don't give up.

Some useful/interesting/cool links

[http://zxtunes.com/](http://zxtunes.com/) - a big collection of Spectrum tunes, in pt2 and pt3 format, meaning you'll be able to open them up in vortex tracker and have a look at what it's meant to look like.

[http://ay-riders.speccy.cz/](http://ay-riders.speccy.cz/) - some very talented ZX musicians.

[http://www.raww.org/](http://www.raww.org/) - Raww Arse!

[http://bulba.untergrund.net/emulator\_e.htm](http://bulba.untergrund.net/emulator_e.htm) - play back spectrum music files on your computer.