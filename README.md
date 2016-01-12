#Welcome to Talky
Talky is a C# project that I started. It is a part of my C# learning process.
Please don't hate it. It's nice.

#LAZY!
Not committing that chat client quite yet, so for now - feel free to communicate with it
using Telnet and the [Protocol](#protocol) below.

#MySQL
Since I still don't have a configuration file thing setup for this, to use MySQL you will need to edit this file:

[MySQLConnector.cs](https://github.com/SysVoid/Talky/blob/master/Talky/Database/MySQLConnector.cs)

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

##Ping (TBI)

```
P:PING
```

*Response:*

```
P:PING
```

Ping pong is boring, sorry.

##Watch me make it maybe.
I sometimes stream on [Twitch.TV](https://twitch.tv/sysvoid). Feel free to watch me there if I happen to be streaming or something.

##Contributing
Follow my fucking code style or go away. Thanks. <3
