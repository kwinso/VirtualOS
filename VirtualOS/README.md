# VirtualOS
Virtual is a unix-like terminal for Zip archives.

![VirtualOS](https://i.imgur.com/5mqYVwO.png "VirtualOS System Example")
--
Map
- [Installation](#installation)
- [Creating new system](#creating-new-system)
- [Booting into existing system](#booting-into-existing-system)
- [Working with the system](#working-with-the-system)
- [Notes](#some-notes)
- [Not implemented](#not-implemented-features)


## Installation
```sh
$ git clone https://github.com/uwumouse/VirtualOS.git

$ cd VirtualOS/VirtualOS

$ dotnet build
```
To execute the program simply run `dotnet run`

## Creating new system
In the Boot Manager, select second option.  
`Mode: 2`  
Then, answer the questions.
![VirtualOS](https://i.imgur.com/vLrfvB7.png "Installation Example")

Your system is installed.  
Now, you can find it in installation directory with the name of a system. For me it's `/home/umouse/vos/example.vos`  

Don't worry about .vos extension. It's just a zip archive. But Boot Manager recognizes only .vos files. 

## Booting into existing system
Now, when your system is installed, you can run it.  
In the Boot Manager, select first option. You will be prompted with User Name and Password.  
Enter name and password of user you've created while installing the system.  

Voila, you're in the system!

## Working with the system.
VirtualOS gives you some commands to work with the system.  

Here's list of the commands:  
(Use `<command> -h` to get help.)
- `ls/dir` - Show files in the directory.
- `cd` - Change current location.
- `echo` - Print text to the console / Write to the file.
- `cat/read` - To see text file contents.
- `rm` - To delete directory / file.
- `md/mkdir` - To create a directory.
- `touch/new/nf` - To create new file.
- `pwd` - Print Working Directory. (Shows directory you're in)
- `clear/cls` - Clear screen.
- `reboot` - Reboot the system.
- `shutdown` - Safely exit the program with saving all state.

## Some notes.
- VirtualOS has kind of prediction of unsafe program exiting and saves files.  
But it's recommended to exit the system with `shutdown` command, because if you can be sure that
no files will be lost.
- Do not delete any files in `/sys` folder, without these files system will not start.

## Not implemented features
- VirtualOS supports most of the navigation shortcuts, like `./path`, `../path`, `..`.
But navigating with multiply dots in path is not supported ( `../../path` )
- Even you can create user during installation, you still **_cannot create another user_** in system.
It'll be fixed in next few updates.