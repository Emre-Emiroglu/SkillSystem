# SkillSystem
SkillSystem is a lightweight, modular and editor-friendly skill framework for Unity.
It allows developers to quickly create skill data (ScriptableObjects), generate skill logic classes, and control skill state changes at runtime through a clean and extensible architecture.

## Features
SkillSystem offers the following capabilities:

### Automatic Class Generation
* Generate SkillData classes (ScriptableObjects)
* Generate corresponding Skill logic classes
* Create Skill ScriptableObject instances directly from the editor
* Automatically assign the SkillName field

### Editor Window Tools
* “Skill Creator” editor window for creating skills with one click
* Simple and predictable editor workflow

### Runtime Skill Manager
* Loads all SkillData assets automatically from Resources
* Dynamically creates skill logic instances at runtime
* Initializes skills on demand
* Changes skill states through a centralized manager
* Reflection is only used for initial skill discovery

### Fully Testable Architecture
* Edit Mode tests for class & asset generation
* Play Mode tests for runtime initialize and change state logic

## Getting Started
Install via UPM with git URL

`https://github.com/Emre-Emiroglu/SkillSystem.git`

Clone the repository
```bash
git clone https://github.com/Emre-Emiroglu/SkillSystem.git
```
This project is developed using Unity version 6000.2.6f2.

## Usage
### 1. Creating Skills from the Editor
Open the skill creation window: **Tools → Skill Creator**

From this window you can:
* Generate **SkillData** class
* Generate **Skill logic** class
* Create the **Skill ScriptableObject**
* Automatically assign the **SkillName** field

### 2. Writing Skill Logic
An example auto-generated skill class:

```csharp using SkillSystem.Runtime.Data;
public sealed class TestSkill :BaseSkill<TestSkillData>
{
    public override void Initialize(TestSkillData skillData)
    {
        base.Initialize(skillData);
        
        // TODO: TestSkill initialize logic here
    }

    public override void ChangeState(SkillState newSkillState)
    {
        base.ChangeState(newSkillState);
        
        // TODO: TestSkill change state logic here
    }
}
```

### 3. Using SkillManager at Runtime
#### Initialize the SkillManager
Call this once (e.g., in a GameManager):

```csharp
SkillManager.InitializeManager();
```

#### Initialize a Skill
Creates and initializes the skill instance if needed:

```csharp
SkillManager.InitializeSkill("TestSkill");
```

#### Change Skill State
Updates the runtime state of a skill:

```csharp
SkillManager.ChangeSkillState("TestSkill", SkillState.Unlocked);
```

## Acknowledgments
Special thanks to the Unity community for their invaluable resources and tools.

For more information, visit the GitHub repository.
