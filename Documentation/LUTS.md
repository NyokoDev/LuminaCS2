### LUT Selection (Beta)

We're excited to introduce the new LUT Selection feature in Lumina v1.5.4. This feature allows you to apply custom LUTs to enhance your game visuals. Here's a brief guide on how to use it:

#### Understanding LUT Selection

Lumina now supports LUTs in `.cube` format for the Unity HDRP Tonemapping. Unlike the previous game, which used PNG files, the post-processing stack in Unity HDRP utilizes `.cube` files.

#### Using Your Own LUTs

1. **Obtain LUTs**: You can find free LUTs online from sites like [on1.com](https://www.on1.com/) or create them yourself using Adobe Photoshop. (Search for tutorials on how to create LUTs with Photoshop.)

2. **Add LUTs**: Drop your `.cube` files into the LUTS folder located at: ``C:\Users\YourUsername\AppData\LocalLow\Colossal Order\Cities Skylines II\ModsData\Lumina\LUTS``

You can also open this folder directly from the Lumina UI.

3. **Apply Your LUT**:
- In the Lumina interface, type the exact name of your LUT (without the `.cube` extension) to apply it.
- You can copy and paste the name from your file explorer for convenience.
- Click "Load LUT."

![Load LUT](https://i.imgur.com/yJ8X9ff.png)


**Note**: The tonemapping feature is a work in progress. Currently, the Tonemapping mode is set to 'External' to display the LUTs, excluding the ACES mode. Future updates will include a dropdown to switch between different modes.
