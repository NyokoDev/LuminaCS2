# Publish LUT Pack to Paradox Mods

This guide will walk you through the steps to create and publish a LUT pack for *Cities: Skylines II* on Paradox Mods. Before proceeding, ensure that you've tested your LUTs manually in the game to verify they work as intended.

---

## Prerequisites

1. **Create LUTs:**
   If you haven't created LUTs yet, refer to the Unity documentation on authoring LUTs:  
   [Authoring LUTs in Unity HDRP](https://docs.unity3d.com/Packages/com.unity.render-pipelines.high-definition@14.0/manual/Authoring-LUTs.html)

   For a video guide on creating LUTs specifically for Lumina, watch this tutorial:  
   [Video on Creating LUTs for Lumina](https://www.youtube.com/watch?v=uAKRjDkZey4)

   This guide provides steps for using Adobe Photoshop and DaVinci Resolve.

---

## Publishing Your LUT Pack

### Step 1: Prepare Your Files

1. **Organize Your Files:**
   Make sure your LUT files are in the correct folder structure. Your LUT files (`.cube` files) should be placed in a directory named `LUTS`.

2. **Create Publish Configuration:**
   Ensure you have a publish configuration file (`PublishConfiguration.xml`) set up. This file defines how your mod will be published.

3. **Add `.lumina` File:**
   Include an additional file with the `.lumina` extension in your mod’s content folder. This file is required for Lumina to recognize and properly handle the LUT pack. The `.lumina` file should be empty or contain minimal metadata but must be present in the content folder.

### Step 2: Use the ModPublisher Command

To publish your LUT pack, you'll use the `ModPublisher` command-line tool provided by the Cities: Skylines II Modding Toolchain. Here’s how to use it based on whether you are publishing a new mod or updating an existing one:

1. **Open Command Prompt:**
   Open a Command Prompt window. You can do this by searching for `cmd` in the Start menu.

2. **Run the Command:**

   - **To Publish a New Mod:**

     ```cmd
     "C:\Program Files (x86)\Steam\steamapps\common\Cities Skylines II\Cities2_Data\StreamingAssets\~Tooling~\ModPublisher\ModPublisher.exe" Publish "C:\Path\To\Your\PublishConfiguration.xml" -c "C:\Path\To\Your\Content" -v
     ```

     **Explanation:**
     - `Publish`: Use this command to create and publish a new mod.
     - `"C:\Path\To\Your\PublishConfiguration.xml"`: Path to your publish configuration file.
     - `-c "C:\Path\To\Your\Content"`: Specifies the content folder containing your LUT files and the required `.lumina` file.
     - `-v`: Enables verbose output for detailed logs.

   - **To Update an Existing Mod:**

     ```cmd
     "C:\Program Files (x86)\Steam\steamapps\common\Cities Skylines II\Cities2_Data\StreamingAssets\~Tooling~\ModPublisher\ModPublisher.exe" NewVersion "C:\Path\To\Your\PublishConfiguration.xml" -c "C:\Path\To\Your\Content" -v
     ```

     **Explanation:**
     - `NewVersion`: Use this command to update an existing mod with a new version.
     - `"C:\Path\To\Your\PublishConfiguration.xml"`: Path to your publish configuration file.
     - `-c "C:\Path\To\Your\Content"`: Specifies the content folder containing your LUT files and the required `.lumina` file.
     - `-v`: Enables verbose output for detailed logs.

   **Note:** Replace the placeholder paths with the actual paths to your configuration file and content folder.

---

## Additional Tips

- **Verify Paths:** Double-check the paths used in the command to ensure they are correct and accessible.
- **Include `.lumina` File:** Ensure the `.lumina` file is present in your content folder. This file is necessary for Lumina to recognize your mod.
- **Test Your LUTs:** Always test your LUTs in-game to confirm they work as expected before publishing.
- **Review Logs:** After running the command, review the verbose output to ensure there were no errors during the publishing process.

---

By following these steps, you should be able to successfully publish or update your LUT pack for *Cities: Skylines II* on Paradox Mods. If you encounter any issues, refer to the error messages for troubleshooting guidance or consult the modding tool documentation for further assistance.