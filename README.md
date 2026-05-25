# QuestForge — Game State Engine

## Data structure choices

### Player inventory — `List<Item>`
A list works better because players can have duplicates (like 2 potions or 3 swords). A dictionary would overwrite items with the same key. The inventory limit is only 20 items, so looping through it is still fine

### Entity registry — `Dictionary<int, GameEntity>`
Entities are accessed by ID a lot during gameplay, so a dictionary is best for fast lookup (O(1)). IDs are unique and auto-incremented, so no collisions. Removing dead entities is also fast

### World map — `LinkedList<Zone>`
The world is basically a linear path (previous and next zones). A linked list matches this well. Moving between zones is just checking Next or Previous, and inserting a zone in between is easy once you have the node

### Event stack — `Stack<GameEvent>`
Events are handled last in, first out. The newest event shows first, which lets you layer things like dialogue before combat. You can also peek at the next event without removing it, which helps with decisions like using a potion

### Combat queue — `Queue<CombatAction>`
Combat needs to stay in order. First action in should be first shown. This keeps the combat log readable. A stack would reverse the order and make it confusing
