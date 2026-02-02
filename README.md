# AbilitySystem
AbilitySystem is a modular, extendable and editor-friendly ability framework for Unity.  
It allows developers to quickly create ability data (ScriptableObjects), generate ability logic classes, and manage abilities at runtime through a dynamic pooling system.

This package provides a clean workflow for creating, executing, canceling, and managing abilities without requiring boilerplate coding.

## Features
AbilitySystem offers the following capabilities:

### Automatic Class Generation
* Generate AbilityData classes (ScriptableObjects)
* Generate corresponding Ability logic classes
* Create ScriptableObject instances directly from the editor

### Editor Window Tools
* “Ability Creator” editor window for creating abilities with one click
* Debug-friendly logs to trace generation steps

### Runtime Ability Manager
* Loads abilities from Resources automatically
* Spawns, executes and cancels abilities at runtime
* Built-in pooling system (Spawn/Release) for efficient reuse
* Support for multiple simultaneous ability instances
* Reflection-free ability logic during runtime except for initial discovery

### Fully Testable Architecture
* Edit Mode tests for class & asset generation
* Play Mode tests for runtime execution, pooling, and cancellation logic

## Getting Started
Install via UPM with git URL

`https://github.com/Emre-Emiroglu/AbilitySystem.git`

Clone the repository
```bash
git clone https://github.com/Emre-Emiroglu/AbilitySystem.git
```
This project is developed using Unity version 6000.2.6f2.

## Usage
### 1. Creating Abilities from the Editor

Open the ability creation window:

**Tools → Ability Creator**

From this window you can:
* Generate **AbilityData** class
* Generate **Ability logic** class
* Create the **ScriptableObject** for the ability
* Automatically assign the **AbilityName** field

### 2. Writing Ability Logic

An example auto-generated ability class:

```csharp using AbilitySystem.Runtime.Data;
public sealed class FireballAbility : BaseAbility
{
    public override void Execute()
    {
        base.Execute(); // Your fireball execute logic here 
    }
}
```

### 3. Using AbilityManager at Runtime

#### Initialize the AbilityManager

Call this once (e.g., in a GameManager):

```csharp
AbilityManager.InitializeManager();
```

#### Spawn an ability instance

Creates a new instance or retrieves one from the pool:

```csharp
var ability = AbilityManager.Spawn("Fireball");
```

#### Release an ability instance (return to pool)

```csharp
AbilityManager.Release(ability);
```

#### Execute an ability instance

```csharp
AbilityManager.Execute(ability);
```

#### Execute all active abilities of a type

```csharp
AbilityManager.ExecuteAll("Fireball");

```

#### Cancel all active abilities of a type

```csharp
AbilityManager..CancelAll("Fireball");
```

## Acknowledgments
Special thanks to the Unity community for their invaluable resources and tools.

For more information, visit the GitHub repository.
