MONO Pang 
========
2014 Mario Macías

![Screenshot](shot.png "Screenshot")

C# over Mono/.NET port of MOO Pang that I did to get introduced in C# language. [Please refer to the original MOO Pang web wite for more information about MOO/MONO Pang](https://github.com/mariomac/MOOPang).

Without any previous knowledge of C#/.NET it took me a few hours of googling/try-error to port an almost exact version from Java to Mono/.NET.

In further work, I would like to adapt the MONO Pang code to the C# coding style, and make use of some C# interesting features such as managing directly attributes instead of the classical getter/setter methos from Java.

The code works perfectly in Linux. However, I have tried it in Mac OS X Mavericks and the program crashes due to some strange Null Reference Exception when the screen is refreshed too often.

Compiling and executing
-----------------------

Open the project with Visual Studio or Monodevelop and click Run.

How to play
-----------
Use cursor keys <- y -> to move left and right.

Use spacebar to shot arrows. If an arrow hits a ball, the ball will be divided into
two smaller balls. When balls are small enough they will disappear when are hit by
an arrow.

If a ball hits you, game is over.
