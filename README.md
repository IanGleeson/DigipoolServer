# DigipoolServer

This server is for Digipool, a physical and virtual pool game. It reads in tags from an Impinj Revolution reader and sends them to a client.

To connect to the reader, an entry must be made in your C:/Windows/System32/drivers/etc/hosts file with the following contents like below:
IPAddress       Reader Addesss

Example Entry:
192.168.0.100		SpeedwayR-12-4D-B5.local

Reader Address is the address of the Impinj Reader.
Signal Variance denotes the variance in signal strength allowed before a tag updates. i.e. If the tag is not moving, it won't update.
Time Between Updates is the amount of time in microseconds before a tag is allowed to send updates.

If connecting to an already connected table, it will replace it.
