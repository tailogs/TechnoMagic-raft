# TechnoMagicCraft
**TechnoMagicСraft** – The game was written in Unity with the goal of exploring the capabilities of the engine and creating a voxel game.

![image](https://github.com/user-attachments/assets/1f6cd074-4b24-40a5-bf7a-cdaa79f905a1)
*Image 1. Built House*

### Description

This game offers players:

- **Voxel Exploration**: A world where players can sculpt the landscape, build, and explore.
- **First-Person Interaction**: Basic movement, jumping, and camera control.
- **Block Manipulation**: Players can destroy and place blocks to shape their environment.

### Implemented Features

- **Player Movement**: Control your character using the WASD keys, jump, and navigate the world.
- **Block Interaction**:
  - Destroy Blocks: Right-click to remove blocks within reach.
  - Place Blocks: Left-click to add blocks, using the color of the last block destroyed.
- **Camera Control**: Look around using mouse movements with adjustable sensitivity.
- **Water Dynamics**: Experience increased drag when moving through water areas.
- **World Generation**: Procedural landscape generation using Perlin noise for a natural feel.
- **Simple Tree Generation**: Add variety to the landscape with different types of trees.

### Design Highlights

- **Visuals**: The game showcases simple block structures like huts or paths, illustrating the block interaction system.
- **Environment**: A world where players can see varied heights, pits filled with water, and trees dotting the landscape.

### Future Plans

- **Magic and Tech Elements**: Implementing magical spells or tech gadgets to enhance gameplay.
- **Crafting**: Add systems for resource collection and item creation.
- **Biome Variety**: Develop diverse environments with unique challenges and aesthetics.
- **Multiplayer**: Support for playing with others.
- **NPCs and Quests**: Introduce characters and missions to enrich the gameplay experience.

### License

**GPLv3**: This license allows for the free use, modification, and distribution of the code, with the requirement that derivative works maintain the same licensing.

TechnoMagicCraft is an evolving project, perfect for those looking to contribute to or learn from open-source voxel game development.

### Project Structure Recommendations

To simplify development and maintain order in the project, consider the following:

1. **Create a Clear Folder Structure**:
   - `Assets` folder for all resources (textures, models, sounds).
   - `Scripts` folder for all scripts.
   - `Prefabs` folder for object prefabs.
   - `Scenes` folder for storing game scenes.

2. **Use Prefabs**:
   - Each object should be represented as a prefab to facilitate management and reuse.

3. **Code Documentation**:
   - Each class and method should have comments describing their functionality.

4. **Regular Code Reviews**:
   - Conduct periodic code reviews to maintain quality and cleanliness in the project.
