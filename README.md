# Overview

This is a terminal implementation of a game I made called Mystic.
Mystic is a deck building, dungeon crawling rouge like where you cast spells to defeat enemies, travelling deeper and deeper into the dungeon.

This version of Mystic has 5 floors each harder than the last.

*Note: this version of Mystic is not being developed any more, a new version will eventually makes it way to Steam*

# Gameplay

The gameplay for this game is all done via commands typed into the terminal.

**Useful commands**

- Typing `?` into the terminal in almost all menus will show all commands accepted at that time
- Typing `!` will clear the terminal in case the prints get to big, for instance in combat

**Starting your first run**

cd to the directory with the csproj and type

```
dotnet run
```

This will build and run the application, make sure you have the dotnet runtime installed on your computer.

*Note: if you have Visual Studio installed, not VSCode, you most likely will already have the dotnet runtime installed*

The game will start by having you choose your character. Every character has a different starting deck and resources. You can view character stats by using the command `v <index>` where the index is the number next to the character.

![Character Selection Screen](screenshots/character-select.png)
