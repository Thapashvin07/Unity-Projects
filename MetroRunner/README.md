Unity Version:

2023.2.20f1



PROBLEM 1 —  Data Optimization

=======================================



APPROACH

--------

* Reduced position data from 96 bits (3 floats) to 35 to 47 bits (max)

using quantization and bit packing.



* Quantization - player's position is encoded into int with step size 0.01 and represented it as bits (12 per axis).



* Bit packing - packed bits using write function by converting int value to byte array. 



1. &nbsp;	I included 3 flags at the beginning for 3 axis (x ,y ,z) respectively (each 1 bit) followed by x , y and z quantized bits and lastly the sequence number of the bits sent for 8 bits.
   
2. So the packed bits is sent as a packet to receiver.
   
3. Where it is decoded with ,help of flags where it is checked for value 1 which represents whether that axis has been sent or not.



* 20Hz Tick Rate — sends max 20 packets per second not every frame.



* Threshold  — not sending packet if player moved less than 0.01m.



* Simulated real network like behaviour :  latency=50ms jitter=20ms loss=5%.



* Sequence Number  — detects and rejects out of order packets





ASSUMPTIONS

-----------

* \- Local simulation only, no real multiplayer backend
* \- Both players share same scene on different floors
* \- Local Player is child of Floor1 at (0,0,0)
* \- Simulated Remote Player is child of Floor2 at (25,0,0)
* \- local position is used so floor handles world offset.



BIT SAVED

-----------

Raw float   = 96 bits

Optimized (X+Z)     = 35 bits  (64% saving)

Optimized (X+Y+Z)   = 47 bits  (51% saving)





HOW TO RUN

----------

1\. Open scene in Problem\_1/Scenes/

2\. Press Play.

3\. Use WASD  or arrow keys to move violet player.

4\. Blue player mirrors movement.

5\. Check console for debug logs.



