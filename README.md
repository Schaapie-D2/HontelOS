<p align="center">
  <picture>
    <source media="(prefers-color-scheme: dark)" srcset="https://raw.githubusercontent.com/Schaapie-D2/HontelOS/refs/heads/main/Art/HontelOS-Logo-White.png">
    <source media="(prefers-color-scheme: light)" srcset="https://raw.githubusercontent.com/Schaapie-D2/HontelOS/refs/heads/main/Art/HontelOS-Logo-Black.png">
    <img width=60% src="">
  </picture>
</p>

# HontelOS
 An Operating System made in C# based on Cosmos Developed by Jort van Dalen.
# Features
- PS/2 Keyboard and Mouse
- GUI (VBE, SVGAII)
- FAT32 + Virtual File System
- Exception Handler
- Networking (HTTP)

# Screenshots
<p align="center"><img src="https://raw.githubusercontent.com/Schaapie-D2/HontelOS/refs/heads/main/Art/Archive/0.2.0/Screenshot-1.png"></p>

# How to run

User: Admin<br>
Password: HontelOS

Download the ISO file from the latest release from the [releases page](https://github.com/Schaapie-D2/HontelOS/releases)
## VMware
1. click on "Create a new virtual machine", select HontelOS last release ISO file and click the "Next" button.

2. Now click on "Other" for "Guest operating system" and "Other" for version, click "Next" again, select "Store virtual disk as a single file" and select "Finish".

3. The Virtual File System won't work so go to "C:\Users\username\Documents\Virtual Machines\Other" and replace the "Other.vmdk" by [this file](https://github.com/CosmosOS/Cosmos/blob/master/Build/VMWare/Workstation/Filesystem.vmdk?raw=true).
## VirtualBox
We currently don't have a guide for VirtualBox, but you can try to follow the VMware guide and see if it works.
## Real Hardware
> **⚠WARNING**:<br>
Do not use HontelOS on real hardware; improper usage of the VFS or bugs/unintended behavior can irrevocably corrupt existing files on your hard drive(s). The VFS implementation of Cosmos is still under construction and should be regarded as unstable. For testing, it's recommended to use a virtual machine!

Please see [this page](https://github.com/CosmosOS/Cosmos/wiki/Deploy-%28install%29-Cosmos-on-physical-Hardware) for instructions on how to deploy HontelOS on real hardware.

Use [Rufus](https://rufus.ie) to make a bootable USB and use the [HontelOS-real-hardware.iso image](https://github.com/Schaapie-D2/HontelOS/releases)
The default resolution is 1920x1080

https://raw.githubusercontent.com/Schaapie-D2/HontelOS/refs/heads/main/Art/HontelOS-v0.2.1-real-hardware.mp4

# Hardware requirements
- 512 MB Ram

# Known issues
- The OS might crash under heavy load or when running for a long time.
- The OS might freeze when opening a large file.
- When clicking on a window that is infront of another window, the controls in the window behind will be triggerd.
- When starting to drag a window that is infront of another windows handle, both windows will be moved.
