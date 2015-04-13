#### Notice
The is the source code from the Grepolis 2 Bot project on http://bots.uthar.nl.
It started as a small hobby project back in 2011, and has currently grown in a full featured bot. Now that the development has come to an end I decided to make the source code public. Feel free to fork this project to continue with your own version of the bot.

# License
Grepolis2Bot is licensed under [GPL v2](https://github.com/josdemmers/Grepolis2Bot/blob/master/README.md)

# Grepolis2Bot
Bot for the browsergame [Grepolis 2](http://en.grepolis.com/) written in C#. The user guide explaining all the features of the bot can be found here:
http://bots.uthar.nl/155/grepolis-2-bot-guide

## Directories
GrepolisBot2: Main project.
GrepBuildings: Custom .NET control used by Grepolis2Bot.
GrepCulture: Custom .NET control used by Grepolis2Bot.
GrepFarmers: Custom .NET control used by Grepolis2Bot.
GrepSchedulerSmall: Custom .NET control used by Grepolis2Bot.
GrepUnits: Custom .NET control used by Grepolis2Bot.

## Development
Most of the important methods are commented, however if you are missing some comments or want me to extend some of them, let me know. In the next section you'll find sequence diagrams to clarify some processes.

### Sequence Diagrams
Simplified diagram of the login process including the first server request used to get basic information about your towns.
![Sequence diagram login](http://i58.tinypic.com/1zml64p.png)





