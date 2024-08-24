### LUT Selection Guide (Beta)

We're excited to introduce and expand the LUT Selection feature in Lumina, starting from v1.5.4 and enhanced in v1.5.6r2. This feature allows you to apply custom LUTs to enhance your game visuals, providing greater control over the visual style of your game.

#### Understanding LUT Selection

Lumina supports LUTs in `.cube` format for Unity HDRP Tonemapping. Unlike the previous game that used PNG files, the post-processing stack in Unity HDRP utilizes `.cube` files, providing more detailed and accurate color grading options.

#### Using Your Own LUTs

1. **Obtain LUTs**: 
   - You can find free LUTs online from sites like [on1.com](https://www.on1.com/) or create them yourself using Adobe Photoshop. (Search for tutorials on how to create LUTs with Photoshop.)

2. **Add LUTs**: 
   - Drop your `.cube` files into the LUTS folder located at:
     ```
     C:\Users\YourUsername\AppData\LocalLow\Colossal Order\Cities Skylines II\ModsData\Lumina\LUTS
     ```
   - You can also open this folder directly from the Lumina UI for easy access.

3. **Apply Your LUT**:
   - In the Lumina interface, type the exact name of your LUT (without the `.cube` extension) to apply it.
   - For convenience, you can copy and paste the name directly from your file explorer.
   - Click "Load LUT" to apply the selected LUT.

   ![Load LUT](https://i.imgur.com/yJ8X9ff.png)

#### Included LUTs

Lumina v1.5.4 includes 10 pre-loaded LUTs from on1.com, ready to enhance your game visuals immediately. These LUTs are a great starting point if you're new to using LUTs.

#### Tonemapping Mode Selection

Starting in Lumina v1.5.6r2, a new feature allows users to switch between different tonemapping modes:

- **None**: No tonemapping is applied.
- **External**: Displays the LUTs (this is the mode required to see the effects of LUTs).
- **Custom**: Custom tonemapping mode.
- **Neutral**: A neutral tonemapping mode.
- **ACES**: ACES mode (currently not fully implemented).

*Important:* To see LUTs, the mode must be set to 'External'; otherwise, they won't show, and only the effects of other tonemapping modes will be visible.

![Modes](https://i.imgur.com/ww1V5oo.png)

#### Improvements in Lumina v1.5.6r2

- **LUTs Texture Format**:
  - The default color texture format for LUTs has been switched to `RGBA64` to resolve inconsistencies with other LUTs.
  - **Note:** Switching between `RGBA64` and `RGBA32` may require a restart of the application for the changes to take effect.