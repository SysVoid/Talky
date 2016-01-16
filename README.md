#Welcome to Talky
Talky is a C# project that I started. It is a part of my C# learning process.
Please don't hate it. It's nice.

#LAZY!
Not committing that chat client quite yet, so for now - feel free to communicate with it
using Telnet and the [Protocol](#protocol) below.

#MySQL
Upon initial connection, a MySQL configuration file is stored at C:\Talky\database.tcfg

#Configuration
Here's the config format:
```
key:value
```
Don't put a space before or after the `:`.

#Authentication
This server comes with some sexy authentication.

Here's the MySQL table CREATE SQL thingy:
```
CREATE TABLE `users` (
  `id` bigint(30) unsigned NOT NULL AUTO_INCREMENT,
  `username` varchar(16) NOT NULL,
  `password` varchar(128) NOT NULL,
  `created_at` datetime NOT NULL,
  `last_login` datetime NOT NULL,
  `role` enum('admin','user') NOT NULL DEFAULT 'user',
  PRIMARY KEY (`id`),
  UNIQUE KEY `username_UNIQUE` (`username`)
) ENGINE=InnoDB AUTO_INCREMENT=2 DEFAULT CHARSET=utf8;
```

Here are the commands:
```
/register <username> <password>
/auth <username> <password>
```

#Protocol
Welcome to the Talky Protocol. It's so difficult, you might have a hard time learning it.

##Messages:
```
M:Message
```

##Commands:
```
M:/command args
```

##Ping (TBI):
```
P:PING
```

*Response:*
```
P:PING
```
Ping pong is boring, sorry.

##STAT:
Stat returns information about whatever you request. It's that simple, really.

Here's how you get channels:
```
S:ChannelList
```
Here's the response:
```
S:ChannelList:channel1;channel2;channel3
```
Nice and simple.

Here's how you get information about the currently connected client:
```
S:Client
```
Here's what it spits back:
```
S:Client:USERNAME;True/False;+Channel
```
You're probably sat there wondering about the True/False. It's simple. True = muted. Work out the False.

Here's how you get account information about the currently connected client:
```
S:Account
```
Here's the response if they are not authenticated:
```
... What are you expecting?
```
Here's the response if they are authenticated:
```
S:Account:ID;Username;Role
```

##Watch me make it maybe.
I sometimes stream on [Twitch.TV](https://twitch.tv/sysvoid). Feel free to watch me there if I happen to be streaming or something.

##Contributing
Follow my fucking code style or go away. Thanks. <3
