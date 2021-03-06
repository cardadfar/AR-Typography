# AR-Typography
AR Core Extension for Real-Time Motion Tracked Text. Works on Surfaces &amp; Walls

AR Typography places text anywhere based on a simple touch of the screen. Built on AR Core in Unity, people can place multiple layers of 3D text, each with their own unique content, into one environment. The text tracks changes in movement, and will maintain its position and rotation values when moving around.

AR Typography is able to generate 3D text onto flat surfaces (including walls) by intercepting the point cloud data from AR core and creating independent tracking planes for each object at the point the screen was tapped. When tapping the screen, the app will project the active 3D point cloud data (active referring to points visible to the camera) onto a 2D surface and find the closest point to where the screen was touched. Iterating over the point cloud data, this point will find the next two closest point in order to find the best cross product that represents the orientation of the point. The point now contains information about the position and orientation that can be used to generate the text.

AR Typography loads multiple strings of text into a list upon startup. The active text will modulate over the list of strings and generate a text object with the contents of the active text. This allows for effective storage and placement of multiple pieces of text in one scene.

In order to create a 3D effect for the text, a white text layer is placed at position p with orientation r, and 15 additional iterations of black text is placed at position (p – (i+1)*k*r) and orientation r, where i is the index [0,14] and k is some constant.

Download TextCore.cs and place it in the Scripts folder of for the HelloAR example included in AR Core. Assign a 3D text layer to textLayer. 

To include text to cycle through, add additional strings to the content list in the Start() function.

```
void Start() {
    content.Add("How Far We've Come");
}
```
