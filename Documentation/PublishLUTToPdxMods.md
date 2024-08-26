# Publish LUT Pack to Paradox Mods

This guide will walk you through the steps to create and publish a LUT pack for Cities: Skylines II on Paradox Mods. Before proceeding, ensure that you've tested your LUTs manually in the game to verify they work as intended.


## Prerequisites

1. **Install Modding Toolchain:**
   Before you start, ensure you have installed all necessary features from the Cities: Skylines II Modding Toolchain. Follow the guide here:  
   [Cities: Skylines II Modding Toolchain](https://cs2.paradoxwikis.com/Modding_Toolchain)

2. **Create LUTs:**
   If you haven't created LUTs yet, refer to the Unity documentation on authoring LUTs:  
   [Authoring LUTs in Unity HDRP](https://docs.unity3d.com/Packages/com.unity.render-pipelines.high-definition@14.0/manual/Authoring-LUTs.html)

   For a video guide on creating LUTs specifically for Lumina, watch this tutorial:  
   [Video on Creating LUTs for Lumina](https://www.youtube.com/watch?v=uAKRjDkZey4)

   This guide provides steps for using Adobe Photoshop and DaVinci Resolve.

3. **Install Visual Studio or Rider:**
   To publish your LUT pack, you'll need Visual Studio or JetBrains Rider. Download and install the necessary software from the following links:  
   [Download Visual Studio](https://visualstudio.microsoft.com/)  
   [Download JetBrains Rider](https://www.jetbrains.com/rider/)


## Publishing Your LUT Pack

### Step 1: Clone the LUT Template Project

1. **Clone the LUT Template:**
   Begin by cloning the LUT template project from GitHub:  
   [LUT Template Repository](https://github.com/NyokoDev/LUTTemplate)

2. **Open the Solution:**
   Open the `LUTTemplate.sln` file in Visual Studio.  
   ![File Explorer LUTTemplate.sln](https://imgur.com/HPtIL4u.png)

3. **Solution Explorer Structure:**
   In the Solution Explorer, you should see a structure like this:  
   ![Solution Explorer Structure](https://i.imgur.com/bfY5xOq.png)

### Step 2: Customize the Project

1. **Rename the Assembly:**
   Rename the assembly name to your desired LUT pack name. To do this, right-click on the project in the Solution Explorer and select "Rename."  
   ![Rename Assembly](https://i.imgur.com/fskjmcz.png)

2. **Create the LUTs Folder:**
   Create a new folder named `LUTS` (uppercase) within your project directory.  
   ![LUTs Folder Preview](https://i.imgur.com/Z9rU16Y.png)

3. **Add Your LUTs:**
   Drag and drop your `.cube` files (your LUTs) into the `LUTS` folder.  
   ![LUTs Folder with .cube Files](https://i.imgur.com/esnbIXy.png)

4. **Set Embedded Resource:**
   For each `.cube` file, right-click, select "Properties," and set the "Build Action" to "Embedded Resource."  
   ![Set Embedded Resource](https://i.imgur.com/9WjmtgB.png)

### Step 3: Publish Your LUT Pack

1. **Publish the Mod:**
   Right-click the project in Solution Explorer, choose "Publish," and select the `PublishNewMod` profile.  
   ![Publish Profile](https://i.imgur.com/bzdyrXj.png)

   **Note:** Ensure that you edit the publish configuration and mod metadata. You can also update the mod metadata later from the Paradox Mods website.